using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GPGRC_MultiPLCs.Models;
using GPGRC_MultiPLCs.ViewModels;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.Models.SECS.ITRISecs;
using GPMVVM.MongoDB.Helpers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;
using Extensions = GPGRC_MultiPLCs.Helpers.Extensions;
#pragma warning disable VSTHRD101

namespace GPGRC_MultiPLCs;

public sealed class Mediator : ObservableObject
{
    private readonly AsyncLock     lockobj    = new();
    private readonly DataoutputCSV CsvCreator = new();

    public  NetworkStream _streamFromServer = default;

    public bool ToConnect
    {
        get => Get<bool>();
        set => Set(value);
    }

    private bool _closed = true;

    public bool closed
    {
        get { return _closed; }
        set { _closed = value; }
    }

    public Language Language
    {
        get => Get<Language>();
        private set
        {
            Set(value);

            DialogVM.Language = value;
            TraceVM.Language = value;
            LogVM.Language = value;
        }
    }

    public int CoaterCount
    {
        get => Get<int>();
        private set
        {
            Set(value);

            TotalVM.CoaterCount = value;
        }
    }

    public User? User
    {
        get => Get<User>();
        private set
        {
            value ??= new User
            {
                Name = "Guest",
                Password = "",
                Level = UserLevel.Guest
            };

            Set(value);
            RecipeVM.UserName = value.Name;
            RecipeVM.UserLevel = value.Level;
        }
    }

    public int RecordDelay
    {
        get => Get<int>();
        private set
        {
            Set(value);

            foreach (var plc in TotalVM.PLC_All)
            {
                plc.RecordDelay = value;
            }
        }
    }

    public int ClearInputDelay
    {
        get => Get<int>();
        private set
        {
            Set(value);

            foreach (var plc in TotalVM.PLC_All)
            {
                plc.ClearInputDelay = value;
            }
        }
    }

    public Authenticator_ViewModel AuthenticatorVM { get; }
    public GlobalDialog_ViewModel DialogVM { get; }
    public LogView_ViewModel LogVM { get; }
    public MainWindow_ViewModel MainVM { get; }
    public RecipeControl_ViewModel RecipeVM { get; }
    public TotalView_ViewModel TotalVM { get; }
    public TraceabilityView_ViewModel TraceVM { get; }
    public IGate PlcGate { get; }

