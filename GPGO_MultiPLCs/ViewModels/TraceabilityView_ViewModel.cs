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
        private readonly CategoryAxis categoryAxis_Single;
        private readonly CategoryAxis categoryAxis_Total;
        private readonly OxyColor fontcolor = OxyColor.FromRgb(50, 70, 60);
        private readonly MongoClient Mongo_Client;
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
        public PlotModel ResultView_SingleVolume { get; }
        public PlotModel ResultView_TotalVolume { get; }

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
                    _Date2 = Date1;
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

                if (_Date1 > Date2)
                {
                    _Date1 = Date2;
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
            ResultView_TotalVolume.Series.Clear();
            ResultView_SingleVolume.Series.Clear();
            categoryAxis_Total.ActualLabels.Clear();
            categoryAxis_Single.ActualLabels.Clear();

            if (Results?.Count > 0)
            {
                if (date2 - date1 > TimeSpan.FromDays(7))
                {
                    var vals = new ColumnSeries { IsStacked = false, StrokeThickness = 1, StrokeColor = bordercolor, FillColor = OxyColors.Cyan };

                    if (_FilterIndex == -1)
                    {
                        var result = ViewResults.GroupBy(x => x.StationNumber).OrderBy(x => x.Key).Select(x => (x.Key, x.Sum(y => y.ProcessCount)));

                        foreach (var (index, count) in result)
                        {
                            categoryAxis_Total.ActualLabels.Add((index + 1).ToString());
                            vals.Items.Add(new ColumnItem(count));
                        }

                        ResultView_TotalVolume.Series.Add(vals);
                    }
                    else
                    {
                        var result = ViewResults.GroupBy(x => x.AddedTime.Date).OrderBy(x => x.Key).Select(x => (x.Key, x.Sum(y => y.ProcessCount)));

                        foreach (var (date, count) in result)
                        {
                            categoryAxis_Single.ActualLabels.Add(date.ToString("dd"));
                            vals.Items.Add(new ColumnItem(count));
                        }

                        ResultView_SingleVolume.Series.Add(vals);
                    }
                }
                else if (date2 - date1 > TimeSpan.FromDays(1))
                {
                    if (_FilterIndex == -1)
                    {
                        var stations = EnumFilter.ToArray();

                        foreach (var s in stations)
                        {
                            categoryAxis_Total.ActualLabels.Add((s + 1).ToString());
                        }

                        var groups = ViewResults.GroupBy(x => x.ProduceCode).OrderBy(x => x.Key).Select(x => (x.Key, x)).ToArray();
                        var color_step = 0.9 / groups.Length;

                        for (var index = 0; index < groups.Length; index++)
                        {
                            var (produceCode, info) = groups[index];
                            var vals = new ColumnSeries
                                       {
                                           Title = produceCode,
                                           IsStacked = true,
                                           StrokeThickness = 0,
                                           StrokeColor = bordercolor,
                                           FillColor = OxyColor.FromHsv(index * color_step, 1, 1)
                                       };

                            for (var i = 0; i < stations.Length; i++)
                            {
                                vals.Items.Add(new ColumnItem(info.Where(x => x.StationNumber == stations[i]).Sum(x => x.ProcessCount), i));
                            }

                            ResultView_TotalVolume.Series.Add(vals);
                        }
                    }
                    else
                    {
                        var dates = ViewResults.Select(x => x.AddedTime.Date).Distinct().OrderBy(x => x).ToArray();

                        foreach (var s in dates)
                        {
                            categoryAxis_Single.ActualLabels.Add(s.ToString("MM/dd"));
                        }

                        var groups = ViewResults.GroupBy(x => x.ProduceCode).OrderBy(x => x.Key).Select(x => (x.Key, x)).ToArray();
                        var color_step = 0.9 / groups.Length;

                        for (var index = 0; index < groups.Length; index++)
                        {
                            var (produceCode, info) = groups[index];
                            var vals = new ColumnSeries
                                       {
                                           Title = produceCode,
                                           IsStacked = true,
                                           StrokeThickness = 0,
                                           StrokeColor = bordercolor,
                                           FillColor = OxyColor.FromHsv(index * color_step, 1, 1)
                                       };

                            for (var i = 0; i < dates.Length; i++)
                            {
                                vals.Items.Add(new ColumnItem(info.Where(x => x.AddedTime.Date == dates[i]).Sum(x => x.ProcessCount), i));
                            }

                            ResultView_SingleVolume.Series.Add(vals);
                        }
                    }
                }
                else
                {
                    if (_FilterIndex == -1)
                    {
                        var stations = EnumFilter.ToArray();

                        foreach (var s in stations)
                        {
                            categoryAxis_Total.ActualLabels.Add((s + 1).ToString());
                        }

                        var groups = ViewResults.GroupBy(x => x.ProduceCode).OrderBy(x => x.Key).Select(x => (x.Key, x)).ToArray();
                        var color_step = 0.9 / groups.Length;

                        for (var index = 0; index < groups.Length; index++)
                        {
                            var (produceCode, info) = groups[index];
                            var vals = new ColumnSeries
                                       {
                                           Title = produceCode,
                                           IsStacked = true,
                                           StrokeThickness = 0,
                                           StrokeColor = bordercolor,
                                           FillColor = OxyColor.FromHsv(index * color_step, 1, 1)
                                       };

                            for (var i = 0; i < stations.Length; i++)
                            {
                                vals.Items.Add(new ColumnItem(info.Where(x => x.StationNumber == stations[i]).Sum(x => x.ProcessCount), i));
                            }

                            ResultView_TotalVolume.Series.Add(vals);
                        }
                    }
                    else
                    {
                        var dates = ViewResults.Select(x => (date: x.AddedTime.Date, hour: x.AddedTime.Hour)).Distinct().OrderBy(x => x).ToArray();

                        foreach (var (date, hour) in dates)
                        {
                            categoryAxis_Single.ActualLabels.Add(date.ToString("dd") + "日" + hour + "時");
                        }

                        var groups = ViewResults.GroupBy(x => x.ProduceCode).OrderBy(x => x.Key).Select(x => (x.Key, x)).ToArray();
                        var color_step = 0.9 / groups.Length;

                        for (var index = 0; index < groups.Length; index++)
                        {
                            var (produceCode, info) = groups[index];
                            var vals = new ColumnSeries
                                       {
                                           Title = produceCode,
                                           IsStacked = true,
                                           StrokeThickness = 0,
                                           StrokeColor = bordercolor,
                                           FillColor = OxyColor.FromHsv(index * color_step, 1, 1)
                                       };

                            for (var i = 0; i < dates.Length; i++)
                            {
                                vals.Items.Add(new ColumnItem(info.Where(x => x.AddedTime.Date == dates[i].date && x.AddedTime.Hour == dates[i].hour).Sum(x => x.ProcessCount), i));
                            }

                            ResultView_SingleVolume.Series.Add(vals);
                        }
                    }
                }
            }

            ResultView_TotalVolume.InvalidatePlot(true);
            ResultView_SingleVolume.InvalidatePlot(true);
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

            ResultView_TotalVolume = new PlotModel
                                     {
                                         DefaultFont = "Microsoft JhengHei",
                                         PlotAreaBorderColor = bordercolor,
                                         PlotAreaBorderThickness = new OxyThickness(0, 1, 1, 0),
                                         PlotMargins = new OxyThickness(50, 10, 10, 40),
                                         LegendTextColor = fontcolor,
                                         LegendBorder = bordercolor,
                                         LegendBackground = OxyColor.FromArgb(0, 0, 0, 0),
                                         LegendPlacement = LegendPlacement.Outside,
                                         LegendPosition = LegendPosition.RightTop,
                                         LegendOrientation = LegendOrientation.Vertical,
                                         LegendFontSize = 14,
                                         LegendItemOrder = LegendItemOrder.Reverse
                                     };

            var linearAxis_Total = new LinearAxis
                                   {
                                       TitleColor = fontcolor,
                                       Title = "數量",
                                       Unit = "片",
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

            var linearAxis_Single = new LinearAxis
                                    {
                                        TitleColor = fontcolor,
                                        Title = "數量",
                                        Unit = "片",
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

            categoryAxis_Total = new CategoryAxis
                                 {
                                     TitleColor = fontcolor,
                                     Title = "烤箱",
                                     Unit = "站",
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
                                     Position = AxisPosition.Bottom
                                 };

            categoryAxis_Single = new CategoryAxis
                                  {
                                      TitleColor = fontcolor,
                                      Title = "日期/時間",
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
                                      Position = AxisPosition.Bottom
                                  };

            ResultView_SingleVolume = new PlotModel
                                      {
                                          DefaultFont = "Microsoft JhengHei",
                                          PlotAreaBorderColor = bordercolor,
                                          PlotAreaBorderThickness = new OxyThickness(0, 1, 1, 0),
                                          PlotMargins = new OxyThickness(50, 10, 10, 40),
                                          LegendTextColor = fontcolor,
                                          LegendBorder = bordercolor,
                                          LegendBackground = OxyColor.FromArgb(0, 0, 0, 0),
                                          LegendPlacement = LegendPlacement.Outside,
                                          LegendPosition = LegendPosition.RightTop,
                                          LegendOrientation = LegendOrientation.Vertical,
                                          LegendFontSize = 14,
                                          LegendItemOrder = LegendItemOrder.Reverse
                                      };

            ResultView_TotalVolume.Axes.Add(linearAxis_Total);
            ResultView_TotalVolume.Axes.Add(categoryAxis_Total);
            ResultView_SingleVolume.Axes.Add(linearAxis_Single);
            ResultView_SingleVolume.Axes.Add(categoryAxis_Single);
        }

        //todo  輸出excel
    }
}