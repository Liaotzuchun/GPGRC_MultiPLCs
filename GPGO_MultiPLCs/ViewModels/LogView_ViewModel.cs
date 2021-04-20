using GPGO_MultiPLCs.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPMVVM.Helpers;
using GPMVVM.Models;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>紀錄/檢視系統事件</summary>
    public class LogView_ViewModel : DataCollectionByDate<LogEvent>
    {
        public Language Language = Language.TW;

        /// <summary>執行詳情顯示</summary>
        public RelayCommand GoCommand { get; }

        /// <summary>基於PLC站號的Filter</summary>
        public FilterGroup OvenFilter { get; }

        /// <summary>輸出Excel報表</summary>
        public RelayCommand ToFileCommand { get; }

        /// <summary>基於事件類型的Filter</summary>
        public FilterGroup TypeFilter { get; }

        /// <summary>事件值為On，所選擇的index</summary>
        public int SelectedIndex1
        {
            get => Get<int>();
            set
            {
                Set(value);

                if (value <= -1)
                {
                    SelectedProcessInfo = null;

                    return;
                }

                FindNextOFF(value);
            }
        }

        /// <summary>事件值為Off，所選擇的index</summary>
        public int SelectedIndex2
        {
            get => Get<int>();
            set
            {
                Set(value);

                if (value <= -1)
                {
                    SelectedProcessInfo = null;

                    return;
                }

                FindNextON(value);
            }
        }

        public ProcessInfo SelectedProcessInfo
        {
            get => Get<ProcessInfo>();
            set => Set(value);
        }

        /// <summary>顯示的資料列表</summary>
        public List<LogEvent> ViewResults
        {
            get => Get<List<LogEvent>>();
            set
            {
                Set(value);

                ViewResults_On  = value?.Where(x => x.Value is bool and true).ToList();
                ViewResults_Off = value?.Where(x => x.Value is bool and false).ToList();
            }
        }

        public List<LogEvent> ViewResults_Off
        {
            get => Get<List<LogEvent>>();
            set => Set(value);
        }

        public List<LogEvent> ViewResults_On
        {
            get => Get<List<LogEvent>>();
            set => Set(value);
        }

        public event Action<(ProcessInfo info, LogEvent _event)> GoDetailView;

        public event Func<(int station, DateTime time), ValueTask<ProcessInfo>> WantInfo;

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
                Log.Error(ex, "事件紀錄寫入資料庫失敗");
            }
        }

        public async void FindNextOFF(int index)
        {
            Standby = false;

            var _on = ViewResults_On[index];

            await Task.Factory.StartNew(() =>
                                        {
                                            for (var i = 0; i < ViewResults_Off.Count; i++)
                                            {
                                                var off = ViewResults_Off[i];
                                                if (off.AddedTime > _on.AddedTime && off.StationNumber == _on.StationNumber && off.Type == _on.Type && off.Description == _on.Description)
                                                {
                                                    Set(off.Value == _on.Value ? -1 : i, nameof(SelectedIndex2));

                                                    return;
                                                }
                                            }

                                            Set(-1, nameof(SelectedIndex2));
                                        });

            if (WantInfo != null)
            {
                SelectedProcessInfo = await WantInfo((_on.StationNumber, _on.AddedTime));
            }

            Standby = true;
        }

        public async void FindNextON(int index)
        {
            Standby = false;

            var _off = ViewResults_Off[index];

            await Task.Factory.StartNew(() =>
                                        {
                                            for (var i = 0; i < ViewResults_On.Count; i++)
                                            {
                                                var on = ViewResults_On[i];
                                                if (on.AddedTime < _off.AddedTime && on.StationNumber == _off.StationNumber && on.Type == _off.Type && on.Description == _off.Description)
                                                {
                                                    Set(on.Value == _off.Value ? -1 : i, nameof(SelectedIndex1));

                                                    return;
                                                }
                                            }

                                            Set(-1, nameof(SelectedIndex1));
                                        });

            if (WantInfo != null)
            {
                SelectedProcessInfo = await WantInfo((_off.StationNumber, _off.AddedTime));
            }

            Standby = true;
        }

        public async Task<bool> SaveToCSV(string path)
        {
            Standby = false;

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "CSV輸出資料夾無法創建");

                    return false;
                }
            }

            var csv = ViewResults.ToCSV(Language,
                                        new[]
                                        {
                                            typeof(LogEvent).GetProperty(nameof(LogEvent.AddedTime)), typeof(LogEvent).GetProperty(nameof(LogEvent.StationNumber)), typeof(LogEvent).GetProperty(nameof(LogEvent.Type)), typeof(LogEvent).GetProperty(nameof(LogEvent.Description2)),
                                            typeof(LogEvent).GetProperty(nameof(LogEvent.Value))
                                        });

            try
            {
                using var outputFile = new StreamWriter($"{path}\\{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.csv", false, Encoding.UTF8);
                await outputFile.WriteAsync(csv);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "輸出CSV失敗");

                return false;
            }

            Standby = true;

            return true;
        }

        private void UpdateViewResult()
        {
            ViewResults = EndIndex >= BeginIndex && Results?.Count > 0 ?
                              Results?.GetRange(BeginIndex, EndIndex - BeginIndex + 1)
                                      .Where(x => OvenFilter.Check(x.StationNumber) && TypeFilter.Check(x.Type))
                                      .OrderByDescending(x => x.AddedTime)
                                      .ToList() :
                              null;
        }

        public LogView_ViewModel(IDataBase<LogEvent> db, IDialogService dialog) : base(db)
        {
            ToFileCommand = new RelayCommand(async _ =>
                                             {
                                                 var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\EventLogs";
                                                 if (await SaveToCSV(path))
                                                 {
                                                     dialog?.Show(new Dictionary<Language, string>
                                                                  {
                                                                      {Language.TW, $"檔案已輸出至\n{path}"},
                                                                      {Language.CHS, $"档案已输出至\n{path}"},
                                                                      {Language.EN, $"The file has been output to\n{path}"}
                                                                  },
                                                                  TimeSpan.FromSeconds(6));
                                                 }
                                             });

            GoCommand = new RelayCommand(_ =>
                                         {
                                             if (SelectedProcessInfo == null)
                                             {
                                                 return;
                                             }

                                             GoDetailView?.Invoke((SelectedProcessInfo, SelectedIndex1 > -1 ? ViewResults_On[SelectedIndex1] : ViewResults_Off[SelectedIndex2]));
                                         });

            OvenFilter = new FilterGroup(UpdateViewResult);
            TypeFilter = new FilterGroup(UpdateViewResult);

            ResultsChanged += e =>
                              {
                                  OvenFilter.Filter = e?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                                  TypeFilter.Filter = e?.Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();

                                  UpdateViewResult();
                              };

            BeginIndexChanged += _ =>
                                 {
                                     UpdateViewResult();
                                 };

            EndIndexChanged += _ =>
                               {
                                   UpdateViewResult();
                               };
        }
    }
}