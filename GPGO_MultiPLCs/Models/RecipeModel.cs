using GPGO_MultiPLCs.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Models
{
    public abstract class RecipeModel<T> : ObservableObject where T : RecipeBase<T>, new()
    {
        public readonly IDataBase<T> RecipeCollection;
        public readonly IDataBase<T> RecipeCollection_History;

        /// <summary>未儲存未修改之配方(備份或還原用)</summary>
        public T Selected_Recipe_Origin;

        public string UserName;

        /// <summary>所有配方的列表</summary>
        public List<T> Recipes;

        public virtual RelayCommand AddCommand { get; }
        public virtual RelayCommand DeleteCommand { get; }
        public virtual RelayCommand ExprotCommand { get; }
        public virtual RelayCommand ImportCommand { get; }

        /// <summary>讀取配方列表</summary>
        public virtual RelayCommand InitialLoadCommand { get; }

        /// <summary>重新讀取配方參數(未儲存時)</summary>
        public virtual RelayCommand ResetCommand { get; }

        public virtual RelayCommand ReNameCommand { get; }
        public virtual RelayCommand SaveCommand { get; }

        /// <summary>辨識是否可新增配方(列表中沒有和輸入名相同的配方)</summary>
        public virtual bool Add_Enable => !string.IsNullOrEmpty(TypedName) && Recipes.All(x => x.RecipeName != TypedName);

        /// <summary>辨識是否可刪除配方(列表中有和輸入名相同的配方，且該配方無烤箱正在使用)</summary>
        public virtual bool Delete_Enable => Selected_Recipe != null;

        /// <summary>辨別是否可儲存配方(有正在選取的配方)</summary>
        public virtual bool Save_Enable => Selected_Recipe != null && !Selected_Recipe.Equals(Selected_Recipe_Origin);

        public T SelectedHistory => Old_ViewRecipes == null || HistoryIndex >= Old_ViewRecipes.Count ? null : Old_ViewRecipes[HistoryIndex];

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
                Set(string.IsNullOrEmpty(TypedName) ? -1 : ViewRecipes?.ToList().FindIndex(x => x.RecipeName == TypedName) ?? -1, nameof(Selected_Recipe_Index));

                Selected_Recipe = string.IsNullOrEmpty(TypedName) ? null : Recipes?.FirstOrDefault(x => x.RecipeName == TypedName)?.Copy(UserName);

                NotifyPropertyChanged(nameof(Save_Enable));
                NotifyPropertyChanged(nameof(Add_Enable));
                NotifyPropertyChanged(nameof(Delete_Enable));
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

        public IList<T> Old_ViewRecipes
        {
            get => Get<IList<T>>();
            set
            {
                Set(value);
                HistoryIndex = value?.Count - 1 ?? 0;
            }
        }

        /// <summary>目前選取的配方</summary>
        public T Selected_Recipe
        {
            get => Get<T>();
            set
            {
                var old_value = Selected_Recipe;

                if (old_value != null)
                {
                    old_value.PropertyChanged -= RecipePropertyChanged;
                }

                Selected_Recipe_Origin = value?.Copy(UserName);
                Set(value);

                if (value == null)
                {
                    Old_ViewRecipes = null;
                }
                else
                {
                    Selected_Recipe.PropertyChanged += RecipePropertyChanged;

                    GetHistory(value.RecipeName);
                }
            }
        }

        /// <summary>目前選取配方在列表中的index</summary>
        public int Selected_Recipe_Index
        {
            get => Get<int>();
            set
            {
                if (value > -1)
                {
                    Set(ViewRecipes.ElementAt(value).RecipeName, nameof(TypedName));

                    Selected_Recipe = string.IsNullOrEmpty(TypedName) ? null : Recipes?.FirstOrDefault(x => x.RecipeName == TypedName)?.Copy(UserName);

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
        public IQueryable<T> ViewRecipes
        {
            get => Get<IQueryable<T>>();
            set => Set(value);
        }

        /// <summary>配方列表更新事件</summary>
        public event Action<(List<T> list, bool tip)> ListUpdatedEvent;

        /// <summary>單一配方讀取完成事件</summary>
        public event Action RecipeLoadedEvent;

        /// <summary>獲取指定配方</summary>
        /// <param name="name">配方名</param>
        /// <returns></returns>
        public T GetRecipe(string name)
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

        public async void GetHistory(string name)
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
        public async Task Load(string name)
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
        public async Task RefreshList(bool Tip)
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
                    ListUpdatedEvent?.Invoke((Recipes, Tip));
                }
            }
        }

        /// <summary>儲存配方</summary>
        /// <param name="name">配方名</param>
        /// <returns></returns>
        public async Task Save(string name)
        {
            Standby = false;

            var TempSet = Selected_Recipe == null ? (T)Activator.CreateInstance(typeof(T), name, UserName) : Selected_Recipe.Copy(UserName);

            try
            {
                await RecipeCollection.UpsertAsync(x => x.RecipeName.Equals(TempSet.RecipeName), TempSet);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
            }

            if (Selected_Recipe_Origin != null)
            {
                try
                {
                    await RecipeCollection_History.AddAsync(Selected_Recipe_Origin);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "");
                }
            }

            await RefreshList(true);

            Standby = true;
        }

        public RecipeModel(IDataBase<T> db, IDataBase<T> db_history, IDialogService dialog)
        {
            RecipeCollection = db;
            RecipeCollection_History = db_history;

            InitialLoadCommand = new RelayCommand(async e =>
            {
                if (Recipes == null || Recipes.Count == 0)
                {
                    Standby = false;

                    await RefreshList(false);

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
                if (await dialog.Show(new Dictionary<Language, string>{
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
                Selected_Recipe.CopyValue(UserName, SelectedHistory);

                if (await dialog.Show(new Dictionary<Language, string>{
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

                await RefreshList(false);

                Standby = true;
            });

            ExprotCommand = new RelayCommand(e =>
            {
                var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Recipes";

                if (SavetoJson(path))
                {
                    dialog?.Show(new Dictionary<Language, string>{
                                                                    { Language.TW, $"檔案已輸出至\n{path}" },
                                                                    { Language.CHS, $"档案已输出至\n{path}" },
                                                                    { Language.EN, $"The file has been output to\n{path}" }
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
                        var recipe = file.FullName.ReadFromJsonFile<T>();
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

                await RefreshList(false);
                Standby = true;

                dialog?.Show(new Dictionary<Language, string>{
                                                                { Language.TW, $"{adds}個配方已新增\n{updates}個配方已更新" },
                                                                { Language.CHS, $"{adds}个配方已新增\n{updates}个配方已更新" },
                                                                { Language.EN, $"{adds}recipe{(adds > 1 ? "s" : "")} have been added\n{updates}recipe{(updates > 1 ? "s" : "")} have been updated" }
                                                             },
                             TimeSpan.FromSeconds(6));
            });

            ReNameCommand = new RelayCommand(async e =>
            {
                var (result, input) = await dialog.ShowWithInput(new Dictionary<Language, string>{
                                                                                     { Language.TW, "請輸入新配方名稱" },
                                                                                     { Language.CHS, "请输入新配方名称" },
                                                                                     { Language.EN, "Please enter a new recipe name" }
                                                                                 },
                                                 new Dictionary<Language, string>{
                                                                                     { Language.TW, "更改配方名稱" },
                                                                                     { Language.CHS, "更改配方名称" },
                                                                                     { Language.EN, "Change recipe name" }
                                                                                 }
                                                 );

                var name = input.ToString();
                name = name.Replace(" ", "_");
                name = name.Length > 26 ? name.Substring(0, 26) : name;

                if (!result || string.IsNullOrEmpty(name)) return;

                if (Recipes.Any(x => string.Equals(x.RecipeName, name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    dialog?.Show(new Dictionary<Language, string>{
                                                                    { Language.TW, "已有相同名稱！" }, { Language.CHS, "已有相同名称！" }, { Language.EN, "The same name already exists!" }
                                                                 },
                                 DialogMsgType.Alert);

                    return;
                }

                if (await dialog.Show(new Dictionary<Language, string>{
                                                                        { Language.TW, $"更改配方名稱：\n{TypedName} -> {name}\n無法復原！ 確定更改？" },
                                                                        { Language.CHS, $"更改配方名称：\n{TypedName} -> {name}\n无法复原！ 确定更改？" },
                                                                        { Language.EN, $"Change recipe name:\n{TypedName} -> {name}\nCannot be restored! Are you sure?" }
                                                                      },
                                      true))
                {
                    await RecipeCollection.UpdateOneAsync(x => x.RecipeName.Equals(TypedName), nameof(RecipeBase<T>.RecipeName), name);
                    await RecipeCollection_History.UpdateManyAsync(x => x.RecipeName.Equals(TypedName), nameof(RecipeBase<T>.RecipeName), name);
                    await RefreshList(false);
                }
            });
        }
    }
}