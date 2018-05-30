using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using MongoDB.Driver;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class RecipeControl_ViewModel : ViewModelBase
    {
        public delegate void ListUpdated(List<PLC_Recipe> list);

        private readonly MongoClient Mongo_Client;
        private string _SearchName;
        private PLC_Recipe _Selected_PLC_Recipe;
        private bool _Standby;
        private string _TypedName;

        private List<PLC_Recipe> _ViewRecipes;
        private List<PLC_Recipe> Recipes;

        public PLC_Recipe Selected_PLC_Recipe
        {
            get => _Selected_PLC_Recipe;
            set
            {
                _Selected_PLC_Recipe = value;

                if (_Selected_PLC_Recipe != null)
                {
                    _TypedName = _Selected_PLC_Recipe.RecipeName;
                    NotifyPropertyChanged(nameof(TypedName));
                }

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Load_Enable));
                NotifyPropertyChanged(nameof(Add_Enable));
                NotifyPropertyChanged(nameof(Delete_Enable));
            }
        }

        public List<PLC_Recipe> ViewRecipes
        {
            get => _ViewRecipes;
            set
            {
                _ViewRecipes = value;
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
                value = value.Replace(" ", "_");
                _TypedName = value.Length > 26 ? value.Substring(0, 26) : value;

                _Selected_PLC_Recipe = Recipes?.FirstOrDefault(x => x.RecipeName == _TypedName);
                NotifyPropertyChanged(nameof(Selected_PLC_Recipe));

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
                value = value.Replace(" ", "_");
                _SearchName = value;
                NotifyPropertyChanged();
            }
        }

        public bool Load_Enable => !string.IsNullOrEmpty(_TypedName) && Recipes != null && Recipes.Any(x => x.RecipeName == _TypedName);

        public bool Add_Enable => !string.IsNullOrEmpty(_TypedName) && Recipes.All(x => x.RecipeName != _TypedName);

        public bool Delete_Enable => Load_Enable && _Selected_PLC_Recipe.Used_Stations.Any(x => x);

        public RelayCommand InitialLoadCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand ResetCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
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
                                              SearchName = "";

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

            RefreshCommand = new RelayCommand(async e =>
                                              {
                                                  await RefreshList();
                                              });
        }

        public event ListUpdated ListUpdatedEvent;

        public event Action RecipeLoadedEvent;

        private async Task Save(string name)
        {
            Standby = false;

            var TempSet = Selected_PLC_Recipe == null ? new PLC_Recipe(name) : Selected_PLC_Recipe.Copy();

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

                TypedName = "";
                Recipes = await (await Sets.FindAsync(x => true)).ToListAsync();
                ViewRecipes = Recipes.Where(x => string.IsNullOrEmpty(_SearchName) || x.RecipeName.ToLower().Contains(_SearchName.ToLower())).ToList();

                if (Recipes != null && Recipes.Count > 0)
                {
                    ListUpdatedEvent?.Invoke(Recipes);
                }
            }
            catch (Exception ex)
            {
                ErrorRecoder.RecordError(ex);
            }
        }
    }
}