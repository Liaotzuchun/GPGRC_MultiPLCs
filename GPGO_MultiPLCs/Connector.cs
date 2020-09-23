using GPGO_MultiPLCs.Models;
using GPGO_MultiPLCs.ViewModels;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PLCService;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GPCore;
using AsyncAutoResetEvent = GPMVVM.Helpers.AsyncAutoResetEvent;
using AsyncLock = GPMVVM.Helpers.AsyncLock;

namespace GPGO_MultiPLCs
{
    public sealed class Connector : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty OvenCountProperty     = DependencyProperty.Register(nameof(OvenCount),     typeof(int),    typeof(Connector), new PropertyMetadata(0,  OvenCountChanged));
        public static readonly DependencyProperty DataInputPathProperty = DependencyProperty.Register(nameof(DataInputPath), typeof(string), typeof(Connector), new PropertyMetadata("", null));

        public string DataInputPath
        {
            get => (string)GetValue(DataInputPathProperty);
            set => SetValue(DataInputPathProperty, value);
        }

        public static readonly DependencyProperty DataOutputPathProperty = DependencyProperty.Register(nameof(DataOutputPath), typeof(string), typeof(Connector), new PropertyMetadata("", null));

        public string DataOutputPath
        {
            get => (string)GetValue(DataOutputPathProperty);
            set => SetValue(DataOutputPathProperty, value);
        }

        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(nameof(User), typeof(User), typeof(Connector), new PropertyMetadata(default(User), UserChanged));

        public static readonly DependencyProperty LanguageProperty =
            DependencyProperty.Register(nameof(Language), typeof(Language), typeof(Connector), new PropertyMetadata(Language.TW, LanguageChanged));

