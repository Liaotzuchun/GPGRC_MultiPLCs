using System;
using System.Collections.Generic;
using System.Linq;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using MongoDB.Driver;

namespace GPGO_MultiPLCs.ViewModels
{
    public class LogView_ViewModel : ViewModelBase
    {
        public Language Language = Language.TW;
        private readonly IMongoCollection<LogEvent> EventCollection;

        private DateTime _Date1;
        private DateTime _Date2;
        private int _Index1;
        private int _Index2;
        private FilterGroup _OvenFilter;
        private List<LogEvent> _Results;
        private bool _Standby = true;
        private FilterGroup _TypeFilter;
        private List<LogEvent> _ViewResults;

        /// <summary>位移+1天</summary>
        public RelayCommand AddDayCommand { get; }

        /// <summary>位移+1月</summary>
        public RelayCommand AddMonthCommand { get; }

        /// <summary>位移+1週</summary>
        public RelayCommand AddWeekCommand { get; }

        /// <summary>日期範圍的開始</summary>
        public DateTime? LowerDate => _Results?.Count > 0 ? _Results[_Index1]?.Time : null;

        /// <summary>位移-1天</summary>
        public RelayCommand SubDayCommand { get; }

        /// <summary>位移-1月</summary>
        public RelayCommand SubMonthCommand { get; }

        /// <summary>位移-1週</summary>
        public RelayCommand SubWeekCommand { get; }

        /// <summary>指定至本月</summary>
        public RelayCommand ThisMonthCommand { get; }

        /// <summary>指定至本週</summary>
        public RelayCommand ThisWeekCommand { get; }

        /// <summary>指定至本日</summary>
        public RelayCommand TodayCommand { get; }

        /// <summary>輸出Excel報表</summary>
        public RelayCommand ToExcelCommand { get; }

        public int TotalCount => _Results?.Count > 0 ? _Results.Count - 1 : 0;

        /// <summary>日期範圍的結束</summary>
        public DateTime? UpperDate => _Results?.Count > 0 ? _Results[_Index2]?.Time : null;

        /// <summary>選取的開始日期(資料庫)</summary>
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

        /// <summary>選取的結束日期(資料庫)</summary>
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

        /// <summary>篩選的開始時間點(RAM)</summary>
        public int Index1
        {
            get => _Index1;
            set
            {
                _Index1 = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(LowerDate));

                UpdateViewResult();
            }
        }

        /// <summary>篩選的結束時間點(RAM)</summary>
        public int Index2
        {
            get => _Index2;
            set
            {
                _Index2 = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(UpperDate));

                UpdateViewResult();
            }
        }

        /// <summary>基於PLC站號的Filter</summary>
        public FilterGroup OvenFilter
        {
            get => _OvenFilter;
            set
            {
                _OvenFilter = value;
                _OvenFilter.StatusChanged += UpdateViewResult;

                NotifyPropertyChanged();
            }
        }

        /// <summary>資料庫查詢結果</summary>
        public List<LogEvent> Results
        {
            get => _Results;
            set
            {
                _Results = value;
                OvenFilter.Filter = _Results?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                TypeFilter.Filter = _Results?.Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TotalCount));

                UpdateViewResult();
            }
        }

        /// <summary>辨別是否處在讀取資料中</summary>
        public bool Standby
        {
            get => _Standby;
            set
            {
                _Standby = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>基於事件類型的Filter</summary>
        public FilterGroup TypeFilter
        {
            get => _TypeFilter;
            set
            {
                _TypeFilter = value;
                _TypeFilter.StatusChanged += UpdateViewResult;

                NotifyPropertyChanged();
            }
        }

        /// <summary>顯示的資料列表</summary>
        public List<LogEvent> ViewResults
        {
            get => _ViewResults;
            set
            {
                _ViewResults = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>新增至資料庫</summary>
        /// <param name="ev">紀錄資訊</param>
        /// <param name="UpdateResult">決定是否更新Ram Data</param>
        public async void AddToDB(LogEvent ev, bool UpdateResult = false)
        {
            try
            {
                await EventCollection.InsertOneAsync(ev);

                if (UpdateResult)
                {
                    Results = await (await EventCollection.FindAsync(x => x.Time >= _Date1 && x.Time < _Date2.AddDays(1))).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorRecoder.RecordError(ex, "事件紀錄寫入資料庫失敗");
            }
        }

        /// <summary>依據條件，更新查詢資料庫結果列表</summary>
        /// <param name="date1">起始時間</param>
        /// <param name="date2">結束時間</param>
        public async void UpdateResults(DateTime date1, DateTime date2)
        {
            Standby = false;

            try
            {
                Results = await (await EventCollection.FindAsync(x => x.Time >= date1 && x.Time < date2.AddDays(1))).ToListAsync();
            }
            catch (Exception)
            {
            }

            Standby = true;
        }

        private void UpdateViewResult()
        {
            ViewResults = _Index2 >= _Index1 && _Results?.Count > 0 ?
                              _Results?.GetRange(_Index1, _Index2 - _Index1 + 1).Where(x => _OvenFilter.Check(x.StationNumber) && _TypeFilter.Check(x.Type)).ToList() : null;
        }

        public LogView_ViewModel(IMongoCollection<LogEvent> mongo)
        {
            EventCollection = mongo;

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

            TodayCommand = new RelayCommand(o =>
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

            ToExcelCommand = new RelayCommand(o =>
                                              {
                                              });

            OvenFilter = new FilterGroup();
            TypeFilter = new FilterGroup();
        }
    }
}