using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>配方管理</summary>
    public class RecipeControl_ViewModel : ObservableObject
    {
        /// <summary>未儲存未修改之配方(備份或還原用)</summary>
        public PLC_Recipe Selected_PLC_Recipe_Origin;

        public string UserName;
        private readonly IDataBase<PLC_Recipe> RecipeCollection;
        private readonly IDataBase<PLC_Recipe> RecipeCollection_History;

        /// <summary>所有配方的列表</summary>
        private List<PLC_Recipe> Recipes;

        /// <summary>辨識是否可新增配方(列表中沒有和輸入名相同的配方)</summary>
        public bool Add_Enable => !string.IsNullOrEmpty(TypedName) && Recipes.All(x => x.RecipeName != TypedName);

        public RelayCommand AddCommand { get; }

        /// <summary>辨識是否可刪除配方(列表中有和輸入名相同的配方，且該配方無烤箱正在使用)</summary>
        public bool Delete_Enable => Selected_PLC_Recipe != null && !Selected_PLC_Recipe.Used_Stations.Any(x => x);

        public RelayCommand DeleteCommand { get; }

        /// <summary>讀取配方列表</summary>
        public RelayCommand InitialLoadCommand { get; }

        /// <summary>重新讀取配方參數(未儲存時)</summary>
        public RelayCommand ResetCommand { get; }

        /// <summary>辨別是否可儲存配方(有正在選取的配方)</summary>
        public bool Save_Enable => Selected_PLC_Recipe != null && !Selected_PLC_Recipe.Equals(Selected_PLC_Recipe_Origin);

        public RelayCommand SaveCommand { get; }

        public PLC_Recipe SelectedHistory => Old_ViewRecipes == null || HistoryIndex >= Old_ViewRecipes.Count ? null : Old_ViewRecipes[HistoryIndex];

        public int HistoryIndex
        {
            get => Get<int>();
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                Set(value);
                NotifyPropertyChanged(nameof(SelectedHistory));
            }
        }

        public IList<PLC_Recipe> Old_ViewRecipes
        {
            get => Get<IList<PLC_Recipe>>();
            set
            {
                Set(value);
                HistoryIndex = value?.Count - 1 ?? 0;
            }
        }

        /// <summary>配方搜尋的關鍵字</summary>
        public string SearchName
        {
            get => Get<string>();
            set
            {
                value = value.Replace(" ", "_");
                Set(value);

                ViewRecipes = string.IsNullOrEmpty(SearchName) ? Recipes.AsQueryable() : Recipes?.AsQueryable().Where(x => x.RecipeName.ToLower().Contains(SearchName.ToLower()));
            }
        }

        /// <summary>目前選取的配方</summary>
        public PLC_Recipe Selected_PLC_Recipe
        {
            get => Get<PLC_Recipe>();
            set
            {
                var old_value = Selected_PLC_Recipe;

                if (old_value != null)
                {
                    old_value.PropertyChanged -= RecipePropertyChanged;
                }

                Selected_PLC_Recipe_Origin = value?.Copy(UserName);
                Set(value);

                if (value == null)
                {
                    Old_ViewRecipes = null;
                }
                else
                {
                    Selected_PLC_Recipe.PropertyChanged += RecipePropertyChanged;

                    GetHistory(value.RecipeName);
                }
            }
        }

        /// <summary>目前選取配方在列表中的index</summary>
        public int Selected_PLC_Recipe_Index
        {
            get => Get<int>();
            set
            {
                if (value > -1)
                {
                    Set(ViewRecipes.ElementAt(value).RecipeName, nameof(TypedName));

                    Selected_PLC_Recipe = string.IsNullOrEmpty(TypedName) ? null : Recipes?.FirstOrDefault(x => x.RecipeName == TypedName)?.Copy(UserName);

                    NotifyPropertyChanged(nameof(Save_Enable));
                    NotifyPropertyChanged(nameof(Add_Enable));
                    NotifyPropertyChanged(nameof(Delete_Enable));
                }

                Set(value);
            }
        }

        /// <summary>辨識是否不在忙碌中</summary>
        public bool Standby
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>輸入/選定的配方名</summary>
        public string TypedName
        {
            get => Get<string>();
            set
            {
                value = value.Replace(" ", "_");
                Set(value.Length > 26 ? value.Substring(0, 26) : value);
                Set(string.IsNullOrEmpty(TypedName) ? -1 : ViewRecipes?.ToList().FindIndex(x => x.RecipeName == TypedName) ?? -1, nameof(Selected_PLC_Recipe_Index));

                Selected_PLC_Recipe = string.IsNullOrEmpty(TypedName) ? null : Recipes?.FirstOrDefault(x => x.RecipeName == TypedName)?.Copy(UserName);

                NotifyPropertyChanged(nameof(Save_Enable));
                NotifyPropertyChanged(nameof(Add_Enable));
                NotifyPropertyChanged(nameof(Delete_Enable));
            }
        }

        /// <summary>顯示的配方列表(依據搜尋條件)</summary>
        public IQueryable<PLC_Recipe> ViewRecipes
        {
            get => Get<IQueryable<PLC_Recipe>>();
            set => Set(value);
        }

        /// <summary>配方列表更新事件</summary>
        public event Action<List<PLC_Recipe>> ListUpdatedEvent;

        /// <summary>單一配方讀取完成事件</summary>
        public event Action RecipeLoadedEvent;

        /// <summary>獲取指定配方</summary>
        /// <param name="index">烤箱站號</param>
        /// <param name="name">配方名</param>
        /// <returns></returns>
        public async ValueTask<PLC_Recipe> GetRecipe(int index, string name)
        {
            var result = Recipes.FirstOrDefault(x => x.RecipeName == name);

            if (result != null)
            {
                try
                {
                    foreach (var recipe in Recipes.Where(x => x.Used_Stations[index]))
                    {
                        recipe.Used_Stations[index] = false;
                        await RecipeCollection.UpdateOneAsync(x => x.RecipeName.Equals(recipe.RecipeName), nameof(PLC_Recipe.Used_Stations), recipe.Used_Stations);
                    }

                    result.Used_Stations[index] = true;
                    await RecipeCollection.UpdateOneAsync(x => x.RecipeName.Equals(result.RecipeName), nameof(PLC_Recipe.Used_Stations), result.Used_Stations);
                }
                catch (Exception ex)
                {
                    ex.RecordError("配方資料庫更新使用站點資訊失敗");
                }
            }

            return result;
        }

        public void RecipePropertyChanged(object s, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Save_Enable));
        }

        private async void GetHistory(string name)
        {
            var list = await RecipeCollection_History.FindAsync(x => x.RecipeName == name);
            if (list != null && list.Count > 0)
            {
                Old_ViewRecipes = list.OrderBy(x => x.Updated).ToList();
            }
            else
            {
                Old_ViewRecipes = null;
            }
        }

        /// <summary>讀取配方</summary>
        /// <param name="name">配方名</param>
        /// <returns></returns>
        private async Task Load(string name)
        {
            Standby = false;

            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    var temp = await RecipeCollection.FindAsync(x => x.RecipeName.Equals(name));

                    if (temp.Any())
                    {
                        TypedName = temp[0].RecipeName;

                        RecipeLoadedEvent?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    ex.RecordError();
                }
            }

            Standby = true;
        }

        /// <summary>更新配方列表</summary>
        /// <returns></returns>
        private async Task RefreshList()
        {
            try
            {
                TypedName = "";

                Recipes = await RecipeCollection.FindAsync(x => true);
                ViewRecipes = Recipes?.AsQueryable().Where(x => string.IsNullOrEmpty(SearchName) || x.RecipeName.ToLower().Contains(SearchName.ToLower()));
            }
            catch (Exception ex)
            {
                ex.RecordError();
            }
            finally
            {
                if (Recipes != null && Recipes.Count > 0)
                {
                    ListUpdatedEvent?.Invoke(Recipes);
                }
            }
        }

        /// <summary>儲存配方</summary>
        /// <param name="name">配方名</param>
        /// <returns></returns>
        private async Task Save(string name)
        {
            Standby = false;

            var TempSet = Selected_PLC_Recipe == null ? new PLC_Recipe(name, UserName) : Selected_PLC_Recipe.Copy(UserName);

            try
            {
                await RecipeCollection.UpsertAsync(x => x.RecipeName.Equals(TempSet.RecipeName), TempSet);
            }
            catch (Exception ex)
            {
                ex.RecordError();
            }

            if (Selected_PLC_Recipe_Origin != null)
            {
                try
                {
                    await RecipeCollection_History.AddAsync(Selected_PLC_Recipe_Origin);
                }
                catch (Exception ex)
                {
                    ex.RecordError();
                }
            }

            await RefreshList();

            Standby = true;
        }

        public RecipeControl_ViewModel(IDataBase<PLC_Recipe> db, IDataBase<PLC_Recipe> db_history, IDialogService dialog)
        {
            RecipeCollection = db;
            RecipeCollection_History = db_history;

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
                                               if (await dialog.Show(new Dictionary<Language, string>
                                                                     {
                                                                         { Language.TW, "即將儲存並覆蓋同名配方，確定儲存?" },
                                                                         { Language.CHS, "即将储存并覆盖同名配方，确定储存?" },
                                                                         { Language.EN, "The recipe is going to save or replace the same one.\n" + "Are you sure?" }
                                                                     },
                                                                     true))
                                               {
                                                   await Save(TypedName);
                                               }
                                           });

            ResetCommand = new RelayCommand(async e =>
                                            {
                                                Selected_PLC_Recipe.CopyValue(UserName, SelectedHistory);

                                                if (await dialog.Show(new Dictionary<Language, string>
                                                                      {
                                                                          { Language.TW, "即將儲存並覆蓋同名配方，確定儲存?" },
                                                                          { Language.CHS, "即将储存并覆盖同名配方，确定储存?" },
                                                                          { Language.EN, "The recipe is going to save or replace the same one.\n" + "Are you sure?" }
                                                                      },
                                                                      true))
                                                {
                                                    await Save(TypedName);
                                                }
                                            });

            AddCommand = new RelayCommand(async e =>
                                          {
                                              await Save(TypedName);
                                              SearchName = "";
                                          });

            DeleteCommand = new RelayCommand(async e =>
                                             {
                                                 Standby = false;

                                                 try
                                                 {
                                                     await RecipeCollection.DeleteOneAsync(x => x.RecipeName.Equals(TypedName));
                                                 }
                                                 catch
                                                 {
                                                 }

                                                 await RefreshList();

                                                 Standby = true;
                                             });
        }
    }
}