    public Mediator()
    {
        var db = new MongoClient("mongodb://localhost:27017").GetDatabase("GP_GRC");
        DialogVM = new GlobalDialog_ViewModel();
        MainVM = new MainWindow_ViewModel();
        RecipeVM = new RecipeControl_ViewModel(new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("PLC_Recipes")),
                                               new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("Old_PLC_Recipes")),
                                               DialogVM);
        TraceVM = new TraceabilityView_ViewModel(new MongoBase<ProcessInfo>(db.GetCollection<ProcessInfo>("ProductInfos")), DialogVM);
        LogVM = new LogView_ViewModel(new MongoBase<LogEvent>(db.GetCollection<LogEvent>("EventLogs")), DialogVM);
        PlcGate = new JsonRPCPLCGate();
        AuthenticatorVM = new Authenticator_ViewModel();
        TotalVM = new TotalView_ViewModel(AuthenticatorVM.Settings.CoaterCount, PlcGate, IPAddress.Parse(AuthenticatorVM.IPString), DialogVM);
        Language = AuthenticatorVM.Settings.Lng;
        CoaterCount = AuthenticatorVM.Settings.CoaterCount;

        #region AuthenticatorVM
        AuthenticatorVM.NowUser = new User
        {
            Name = "Guest",
            Password = "",
            Level = UserLevel.Guest
        };
        User = AuthenticatorVM.NowUser;

        AuthenticatorVM.Settings.PropertyChanged += (s, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(GlobalSettings.PLCIP):
                    DialogVM.Show(new Dictionary<Language, string>
                                                                              {
                                                                                  { Language.TW, "系統設定變更，請重新啟動程式！" },
                                                                                  { Language.CHS, "系统设定变更，请重新启动程序！" },
                                                                                  { Language.EN, "System settings changed, please restart the program." }
                                                                              });
                    break;
                case nameof(GlobalSettings.Lng):
                    Language = ((GlobalSettings)s).Lng;
                    break;
                case nameof(GlobalSettings.CoaterCount):
                    CoaterCount = ((GlobalSettings)s).CoaterCount;
                    break;
                case nameof(GlobalSettings.RecordDelay):
                    RecordDelay = ((GlobalSettings)s).RecordDelay;
                    break;
                case nameof(GlobalSettings.ClearInputDelay):
                    ClearInputDelay = ((GlobalSettings)s).ClearInputDelay;
                    break;
            }
        };

        AuthenticatorVM.PropertyChanged += async (s, e) =>
        {
            if (e.PropertyName == nameof(Authenticator_ViewModel.NowUser))
            {
                User = ((Authenticator_ViewModel)s).NowUser;
                Extensions.IsGodMode = User?.Level >= UserLevel.Administrator;
            }
        };
        #endregion

        #region MainVM
        //! 當回到主頁時，也將生產總覽回到總覽頁
        MainVM.IndexChangedEvent += i =>
        {
            TotalVM.Index = 0;
            TraceVM.SelectedIndex = -1;
            TraceVM.ShowProducts = false;
        };

        //! 當主視窗讀取完成時，再讀取配方和生產履歷資料庫
        MainVM.LoadedEvent += dp =>
        {
            if (dp == null)
            {
                return;
            }

            TotalVM.StartPLCGate();

            _ = dp.InvokeAsync(() =>
            {
                RecipeVM.InitialLoadCommand.Execute(null);
                TraceVM.TodayCommand.Execute(null);
            },
                               DispatcherPriority.SystemIdle);
        };

        //! 當OP試圖關閉程式時，進行狀態和權限檢查
        MainVM.CheckClosing += async () =>
        {
            if (TotalVM.PLC_All.Any(plc => plc.IsExecuting))
            {
                DialogVM.Show(new Dictionary<Language, string>
                                                     {
                                                         { Language.TW, "仍在生產中，無法終止程式！" },
                                                         { Language.CHS, "仍在生产中，无法终止程序！" },
                                                         { Language.EN, "Still processing,\ncannot terminate the program." }
                                                     });
            }
            else if (User.Level > UserLevel.Operator)
            {
                var user = User.Copy()!;
                var result = await DialogVM.CheckCondition(new Dictionary<Language, string>
                                                                                  {
                                                                                      { Language.TW, "請輸入權限密碼：" },
                                                                                      { Language.CHS, "请输入权限密码：" },
                                                                                      { Language.EN, "Please enter the permission password:" }
                                                                                  },
                                                                                  new Dictionary<Language, string>
                                                                                  {
                                                                                      { Language.TW, "驗證" },
                                                                                      { Language.CHS, "验证" },
                                                                                      { Language.EN, "Identify" }
                                                                                  },
                                                                                  true,
                                                                                  x => (x.ToString() == user.Password,
                                                                                        new Dictionary<Language, string>
                                                                                        {
                                                                                            { Language.TW, "密碼錯誤！" },
                                                                                            { Language.CHS, "密码错误！" },
                                                                                            { Language.EN, "Wrong password!" }
                                                                                        }));

                if (result.result)
                {
                    var sb = new StringBuilder();
                    sb.Append(user.Name);
                    sb.Append(", Level:");
                    sb.Append(user.Level.ToString());
                    sb.Append(", App ShutDown.");
                    await LogVM.AddToDBAsync(new LogEvent
                    {
                        AddedTime = DateTime.Now,
                        StationNumber = 0,
                        Type = EventType.Operator,
                        Description = sb.ToString(),
                        Value = true
                    });
                    Application.Current.Shutdown(23555277);
                }
            }
            else
            {
                DialogVM.Show(new Dictionary<Language, string>
                                                     {
                                                         { Language.TW, "權限不足，不可關閉程式！" },
                                                         { Language.CHS, "权限不足，不可关闭程序！" },
                                                         { Language.EN, "Insufficient permissions,\ncan't close the program." }
                                                     });
            }
        };
        #endregion

        #region RecipeVM
        //! 當配方列表更新時，依據使用站別發佈配方
        RecipeVM.ListUpdatedEvent += async e =>
        {
            var (list, added, removed, updated, showtip) = e;
            if (list != null)
            {
                TotalVM.SetRecipeNames(list.Select(x => x.RecipeName).ToList());

                var path = $"{TotalVM.SecsGemEquipment.BasePath}\\ProcessJob";

                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "ProcessJob資料夾無法創建");
                    }
                }
                else
                {
                    var files = new DirectoryInfo(path).GetFiles("*.pjb");
                    foreach (var file in files)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "ProcessJob資料夾內檔案無法刪除");
                        }
                    }
                }

                if (TotalVM.SecsGemEquipment.CCodeDocument?.CCodeItems.TryGetValue("1", out var ccode) == true)
                {
                    foreach (var recipe in list)
                    {
                        var _recipe = recipe.ToDictionary();
                        var fpath   = $"{path}\\{recipe.RecipeName}.pjb";
                        var ini     = new IniParser(fpath);

                        foreach (var param in ccode.ParameterItems)
                        {
                            if (_recipe.TryGetValue(param.PParameterName, out var val))
                            {
                                if (val is double d)
                                {
                                    ini[ccode.CCodeName][param.PParameterName] = d.ToString("0.0").ToUpper();
                                }
                                else if (val is float f)
                                {
                                    ini[ccode.CCodeName][param.PParameterName] = f.ToString("0.0").ToUpper();
                                }
                                else
                                {
                                    ini[ccode.CCodeName][param.PParameterName] = val.ToString().ToUpper();
                                }
                            }
                        }

                        try
                        {
                            await ini.SaveAsync();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "pjb寫入失敗");
                        }
                    }
                }
                //! 輸出欣興Recipe CSV
                //await CsvCreator.ExportRecipe(list, AuthenticatorVM.Settings.DataOutputPath);
            }

            var sb = new StringBuilder();
            if (added?.Count > 0)
            {
                sb.AppendLine($"Added:{string.Join(",", added.Select(x => x.RecipeName))}");
            }
            if (removed?.Count > 0)
            {
                sb.AppendLine($"Removed:{string.Join(",", removed.Select(x => x.RecipeName))}");
            }
            if (updated?.Count > 0)
            {
                sb.AppendLine($"Updated:{string.Join(",", updated.Select(x => x.RecipeName))}");
            }

            if (sb.Length > 0)
            {
                await LogVM.AddToDBAsync(new LogEvent
                {
                    AddedTime = DateTime.Now,
                    StationNumber = 0,
                    Type = EventType.RecipeChanged,
                    Description = sb.ToString().TrimEnd('\r', '\n'),
                    Value = true
                });
            }

            if (added != null)
            {
                foreach (var add in added)
                {
                    TotalVM.InvokeRecipe(add.RecipeName, PPStatus.Create);
                }
            }

            if (removed != null)
            {
                foreach (var remove in removed)
                {
                    TotalVM.InvokeRecipe(remove.RecipeName, PPStatus.Delete);
                }
            }

            if (updated != null)
            {
                foreach (var update in updated)
                {
                    TotalVM.InvokeRecipe(update.RecipeName, PPStatus.Change);
                }
            }
        };
        #endregion

        #region TotalVM
        TotalVM.CheckUser += op => AuthenticatorVM.UserList.List.Keys.Any(x => x.ToUpper() == op);

        //! 當某站烤箱要求配方時，自資料庫讀取配方並發送
        TotalVM.GetRecipe += recipename => string.IsNullOrEmpty(recipename) ? null : RecipeVM.GetRecipe(recipename);

        TotalVM.CheckRecipeCommand_KeyIn += e =>
        {
            TotalVM.PLC_All[0].CheckRecipeCommand_KeyIn.Execute(e);
            TotalVM.PLC_All[1].CheckRecipeCommand_KeyIn.Execute(e);
            TotalVM.PLC_All[2].CheckRecipeCommand_KeyIn.Execute(e);
        }
        ;

        TotalVM.GetRecipeList += () =>
        {
            if (RecipeVM.ViewRecipes != null)
                return RecipeVM.ViewRecipes.Select(x => x.RecipeName).ToList();
            else
                return null;
        };

        //! 當某站烤箱完成烘烤程序時，將生產資訊寫入資料庫並輸出至上傳資料夾，並回傳當日產量
        TotalVM.AddRecordToDB += async e =>
        {
            var (stationIndex, info) = e;

            //! 確認資料是否小於bson限制，否則直接將溫度記錄每2筆移除1筆(砍半)
            await Task.Run(() =>
            {
                while (!info.CheckBosnSizeIsOK())
                {
                    info.RecordTemperatures.RemoveEvery(2, 0, x => x.KeyPoint);
                }
            });

            using (await lockobj.LockAsync())
            {
                await TraceVM.AddToDBAsync(stationIndex, info);

                //! 輸出欣興CSV紀錄
                //await CsvCreator.AddInfo(info, AuthenticatorVM.Settings.DataOutputPath);
            }

            return await TraceVM.CheckProductions(stationIndex);
        };

        TotalVM.EventHappened += e =>
        {
            var (stationIndex, type, time, note, tag, value) = e;
            var logevent = new LogEvent
            {
                StationNumber = stationIndex + 1,
                AddedTime     = time,
                Type          = type,
                Description   = note,
                TagCode       = tag,
                Value         = value
            };
            _ = LogVM.AddToDBAsync(logevent);

            //! 輸出欣興CSV紀錄
            //_ = CsvCreator.AddEvent(logevent, AuthenticatorVM.Settings.DataOutputPath);
        };


        TotalVM.UpsertRecipe += recipe => RecipeVM.Upsert(recipe).Result;

        TotalVM.DeleteRecipe += recipe => RecipeVM.Delete(recipe).Result;
        #endregion

        #region LogVM
        LogVM.WantInfo += e => TraceVM.FindInfo(e.station, e.time);

        LogVM.GoDetailView += async e =>
        {
            TraceVM.Standby = false; //! 強制讓TraceVM處於須等待狀態，因此時畫面仍在變化仍未loaded，但TraceVM.Standby為true，將導致以下的迴圈等待沒效果
            MainVM.ViewIndex = 2;
            var (info, logEvent) = e;

            if (await Task.Factory.StartNew(() => SpinWait.SpinUntil(() => TraceVM.Standby, 3000),
                                            CancellationToken.None,
                                            TaskCreationOptions.None,
                                            TaskScheduler.Default))
            {
                await Task.Delay(150);

                TraceVM.SearchResult = info;
                TraceVM.SearchEvent = logEvent;
                TraceVM.Date1 = info.AddedTime.Date;
            }
        };

        LogVM.LogAdded += log => TotalVM.InsertMessage(log);

        _ = Task.Run(() =>
        {
            using var evs = LogVM.DataCollection.Find(x => x.AddedTime > DateTime.Now.AddDays(-1) && x.AddedTime <= DateTime.Now && x.Type > EventType.StatusChanged).OrderByDescending(x => x.AddedTime).Take(50).ToPooledList();
            TotalVM.InsertMessage(evs);
        });
        #endregion

        #region 產生測試用生產數據資料庫，務必先建立配方！！
        //DialogVM.Show(new Dictionary<Language, string>
        //              {
        //                  { Language.TW, "測試資料產生中，請稍後！" },
        //                  { Language.CHS, "测试数据产生中，请稍后！" },
        //                  { Language.EN, "Testing data is being generated, please wait!" }
        //              },
        //              Task.Factory.StartNew(() =>
        //                                    {
        //                                        try
        //                                        {
        //                                            SpinWait.SpinUntil(() => false, 1200);

        //                                            MakeTestData(1);

        //                                            var evs = LogVM.DataCollection.Find(x => x.AddedTime > DateTime.Now.AddDays(-1)).Where(x => (int)x.Type > 1).Take(50).ToPooledList();
        //                                            TotalVM.InsertMessage(evs);
        //                                        }
        //                                        catch
        //                                        {
        //                                            ignored
        //                                        }
        //                                    }),
        //              TimeSpan.FromMinutes(5));
        #endregion
    }

}