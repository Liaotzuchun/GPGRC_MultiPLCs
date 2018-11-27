using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>連接PLC並提供PLC資訊</summary>
    public sealed class PLC_DataProvider : PLC_Data, IDisposable
    {
        public void Dispose()
        {
            CTS.Dispose();
        }

        public enum Status
        {
            離線 = -1,
            待命中,
            準備中,
            升溫,
            恆溫,
            降溫
        }

        /// <summary>控制紀錄任務結束</summary>
        public CancellationTokenSource CTS;

        private readonly IDialogService Dialog;
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

                EventHappened?.Invoke((EventType.Alarm, DateTime.Now, "PLC Offline!", (int)PCEventCode.PC_Offline, value));
                if (IsRecording)
                {
                    AddProcessEvent(EventType.Alarm, OvenInfo.StartTime, DateTime.Now, "PLC Offline!", (int)PCEventCode.PC_Offline, value);
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

                                       //!結束生產，填入資料
                                       OvenInfo.EndTime = DateTime.Now;
                                       OvenInfo.RecipeName = RecipeName;
                                       OvenInfo.HeatingTime = HeatingTime_1;
                                       OvenInfo.WarmingTime = WarmingTime_1;
                                       OvenInfo.TotalHeatingTime = (OvenInfo.EndTime - OvenInfo.StartTime).Minutes;
                                       OvenInfo.TargetOvenTemperature = TargetTemperature_1;

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
        public event Action<(EventType type, DateTime time, string note, int tag, bool value)> EventHappened;
        public event Func<ValueTask<Dictionary<int, short>>> GetPLCRecipeParameter;
        public event Func<string, ValueTask<PLC_Recipe>> GetRecipeEvent;
        public event Action<string> MachineCodeChanged;
        public event Action RecipeKeyInError;
        public event Func<(BaseInfo baseInfo, ICollection<ProductInfo> productInfo, bool Pass), ValueTask> RecordingFinished;
        public event Func<Dictionary<int, short>, ValueTask> SetPLCRecipeParameter;
        public event Func<(string TrolleyCode, string OrderCode), ValueTask<List<string>>> WantFrontData;

        public void AddProcessEvent(EventType type, DateTime start, DateTime addtime, string note, int tag, bool value)
        {
            OvenInfo.EventList.Add(new LogEvent
                                   {
                                       Type = type,
                                       StartTime = start,
                                       AddedTime = addtime,
                                       Description = note,
                                       TagCode = tag,
                                       Value = value
                                   });
        }

        public void AddTemperatures(DateTime start, DateTime addtime, double t0, double t1, double t2, double t3, double t4, double t5, double t6, double t7, double t8)
        {
            OvenInfo.RecordTemperatures.Add(new RecordTemperatures
                                            {
                                                StartTime = start,
                                                AddedTime = addtime,
                                                ThermostatTemperature = t0,
                                                OvenTemperatures_1 = t1,
                                                OvenTemperatures_2 = t2,
                                                OvenTemperatures_3 = t3,
                                                OvenTemperatures_4 = t4,
                                                OvenTemperatures_5 = t5,
                                                OvenTemperatures_6 = t6,
                                                OvenTemperatures_7 = t7,
                                                OvenTemperatures_8 = t8
                                            });
        }

        /// <summary>重設PLC資料對應列表</summary>
        /// <param name="map"></param>
        public void ResetMapList(PLC_DevicesMap map)
        {
            M_Values.Clear();
            D_Values.Clear();
            Recipe_Values.Clear();

            foreach (var loc in map.SignalList)
            {
                M_Values.Add(loc.Key, loc.Value, false);
            }

            foreach (var loc in map.DataList)
            {
                D_Values.Add(loc.Key, loc.Value, 0);
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
            if (GetRecipeEvent != null && await GetRecipeEvent.Invoke(recipeName) is PLC_Recipe recipe)
            {
                if (await Dialog.Show(new Dictionary<Language, string> { { Language.TW, "請確認配方內容：" }, { Language.CHS, "请确认配方内容：" }, { Language.EN, "Please confirm this recipe:" } }, recipe, true))
                {
                    recipe.CopyTo(this);

                    Set(RecipeName, nameof(Selected_Name));
                }
            }

            Intput_Name = Selected_Name;

            if (SetPLCRecipeParameter != null)
            {
                //!PLC配方位置在監視位置+100的位置
                await SetPLCRecipeParameter.Invoke(Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key + 100, x => x.Value));
            }
        }

        /// <summary>開始記錄</summary>
        /// <param name="cycle_ms">紀錄週期，單位為毫秒</param>
        /// <param name="ct">取消任務的token</param>
        /// <returns></returns>
        public async Task StartRecoder(long cycle_ms, CancellationToken ct)
        {
            OvenInfo.StartTime = DateTime.Now;

            await Task.Factory.StartNew(() =>
                                        {
                                            var n = TimeSpan.Zero;

                                            while (!ct.IsCancellationRequested)
                                            {
                                                var nt = DateTime.Now;
                                                var et = nt - OvenInfo.StartTime;
                                                if (et >= n)
                                                {
                                                    AddTemperatures(OvenInfo.StartTime,
                                                                    nt,
                                                                    ThermostatTemperature,
                                                                    OvenTemperature_1,
                                                                    OvenTemperature_2,
                                                                    OvenTemperature_3,
                                                                    OvenTemperature_4,
                                                                    OvenTemperature_5,
                                                                    OvenTemperature_6,
                                                                    OvenTemperature_7,
                                                                    OvenTemperature_8);

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
                                        TaskCreationOptions.LongRunning);
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
                                                                Dialog.Show(new Dictionary<Language, string> { { Language.TW, "配方無變更" }, { Language.CHS, "配方无变更" }, { Language.EN, "No change." } });
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
                                                             var para = (string)o;

                                                             var (result1, intput1) =
                                                                 await Dialog.ShowWithIntput(new Dictionary<Language, string>
                                                                                             {
                                                                                                 { Language.TW, "輸入操作人員ID" }, { Language.CHS, "输入操作人员ID" }, { Language.EN, "Enter the Operator ID" }
                                                                                             },
                                                                                             new Dictionary<Language, string> { { Language.TW, para }, { Language.CHS, para }, { Language.EN, para } },
                                                                                             x =>
                                                                                             {
                                                                                                 var str = x.Trim();

                                                                                                 return (str.Length > 4 && str.Length < 10,
                                                                                                         new Dictionary<Language, string>
                                                                                                         {
                                                                                                             { Language.TW, "字數錯誤，請重試!" },
                                                                                                             { Language.CHS, "字数错误，请重试!" },
                                                                                                             { Language.EN, "Input error, please try again!" }
                                                                                                         });
                                                                                             });

                                                             if (result1)
                                                             {
                                                                 var (result2, intput2) =
                                                                     await Dialog.ShowWithIntput(new Dictionary<Language, string>
                                                                                                 {
                                                                                                     { Language.TW, "輸入台車碼" }, { Language.CHS, "输入台车码" }, { Language.EN, "Enter the Trolley Code" }
                                                                                                 },
                                                                                                 new Dictionary<Language, string>
                                                                                                 {
                                                                                                     { Language.TW, para }, { Language.CHS, para }, { Language.EN, para }
                                                                                                 },
                                                                                                 x =>
                                                                                                 {
                                                                                                     var str = x.Trim();

                                                                                                     return (str.Length > 4 && str.Length < 15,
                                                                                                             new Dictionary<Language, string>
                                                                                                             {
                                                                                                                 { Language.TW, "字數錯誤，請重試!" },
                                                                                                                 { Language.CHS, "字数错误，请重试!" },
                                                                                                                 { Language.EN, "Input error, please try again!" }
                                                                                                             });
                                                                                                 });

                                                                 if (result2)
                                                                 {
                                                                     OvenInfo.OperatorID = intput1;
                                                                     OvenInfo.TrolleyCode = intput2;

                                                                     var (result3, intput3) =
                                                                         await Dialog.ShowWithIntput(new Dictionary<Language, string>
                                                                                                     {
                                                                                                         { Language.TW, "輸入工單碼和製程序" },
                                                                                                         { Language.CHS, "输入工单码和制程序" },
                                                                                                         { Language.EN, "Enter order code and process number" }
                                                                                                     },
                                                                                                     new Dictionary<Language, string>
                                                                                                     {
                                                                                                         { Language.TW, para }, { Language.CHS, para }, { Language.EN, para }
                                                                                                     },
                                                                                                     x =>
                                                                                                     {
                                                                                                         var str = x.Trim();

                                                                                                         if (!str.Contains(','))
                                                                                                         {
                                                                                                             return (false,
                                                                                                                     new Dictionary<Language, string>
                                                                                                                     {
                                                                                                                         { Language.TW, "未包含\",\"分隔號，請重試!" },
                                                                                                                         { Language.CHS, "未包含\",\"分隔符，请重试!" },
                                                                                                                         { Language.EN, "Does not contain the separator \",\", please try again!" }
                                                                                                                     });
                                                                                                         }

                                                                                                         var s = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                                                                                         return
                                                                                                             (s.Length > 1 && s[0].Length >= 8 && s[0].Length <= 12 && s[1].Length > 0 && s[1].Length <= 4,
                                                                                                              new Dictionary<Language, string>
                                                                                                              {
                                                                                                                  { Language.TW, "字數錯誤，請重試!" },
                                                                                                                  { Language.CHS, "字数错误，请重试!" },
                                                                                                                  { Language.EN, "Input error, please try again!" }
                                                                                                              });
                                                                                                     });

                                                                     if (!result3 || WantFrontData == null)
                                                                     {
                                                                         return false;
                                                                     }

                                                                     var panels = await WantFrontData.Invoke((OvenInfo.TrolleyCode,
                                                                                                              intput3.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0]));

                                                                     if (!panels.Any())
                                                                     {
                                                                         Dialog.Show(new Dictionary<Language, string>
                                                                                     {
                                                                                         { Language.TW, "查無資料!" }, { Language.CHS, "查无资料!" }, { Language.EN, "No data found!" }
                                                                                     },
                                                                                     DialogMsgType.Alarm);

                                                                         return false;
                                                                     }

                                                                     Ext_Info.Clear();
                                                                     var info = new ProductInfo(intput3) { PanelCodes = panels };
                                                                     Ext_Info.Add(info);

                                                                     if (!PC_InUsed)
                                                                     {
                                                                         return await Dialog.Show(new Dictionary<Language, string>
                                                                                                  {
                                                                                                      { Language.TW, "目前烤箱處於\"PC PASS\"模式，無法遠端設定配方\n確定投產嗎?" },
                                                                                                      { Language.CHS, "目前烤箱处于\"PC PASS\"模式，无法远程设定配方\n确定投产吗?" },
                                                                                                      {
                                                                                                          Language.EN,
                                                                                                          "The oven is in \"PC PASS\" mode, can't set recipe remotely.\nAre you sure to execute?"
                                                                                                      }
                                                                                                  },
                                                                                                  true);
                                                                     }

                                                                     if (GetRecipeEvent != null && await GetRecipeEvent.Invoke(RecipeName) is PLC_Recipe recipe)
                                                                     {
                                                                         recipe.CopyTo(this);
                                                                     }

                                                                     if (SetPLCRecipeParameter != null)
                                                                     {
                                                                         //!PLC配方位置在監視位置+100的位置
                                                                         await SetPLCRecipeParameter.Invoke(Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key + 100, x => x.Value));
                                                                     }

                                                                     return true;
                                                                 }
                                                             }

                                                             return false;
                                                         });

            CancelCheckInCommand = new RelayCommand(e =>
                                                    {
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

            M_Values = new TwoKeyDictionary<SignalNames, int, bool>();
            D_Values = new TwoKeyDictionary<DataNames, int, short>();
            Recipe_Values = new TwoKeyDictionary<DataNames, int, short>();

            foreach (var loc in map.SignalList)
            {
                M_Values.Add(loc.Key, loc.Value, false);
            }

            foreach (var loc in map.DataList)
            {
                D_Values.Add(loc.Key, loc.Value, 0);
            }

            foreach (var loc in map.RecipeList)
            {
                Recipe_Values.Add(loc.Key, loc.Value, 0);
            }

            #region 將PLC掃描值和ViewModel上的Property做map連結

            var M_Map = new Dictionary<SignalNames, string>
                        {
                            { SignalNames.PC_InUsed, nameof(PC_InUsed) },
                            { SignalNames.自動模式, nameof(AutoMode) },
                            { SignalNames.自動啟動, nameof(AutoMode_Start) },
                            { SignalNames.自動停止, nameof(AutoMode_Stop) },
                            { SignalNames.手動模式, nameof(ManualMode) },
                            { SignalNames.降溫中, nameof(IsCooling) },
                            { SignalNames.程式結束, nameof(ProgramStop) },
                            { SignalNames.加熱門未關, nameof(DoorNotClosed) },
                            { SignalNames.緊急停止, nameof(EmergencyStop) },
                            { SignalNames.溫控器低溫異常, nameof(LowTemperature) },
                            { SignalNames.電源反相, nameof(PowerInversion) },
                            { SignalNames.OTP超溫異常, nameof(OTP_TemperatureError) },
                            { SignalNames.循環風車過載, nameof(CirculatingFanOverload) },
                            { SignalNames.冷卻進氣風車異常, nameof(CoolingFanAbnormal) },
                            { SignalNames.超溫警報, nameof(OverTemperatureAlarm) },
                            { SignalNames.停止後未開門, nameof(DoorNotOpen) },
                            { SignalNames.循環風車INV異常, nameof(CirculatingFanInversion) },
                            { SignalNames.充氮氣逾時, nameof(InflatingTimeExceeded) },
                            { SignalNames.門未關定位異常, nameof(DoorNotClosed_AbnormalPositioning) },
                            { SignalNames.升恆溫逾時, nameof(HeatingTimeExceeded) }
                        };
            var D_Map = new Dictionary<DataNames, string>
                        {
                            { DataNames.溫控器溫度, nameof(ThermostatTemperature) },
                            { DataNames.片段剩餘時間, nameof(Segment_RemainingTime) },
                            { DataNames.總剩餘時間, nameof(Total_RemainingTime) },
                            { DataNames.目前段數, nameof(CurrentSegment) },
                            { DataNames.爐內溫度_1, nameof(OvenTemperature_1) },
                            { DataNames.爐內溫度_2, nameof(OvenTemperature_2) },
                            { DataNames.爐內溫度_3, nameof(OvenTemperature_3) },
                            { DataNames.爐內溫度_4, nameof(OvenTemperature_4) },
                            { DataNames.爐內溫度_5, nameof(OvenTemperature_5) },
                            { DataNames.爐內溫度_6, nameof(OvenTemperature_6) },
                            { DataNames.爐內溫度_7, nameof(OvenTemperature_7) },
                            { DataNames.爐內溫度_8, nameof(OvenTemperature_8) },
                            { DataNames.目標溫度_1, nameof(TargetTemperature_1) },
                            { DataNames.升溫時間_1, nameof(HeatingTime_1) },
                            { DataNames.恆溫溫度_1, nameof(ThermostaticTemperature_1) },
                            { DataNames.恆溫時間_1, nameof(WarmingTime_1) },
                            { DataNames.目標溫度_2, nameof(TargetTemperature_2) },
                            { DataNames.升溫時間_2, nameof(HeatingTime_2) },
                            { DataNames.恆溫溫度_2, nameof(ThermostaticTemperature_2) },
                            { DataNames.恆溫時間_2, nameof(WarmingTime_2) },
                            { DataNames.目標溫度_3, nameof(TargetTemperature_3) },
                            { DataNames.升溫時間_3, nameof(HeatingTime_3) },
                            { DataNames.恆溫溫度_3, nameof(ThermostaticTemperature_3) },
                            { DataNames.恆溫時間_3, nameof(WarmingTime_3) },
                            { DataNames.目標溫度_4, nameof(TargetTemperature_4) },
                            { DataNames.升溫時間_4, nameof(HeatingTime_4) },
                            { DataNames.恆溫溫度_4, nameof(ThermostaticTemperature_4) },
                            { DataNames.恆溫時間_4, nameof(WarmingTime_4) },
                            { DataNames.目標溫度_5, nameof(TargetTemperature_5) },
                            { DataNames.升溫時間_5, nameof(HeatingTime_5) },
                            { DataNames.恆溫溫度_5, nameof(ThermostaticTemperature_5) },
                            { DataNames.恆溫時間_5, nameof(WarmingTime_5) },
                            { DataNames.目標溫度_6, nameof(TargetTemperature_6) },
                            { DataNames.升溫時間_6, nameof(HeatingTime_6) },
                            { DataNames.恆溫溫度_6, nameof(ThermostaticTemperature_6) },
                            { DataNames.恆溫時間_6, nameof(WarmingTime_6) },
                            { DataNames.目標溫度_7, nameof(TargetTemperature_7) },
                            { DataNames.升溫時間_7, nameof(HeatingTime_7) },
                            { DataNames.恆溫溫度_7, nameof(ThermostaticTemperature_7) },
                            { DataNames.恆溫時間_7, nameof(WarmingTime_7) },
                            { DataNames.目標溫度_8, nameof(TargetTemperature_8) },
                            { DataNames.升溫時間_8, nameof(HeatingTime_8) },
                            { DataNames.恆溫溫度_8, nameof(ThermostaticTemperature_8) },
                            { DataNames.恆溫時間_8, nameof(WarmingTime_8) },
                            { DataNames.降溫溫度, nameof(CoolingTemperature) },
                            { DataNames.充氣時間, nameof(InflatingTime) },
                            { DataNames.使用段數, nameof(UsedSegmentCounts) },
                            { DataNames.配方名稱_01, nameof(RecipeName) },
                            { DataNames.配方名稱_02, nameof(RecipeName) },
                            { DataNames.配方名稱_03, nameof(RecipeName) },
                            { DataNames.配方名稱_04, nameof(RecipeName) },
                            { DataNames.配方名稱_05, nameof(RecipeName) },
                            { DataNames.配方名稱_06, nameof(RecipeName) },
                            { DataNames.配方名稱_07, nameof(RecipeName) },
                            { DataNames.配方名稱_08, nameof(RecipeName) },
                            { DataNames.配方名稱_09, nameof(RecipeName) },
                            { DataNames.配方名稱_10, nameof(RecipeName) },
                            { DataNames.配方名稱_11, nameof(RecipeName) },
                            { DataNames.配方名稱_12, nameof(RecipeName) },
                            { DataNames.配方名稱_13, nameof(RecipeName) }
                        };

            #endregion

            #region 註冊PLC事件

            M_Values.UpdatedEvent += async (key1, key2, value) =>
                                     {
                                         NotifyPropertyChanged(M_Map[key1]);
                                         var nt = DateTime.Now;

                                         if (key1 == SignalNames.自動啟動)
                                         {
                                             EventHappened?.Invoke((EventType.Normal, nt, key1.ToString(), key2, value));

                                             if (!value)
                                             {
                                                 return;
                                             }

                                             if (IsRecording)
                                             {
                                                 CTS?.Cancel();

                                                 await RecordingTask;
                                             }

                                             ResetStopTokenSource();

                                             //! 當沒有刷取台車code時，抓取PLC目前配方參數以記錄
                                             if (string.IsNullOrEmpty(OvenInfo.TrolleyCode) && GetPLCRecipeParameter != null && await GetPLCRecipeParameter.Invoke() is Dictionary<int, short> recipe)
                                             {
                                                 foreach (var val in recipe)
                                                 {
                                                     Recipe_Values[val.Key] = val.Value;
                                                 }
                                             }

                                             RecordingTask = StartRecoder(60000, CTS.Token);
                                         }
                                         else if (IsRecording)
                                         {
                                             if (key1 == SignalNames.程式結束)
                                             {
                                                 PassTag = true;

                                                 EventHappened?.Invoke((EventType.Normal, nt, key1.ToString(), key2, value));
                                                 AddProcessEvent(EventType.Normal, OvenInfo.StartTime, nt, key1.ToString(), key2, value);

                                                 if (!value)
                                                 {
                                                     return;
                                                 }

                                                 CTS?.Cancel();
                                             }
                                             else if (key1 == SignalNames.自動停止)
                                             {
                                                 EventHappened?.Invoke((EventType.Normal, nt, key1.ToString(), key2, value));
                                                 AddProcessEvent(EventType.Normal, OvenInfo.StartTime, nt, key1.ToString(), key2, value);

                                                 if (!value)
                                                 {
                                                     return;
                                                 }

                                                 CTS?.Cancel();
                                             }
                                             else if (key1 == SignalNames.緊急停止 || key1 == SignalNames.電源反相 || key1 == SignalNames.循環風車過載 || key1 == SignalNames.循環風車INV異常)
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

                                                 EventHappened?.Invoke((EventType.Normal, nt, key1.ToString(), key2, value));
                                                 AddProcessEvent(EventType.Normal, OvenInfo.StartTime, nt, key1.ToString(), key2, value);
                                                 NotifyPropertyChanged(nameof(Progress));
                                                 NotifyPropertyChanged(nameof(ProgressStatus));
                                             }
                                         }
                                     };

            D_Values.UpdatedEvent += (key1, key2, value) =>
                                     {
                                         NotifyPropertyChanged(D_Map[key1]);
                                         var nt = DateTime.Now;

                                         if (key1 == DataNames.目前段數)
                                         {
                                             if (IsRecording)
                                             {
                                                 AddProcessEvent(EventType.Normal,
                                                                 OvenInfo.StartTime,
                                                                 nt,
                                                                 CurrentSegment == 0 ? "準備中" : "第" + Math.Ceiling(CurrentSegment / 2.0).ToString("0") + "段" + (CurrentSegment % 2 == 0 ? "恆溫" : "升溫"),
                                                                 0,
                                                                 true);
                                             }

                                             NotifyPropertyChanged(nameof(Progress));
                                             NotifyPropertyChanged(nameof(ProgressStatus));
                                         }
                                     };

            Recipe_Values.UpdatedEvent += (key1, key2, value) =>
                                          {
                                              NotifyPropertyChanged(D_Map[key1]);

                                              if (key1.ToString().Contains("配方名稱"))
                                              {
                                                  Set(RecipeName, nameof(Selected_Name));
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

            #endregion
        }
    }
}