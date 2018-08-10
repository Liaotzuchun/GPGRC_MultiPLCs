﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class Connector : DependencyObject
    {
        public static readonly DependencyProperty DataOutputPathProperty =
            DependencyProperty.Register(nameof(DataOutputPath), typeof(string), typeof(Connector), new PropertyMetadata("", DataOutputPathChanged));

        private static void DataOutputPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //! 
        }

        [Bindable(true)]
        public string DataOutputPath
        {
            get => (string)GetValue(DataOutputPathProperty);
            set => SetValue(DataOutputPathProperty, value);
        }

        public static readonly DependencyProperty LanguageProperty =
            DependencyProperty.Register(nameof(Language), typeof(Language), typeof(Connector), new PropertyMetadata(Language.TW, LanguageChanged));

        private static void LanguageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((Connector)sender).DialogVM.Language = (Language)Enum.Parse(typeof(Language), e.NewValue.ToString());
            ((Connector)sender).TraceVM.Language = (Language)Enum.Parse(typeof(Language), e.NewValue.ToString());
        }

        [Bindable(true)]
        public Language Language
        {
            get => (Language)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }

        public readonly MongoClient Mongo;

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

            for (var j = 1; j <= new DateTime(time.Year, time.Month, 1).AddMonths(2).AddDays(-1).Day; j++)
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
            Mongo = new MongoClient("mongodb://localhost:27017");
            var db = Mongo.GetDatabase("GP");

            DialogVM = new GlobalDialog_ViewModel();
            MainVM = new MainWindow_ViewModel();
            RecipeVM = new RecipeControl_ViewModel(db.GetCollection<PLC_Recipe>("PLC_Recipes"), DialogVM);
            TraceVM = new TraceabilityView_ViewModel(db.GetCollection<ProcessInfo>("Product_Infos"));
            LogVM = new LogView_ViewModel(db.GetCollection<LogEvent>("Event_Logs"));
            TotalVM = new TotalView_ViewModel(20, DialogVM);

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
                                         LogVM.AddToDB(new LogEvent { StationNumber = e.StationIndex, Time = e.time, Type = e.type, Description = e.note });
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