using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>配方管理</summary>
    public class RecipeControl_ViewModel : RecipeModel<PLC_Recipe>
    {
        public override RelayCommand AddCommand { get; }
        public override RelayCommand DeleteCommand { get; }
        public override RelayCommand ExprotCommand { get; }
        public override RelayCommand ImportCommand { get; }

        /// <summary>讀取配方列表</summary>
        public override RelayCommand InitialLoadCommand { get; }

        /// <summary>重新讀取配方參數(未儲存時)</summary>
        public override RelayCommand ResetCommand { get; }

        public override RelayCommand ReNameCommand { get; }

        public override RelayCommand SaveCommand { get; }

        /// <summary>辨識是否可刪除配方(列表中有和輸入名相同的配方，且該配方無烤箱正在使用)</summary>
        public override bool Delete_Enable => Selected_Recipe != null && !Selected_Recipe.Used_Stations.Any(x => x);

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

        public RecipeControl_ViewModel(IDataBase<PLC_Recipe> db, IDataBase<PLC_Recipe> db_history, IDialogService dialog) : base(db, db_history)
        {
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
                                                Selected_Recipe.CopyValue(UserName, SelectedHistory);

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

                                                 await RefreshList(false);

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

                                                 await RefreshList(false);
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
                                                     await RefreshList(false);
                                                 }
                                             });
        }
    }
}