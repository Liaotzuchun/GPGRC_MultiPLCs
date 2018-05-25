using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using MongoDB.Driver;

namespace GPGO_MultiPLCs.ViewModels
{
    public class RecipeControl_ViewModel : ViewModelBase
    {
        private readonly MongoClient Mongo_Client;

        private PLC_Recipe _Selected_PLC_Recipe;
        private List<PLC_Recipe> _Recipes;
        private string _SearchName;
        private bool _Standby;
        private string _TypedName;

        public PLC_Recipe Selected_PLC_Recipe
        {
            get => _Selected_PLC_Recipe;
            set
            {
                _Selected_PLC_Recipe = value;
                NotifyPropertyChanged();
            }
        }

        public List<PLC_Recipe> Recipes
        {
            get => _Recipes;
            set
            {
                _Recipes = value;
                NotifyPropertyChanged();
            }
        }

        public bool Standby
        {
            get => _Standby;
            set
            {
                _Standby = value;
                NotifyPropertyChanged();
            }
        }

        public string TypedName
        {
            get => _TypedName;
            set
            {
                _TypedName = value.Length > 8 ? value.Substring(0, 8) : value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Load_Enable));
                NotifyPropertyChanged(nameof(Add_Enable));
                NotifyPropertyChanged(nameof(Delete_Enable));
            }
        }

        public string SearchName
        {
            get => _SearchName;
            set
            {
                _SearchName = value;
                NotifyPropertyChanged();
            }
        }

        public bool Load_Enable => !string.IsNullOrEmpty(_TypedName) && _Recipes != null && _Recipes.Any(x => x.RecipeName == _TypedName);

        public bool Add_Enable => !string.IsNullOrEmpty(_TypedName) && _Recipes.All(x => x.RecipeName != _TypedName);

        public bool Delete_Enable => Load_Enable && !_Selected_PLC_Recipe.Used_Stations.Any();

        public RelayCommand InitialLoadCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand ResetCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SelectedCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public RecipeControl_ViewModel(MongoClient mongo, IDialogService dialog)
        {
            Mongo_Client = mongo;
            InitialLoadCommand = new RelayCommand(async e =>
                                                  {
                                                      Standby = false;

                                                      TypedName = "";
                                                      SearchName = "";

                                                      await RefreshList();

                                                      Standby = true;
                                                  });

            SaveCommand = new RelayCommand(async e =>
                                           {
                                               if (await dialog.Show("將儲存並覆蓋同名配方，無法復原\n" + "確定儲存?", true))
                                               {
                                                   await Save(_TypedName);
                                               }
                                           });

            ResetCommand = new RelayCommand(async e =>
                                           {
                                               if (string.IsNullOrEmpty(_TypedName))
                                               {
                                                   return;
                                               }

                                               await Load(_TypedName);
                                           });

            AddCommand = new RelayCommand(async e =>
                                          {
                                              await Save(_TypedName);

                                              await RefreshList();
                                          });

            DeleteCommand = new RelayCommand(async e =>
                                             {
                                                 Standby = false;

                                                 try
                                                 {
                                                     var db = Mongo_Client.GetDatabase("GP");
                                                     var Load_Sets = db.GetCollection<PLC_Recipe>("PLC_Recipes");
                                                     await Load_Sets.DeleteOneAsync(x => x.RecipeName.Equals(_TypedName));
                                                 }
                                                 catch (Exception)
                                                 {
                                                 }

                                                 await RefreshList();

                                                 Standby = true;
                                             });

            SelectedCommand = new RelayCommand(e =>
                                               {
                                                   if ((int)e > -1)
                                                   {
                                                       Selected_PLC_Recipe = _Recipes.ElementAtOrDefault((int)e);
                                                       TypedName = Selected_PLC_Recipe?.RecipeName;
                                                   }
                                               });

            RefreshCommand = new RelayCommand(async e =>
                                              {
                                                  await RefreshList();
                                              });
        }

        public event Action RecipeLoadedEvent;

        private async Task Save(string name)
        {
            Standby = false;

            if (!string.IsNullOrEmpty(name))
            {
                return;
            }

            var TempSet = Selected_PLC_Recipe == null ? new PLC_Recipe() : Selected_PLC_Recipe.Copy();

            if (Mongo_Client != null)
            {
                try
                {
                    var db = Mongo_Client.GetDatabase("GP");
                    var Sets = db.GetCollection<PLC_Recipe>("PLC_Recipes");
                    await Sets.ReplaceOneAsync(x => x.RecipeName.Equals(TempSet.RecipeName), TempSet, new UpdateOptions { IsUpsert = true });
                }
                catch (Exception ex)
                {
                    ErrorRecoder.RecordError(ex);
                }
            }

            await RefreshList();

            Standby = true;
        }

        private async Task Load(string name)
        {
            Standby = false;

            if (Mongo_Client != null && !string.IsNullOrEmpty(name))
            {
                try
                {
                    var db = Mongo_Client.GetDatabase("GP");
                    var Load_Sets = db.GetCollection<PLC_Recipe>("PLC_Recipes");
                    var temp = await (await Load_Sets.FindAsync(x => x.RecipeName.Equals(name))).ToListAsync();

                    if (temp.Count > 0)
                    {
                        Selected_PLC_Recipe = temp[0];

                        RecipeLoadedEvent?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    ErrorRecoder.RecordError(ex);
                }
            }

            Standby = true;
        }

        private async Task RefreshList()
        {
            try
            {
                var db = Mongo_Client.GetDatabase("GP");
                var Sets = db.GetCollection<PLC_Recipe>("PLC_Recipes");

                Recipes = string.IsNullOrEmpty(_SearchName) ? await (await Sets.FindAsync(x => true)).ToListAsync() :
                              await (await Sets.FindAsync(x => x.RecipeName.ToLower().Contains(_SearchName.ToLower()))).ToListAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}