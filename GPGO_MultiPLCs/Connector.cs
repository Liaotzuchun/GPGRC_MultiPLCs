using System;
using System.ComponentModel;
using System.Linq;
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

        public readonly MongoClient Mongo;

        public GlobalDialog_ViewModel DialogVM { get; }
        public MainWindow_ViewModel MainVM { get; }
        public RecipeControl_ViewModel RecipeVM { get; }
        public TotalView_ViewModel TotalVM { get; }
        public TraceabilityView_ViewModel TraceVM { get; }

        public void MakeTestData(int PLC_Count)
        {
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
                                info.EventList.Add(new RecordEvent { Type = (EventType)rn.Next(0, 3), Time = t, Description = "警報" + m });
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
                                         TraceVM.AddToDB(i, info);

                                         //! 紀錄資料到指定輸出資料夾
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