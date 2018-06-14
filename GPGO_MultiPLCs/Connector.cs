using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Models;
using GPGO_MultiPLCs.ViewModels;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GPGO_MultiPLCs
{
    public class Connector
    {
        public const int PLC_Count = 20;

        public readonly MongoClient Mongo;
        public GlobalDialog_ViewModel DialogVM { get; }
        public MainWindow_ViewModel MainVM { get; }
        public RecipeControl_ViewModel RecipeVM { get; }
        public TotalView_ViewModel TotalVM { get; }
        public TraceabilityView_ViewModel TraceVM { get; }

        public void MakeTestData()
        {
            var p_code = new[] { "ooxx", "abc", "zzz", "qoo", "boom", "xxx", "wunmao" };

            for (var j = 1; j < 30; j++)
            {
                for (var i = 0; i < PLC_Count; i++)
                {
                    var time = DateTime.Now;
                    var rn = new Random(i + j);

                    for (var k = 0; k < 10; k++)
                    {
                        var info = new ProcessInfo
                                   {
                                       ProduceCode = p_code[rn.Next(0, p_code.Length)],
                                       ProcessCount = rn.Next(50, 100),
                                       StartTime = new DateTime(time.Year, time.Month, j, 8 + k, i, 0),
                                       TrolleyCode = rn.Next(1, 100).ToString("000"),
                                       OperatorID = rn.Next(1, 10).ToString("000")
                                   };

                        var t = new TimeSpan();
                        for (var m = 0; m < 100; m++)
                        {
                            var vals = new RecordTemperatures
                                       {
                                           Time = t,
                                           ThermostatTemperature = rn.Next(0, (t.Minutes + 1) * 80),
                                           OvenTemperatures =
                                           {
                                               [0] = rn.Next(0, (t.Minutes + 1) * 80),
                                               [1] = rn.Next(0, (t.Minutes + 1) * 80),
                                               [2] = rn.Next(0, (t.Minutes + 1) * 80),
                                               [3] = rn.Next(0, (t.Minutes + 1) * 80),
                                               [4] = rn.Next(0, (t.Minutes + 1) * 80),
                                               [5] = rn.Next(0, (t.Minutes + 1) * 80),
                                               [6] = rn.Next(0, (t.Minutes + 1) * 80),
                                               [7] = rn.Next(0, (t.Minutes + 1) * 80)
                                           }
                                       };

                            info.RecordTemperatures.Add(vals);

                            t = t.Add(TimeSpan.FromMinutes(1));
                        }

                        info.EndTime = info.StartTime + t;

                        TraceVM.AddToDB(i, info, info.EndTime.AddMinutes(1));
                    }
                }
            }
        }

        public Connector()
        {
            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
            var mongo_service = new ServiceController("MongoDB");
            if (mongo_service.Status == ServiceControllerStatus.Stopped)
            {
                mongo_service.Start();
                mongo_service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(9));
            }

            Mongo = new MongoClient("mongodb://localhost:27017");

            DialogVM = new GlobalDialog_ViewModel();
            MainVM = new MainWindow_ViewModel();
            TotalVM = new TotalView_ViewModel(DialogVM);
            RecipeVM = new RecipeControl_ViewModel(Mongo, DialogVM);
            TraceVM = new TraceabilityView_ViewModel(Mongo);

            RecipeVM.ListUpdatedEvent += async list =>
                                         {
                                             TotalVM.SetRecipeNames(list.Select(x => x.RecipeName).ToArray());

                                             await Task.Factory.StartNew(() =>
                                                                         {
                                                                             Parallel.ForEach(list,
                                                                                              recipe =>
                                                                                              {
                                                                                                  for (var i = 0; i < recipe.Used_Stations.Length; i++)
                                                                                                  {
                                                                                                      if (recipe.Used_Stations[i])
                                                                                                      {
                                                                                                          TotalVM.SetRecipe(i, recipe).Wait();
                                                                                                      }
                                                                                                  }
                                                                                              });
                                                                         });
                                         };

            TotalVM.WantRecipe += async (i, recipe, obj) =>
                                  {
                                      await TotalVM.SetRecipe(i, await RecipeVM.GetRecipe(i, recipe));

                                      obj?.Set();
                                  };

            TotalVM.AddRecordToDB += (i, info) =>
                                     {
                                         TraceVM.AddToDB(i, info);
                                     };

            //MakeTestData();
        }
    }
}