using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GPGO_MultiPLCs.Models
{
    public class PLC_DataProvider : PLC_Data
    {
        public delegate void RecordingFinishedEventHandler(ProcessInfo info);

        public delegate void StartRecordingHandler(string recipe, AutoResetEvent LockObj);

        public delegate void SwitchRecipeEventHandler(string recipe);

        public CancellationTokenSource CTS;
        private readonly OxyColor bgcolor = OxyColor.FromRgb(240, 255, 235);
        private readonly OxyColor bordercolor = OxyColor.FromRgb(174, 187, 168);
        private readonly OxyColor fontcolor = OxyColor.FromRgb(50, 70, 60);

        /// <summary>
        ///     溫控器溫度+槽內溫度共9項
        /// </summary>
        private readonly LineSeries[] LineSeries = new LineSeries[9];

        private readonly AutoResetEvent LockHandle = new AutoResetEvent(false);
        private readonly Stopwatch sw = new Stopwatch();
        private readonly TimeSpanAxis TimeAxis;
        private bool _OnlineStatus;
        private ICollection<string> _Recipe_Names;
        private Task _RecordingTask;
        private string _Selected_Name;
        public CommandWithResult<bool> CheckInCommand { get; }

        public bool IsRecording => _RecordingTask?.Status == TaskStatus.Running ||
                                   _RecordingTask?.Status == TaskStatus.RanToCompletion ||
                                   _RecordingTask?.Status == TaskStatus.WaitingForActivation ||
                                   _RecordingTask?.Status == TaskStatus.WaitingToRun;

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

        public string ProgressString
        {
            get
            {
                if (IsRecording && CurrentSegment == 0)
                {
                    return "準備中";
                }

                if (IsCooling && CurrentSegment >= UsedSegmentCounts * 2)
                {
                    return "降溫中";
                }

                return CurrentSegment % 2 == 0 ? CurrentSegment == 0 ? "待命中" : "恆溫中" : "升溫中";
            }
        }

        public PlotModel RecordView { get; }

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

        public event RecordingFinishedEventHandler RecordingFinished;
        public event StartRecordingHandler StartRecording;
        public event SwitchRecipeEventHandler SwitchRecipeEvent;

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

                                            foreach (var ls in LineSeries)
                                            {
                                                ls.Points.Clear();
                                            }

                                            TimeAxis.Unit = "秒";
                                            TimeAxis.MajorStep = 10;
                                            TimeAxis.MinorStep = 1;
                                            TimeAxis.Maximum = 60;
                                            TimeAxis.StringFormat = "m:ss";

                                            RecordView.InvalidatePlot(true);

                                            LockHandle.WaitOne();

                                            var n = TimeSpan.Zero;
                                            sw.Restart();

                                            while (!ct.IsCancellationRequested)
                                            {
                                                if (sw.Elapsed >= n)
                                                {
                                                    var vals = new RecordTemperatures
                                                               {
                                                                   Time = sw.Elapsed,
                                                                   ThermostatTemperature = ThermostatTemperature,
                                                                   OvenTemperatures =
                                                                   {
                                                                       [0] = OvenTemperature_1,
                                                                       [1] = OvenTemperature_2,
                                                                       [2] = OvenTemperature_3,
                                                                       [3] = OvenTemperature_4,
                                                                       [4] = OvenTemperature_5,
                                                                       [5] = OvenTemperature_6,
                                                                       [6] = OvenTemperature_7,
                                                                       [7] = OvenTemperature_8
                                                                   }
                                                               };

                                                    //var rn = new Random();
                                                    //var t = sw.Elapsed;
                                                    //var mins = (int)t.TotalMinutes + 1;

                                                    //var vals = new RecordTemperatures
                                                    //           {
                                                    //               Time = t,
                                                    //               ThermostatTemperature = rn.Next(40 + mins * 2, 40 + mins * 5),
                                                    //               OvenTemperatures =
                                                    //               {
                                                    //                   [0] = rn.Next(40 + mins * 2, 40 + mins * 5),
                                                    //                   [1] = rn.Next(40 + mins * 2, 40 + mins * 5),
                                                    //                   [2] = rn.Next(40 + mins * 2, 40 + mins * 5),
                                                    //                   [3] = rn.Next(40 + mins * 2, 40 + mins * 5),
                                                    //                   [4] = rn.Next(40 + mins * 2, 40 + mins * 5),
                                                    //                   [5] = rn.Next(40 + mins * 2, 40 + mins * 5),
                                                    //                   [6] = rn.Next(40 + mins * 2, 40 + mins * 5),
                                                    //                   [7] = rn.Next(40 + mins * 2, 40 + mins * 5)
                                                    //               }
                                                    //           };

                                                    Process_Info.RecordTemperatures.Add(vals);
                                                    AddPlot(sw.Elapsed, vals);

                                                    if (n >= TimeSpan.FromMinutes(10))
                                                    {
                                                        n += TimeSpan.FromMinutes(1);
                                                    }
                                                    else if (n >= TimeSpan.FromMinutes(1))
                                                    {
                                                        n += TimeSpan.FromSeconds(20);
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

                                            sw.Stop();
                                        },
                                        TaskCreationOptions.LongRunning);
        }

        private void AddPlot(TimeSpan t, RecordTemperatures vals)
        {
            if (t > TimeSpan.FromMinutes(60))
            {
                TimeAxis.Unit = "分";
                TimeAxis.MajorStep = 60 * 30;
                TimeAxis.MinorStep = 60;
                TimeAxis.Maximum = 60 * 60 * 3;
                TimeAxis.StringFormat = "hh:mm";
            }
            else if (t > TimeSpan.FromMinutes(1))
            {
                TimeAxis.Unit = "分";
                TimeAxis.MajorStep = 60 * 10;
                TimeAxis.MinorStep = 60;
                TimeAxis.Maximum = 60 * 60;
                TimeAxis.StringFormat = "hh:mm";

                //foreach (var ls in LineSeries)
                //{
                //    var pt = ls.Points.First();
                //    ls.Points.Clear();
                //    ls.Points.Add(pt);
                //}
            }

            var time = TimeSpanAxis.ToDouble(t);
            LineSeries[8].Points.Add(new DataPoint(time, vals.ThermostatTemperature));
            LineSeries[7].Points.Add(new DataPoint(time, vals.OvenTemperatures[0]));
            LineSeries[6].Points.Add(new DataPoint(time, vals.OvenTemperatures[1]));
            LineSeries[5].Points.Add(new DataPoint(time, vals.OvenTemperatures[2]));
            LineSeries[4].Points.Add(new DataPoint(time, vals.OvenTemperatures[3]));
            LineSeries[3].Points.Add(new DataPoint(time, vals.OvenTemperatures[4]));
            LineSeries[2].Points.Add(new DataPoint(time, vals.OvenTemperatures[5]));
            LineSeries[1].Points.Add(new DataPoint(time, vals.OvenTemperatures[6]));
            LineSeries[0].Points.Add(new DataPoint(time, vals.OvenTemperatures[7]));

            RecordView.InvalidatePlot(true);
        }

        public PLC_DataProvider(Dictionary<SignalNames, int> M_MapList, Dictionary<DataNames, int> D_MapList, Dictionary<DataNames, int> Recipe_MapList, IDialogService<string> dialog)
        {
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

            RecordView = new PlotModel
                         {
                             DefaultFont = "Microsoft JhengHei",
                             PlotAreaBackground = bgcolor,
                             PlotAreaBorderThickness = new OxyThickness(0, 1, 1, 0),
                             PlotAreaBorderColor = bordercolor,
                             PlotMargins = new OxyThickness(50, 10, 0, 40),
                             LegendBorder = bordercolor,
                             LegendTextColor = fontcolor,
                             LegendBackground = bgcolor,
                             LegendPlacement = LegendPlacement.Outside,
                             LegendPosition = LegendPosition.RightTop,
                             LegendFontSize = 14,
                             LegendItemOrder = LegendItemOrder.Reverse,
                             LegendOrientation = LegendOrientation.Vertical,
                             LegendItemSpacing = 8,
                             LegendLineSpacing = 2
                         };

            var temperatureAxis = new LinearAxis
                                  {
                                      TitleColor = fontcolor,
                                      Title = "溫度",
                                      Unit = "°C",
                                      Position = AxisPosition.Left,
                                      TickStyle = TickStyle.Inside,
                                      MajorGridlineStyle = LineStyle.Dot,
                                      MinorGridlineStyle = LineStyle.None,
                                      MinorTickSize = 0,
                                      MajorTickSize = 0,
                                      AxislineStyle = LineStyle.Solid,
                                      AxislineColor = bordercolor,
                                      MajorGridlineColor = bordercolor,
                                      MinorGridlineColor = bordercolor,
                                      TicklineColor = bordercolor,
                                      ExtraGridlineColor = bordercolor,
                                      TextColor = fontcolor,
                                      MaximumPadding = 0.2,
                                      Minimum = 0,
                                      MinimumPadding = 0
                                  };

            TimeAxis = new TimeSpanAxis
                       {
                           TitleColor = fontcolor,
                           Title = "歷時",
                           Unit = "秒",
                           MinimumPadding = 0,
                           MaximumPadding = 0,
                           MinorTickSize = 0,
                           MajorTickSize = 0,
                           TickStyle = TickStyle.Inside,
                           MajorGridlineStyle = LineStyle.Dot,
                           MajorStep = 10,
                           MinorGridlineStyle = LineStyle.None,
                           MinorStep = 1,
                           Position = AxisPosition.Bottom,
                           AxislineStyle = LineStyle.Solid,
                           AxislineColor = bordercolor,
                           MajorGridlineColor = bordercolor,
                           MinorGridlineColor = bordercolor,
                           TicklineColor = bordercolor,
                           ExtraGridlineColor = bordercolor,
                           TextColor = fontcolor,
                           StringFormat = "m:ss",
                           Maximum = 60,
                           Minimum = 0
                       };

            LineSeries[8] = new LineSeries
                            {
                                Title = nameof(DataNames.溫控器溫度),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                MarkerFill = OxyColors.Red,
                                MarkerType = MarkerType.None,
                                Color = OxyColors.Red,
                                MarkerSize = 1
                            };

            LineSeries[7] = new LineSeries
                            {
                                Title = nameof(DataNames.爐內溫度_1),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                MarkerFill = OxyColors.DarkOrange,
                                MarkerType = MarkerType.None,
                                Color = OxyColors.DarkOrange,
                                MarkerSize = 1
                            };

            LineSeries[6] = new LineSeries
                            {
                                Title = nameof(DataNames.爐內溫度_2),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                MarkerFill = OxyColors.Gold,
                                MarkerType = MarkerType.None,
                                Color = OxyColors.Gold,
                                MarkerSize = 1
                            };

            LineSeries[5] = new LineSeries
                            {
                                Title = nameof(DataNames.爐內溫度_3),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                MarkerFill = OxyColors.Lime,
                                MarkerType = MarkerType.None,
                                Color = OxyColors.Lime,
                                MarkerSize = 1
                            };

            LineSeries[4] = new LineSeries
                            {
                                Title = nameof(DataNames.爐內溫度_4),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                MarkerFill = OxyColors.DodgerBlue,
                                MarkerType = MarkerType.None,
                                Color = OxyColors.DodgerBlue,
                                MarkerSize = 1
                            };

            LineSeries[3] = new LineSeries
                            {
                                Title = nameof(DataNames.爐內溫度_5),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                MarkerFill = OxyColors.DarkOrchid,
                                MarkerType = MarkerType.None,
                                Color = OxyColors.DarkOrchid,
                                MarkerSize = 1
                            };

            LineSeries[2] = new LineSeries
                            {
                                Title = nameof(DataNames.爐內溫度_6),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                MarkerFill = OxyColors.Magenta,
                                MarkerType = MarkerType.None,
                                Color = OxyColors.Magenta,
                                MarkerSize = 1
                            };

            LineSeries[1] = new LineSeries
                            {
                                Title = nameof(DataNames.爐內溫度_7),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                MarkerFill = OxyColors.Brown,
                                MarkerType = MarkerType.None,
                                Color = OxyColors.Brown,
                                MarkerSize = 1
                            };

            LineSeries[0] = new LineSeries
                            {
                                Title = nameof(DataNames.爐內溫度_8),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                MarkerFill = OxyColors.BurlyWood,
                                MarkerType = MarkerType.None,
                                Color = OxyColors.BurlyWood,
                                MarkerSize = 1
                            };

            RecordView.Axes.Add(temperatureAxis);
            RecordView.Axes.Add(TimeAxis);
            foreach (var ls in LineSeries)
            {
                RecordView.Series.Add(ls);
            }

            Process_Info = new ProcessInfo();

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
                                                         Process_Info.EventList.Add(new RecordEvent { Type = EventType.Normal, Time = sw.Elapsed, Description = key.ToString() });
                                                         CTS?.Cancel();
                                                     }
                                                     else if (key == SignalNames.緊急停止 || key == SignalNames.電源反相 || key == SignalNames.循環風車過載 || key == SignalNames.循環風車INV異常)
                                                     {
                                                         Process_Info.EventList.Add(new RecordEvent { Type = EventType.Alarm, Time = sw.Elapsed, Description = key.ToString() });
                                                         CTS?.Cancel();
                                                     }
                                                     else if (key == SignalNames.降溫中)
                                                     {
                                                         Process_Info.EventList.Add(new RecordEvent { Type = EventType.Normal, Time = sw.Elapsed, Description = key.ToString() });
                                                         NotifyPropertyChanged(nameof(ProgressString));
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
                                                     Process_Info.EventList.Add(new RecordEvent
                                                                                {
                                                                                    Type = EventType.Normal,
                                                                                    Time = sw.Elapsed,
                                                                                    Description =
                                                                                        "第" +
                                                                                        (CurrentSegment / 2 + 1) +
                                                                                        "段" +
                                                                                        (CurrentSegment % 2 == 0 ? CurrentSegment == 0 ? "待命" : "恆溫" : "升溫")
                                                                                });
                                                 }

                                                 NotifyPropertyChanged(nameof(Progress));
                                                 NotifyPropertyChanged(nameof(ProgressString));
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
                                                      NotifyPropertyChanged(nameof(ProgressString));
                                                  }
                                              };

            #endregion
        }
    }
}