﻿using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
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

        public static readonly DependencyProperty LanguageProperty = DependencyProperty.Register(nameof(Language), typeof(string), typeof(Connector), new PropertyMetadata("", LanguageChanged));

        private static void LanguageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((Connector)sender).DialogVM.Language = (GlobalTempSettings.Language)Enum.Parse(typeof(GlobalTempSettings.Language), e.NewValue.ToString());
        }

        [Bindable(true)]
        public GlobalTempSettings.Language Language
        {
            get => (GlobalTempSettings.Language)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }

        public readonly MongoClient Mongo;

        public GlobalDialog_ViewModel DialogVM { get; }
        public MainWindow_ViewModel MainVM { get; }
        public RecipeControl_ViewModel RecipeVM { get; }
        public TotalView_ViewModel TotalVM { get; }
        public TraceabilityView_ViewModel TraceVM { get; }

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

            for (var j = 1; j < new DateTime(time.Year, time.Month, 1).AddMonths(1).AddDays(-1).Day; j++)
            {
                for (var i = 0; i < PLC_Count; i++)
                {
                    var rn = new Random(i + j);

                    var st = new DateTime(time.Year, time.Month, j, 8, i + rn.Next(0, 10), i);

                    for (var k = 0; k < 8; k++)
                    {
                        var info = new ProcessInfo
                                   {
                                       OrderCode = order_code[rn.Next(0, order_code.Length)],
                                       ProcessCount = rn.Next(50, 100),
                                       StartTime = st,
                                       TrolleyCode = rn.Next(1, 100).ToString("000"),
                                       OperatorID = rn.Next(1, 10).ToString("000")
                                   };

                        var t = new TimeSpan();
                        for (var m = 0; m < 100; m++)
                        {
                            var mins = (int)t.TotalMinutes + 1;
                            var vals = new RecordTemperatures
                                       {
                                           Time = t,
                                           ThermostatTemperature = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_0 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_1 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_2 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_3 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_4 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_5 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_6 = rn.Next(40 + mins * 2, 40 + mins * 5),
                                           OvenTemperatures_7 = rn.Next(40 + mins * 2, 40 + mins * 5)
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

                        TraceVM.AddToDB(i, info, info.EndTime.AddMinutes(1));
                    }
                }
            }
        }

        public Connector()
        {
            Mongo = new MongoClient("mongodb://localhost:27017");

            DialogVM = new GlobalDialog_ViewModel();
            MainVM = new MainWindow_ViewModel();
            RecipeVM = new RecipeControl_ViewModel(Mongo, DialogVM);
            TraceVM = new TraceabilityView_ViewModel(Mongo);
            TotalVM = new TotalView_ViewModel(20, DialogVM);

            MainVM.IndexChangedEvent += index =>
                                        {
                                            if (index == 0)
                                            {
                                                TotalVM.Index = 0;
                                            }
                                        };

            MainVM.LoadedEvent += async dp =>
                                  {
                                      await dp.InvokeAsync(() =>
                                                           {
                                                               RecipeVM.InitialLoadCommand.Execute(null);
                                                               TraceVM.TodayCommand.Execute(null);
                                                           },
                                                           DispatcherPriority.SystemIdle);
                                  };

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

            TotalVM.WantRecipe += async (i, recipe, obj) =>
                                  {
                                      if (!string.IsNullOrEmpty(recipe))
                                      {
                                          await TotalVM.SetRecipe(i, await RecipeVM.GetRecipe(i, recipe));
                                      }

                                      obj?.Set();
                                  };

            TotalVM.AddRecordToDB += (i, info) =>
                                     {
                                         if (Directory.Exists(DataOutputPath))
                                         {
                                             TraceVM.AddToDB(i, info);
                                             var path = DataOutputPath + "\\" + info.ProduceCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + (i + 1).ToString() + "_";

                                             var n = 1;
                                             while (File.Exists(path + n.ToString()))
                                             {
                                                 n++;
                                             }

                                             File.WriteAllText(path + n.ToString(), info.ToString(), Encoding.ASCII);

                                             //! 紀錄資料到指定輸出資料夾
                                         }
                                     };

            TraceVM.TodayProductionUpdated += datas =>
                                              {
                                                  foreach (var (station, production) in datas)
                                                  {
                                                      TotalVM.TotalProduction[station] = production;
                                                  }
                                              };

            //MakeTestData(20);
        }
    }
}