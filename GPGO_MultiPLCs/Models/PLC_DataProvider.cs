using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Input;
using GPGO_MultiPLCs.GP_PLCs;
using GPMVVM.Helpers;
using GPMVVM.Models;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>連接PLC並提供PLC資訊</summary>
    public sealed class PLC_DataProvider : PLC_Data, IDisposable
    {
        public void Dispose() { CTS.Dispose(); }

        public enum Status
        {
            離線 = -1,
            待命中,
            準備中,
            升溫,
            恆溫,
            降溫
        }

        private readonly IDialogService Dialog;
        private readonly TaskFactory    OneScheduler = new TaskFactory(new StaTaskScheduler(1));

        /// <summary>控制紀錄任務結束</summary>
        public CancellationTokenSource CTS;

        private bool PassTag;

        /// <summary>取消投產</summary>
        public RelayCommand CancelCheckInCommand { get; }

        /// <summary>投產</summary>
        public CommandWithResult<bool> CheckInCommand { get; }

        public RelayCommand CheckRecipeCommand_KeyIn { get; }

        public RelayCommand CheckRecipeCommand_KeyLeave { get; }

        /// <summary>前製程資訊</summary>
        public ObservableConcurrentCollection<ProductInfo> Ext_Info { get; } = new ObservableConcurrentCollection<ProductInfo>();

        /// <summary>取得是否正在紀錄溫度</summary>
        public bool IsRecording => RecordingTask?.Status == TaskStatus.Running || RecordingTask?.Status == TaskStatus.WaitingForActivation || RecordingTask?.Status == TaskStatus.WaitingToRun;

        /// <summary>紀錄的資訊</summary>
        public BaseInfo OvenInfo { get; }

        public int ProcessCounts => Ext_Info.Sum(x => x.PanelCodes.Count);

        /// <summary>生產進度</summary>
        public double Progress
        {
            get
            {
                if (!OnlineStatus || !IsRecording)
                {
                    return 0.0;
                }

                var val = (double)CurrentSegment / UsedSegmentCounts / 2;

                if (double.IsNaN(val) || double.IsInfinity(val) || val <= 0.0)
                {
                    return 0.0;
                }

                return val >= 1.0 ? 1.0 : val;
            }
        }

        /// <summary>進度狀態</summary>
        public Status ProgressStatus
        {
            get
            {
                if (!OnlineStatus)
                {
                    return Status.離線;
                }

                if (!IsRecording)
                {
                    return Status.待命中;
                }

                if (IsCooling)
                {
                    return Status.降溫;
                }

                return CurrentSegment % 2 == 0 ? CurrentSegment == 0 ? Status.準備中 : Status.恆溫 : Status.升溫;
            }
        }

        /// <summary>OP輸入的配方名稱</summary>
        public string Intput_Name
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>PLC連線狀態</summary>
        public bool OnlineStatus
        {
            get => Get<bool>();
            set
            {
                Set(value);
                NotifyPropertyChanged(nameof(Progress));
                NotifyPropertyChanged(nameof(ProgressStatus));

                EventHappened?.Invoke((EventType.Alarm, DateTime.Now, "PLC Offline!", (BitType.S, (int)PCEventCode.PC_Offline), value));
                if (IsRecording)
                {
                    AddProcessEvent(EventType.Alarm, OvenInfo.StartTime, DateTime.Now, "PLC Offline!", (BitType.S, (int)PCEventCode.PC_Offline), value);
                    CTS?.Cancel();
                }
            }
        }

        public ICollection<string> Recipe_Names
        {
            get => Get<ICollection<string>>();
            set => Set(value);
        }

        /// <summary>用來紀錄的任務，可追蹤狀態</summary>
        public Task RecordingTask
        {
            get => Get<Task>();
            set
            {
                Set(value);

                value.ContinueWith(async x =>
                                   {
                                       NotifyPropertyChanged(nameof(IsRecording));
                                       NotifyPropertyChanged(nameof(Progress));
                                       NotifyPropertyChanged(nameof(ProgressStatus));

                                       x.Dispose();

                                       var h = new int[]
                                               {
                                                   HeatingTime_1, HeatingTime_2, HeatingTime_3, HeatingTime_4,
                                                   HeatingTime_5, HeatingTime_6, HeatingTime_7, HeatingTime_8
                                               };
                                       var w = new int[]
                                               {
                                                   WarmingTime_1, WarmingTime_2, WarmingTime_3, WarmingTime_4,
                                                   WarmingTime_5, WarmingTime_6, WarmingTime_7, WarmingTime_8
                                               };

                                       var ha = new int[]
                                                {
                                                    HeatingAlarm_1, HeatingAlarm_2, HeatingAlarm_3, HeatingAlarm_4,
                                                    HeatingAlarm_5, HeatingAlarm_6, HeatingAlarm_7, HeatingAlarm_8
                                                };
                                       var wa = new int[]
                                                {
                                                    WarmingAlarm_1, WarmingAlarm_2, WarmingAlarm_3, WarmingAlarm_4,
                                                    WarmingAlarm_5, WarmingAlarm_6, WarmingAlarm_7, WarmingAlarm_8
                                                };
                                       var t = new[]
                                               {
                                                   TargetTemperature_1, TargetTemperature_2, TargetTemperature_3, TargetTemperature_4,
                                                   TargetTemperature_5, TargetTemperature_6, TargetTemperature_7, TargetTemperature_8
                                               };
                                       var s = new[]
                                               {
                                                   ThermostaticTemperature_1, ThermostaticTemperature_2, ThermostaticTemperature_3, ThermostaticTemperature_4,
                                                   ThermostaticTemperature_5, ThermostaticTemperature_6, ThermostaticTemperature_7, ThermostaticTemperature_8
                                               };
                                       Array.Resize(ref h,  UsedSegmentCounts);
                                       Array.Resize(ref w,  UsedSegmentCounts);
                                       Array.Resize(ref ha, UsedSegmentCounts);
                                       Array.Resize(ref wa, UsedSegmentCounts);
                                       Array.Resize(ref t,  UsedSegmentCounts);
                                       Array.Resize(ref s,  UsedSegmentCounts);

                                       //!結束生產，填入資料
                                       OvenInfo.EndTime                  = DateTime.Now;
                                       OvenInfo.RecipeName               = RecipeName;
                                       OvenInfo.HeatingTimes             = h.ToList();
                                       OvenInfo.WarmingTimes             = w.ToList();
                                       OvenInfo.HeatingAlarms            = ha.ToList();
                                       OvenInfo.WarmingAlarms            = wa.ToList();
                                       OvenInfo.TotalHeatingTime         = (OvenInfo.EndTime - OvenInfo.StartTime).Minutes;
                                       OvenInfo.TargetOvenTemperatures   = t.ToList();
                                       OvenInfo.ThermostaticTemperatures = s.ToList();

                                       if (RecordingFinished != null)
                                       {
                                           await RecordingFinished.Invoke((OvenInfo.Copy(), Ext_Info.ToArray(), PassTag));
                                       }

                                       //!需在引發紀錄完成後才觸發取消投產
                                       CheckInCommand.Result = false;
                                   });

                NotifyPropertyChanged(nameof(IsRecording));
                NotifyPropertyChanged(nameof(Progress));
                NotifyPropertyChanged(nameof(ProgressStatus));
            }
        }

        /// <summary>OP選擇的配方名稱</summary>
        public string Selected_Name
        {
            get => Get<string>();
            set => SetRecipe(value);
        }

        public event Action<string> AssetNumberChanged;

        public event Action<string> CancelCheckIn;

        public event Action<(EventType type, DateTime time, string note, (BitType, int) tag, bool value)> EventHappened;

        public event Func<(DataType, int)[], ValueTask<Dictionary<(DataType, int), short>>> GetPLCParameters;

        public event Func<string, PLC_Recipe> GetRecipe;

        public event Action<string> MachineCodeChanged;

        public event Action RecipeKeyInError;

        public event Action<string> RecipeUsed;

        public event Func<(BaseInfo baseInfo, ICollection<ProductInfo> productInfo, bool Pass), ValueTask> RecordingFinished;

        public event Func<Dictionary<(DataType, int), short>, ValueTask> SetPLCParameters;

        public event Func<string, ValueTask<ICollection<ProductInfo>>> WantFrontData;

        public event Func<User> GetUser;

        public void SetSelectedRecipeName(string name)
        {
            Set(name, nameof(Selected_Name));
            Intput_Name = Selected_Name;
        }

        public void AddProcessEvent(EventType type, DateTime start, DateTime addtime, string note, (BitType, int) tag, bool value)
        {
            OvenInfo.EventList.Add(new LogEvent
                                   {
                                       Type        = type,
                                       StartTime   = start,
                                       AddedTime   = addtime,
                                       Description = note,
                                       TagCode     = $"{tag.Item1}{tag.Item2}",
                                       Value       = value
                                   });
        }

        public void AddTemperatures(DateTime start, DateTime addtime, double t0, double t1, double t2, double t3, double t4, double t5, double t6, double t7, double t8)
        {
            OvenInfo.RecordTemperatures.Add(new RecordTemperatures
                                            {
                                                StartTime             = start,
                                                AddedTime             = addtime,
                                                ThermostatTemperature = t0,
                                                OvenTemperatures_1    = t1,
                                                OvenTemperatures_2    = t2,
                                                OvenTemperatures_3    = t3,
                                                OvenTemperatures_4    = t4,
                                                OvenTemperatures_5    = t5,
                                                OvenTemperatures_6    = t6,
                                                OvenTemperatures_7    = t7,
                                                OvenTemperatures_8    = t8
                                            });
        }

        /// <summary>重設PLC資料對應列表</summary>
        /// <param name="map"></param>
        public void ResetMapList(PLC_DevicesMap map)
        {
            Bit_Values.Clear();
            Data_Values.Clear();
            Recipe_Values.Clear();

            foreach (var loc in map.SignalList)
            {
                Bit_Values.Add(loc.Key, loc.Value, false);
            }

            foreach (var loc in map.DataList)
            {
                Data_Values.Add(loc.Key, loc.Value, 0);
            }

            foreach (var loc in map.RecipeList)
            {
                Recipe_Values.Add(loc.Key, loc.Value, 0);
            }
        }

        /// <summary>重設CancellationTokenSource狀態</summary>
        public void ResetStopTokenSource()
        {
            PassTag = false;

            CTS?.Dispose();

            CTS = new CancellationTokenSource();

            //!可註冊取消時須執行的動作
            //CTS.Token.Register(() =>
            //{
            //
            //});
        }

        public async void SetRecipe(string recipeName)
        {
            if (GetRecipe?.Invoke(recipeName) is PLC_Recipe recipe)
            {
                if (await Dialog.Show(new Dictionary<Language, string>
                                      {
                                          {Language.TW, "請確認配方內容："},
                                          {Language.CHS, "请确认配方内容："},
                                          {Language.EN, "Please confirm this recipe:"}
                                      }, recipe, true))
                {
                    RecipeUsed?.Invoke(recipeName);
                    recipe.CopyToObj(this);
                }

                Set(RecipeName, nameof(Selected_Name));
                Intput_Name = Selected_Name;

                if (SetPLCParameters != null)
                {
                    await SetPLCParameters.Invoke(Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key, x => x.Value));
                }
            }
        }

        /// <summary>開始記錄</summary>
        /// <param name="cycle_ms">紀錄週期，單位為毫秒</param>
        /// <param name="ct">取消任務的token</param>
        /// <returns></returns>
        public async Task StartRecoder(long cycle_ms, CancellationToken ct)
        {
            OvenInfo.StartTime = DateTime.Now;
            OvenInfo.RecordTemperatures.Clear();

            await OneScheduler.StartNew(() =>
                                        {
                                            var n                      = TimeSpan.Zero;
                                            var _ThermostatTemperature = ThermostatTemperature;
                                            var _OvenTemperature_1     = OvenTemperature_1;
                                            var _OvenTemperature_2     = OvenTemperature_2;
                                            var _OvenTemperature_3     = OvenTemperature_3;
                                            var _OvenTemperature_4     = OvenTemperature_4;
                                            var _OvenTemperature_5     = OvenTemperature_5;
                                            var _OvenTemperature_6     = OvenTemperature_6;
                                            var _OvenTemperature_7     = OvenTemperature_7;
                                            var _OvenTemperature_8     = OvenTemperature_8;

                                            while (!ct.IsCancellationRequested)
                                            {
                                                _ThermostatTemperature = ThermostatTemperature <= 0 ? _ThermostatTemperature : ThermostatTemperature;
                                                _OvenTemperature_1     = OvenTemperature_1 <= 0 ? _OvenTemperature_1 : OvenTemperature_1;
                                                _OvenTemperature_2     = OvenTemperature_2 <= 0 ? _OvenTemperature_2 : OvenTemperature_2;
                                                _OvenTemperature_3     = OvenTemperature_3 <= 0 ? _OvenTemperature_3 : OvenTemperature_3;
                                                _OvenTemperature_4     = OvenTemperature_4 <= 0 ? _OvenTemperature_4 : OvenTemperature_4;
                                                _OvenTemperature_5     = OvenTemperature_5 <= 0 ? _OvenTemperature_5 : OvenTemperature_5;
                                                _OvenTemperature_6     = OvenTemperature_6 <= 0 ? _OvenTemperature_6 : OvenTemperature_6;
                                                _OvenTemperature_7     = OvenTemperature_7 <= 0 ? _OvenTemperature_7 : OvenTemperature_7;
                                                _OvenTemperature_8     = OvenTemperature_8 <= 0 ? _OvenTemperature_8 : OvenTemperature_8;

                                                if (DateTime.Now - OvenInfo.StartTime >= n)
                                                {
                                                    AddTemperatures(OvenInfo.StartTime,
                                                                    DateTime.Now,
                                                                    _ThermostatTemperature,
                                                                    _OvenTemperature_1,
                                                                    _OvenTemperature_2,
                                                                    _OvenTemperature_3,
                                                                    _OvenTemperature_4,
                                                                    _OvenTemperature_5,
                                                                    _OvenTemperature_6,
                                                                    _OvenTemperature_7,
                                                                    _OvenTemperature_8);

                                                    if (n >= TimeSpan.FromMinutes(30))
                                                    {
                                                        n += TimeSpan.FromSeconds(30);
                                                    }
                                                    else if (n >= TimeSpan.FromMinutes(1))
                                                    {
                                                        n += TimeSpan.FromSeconds(10);
                                                    }
                                                    else
                                                    {
                                                        n += TimeSpan.FromSeconds(2);
                                                    }
                                                }
                                                else
                                                {
                                                    Thread.Sleep(15);
                                                }
                                            }

                                            AddTemperatures(OvenInfo.StartTime,
                                                            DateTime.Now,
                                                            ThermostatTemperature,
                                                            OvenTemperature_1,
                                                            OvenTemperature_2,
                                                            OvenTemperature_3,
                                                            OvenTemperature_4,
                                                            OvenTemperature_5,
                                                            OvenTemperature_6,
                                                            OvenTemperature_7,
                                                            OvenTemperature_8);
                                        },
                                        ct);
        }

        public PLC_DataProvider(PLC_DevicesMap map, IDialogService dialog)
        {
            Dialog = dialog;

            CheckRecipeCommand_KeyIn = new RelayCommand(e =>
                                                        {
                                                            if (((KeyEventArgs)e).Key != Key.Enter || Selected_Name == null)
                                                            {
                                                                return;
                                                            }

                                                            if (Selected_Name == Intput_Name)
                                                            {
                                                                Dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                {Language.TW, "配方無變更"},
                                                                                {Language.CHS, "配方无变更"},
                                                                                {Language.EN, "No change."}
                                                                            });
                                                            }
                                                            else if (Recipe_Names.Contains(Intput_Name))
                                                            {
                                                                SetRecipe(Intput_Name);
                                                            }
                                                            else
                                                            {
                                                                Intput_Name = Selected_Name;
                                                                RecipeKeyInError?.Invoke();
                                                            }
                                                        });

            CheckRecipeCommand_KeyLeave = new RelayCommand(e =>
                                                           {
                                                               Intput_Name = Selected_Name;
                                                           });

            //! 投產
            //CheckInCommand = new CommandWithResult<bool>(async o =>
            //                                             {
            //                                                 var para = (string)o;

            //                                                 var (result1, input1) =
            //                                                     await Dialog.CheckCondition(new Dictionary<Language, string>
            //                                                                                 {
            //                                                                                     {Language.TW, "輸入操作人員ID"},
            //                                                                                     {Language.CHS, "输入操作人员ID"},
            //                                                                                     {Language.EN, "Enter the Operator ID"}
            //                                                                                 },
            //                                                                                 new Dictionary<Language, string>
            //                                                                                 {
            //                                                                                     {Language.TW, para},
            //                                                                                     {Language.CHS, para},
            //                                                                                     {Language.EN, para}
            //                                                                                 },
            //                                                                                 true,
            //                                                                                 x =>
            //                                                                                 {
            //                                                                                     var str = x.ToString().Trim();

            //                                                                                     return (str.Length > 4 && str.Length < 10,
            //                                                                                             new Dictionary<Language, string>
            //                                                                                             {
            //                                                                                                 {Language.TW, "字數錯誤，請重試！"},
            //                                                                                                 {Language.CHS, "字数错误，请重试！"},
            //                                                                                                 {Language.EN, "Input error, please try again!"}
            //                                                                                             });
            //                                                                                 });

            //                                                 if (result1)
            //                                                 {
            //                                                     var (result2, input2) =
            //                                                         await Dialog.CheckCondition(new Dictionary<Language, string>
            //                                                                                     {
            //                                                                                         {Language.TW, "輸入台車碼"},
            //                                                                                         {Language.CHS, "输入台车码"},
            //                                                                                         {Language.EN, "Enter the Trolley Code"}
            //                                                                                     },
            //                                                                                     new Dictionary<Language, string>
            //                                                                                     {
            //                                                                                         {Language.TW, para},
            //                                                                                         {Language.CHS, para},
            //                                                                                         {Language.EN, para}
            //                                                                                     },
            //                                                                                     true,
            //                                                                                     x =>
            //                                                                                     {
            //                                                                                         var str = x.ToString().Trim();

            //                                                                                         return (str.Length > 4 && str.Length < 15,
            //                                                                                                 new Dictionary<Language, string>
            //                                                                                                 {
            //                                                                                                     {Language.TW, "字數錯誤，請重試！"},
            //                                                                                                     {Language.CHS, "字数错误，请重试！"},
            //                                                                                                     {Language.EN, "Input error, please try again!"}
            //                                                                                                 });
            //                                                                                     });

            //                                                     if (result2 && WantFrontData != null)
            //                                                     {
            //                                                         OvenInfo.OperatorID  = input1.ToString();
            //                                                         OvenInfo.TrolleyCode = input2.ToString();

            //                                                         //! 取得上位資訊(料號、總量、投產量)
            //                                                         var panels = await WantFrontData.Invoke(OvenInfo.TrolleyCode);
            //                                                         if (panels == null || panels.Count == 0)
            //                                                         {
            //                                                             Dialog.Show(new Dictionary<Language, string>
            //                                                                         {
            //                                                                             {Language.TW, "查無資料！"},
            //                                                                             {Language.CHS, "查无资料！"},
            //                                                                             {Language.EN, "No data found!"}
            //                                                                         },
            //                                                                         DialogMsgType.Alarm);

            //                                                             return false;
            //                                                         }

            //                                                         var (result3, intput3) =
            //                                                             await Dialog.CheckCondition(new Dictionary<Language, string>
            //                                                                                         {
            //                                                                                             {Language.TW, "輸入製程序"},
            //                                                                                             {Language.CHS, "输入制程序"},
            //                                                                                             {Language.EN, "Enter the process number"}
            //                                                                                         },
            //                                                                                         new Dictionary<Language, string>
            //                                                                                         {
            //                                                                                             {Language.TW, para},
            //                                                                                             {Language.CHS, para},
            //                                                                                             {Language.EN, para}
            //                                                                                         },
            //                                                                                         true,
            //                                                                                         x =>
            //                                                                                         {
            //                                                                                             var str = x.ToString().Trim();

            //                                                                                             return (str.Length > 0 && str.Length <= 4 && str.All(char.IsDigit),
            //                                                                                                     new Dictionary<Language, string>
            //                                                                                                     {
            //                                                                                                         {Language.TW, "字數錯誤或非整數，請重試！"},
            //                                                                                                         {Language.CHS, "字数错误或非整数，请重试！"},
            //                                                                                                         {Language.EN, "Input error, please try again!"}
            //                                                                                                     });
            //                                                                                         });

            //                                                         if (!result3 || WantFrontData == null)
            //                                                         {
            //                                                             return false;
            //                                                         }

            //                                                         Ext_Info.Clear();

            //                                                         if (int.TryParse(intput3.ToString(), out var num))
            //                                                         {
            //                                                             foreach (var panel in panels)
            //                                                             {
            //                                                                 panel.ProcessNumber = num;
            //                                                                 Ext_Info.Add(panel);
            //                                                             }
            //                                                         }
            //                                                         else
            //                                                         {
            //                                                             foreach (var panel in panels)
            //                                                             {
            //                                                                 Ext_Info.Add(panel);
            //                                                             }
            //                                                         }

            //                                                         if (!PC_InUsed &&
            //                                                             !await Dialog.Show(new Dictionary<Language, string>
            //                                                                                {
            //                                                                                    {Language.TW, "目前烤箱處於\"PC PASS\"模式，無法遠端設定配方\n確定投產嗎？"},
            //                                                                                    {Language.CHS, "目前烤箱处于\"PC PASS\"模式，无法远程设定配方\n确定投产吗？"},
            //                                                                                    {Language.EN, "The oven is in \"PC PASS\" mode, can't set recipe remotely.\nAre you sure to execute?"}
            //                                                                                },
            //                                                                                true))
            //                                                         {
            //                                                             return false;
            //                                                         }

            //                                                         if (GetRecipe?.Invoke(Selected_Name) is PLC_Recipe recipe)
            //                                                         {
            //                                                             recipe.CopyToObj(this);

            //                                                             if (SetPLCParameters != null)
            //                                                             {
            //                                                                 await SetPLCParameters.Invoke(Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key, x => x.Value));
            //                                                             }
            //                                                         }

            //                                                         return true;
            //                                                     }
            //                                                 }

            //                                                 return false;
            //                                             });


            CheckInCommand = new CommandWithResult<bool>(async o =>
                                                         {
                                                             var (result1, partNo) =
                                                                 await Dialog.CheckCondition(new Dictionary<Language, string>
                                                                                             {
                                                                                                 {Language.TW, "輸入料號"},
                                                                                                 {Language.CHS, "输入料号"},
                                                                                                 {Language.EN, "Enter the Part Number"}
                                                                                             },
                                                                                             new Dictionary<Language, string>
                                                                                             {
                                                                                                 {Language.TW, "5 ~ 14個英數字"},
                                                                                                 {Language.CHS, "5 ~ 14个英数字"},
                                                                                                 {Language.EN, "5 ~ 14 alphanumerics"}
                                                                                             },
                                                                                             true,
                                                                                             x =>
                                                                                             {
                                                                                                 var str = x.ToString().Trim();

                                                                                                 return (str.Length > 4 && str.Length < 15,
                                                                                                         new Dictionary<Language, string>
                                                                                                         {
                                                                                                             {Language.TW, "字數錯誤！"},
                                                                                                             {Language.CHS, "字数错误！"},
                                                                                                             {Language.EN, "Input error!"}
                                                                                                         });
                                                                                             });

                                                             if (!result1)
                                                             {
                                                                 return false;
                                                             }

                                                             var (result2, batchNo) =
                                                                 await Dialog.CheckCondition(new Dictionary<Language, string>
                                                                                             {
                                                                                                 {Language.TW, "輸入批號"},
                                                                                                 {Language.CHS, "输入批号"},
                                                                                                 {Language.EN, "Enter the Batch Number"}
                                                                                             },
                                                                                             new Dictionary<Language, string>
                                                                                             {
                                                                                                 {Language.TW, "5 ~ 14個英數字"},
                                                                                                 {Language.CHS, "5 ~ 14个英数字"},
                                                                                                 {Language.EN, "5 ~ 14 alphanumerics"}
                                                                                             },
                                                                                             true,
                                                                                             x =>
                                                                                             {
                                                                                                 var str = x.ToString().Trim();

                                                                                                 return (str.Length > 4 && str.Length < 15,
                                                                                                         new Dictionary<Language, string>
                                                                                                         {
                                                                                                             {Language.TW, "字數錯誤！"},
                                                                                                             {Language.CHS, "字数错误！"},
                                                                                                             {Language.EN, "Input error!"}
                                                                                                         });
                                                                                             });

                                                             if (!result2)
                                                             {
                                                                 return false;
                                                             }

                                                             var counts = 0;
                                                             var (result4, input4) =
                                                                 await Dialog.CheckCondition(new Dictionary<Language, string>
                                                                                             {
                                                                                                 {Language.TW, "輸入數量"},
                                                                                                 {Language.CHS, "输入数量"},
                                                                                                 {Language.EN, "Enter the quantity"}
                                                                                             },
                                                                                             new Dictionary<Language, string>
                                                                                             {
                                                                                                 {Language.TW, "1 ~ 100"},
                                                                                                 {Language.CHS, "1 ~ 100"},
                                                                                                 {Language.EN, "1 ~ 100"}
                                                                                             },
                                                                                             true,
                                                                                             x =>
                                                                                             {
                                                                                                 var str = x.ToString().Trim();

                                                                                                 return (int.TryParse(str, out counts) && counts > 0 && counts <= 100,
                                                                                                         new Dictionary<Language, string>
                                                                                                         {
                                                                                                             {Language.TW, "數量錯誤！"},
                                                                                                             {Language.CHS, "数量错误！"},
                                                                                                             {Language.EN, "Wrong quantity!"}
                                                                                                         });
                                                                                             });

                                                             if (!result4)
                                                             {
                                                                 return false;
                                                             }

                                                             if (GetUser != null)
                                                             {
                                                                 OvenInfo.OperatorID = GetUser().Name;
                                                             }

                                                             Ext_Info.Clear();

                                                             for (var i = 0; i < counts; i++)
                                                             {
                                                                 Ext_Info.Add(new ProductInfo
                                                                              {
                                                                                  PartNumber  = partNo.ToString().Trim(),
                                                                                  BatchNumber = batchNo.ToString().Trim()
                                                                              });
                                                             }

                                                             if (!PC_InUsed &&
                                                                 !await Dialog.Show(new Dictionary<Language, string>
                                                                                    {
                                                                                        {Language.TW, "目前烤箱處於\"PC PASS\"模式，無法遠端設定配方\n確定投產嗎？"},
                                                                                        {Language.CHS, "目前烤箱处于\"PC PASS\"模式，无法远程设定配方\n确定投产吗？"},
                                                                                        {Language.EN, "The oven is in \"PC PASS\" mode, can't set recipe remotely.\nAre you sure to execute?"}
                                                                                    },
                                                                                    true))
                                                             {
                                                                 return false;
                                                             }

                                                             if (GetRecipe?.Invoke(Selected_Name) is PLC_Recipe recipe)
                                                             {
                                                                 recipe.CopyToObj(this);

                                                                 if (SetPLCParameters != null)
                                                                 {
                                                                     await SetPLCParameters.Invoke(Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key, x => x.Value));
                                                                 }
                                                             }

                                                             return true;
                                                         });

            CancelCheckInCommand = new RelayCommand(async e =>
                                                    {
                                                        if (RecordingTask != null && IsRecording)
                                                        {
                                                            CTS?.Cancel();

                                                            await RecordingTask;
                                                        }

                                                        CancelCheckIn?.Invoke(OvenInfo.TrolleyCode);
                                                        OvenInfo.Clear();
                                                        Ext_Info.Clear();
                                                    });

            OvenInfo = new BaseInfo();
            OvenInfo.PropertyChanged += (s, e) =>
                                        {
                                            //!在機台編號或財產編號變更時需通知儲存
                                            if (e.PropertyName == nameof(BaseInfo.MachineCode))
                                            {
                                                MachineCodeChanged?.Invoke((s as BaseInfo)?.MachineCode);
                                            }
                                            else if (e.PropertyName == nameof(BaseInfo.AssetNumber))
                                            {
                                                AssetNumberChanged?.Invoke((s as BaseInfo)?.AssetNumber);
                                            }
                                        };

            Bit_Values    = new TwoKeyDictionary<SignalNames, (BitType, int), bool>();
            Data_Values   = new TwoKeyDictionary<DataNames, (DataType, int), short>();
            Recipe_Values = new TwoKeyDictionary<DataNames, (DataType, int), short>();

            foreach (var loc in map.SignalList)
            {
                Bit_Values.Add(loc.Key, loc.Value, false);
            }

            foreach (var loc in map.DataList)
            {
                Data_Values.Add(loc.Key, loc.Value, 0);
            }

            foreach (var loc in map.RecipeList)
            {
                Recipe_Values.Add(loc.Key, loc.Value, 0);
            }

            #region 將PLC掃描值和ViewModel上的Property做map連結

            var Bit_Map = new Dictionary<SignalNames, string>
                          {
                              {SignalNames.蜂鳴器, nameof(Buzzer)},
                              {SignalNames.綠燈, nameof(GreenLight)},
                              {SignalNames.黃燈, nameof(YellowLight)},
                              {SignalNames.紅燈, nameof(RedLight)},
                              {SignalNames.PC_InUsed, nameof(PC_InUsed)},
                              {SignalNames.自動模式, nameof(AutoMode)},
                              {SignalNames.自動啟動, nameof(AutoMode_Start)},
                              {SignalNames.自動停止, nameof(AutoMode_Stop)},
                              {SignalNames.手動模式, nameof(ManualMode)},
                              {SignalNames.降溫中, nameof(IsCooling)},
                              {SignalNames.程式結束, nameof(ProgramStop)},
                              {SignalNames.加熱門未關, nameof(DoorNotClosed)},
                              {SignalNames.緊急停止, nameof(EmergencyStop)},
                              {SignalNames.溫控器低溫異常, nameof(LowTemperature)},
                              {SignalNames.電源反相, nameof(PowerInversion)},
                              {SignalNames.OTP超溫異常, nameof(OTP_TemperatureError)},
                              {SignalNames.循環風車過載, nameof(CirculatingFanOverload)},
                              {SignalNames.循環風車電流異常, nameof(CirculatingFanCurrentException)},
                              {SignalNames.冷卻進氣風車異常, nameof(CoolingFanException)},
                              {SignalNames.冷卻進氣風車電流異常, nameof(CoolingFanCurrentException)},
                              {SignalNames.超溫警報, nameof(OverTemperatureAlarm)},
                              {SignalNames.停止後未開門, nameof(DoorNotOpen)},
                              {SignalNames.循環風車INV異常, nameof(CirculatingFanInversion)},
                              {SignalNames.充氮氣逾時, nameof(InflatingTimeExceeded)},
                              {SignalNames.門未關定位異常, nameof(DoorNotClosed_PositionException)},
                              {SignalNames.升恆溫逾時, nameof(HeatingTimeExceeded)},
                              {SignalNames.加熱分路跳脫, nameof(HeatingBranchException)},
                              {SignalNames.溫控器異常, nameof(ThermostatException)},
                              {SignalNames.通訊異常, nameof(CommunicationException)},
                              {SignalNames.寫入溫度比對異常, nameof(TemperatureWriteError)}
                          };
            var Data_Map = new Dictionary<DataNames, string>
                           {
                               {DataNames.溫控器實際溫度, nameof(ThermostatTemperature)},
                               {DataNames.溫控器設定溫度, nameof(ThermostatTemperatureSet)},
                               {DataNames.片段剩餘時間, nameof(Segment_RemainingTime)},
                               {DataNames.總剩餘時間, nameof(Total_RemainingTime)},
                               {DataNames.目前段數, nameof(CurrentSegment)},
                               {DataNames.爐內溫度_1, nameof(OvenTemperature_1)},
                               {DataNames.爐內溫度_2, nameof(OvenTemperature_2)},
                               {DataNames.爐內溫度_3, nameof(OvenTemperature_3)},
                               {DataNames.爐內溫度_4, nameof(OvenTemperature_4)},
                               {DataNames.爐內溫度_5, nameof(OvenTemperature_5)},
                               {DataNames.爐內溫度_6, nameof(OvenTemperature_6)},
                               {DataNames.爐內溫度_7, nameof(OvenTemperature_7)},
                               {DataNames.爐內溫度_8, nameof(OvenTemperature_8)},
                               {DataNames.目標溫度_1, nameof(TargetTemperature_1)},
                               {DataNames.升溫時間_1, nameof(HeatingTime_1)},
                               {DataNames.升溫警報_1, nameof(HeatingAlarm_1)},
                               {DataNames.恆溫溫度_1, nameof(ThermostaticTemperature_1)},
                               {DataNames.恆溫時間_1, nameof(WarmingTime_1)},
                               {DataNames.恆溫警報_1, nameof(WarmingAlarm_1)},
                               {DataNames.目標溫度_2, nameof(TargetTemperature_1)},
                               {DataNames.升溫時間_2, nameof(HeatingTime_1)},
                               {DataNames.升溫警報_2, nameof(HeatingAlarm_1)},
                               {DataNames.恆溫溫度_2, nameof(ThermostaticTemperature_1)},
                               {DataNames.恆溫時間_2, nameof(WarmingTime_1)},
                               {DataNames.恆溫警報_2, nameof(WarmingAlarm_1)},
                               {DataNames.目標溫度_3, nameof(TargetTemperature_1)},
                               {DataNames.升溫時間_3, nameof(HeatingTime_1)},
                               {DataNames.升溫警報_3, nameof(HeatingAlarm_1)},
                               {DataNames.恆溫溫度_3, nameof(ThermostaticTemperature_1)},
                               {DataNames.恆溫時間_3, nameof(WarmingTime_1)},
                               {DataNames.恆溫警報_3, nameof(WarmingAlarm_1)},
                               {DataNames.目標溫度_4, nameof(TargetTemperature_1)},
                               {DataNames.升溫時間_4, nameof(HeatingTime_1)},
                               {DataNames.升溫警報_4, nameof(HeatingAlarm_1)},
                               {DataNames.恆溫溫度_4, nameof(ThermostaticTemperature_1)},
                               {DataNames.恆溫時間_4, nameof(WarmingTime_1)},
                               {DataNames.恆溫警報_4, nameof(WarmingAlarm_1)},
                               {DataNames.目標溫度_5, nameof(TargetTemperature_1)},
                               {DataNames.升溫時間_5, nameof(HeatingTime_1)},
                               {DataNames.升溫警報_5, nameof(HeatingAlarm_1)},
                               {DataNames.恆溫溫度_5, nameof(ThermostaticTemperature_1)},
                               {DataNames.恆溫時間_5, nameof(WarmingTime_1)},
                               {DataNames.恆溫警報_5, nameof(WarmingAlarm_1)},
                               {DataNames.目標溫度_6, nameof(TargetTemperature_1)},
                               {DataNames.升溫時間_6, nameof(HeatingTime_1)},
                               {DataNames.升溫警報_6, nameof(HeatingAlarm_1)},
                               {DataNames.恆溫溫度_6, nameof(ThermostaticTemperature_1)},
                               {DataNames.恆溫時間_6, nameof(WarmingTime_1)},
                               {DataNames.恆溫警報_6, nameof(WarmingAlarm_1)},
                               {DataNames.目標溫度_7, nameof(TargetTemperature_1)},
                               {DataNames.升溫時間_7, nameof(HeatingTime_1)},
                               {DataNames.升溫警報_7, nameof(HeatingAlarm_1)},
                               {DataNames.恆溫溫度_7, nameof(ThermostaticTemperature_1)},
                               {DataNames.恆溫時間_7, nameof(WarmingTime_1)},
                               {DataNames.恆溫警報_7, nameof(WarmingAlarm_1)},
                               {DataNames.目標溫度_8, nameof(TargetTemperature_1)},
                               {DataNames.升溫時間_8, nameof(HeatingTime_1)},
                               {DataNames.升溫警報_8, nameof(HeatingAlarm_1)},
                               {DataNames.恆溫溫度_8, nameof(ThermostaticTemperature_1)},
                               {DataNames.恆溫時間_8, nameof(WarmingTime_1)},
                               {DataNames.恆溫警報_8, nameof(WarmingAlarm_1)},
                               {DataNames.降溫溫度, nameof(CoolingTemperature)},
                               {DataNames.降溫時間, nameof(CoolingTime)},
                               {DataNames.充氣時間, nameof(InflatingTime)},
                               {DataNames.使用段數, nameof(UsedSegmentCounts)},
                               {DataNames.程式結束警報時間, nameof(ProgramStopAlarmTime)},
                               {DataNames.配方名稱_01, nameof(RecipeName)},
                               {DataNames.配方名稱_02, nameof(RecipeName)},
                               {DataNames.配方名稱_03, nameof(RecipeName)},
                               {DataNames.配方名稱_04, nameof(RecipeName)},
                               {DataNames.配方名稱_05, nameof(RecipeName)},
                               {DataNames.配方名稱_06, nameof(RecipeName)},
                               {DataNames.配方名稱_07, nameof(RecipeName)},
                               {DataNames.配方名稱_08, nameof(RecipeName)},
                               {DataNames.配方名稱_09, nameof(RecipeName)},
                               {DataNames.配方名稱_10, nameof(RecipeName)},
                               {DataNames.配方名稱_11, nameof(RecipeName)},
                               {DataNames.配方名稱_12, nameof(RecipeName)},
                               {DataNames.配方名稱_13, nameof(RecipeName)},
                               {DataNames.配方名稱_14, nameof(RecipeName)},
                               {DataNames.配方名稱_15, nameof(RecipeName)},
                               {DataNames.配方名稱_16, nameof(RecipeName)},
                               {DataNames.配方名稱_17, nameof(RecipeName)},
                               {DataNames.配方名稱_18, nameof(RecipeName)},
                               {DataNames.配方名稱_19, nameof(RecipeName)},
                               {DataNames.配方名稱_20, nameof(RecipeName)}
                           };

            #endregion 將PLC掃描值和ViewModel上的Property做map連結

            #region 註冊PLC事件

            Bit_Values.UpdatedEvent += async (key1, key2, value) =>
                                       {
                                           NotifyPropertyChanged(Bit_Map[key1]);
                                           var nt = DateTime.Now;

                                           if (key1 == SignalNames.自動啟動)
                                           {
                                               EventHappened?.Invoke((EventType.Trigger, nt, key1.ToString(), key2, value));

                                               if (!value)
                                               {
                                                   return;
                                               }

                                               if (RecordingTask != null && IsRecording)
                                               {
                                                   CTS?.Cancel();

                                                   await RecordingTask;
                                               }

                                               ResetStopTokenSource();

                                               //todo 讀取配方實際值，實際位置為寫入位置-100(需確認)
                                               if (GetPLCParameters != null &&
                                                   await GetPLCParameters.Invoke(map.RecipeList.Where(x => x.Value.Item2 >= 0).Select(x => (x.Value.Item1, x.Value.Item2)).ToArray()) is Dictionary<(DataType, int), short> recipe)
                                               {
                                                   foreach (var val in recipe)
                                                   {
                                                       Recipe_Values[(val.Key.Item1, val.Key.Item2)] = val.Value;
                                                   }
                                               }

                                               RecordingTask = StartRecoder(60000, CTS.Token);
                                           }
                                           else if (IsRecording)
                                           {
                                               if (key1 == SignalNames.程式結束)
                                               {
                                                   PassTag = true;

                                                   EventHappened?.Invoke((EventType.Trigger, nt, key1.ToString(), key2, value));
                                                   AddProcessEvent(EventType.Trigger, OvenInfo.StartTime, nt, key1.ToString(), key2, value);

                                                   if (!value)
                                                   {
                                                       return;
                                                   }

                                                   CTS?.Cancel();
                                               }
                                               else if (key1 == SignalNames.自動停止)
                                               {
                                                   EventHappened?.Invoke((EventType.Trigger, nt, key1.ToString(), key2, value));
                                                   AddProcessEvent(EventType.Trigger, OvenInfo.StartTime, nt, key1.ToString(), key2, value);

                                                   if (!value)
                                                   {
                                                       return;
                                                   }

                                                   CTS?.Cancel();
                                               }
                                               else if (key1 == SignalNames.緊急停止 || key1 == SignalNames.循環風車過載 || key1 == SignalNames.循環風車INV異常)
                                               {
                                                   EventHappened?.Invoke((EventType.Alarm, nt, key1.ToString(), key2, value));
                                                   AddProcessEvent(EventType.Alarm, OvenInfo.StartTime, nt, key1.ToString(), key2, value);

                                                   if (!value)
                                                   {
                                                       return;
                                                   }

                                                   CTS?.Cancel();
                                               }
                                               else if (key1 == SignalNames.降溫中)
                                               {
                                                   if (IsCooling)
                                                   {
                                                       PassTag = true;
                                                   }

                                                   EventHappened?.Invoke((EventType.Trigger, nt, key1.ToString(), key2, value));
                                                   AddProcessEvent(EventType.Trigger, OvenInfo.StartTime, nt, key1.ToString(), key2, value);
                                                   NotifyPropertyChanged(nameof(Progress));
                                                   NotifyPropertyChanged(nameof(ProgressStatus));
                                               }
                                               else if (key1 == SignalNames.電源反相 ||
                                                        key1 == SignalNames.OTP超溫異常 ||
                                                        key1 == SignalNames.充氮氣逾時 ||
                                                        key1 == SignalNames.冷卻進氣風車異常 ||
                                                        key1 == SignalNames.溫控器低溫異常 ||
                                                        key1 == SignalNames.超溫警報 ||
                                                        key1 == SignalNames.升恆溫逾時 ||
                                                        key1 == SignalNames.加熱門未關 ||
                                                        key1 == SignalNames.門未關定位異常)
                                               {
                                                   EventHappened?.Invoke((EventType.Alarm, nt, key1.ToString(), key2, value));
                                                   AddProcessEvent(EventType.Alarm, OvenInfo.StartTime, nt, key1.ToString(), key2, value);
                                               }
                                           }
                                       };

            Data_Values.UpdatedEvent += (key1, key2, value) =>
                                        {
                                            NotifyPropertyChanged(Data_Map[key1]);
                                            var nt = DateTime.Now;

                                            if (key1 == DataNames.目前段數)
                                            {
                                                if (IsRecording)
                                                {
                                                    AddProcessEvent(EventType.Normal,
                                                                    OvenInfo.StartTime,
                                                                    nt,
                                                                    CurrentSegment == 0 ? "準備中" : $"第{Math.Ceiling(CurrentSegment / 2.0):0}段{(CurrentSegment % 2 == 0 ? "恆溫" : "升溫")}",
                                                                    (BitType.S, (int)PCEventCode.段數切換),
                                                                    true);
                                                }

                                                NotifyPropertyChanged(nameof(Progress));
                                                NotifyPropertyChanged(nameof(ProgressStatus));
                                            }
                                        };

            Recipe_Values.UpdatedEvent += (key1, key2, value) =>
                                          {
                                              NotifyPropertyChanged(Data_Map[key1]);

                                              if (key1.ToString().Contains("配方名稱"))
                                              {
                                                  Set(RecipeName, nameof(Selected_Name));
                                                  Intput_Name = Selected_Name;
                                              }
                                              else if (key1 == DataNames.使用段數)
                                              {
                                                  NotifyPropertyChanged(nameof(Progress));
                                                  NotifyPropertyChanged(nameof(ProgressStatus));
                                              }
                                          };

            Ext_Info.CollectionChanged += (s, e) =>
                                          {
                                              NotifyPropertyChanged(nameof(ProcessCounts));
                                          };

            #endregion 註冊PLC事件
        }
    }
}