        private static void LanguageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var lng = (Language)Enum.Parse(typeof(Language), e.NewValue.ToString());
            ((Connector)sender).DialogVM.Language = lng;
            ((Connector)sender).TraceVM.Language  = lng;
            ((Connector)sender).LogVM.Language    = lng;
        }

        public Language Language
        {
            get => (Language)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }

        private static void OvenCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var count = (int)e.NewValue;
            ((Connector)sender).TotalVM.OvenCount = count;
        }

        public int OvenCount
        {
            get => (int)GetValue(OvenCountProperty);
            set => SetValue(OvenCountProperty, value);
        }

        private static void UserChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var user = (User)e.NewValue;
            ((Connector)sender).RecipeVM.UserName = user.Name;
        }

        public User User
        {
            get => (User)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public void Dispose() { TotalVM.Dispose(); }

        private readonly AsyncLock lockobj = new AsyncLock();

        private AsyncAutoResetEvent _Testdatalock;

        public GlobalDialog_ViewModel     DialogVM { get; }
        public LogView_ViewModel          LogVM    { get; }
        public MainWindow_ViewModel       MainVM   { get; }
        public RecipeControl_ViewModel    RecipeVM { get; }
        public TotalView_ViewModel        TotalVM  { get; }
        public TraceabilityView_ViewModel TraceVM  { get; }

        /// <summary>產生測試資料至資料庫</summary>
        /// <param name="PLC_Count"></param>
        public async void MakeTestData(int PLC_Count)
        {
            _Testdatalock = new AsyncAutoResetEvent();
            await _Testdatalock.WaitAsync();

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
                    var rn = new Random(i + j);

                    var st = new DateTime(time.Year, time.Month, j, 8, i + rn.Next(0, 10), i);

                    for (var k = 0; k < 8; k++)
                    {
                        var info = new ProcessInfo
                                   {
                                       StartTime  = st,
                                       RackID     = rn.Next(1, 10000).ToString("00000"),
                                       OperatorID = rn.Next(1, 10).ToString("000")
                                   };
                        var h = new[]
                                {
                                    TotalVM.PLC_All[i].RampTime_1, TotalVM.PLC_All[i].RampTime_2, TotalVM.PLC_All[i].RampTime_3, TotalVM.PLC_All[i].RampTime_4,
                                    TotalVM.PLC_All[i].RampTime_5, TotalVM.PLC_All[i].RampTime_6, TotalVM.PLC_All[i].RampTime_7, TotalVM.PLC_All[i].RampTime_8
                                };
                        var w = new[]
                                {
                                    TotalVM.PLC_All[i].DwellTime_1, TotalVM.PLC_All[i].DwellTime_2, TotalVM.PLC_All[i].DwellTime_3, TotalVM.PLC_All[i].DwellTime_4,
                                    TotalVM.PLC_All[i].DwellTime_5, TotalVM.PLC_All[i].DwellTime_6, TotalVM.PLC_All[i].DwellTime_7, TotalVM.PLC_All[i].DwellTime_8
                                };

                        var ha = new[]
                                 {
                                     TotalVM.PLC_All[i].RampAlarm_1, TotalVM.PLC_All[i].RampAlarm_2, TotalVM.PLC_All[i].RampAlarm_3, TotalVM.PLC_All[i].RampAlarm_4,
                                     TotalVM.PLC_All[i].RampAlarm_5, TotalVM.PLC_All[i].RampAlarm_6, TotalVM.PLC_All[i].RampAlarm_7, TotalVM.PLC_All[i].RampAlarm_8
                                 };
                        var wa = new[]
                                 {
                                     TotalVM.PLC_All[i].DwellAlarm_1, TotalVM.PLC_All[i].DwellAlarm_2, TotalVM.PLC_All[i].DwellAlarm_3, TotalVM.PLC_All[i].DwellAlarm_4,
                                     TotalVM.PLC_All[i].DwellAlarm_5, TotalVM.PLC_All[i].DwellAlarm_6, TotalVM.PLC_All[i].DwellAlarm_7, TotalVM.PLC_All[i].DwellAlarm_8
                                 };
                        var t = new[]
                                {
                                    TotalVM.PLC_All[i].TemperatureSetpoint_1, TotalVM.PLC_All[i].TemperatureSetpoint_2, TotalVM.PLC_All[i].TemperatureSetpoint_3, TotalVM.PLC_All[i].TemperatureSetpoint_4,
                                    TotalVM.PLC_All[i].TemperatureSetpoint_5, TotalVM.PLC_All[i].TemperatureSetpoint_6, TotalVM.PLC_All[i].TemperatureSetpoint_7, TotalVM.PLC_All[i].TemperatureSetpoint_8
                                };
                        var s = new[]
                                {
                                    TotalVM.PLC_All[i].DwellTemperature_1, TotalVM.PLC_All[i].DwellTemperature_2, TotalVM.PLC_All[i].DwellTemperature_3, TotalVM.PLC_All[i].DwellTemperature_4,
                                    TotalVM.PLC_All[i].DwellTemperature_5, TotalVM.PLC_All[i].DwellTemperature_6, TotalVM.PLC_All[i].DwellTemperature_7, TotalVM.PLC_All[i].DwellTemperature_8
                                };
                        Array.Resize(ref h,  TotalVM.PLC_All[i].StepCounts);
                        Array.Resize(ref w,  TotalVM.PLC_All[i].StepCounts);
                        Array.Resize(ref ha, TotalVM.PLC_All[i].StepCounts);
                        Array.Resize(ref wa, TotalVM.PLC_All[i].StepCounts);
                        Array.Resize(ref t,  TotalVM.PLC_All[i].StepCounts);
                        Array.Resize(ref s,  TotalVM.PLC_All[i].StepCounts);

                        info.Recipe                 = RecipeVM.Recipes[new Random().Next(0, RecipeVM.Recipes.Count)].ToDictionary(Language);
                        info.RampTimes              = h.ToList();
                        info.DwellTimes             = w.ToList();
                        info.RampAlarms             = ha.ToList();
                        info.DwellAlarms            = wa.ToList();
                        info.TargetOvenTemperatures = t.ToList();
                        info.DwellTemperatures      = s.ToList();

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

        public Connector()
        {
            var db = new MongoClient("mongodb://localhost:27017").GetDatabase("GP");

            DialogVM = new GlobalDialog_ViewModel();
            MainVM   = new MainWindow_ViewModel();
            RecipeVM = new RecipeControl_ViewModel(new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("PLC_Recipes")),
                                                   new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("Old_PLC_Recipes")),
                                                   DialogVM);
            TraceVM = new TraceabilityView_ViewModel(new MongoBase<ProcessInfo>(db.GetCollection<ProcessInfo>("ProductInfos")), DialogVM);
            LogVM   = new LogView_ViewModel(new MongoBase<LogEvent>(db.GetCollection<LogEvent>("EventLogs")), DialogVM);

            TotalVM = new TotalView_ViewModel(20, DialogVM);

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
                                                             {Language.TW, "仍在生產中，無法終止程式！"},
                                                             {Language.CHS, "仍在生产中，无法终止程序！"},
                                                             {Language.EN, "Still processing,\ncannot terminate the program."}
                                                         });
                                       }
                                       else if (User.Level > UserLevel.Operator)
                                       {
                                           var user = User.Copy();
                                           var result = await DialogVM.CheckCondition(new Dictionary<Language, string>
                                                                                      {
                                                                                          {Language.TW, "請輸入權限密碼："},
                                                                                          {Language.CHS, "请输入权限密码："},
                                                                                          {Language.EN, "Please enter the permission password:"}
                                                                                      },
                                                                                      new Dictionary<Language, string>
                                                                                      {
                                                                                          {Language.TW, "驗證"},
                                                                                          {Language.CHS, "验证"},
                                                                                          {Language.EN, "Identify"}
                                                                                      },
                                                                                      true,
                                                                                      x => (x.ToString() == user.Password,
                                                                                            new Dictionary<Language, string>
                                                                                            {
                                                                                                {Language.TW, "密碼錯誤！"},
                                                                                                {Language.CHS, "密码错误！"},
                                                                                                {Language.EN, "Wrong password!"}
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
                                                             {Language.TW, "權限不足，不可關閉程式！"},
                                                             {Language.CHS, "权限不足，不可关闭程序！"},
                                                             {Language.EN, "Insufficient permissions,\ncan't close the program."}
                                                         });
                                       }
                                   };

            //!當配方列表更新時，依據使用站別發佈配方
            RecipeVM.ListUpdatedEvent += e =>
                                         {
                                             var (list, showtip) = e;

                                             //todo 配方更新至C:\ITRIinit\0\ProcessJob
                                             //todo 引發GemProcessProgramChange並且更新兩個DV:GemPPChangeName、GemPPChangeStatus

                                             var di = new DirectoryInfo("C:\\ITRIinit\\0\\ProcessJob");

                                             foreach (var file in di.GetFiles())
                                             {
                                                 file.Delete();
                                             }

                                             foreach (var recipe in list)
                                             {
                                                 var si = new StreamReaderIni();
                                                 var t  = si.AddIniSection("CCodeID1");
                                                 t.AddElement(nameof(PLC_Recipe.ProgramEndWarningTime), recipe.ProgramEndWarningTime.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.CoolingTime),           recipe.CoolingTime.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.CoolingTemperature),    recipe.CoolingTemperature.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.StepCounts),            recipe.StepCounts.ToString());
                                                 t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_1), recipe.TemperatureSetpoint_1.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_2), recipe.TemperatureSetpoint_2.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_3), recipe.TemperatureSetpoint_3.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_4), recipe.TemperatureSetpoint_4.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_5), recipe.TemperatureSetpoint_5.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.TemperatureSetpoint_6), recipe.TemperatureSetpoint_6.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampTime_1),            recipe.RampTime_1.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampTime_2),            recipe.RampTime_2.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampTime_3),            recipe.RampTime_3.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampTime_4),            recipe.RampTime_4.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampTime_5),            recipe.RampTime_5.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampTime_6),            recipe.RampTime_6.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampAlarm_1),           recipe.RampAlarm_1.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampAlarm_2),           recipe.RampAlarm_2.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampAlarm_3),           recipe.RampAlarm_3.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampAlarm_4),           recipe.RampAlarm_4.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampAlarm_5),           recipe.RampAlarm_5.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.RampAlarm_6),           recipe.RampAlarm_6.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellTime_1),           recipe.DwellTime_1.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellTime_2),           recipe.DwellTime_2.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellTime_3),           recipe.DwellTime_3.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellTime_4),           recipe.DwellTime_4.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellTime_5),           recipe.DwellTime_5.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellTime_6),           recipe.DwellTime_6.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellAlarm_1),          recipe.DwellAlarm_1.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellAlarm_2),          recipe.DwellAlarm_2.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellAlarm_3),          recipe.DwellAlarm_3.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellAlarm_4),          recipe.DwellAlarm_4.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellAlarm_5),          recipe.DwellAlarm_5.ToString("0.0"));
                                                 t.AddElement(nameof(PLC_Recipe.DwellAlarm_6),          recipe.DwellAlarm_6.ToString("0.0"));
                                                 si.EncodindIni($"C:\\ITRIinit\\0\\ProcessJob\\{recipe.RecipeName}.pjb");
                                             }

                                             TotalVM.SetRecipeNames(list.Select(x => x.RecipeName).ToArray());

                                             var l = new List<int>();

                                             for (var i = 0; i < TotalVM.PLC_All.Count; i++)
                                             {
                                                 var j      = i;
                                                 var recipe = list.Find(x => j < x.Used_Stations.Count && x.Used_Stations[j]);
                                                 if (recipe != null)
                                                 {
                                                     if (!TotalVM.SetRecipe(i, recipe))
                                                     {
                                                         l.Add(i + 1);
                                                     }
                                                 }
                                             }

                                             if (showtip && l.Any())
                                             {
                                                 var str = string.Join(", ", l);

                                                 DialogVM.Show(new Dictionary<Language, string>
                                                               {
                                                                   {Language.TW, $"{str} 號\n烤箱目前無法寫入配方！"},
                                                                   {Language.CHS, $"{str} 号\n烤箱目前无法写入配方！"},
                                                                   {Language.EN, $"No. {str} oven{(l.Count > 1 ? "s" : "")} {(l.Count > 1 ? "are" : "is")}\ncurrently unable to write the recipe!"}
                                                               },
                                                               TimeSpan.FromSeconds(3),
                                                               DialogMsgType.Alert);
                                             }

                                             //!當偵測到有測試資料需生成時
                                             if (_Testdatalock != null && !_Testdatalock.IsSet())
                                             {
                                                 _Testdatalock.Set();
                                             }
                                         };

            //!當某站烤箱要求配方時，自資料庫讀取配方並發送
            TotalVM.GetRecipe += e => string.IsNullOrEmpty(e.RecipeName) ? null : RecipeVM.GetRecipe(e.RecipeName);

            //!設定配方被該站使用
            TotalVM.RecipeUsed += e => RecipeVM.SetUsed(e.StationIndex, e.RecipeName);

            //!由板架code取得前端生產資訊
            TotalVM.WantFrontData += async e =>
                                     {
                                         var (stationIndex, RackID) = e;
                                         var path = $"{DataInputPath}\\{RackID}";

                                         if (Directory.Exists(path))
                                         {
                                             var products = new List<(string ordercode, int number, string PanelID)>();

                                             await Task.Factory.StartNew(() =>
                                                                         {
                                                                             var files = new DirectoryInfo(path).GetFiles("*.txt");
                                                                             foreach (var file in files)
                                                                             {
                                                                                 try
                                                                                 {
                                                                                     var str = File.ReadAllText(file.FullName, Encoding.ASCII);
                                                                                     var result = str.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None)
                                                                                                     .Where(x => x.StartsWith("General") && x.Contains("="))
                                                                                                     .Select(x => x.Split('='))
                                                                                                     .ToDictionary(x => x[0], x => x[1]);

                                                                                     int.TryParse(Extensions.OnlyASCII(result["General2"]), out var number);
                                                                                     products.Add((Extensions.OnlyASCII(result["General1"]), number, Extensions.OnlyASCII(result["General7"])));

                                                                                     var backname = $"{file.FullName}.bak{stationIndex}";
                                                                                     if (File.Exists(backname))
                                                                                     {
                                                                                         File.Delete(backname);
                                                                                     }

                                                                                     file.MoveTo(backname);
                                                                                 }
                                                                                 catch (Exception ex)
                                                                                 {
                                                                                     Log.Error(ex, "");
                                                                                 }
                                                                             }
                                                                         });

                                             return products.GroupBy(x => x.ordercode)
                                                            .Select(x => new ProductInfo(x.Key, x.First().number)
                                                                         {
                                                                             PanelIDs = x.Select(y => y.PanelID).ToList()
                                                                         })
                                                            .ToList();
                                         }

                                         try
                                         {
                                             Directory.CreateDirectory(path);
                                         }
                                         catch (Exception ex)
                                         {
                                             Log.Error(ex, "板架資料夾不存在且無法創建");
                                         }

                                         return null;
                                     };

            TotalVM.GetUser += () => User;

            TotalVM.CancelCheckIn += e =>
                                     {
                                         var (stationIndex, RackID) = e;
                                         var path = $"{DataInputPath}\\{RackID}";

                                         if (!Directory.Exists(path))
                                         {
                                             return;
                                         }

                                         var tag   = $".bak{stationIndex}";
                                         var files = new DirectoryInfo(path).GetFiles($"*{tag}");
                                         foreach (var file in files)
                                         {
                                             var sourcename = file.FullName.TrimEnd(tag.ToCharArray());

                                             if (File.Exists(sourcename))
                                             {
                                                 File.Delete(sourcename);
                                             }

                                             file.MoveTo(sourcename);
                                         }
                                     };

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

            //!更新每日產量
            TraceVM.TodayProductionUpdated += datas =>
                                              {
                                                  foreach (var (StationIndex, Production) in datas)
                                                  {
                                                      if (StationIndex < TotalVM.TotalProduction.Count)
                                                      {
                                                          TotalVM.TotalProduction[StationIndex] = Production;
                                                      }
                                                  }
                                              };

            LogVM.WantInfo += async e => await TraceVM.FindInfo(e.station, e.time);

            LogVM.GoDetailView += async e =>
                                  {
                                      TraceVM.Standby  = false; //!強制讓TraceVM處於須等待狀態，因此時畫面仍在變化仍未loaded，但TraceVM.Standby為true，將導致以下的迴圈等待沒效果
                                      MainVM.ViewIndex = 2;

                                      await Task.Factory.StartNew(() =>
                                                                  {
                                                                      do
                                                                      {
                                                                          Thread.Sleep(30);
                                                                      } while (!TraceVM.Standby);
                                                                  });

                                      var (info, logEvent) = e;
                                      TraceVM.SearchResult = info;
                                      TraceVM.SearchEvent  = logEvent;
                                      TraceVM.Date1        = info.AddedTime.Date;
                                  };

            //MakeTestData(20);
        }
    }
}