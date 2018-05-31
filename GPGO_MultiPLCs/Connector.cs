using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using GPGO_MultiPLCs.ViewModels;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GPGO_MultiPLCs
{
    public class Connector
    {
        public readonly MongoClient Mongo;
        public MainWindow_ViewModel MainVM { get; }
        public TotalView_ViewModel TotalVM { get; }
        public RecipeControl_ViewModel RecipeVM { get; }
        public TraceabilityView_ViewModel TraceVM { get; }
        public GlobalDialog_ViewModel DialogVM { get; }

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
            TraceVM = new TraceabilityView_ViewModel();

            RecipeVM.ListUpdatedEvent += async list =>
                                         {
                                             TotalVM.SetRecipeNames(list.Select(x => x.RecipeName).ToArray());

                                             await Task.Factory.StartNew(() =>
                                                                         {
                                                                             foreach (var recipe in list)
                                                                             {
                                                                                 for (var i = 0; i < recipe.Used_Stations.Length; i++)
                                                                                 {
                                                                                     if (recipe.Used_Stations[i])
                                                                                     {
                                                                                         TotalVM.SetRecipe(i, recipe);
                                                                                     }
                                                                                 }
                                                                             }
                                                                         });
                                         };

            TotalVM.WantRecipe += async (i, recipe) =>
                                  {
                                      TotalVM.SetRecipe(i, await RecipeVM.GetRecipe(i, recipe));
                                  };
        }
    }
}