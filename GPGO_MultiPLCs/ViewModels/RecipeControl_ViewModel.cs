using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPGRC_MultiPLCs.Models;
using GPMVVM.Helpers;
using GPMVVM.Models;
using Serilog;

namespace GPGRC_MultiPLCs.ViewModels;

/// <summary>配方管理</summary>
public class RecipeControl_ViewModel : RecipeModel<PLC_Recipe>
{
    private Dictionary<string, int> RecipeTitles;

    public override RelayCommand ExprotCommand { get; }

    /// <summary>辨識是否可刪除配方(列表中有和輸入名相同的配方，且該配方無烤箱正在使用)</summary>
    public override bool DeleteEnable => SelectedRecipe != null;

    public RecipeControl_ViewModel(IDataBase<PLC_Recipe> db, IDataBase<PLC_Recipe> db_history, IDialogService dialog) : base(db, db_history, dialog)
    {
        var i = 0;
        RecipeTitles = new PLC_Recipe().ToDictionary().Keys.ToDictionary(x => x, _ => i++);

        ExprotCommand = new RelayCommand(_ =>
                                         {
                                             var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Recipes";

                                             if (SavetoJson(path))
                                             {
                                                 dialog.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, $"檔案已輸出至\n{path}" },
                                                                  { Language.CHS, $"档案已输出至\n{path}" },
                                                                  { Language.EN, $"The file has been output to\n{path}" }
                                                              },
                                                              TimeSpan.FromSeconds(6));
                                             }
                                         });
    }

    public async Task<bool> Upsert(PLC_Recipe? recipe)
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

    public async Task<bool> Delete(string recipeName)
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
}