using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PLCService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>連接PLC並提供PLC資訊</summary>
    public sealed class PLC_DataProvider : GOL_DataModel, IDisposable
    {
        public void Dispose() { CTS.Dispose(); }

        public enum Status
        {
            離線 = -1,
            未知,
            運轉中 = 1,
            待命 = 2,
            準備中,
            維修 = 8,
            停止 = 16,
            錯誤 = 4
        }

        private readonly IDialogService Dialog;
        private readonly TaskFactory    OneScheduler = new TaskFactory(new StaTaskScheduler(1));

        /// <summary>控制紀錄任務結束</summary>
        public CancellationTokenSource CTS;

        /// <summary>取消投產</summary>
        public RelayCommand CancelCheckInCommand { get; }

        /// <summary>投產</summary>
        public CommandWithResult<bool> CheckInCommand { get; }

        public RelayCommand CheckRecipeCommand_KeyIn { get; }

        public RelayCommand CheckRecipeCommand_KeyLeave { get; }

        /// <summary>前製程資訊</summary>
        public ObservableConcurrentCollection<ProductInfo> Ext_Info { get; } = new ObservableConcurrentCollection<ProductInfo>();

        /// <summary>取得是否正在紀錄溫度</summary>
        public bool IsExecuting => ExecutingTask?.Status == TaskStatus.Running || ExecutingTask?.Status == TaskStatus.WaitingForActivation || ExecutingTask?.Status == TaskStatus.WaitingToRun;

        /// <summary>紀錄的資訊</summary>
        public BaseInfo OvenInfo { get; }

        public int Quantity => Ext_Info.Sum(x => x.PanelCodes.Count);

        /// <summary>生產進度</summary>
        public double Progress
        {
            get
            {
                if (!OnlineStatus || !IsExecuting)
                {
                    return 0.0;
                }

                var val = (double)CurrentSegment / UsedSegmentCounts;

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

                if (Enum.IsDefined(typeof(Status), EquipmentStatus))
                {
                    return (Status)EquipmentStatus;
                }

                return Status.未知;
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

                EventHappened?.Invoke((EventType.Alarm, DateTime.Now, "PLC Offline!", string.Empty, value));
                if (IsExecuting)
                {
                    AddProcessEvent((EventType.Alarm, DateTime.Now, "PLC Offline!", string.Empty, value));
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
        public Task ExecutingTask
        {
            get => Get<Task>();
            set
            {
                Set(value);

                value.ContinueWith(async x =>
                                   {
                                       NotifyPropertyChanged(nameof(IsExecuting));

                                       x.Dispose();

                                       var h = new[]
                                               {
                                                   HeatingTime_1, HeatingTime_2, HeatingTime_3, HeatingTime_4,
                                                   HeatingTime_5, HeatingTime_6, HeatingTime_7, HeatingTime_8
                                               };
                                       var w = new[]
                                               {
                                                   WarmingTime_1, WarmingTime_2, WarmingTime_3, WarmingTime_4,
                                                   WarmingTime_5, WarmingTime_6, WarmingTime_7, WarmingTime_8
                                               };

                                       var ha = new[]
                                                {
                                                    HeatingAlarm_1, HeatingAlarm_2, HeatingAlarm_3, HeatingAlarm_4,
                                                    HeatingAlarm_5, HeatingAlarm_6, HeatingAlarm_7, HeatingAlarm_8
                                                };
                                       var wa = new[]
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
                                       OvenInfo.Recipe                   = this.ObjCopy<PLC_Recipe>().ToDictionary(GetLanguage?.Invoke() ?? Language.TW);
                                       OvenInfo.HeatingTimes             = h.ToList();
                                       OvenInfo.WarmingTimes             = w.ToList();
                                       OvenInfo.HeatingAlarms            = ha.ToList();
                                       OvenInfo.WarmingAlarms            = wa.ToList();
                                       OvenInfo.TotalHeatingTime         = (OvenInfo.EndTime - OvenInfo.StartTime).Minutes;
                                       OvenInfo.TargetOvenTemperatures   = t.ToList();
                                       OvenInfo.ThermostaticTemperatures = s.ToList();

                                       if (ExecutingFinished != null)
                                       {
                                           await ExecutingFinished.Invoke((OvenInfo.Copy(), Ext_Info.ToArray()));
                                       }

                                       //!需在引發紀錄完成後才觸發取消投產
                                       CheckInCommand.Result = false;
                                   });

                NotifyPropertyChanged(nameof(IsExecuting));
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

        public event Action<(EventType type, DateTime time, string note, string tag, object value)> EventHappened;

        public event Func<string, PLC_Recipe> GetRecipe;

        public event Action<string> MachineCodeChanged;

        public event Action RecipeKeyInError;

        public event Action<string> RecipeUsed;

        public event Func<Language> GetLanguage;

        public event Func<(BaseInfo baseInfo, ICollection<ProductInfo> productInfo), ValueTask> ExecutingFinished;

        public event Func<(BitType, int), bool, ValueTask> SetPLCSignal;

        public event Func<string, ValueTask<ICollection<ProductInfo>>> WantFrontData;

        public event Func<User> GetUser;

        public void SetSelectedRecipeName(string name)
        {
            Set(name, nameof(Selected_Name));
            Intput_Name = Selected_Name;
        }

        public void AddProcessEvent((EventType type, DateTime addtime, string note, string tag, object value) e)
        {
            var (type, addtime, note, tag, value) = e;
            OvenInfo.EventList.Add(new LogEvent
                                   {
                                       Type        = type,
                                       StartTime   = OvenInfo.StartTime,
                                       AddedTime   = addtime,
                                       Description = note,
                                       TagCode     = tag,
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

        /// <summary>重設CancellationTokenSource狀態</summary>
        public void ResetStopTokenSource()
        {
            CTS?.Dispose();

            CTS = new CancellationTokenSource();

            //!可註冊取消時須執行的動作
            //CTS.Token.Register(() =>
            //{
            //
            //});
        }

        /// <summary>
        /// 自資料庫取得配方
        /// </summary>
        /// <param name="recipeName"></param>
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

                Set(recipeName, nameof(Selected_Name));
                Intput_Name = Selected_Name;
            }
        }

        /// <summary>開始記錄</summary>
        /// <param name="cycle_ms">紀錄週期，單位為毫秒</param>
        /// <param name="ct">取消任務的token</param>
        /// <returns></returns>
        private async Task StartRecoder(long cycle_ms, CancellationToken ct)
        {
            OvenInfo.IsFinished = false;
            OvenInfo.EventList.Clear();
            OvenInfo.RecordTemperatures.Clear();
            OvenInfo.StartTime = DateTime.Now;

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

        public async Task StartPP()
        {
            await StopPP();

            ResetStopTokenSource();
            ExecutingTask = StartRecoder(60000, CTS.Token);
        }

        public async Task StopPP()
        {
            if (ExecutingTask != null && IsExecuting)
            {
                CTS?.Cancel();

                await ExecutingTask;
            }
        }

        public PLC_DataProvider(IDialogService dialog)
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

            CheckInCommand = new CommandWithResult<bool>(async o =>
                                                         {
                                                             var (result0, opId) =
                                                                 await Dialog.CheckCondition(new Dictionary<Language, string>
                                                                                             {
                                                                                                 {Language.TW, "輸入人員ID"},
                                                                                                 {Language.CHS, "输入人员ID"},
                                                                                                 {Language.EN, "Enter the Operator's Id"}
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

                                                             if (!result0)
                                                             {
                                                                 return false;
                                                             }

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

                                                             var batches = new Dictionary<string, int>();

                                                             do
                                                             {
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
                                                                     if (batches.Count == 0)
                                                                     {
                                                                         return false;
                                                                     }

                                                                     continue;
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

                                                                 if (!result4 && batches.Count == 0)
                                                                 {
                                                                     return false;
                                                                 }

                                                                 batches[batchNo.ToString().Trim()] = counts;
                                                             } while (await dialog.Show(new Dictionary<Language, string>
                                                                                        {
                                                                                            {Language.TW, "是否要繼續新增批號？"},
                                                                                            {Language.CHS, "是否要继续新增批号？"},
                                                                                            {Language.EN, "Continue to add batch number?"}
                                                                                        }, batches, true));

                                                             OvenInfo.OperatorID = opId.ToString().Trim();

                                                             Ext_Info.Clear();
                                                             foreach (var batch in batches)
                                                             {
                                                                 var info = new ProductInfo
                                                                            {
                                                                                PartNumber  = partNo.ToString().Trim(),
                                                                                BatchNumber = batch.Key.Trim()
                                                                            };
                                                                 for (var i = 1; i <= batch.Value; i++)
                                                                 {
                                                                     info.PanelCodes.Add($"{info.PartNumber}-{info.BatchNumber}-{i}");
                                                                 }

                                                                 Ext_Info.Add(info);
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
                                                                 OvenInfo.Recipe = this.ObjCopy<PLC_Recipe>().ToDictionary(GetLanguage?.Invoke() ?? Language.TW);
                                                             }

                                                             return true;
                                                         });

            CancelCheckInCommand = new RelayCommand(async e =>
                                                    {
                                                        if (ExecutingTask != null && IsExecuting)
                                                        {
                                                            CTS?.Cancel();

                                                            await ExecutingTask;
                                                        }

                                                        CancelCheckIn?.Invoke(OvenInfo.RackID);
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

            #region 註冊PLC事件

            ValueChanged += async (LogType, data) =>
                            {
                                var nowtime = DateTime.Now;

                                if (LogType == LogType.Status)
                                {
                                    var eventval = (EventType.StatusChanged, nowtime, data.Name, $"{(DataType)data.TypeEnum}{data.Subscriptions.First()}", data.Value);
                                    EventHappened?.Invoke(eventval);
                                    if (IsExecuting)
                                    {
                                        AddProcessEvent(eventval);
                                    }

                                    if (data.Value is bool value)
                                    {
                                        if (data.Name == nameof(AutoMode_Start))
                                        {
                                            if (!value)
                                            {
                                                return;
                                            }

                                            await StopPP();

                                            await StartPP();
                                        }
                                        else if (data.Name == nameof(ProgramStop))
                                        {
                                            if (!value)
                                            {
                                                return;
                                            }

                                            await StopPP();
                                        }
                                        else if (data.Name == nameof(AutoMode_Stop))
                                        {
                                            OvenInfo.IsFinished = true;

                                            if (!value)
                                            {
                                                return;
                                            }

                                            await StopPP();
                                        }
                                    }
                                    else if (data.Name == nameof(CurrentSegment))
                                    {
                                        NotifyPropertyChanged(nameof(Progress));
                                    }
                                    else if (data.Name == nameof(EquipmentStatus))
                                    {
                                        NotifyPropertyChanged(nameof(ProgressStatus));
                                    }
                                }
                                else if (LogType == LogType.Alarm)
                                {
                                    var eventval = (EventType.Alarm, nowtime, data.Name, $"{(BitType)data.TypeEnum}{data.Subscriptions.First()}", data.Value);
                                    EventHappened?.Invoke(eventval);
                                    if (IsExecuting)
                                    {
                                        AddProcessEvent(eventval);
                                    }

                                    if (data.Value is bool val && val && data.Name == nameof(EmergencyStop))
                                    {
                                        await StopPP();
                                    }
                                }
                                else if (LogType == LogType.Recipe)
                                {
                                    if (data.Name == nameof(UsedSegmentCounts))
                                    {
                                        NotifyPropertyChanged(nameof(Progress));
                                        NotifyPropertyChanged(nameof(ProgressStatus));
                                    }
                                }
                                else if (LogType == LogType.Trigger)
                                {
                                    if (data.Value is bool value)
                                    {
                                        var eventval = (EventType.Trigger, nowtime, data.Name, $"{(BitType)data.TypeEnum}{data.Subscriptions.First()}", value);

                                        if (data.Name == nameof(RemoteCommandStart))
                                        {
                                            EventHappened?.Invoke(eventval);

                                            //todo
                                        }
                                        else if (data.Name == nameof(RemoteCommandStop))
                                        {
                                            EventHappened?.Invoke(eventval);

                                            //todo
                                        }
                                    }   
                                }
                            };

            Ext_Info.CollectionChanged += (s, e) =>
                                          {
                                              NotifyPropertyChanged(nameof(Quantity));
                                          };

            #endregion 註冊PLC事件
        }
    }
}