using System;
using System.Collections.Generic;
using System.Linq;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.ViewModels
{
    public class LogView_ViewModel : BindableBase
    {
        public Language Language = Language.TW;
        private readonly IDataBase<LogEvent> EventCollection;

        /// <summary>位移+1天</summary>
        public RelayCommand AddDayCommand { get; }

        /// <summary>位移+1月</summary>
        public RelayCommand AddMonthCommand { get; }

        /// <summary>位移+1週</summary>
        public RelayCommand AddWeekCommand { get; }

        /// <summary>日期範圍的開始</summary>
        public DateTime? LowerDate => Results?.Count > 0 ? Results[Index1]?.Time : null;

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

        public int TotalCount => Results?.Count > 0 ? Results.Count - 1 : 0;

        /// <summary>日期範圍的結束</summary>
        public DateTime? UpperDate => Results?.Count > 0 ? Results[Index2]?.Time : null;

        /// <summary>選取的開始日期(資料庫)</summary>
        public DateTime Date1
        {
            get => Get<DateTime>();
            set
            {
                Set(value);

                if (Date2 < value)
                {
                    Set(value, nameof(Date2));
                }
                else if (Date2 - value > TimeSpan.FromDays(30))
                {
                    Set(value + TimeSpan.FromDays(30), nameof(Date2));
                }

                UpdateResults(value, Date2);
            }
        }

        /// <summary>選取的結束日期(資料庫)</summary>
        public DateTime Date2
        {
            get => Get<DateTime>();
            set
            {
                Set(value);

                if (Date1 > value)
                {
                    Set(value, nameof(Date1));
                }
                else if (value - Date1 > TimeSpan.FromDays(30))
                {
                    Set(value - TimeSpan.FromDays(30), nameof(Date1));
                }

                UpdateResults(Date1, value);
            }
        }

        /// <summary>篩選的開始時間點(RAM)</summary>
        public int Index1
        {
            get => Get<int>();
            set
            {
                Set(value);
                NotifyPropertyChanged(nameof(LowerDate));

                UpdateViewResult();
            }
        }

        /// <summary>篩選的結束時間點(RAM)</summary>
        public int Index2
        {
            get => Get<int>();
            set
            {
                Set(value);
                NotifyPropertyChanged(nameof(UpperDate));

                UpdateViewResult();
            }
        }

        /// <summary>基於PLC站號的Filter</summary>
        public FilterGroup OvenFilter
        {
            get => Get<FilterGroup>();
            set
            {
                if (OvenFilter != null)
                {
                    OvenFilter.StatusChanged -= UpdateViewResult;
                }

                Set(value);
                OvenFilter.StatusChanged += UpdateViewResult;
            }
        }

        /// <summary>資料庫查詢結果</summary>
        public List<LogEvent> Results
        {
            get => Get<List<LogEvent>>();
            set
            {
                OvenFilter.Filter = value?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                TypeFilter.Filter = value?.Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();

                Set(value);
                NotifyPropertyChanged(nameof(TotalCount));

                UpdateViewResult();
            }
        }

        /// <summary>辨別是否處在讀取資料中</summary>
        public bool Standby
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>基於事件類型的Filter</summary>
        public FilterGroup TypeFilter
        {
            get => Get<FilterGroup>();
            set
            {
                if (TypeFilter != null)
                {
                    TypeFilter.StatusChanged -= UpdateViewResult;
                }

                Set(value);
                TypeFilter.StatusChanged += UpdateViewResult;
            }
        }

        /// <summary>顯示的資料列表</summary>
        public List<LogEvent> ViewResults
        {
            get => Get<List<LogEvent>>();
            set => Set(value);
        }

        /// <summary>新增至資料庫</summary>
        /// <param name="ev">紀錄資訊</param>
        /// <param name="UpdateResult">決定是否更新Ram Data</param>
        public async void AddToDB(LogEvent ev, bool UpdateResult = false)
        {
            try
            {
                await EventCollection.AddAsync(ev);

                if (UpdateResult)
                {
                    Results = await EventCollection.FindAsync(x => x.Time >= Date1 && x.Time < Date2.AddDays(1));
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
                Results = await EventCollection.FindAsync(x => x.Time >= date1 && x.Time < date2.AddDays(1));
            }
            catch
            {
            }

            Standby = true;
        }

        private void UpdateViewResult()
        {
            ViewResults = Index2 >= Index1 && Results?.Count > 0 ?
                          Results?.GetRange(Index1, Index2 - Index1 + 1).Where(x => OvenFilter.Check(x.StationNumber) && TypeFilter.Check(x.Type)).ToList() : null;
        }

        public LogView_ViewModel(IDataBase<LogEvent> db)
        {
            EventCollection = db;

            void Act()
            {
                NotifyPropertyChanged(nameof(Date1));
                NotifyPropertyChanged(nameof(Date2));

                UpdateResults(Date1, Date2);
            }

            SubDayCommand = new RelayCommand(o =>
                                             {
                                                 Date1 = Date1.AddDays(-1);
                                                 Date2 = Date1;

                                                 Act();
                                             });

            TodayCommand = new RelayCommand(o =>
                                            {
                                                Date1 = DateTime.Today.Date;
                                                Date2 = Date1;

                                                Act();
                                            });

            AddDayCommand = new RelayCommand(o =>
                                             {
                                                 Date1 = Date1.AddDays(1);
                                                 Date2 = Date1;

                                                 Act();
                                             });

            SubWeekCommand = new RelayCommand(o =>
                                              {
                                                  Date1 = Date1.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
                                                  Date2 = Date1.AddDays(6);

                                                  Act();
                                              });

            ThisWeekCommand = new RelayCommand(o =>
                                               {
                                                   Date1 = DateTime.Today.Date.StartOfWeek(DayOfWeek.Monday);
                                                   Date2 = Date1.AddDays(6);

                                                   Act();
                                               });

            AddWeekCommand = new RelayCommand(o =>
                                              {
                                                  Date1 = Date1.StartOfWeek(DayOfWeek.Monday).AddDays(7);
                                                  Date2 = Date1.AddDays(6);

                                                  Act();
                                              });

            SubMonthCommand = new RelayCommand(o =>
                                               {
                                                   Date1 = new DateTime(Date1.Year, Date1.Month - 1, 1);
                                                   Date2 = Date1.AddMonths(1).AddDays(-1);

                                                   Act();
                                               });
            ThisMonthCommand = new RelayCommand(o =>
                                                {
                                                    Date1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                                                    Date2 = Date1.AddMonths(1).AddDays(-1);

                                                    Act();
                                                });

            AddMonthCommand = new RelayCommand(o =>
                                               {
                                                   Date1 = new DateTime(Date1.Year, Date1.Month + 1, 1);
                                                   Date2 = Date1.AddMonths(1).AddDays(-1);

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