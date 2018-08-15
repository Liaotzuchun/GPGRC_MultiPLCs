using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using GPGO_MultiPLCs.ViewModels;
using MongoDB.Driver;

namespace GPGO_MultiPLCs
{
    public sealed class Connector : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty DataOutputPathProperty =
            DependencyProperty.Register(nameof(DataOutputPath), typeof(string), typeof(Connector), new PropertyMetadata("", DataOutputPathChanged));

        private static void DataOutputPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //! 
        }

        public string DataOutputPath
        {
            get => (string)GetValue(DataOutputPathProperty);
            set => SetValue(DataOutputPathProperty, value);
        }

        public static readonly DependencyProperty LanguageProperty =
            DependencyProperty.Register(nameof(Language), typeof(Language), typeof(Connector), new PropertyMetadata(Language.TW, LanguageChanged));

        private static void LanguageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var lng = (Language)Enum.Parse(typeof(Language), e.NewValue.ToString());
            ((Connector)sender).DialogVM.Language = lng;
            ((Connector)sender).TraceVM.Language = lng;
            ((Connector)sender).LogVM.Language = lng;
        }

        public Language Language
        {
            get => (Language)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }

        public void Dispose()
        {
            DialogVM.Dispose();
            TotalVM.Dispose();
        }

        public GlobalDialog_ViewModel DialogVM { get; }
        public LogView_ViewModel LogVM { get; }
        public MainWindow_ViewModel MainVM { get; }
        public RecipeControl_ViewModel RecipeVM { get; }
        public TotalView_ViewModel TotalVM { get; }
        public TraceabilityView_ViewModel TraceVM { get; }

        /// <summary>產生測試資料至資料庫</summary>
        /// <param name="PLC_Count"></param>
        public void MakeTestData(int PLC_Count)
        {
            var events = new[]
                         {
                             "自動停止",
                             "程式結束",
                             "電源反相",
                             "循環風車過載",
                             "循環風車INV異常",
                             "第1段恆溫",
                             "第1段升溫",
                             "第2段恆溫",
                             "第2段升溫",
                             "第3段恆溫",
                             "第3段升溫",
                             "第4段恆溫",
                             "第4段升溫",
                             "第5段恆溫",
                             "第5段升溫",
                             "第6段恆溫",
                             "第6段升溫",
                             "第7段恆溫",
                             "第7段升溫",
                             "第8段恆溫",
                             "第8段升溫"
                         };
            var order_code = new[] { "ooxx", "abc", "zzz", "qoo", "boom", "xxx", "wunmao" };
            var time = DateTime.Now;

            for (var j = 1; j <= new DateTime(time.Year, time.Month, 1).AddMonths(1).AddDays(-1).Day; j++)
            {
                for (var i = 0; i < PLC_Count; i++)
                {
                    var rn = new Random(i + j);

                    var st = new DateTime(time.Year, time.Month, j, 8, i + rn.Next(0, 10), i);

                    for (var k = 0; k < 8; k++)
                    {
                        var info = new ProcessInfo { StartTime = st, TrolleyCode = rn.Next(1, 100).ToString("000"), OperatorID = rn.Next(1, 10).ToString("000") };

                        var t = new TimeSpan();
                        for (var m = 0; m < 100; m++)
                        {
                            if (rn.Next(0, 100) > 96)
                            {
                                LogVM.AddToDB(new LogEvent { StationNumber = i + 1, Time = time + t, Description = "", Type = (EventType)rn.Next(0, 4) });
                            }

                            var mins = (int)t.TotalMinutes + 1;
                            var vals = new RecordTemperatures
                                       {
                                           Time = t,
                                           ThermostatTemperature = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_1 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_2 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_3 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_4 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_5 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_6 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_7 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_8 = rn.Next(40 + mins * 2, 40 + mins * 5)
                                       };

                            if (rn.Next(0, 100) > 50)
                            {
                                info.EventList.Add(new RecordEvent { Type = (EventType)rn.Next(0, 3), Time = t, Description = events[rn.Next(0, events.Length)] });
                            }

                            info.RecordTemperatures.Add(vals);

                            t = t.Add(TimeSpan.FromMinutes(1));
                        }

                        info.EndTime = info.StartTime + t;

                        st = info.EndTime + TimeSpan.FromMinutes(10);

                        var infos = new List<ProcessInfo>();
                        var temp = new List<int>();
                        var n = rn.Next(0, 3);
                        for (var p = 0; p <= n; p++)
                        {
                            var _info = info.Copy();
                            var index = rn.Next(0, order_code.Length);
                            while (temp.Contains(index))
                            {
                                index = rn.Next(0, order_code.Length);
                            }

                            temp.Add(index);
                            _info.OrderCode = order_code[index];
                            _info.ProcessCount = rn.Next(10, 20);

                            infos.Add(_info);
                        }

                        TraceVM.AddToDB(i, infos, info.EndTime.AddMinutes(1));
                    }
                }
            }
        }

        public Connector()
        {
            var db = new MongoClient("mongodb://localhost:27017").GetDatabase("GP");

            DialogVM = new GlobalDialog_ViewModel();
            MainVM = new MainWindow_ViewModel();
            RecipeVM = new RecipeControl_ViewModel(new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("PLC_Recipes")), DialogVM);
            TraceVM = new TraceabilityView_ViewModel(new MongoBase<ProcessInfo>(db.GetCollection<ProcessInfo>("Product_Infos")));
            LogVM = new LogView_ViewModel(new MongoBase<LogEvent>(db.GetCollection<LogEvent>("Event_Logs")));

            var map = new PLC_DevicesMap(
                                         //!PLC的M區位置
                                         new Dictionary<SignalNames, int>
                                         {
                                             { SignalNames.PC_ByPass, 20 },
                                             { SignalNames.自動模式, 50 },
                                             { SignalNames.自動啟動, 51 },
                                             { SignalNames.自動停止, 52 },
                                             { SignalNames.手動模式, 60 },
                                             { SignalNames.降溫中, 208 },
                                             { SignalNames.程式結束, 209 },
                                             { SignalNames.加熱門未關, 250 },
                                             { SignalNames.緊急停止, 700 },
                                             { SignalNames.溫控器低溫異常, 701 },
                                             { SignalNames.電源反相, 702 },
                                             { SignalNames.OTP超溫異常, 703 },
                                             { SignalNames.循環風車過載, 704 },
                                             { SignalNames.冷卻進氣風車異常, 705 },
                                             { SignalNames.超溫警報, 710 },
                                             { SignalNames.停止後未開門, 715 },
                                             { SignalNames.循環風車INV異常, 718 },
                                             { SignalNames.充氮氣逾時, 721 },
                                             { SignalNames.門未關定位異常, 722 },
                                             { SignalNames.升恆溫逾時, 723 }
                                         },
                                         //!PLC的D區位置
                                         new Dictionary<DataNames, int>
                                         {
                                             { DataNames.溫控器溫度, 130 },
                                             { DataNames.片段剩餘時間, 132 },
                                             { DataNames.總剩餘時間, 134 },
                                             { DataNames.目前段數, 140 },
                                             { DataNames.爐內溫度_1, 380 },
                                             { DataNames.爐內溫度_2, 381 },
                                             { DataNames.爐內溫度_3, 382 },
                                             { DataNames.爐內溫度_4, 383 },
                                             { DataNames.爐內溫度_5, 384 },
                                             { DataNames.爐內溫度_6, 385 },
                                             { DataNames.爐內溫度_7, 386 },
                                             { DataNames.爐內溫度_8, 387 }
                                         },
                                         //!PLC的配方參數位置
                                         new Dictionary<DataNames, int>
                                         {
                                             { DataNames.目標溫度_1, 712 },
                                             { DataNames.升溫時間_1, 713 },
                                             { DataNames.恆溫溫度_1, 714 },
                                             { DataNames.恆溫時間_1, 715 },
                                             { DataNames.目標溫度_2, 716 },
                                             { DataNames.升溫時間_2, 717 },
                                             { DataNames.恆溫溫度_2, 718 },
                                             { DataNames.恆溫時間_2, 719 },
                                             { DataNames.目標溫度_3, 720 },
                                             { DataNames.升溫時間_3, 721 },
                                             { DataNames.恆溫溫度_3, 722 },
                                             { DataNames.恆溫時間_3, 723 },
                                             { DataNames.目標溫度_4, 724 },
                                             { DataNames.升溫時間_4, 725 },
                                             { DataNames.恆溫溫度_4, 726 },
                                             { DataNames.恆溫時間_4, 727 },
                                             { DataNames.目標溫度_5, 728 },
                                             { DataNames.升溫時間_5, 729 },
                                             { DataNames.恆溫溫度_5, 730 },
                                             { DataNames.恆溫時間_5, 731 },
                                             { DataNames.目標溫度_6, 732 },
                                             { DataNames.升溫時間_6, 733 },
                                             { DataNames.恆溫溫度_6, 734 },
                                             { DataNames.恆溫時間_6, 735 },
                                             { DataNames.目標溫度_7, 736 },
                                             { DataNames.升溫時間_7, 737 },
                                             { DataNames.恆溫溫度_7, 738 },
                                             { DataNames.恆溫時間_7, 739 },
                                             { DataNames.目標溫度_8, 740 },
                                             { DataNames.升溫時間_8, 741 },
                                             { DataNames.恆溫溫度_8, 742 },
                                             { DataNames.恆溫時間_8, 743 },
                                             { DataNames.降溫溫度, 745 },
                                             { DataNames.充氣時間, 747 },
                                             { DataNames.使用段數, 749 },
                                             { DataNames.配方名稱_01, 750 },
                                             { DataNames.配方名稱_02, 751 },
                                             { DataNames.配方名稱_03, 752 },
                                             { DataNames.配方名稱_04, 753 },
                                             { DataNames.配方名稱_05, 754 },
                                             { DataNames.配方名稱_06, 755 },
                                             { DataNames.配方名稱_07, 756 },
                                             { DataNames.配方名稱_08, 757 },
                                             { DataNames.配方名稱_09, 758 },
                                             { DataNames.配方名稱_10, 759 },
                                             { DataNames.配方名稱_11, 760 },
                                             { DataNames.配方名稱_12, 761 },
                                             { DataNames.配方名稱_13, 762 }
                                         });

            TotalVM = new TotalView_ViewModel(Enumerable.Repeat(map, 20).ToArray(), DialogVM);

            //!當回到主頁時，也將生產總覽回到總覽頁
            MainVM.IndexChangedEvent += index =>
                                        {
                                            if (index == 0)
                                            {
                                                TotalVM.Index = 0;
                                            }
                                        };

            //!當主視窗讀取完成時，再讀取配方和生產履歷資料庫
            MainVM.LoadedEvent += async dp =>
                                  {
                                      await dp.InvokeAsync(() =>
                                                           {
                                                               RecipeVM.InitialLoadCommand.Execute(null);
                                                               TraceVM.TodayCommand.Execute(null);
                                                           },
                                                           DispatcherPriority.SystemIdle);
                                  };

            //!當配方列表更新時，依據使用站別發佈配方
            RecipeVM.ListUpdatedEvent += async list =>
                                         {
                                             TotalVM.SetRecipeNames(list.Select(x => x.RecipeName).ToArray());

                                             foreach (var recipe in list)
                                             {
                                                 for (var i = 0; i < recipe.Used_Stations.Length; i++)
                                                 {
                                                     if (recipe.Used_Stations[i])
                                                     {
                                                         await TotalVM.SetRecipe(i, recipe);
                                                     }
                                                 }
                                             }
                                         };

            //!當某站烤箱要求配方時，自資料庫讀取配方並發送
            TotalVM.WantRecipe += async e =>
                                  {
                                      if (!string.IsNullOrEmpty(e.RecipeName))
                                      {
                                          await TotalVM.SetRecipe(e.StationIndex, await RecipeVM.GetRecipe(e.StationIndex, e.RecipeName));
                                      }

                                      e.Lock?.Set();
                                  };

            //!當某站烤箱完成烘烤程序時，將生產資訊寫入資料庫並輸出至上傳資料夾
            TotalVM.AddRecordToDB += async e =>
                                     {
                                         TraceVM.AddToDB(e.StationIndex, e.Infos);

                                         if (!Directory.Exists(DataOutputPath))
                                         {
                                             Directory.CreateDirectory(DataOutputPath);
                                         }

                                         await Task.Factory.StartNew(() =>
                                                                     {
                                                                         foreach (var info in e.Infos)
                                                                         {
                                                                             for (var i = 1; i <= info.ProcessCount; i++)
                                                                             {
                                                                                 var ProduceCode = info.OrderCode + (i + info.ProcessNumber).ToString("000");

                                                                                 var path = DataOutputPath +
                                                                                            "\\" +
                                                                                            ProduceCode +
                                                                                            "_" +
                                                                                            DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                                                                            "_" +
                                                                                            (e.StationIndex + 1) +
                                                                                            "_";

                                                                                 var n = 1;
                                                                                 while (File.Exists(path + n))
                                                                                 {
                                                                                     n++;
                                                                                 }

                                                                                 File.WriteAllText(path + n, info.ToString(ProduceCode), Encoding.ASCII);
                                                                                 //!紀錄資料到指定輸出資料夾
                                                                             }
                                                                         }
                                                                     },
                                                                     TaskCreationOptions.LongRunning);
                                     };

            TotalVM.EventHappened += e =>
                                     {
                                         LogVM.AddToDB(new LogEvent { StationNumber = e.StationIndex + 1, Time = e.time, Type = e.type, Description = e.note });
                                     };

            //!更新每日產量
            TraceVM.TodayProductionUpdated += datas =>
                                              {
                                                  foreach (var (StationIndex, Production) in datas)
                                                  {
                                                      TotalVM.TotalProduction[StationIndex] = Production;
                                                  }
                                              };

            //MakeTestData(20);
        }
    }
}