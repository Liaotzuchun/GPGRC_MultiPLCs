using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using Serilog;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>配方管理</summary>
    public class RecipeControl_ViewModel : ObservableObject
    {
        private readonly IDataBase<PLC_Recipe> RecipeCollection;
        private readonly IDataBase<PLC_Recipe> RecipeCollection_History;

        /// <summary>未儲存未修改之配方(備份或還原用)</summary>
        public PLC_Recipe Selected_PLC_Recipe_Origin;

        public string UserName;

        /// <summary>所有配方的列表</summary>
        private List<PLC_Recipe> Recipes;

        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ExprotCommand { get; }
        public RelayCommand ImportCommand { get; }

        /// <summary>讀取配方列表</summary>
        public RelayCommand InitialLoadCommand { get; }

        /// <summary>重新讀取配方參數(未儲存時)</summary>
        public RelayCommand ResetCommand { get; }

        public RelayCommand ReNameCommand { get; }

        public RelayCommand SaveCommand { get; }

        /// <summary>辨識是否可新增配方(列表中沒有和輸入名相同的配方)</summary>
        public bool Add_Enable => !string.IsNullOrEmpty(TypedName) && Recipes.All(x => x.RecipeName != TypedName);

        /// <summary>辨識是否可刪除配方(列表中有和輸入名相同的配方，且該配方無烤箱正在使用)</summary>
        public bool Delete_Enable => Selected_PLC_Recipe != null && !Selected_PLC_Recipe.Used_Stations.Any(x => x);

        /// <summary>辨別是否可儲存配方(有正在選取的配方)</summary>
        public bool Save_Enable => Selected_PLC_Recipe != null && !Selected_PLC_Recipe.Equals(Selected_PLC_Recipe_Origin);

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

        /// <summary>修改配方名</summary>
        public string EditedName
        {
            get => Get<string>();
            set
            {
                value = value.Replace(" ", "_");
                Set(value.Length > 26 ? value.Substring(0, 26) : value);
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

        public IList<PLC_Recipe> Old_ViewRecipes
        {
            get => Get<IList<PLC_Recipe>>();
            set
            {
                Set(value);
                HistoryIndex = value?.Count - 1 ?? 0;
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
        public PLC_Recipe GetRecipe(int index, string name)
        {
            return Recipes.FirstOrDefault(x => x.RecipeName == name);
        }

        public void RecipePropertyChanged(object s, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Save_Enable));
        }

        public bool SavetoJson(string path)
        {
            if (Recipes == null || Recipes.Count == 0)
            {
                return false;
            }

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "配方輸出資料夾無法創建");

                    return false;
                }
            }

            foreach (var recipe in Recipes)
            {
                try
                {
                    recipe.WriteToJsonFile($"{path}\\{recipe.RecipeName}.json");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "輸出配方失敗");
                }
            }

            return true;
        }

        public async void SetUsed(int index, string name)
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
                    Log.Error(ex, "配方資料庫更新使用站點資訊失敗");
                }
            }
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
                    Log.Error(ex, "");
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
                Log.Error(ex, "");
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
                Log.Error(ex, "");
            }

            if (Selected_PLC_Recipe_Origin != null)
            {
                try
                {
                    await RecipeCollection_History.AddAsync(Selected_PLC_Recipe_Origin);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "");
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
                                                                         { Language.TW, "即將儲存配方，確定儲存？" },
                                                                         { Language.CHS, "即将储存配方，确定储存？" },
                                                                         { Language.EN, "The recipe is going to save.\nAre you sure?" }
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
                                                                          { Language.TW, "即將儲存配方，確定儲存？" },
                                                                          { Language.CHS, "即将储存配方，确定储存？" },
                                                                          { Language.EN, "The recipe is going to save.\nAre you sure?" }
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

            ExprotCommand = new RelayCommand(e =>
                                             {
                                                 var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Recipes";

                                                 if (SavetoJson(path))
                                                 {
                                                     dialog?.Show(new Dictionary<Language, string>
                                                                  {
                                                                      { Language.TW, $"檔案已輸出至\n{path}" }, { Language.CHS, "档案已输出至\n" + path }, { Language.EN, "The file has been output to\n" + path }
                                                                  },
                                                                  TimeSpan.FromSeconds(6));
                                                 }
                                             });

            ImportCommand = new RelayCommand(async e =>
                                             {
                                                 Standby = false;

                                                 var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Recipes";
                                                 var files = new DirectoryInfo(path).GetFiles("*.json");
                                                 var updates = 0;
                                                 var adds = 0;

                                                 foreach (var file in files)
                                                 {
                                                     try
                                                     {
                                                         var recipe = file.FullName.ReadFromJsonFile<PLC_Recipe>();
                                                         if (recipe != null)
                                                         {
                                                             recipe.RecipeName = Path.GetFileNameWithoutExtension(file.Name);
                                                             var old_recipe = Recipes.Find(x => x.RecipeName == recipe.RecipeName);
                                                             var new_recipe = recipe.Copy(UserName);

                                                             if (old_recipe != null)
                                                             {
                                                                 if (old_recipe.Equals(recipe))
                                                                 {
                                                                     continue;
                                                                 }

                                                                 new_recipe.Used_Stations = old_recipe.Used_Stations;
                                                                 await RecipeCollection_History.AddAsync(old_recipe);
                                                                 updates += 1;
                                                             }
                                                             else
                                                             {
                                                                 adds += 1;
                                                             }

                                                             await RecipeCollection.UpsertAsync(x => x.RecipeName.Equals(new_recipe.RecipeName), new_recipe);
                                                         }
                                                     }
                                                     catch (Exception ex)
                                                     {
                                                         Log.Error(ex, "");
                                                     }
                                                 }

                                                 await RefreshList();
                                                 Standby = true;

                                                 dialog?.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, $"{adds}個配方已新增\n{updates}個配方已更新" },
                                                                  { Language.CHS, $"{adds}个配方已新增\n{updates}个配方已更新" },
                                                                  { Language.EN, $"{adds}recipe{(adds > 1 ? "s" : "")} have been added\n{updates}recipe{(updates > 1 ? "s" : "")} have been updated" }
                                                              },
                                                              TimeSpan.FromSeconds(6));
                                             });

            ReNameCommand = new RelayCommand(async e =>
                                             {
                                                 if (string.IsNullOrEmpty(EditedName))
                                                 {
                                                     //dialog?.Show(new Dictionary<Language, string>
                                                     //             {
                                                     //                 { Language.TW, "配方名稱不可為空白！" }, { Language.CHS, "配方名称不可为空白！" }, { Language.EN, "Recipe Name cannot be blank!" }
                                                     //             },
                                                     //             DialogMsgType.Alert);

                                                     return;
                                                 }

                                                 if (Recipes.Any(x => string.Equals(x.RecipeName, EditedName, StringComparison.CurrentCultureIgnoreCase)))
                                                 {
                                                     dialog?.Show(new Dictionary<Language, string>
                                                                  {
                                                                      { Language.TW, "已有相同名稱！" }, { Language.CHS, "已有相同名称！" }, { Language.EN, "The same name already exists!" }
                                                                  },
                                                                  DialogMsgType.Alert);

                                                     return;
                                                 }

                                                 if (await dialog.Show(new Dictionary<Language, string>
                                                                       {
                                                                           { Language.TW, $"更改配方名稱：\n{TypedName} -> {EditedName}\n無法復原！ 確定更改？" },
                                                                           { Language.CHS, $"更改配方名称：\n{TypedName} -> {EditedName}\n无法复原！ 确定更改？" },
                                                                           { Language.EN, $"Change recipe name:\n{TypedName} -> {EditedName}\nCannot be restored! Are you sure?" }
                                                                       },
                                                                       true))
                                                 {
                                                     await RecipeCollection.UpdateOneAsync(x => x.RecipeName.ToLower() == TypedName.ToLower(), nameof(PLC_Recipe.RecipeName), EditedName);
                                                     await RecipeCollection_History.UpdateManyAsync(x => x.RecipeName.ToLower() == TypedName.ToLower(), nameof(PLC_Recipe.RecipeName), EditedName);
                                                     EditedName = "";
                                                     await RefreshList();
                                                 }
                                             });
        }
    }
}