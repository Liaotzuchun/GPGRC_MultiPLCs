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
using GPMVVM.Helpers;
using GPMVVM.Models;
using MongoDB.Driver;

namespace GPGO_MultiPLCs;

public sealed class Mediator : ObservableObject
{
    public Language Language
    {
        get => Get<Language>();
        set
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
        set
        {
            Set(value);

            TotalVM.OvenCount = value;
        }
    }

    public User User
    {
        get => Get<User>();
        set
        {
            Set(value);

            RecipeVM.UserName = value.Name;
        }
    }

    private readonly AsyncLock lockobj = new();

    public Authenticator_ViewModel    AuthenticatorVM { get; }
    public GlobalDialog_ViewModel     DialogVM        { get; }
    public LogView_ViewModel          LogVM           { get; }
    public MainWindow_ViewModel       MainVM          { get; }
    public RecipeControl_ViewModel    RecipeVM        { get; }
    public TotalView_ViewModel        TotalVM         { get; }
    public TraceabilityView_ViewModel TraceVM         { get; }
    public IGate                      PlcGate         { get; }

    /// <summary>產生測試資料至資料庫</summary>
    /// <param name="PLC_Count"></param>
    public void MakeTestData(int PLC_Count)
    {
        var partnum = new[]
                      {
                          "ooxx", "abc", "zzz", "qoo",
                          "boom", "xxx", "wunmao"
                      };

        var lotid = new[]
                    {
                        "111", "222", "333", "444",
                        "555", "666", "777"
                    };

        var time = DateTime.Now;

        for (var j = 1; j <= DateTime.DaysInMonth(time.Year, time.Month); j++) //! 產生一個月的資料
        {
            for (var i = 0; i < PLC_Count; i++)
            {
                var rn = new Random(i                                + j);
                var st = new DateTime(time.Year, time.Month, j, 8, i + rn.Next(0, 10), rn.Next(0, 60)); //! 早上8點開始

                for (var k = 0; k < 8; k++) //! 每天每烤箱8筆
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
                                          StationNumber = i + 1,
                                          StartTime     = st,
                                          AddedTime     = st + ttime,
                                          Description   = $"{i}{j}{m}",
                                          TagCode       = $"ooxx{m}",
                                          Type          = (EventType)new Random(DateTime.Now.Millisecond + m).Next(0, 6),
                                          Value         = new Random(DateTime.Now.Millisecond            + m + 1).Next(2) > 0
                                      };

                            LogVM.DataCollection.Add(ev1);
                            info.EventList.Add(ev1);
                        }

                        var tempt = 30 * (1 + 5 * 1 / (1 + Math.Exp(-0.12 * cc + 3)));
                        var vals = new RecordTemperatures
                                   {
                                       StartTime                = st,
                                       AddedTime                = st + ttime,
                                       PV_ThermostatTemperature = tempt,
                                       OvenTemperatures_1       = tempt + rn.Next(-5, 5),
                                       OvenTemperatures_2       = tempt + rn.Next(-5, 5),
                                       OvenTemperatures_3       = tempt + rn.Next(-5, 5),
                                       OvenTemperatures_4       = tempt + rn.Next(-5, 5),
                                       OvenTemperatures_5       = tempt + rn.Next(-5, 5),
                                       OvenTemperatures_6       = tempt + rn.Next(-5, 5),
                                       OvenTemperatures_7       = tempt + rn.Next(-5, 5),
                                       OvenTemperatures_8       = tempt + rn.Next(-5, 5)
                                   };

                        cc += 1;
                        info.RecordTemperatures.Add(vals);

                        ttime = ttime.Add(TimeSpan.FromMinutes(1)); //! 間隔1分鐘
                    }

                    info.EndTime       = info.StartTime + ttime;
                    info.IsFinished    = new Random().NextDouble() > 0.5;
                    info.TotalRampTime = (info.EndTime - info.StartTime).Minutes;

                    st = info.EndTime + TimeSpan.FromHours(2);

                    var n = rn.Next(0, 8); //! 階層
                    for (var p = 0; p <= n; p++)
                    {
                        var product = new ProductInfo
                                      {
                                          PartID = partnum[rn.Next(0, partnum.Length)],
                                          LotID  = lotid[rn.Next(0,   lotid.Length)],
                                          Layer  = p
                                      };

                        var count = rn.Next(10, 20);
                        for (var m = 1; m <= count; m++)
                        {
                            product.PanelIDs.Add($"{product.PartID}-{product.LotID}-{product.Layer}-{m}");
                        }

                        info.Products.Add(product);
                    }

                    info.StationNumber = i + 1;
                    info.AddedTime     = info.EndTime.AddSeconds(10);

