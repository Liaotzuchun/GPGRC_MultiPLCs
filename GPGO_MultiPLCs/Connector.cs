using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using GPGO_MultiPLCs.ViewModels;
using MongoDB.Driver;
using Serilog;

namespace GPGO_MultiPLCs
{
    public sealed class Connector : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty OvenCountProperty = DependencyProperty.Register(nameof(OvenCount), typeof(int), typeof(Connector), new PropertyMetadata(0, OvenCountChanged));
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
            ((Connector)sender).TraceVM.Language = lng;
            ((Connector)sender).LogVM.Language = lng;
        }

        public Language Language
        {
            get => (Language)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }

        private static void OvenCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var count = (int)e.NewValue;
            ((Connector)sender).TotalVM.TotalProduction_ViewCount = count;
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

        public void Dispose()
        {
            DialogVM.Dispose();
            TotalVM.Dispose();
        }

        private readonly AsyncLock lockobj = new AsyncLock();

        private AsyncAutoResetEvent _Testdatalock;

        public GlobalDialog_ViewModel DialogVM { get; }
        public LogView_ViewModel LogVM { get; }
        public MainWindow_ViewModel MainVM { get; }
        public RecipeControl_ViewModel RecipeVM { get; }
        public TotalView_ViewModel TotalVM { get; }
        public TraceabilityView_ViewModel TraceVM { get; }

        /// <summary>產生測試資料至資料庫</summary>
        /// <param name="PLC_Count"></param>
        public async void MakeTestData(int PLC_Count)
        {
            _Testdatalock = new AsyncAutoResetEvent();
            await _Testdatalock.WaitAsync();

            var order_code = new[] { "ooxx", "abc", "zzz", "qoo", "boom", "xxx", "wunmao" };
            var time = DateTime.Now;

            for (var j = 1; j <= new DateTime(time.Year, time.Month, 1).AddMonths(1).AddDays(-1).Day; j++)
            {
                for (var i = 0; i < PLC_Count; i++)
                {
                    var rn = new Random(i + j);

                    var st = new DateTime(time.Year, time.Month, j, 8, i + rn.Next(0, 10), i);

                    for (var k = 0; k < 8; k++)
                    {
                        var info = new ProcessInfo { StartTime = st, TrolleyCode = rn.Next(1, 10000).ToString("00000"), OperatorID = rn.Next(1, 10).ToString("000") };
                        var h = new int[]
                                {
                                    TotalVM.PLC_All[i].HeatingTime_1,
                                    TotalVM.PLC_All[i].HeatingTime_2,
                                    TotalVM.PLC_All[i].HeatingTime_3,
                                    TotalVM.PLC_All[i].HeatingTime_4,
                                    TotalVM.PLC_All[i].HeatingTime_5,
                                    TotalVM.PLC_All[i].HeatingTime_6,
                                    TotalVM.PLC_All[i].HeatingTime_7,
                                    TotalVM.PLC_All[i].HeatingTime_8
                                };
                        var w = new int[]
                                {
                                    TotalVM.PLC_All[i].WarmingTime_1,
                                    TotalVM.PLC_All[i].WarmingTime_2,
                                    TotalVM.PLC_All[i].WarmingTime_3,
                                    TotalVM.PLC_All[i].WarmingTime_4,
                                    TotalVM.PLC_All[i].WarmingTime_5,
                                    TotalVM.PLC_All[i].WarmingTime_6,
                                    TotalVM.PLC_All[i].WarmingTime_7,
                                    TotalVM.PLC_All[i].WarmingTime_8
                                };
                        var t = new[]
                                {
                                    TotalVM.PLC_All[i].TargetTemperature_1,
                                    TotalVM.PLC_All[i].TargetTemperature_2,
                                    TotalVM.PLC_All[i].TargetTemperature_3,
                                    TotalVM.PLC_All[i].TargetTemperature_4,
                                    TotalVM.PLC_All[i].TargetTemperature_5,
                                    TotalVM.PLC_All[i].TargetTemperature_6,
                                    TotalVM.PLC_All[i].TargetTemperature_7,
                                    TotalVM.PLC_All[i].TargetTemperature_8
                                };
                        var s = new[]
                                {
                                    TotalVM.PLC_All[i].ThermostaticTemperature_1,
                                    TotalVM.PLC_All[i].ThermostaticTemperature_2,
                                    TotalVM.PLC_All[i].ThermostaticTemperature_3,
                                    TotalVM.PLC_All[i].ThermostaticTemperature_4,
                                    TotalVM.PLC_All[i].ThermostaticTemperature_5,
                                    TotalVM.PLC_All[i].ThermostaticTemperature_6,
                                    TotalVM.PLC_All[i].ThermostaticTemperature_7,
                                    TotalVM.PLC_All[i].ThermostaticTemperature_8
                                };
                        Array.Resize(ref h, TotalVM.PLC_All[i].UsedSegmentCounts);
                        Array.Resize(ref w, TotalVM.PLC_All[i].UsedSegmentCounts);
                        Array.Resize(ref t, TotalVM.PLC_All[i].UsedSegmentCounts);
                        Array.Resize(ref s, TotalVM.PLC_All[i].UsedSegmentCounts);

                        info.RecipeName = TotalVM.PLC_All[i].RecipeName;
                        info.HeatingTimes = h.ToList();
                        info.WarmingTimes = w.ToList();
                        info.TargetOvenTemperatures = t.ToList();
                        info.ThermostaticTemperatures = s.ToList();

                        var ttime = new TimeSpan(0, 0, 1);
                        var cc = 0;

                        var _ev = new LogEvent
                                 {
                                     StationNumber = i + 1,
                                     StartTime = st,
                                     AddedTime = st + ttime,
                                     Description = "第1段升溫",
                                     TagCode = 100,
                                     Type = EventType.Normal,
                                     Value = true
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
                                             StartTime = st,
                                             AddedTime = st + ttime - TimeSpan.FromMilliseconds(1),
                                             Description = "第1段升溫",
                                             TagCode = 100,
                                             Type = EventType.Normal,
                                             Value = false
                                         };

                                LogVM.AddToDB(ev1);
                                info.EventList.Add(ev1);

                                var ev2 = new LogEvent
                                          {
                                              StationNumber = i + 1,
                                              StartTime = st,
                                              AddedTime = st + ttime,
                                              Description = "第1段恆溫",
                                              TagCode = 101,
                                              Type = EventType.Normal,
                                              Value = true
                                          };

                                LogVM.AddToDB(ev2);
                                info.EventList.Add(ev2);
                            }

                            var tempt = 30 * (1 + 5 * 1 / (1 + Math.Exp(-0.12 * cc + 3)));
                            var vals = new RecordTemperatures
                                       {
                                           StartTime = st,
                                           AddedTime = st + ttime,
                                           ThermostatTemperature = tempt,
                                           OvenTemperatures_1 = tempt + rn.Next(-5, 5),
                                           OvenTemperatures_2 = tempt + rn.Next(-5, 5),
                                           OvenTemperatures_3 = tempt + rn.Next(-5, 5),
                                           OvenTemperatures_4 = tempt + rn.Next(-5, 5),
                                           OvenTemperatures_5 = tempt + rn.Next(-5, 5),
                                           OvenTemperatures_6 = tempt + rn.Next(-5, 5),
                                           OvenTemperatures_7 = tempt + rn.Next(-5, 5),
                                           OvenTemperatures_8 = tempt + rn.Next(-5, 5)
                                       };

                            cc += 1;
                            info.RecordTemperatures.Add(vals);

                            ttime = ttime.Add(TimeSpan.FromMinutes(1));
                        }

                        var ev_1 = new LogEvent
                                  {
                                      StationNumber = i + 1,
                                      StartTime = st,
                                      AddedTime = st + ttime - TimeSpan.FromSeconds(60),
                                      Description = "第1段恆溫",
                                      TagCode = 100,
                                      Type = EventType.Normal,
                                      Value = false
                                  };

                        LogVM.AddToDB(ev_1);
                        info.EventList.Add(ev_1);

                        var ev_2 = new LogEvent
                                  {
                                      StationNumber = i + 1,
                                      StartTime = st,
                                      AddedTime = st + ttime - TimeSpan.FromSeconds(1),
                                      Description = "程式結束",
                                      TagCode = 102,
                                      Type = EventType.Trigger,
                                      Value = true
                                  };

                        LogVM.AddToDB(ev_2);
                        info.EventList.Add(ev_2);

                        var ev_3 = new LogEvent
                                   {
                                       StationNumber = i + 1,
                                       StartTime = st,
                                       AddedTime = st + ttime,
                                       Description = "自動停止",
                                       TagCode = 102,
                                       Type = EventType.Trigger,
                                       Value = true
                                   };

                        LogVM.AddToDB(ev_3);
                        info.EventList.Add(ev_3);

                        info.EndTime = info.StartTime + ttime;
                        info.TotalHeatingTime = (info.EndTime - info.StartTime).Minutes;

                        st = info.EndTime + TimeSpan.FromMinutes(10);

                        var infos = new List<ProcessInfo>();
                        var temp = new List<int>();
                        var n = rn.Next(0, 1);
                        for (var p = 0; p <= n; p++)
                        {
                            var _info = info.Copy();
                            var index = rn.Next(0, order_code.Length);
                            while (temp.Contains(index))
                            {
                                index = rn.Next(0, order_code.Length);
                            }

                            temp.Add(index);
                            _info.OrderCode = order_code[index];

                            var count = rn.Next(10, 20);
                            for (var m = 0; m < count; m++)
                            {
                                _info.PanelCodes.Add(new object().GetHashCode().ToString());
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
            MainVM = new MainWindow_ViewModel();
            RecipeVM = new RecipeControl_ViewModel(new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("PLC_Recipes")),
                                                   new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("Old_PLC_Recipes")),
                                                   DialogVM);
            TraceVM = new TraceabilityView_ViewModel(new MongoBase<ProcessInfo>(db.GetCollection<ProcessInfo>("Product_Infos")), DialogVM);
            LogVM = new LogView_ViewModel(new MongoBase<LogEvent>(db.GetCollection<LogEvent>("Event_Logs")), DialogVM);

            var map = new PLC_DevicesMap(
                                         //!PLC的M區位置
                                         new Dictionary<SignalNames, int>
                                         {
                                             { SignalNames.PC_InUsed, 20 },
                                             { SignalNames.自動模式, 50 },
                                             { SignalNames.自動啟動, 51 },
                                             { SignalNames.自動停止, 52 },
                                             { SignalNames.手動模式, 60 },
                                             { SignalNames.降溫中, 208 },
                                             { SignalNames.程式結束, 209 },
                                             { SignalNames.加熱門未關, 250 },
                                             { SignalNames.緊急停止, 700 },
                                             { SignalNames.溫控器低溫異常, 701 },
                                             { SignalNames.電源反相, 702 },
                                             { SignalNames.OTP超溫異常, 703 },
                                             { SignalNames.循環風車過載, 704 },
                                             { SignalNames.冷卻進氣風車異常, 705 },
                                             { SignalNames.超溫警報, 710 },
                                             { SignalNames.停止後未開門, 715 },
                                             { SignalNames.循環風車INV異常, 718 },
                                             { SignalNames.充氮氣逾時, 721 },
                                             { SignalNames.門未關定位異常, 722 },
                                             { SignalNames.升恆溫逾時, 723 }
                                         },
                                         //!PLC的D區位置
                                         new Dictionary<DataNames, int>
                                         {
                                             { DataNames.溫控器溫度, 130 },
                                             { DataNames.片段剩餘時間, 132 },
                                             { DataNames.總剩餘時間, 134 },
                                             { DataNames.目前段數, 140 },
                                             { DataNames.爐內溫度_1, 380 },
                                             { DataNames.爐內溫度_2, 381 },
                                             { DataNames.爐內溫度_3, 382 },
                                             { DataNames.爐內溫度_4, 383 },
                                             { DataNames.爐內溫度_5, 384 },
                                             { DataNames.爐內溫度_6, 385 },
                                             { DataNames.爐內溫度_7, 386 },
                                             { DataNames.爐內溫度_8, 387 }
                                         },
                                         //!PLC的配方參數位置
                                         new Dictionary<DataNames, int>
                                         {
                                             { DataNames.目標溫度_1, 812 },
                                             { DataNames.升溫時間_1, 813 },
                                             { DataNames.恆溫溫度_1, 814 },
                                             { DataNames.恆溫時間_1, 815 },
                                             { DataNames.目標溫度_2, 816 },
                                             { DataNames.升溫時間_2, 817 },
                                             { DataNames.恆溫溫度_2, 818 },
                                             { DataNames.恆溫時間_2, 819 },
                                             { DataNames.目標溫度_3, 820 },
                                             { DataNames.升溫時間_3, 821 },
                                             { DataNames.恆溫溫度_3, 822 },
                                             { DataNames.恆溫時間_3, 823 },
                                             { DataNames.目標溫度_4, 824 },
                                             { DataNames.升溫時間_4, 825 },
                                             { DataNames.恆溫溫度_4, 826 },
                                             { DataNames.恆溫時間_4, 827 },
                                             { DataNames.目標溫度_5, 828 },
                                             { DataNames.升溫時間_5, 829 },
                                             { DataNames.恆溫溫度_5, 830 },
                                             { DataNames.恆溫時間_5, 831 },
                                             { DataNames.目標溫度_6, 832 },
                                             { DataNames.升溫時間_6, 833 },
                                             { DataNames.恆溫溫度_6, 834 },
                                             { DataNames.恆溫時間_6, 835 },
                                             { DataNames.目標溫度_7, 836 },
                                             { DataNames.升溫時間_7, 837 },
                                             { DataNames.恆溫溫度_7, 838 },
                                             { DataNames.恆溫時間_7, 839 },
                                             { DataNames.目標溫度_8, 840 },
                                             { DataNames.升溫時間_8, 841 },
                                             { DataNames.恆溫溫度_8, 842 },
                                             { DataNames.恆溫時間_8, 843 },
                                             { DataNames.降溫溫度, 845 },
                                             { DataNames.充氣時間, 847 },
                                             { DataNames.使用段數, 849 },
                                             { DataNames.配方名稱_01, 850 },
                                             { DataNames.配方名稱_02, 851 },
                                             { DataNames.配方名稱_03, 852 },
                                             { DataNames.配方名稱_04, 853 },
                                             { DataNames.配方名稱_05, 854 },
                                             { DataNames.配方名稱_06, 855 },
                                             { DataNames.配方名稱_07, 856 },
                                             { DataNames.配方名稱_08, 857 },
                                             { DataNames.配方名稱_09, 858 },
                                             { DataNames.配方名稱_10, 859 },
                                             { DataNames.配方名稱_11, 860 },
                                             { DataNames.配方名稱_12, 861 },
                                             { DataNames.配方名稱_13, 862 }
                                         });

            TotalVM = new TotalView_ViewModel(Enumerable.Repeat(map, 20).ToArray(), DialogVM);

            //!當回到主頁時，也將生產總覽回到總覽頁
            MainVM.IndexChangedEvent += index =>
                                        {
                                            if (index == 0)
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
                                       if (TotalVM.PLC_All.Any(plc => plc.IsRecording))
                                       {
                                           DialogVM.Show(new Dictionary<Language, string>
                                                         {
                                                             { Language.TW, "仍在生產中，無法終止程式！" }, { Language.CHS, "仍在生产中，无法终止程序！" }, { Language.EN, "Still processing,\ncannot terminate the program." }
                                                         });
                                       }
                                       else if (User.Level > User.UserLevel.Operator)
                                       {
                                           var user = User.Copy();
                                           var result = await DialogVM.ShowWithIntput(new Dictionary<Language, string>
                                                                                      {
                                                                                          { Language.TW, "請輸入權限密碼：" },
                                                                                          { Language.CHS, "请输入权限密码：" },
                                                                                          { Language.EN, "Please enter the permission password:" }
                                                                                      },
                                                                                      new Dictionary<Language, string> { { Language.TW, "驗證" }, { Language.CHS, "验证" }, { Language.EN, "Identify" } },
                                                                                      x => (x == user.Password,
                                                                                            new Dictionary<Language, string>
                                                                                            {
                                                                                                { Language.TW, "密碼錯誤！" }, { Language.CHS, "密码错误！" }, { Language.EN, "Wrong password!" }
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
                                                             { Language.TW, "權限不足，不可關閉程式！" }, { Language.CHS, "权限不足，不可关闭程序！" }, { Language.EN, "Insufficient permissions,\ncan't close the program." }
                                                         });
                                       }
                                   };

            //!當配方列表更新時，依據使用站別發佈配方
            RecipeVM.ListUpdatedEvent += async e =>
                                         {
                                             var (list, tip) = e;

                                             TotalVM.SetRecipeNames(list.Select(x => x.RecipeName).ToArray());

                                             var l = new List<int>();

                                             for (var i = 0; i < TotalVM.PLC_All.Count; i++)
                                             {
                                                 var j = i;
                                                 var recipe = list.Find(x => x.Used_Stations[j]);
                                                 if (recipe != null)
                                                 {
                                                     if (!await TotalVM.SetRecipe(i, recipe, true))
                                                     {
                                                         l.Add(i + 1);
                                                     }
                                                 }
                                             }

                                             if (tip && l.Any())
                                             {
                                                 var str = string.Join(", ", l);

                                                 DialogVM.Show(new Dictionary<Language, string>
                                                               {
                                                                   { Language.TW, $"{str} 號\n烤箱目前無法寫入配方！" },
                                                                   { Language.CHS, $"{str} 号\n烤箱目前无法写入配方！" },
                                                                   { Language.EN, $"No. {str} oven{(l.Count > 1 ? "s" : "")} {(l.Count > 1 ? "are" : "is")}\ncurrently unable to write the recipe!" }
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

            //!由台車code取得前端生產資訊
            TotalVM.WantFrontData += async e =>
                                     {
                                         var path = $"{DataInputPath}\\{e.TrolleyCode}";

                                         if (Directory.Exists(path))
                                         {
                                             var products = new List<(string ordercode, int number, string panelcode)>();

                                             await Task.Factory.StartNew(() =>
                                                                         {
                                                                             var files = new DirectoryInfo(path).GetFiles("*.txt");
                                                                             foreach (var file in files)
                                                                             {
                                                                                 try
                                                                                 {
                                                                                     var str = File.ReadAllText(file.FullName, Encoding.ASCII);
                                                                                     var result = str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                                                                                                     .Where(x => x.StartsWith("General") && x.Contains("="))
                                                                                                     .Select(x => x.Split('='))
                                                                                                     .ToDictionary(x => x[0], x => x[1]);

                                                                                     int.TryParse(result["General2"].OnlyASCII(), out var number);
                                                                                     products.Add((result["General1"].OnlyASCII(), number, result["General7"].OnlyASCII()));

                                                                                     var backname = $"{file.FullName}.bak{e.StationIndex}";
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
                                                            .Select(x => new ProductInfo(x.Key, x.First().number) { PanelCodes = x.Select(y => y.panelcode).ToList() })
                                                            .ToList();
                                         }

                                         try
                                         {
                                             Directory.CreateDirectory(path);
                                         }
                                         catch (Exception ex)
                                         {
                                             Log.Error(ex, "台車資料夾不存在且無法創建");
                                         }

                                         return null;
                                     };

            TotalVM.CancelCheckIn += e =>
                                     {
                                         var path = $"{DataInputPath}\\{e.TrolleyCode}";

                                         if (Directory.Exists(path))
                                         {
                                             var tag = $".bak{e.StationIndex}";
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
                                         }
                                     };

            //!當某站烤箱完成烘烤程序時，將生產資訊寫入資料庫並輸出至上傳資料夾，並回傳當日產量
            TotalVM.AddRecordToDB += async e =>
                                     {
                                         var inpath = "";
                                         var outpath = "";

                                         Dispatcher.Invoke(() =>
                                                           {
                                                               inpath = DataInputPath;
                                                               outpath = DataOutputPath;
                                                           });

                                         using (await lockobj.LockAsync())
                                         {
                                             TraceVM.AddToDB(e.StationIndex, e.Infos);

                                             //!輸出嘉聯益資料
                                             if (!string.IsNullOrEmpty(inpath) && !string.IsNullOrEmpty(outpath) && e.Infos.Any())
                                             {
                                                 if (!Directory.Exists(outpath))
                                                 {
                                                     try
                                                     {
                                                         Directory.CreateDirectory(outpath);
                                                     }
                                                     catch (Exception ex)
                                                     {
                                                         Log.Error(ex, "上傳資料夾不存在且無法創建");
                                                     }
                                                 }

                                                 foreach (var info in e.Infos)
                                                 {
                                                     for (var i = 0; i < info.ProcessCount; i++)
                                                     {
                                                         var path = $"{outpath}\\{info.AssetNumber}_{DateTime.Now:yyyyMMddHHmmssfff}_{e.StationIndex + 1}_";

                                                         var n = 1;
                                                         while (File.Exists($"{path}{n}"))
                                                         {
                                                             n++;
                                                         }

                                                         try
                                                         {
                                                             using (var outputFile = new StreamWriter($"{path}{n}.txt", false, Encoding.ASCII))
                                                             {
                                                                 await outputFile.WriteAsync(info.ToString(i));
                                                             }

                                                             await Task.Delay(1);
                                                             //!紀錄資料到指定輸出資料夾
                                                         }
                                                         catch (Exception ex)
                                                         {
                                                             Log.Error(ex, "資料輸出上傳失敗");
                                                         }
                                                     }
                                                 }

                                                 var _path = $"{inpath}\\{e.Infos.First().TrolleyCode}";

                                                 if (Directory.Exists(_path))
                                                 {
                                                     var tag = $".bak{e.StationIndex}";
                                                     var files = new DirectoryInfo(_path).GetFiles($"*{tag}");
                                                     foreach (var file in files)
                                                     {
                                                         file.Delete();
                                                     }
                                                 }
                                             }
                                         }

                                         return await TraceVM.CheckProductions(e.StationIndex);
                                     };

            TotalVM.EventHappened += e =>
                                     {
                                         LogVM.AddToDB(new LogEvent
                                                       {
                                                           StationNumber = e.StationIndex + 1,
                                                           AddedTime = e.time,
                                                           Type = e.type,
                                                           Description = e.note,
                                                           TagCode = e.tag,
                                                           Value = e.value
                                                       });
                                     };

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
                                      MainVM.ViewIndex = 2;

                                      await Task.Factory.StartNew(() =>
                                                                  {
                                                                      //Thread.Sleep(300);

                                                                      do
                                                                      {
                                                                          Thread.Sleep(30);
                                                                      } while (!TraceVM.Standby);
                                                                  });

                                      TraceVM.SearchResult = e.info;
                                      TraceVM.SearchEvent = e._event;
                                      TraceVM.Date1 = e.info.AddedTime.Date;
                                  };

            //MakeTestData(20);
        }
    }
}