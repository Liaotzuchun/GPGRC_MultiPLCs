using System;
using System.Collections.Generic;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    public interface ILogData
    {
        DateTime AddedTime{ get; set; }
    }

    public class DataCollectionByDate<T> : ObservableObject where T : ILogData, new()
    {
        public readonly IDataBase<T> DataCollection;

        /// <summary>位移+1天</summary>
        public RelayCommand AddDayCommand { get; }

        /// <summary>位移+1月</summary>
        public RelayCommand AddMonthCommand { get; }

        /// <summary>位移+1週</summary>
        public RelayCommand AddWeekCommand { get; }

        /// <summary>日期範圍的開始</summary>
        public DateTime? BeginTime => Results?.Count > 0 ? Results[BeginIndex]?.AddedTime : null;

        /// <summary>日期範圍的結束</summary>
        public DateTime? EndTime => Results?.Count > 0 ? Results[EndIndex]?.AddedTime : null;

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

        /// <summary>篩選的開始時間點(RAM)</summary>
        public int BeginIndex
        {
            get => Get<int>();
            set
            {
                Set(value);
                NotifyPropertyChanged(nameof(BeginTime));

                BeginIndexChanged?.Invoke(value);
            }
        }

        /// <summary>選取的開始日期(資料庫)</summary>
        public DateTime Date1
        {
            get => Get<DateTime>();
            set
            {
                Set(value);
                Set(value, nameof(Date2));

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

        /// <summary>篩選的結束時間點(RAM)</summary>
        public int EndIndex
        {
            get => Get<int>();
            set
            {
                Set(value);
                NotifyPropertyChanged(nameof(EndTime));

                EndIndexChanged?.Invoke(value);
            }
        }

        /// <summary>資料庫查詢結果</summary>
        public List<T> Results
        {
            get => Get<List<T>>();
            set
            {
                Set(value);

                ResultsChanged?.Invoke(value);
            }
        }

        /// <summary>辨別是否處在讀取資料中</summary>
        public bool Standby
        {
            get => Get<bool>();
            set => Set(value);
        }

        public event Action<int> BeginIndexChanged;
        public event Action<int> EndIndexChanged;
        public event Action<List<T>> ResultsChanged;

        /// <summary>依據條件，更新查詢資料庫結果列表</summary>
        /// <param name="date1">起始時間</param>
        /// <param name="date2">結束時間</param>
        public async void UpdateResults(DateTime date1, DateTime date2)
        {
            Standby = false;

            try
            {
                Results = await DataCollection.FindAsync(x => x.AddedTime >= date1 && x.AddedTime < date2.AddDays(1));
            }
            catch
            {
            }

            Standby = true;
        }

        public DataCollectionByDate(IDataBase<T> db)
        {
            DataCollection = db;

            void Act()
            {
                NotifyPropertyChanged(nameof(Date1));
                NotifyPropertyChanged(nameof(Date2));

                UpdateResults(Date1, Date2);
            }

            SubDayCommand = new RelayCommand(o =>
                                             {
                                                 Set(Date1.AddDays(-1), nameof(Date1));
                                                 Set(Date1, nameof(Date2));

                                                 Act();
                                             });

            TodayCommand = new RelayCommand(o =>
                                            {
                                                Set(DateTime.Today.Date, nameof(Date1));
                                                Set(Date1, nameof(Date2));

                                                Act();
                                            });

            AddDayCommand = new RelayCommand(o =>
                                             {
                                                 Set(Date1.AddDays(1), nameof(Date1));
                                                 Set(Date1, nameof(Date2));

                                                 Act();
                                             });

            SubWeekCommand = new RelayCommand(o =>
                                              {
                                                  Set(Date1.StartOfWeek(DayOfWeek.Monday).AddDays(-7), nameof(Date1));
                                                  Set(Date1.AddDays(6), nameof(Date2));

                                                  Act();
                                              });

            ThisWeekCommand = new RelayCommand(o =>
                                               {
                                                   Set(DateTime.Today.Date.StartOfWeek(DayOfWeek.Monday), nameof(Date1));
                                                   Set(Date1.AddDays(6), nameof(Date2));

                                                   Act();
                                               });

            AddWeekCommand = new RelayCommand(o =>
                                              {
                                                  Set(Date1.StartOfWeek(DayOfWeek.Monday).AddDays(7), nameof(Date1));
                                                  Set(Date1.AddDays(6), nameof(Date2));

                                                  Act();
                                              });

            SubMonthCommand = new RelayCommand(o =>
                                               {
                                                   Set(new DateTime(Date1.Year, Date1.Month - 1, 1), nameof(Date1));
                                                   Set(Date1.AddMonths(1).AddDays(-1), nameof(Date2));

                                                   Act();
                                               });
            ThisMonthCommand = new RelayCommand(o =>
                                                {
                                                    Set(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), nameof(Date1));
                                                    Set(Date1.AddMonths(1).AddDays(-1), nameof(Date2));

                                                    Act();
                                                });

            AddMonthCommand = new RelayCommand(o =>
                                               {
                                                   Set(new DateTime(Date1.Year, Date1.Month + 1, 1), nameof(Date1));
                                                   Set(Date1.AddMonths(1).AddDays(-1), nameof(Date2));

                                                   Act();
                                               });
        }
    }
}