                    TraceVM.DataCollection.Add(info);
                }
            }
        }
    }

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
        //!請勿更動20這個數字，要變更實際烤箱數量需至程式資料夾內修改Settings.json內的OvenCount數字或是設定AuthenticatorVM的Settings.OvenCount

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

        //!當回到主頁時，也將生產總覽回到總覽頁
        MainVM.IndexChangedEvent += i =>
                                    {
                                        if (i == 0 && !TotalVM.PLC_All[0].IsExecuting)
                                        {
                                            TotalVM.Index = 0;
                                        }

                                        TraceVM.SelectedIndex = -1;
                                        TraceVM.ShowProducts  = false;
                                    };

        //!當主視窗讀取完成時，再讀取配方和生產履歷資料庫
        MainVM.LoadedEvent += async dp =>
                              {
                                  TotalVM.StartPLCGate();

                                  await dp.InvokeAsync(() =>
                                                       {
                                                           RecipeVM.InitialLoadCommand.Execute(null);
                                                           TraceVM.TodayCommand.Execute(null);
                                                       },
                                                       DispatcherPriority.SystemIdle);
                              };

        //!當OP試圖關閉程式時，進行狀態和權限檢查
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
                                       var user = User.Copy();
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

        List<PLC_Recipe> tempRecipeList = null;
        //!當配方列表更新時，依據使用站別發佈配方
        RecipeVM.ListUpdatedEvent += async e =>
                                     {
                                         var (list, showtip) = e;

                                         var di = new DirectoryInfo("C:\\ITRIinit\\0\\ProcessJob");

                                         foreach (var file in di.GetFiles())
                                         {
                                             file.Delete();
                                         }

                                         foreach (var recipe in list)
                                         {
                                             if (tempRecipeList != null)
                                             {
                                                 var result = tempRecipeList.FirstOrDefault(x => x.RecipeName == recipe.RecipeName);
                                                 if (result != null)
                                                 {
                                                     if (!result.Equals(recipe))
                                                     {
                                                         TotalVM.InvokeRecipe(recipe.RecipeName, SECSThread.PPStatus.Change);
                                                     }

                                                     tempRecipeList.RemoveAll(x => x.RecipeName == recipe.RecipeName);
                                                 }
                                                 else
                                                 {
                                                     TotalVM.InvokeRecipe(recipe.RecipeName, SECSThread.PPStatus.Create);
                                                 }
                                             }

                                             var si = new StreamReaderIni();
                                             var t  = si.AddIniSection("CCodeID1");
                                             t.AddElement(nameof(PLC_Recipe.ProgramEndWarningTime), recipe.ProgramEndWarningTime.ToString("0.0"));
                                             t.AddElement(nameof(PLC_Recipe.CoolingTime),           recipe.CoolingTime.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.CoolingTemperature),    recipe.CoolingTemperature.ToString("0.0"));
                                             t.AddElement(nameof(PLC_Recipe.StepCounts),            recipe.StepCounts.ToString());
                                             t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_1), recipe.TemperatureSetpoint_1.ToString("0.0"));
                                             t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_2), recipe.TemperatureSetpoint_2.ToString("0.0"));
                                             t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_3), recipe.TemperatureSetpoint_3.ToString("0.0"));
                                             t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_4), recipe.TemperatureSetpoint_4.ToString("0.0"));
                                             t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_5), recipe.TemperatureSetpoint_5.ToString("0.0"));
                                             t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_6), recipe.TemperatureSetpoint_6.ToString("0.0"));
                                             t.AddElement(nameof(PLC_Recipe.RampTime_1),            recipe.RampTime_1.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampTime_2),            recipe.RampTime_2.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampTime_3),            recipe.RampTime_3.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampTime_4),            recipe.RampTime_4.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampTime_5),            recipe.RampTime_5.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampTime_6),            recipe.RampTime_6.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampAlarm_1),           recipe.RampAlarm_1.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampAlarm_2),           recipe.RampAlarm_2.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampAlarm_3),           recipe.RampAlarm_3.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampAlarm_4),           recipe.RampAlarm_4.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampAlarm_5),           recipe.RampAlarm_5.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.RampAlarm_6),           recipe.RampAlarm_6.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellTime_1),           recipe.DwellTime_1.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellTime_2),           recipe.DwellTime_2.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellTime_3),           recipe.DwellTime_3.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellTime_4),           recipe.DwellTime_4.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellTime_5),           recipe.DwellTime_5.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellTime_6),           recipe.DwellTime_6.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellAlarm_1),          recipe.DwellAlarm_1.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellAlarm_2),          recipe.DwellAlarm_2.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellAlarm_3),          recipe.DwellAlarm_3.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellAlarm_4),          recipe.DwellAlarm_4.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellAlarm_5),          recipe.DwellAlarm_5.ToString("0"));
                                             t.AddElement(nameof(PLC_Recipe.DwellAlarm_6),          recipe.DwellAlarm_6.ToString("0"));
                                             si.EncodindIni($"C:\\ITRIinit\\0\\ProcessJob\\{recipe.RecipeName}.pjb");
                                         }

                                         if (tempRecipeList != null)
                                         {
                                             foreach (var recipe in tempRecipeList)
                                             {
                                                 TotalVM.InvokeRecipe(recipe.RecipeName, SECSThread.PPStatus.Delete);
                                             }
                                         }

                                         tempRecipeList = list;

                                         TotalVM.SetRecipeNames(list.Select(x => x.RecipeName).ToArray());

                                         var l1 = new List<int>();
                                         var l2 = new List<int>();
                                         var l3 = new List<int>();

                                         for (var i = 0; i < TotalVM.PLC_All.Count; i++)
                                         {
                                             var j      = i;
                                             var recipe = list.Find(x => j < x.Used_Stations.Count && x.Used_Stations[j]);
                                             if (recipe != null)
                                             {
                                                 switch (await TotalVM.SetRecipe(i, recipe))
                                                 {
                                                     case SetRecipeResult.條件不允許:
                                                         l1.Add(i + 1);
                                                         break;
                                                     case SetRecipeResult.PLC錯誤:
                                                         l2.Add(i + 1);
                                                         break;
                                                     case SetRecipeResult.比對錯誤:
                                                         l3.Add(i + 1);
                                                         break;
                                                 }
                                             }
                                         }

                                         if (showtip && (l1.Any() || l2.Any() || l3.Any()))
                                         {
                                             //var str1 = string.Join(", ", l1);
                                             //var str2 = string.Join(", ", l2);
                                             //var str3 = string.Join(", ", l3);

                                             var tw  = new StringBuilder();
                                             var chs = new StringBuilder();
                                             var en  = new StringBuilder();

                                             if (l1.Any())
                                             {
                                                 tw.Append("烤箱未符合設定條件！");
                                                 chs.Append("烤箱未符合设定条件！");
                                                 en.Append("Not meet the conditions！");
                                             }

                                             if (l2.Any())
                                             {
                                                 tw.AppendLine();
                                                 chs.AppendLine();
                                                 en.AppendLine();
                                                 tw.Append("烤箱配方設定失敗！");
                                                 chs.Append("烤箱配方设定失败！");
                                                 en.Append("Recipe set failure！");
                                             }

                                             if (l3.Any())
                                             {
                                                 tw.AppendLine();
                                                 chs.AppendLine();
                                                 en.AppendLine();
                                                 tw.Append("烤箱配方比對錯誤！");
                                                 chs.Append("烤箱配方比对错误！");
                                                 en.Append("Recipe check error！");
                                             }

                                             DialogVM.Show(new Dictionary<Language, string>
                                                           {
                                                               { Language.TW, tw.ToString() },
                                                               { Language.CHS, chs.ToString() },
                                                               { Language.EN, en.ToString() }
                                                           },
                                                           TimeSpan.FromSeconds(3),
                                                           DialogMsgType.Alert);
                                         }
                                     };

        TotalVM.WantLogin += () => AuthenticatorVM.StartLogin?.Execute(null);

        //!當某站烤箱要求配方時，自資料庫讀取配方並發送
        TotalVM.GetRecipe += e => string.IsNullOrEmpty(e.RecipeName) ? null : RecipeVM.GetRecipe(e.RecipeName);

        //!設定配方被該站使用
        TotalVM.RecipeUsed += e => RecipeVM.SetUsed(e.StationIndex, e.RecipeName);

        TotalVM.GetUser += () => User;

        //!當某站烤箱完成烘烤程序時，將生產資訊寫入資料庫並輸出至上傳資料夾，並回傳當日產量
        TotalVM.AddRecordToDB += async e =>
                                 {
                                     //var (inpath, outpath) = await Dispatcher.InvokeAsync(() => (DataInputPath, DataOutputPath));

                                     var (stationIndex, infos) = e;
                                     using (await lockobj.LockAsync())
                                     {
                                         await TraceVM.AddToDBAsync(stationIndex, infos);

                                         //!輸出嘉聯益資料
                                         //if (!string.IsNullOrEmpty(inpath) && !string.IsNullOrEmpty(outpath) && e.Infos.Any())
                                         //{
                                         //    if (!Directory.Exists(outpath))
                                         //    {
                                         //        try
                                         //        {
                                         //            Directory.CreateDirectory(outpath);
                                         //        }
                                         //        catch (Exception ex)
                                         //        {
                                         //            Log.Error(ex, "上傳資料夾不存在且無法創建");
                                         //        }
                                         //    }

                                         //    foreach (var info in e.Infos)
                                         //    {
                                         //        for (var i = 0; i < info.Quantity; i++)
                                         //        {
                                         //            var path = $"{outpath}\\{info.AssetNumber}_{DateTime.Now:yyyyMMddHHmmssfff}_{e.StationIndex + 1}_";

                                         //            var n = 1;
                                         //            while (File.Exists($"{path}{n}"))
                                         //            {
                                         //                n++;
                                         //            }

                                         //            try
                                         //            {
                                         //                using (var outputFile = new StreamWriter($"{path}{n}.txt", false, Encoding.ASCII))
                                         //                {
                                         //                    await outputFile.WriteAsync(info.ToString(i));
                                         //                }

                                         //                await Task.Delay(1);
                                         //                //!紀錄資料到指定輸出資料夾
                                         //            }
                                         //            catch (Exception ex)
                                         //            {
                                         //                Log.Error(ex, "資料輸出上傳失敗");
                                         //            }
                                         //        }
                                         //    }

                                         //    var _path = $"{inpath}\\{e.Infos.First().RackID}";

                                         //    if (Directory.Exists(_path))
                                         //    {
                                         //        var tag   = $".bak{e.StationIndex}";
                                         //        var files = new DirectoryInfo(_path).GetFiles($"*{tag}");
                                         //        foreach (var file in files)
                                         //        {
                                         //            file.Delete();
                                         //        }
                                         //    }
                                         //}
                                     }

                                     return await TraceVM.CheckProductions(stationIndex);
                                 };

        TotalVM.EventHappened += async e =>
                                 {
                                     var (stationIndex, type, time, note, tag, value) = e;
                                     await LogVM.AddToDBAsync(new LogEvent
                                                              {
                                                                  StationNumber = stationIndex + 1,
                                                                  AddedTime     = time,
                                                                  Type          = type,
                                                                  Description   = note,
                                                                  TagCode       = tag,
                                                                  Value         = value
                                                              });
                                 };

        TotalVM.UpsertRecipe += recipe => RecipeVM.Upsert(recipe);

        TotalVM.DeleteRecipe += recipeName => RecipeVM.Delete(recipeName);

        TotalVM.RetrieveLotData += async lotid => (await TraceVM.FindInfo(lotid)).OrderByDescending(x => x.EndTime).FirstOrDefault();

        LogVM.WantInfo += e => TraceVM.FindInfo(e.station, e.time);

        LogVM.GoDetailView += async e =>
                              {
                                  TraceVM.Standby      = false; //!強制讓TraceVM處於須等待狀態，因此時畫面仍在變化仍未loaded，但TraceVM.Standby為true，將導致以下的迴圈等待沒效果
                                  MainVM.ViewIndex     = 2;
                                  var (info, logEvent) = e;

                                  await Task.Factory.StartNew(() =>
                                                              {
                                                                  SpinWait.SpinUntil(() => TraceVM.Standby);
                                                              });

                                  await Task.Delay(150);

                                  TraceVM.SearchResult = info;
                                  TraceVM.SearchEvent  = logEvent;
                                  TraceVM.Date1        = info.AddedTime.Date;
                              };

        LogVM.LogAdded += log =>
                          {
                              TotalVM.InsertMessage(log);
                          };

        Task.Run(() =>
                 {
                     var evs = LogVM.DataCollection.Find(x => x.AddedTime > DateTime.Now.AddDays(-1) && x.AddedTime <= DateTime.Now && x.Type > EventType.StatusChanged).OrderByDescending(x => x.AddedTime).Take(50).ToArray();
                     TotalVM.InsertMessage(evs);
                 });

        //#region 產生測試用生產數據資料庫，務必先建立配方！！

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
        //                                            Thread.Sleep(1200);

        //                                            MakeTestData(1);

        //                                            var evs = LogVM.DataCollection.Find(x => x.AddedTime > DateTime.Now.AddDays(-1)).Where(x => (int)x.Type > 1).Take(50).ToArray();
        //                                            TotalVM.InsertMessage(evs);
        //                                        }
        //                                        catch
        //                                        {
        //                                            // ignored
        //                                        }
        //                                    }),
        //              TimeSpan.FromMinutes(5));

        //#endregion
    }
}