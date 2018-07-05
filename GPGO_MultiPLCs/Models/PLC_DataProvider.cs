using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    public class PLC_DataProvider : PLC_Data
    {
        public enum Status
        {
            待命中,
            準備中,
            升溫,
            恆溫,
            降溫
        }

        public delegate void MachineCodeChangedHandler(string code);

        public delegate void RecordingFinishedEventHandler(ProcessInfo info);

        public delegate void StartRecordingHandler(string recipe, AutoResetEvent LockObj);

        public delegate void SwitchRecipeEventHandler(string recipe);

        public CancellationTokenSource CTS;
        private readonly AutoResetEvent LockHandle = new AutoResetEvent(false);
        private readonly Stopwatch sw = new Stopwatch();
        private bool _OnlineStatus;
        private ICollection<string> _Recipe_Names;
        private Task _RecordingTask;
        private string _Selected_Name;
        private string _Intput_Name;

        public RelayCommand CheckRecipeCommand_KeyIn { get; }

        public RelayCommand CheckRecipeCommand_KeyLeave { get; }

        public CommandWithResult<bool> CheckInCommand { get; }

        public bool IsRecording => _RecordingTask?.Status == TaskStatus.Running || _RecordingTask?.Status == TaskStatus.WaitingForActivation || _RecordingTask?.Status == TaskStatus.WaitingToRun;

        public ProcessInfo Process_Info { get; }

        public double Progress
        {
            get
            {
                var val = (double)CurrentSegment / UsedSegmentCounts / 2;

                if (double.IsNaN(val) || double.IsInfinity(val) || val < 0.0)
                {
                    return 0.0;
                }

                return val > 1.0 ? 1.0 : val;
            }
        }

        public Status ProgressStatus
        {
            get
            {
                if (IsRecording && CurrentSegment == 0)
                {
                    return Status.準備中;
                }

                if (IsCooling && CurrentSegment >= UsedSegmentCounts * 2)
                {
                    return Status.降溫;
                }

                return CurrentSegment % 2 == 0 ? CurrentSegment == 0 ? Status.待命中 : Status.恆溫 : Status.升溫;
            }
        }

        public bool OnlineStatus
        {
            get => _OnlineStatus;
            set
            {
                _OnlineStatus = value;
                NotifyPropertyChanged();
            }
        }

        public ICollection<string> Recipe_Names
        {
            get => _Recipe_Names;
            set
            {
                _Recipe_Names = value;
                NotifyPropertyChanged();
            }
        }

        public Task RecordingTask
        {
            get => _RecordingTask;
            set
            {
                _RecordingTask = value;
                Process_Info.StartTime = DateTime.Now;

                _RecordingTask.ContinueWith(x =>
                                            {
                                                NotifyPropertyChanged(nameof(IsRecording));

                                                x.Dispose();
                                                Process_Info.EndTime = DateTime.Now;

                                                RecordingFinished?.Invoke(Process_Info);
                                            });

                NotifyPropertyChanged(nameof(IsRecording));
            }
        }

        public string Selected_Name
        {
            get => _Selected_Name;
            set
            {
                _Selected_Name = value;
                NotifyPropertyChanged();

                SwitchRecipeEvent?.Invoke(_Selected_Name);
            }
        }

        public string Intput_Name
        {
            get => _Intput_Name;
            set
            {
                _Intput_Name = value;
                NotifyPropertyChanged();
            }
        }

        public event Action RecipeKeyInError;
        public event MachineCodeChangedHandler MachineCodeChanged;
        public event RecordingFinishedEventHandler RecordingFinished;
        public event StartRecordingHandler StartRecording;
        public event SwitchRecipeEventHandler SwitchRecipeEvent;

        public void AddProcessEvent(EventType type, TimeSpan time, string note)
        {
            Process_Info.EventList.Add(new RecordEvent { Type = type, Time = time, Description = note });
        }

        public void AddTemperatures(TimeSpan time, double t0, double t1, double t2, double t3, double t4, double t5, double t6, double t7, double t8)
        {
            Process_Info.RecordTemperatures.Add(new RecordTemperatures
                                                {
                                                    Time = time,
                                                    ThermostatTemperature = t0,
                                                    OvenTemperatures_0 = t1,
                                                    OvenTemperatures_1 = t2,
                                                    OvenTemperatures_2 = t3,
                                                    OvenTemperatures_3 = t4,
                                                    OvenTemperatures_4 = t5,
                                                    OvenTemperatures_5 = t6,
                                                    OvenTemperatures_6 = t7,
                                                    OvenTemperatures_7 = t8
                                                });
        }

        public void ResetStopTokenSource()
        {
            CTS?.Dispose();

            CTS = new CancellationTokenSource();
            //CTS.Token.Register(() =>
            //{
            //
            //});
        }

        public async Task StartRecoder(long cycle_ms, CancellationToken ct)
        {
            StartRecording?.Invoke(RecipeName, LockHandle); //! 引發開始記錄事件並以LockHandle等待完成

            await Task.Factory.StartNew(() =>
                                        {
                                            Process_Info.EventList.Clear();
                                            Process_Info.RecordTemperatures.Clear();

                                            LockHandle.WaitOne();

                                            var n = TimeSpan.Zero;
                                            sw.Restart();

                                            while (!ct.IsCancellationRequested)
                                            {
                                                if (sw.Elapsed >= n)
                                                {
                                                    AddTemperatures(sw.Elapsed,
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

                                            AddTemperatures(sw.Elapsed,
                                                            ThermostatTemperature,
                                                            OvenTemperature_1,
                                                            OvenTemperature_2,
                                                            OvenTemperature_3,
                                                            OvenTemperature_4,
                                                            OvenTemperature_5,
                                                            OvenTemperature_6,
                                                            OvenTemperature_7,
                                                            OvenTemperature_8);

                                            sw.Stop();
                                        },
                                        TaskCreationOptions.LongRunning);
        }

        public PLC_DataProvider(Dictionary<SignalNames, int> M_MapList, Dictionary<DataNames, int> D_MapList, Dictionary<DataNames, int> Recipe_MapList, IDialogService<string> dialog)
        {
            CheckRecipeCommand_KeyIn = new RelayCommand(e =>
                                                        {
                                                            if (_Selected_Name != _Intput_Name)
                                                            {
                                                                var args = (KeyEventArgs)e;
                                                                if (args.Key == Key.Enter)
                                                                {
                                                                    if (_Recipe_Names.Contains(_Intput_Name))
                                                                    {
                                                                        Selected_Name = _Intput_Name;
                                                                    }
                                                                    else
                                                                    {
                                                                        Intput_Name = "";
                                                                        RecipeKeyInError?.Invoke();
                                                                    }
                                                                }
                                                            }
                                                        });

            CheckRecipeCommand_KeyLeave = new RelayCommand(e =>
                                                           {
                                                               if(_Selected_Name != _Intput_Name)
                                                               {
                                                                   if (_Recipe_Names.Contains(_Intput_Name))
                                                                   {
                                                                       Selected_Name = _Intput_Name;
                                                                   }
                                                                   else
                                                                   {
                                                                       if (_Intput_Name != "")
                                                                       {
                                                                           RecipeKeyInError?.Invoke();
                                                                       }

                                                                       Intput_Name = _Selected_Name;
                                                                   }
                                                               }
                                                           });

            CheckInCommand = new CommandWithResult<bool>(async o =>
                                                         {
                                                             var para = (string)o;

                                                             var (result1, intput1) = await dialog.ShowWithIntput("輸入操作人員ID",
                                                                                                                  para,
                                                                                                                  x =>
                                                                                                                  {
                                                                                                                      var str = x.Trim();

                                                                                                                      return (str.Length > 0 && str.Length < 8, "字數錯誤，請重試!");
                                                                                                                  });

                                                             if (result1)
                                                             {
                                                                 var (result2, intput2) = await dialog.ShowWithIntput("輸入台車Code",
                                                                                                                      para,
                                                                                                                      x =>
                                                                                                                      {
                                                                                                                          var str = x.Trim();

                                                                                                                          return (str.Length > 0 && str.Length < 4, "字數錯誤，請重試!");
                                                                                                                      });

                                                                 if (result2)
                                                                 {
                                                                     Process_Info.OperatorID = intput1;
                                                                     Process_Info.TrolleyCode = intput2;

                                                                     SwitchRecipeEvent?.Invoke(_Selected_Name); //! 投產成功，獲取配方參數並寫入PLC

                                                                     return true;
                                                                 }
                                                             }

                                                             return false;
                                                         });

            Process_Info = new ProcessInfo();
            Process_Info.PropertyChanged += (s, e) =>
                                            {
                                                if (e.PropertyName == nameof(ProcessInfo.MachineCode))
                                                {
                                                    MachineCodeChanged?.Invoke((s as ProcessInfo)?.MachineCode);
                                                }
                                            };

            var M_Map = new Dictionary<SignalNames, string>
                        {
                            { SignalNames.PC_ByPass, nameof(PC_ByPass) },
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
                            { DataNames.恆溫時間_1, nameof(ConstantTime_1) },
                            { DataNames.目標溫度_2, nameof(TargetTemperature_2) },
                            { DataNames.升溫時間_2, nameof(HeatingTime_2) },
                            { DataNames.恆溫溫度_2, nameof(ThermostaticTemperature_2) },
                            { DataNames.恆溫時間_2, nameof(ConstantTime_2) },
                            { DataNames.目標溫度_3, nameof(TargetTemperature_3) },
                            { DataNames.升溫時間_3, nameof(HeatingTime_3) },
                            { DataNames.恆溫溫度_3, nameof(ThermostaticTemperature_3) },
                            { DataNames.恆溫時間_3, nameof(ConstantTime_3) },
                            { DataNames.目標溫度_4, nameof(TargetTemperature_4) },
                            { DataNames.升溫時間_4, nameof(HeatingTime_4) },
                            { DataNames.恆溫溫度_4, nameof(ThermostaticTemperature_4) },
                            { DataNames.恆溫時間_4, nameof(ConstantTime_4) },
                            { DataNames.目標溫度_5, nameof(TargetTemperature_5) },
                            { DataNames.升溫時間_5, nameof(HeatingTime_5) },
                            { DataNames.恆溫溫度_5, nameof(ThermostaticTemperature_5) },
                            { DataNames.恆溫時間_5, nameof(ConstantTime_5) },
                            { DataNames.目標溫度_6, nameof(TargetTemperature_6) },
                            { DataNames.升溫時間_6, nameof(HeatingTime_6) },
                            { DataNames.恆溫溫度_6, nameof(ThermostaticTemperature_6) },
                            { DataNames.恆溫時間_6, nameof(ConstantTime_6) },
                            { DataNames.目標溫度_7, nameof(TargetTemperature_7) },
                            { DataNames.升溫時間_7, nameof(HeatingTime_7) },
                            { DataNames.恆溫溫度_7, nameof(ThermostaticTemperature_7) },
                            { DataNames.恆溫時間_7, nameof(ConstantTime_7) },
                            { DataNames.目標溫度_8, nameof(TargetTemperature_8) },
                            { DataNames.升溫時間_8, nameof(HeatingTime_8) },
                            { DataNames.恆溫溫度_8, nameof(ThermostaticTemperature_8) },
                            { DataNames.恆溫時間_8, nameof(ConstantTime_8) },
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

            M_Values = new TwoKeyDictionary<SignalNames, int, bool>();
            D_Values = new TwoKeyDictionary<DataNames, int, short>();
            Recipe_Values = new TwoKeyDictionary<DataNames, int, short>();

            foreach (var loc in M_MapList)
            {
                M_Values.Add(loc.Key, loc.Value, false);
            }

            foreach (var loc in D_MapList)
            {
                D_Values.Add(loc.Key, loc.Value, 0);
            }

            foreach (var loc in Recipe_MapList)
            {
                Recipe_Values.Add(loc.Key, loc.Value, 0);
            }

            #region 註冊PLC事件

            M_Values.Key1UpdatedEvent += async (key, value) =>
                                         {
                                             NotifyPropertyChanged(M_Map[key]);

                                             if (value)
                                             {
                                                 if (key == SignalNames.自動啟動)
                                                 {
                                                     if (IsRecording)
                                                     {
                                                         CTS?.Cancel();

                                                         await _RecordingTask;
                                                     }

                                                     ResetStopTokenSource();
                                                     RecordingTask = StartRecoder(60000, CTS.Token);
                                                 }
                                                 else if (IsRecording)
                                                 {
                                                     if (key == SignalNames.自動停止 || key == SignalNames.程式結束)
                                                     {
                                                         AddProcessEvent(EventType.Normal, sw.Elapsed, key.ToString());
                                                         CTS?.Cancel();
                                                     }
                                                     else if (key == SignalNames.緊急停止 || key == SignalNames.電源反相 || key == SignalNames.循環風車過載 || key == SignalNames.循環風車INV異常)
                                                     {
                                                         AddProcessEvent(EventType.Alarm, sw.Elapsed, key.ToString());
                                                         CTS?.Cancel();
                                                     }
                                                     else if (key == SignalNames.降溫中)
                                                     {
                                                         AddProcessEvent(EventType.Normal, sw.Elapsed, key.ToString());
                                                         NotifyPropertyChanged(nameof(ProgressStatus));
                                                     }
                                                 }
                                             }
                                         };

            D_Values.Key1UpdatedEvent += (key, value) =>
                                         {
                                             NotifyPropertyChanged(D_Map[key]);

                                             if (key == DataNames.目前段數)
                                             {
                                                 if (IsRecording)
                                                 {
                                                     AddProcessEvent(EventType.Normal,
                                                                     sw.Elapsed,
                                                                     CurrentSegment == 0 ? "準備中" : "第" + (CurrentSegment + 1) / 2 + "段" + (CurrentSegment % 2 == 0 ? "恆溫" : "升溫"));
                                                 }

                                                 NotifyPropertyChanged(nameof(Progress));
                                                 NotifyPropertyChanged(nameof(ProgressStatus));
                                             }
                                         };

            Recipe_Values.Key1UpdatedEvent += (key, value) =>
                                              {
                                                  NotifyPropertyChanged(D_Map[key]);

                                                  if (key.ToString().Contains("配方名稱"))
                                                  {
                                                      _Selected_Name = RecipeName;
                                                      NotifyPropertyChanged(nameof(Selected_Name));
                                                  }
                                                  else if (key == DataNames.使用段數)
                                                  {
                                                      NotifyPropertyChanged(nameof(Progress));
                                                      NotifyPropertyChanged(nameof(ProgressStatus));
                                                  }
                                              };

            #endregion
        }
    }
}