using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GPGO_MultiPLCs.Models;
using GPGO_MultiPLCs.ViewModels;
using GPMVVM.Core.Helpers;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PooledCollections;
using GPMVVM.SECSGEM;
using MongoDB.Driver;
using Serilog;

namespace GPGO_MultiPLCs;

public sealed class Mediator : ObservableObject
{
    private readonly AsyncLock     lockobj    = new();
    private readonly DataoutputCSV CsvCreator = new();

    public Language Language
    {
        get => Get<Language>();
        private set
        {
            Set(value);

            DialogVM.Language = value;
            TraceVM.Language  = value;
            LogVM.Language    = value;
        }
    }

    public int OvenCount
    {
        get => Get<int>();
        private set
        {
            Set(value);

            TotalVM.OvenCount = value;
        }
    }

    public User? User
    {
        get => Get<User>();
        private set
        {
            value ??= new User
                      {
                          Name     = "Guest",
                          Password = "",
                          Level    = UserLevel.Guest
                      };

            Set(value);
            RecipeVM.UserName  = value.Name;
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
                plc.Delay = value;
            }
        }
    }

    public Authenticator_ViewModel    AuthenticatorVM { get; }
    public GlobalDialog_ViewModel     DialogVM        { get; }
    public LogView_ViewModel          LogVM           { get; }
    public MainWindow_ViewModel       MainVM          { get; }
    public RecipeControl_ViewModel    RecipeVM        { get; }
    public TotalView_ViewModel        TotalVM         { get; }
    public TraceabilityView_ViewModel TraceVM         { get; }
    public IGate                      PlcGate         { get; }

    public Mediator()
    {
        var db = new MongoClient("mongodb://localhost:27017").GetDatabase("GP");

        DialogVM = new GlobalDialog_ViewModel();
        MainVM   = new MainWindow_ViewModel();
        RecipeVM = new RecipeControl_ViewModel(new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("PLC_Recipes")),
                                               new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("Old_PLC_Recipes")),
                                               DialogVM);

        TraceVM = new TraceabilityView_ViewModel(new MongoBase<ProcessInfo>(db.GetCollection<ProcessInfo>("ProductInfos")), DialogVM);
        LogVM   = new LogView_ViewModel(new MongoBase<LogEvent>(db.GetCollection<LogEvent>("EventLogs")), DialogVM);

        PlcGate = new JsonRPCPLCGate();

        AuthenticatorVM = new Authenticator_ViewModel();
        TotalVM         = new TotalView_ViewModel(AuthenticatorVM.Settings.OvenCount, PlcGate, IPAddress.Parse(AuthenticatorVM.IPString), DialogVM);
        Language        = AuthenticatorVM.Settings.Lng;
        OvenCount       = AuthenticatorVM.Settings.OvenCount;
        AuthenticatorVM.NowUser = new User
                                  {
                                      Name     = "Guest",
                                      Password = "",
                                      Level    = UserLevel.Guest
                                  };
        User = AuthenticatorVM.NowUser;
        //Helpers.Extensions.ReaderName = AuthenticatorVM.Settings.CodeReaderName;

        AuthenticatorVM.Settings.PropertyChanged += (s, e) =>
                                                    {
                                                        switch (e.PropertyName)
                                                        {
                                                            //case nameof(GlobalSettings.CodeReaderName):
                                                            //    Helpers.Extensions.ReaderName = ((GlobalSettings)s).CodeReaderName;
                                                            //    break;
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
                                                            case nameof(GlobalSettings.OvenCount):
                                                                OvenCount = ((GlobalSettings)s).OvenCount;
                                                                break;
                                                        }
                                                    };

        AuthenticatorVM.PropertyChanged += (s, e) =>
                                           {
                                               if (e.PropertyName == nameof(Authenticator_ViewModel.NowUser))
                                               {
                                                   User = ((Authenticator_ViewModel)s).NowUser;
                                               }
                                           };

        //! 當回到主頁時，也將生產總覽回到總覽頁
        MainVM.IndexChangedEvent += i =>
                                    {
                                        if (i == 0 && !TotalVM.PLC_All[0].IsExecuting)
                                        {
                                            TotalVM.Index = 0;
                                        }

                                        TraceVM.SelectedIndex = -1;
                                        TraceVM.ShowProducts  = false;
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
                                                                        AddedTime     = DateTime.Now,
                                                                        StationNumber = 0,
                                                                        Type          = EventType.Operator,
                                                                        Description   = sb.ToString(),
                                                                        Value         = true
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

        //! 當配方列表更新時，依據使用站別發佈配方
        RecipeVM.ListUpdatedEvent += async e =>
                                     {
                                         var (list, added, removed, updated, showtip) = e;
                                         if (list != null)
                                         {
                                             TotalVM.SetRecipeNames(list.Select(x => x.RecipeName).ToList());

                                             const string path = "C:\\ITRIinit\\0\\ProcessJob";

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

                                             var ccode = TotalVM.SecsGemEquipment.CCodeDocument.CCodeItems[0];
                                             foreach (var recipe in list)
                                             {
                                                 var _recipe = recipe.ToDictionary();
                                                 var fpath   = $"{path}\\{recipe.RecipeName}.pjb";
                                                 var ini     = new IniParser(fpath);

                                                 foreach (var parm in ccode.ParameterItems)
                                                 {
                                                     if (_recipe.TryGetValue(parm.PParameterName, out var val))
                                                     {
                                                         ini[ccode.CCodeName][parm.PParameterName] = val.ToString().ToUpper();
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

                                             //! 輸出欣興Recipe CSV
                                             await CsvCreator.ExportRecipe(list, AuthenticatorVM.Settings.DataOutputPath);
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
                                                                          AddedTime     = DateTime.Now,
                                                                          StationNumber = 0,
                                                                          Type          = EventType.RecipeChanged,
                                                                          Description   = sb.ToString().TrimEnd('\r', '\n'),
                                                                          Value         = true
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

        TotalVM.WantLogin += () => AuthenticatorVM.StartLogin.Execute(null);

        //! 當某站烤箱要求配方時，自資料庫讀取配方並發送
        TotalVM.GetRecipe += e => string.IsNullOrEmpty(e.RecipeName) ? null : RecipeVM.GetRecipe(e.RecipeName);

        //! 設定配方被該站使用
        TotalVM.RecipeUsed += e => RecipeVM.SetUsed(e.StationIndex, e.RecipeName);

        //! 當某站烤箱完成烘烤程序時，將生產資訊寫入資料庫並輸出至上傳資料夾，並回傳當日產量
        TotalVM.AddRecordToDB += async e =>
                                 {
                                     var (stationIndex, info) = e;
                                     using (await lockobj.LockAsync())
                                     {
                                         await TraceVM.AddToDBAsync(stationIndex, info);

                                         //! 輸出欣興CSV紀錄
                                         await CsvCreator.AddInfo(info, AuthenticatorVM.Settings.DataOutputPath);
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
                                     _ = CsvCreator.AddEvent(logevent, AuthenticatorVM.Settings.DataOutputPath);
                                 };

        TotalVM.UpsertRecipe += recipe => RecipeVM.Upsert(recipe).Result;

        TotalVM.DeleteRecipe += recipe => RecipeVM.Delete(recipe).Result;

        LogVM.WantInfo += e => TraceVM.FindInfo(e.station, e.time);

        LogVM.GoDetailView += async e =>
                              {
                                  TraceVM.Standby      = false; //! 強制讓TraceVM處於須等待狀態，因此時畫面仍在變化仍未loaded，但TraceVM.Standby為true，將導致以下的迴圈等待沒效果
                                  MainVM.ViewIndex     = 2;
                                  var (info, logEvent) = e;

                                  if (await Task.Factory.StartNew(() => SpinWait.SpinUntil(() => TraceVM.Standby, 3000),
                                                                  CancellationToken.None,
                                                                  TaskCreationOptions.None,
                                                                  TaskScheduler.Default))
                                  {
                                      await Task.Delay(150);

                                      TraceVM.SearchResult = info;
                                      TraceVM.SearchEvent  = logEvent;
                                      TraceVM.Date1        = info.AddedTime.Date;
                                  }
                              };

        LogVM.LogAdded += log => TotalVM.InsertMessage(log);

        _ = Task.Run(() =>
                     {
                         using var evs = LogVM.DataCollection.Find(x => x.AddedTime > DateTime.Now.AddDays(-1) && x.AddedTime <= DateTime.Now && x.Type > EventType.StatusChanged).OrderByDescending(x => x.AddedTime).Take(50).ToPooledList();
                         TotalVM.InsertMessage(evs);
                     });

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
        //                                            // ignored
        //                                        }
        //                                    }),
        //              TimeSpan.FromMinutes(5));
        #endregion
    }

    /// <summary>產生測試資料至資料庫</summary>
    /// <param name="PLC_Count"></param>
    public void MakeTestData(int PLC_Count)
    {
        var partnum = new[]
                      {
                          "ooxx",
                          "abc",
                          "zzz",
                          "qoo",
                          "boom",
                          "xxx",
                          "wunmao"
                      };

        var lotid = new[]
                    {
                        "111",
                        "222",
                        "333",
                        "444",
                        "555",
                        "666",
                        "777"
                    };

        var time = DateTime.Now;

        for (var j = 1; j <= DateTime.DaysInMonth(time.Year, time.Month); j++) //! 產生一個月的資料
        {
            for (var i = 0; i < PLC_Count; i++)
            {
                var rn = new Random(i + j);
                var st = new DateTime(time.Year,
                                      time.Month,
                                      j,
                                      8,
                                      i + rn.Next(0, 10),
                                      rn.Next(0, 60)); //! 早上8點開始

                for (var k = 0; k < 10; k++) //! 每天每烤箱8筆
                {
                    var info = new ProcessInfo
                               {
                                   StartTime  = st,
                                   RackID     = rn.Next(1, 10000).ToString("00000"),
                                   OperatorID = rn.Next(1, 10).ToString("000"),
                                   Recipe     = RecipeVM.Recipes == null || RecipeVM.Recipes.Count == 0 ? new PLC_Recipe { RecipeName = "NoName" } : RecipeVM.Recipes[new Random().Next(0, RecipeVM.Recipes.Count)]
                               };

                    var ttime = new TimeSpan(0, 0, 1);
                    var cc    = 0;

                    for (var m = 0; m < 100; m++) //! 產生100筆溫度資料，間隔1分鐘
                    {
                        if (m % 10 == 0) //! 每10分鐘產生一筆事件
                        {
                            var ev1 = new LogEvent
                                      {
                                          StationNumber = i  + 1,
                                          AddedTime     = st + ttime,
                                          Description   = $"{i}{j}{m}",
                                          TagCode       = $"ooxx{m}",
                                          Type          = (EventType)new Random(DateTime.Now.Millisecond + m).Next(0, 6),
                                          Value         = new Random(DateTime.Now.Millisecond            + m + 1).Next(2) > 0
                                      };

                            LogVM.DataCollection.Add(ev1);
                            info.EventList.Add(ev1);
                        }

                        var tempt = 30 * (1 + 5 / (1 + Math.Exp(-0.12 * cc + 3)));
                        var vals = new RecordTemperatures
                                   {
                                       AddedTime                = st + ttime,
                                       PV_ThermostatTemperature = Math.Round(tempt,                                          1),
                                       OvenTemperatures_1       = Math.Round(tempt + rn.Next(-5, 5),                         1),
                                       OvenTemperatures_2       = Math.Round(tempt + rn.Next(-5, 5),                         1),
                                       OvenTemperatures_3       = Math.Round(tempt + rn.Next(-5, 5),                         1),
                                       OvenTemperatures_4       = Math.Round(tempt + rn.Next(-5, 5),                         1),
                                       OvenTemperatures_5       = Math.Round(tempt + rn.Next(-5, 5),                         1),
                                       OvenTemperatures_6       = Math.Round(tempt + rn.Next(-5, 5),                         1),
                                       OvenTemperatures_7       = Math.Round(tempt + rn.Next(-5, 5),                         1),
                                       OvenTemperatures_8       = Math.Round(tempt + rn.Next(-5, 5),                         1),
                                       OxygenContent            = Math.Round(new Random(i + j + k + m).NextDouble() * 100.0, 1)
                                   };

                        cc += 1;
                        info.RecordTemperatures.Add(vals);

                        ttime = ttime.Add(TimeSpan.FromMinutes(1)); //! 間隔1分鐘
                    }

                    info.EndTime       = info.StartTime + ttime;
                    info.IsFinished    = new Random().NextDouble() > 0.5;
                    info.TotalRampTime = (info.EndTime - info.StartTime).Minutes;

                    st = info.EndTime + TimeSpan.FromHours(2);

                    var n = rn.Next(0, 8) + 1; //! 階層
                    for (var p = 1; p <= n; p++)
                    {
                        var product = new ProductInfo
                                      {
                                          PartID   = partnum[rn.Next(0, partnum.Length)],
                                          LotID    = lotid[rn.Next(0,   lotid.Length)],
                                          Layer    = p,
                                          Quantity = rn.Next(10, 20)
                                      };

                        info.Products.Add(product);
                    }

                    info.StationNumber = i + 1;
                    info.AddedTime     = info.EndTime.AddSeconds(10);

                    TraceVM.DataCollection.Add(info);
                }
            }
        }
    }
}