using System;
using System.Collections.Generic;
using System.Linq;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using MongoDB.Driver;
using OxyPlot;
using OxyPlot.Axes;

namespace GPGO_MultiPLCs.ViewModels
{
    public class TraceabilityView_ViewModel : ViewModelBase
    {
        private readonly MongoClient Mongo_Client;
        private readonly CategoryAxis StationAxis;
        private readonly DateTimeAxis TimeAxis;
        private readonly LinearAxis VolumeAxis_Single;

        private readonly LinearAxis VolumeAxis_Total;
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
        public List<int> EnumFilter => _Results?.Select(x => x.StationNumber + 1).Distinct().OrderBy(x => x).ToList();

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
            }
        }

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

        public async void AddToDB(int index, ProcessInfo info)
        {
            info.StationNumber = index;
            info.AddedTime = DateTime.Now;

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

            var color = OxyColor.FromRgb(50, 70, 60);

            ResultView_TotalVolume = new PlotModel
                                     {
                                         DefaultFont = "Microsoft JhengHei",
                                         PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0),
                                         PlotMargins = new OxyThickness(50, 0, 30, 40),
                                         LegendTextColor = color,
                                         LegendBackground = OxyColor.FromArgb(0, 0, 0, 0),
                                         LegendPlacement = LegendPlacement.Outside,
                                         LegendPosition = LegendPosition.RightTop,
                                         LegendMaxHeight = 30,
                                         LegendFontSize = 14,
                                         LegendItemOrder = LegendItemOrder.Reverse
                                     };

            VolumeAxis_Total = new LinearAxis
                               {
                                   TitleColor = color,
                                   Title = "數量",
                                   Unit = "片",
                                   TickStyle = TickStyle.Inside,
                                   MajorGridlineStyle = LineStyle.Solid,
                                   MajorStep = 100,
                                   MinorGridlineStyle = LineStyle.None,
                                   MinorTickSize = 0,
                                   MinorStep = 10,
                                   AxislineStyle = LineStyle.Solid,
                                   AxislineColor = color,
                                   MajorGridlineColor = color,
                                   MinorGridlineColor = color,
                                   TicklineColor = color,
                                   ExtraGridlineColor = color,
                                   TextColor = color,
                                   Minimum = 0
                               };

            VolumeAxis_Single = new LinearAxis
                                {
                                    TitleColor = color,
                                    Title = "數量",
                                    Unit = "片",
                                    TickStyle = TickStyle.Inside,
                                    MajorGridlineStyle = LineStyle.Solid,
                                    MajorStep = 100,
                                    MinorGridlineStyle = LineStyle.None,
                                    MinorTickSize = 0,
                                    MinorStep = 10,
                                    AxislineStyle = LineStyle.Solid,
                                    AxislineColor = color,
                                    MajorGridlineColor = color,
                                    MinorGridlineColor = color,
                                    TicklineColor = color,
                                    ExtraGridlineColor = color,
                                    TextColor = color,
                                    Minimum = 0
                                };

            TimeAxis = new DateTimeAxis
                       {
                           TitleColor = color,
                           MinimumPadding = 0,
                           MaximumPadding = 0,
                           TickStyle = TickStyle.Inside,
                           MajorGridlineStyle = LineStyle.Dot,
                           MajorStep = 60,
                           MinorGridlineStyle = LineStyle.None,
                           MinorStep = 10,
                           Position = AxisPosition.Bottom,
                           AxislineStyle = LineStyle.Solid,
                           AxislineColor = color,
                           MajorGridlineColor = color,
                           MinorGridlineColor = color,
                           TicklineColor = color,
                           ExtraGridlineColor = color,
                           TextColor = color,
                           //StringFormat = "hh:mm",
                           Maximum = DateTimeAxis.ToDouble(DateTime.Today.AddDays(1)),
                           Minimum = DateTimeAxis.ToDouble(DateTime.Today)
                       };

            StationAxis = new CategoryAxis
                          {
                              MajorGridlineColor = color,
                              MinorGridlineColor = color,
                              TicklineColor = color,
                              ExtraGridlineColor = color,
                              TextColor = color,
                              TickStyle = TickStyle.Inside,
                              AxislineStyle = LineStyle.Solid,
                              AxislineColor = color,
                              GapWidth = 0,
                              MinorStep = 1,
                              MajorStep = 1,
                              Position = AxisPosition.Bottom
                          };

            for (var i = 1; i <= Connector.PLC_Count; i++)
            {
                StationAxis.ActualLabels.Add("第" + i + "站");
            }

            ResultView_SingleVolume = new PlotModel
                                      {
                                          DefaultFont = "Microsoft JhengHei",
                                          PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0),
                                          PlotMargins = new OxyThickness(50, 0, 30, 40),
                                          LegendTextColor = color,
                                          LegendBackground = OxyColor.FromArgb(0, 0, 0, 0),
                                          LegendPlacement = LegendPlacement.Outside,
                                          LegendPosition = LegendPosition.RightTop,
                                          LegendMaxHeight = 30,
                                          LegendFontSize = 14,
                                          LegendItemOrder = LegendItemOrder.Reverse
                                      };

            ResultView_TotalVolume.Axes.Add(VolumeAxis_Total);
            ResultView_TotalVolume.Axes.Add(StationAxis);
            ResultView_SingleVolume.Axes.Add(VolumeAxis_Single);
            ResultView_SingleVolume.Axes.Add(TimeAxis);
        }

        //todo  輸出excel
    }
}