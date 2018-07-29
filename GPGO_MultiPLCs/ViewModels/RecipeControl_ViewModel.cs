﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using MongoDB.Driver;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>配方管理</summary>
    public class RecipeControl_ViewModel : ViewModelBase
    {
        public delegate void ListUpdated(List<PLC_Recipe> list);

        private readonly IMongoCollection<PLC_Recipe> RecipeCollection;

        private string _SearchName;
        private PLC_Recipe _Selected_PLC_Recipe;
        private int _Selected_PLC_Recipe_Index;
        private bool _Standby;
        private string _TypedName;
        private IQueryable<PLC_Recipe> _ViewRecipes;

        /// <summary>所有配方的列表</summary>
        private List<PLC_Recipe> Recipes;

        /// <summary>辨識是否可新增配方(列表中沒有和輸入名相同的配方)</summary>
        public bool Add_Enable => !string.IsNullOrEmpty(_TypedName) && Recipes.All(x => x.RecipeName != _TypedName);

        public RelayCommand AddCommand { get; }

        /// <summary>辨識是否可刪除配方(列表中有和輸入名相同的配方，且該配方無烤箱正在使用)</summary>
        public bool Delete_Enable => _Selected_PLC_Recipe != null && !_Selected_PLC_Recipe.Used_Stations.Any(x => x);

        public RelayCommand DeleteCommand { get; }

        /// <summary>讀取配方列表</summary>
        public RelayCommand InitialLoadCommand { get; }

        /// <summary>重新讀取配方參數(未儲存時)</summary>
        public RelayCommand ResetCommand { get; }

        /// <summary>辨別是否可儲存配方(有正在選取的配方)</summary>
        public bool Save_Enable => _Selected_PLC_Recipe != null;

        public RelayCommand SaveCommand { get; }

        /// <summary>配方搜尋的關鍵字</summary>
        public string SearchName
        {
            get => _SearchName;
            set
            {
                value = value.Replace(" ", "_");
                _SearchName = value;
                NotifyPropertyChanged();

                ViewRecipes = string.IsNullOrEmpty(_SearchName) ? Recipes.AsQueryable() : Recipes?.AsQueryable().Where(x => x.RecipeName.ToLower().Contains(_SearchName.ToLower()));
            }
        }

        /// <summary>目前選取的配方</summary>
        public PLC_Recipe Selected_PLC_Recipe
        {
            get => _Selected_PLC_Recipe;
            set
            {
                _Selected_PLC_Recipe = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>目前選取配方在列表中的index</summary>
        public int Selected_PLC_Recipe_Index
        {
            get => _Selected_PLC_Recipe_Index;
            set
            {
                _Selected_PLC_Recipe_Index = value;

                if (_Selected_PLC_Recipe_Index > -1)
                {
                    _TypedName = _ViewRecipes.ElementAt(_Selected_PLC_Recipe_Index).RecipeName;
                    NotifyPropertyChanged(nameof(TypedName));

                    Selected_PLC_Recipe = string.IsNullOrEmpty(_TypedName) ? null : Recipes?.FirstOrDefault(x => x.RecipeName == _TypedName)?.Copy();

                    NotifyPropertyChanged(nameof(Save_Enable));
                    NotifyPropertyChanged(nameof(Add_Enable));
                    NotifyPropertyChanged(nameof(Delete_Enable));
                }

                NotifyPropertyChanged();
            }
        }

        /// <summary>辨識是否不在忙碌中</summary>
        public bool Standby
        {
            get => _Standby;
            set
            {
                _Standby = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>輸入/選定的配方名</summary>
        public string TypedName
        {
            get => _TypedName;
            set
            {
                value = value.Replace(" ", "_");
                _TypedName = value.Length > 26 ? value.Substring(0, 26) : value;
                NotifyPropertyChanged();

                _Selected_PLC_Recipe_Index = string.IsNullOrEmpty(_TypedName) ? -1 : _ViewRecipes?.ToList().FindIndex(x => x.RecipeName == _TypedName) ?? -1;
                NotifyPropertyChanged(nameof(Selected_PLC_Recipe_Index));

                Selected_PLC_Recipe = string.IsNullOrEmpty(_TypedName) ? null : Recipes?.FirstOrDefault(x => x.RecipeName == _TypedName)?.Copy();

                NotifyPropertyChanged(nameof(Save_Enable));
                NotifyPropertyChanged(nameof(Add_Enable));
                NotifyPropertyChanged(nameof(Delete_Enable));
            }
        }

        /// <summary>顯示的配方列表(依據搜尋條件)</summary>
        public IQueryable<PLC_Recipe> ViewRecipes
        {
            get => _ViewRecipes;
            set
            {
                _ViewRecipes = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>配方列表更新事件</summary>
        public event ListUpdated ListUpdatedEvent;

        /// <summary>單一配方讀取完成事件</summary>
        public event Action RecipeLoadedEvent;

        /// <summary>獲取指定配方</summary>
        /// <param name="index">烤箱站號</param>
        /// <param name="name">配方名</param>
        /// <returns></returns>
        public async Task<PLC_Recipe> GetRecipe(int index, string name)
        {
            var result = Recipes.FirstOrDefault(x => x.RecipeName == name);

            if (result != null)
            {
                try
                {
                    foreach (var recipe in Recipes.Where(x => x.Used_Stations[index]))
                    {
                        recipe.Used_Stations[index] = false;
                        await RecipeCollection.UpdateOneAsync(x => x.RecipeName.Equals(recipe.RecipeName), Builders<PLC_Recipe>.Update.Set(x => x.Used_Stations, recipe.Used_Stations));
                    }

                    result.Used_Stations[index] = true;
                    await RecipeCollection.UpdateOneAsync(x => x.RecipeName.Equals(result.RecipeName), Builders<PLC_Recipe>.Update.Set(x => x.Used_Stations, result.Used_Stations));

                    //ViewRecipes = Recipes.Where(x => string.IsNullOrEmpty(_SearchName) || x.RecipeName.ToLower().Contains(_SearchName.ToLower())).ToList();
                }
                catch (Exception ex)
                {
                    ErrorRecoder.RecordError(ex, "配方資料庫更新使用站點資訊失敗");
                }
            }

            return result;
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
                    var temp = await (await RecipeCollection.FindAsync(x => x.RecipeName.Equals(name))).ToListAsync();

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

        /// <summary>更新配方列表</summary>
        /// <returns></returns>
        private async Task RefreshList()
        {
            try
            {
                TypedName = "";

                Recipes = await (await RecipeCollection.FindAsync(x => true)).ToListAsync();
                ViewRecipes = Recipes?.AsQueryable().Where(x => string.IsNullOrEmpty(_SearchName) || x.RecipeName.ToLower().Contains(_SearchName.ToLower()));
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

        /// <summary>儲存配方</summary>
        /// <param name="name">配方名</param>
        /// <returns></returns>
        private async Task Save(string name)
        {
            Standby = false;

            var TempSet = Selected_PLC_Recipe == null ? new PLC_Recipe(name) : Selected_PLC_Recipe.Copy();

            try
            {
                await RecipeCollection.ReplaceOneAsync(x => x.RecipeName.Equals(TempSet.RecipeName), TempSet, new UpdateOptions { IsUpsert = true });
            }
            catch (Exception ex)
            {
                ErrorRecoder.RecordError(ex);
            }

            await RefreshList();

            Standby = true;
        }

        public RecipeControl_ViewModel(IMongoCollection<PLC_Recipe> mongo, IDialogService<string> dialog)
        {
            RecipeCollection = mongo;

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
                                                                         { Language.TW, "即將儲存並覆蓋同名配方，無法復原!\n" + "確定儲存?" },
                                                                         { Language.CHS, "即将储存并覆盖同名配方，无法复原!\n" + "确定储存?" },
                                                                         {
                                                                             Language.EN,
                                                                             "The recipe is going to save or replace the same one,\n" + "Can't be restored!" + " OK?"
                                                                         }
                                                                     },
                                                                     true,
                                                                     DialogMsgType.Alert))
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
                                                     await RecipeCollection.DeleteOneAsync(x => x.RecipeName.Equals(_TypedName));
                                                 }
                                                 catch (Exception)
                                                 {
                                                 }

                                                 await RefreshList();

                                                 Standby = true;
                                             });
        }
    }
}