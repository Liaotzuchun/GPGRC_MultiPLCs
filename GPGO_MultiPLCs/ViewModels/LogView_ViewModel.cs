using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>紀錄/檢視系統事件</summary>
    public class LogView_ViewModel : DataCollectionByDate<LogEvent>
    {
        public Language Language = Language.TW;

        /// <summary>日期範圍的開始</summary>
        public DateTime? LowerDate => Results?.Count > 0 ? Results[Index1]?.AddedTime : null;

        /// <summary>基於PLC站號的Filter</summary>
        public FilterGroup OvenFilter { get; }

        /// <summary>輸出Excel報表</summary>
        public RelayCommand ToFileCommand { get; }

        /// <summary>Slider的Maximum值，以0起始，總數-1</summary>
        public int TotalCount => Results?.Count > 0 ? Results.Count - 1 : 0;

        /// <summary>基於事件類型的Filter</summary>
        public FilterGroup TypeFilter { get; }

        /// <summary>日期範圍的結束</summary>
        public DateTime? UpperDate => Results?.Count > 0 ? Results[Index2]?.AddedTime : null;

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
                await DataCollection.AddAsync(ev);

                if (UpdateResult)
                {
                    Results = await DataCollection.FindAsync(x => x.AddedTime >= Date1 && x.AddedTime < Date2.AddDays(1));
                }
            }
            catch (Exception ex)
            {
                ex.RecordError("事件紀錄寫入資料庫失敗");
            }
        }

        public async Task<bool> SaveToCSV(string path)
        {
            Standby = false;

            var result = true;

            await Task.Factory.StartNew(() =>
                                        {
                                            if (!Directory.Exists(path))
                                            {
                                                try
                                                {
                                                    Directory.CreateDirectory(path);
                                                }
                                                catch (Exception ex)
                                                {
                                                    ex.RecordError("CSV輸出資料夾無法創建");
                                                    result = false;

                                                    return;
                                                }
                                            }

                                            var csv = ViewResults.ToCSV(Language,
                                                                        new[]
                                                                        {
                                                                            typeof(LogEvent).GetProperty(nameof(LogEvent.AddedTime)),
                                                                            typeof(LogEvent).GetProperty(nameof(LogEvent.StationNumber)),
                                                                            typeof(LogEvent).GetProperty(nameof(LogEvent.Type)),
                                                                            typeof(LogEvent).GetProperty(nameof(LogEvent.Description))
                                                                        });

                                            try
                                            {
                                                File.WriteAllText(path + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".csv", csv, Encoding.UTF8);
                                            }
                                            catch (Exception ex)
                                            {
                                                ex.RecordError("輸出CSV失敗");
                                                result = false;
                                            }
                                        });

            Standby = true;

            return result;
        }

        private void UpdateViewResult()
        {
            ViewResults = Index2 >= Index1 && Results?.Count > 0 ? Results?.GetRange(Index1, Index2 - Index1 + 1)
                                                                          .Where(x => OvenFilter.Check(x.StationNumber) && TypeFilter.Check(x.Type))
                                                                          .OrderByDescending(x => x.AddedTime)
                                                                          .ToList() : null;
        }

        public LogView_ViewModel(IDataBase<LogEvent> db, IDialogService dialog) : base(db)
        {
            ToFileCommand = new RelayCommand(async o =>
                                             {
                                                 var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\EventLogs";
                                                 if (await SaveToCSV(path))
                                                 {
                                                     dialog?.Show(new Dictionary<Language, string>
                                                                  {
                                                                      { Language.TW, "檔案已輸出至\n" + path }, { Language.CHS, "档案已输出至\n" + path }, { Language.EN, "The file has been output to\n" + path }
                                                                  },
                                                                  TimeSpan.FromSeconds(6));
                                                 }
                                             });

            OvenFilter = new FilterGroup(UpdateViewResult);
            TypeFilter = new FilterGroup(UpdateViewResult);

            ResultsChanged += e =>
                              {
                                  OvenFilter.Filter = e?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                                  TypeFilter.Filter = e?.Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();

                                  NotifyPropertyChanged(nameof(TotalCount));

                                  UpdateViewResult();
                              };
        }
    }
}