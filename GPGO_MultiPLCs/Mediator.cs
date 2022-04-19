using GPGO_MultiPLCs.Models;
using GPGO_MultiPLCs.ViewModels;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PLCService;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GPGO_MultiPLCs
{
    public sealed class Mediator : ObservableObject
    {
        public string DataInputPath
        {
            get => Get<string>();
            set => Set(value);
        }

        public string DataOutputPath
        {
            get => Get<string>();
            set => Set(value);
        }

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

            for (var j = 1; j <= new DateTime(time.Year, time.Month, 1).AddMonths(1).AddDays(-1).Day; j++)
            {
                for (var i = 0; i < PLC_Count; i++)
                {
                    var plc = TotalVM.PLC_All[i];
                    var rn  = new Random(i                                + j);
                    var st  = new DateTime(time.Year, time.Month, j, 8, i + rn.Next(0, 10), i);

                    for (var k = 0; k < 8; k++)
                    {
                        var info = new ProcessInfo
                                   {
                                       StartTime  = st,
                                       RackID     = rn.Next(1, 10000).ToString("00000"),
                                       OperatorID = rn.Next(1, 10).ToString("000"),
                                       Recipe     = RecipeVM.Recipes[new Random().Next(0, RecipeVM.Recipes.Count)].ToDictionary(Language),
                                       RampTimes = new[]
                                                   {
                                                       plc.RampTime_1, plc.RampTime_2, plc.RampTime_3, plc.RampTime_4,
                                                       plc.RampTime_5, plc.RampTime_6, plc.RampTime_7, plc.RampTime_8
                                                   }.Take(plc.StepCounts)
                                                    .ToList(),
                                       DwellTimes = new[]
                                                    {
                                                        plc.DwellTime_1, plc.DwellTime_2, plc.DwellTime_3, plc.DwellTime_4,
                                                        plc.DwellTime_5, plc.DwellTime_6, plc.DwellTime_7, plc.DwellTime_8
                                                    }.Take(plc.StepCounts)
                                                     .ToList(),
                                       RampAlarms = new[]
                                                    {
                                                        plc.RampAlarm_1, plc.RampAlarm_2, plc.RampAlarm_3, plc.RampAlarm_4,
                                                        plc.RampAlarm_5, plc.RampAlarm_6, plc.RampAlarm_7, plc.RampAlarm_8
                                                    }.Take(plc.StepCounts)
                                                     .ToList(),
                                       DwellAlarms = new[]
                                                     {
                                                         plc.DwellAlarm_1, plc.DwellAlarm_2, plc.DwellAlarm_3, plc.DwellAlarm_4,
                                                         plc.DwellAlarm_5, plc.DwellAlarm_6, plc.DwellAlarm_7, plc.DwellAlarm_8
                                                     }.Take(plc.StepCounts)
                                                      .ToList(),
                                       TargetOvenTemperatures = new[]
                                                                {
                                                                    plc.TemperatureSetpoint_1, plc.TemperatureSetpoint_2, plc.TemperatureSetpoint_3, plc.TemperatureSetpoint_4,
                                                                    plc.TemperatureSetpoint_5, plc.TemperatureSetpoint_6, plc.TemperatureSetpoint_7, plc.TemperatureSetpoint_8
                                                                }.Take(plc.StepCounts)
                                                                 .ToList(),
                                       DwellTemperatures = new[]
                                                           {
                                                               plc.DwellTemperature_1, plc.DwellTemperature_2, plc.DwellTemperature_3, plc.DwellTemperature_4,
                                                               plc.DwellTemperature_5, plc.DwellTemperature_6, plc.DwellTemperature_7, plc.DwellTemperature_8
                                                           }.Take(plc.StepCounts)
                                                            .ToList()
                                   };

                        var ttime = new TimeSpan(0, 0, 1);
                        var cc    = 0;

                        var _ev = new LogEvent
                                  {
                                      StationNumber = i + 1,
                                      StartTime     = st,
                                      AddedTime     = st + ttime,
                                      Description   = "第1段升溫",
                                      TagCode       = $"{BitType.S}{100}",
                                      Type          = EventType.Normal,
                                      Value         = true
                                  };

                        LogVM.AddToDB(_ev);
                        info.EventList.Add(_ev);

                        for (var m = 0; m < 100; m++)
                        {
                            if (m == 60)
                            {
                                var ev1 = new LogEvent
                                          {
                                              StationNumber = i + 1,
                                              StartTime     = st,
                                              AddedTime     = st + ttime - TimeSpan.FromMilliseconds(1),
                                              Description   = "第1段升溫",
                                              TagCode       = $"{BitType.S}{100}",
                                              Type          = EventType.Normal,
                                              Value         = false
                                          };

                                LogVM.AddToDB(ev1);
                                info.EventList.Add(ev1);

                                var ev2 = new LogEvent
                                          {
                                              StationNumber = i + 1,
                                              StartTime     = st,
                                              AddedTime     = st + ttime,
                                              Description   = "第1段恆溫",
                                              TagCode       = $"{BitType.S}{101}",
                                              Type          = EventType.Normal,
                                              Value         = true
                                          };

                                LogVM.AddToDB(ev2);
                                info.EventList.Add(ev2);
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

                            ttime = ttime.Add(TimeSpan.FromMinutes(1));
                        }

                        var ev_1 = new LogEvent
                                   {
                                       StationNumber = i + 1,
                                       StartTime     = st,
                                       AddedTime     = st + ttime - TimeSpan.FromSeconds(60),
                                       Description   = "第1段恆溫",
                                       TagCode       = $"{BitType.S}{100}",
                                       Type          = EventType.Normal,
                                       Value         = false
                                   };

                        LogVM.AddToDB(ev_1);
                        info.EventList.Add(ev_1);

                        var ev_2 = new LogEvent
                                   {
                                       StationNumber = i + 1,
                                       StartTime     = st,
                                       AddedTime     = st + ttime - TimeSpan.FromSeconds(1),
                                       Description   = "程式結束",
                                       TagCode       = $"{BitType.S}{102}",
                                       Type          = EventType.Trigger,
                                       Value         = true
                                   };

                        LogVM.AddToDB(ev_2);
                        info.EventList.Add(ev_2);

                        var ev_3 = new LogEvent
                                   {
                                       StationNumber = i + 1,
                                       StartTime     = st,
                                       AddedTime     = st + ttime,
                                       Description   = "自動停止",
                                       TagCode       = $"{BitType.S}{102}",
                                       Type          = EventType.Trigger,
                                       Value         = true
                                   };

                        LogVM.AddToDB(ev_3);
                        info.EventList.Add(ev_3);

                        info.EndTime       = info.StartTime + ttime;
                        info.IsFinished    = new Random().NextDouble() > 0.5;
                        info.TotalRampTime = (info.EndTime - info.StartTime).Minutes;

                        st = info.EndTime + TimeSpan.FromMinutes(10);

                        var infos = new List<ProcessInfo>();
                        var temp  = new List<int>();
                        var n     = rn.Next(0, 1);
                        for (var p = 0; p <= n; p++)
                        {
                            var _info = info.Copy();
                            var index = rn.Next(0, partnum.Length);
                            while (temp.Contains(index))
                            {
                                index = rn.Next(0, partnum.Length);
                            }

                            temp.Add(index);
                            _info.PartID = partnum[index];
                            _info.LotID  = lotid[rn.Next(0, lotid.Length)];

                            var count = rn.Next(10, 20);
                            for (var m = 1; m <= count; m++)
                            {
                                _info.PanelIDs.Add($"{_info.PartID}-{_info.LotID}-{m}");
                            }

                            infos.Add(_info);
                        }

                        TraceVM.AddToDB(i, infos, info.EndTime.AddMinutes(1));
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
            TotalVM = new TotalView_ViewModel(20, PlcGate, DialogVM);
            //!請勿更動20這個數字，要變更實際烤箱數量需至程式資料夾內修改Settings.json內的OvenCount數字或是設定AuthenticatorVM的Settings.OvenCount

            AuthenticatorVM = new Authenticator_ViewModel
                              {
                                  Settings =
                                  {
                                      OvenCount = 1
                                  }
                              };

            DataOutputPath = AuthenticatorVM.Settings.DataOutputPath;
            DataInputPath  = AuthenticatorVM.Settings.DataInputPath;
            Language       = AuthenticatorVM.Settings.Lng;
            OvenCount      = AuthenticatorVM.Settings.OvenCount;
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
                                                                case nameof(GlobalSettings.DataInputPath):
                                                                    DataInputPath = ((GlobalSettings)s).DataInputPath;
                                                                    break;
                                                                case nameof(GlobalSettings.DataOutputPath):
                                                                    DataOutputPath = ((GlobalSettings)s).DataOutputPath;
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
                                            if (i == 0)
                                            {
                                                TotalVM.Index = 0;
                                            }
                                        };

            //!當主視窗讀取完成時，再讀取配方和生產履歷資料庫
            MainVM.LoadedEvent += async dp =>
                                  {
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
                                               LogVM.AddToDB(new LogEvent
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

                                                 var si = new GPCore.StreamReaderIni();
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
                                                         case PLC_ViewModel.SetRecipeResult.條件不允許:
                                                             l1.Add(i + 1);
                                                             break;
                                                         case PLC_ViewModel.SetRecipeResult.PLC錯誤:
                                                             l2.Add(i + 1);
                                                             break;
                                                         case PLC_ViewModel.SetRecipeResult.比對錯誤:
                                                             l3.Add(i + 1);
                                                             break;
                                                     }
                                                 }
                                             }

                                             if (showtip && (l1.Any() || l2.Any() || l3.Any()))
                                             {
                                                 var str1 = string.Join(", ", l1);
                                                 var str2 = string.Join(", ", l2);
                                                 var str3 = string.Join(", ", l3);

                                                 var tw  = new StringBuilder();
                                                 var chs = new StringBuilder();
                                                 var en  = new StringBuilder();

                                                 if (l1.Any())
                                                 {
                                                     tw.Append($"{str1} 號烤箱未符合設定條件！");
                                                     chs.Append($"{str1} 号烤箱未符合设定条件！");
                                                     en.Append($"No. {str1} oven{(l1.Count > 1 ? "s" : "")}: Not meet the conditions！");
                                                 }

                                                 if (l2.Any())
                                                 {
                                                     tw.AppendLine();
                                                     chs.AppendLine();
                                                     en.AppendLine();
                                                     tw.Append($"{str2} 號烤箱配方設定失敗！");
                                                     chs.Append($"{str2} 号烤箱配方设定失败！");
                                                     en.Append($"No. {str2} oven{(l2.Count > 1 ? "s" : "")}: Recipe set failure！");
                                                 }

                                                 if (l3.Any())
                                                 {
                                                     tw.AppendLine();
                                                     chs.AppendLine();
                                                     en.AppendLine();
                                                     tw.Append($"{str3} 號烤箱配方比對錯誤！");
                                                     chs.Append($"{str3} 号烤箱配方比对错误！");
                                                     en.Append($"No. {str3} oven{(l3.Count > 1 ? "s" : "")}: Recipe check error！");
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
                                             TraceVM.AddToDB(stationIndex, infos);

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

            TotalVM.EventHappened += e =>
                                     {
                                         var (stationIndex, type, time, note, tag, value) = e;
                                         LogVM.AddToDB(new LogEvent
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
                                                                      do
                                                                      {
                                                                          Thread.Sleep(30);
                                                                      } while (!TraceVM.Standby);
                                                                  });

                                      await Task.Delay(150);

                                      TraceVM.SearchResult = info;
                                      TraceVM.SearchEvent  = logEvent;
                                      TraceVM.Date1        = info.AddedTime.Date;
                                  };

            LogVM.LogAdded += log =>
                              {
                                  if (log.Value is true && (int)log.Type > 1)
                                  {
                                      TotalVM.InsertMessage(log);
                                  }
                              };

            Task.Run(() =>
                     {
                         var evs = LogVM.DataCollection.Find(x => x.AddedTime > DateTime.Now.AddDays(-1)).Where(x => x.Value is true && (int)x.Type > 1).Take(50).ToArray();
                         TotalVM.InsertMessage(evs);
                     });

            //DialogVM.Show(new Dictionary<Language, string>
            //              {
            //                  {Language.TW, "測試資料產生中，請稍後！"},
            //                  {Language.CHS, "测试数据产生中，请稍后！"},
            //                  {Language.EN, "Testing data is being generated, please wait!"}
            //              }, Task.Factory.StartNew(() =>
            //                                       {
            //                                           while (RecipeVM.Recipes == null)
            //                                           {
            //                                               Thread.Sleep(45);
            //                                           }

            //                                           Thread.Sleep(45);

            //                                           MakeTestData(10);

            //                                           Thread.Sleep(45);

            //                                           TraceVM.TodayCommand?.Execute(null);
            //                                       }, TaskCreationOptions.LongRunning), TimeSpan.FromMinutes(5));
        }
    }
}