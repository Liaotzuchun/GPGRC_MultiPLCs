using System;
using System.Collections.Generic;
using System.Linq;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using MongoDB.Driver;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GPGO_MultiPLCs.ViewModels
{
    public class TraceabilityView_ViewModel : ViewModelBase
    {
        private readonly OxyColor bordercolor = OxyColor.FromRgb(174, 187, 168);
        private readonly CategoryAxis categoryAxis1;
        private readonly CategoryAxis categoryAxis2;
        private readonly OxyColor fontcolor = OxyColor.FromRgb(50, 70, 60);
        private readonly MongoClient Mongo_Client;
        private bool _ByPLC;
        private DateTime _Date1;
        private DateTime _Date2;
        private int _FilterIndex;
        private int _Index1;
        private int _Index2;
        private List<ProcessInfo> _Results;
        private bool _Standby = true;

        public RelayCommand AddDayCommand { get; }
        public RelayCommand AddMonthCommand { get; }
        public RelayCommand AddWeekCommand { get; }

        /// <summary>
        ///     基於PLC站號的Filter，站號由1開始
        /// </summary>
        public List<int> EnumFilter => _Results?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).ToList();

        public DateTime? LowerDate => _Results?.Count > 0 ? _Results[_Index1]?.AddedTime : null;
        public PlotModel ResultView { get; }

        public RelayCommand SubDayCommand { get; }
        public RelayCommand SubMonthCommand { get; }
        public RelayCommand SubWeekCommand { get; }
        public RelayCommand ThisDayCommand { get; }
        public RelayCommand ThisMonthCommand { get; }
        public RelayCommand ThisWeekCommand { get; }

        public int TotalCount => _Results?.Count > 0 ? _Results.Count - 1 : 0;

        public DateTime? UpperDate => _Results?.Count > 0 ? _Results[_Index2]?.AddedTime : null;

        /// <summary>
        ///     Results再依據時間段和PLC序號篩選結果
        /// </summary>
        public List<ProcessInfo> ViewResults => _Index2 >= _Index1 && _Results?.Count > 0 ?
                                                    _Results?.GetRange(_Index1, _Index2 - _Index1 + 1).Where(x => _FilterIndex == -1 || x.StationNumber == _FilterIndex).ToList() : null;

        public bool ByPLC
        {
            get => _ByPLC;
            set
            {
                _ByPLC = value;
                NotifyPropertyChanged();

                UpdateChart(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     選取的開始日期(資料庫)
        /// </summary>
        public DateTime Date1
        {
            get => _Date1;
            set
            {
                _Date1 = value;
                NotifyPropertyChanged();

                if (_Date2 < _Date1)
                {
                    _Date2 = _Date1;
                    NotifyPropertyChanged(nameof(Date2));
                }
                else if (_Date2 - Date1 > TimeSpan.FromDays(30))
                {
                    _Date2 = _Date1 + TimeSpan.FromDays(30);
                    NotifyPropertyChanged(nameof(Date2));
                }

                UpdateResults(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     選取的結束日期(資料庫)
        /// </summary>
        public DateTime Date2
        {
            get => _Date2;
            set
            {
                _Date2 = value;
                NotifyPropertyChanged();

                if (_Date1 > _Date2)
                {
                    _Date1 = _Date2;
                    NotifyPropertyChanged(nameof(Date1));
                }
                else if (_Date2 - Date1 > TimeSpan.FromDays(30))
                {
                    _Date1 = _Date2 - TimeSpan.FromDays(30);
                    NotifyPropertyChanged(nameof(Date1));
                }

                UpdateResults(_Date1, _Date2);
            }
        }

        public int FilterIndex
        {
            get => _FilterIndex;
            set
            {
                _FilterIndex = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ViewResults));

                UpdateChart(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     篩選的開始時間點(RAM)
        /// </summary>
        public int Index1
        {
            get => _Index1;
            set
            {
                _Index1 = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(LowerDate));
                NotifyPropertyChanged(nameof(ViewResults));
                NotifyPropertyChanged(nameof(EnumFilter));

                UpdateChart(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     篩選的結束時間點(RAM)
        /// </summary>
        public int Index2
        {
            get => _Index2;
            set
            {
                _Index2 = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(UpperDate));
                NotifyPropertyChanged(nameof(ViewResults));
                NotifyPropertyChanged(nameof(EnumFilter));

                UpdateChart(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     資料庫查詢結果
        /// </summary>
        public List<ProcessInfo> Results
        {
            get => _Results;
            set
            {
                _Results = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TotalCount));
                NotifyPropertyChanged(nameof(ViewResults));
                NotifyPropertyChanged(nameof(EnumFilter));

                UpdateChart(_Date1, _Date2);
            }
        }

        public bool Standby
        {
            get => _Standby;
            set
            {
                _Standby = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     新增至資料庫
        /// </summary>
        /// <param name="index">PLC序號，由0開始</param>
        /// <param name="info">記錄資訊</param>
        /// <param name="dateTime">紀錄時間，預設為當下時間</param>
        public async void AddToDB(int index, ProcessInfo info, DateTime dateTime = default(DateTime))
        {
            info.StationNumber = index;
            info.AddedTime = dateTime == default(DateTime) ? DateTime.Now : dateTime;

            try
            {
                var db = Mongo_Client.GetDatabase("GP");
                var Sets = db.GetCollection<ProcessInfo>("Product_Infos");

                await Sets.InsertOneAsync(info);
            }
            catch (Exception ex)
            {
                ErrorRecoder.RecordError(ex, "生產紀錄寫入資料庫失敗");
            }
        }

        /// <summary>
        ///     更新統計圖
        /// </summary>
        /// <param name="date1">開始日期</param>
        /// <param name="date2">結束日期</param>
        public void UpdateChart(DateTime date1, DateTime date2)
        {
            ResultView.Series.Clear();
            categoryAxis1.ActualLabels.Clear();
            categoryAxis2.ActualLabels.Clear();

            if (Results?.Count > 0)
            {
                var cs = new ColumnSeries
                         {
                             FontSize = 10,
                             LabelFormatString = "{0}",
                             TextColor = fontcolor,
                             IsStacked = false,
                             StrokeThickness = 1,
                             StrokeColor = OxyColor.FromArgb(0, 0, 0, 0),
                             FillColor = OxyColor.FromArgb(0, 0, 0, 0),
                             XAxisKey = "2"
                         };
                var ByDate = date2 - date1 > TimeSpan.FromDays(1);

                var dates = new List<DateTime>();
                var hours = new List<int>();
                var PLCs = new List<int>();

                if (_ByPLC && _FilterIndex == -1)
                {
                    var result = ViewResults.GroupBy(x => x.StationNumber).OrderBy(x => x.Key).Select(x => (x.Key, x.Sum(y => y.ProcessCount)));

                    foreach (var (plc, count) in result)
                    {
                        PLCs.Add(plc);
                        categoryAxis1.ActualLabels.Add((plc + 1).ToString());
                        cs.Items.Add(new ColumnItem(count));
                    }
                }
                else if (ByDate)
                {
                    var result = ViewResults.GroupBy(x => x.AddedTime.Date).OrderBy(x => x.Key).Select(x => (x.Key, x.Sum(y => y.ProcessCount)));

                    foreach (var (date, count) in result)
                    {
                        dates.Add(date);
                        categoryAxis1.ActualLabels.Add(date.ToString("dd") + "th");
                        cs.Items.Add(new ColumnItem(count));
                    }
                }
                else
                {
                    var result = ViewResults.GroupBy(x => x.AddedTime.Hour).OrderBy(x => x.Key).Select(x => (x.Key, x.Sum(y => y.ProcessCount)));

                    foreach (var (hour, count) in result)
                    {
                        hours.Add(hour);
                        categoryAxis1.ActualLabels.Add(hour + ":00");
                        cs.Items.Add(new ColumnItem(count));
                    }
                }

                ResultView.Series.Add(cs);

                var result2 = ViewResults.GroupBy(x => x.ProduceCode).OrderBy(x => x.Key).Select(x => (x.Key, x)).ToArray();
                var color_step = 0.9 / result2.Length;

                for (var i = 0; i < result2.Length; i++)
                {
                    var (produceCode, info) = result2[i];
                    var ccs = new ColumnSeries
                              {
                                  FontSize = 10,
                                  LabelFormatString = "{0}",
                                  LabelPlacement = LabelPlacement.Middle,
                                  TextColor = OxyColors.White,
                                  Title = produceCode,
                                  IsStacked = true,
                                  StrokeThickness = 0,
                                  StrokeColor = bordercolor,
                                  FillColor = OxyColor.FromHsv(i * color_step, 1, 1),
                                  XAxisKey = "1"
                              };

                    if (_ByPLC && _FilterIndex == -1)
                    {
                        for (var j = 0; j < PLCs.Count; j++)
                        {
                            var val = info.Where(x => x.StationNumber == PLCs[j]).Sum(x => x.ProcessCount);
                            if (val > 0)
                            {
                                ccs.Items.Add(new ColumnItem(val, j));
                            }
                        }
                    }
                    else if (ByDate)
                    {
                        for (var j = 0; j < dates.Count; j++)
                        {
                            var val = info.Where(x => x.AddedTime.Date == dates[j]).Sum(x => x.ProcessCount);
                            if (val > 0)
                            {
                                ccs.Items.Add(new ColumnItem(val, j));
                            }
                        }
                    }
                    else
                    {
                        for (var j = 0; j < hours.Count; j++)
                        {
                            var val = info.Where(x => x.AddedTime.Hour == hours[j]).Sum(x => x.ProcessCount);
                            if (val > 0)
                            {
                                ccs.Items.Add(new ColumnItem(val, j));
                            }
                        }
                    }

                    ResultView.Series.Add(ccs);
                }
            }

            ResultView.InvalidatePlot(true);
        }

        /// <summary>
        ///     依據條件，更新查詢資料庫結果列表
        /// </summary>
        /// <param name="date1">起始時間</param>
        /// <param name="date2">結束時間</param>
        public async void UpdateResults(DateTime date1, DateTime date2)
        {
            Standby = false;

            try
            {
                var db = Mongo_Client.GetDatabase("GP");
                var Sets = db.GetCollection<ProcessInfo>("Product_Infos");

                Results = await (await Sets.FindAsync(x => x.AddedTime >= date1 && x.AddedTime < date2.AddDays(1))).ToListAsync();
            }
            catch (Exception)
            {
            }

            Standby = true;
        }

        public TraceabilityView_ViewModel(MongoClient mongo)
        {
            Mongo_Client = mongo;

            void Act()
            {
                NotifyPropertyChanged(nameof(Date1));
                NotifyPropertyChanged(nameof(Date2));

                UpdateResults(_Date1, _Date2);
            }

            SubDayCommand = new RelayCommand(o =>
                                             {
                                                 _Date1 = _Date1.AddDays(-1);
                                                 _Date2 = _Date1;

                                                 Act();
                                             });

            ThisDayCommand = new RelayCommand(o =>
                                              {
                                                  _Date1 = DateTime.Today.Date;
                                                  _Date2 = _Date1;

                                                  Act();
                                              });

            AddDayCommand = new RelayCommand(o =>
                                             {
                                                 _Date1 = _Date1.AddDays(1);
                                                 _Date2 = _Date1;

                                                 Act();
                                             });

            SubWeekCommand = new RelayCommand(o =>
                                              {
                                                  _Date1 = _Date1.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
                                                  _Date2 = _Date1.AddDays(6);

                                                  Act();
                                              });

            ThisWeekCommand = new RelayCommand(o =>
                                               {
                                                   _Date1 = DateTime.Today.Date.StartOfWeek(DayOfWeek.Monday);
                                                   _Date2 = _Date1.AddDays(6);

                                                   Act();
                                               });

            AddWeekCommand = new RelayCommand(o =>
                                              {
                                                  _Date1 = _Date1.StartOfWeek(DayOfWeek.Monday).AddDays(7);
                                                  _Date2 = _Date1.AddDays(6);

                                                  Act();
                                              });

            SubMonthCommand = new RelayCommand(o =>
                                               {
                                                   _Date1 = new DateTime(_Date1.Year, _Date1.Month - 1, 1);
                                                   _Date2 = _Date1.AddMonths(1).AddDays(-1);

                                                   Act();
                                               });
            ThisMonthCommand = new RelayCommand(o =>
                                                {
                                                    _Date1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                                                    _Date2 = _Date1.AddMonths(1).AddDays(-1);

                                                    Act();
                                                });

            AddMonthCommand = new RelayCommand(o =>
                                               {
                                                   _Date1 = new DateTime(_Date1.Year, _Date1.Month + 1, 1);
                                                   _Date2 = _Date1.AddMonths(1).AddDays(-1);

                                                   Act();
                                               });

            ResultView = new PlotModel
                         {
                             DefaultFont = "Microsoft JhengHei",
                             PlotAreaBorderColor = bordercolor,
                             PlotAreaBorderThickness = new OxyThickness(0, 1, 1, 0),
                             PlotMargins = new OxyThickness(50, 10, 10, 40),
                             LegendTitle = nameof(ProcessInfo.ProduceCode),
                             LegendTitleColor = fontcolor,
                             LegendTextColor = fontcolor,
                             LegendBorder = bordercolor,
                             LegendBackground = OxyColor.FromArgb(0, 0, 0, 0),
                             LegendPlacement = LegendPlacement.Outside,
                             LegendPosition = LegendPosition.RightTop,
                             LegendOrientation = LegendOrientation.Vertical,
                             LegendFontSize = 14,
                             LegendItemOrder = LegendItemOrder.Reverse
                         };

            var linearAxis = new LinearAxis
                             {
                                 FontSize = 10,
                                 TitleColor = fontcolor,
                                 TickStyle = TickStyle.Inside,
                                 MajorGridlineStyle = LineStyle.None,
                                 //MajorStep = 100,
                                 MinorGridlineStyle = LineStyle.None,
                                 MinorTickSize = 0,
                                 //MinorStep = 10,
                                 AxislineStyle = LineStyle.Solid,
                                 AxislineColor = bordercolor,
                                 MajorGridlineColor = bordercolor,
                                 MinorGridlineColor = bordercolor,
                                 TicklineColor = bordercolor,
                                 ExtraGridlineColor = bordercolor,
                                 TextColor = fontcolor,
                                 Minimum = 0,
                                 MaximumPadding = 0.1
                                 //Maximum = 1000
                             };

            categoryAxis1 = new CategoryAxis
                            {
                                FontSize = 10,
                                TitleColor = fontcolor,
                                MajorGridlineColor = bordercolor,
                                MinorGridlineColor = bordercolor,
                                TicklineColor = bordercolor,
                                ExtraGridlineColor = bordercolor,
                                TextColor = fontcolor,
                                TickStyle = TickStyle.Inside,
                                MajorTickSize = 0,
                                MinorTickSize = 0,
                                AxislineStyle = LineStyle.Solid,
                                AxislineColor = bordercolor,
                                GapWidth = 0.6,
                                MinorStep = 1,
                                MajorStep = 1,
                                Position = AxisPosition.Bottom,
                                Key = "1"
                            };

            categoryAxis2 = new CategoryAxis
                            {
                                FontSize = 10,
                                TitleColor = fontcolor,
                                MajorGridlineColor = bordercolor,
                                MinorGridlineColor = bordercolor,
                                TicklineColor = bordercolor,
                                ExtraGridlineColor = bordercolor,
                                TextColor = fontcolor,
                                TickStyle = TickStyle.Inside,
                                MajorTickSize = 0,
                                MinorTickSize = 0,
                                AxislineStyle = LineStyle.Solid,
                                AxislineColor = bordercolor,
                                GapWidth = 1,
                                MinorStep = 1,
                                MajorStep = 1,
                                Position = AxisPosition.Bottom,
                                IsAxisVisible = false,
                                Key = "2"
                            };

            ResultView = new PlotModel
                         {
                             DefaultFont = "Microsoft JhengHei",
                             PlotAreaBorderColor = bordercolor,
                             PlotAreaBorderThickness = new OxyThickness(0, 1, 1, 0),
                             PlotMargins = new OxyThickness(30, 0, 0, 10),
                             LegendTitle = nameof(ProcessInfo.ProduceCode),
                             LegendTitleColor = fontcolor,
                             LegendTextColor = fontcolor,
                             LegendBorder = bordercolor,
                             LegendBackground = OxyColor.FromArgb(0, 0, 0, 0),
                             LegendPlacement = LegendPlacement.Outside,
                             LegendPosition = LegendPosition.RightTop,
                             LegendOrientation = LegendOrientation.Vertical,
                             LegendFontSize = 12,
                             LegendItemOrder = LegendItemOrder.Reverse
                         };

            ResultView.Axes.Add(linearAxis);
            ResultView.Axes.Add(categoryAxis2);
            ResultView.Axes.Add(categoryAxis1);
        }

        //todo  輸出excel
    }
}