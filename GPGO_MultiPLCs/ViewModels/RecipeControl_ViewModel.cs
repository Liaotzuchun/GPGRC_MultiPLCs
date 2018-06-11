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
        public delegate void ListUpdated(List<PLC_Recipe> list);

        private readonly MongoClient Mongo_Client;

        private string _SearchName;
        private PLC_Recipe _Selected_PLC_Recipe;
        private int _Selected_PLC_Recipe_Index;
        private bool _Standby;
        private string _TypedName;
        private List<PLC_Recipe> _ViewRecipes;
        private List<PLC_Recipe> Recipes;

        public bool Add_Enable => !string.IsNullOrEmpty(_TypedName) && Recipes.All(x => x.RecipeName != _TypedName);
        public RelayCommand AddCommand { get; }

        public bool Delete_Enable => _Selected_PLC_Recipe != null && !_Selected_PLC_Recipe.Used_Stations.Any(x => x);
        public RelayCommand DeleteCommand { get; }

        public RelayCommand InitialLoadCommand { get; }
        public RelayCommand ResetCommand { get; }

        public bool Save_Enable => _Selected_PLC_Recipe != null;
        public RelayCommand SaveCommand { get; }

        public string SearchName
        {
            get => _SearchName;
            set
            {
                value = value.Replace(" ", "_");
                _SearchName = value;
                NotifyPropertyChanged();

                ViewRecipes = string.IsNullOrEmpty(_SearchName) ? Recipes : Recipes?.Where(x => x.RecipeName.ToLower().Contains(_SearchName.ToLower())).ToList();
            }
        }

        public PLC_Recipe Selected_PLC_Recipe
        {
            get => _Selected_PLC_Recipe;
            set
            {
                _Selected_PLC_Recipe = value;
                NotifyPropertyChanged();
            }
        }

        public int Selected_PLC_Recipe_Index
        {
            get => _Selected_PLC_Recipe_Index;
            set
            {
                _Selected_PLC_Recipe_Index = value;

                if (_Selected_PLC_Recipe_Index > -1)
                {
                    var recipe = _ViewRecipes[_Selected_PLC_Recipe_Index];
                    TypedName = recipe.RecipeName;
                }

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
                NotifyPropertyChanged();

                _Selected_PLC_Recipe_Index = string.IsNullOrEmpty(_TypedName) ? -1 : _ViewRecipes?.FindIndex(x => x.RecipeName == _TypedName) ?? -1;
                NotifyPropertyChanged(nameof(Selected_PLC_Recipe_Index));

                Selected_PLC_Recipe = string.IsNullOrEmpty(_TypedName) ? null : Recipes?.FirstOrDefault(x => x.RecipeName == _TypedName)?.Copy();

                NotifyPropertyChanged(nameof(Save_Enable));
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

        public RecipeControl_ViewModel(MongoClient mongo, IDialogService<string> dialog)
        {
            Mongo_Client = mongo;

            InitialLoadCommand = new RelayCommand(async e =>
                                                  {
                                                      if (Recipes == null || Recipes.Count == 0)
                                                      {
                                                          Standby = false;

                                                          await RefreshList();

                                                          Standby = true;
                                                      }
                                                      else
                                                      {
                                                          TypedName = "";
                                                          SearchName = "";
                                                      }
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
        }

        public event ListUpdated ListUpdatedEvent;

        public event Action RecipeLoadedEvent;

        public async Task<PLC_Recipe> GetRecipe(int index, string name)
        {
            var result = Recipes.FirstOrDefault(x => x.RecipeName == name);

            if (result != null)
            {
                try
                {
                    var db = Mongo_Client.GetDatabase("GP");
                    var Sets = db.GetCollection<PLC_Recipe>("PLC_Recipes");

                    foreach (var recipe in Recipes.Where(x => x.Used_Stations[index]))
                    {
                        recipe.Used_Stations[index] = false;
                        await Sets.UpdateOneAsync(x => x.RecipeName.Equals(recipe.RecipeName), Builders<PLC_Recipe>.Update.Set(x => x.Used_Stations, recipe.Used_Stations));
                    }

                    result.Used_Stations[index] = true;
                    await Sets.UpdateOneAsync(x => x.RecipeName.Equals(result.RecipeName), Builders<PLC_Recipe>.Update.Set(x => x.Used_Stations, result.Used_Stations));
                }
                catch (Exception ex)
                {
                    ErrorRecoder.RecordError(ex, "配方資料庫更新使用站點資訊失敗");
                }

                await RefreshList();
            }

            return result;
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
                        TypedName = temp[0].RecipeName;

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
            }
            catch (Exception ex)
            {
                ErrorRecoder.RecordError(ex);
            }
            finally
            {
                if (Recipes != null && Recipes.Count > 0)
                {
                    ListUpdatedEvent?.Invoke(Recipes);
                }
            }
        }

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
    }
}