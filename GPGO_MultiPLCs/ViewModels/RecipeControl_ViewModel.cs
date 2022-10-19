using GPGO_MultiPLCs.Models;
using GPMVVM.Helpers;
using GPMVVM.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.ViewModels;

/// <summary>配方管理</summary>
public class RecipeControl_ViewModel : RecipeModel<PLC_Recipe>
{
    private Dictionary<string, int> RecipeTitles;

    public override RelayCommand ExprotCommand { get; }
    public override RelayCommand ImportCommand { get; }

    /// <summary>辨識是否可刪除配方(列表中有和輸入名相同的配方，且該配方無烤箱正在使用)</summary>
    public override bool DeleteEnable => SelectedRecipe != null && !SelectedRecipe.Used_Stations.Any(x => x);

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
                    await RecipeCollection.UpdateOneAsync(x => x.RecipeName.Equals(recipe.RecipeName), nameof(PLC_Recipe.Used_Stations), recipe.Used_Stations).ConfigureAwait(false);
                }

                result.Used_Stations[index] = true;
                await RecipeCollection.UpdateOneAsync(x => x.RecipeName.Equals(result.RecipeName), nameof(PLC_Recipe.Used_Stations), result.Used_Stations).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "配方資料庫更新使用站點資訊失敗");
            }
        }
    }

    public async ValueTask<bool> Upsert(PLC_Recipe recipe)
    {
        Standby = false;

        var result = false;
        if (recipe != null)
        {
            var name = recipe.RecipeName;
            if (!string.IsNullOrEmpty(recipe.RecipeName))
            {
                try
                {
                    var TempSet = await RecipeCollection.FindAsync(x => x.RecipeName.Equals(name)).ConfigureAwait(false);

                    if (TempSet.Any())
                    {
                        if (!TempSet[0].Equals(recipe))
                        {
                            await RecipeCollection.UpsertAsync(x => x.RecipeName.Equals(name), recipe).ConfigureAwait(false);
                            await RecipeCollection_History.AddAsync(TempSet[0]).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await RecipeCollection.AddAsync(recipe).ConfigureAwait(false);
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "");
                }

                await RefreshList(true).ConfigureAwait(false);
            }
        }

        Standby = true;

        return result;
    }

    public async ValueTask<bool> Delete(string recipeName)
    {
        Standby = false;

        var result = false;

        try
        {
            await RecipeCollection.DeleteOneAsync(x => x.RecipeName.Equals(recipeName)).ConfigureAwait(false);
            await RecipeCollection_History.DeleteOneAsync(x => x.RecipeName.Equals(recipeName)).ConfigureAwait(false);

            result = true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "");
        }

        await RefreshList(true).ConfigureAwait(false);

        Standby = true;

        return result;
    }

    public RecipeControl_ViewModel(IDataBase<PLC_Recipe> db, IDataBase<PLC_Recipe> db_history, IDialogService dialog) : base(db, db_history, dialog)
    {
        var i = 0;
        RecipeTitles = new PLC_Recipe().ToDictionary().Keys.ToDictionary(x => x, _ => i++);

        ExprotCommand = new RelayCommand(_ =>
                                         {
                                             var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Recipes";

                                             if (SavetoJson(path))
                                             {
                                                 dialog?.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, $"檔案已輸出至\n{path}" },
                                                                  { Language.CHS, $"档案已输出至\n{path}" },
                                                                  { Language.EN, $"The file has been output to\n{path}" }
                                                              },
                                                              TimeSpan.FromSeconds(6));
                                             }
                                         });

        ImportCommand = new RelayCommand(async path =>
                                         {
                                             Standby = false;

                                             if (path is not string filepath || !File.Exists(filepath))
                                             {
                                                 Standby = true;
                                                 return;
                                             }

                                             var recipies = FastCSV.ReadFile<PLC_Recipe>(filepath,
                                                                                        ',',
                                                                                        (recipe, col) =>
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                var dic    = RecipeTitles.ToDictionary(x => x.Key, x => col[x.Value]);
                                                                                                var result = recipe.SetByDictionary(dic);
                                                                                                return result;
                                                                                            }
                                                                                            catch (Exception ex)
                                                                                            {
                                                                                                Log.Error(ex, "ImportRecipe失敗");
                                                                                                return false;
                                                                                            }
                                                                                        });

                                             var updates = 0;
                                             var adds    = 0;

                                             foreach (var recipe in recipies)
                                             {
                                                 try
                                                 {
                                                     if (recipe != null)
                                                     {
                                                         var old_recipe = Recipes?.FirstOrDefault(x => x.RecipeName == recipe.RecipeName);
                                                         var new_recipe = recipe.Copy(UserName, UserLevel);

                                                         if (old_recipe != null)
                                                         {
                                                             if (old_recipe.Equals(new_recipe))
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
    }
}