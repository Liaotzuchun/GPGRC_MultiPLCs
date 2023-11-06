using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using GPGO_MultiPLCs.Models;
using GPGO_MultiPLCs.ViewModels;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.MongoDB.Helpers;
using GPMVVM.PooledCollections;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;
using static GPGO_MultiPLCs.Models.WebDataModel;
using Extensions = GPGO_MultiPLCs.Helpers.Extensions;
#pragma warning disable VSTHRD101

namespace GPGO_MultiPLCs;

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

    public TcpClient mTcpClient
    {
        get => Get<TcpClient>();
        set => Set(value);
    }
    public ServiceHost webServiceHost
    {
        get;
        set;
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
    public bool IsHeartbeat
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool IsNotHeartbeat
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool UseHeartbeat
    {
        get => Get<bool>();
        set => Set(value);
    }
    public Visibility EditTopVisibility
    {
        get => Get<Visibility>();
        set
        {
            Set(value);
            NotifyPropertyChanged();
        }
    }
    public Visibility DetailTopVisibility
    {
        get => Get<Visibility>();
        set
        {
            Set(value);
            NotifyPropertyChanged();
        }
    }
    public Visibility EditBottomVisibility
    {
        get => Get<Visibility>();
        set
        {
            Set(value);
            NotifyPropertyChanged();
        }
    }
    public Visibility DetailBottomVisibility
    {
        get => Get<Visibility>();
        set
        {
            Set(value);
            NotifyPropertyChanged();
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
    public string CarrierID { get; private set; }

    public Mediator()
    {
        var db = new MongoClient("mongodb://localhost:27017").GetDatabase("GP");
        DialogVM = new GlobalDialog_ViewModel();
        MainVM = new MainWindow_ViewModel();
        RecipeVM = new RecipeControl_ViewModel(new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("PLC_Recipes")),
                                               new MongoBase<PLC_Recipe>(db.GetCollection<PLC_Recipe>("Old_PLC_Recipes")),
                                               DialogVM);
        TraceVM = new TraceabilityView_ViewModel(new MongoBase<ProcessInfo>(db.GetCollection<ProcessInfo>("ProductInfos")), DialogVM);
        LogVM = new LogView_ViewModel(new MongoBase<LogEvent>(db.GetCollection<LogEvent>("EventLogs")), DialogVM);
        PlcGate = new JsonRPCPLCGate();
        AuthenticatorVM = new Authenticator_ViewModel();
        TotalVM = new TotalView_ViewModel(AuthenticatorVM.Settings.OvenCount, PlcGate, IPAddress.Parse(AuthenticatorVM.IPString), DialogVM);
        Language = AuthenticatorVM.Settings.Lng;
        OvenCount = AuthenticatorVM.Settings.OvenCount;
        EditBottomVisibility = Visibility.Hidden;
        DetailBottomVisibility = Visibility.Hidden;
        IsNotHeartbeat = true;
        Task.Factory.StartNew(ReadMessage, TaskCreationOptions.LongRunning);
        Task.Factory.StartNew(HeartbeatRun);

        AuthenticatorVM.NowUser = new User
        {
            Name = "Guest",
            Password = "",
            Level = UserLevel.Guest
        };
        User = AuthenticatorVM.NowUser;

        SCC_ServerSideRef.MacIntfWSClient Web = new SCC_ServerSideRef.MacIntfWSClient();
        Web.Open();

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
                case nameof(GlobalSettings.RecordDelay):
                    RecordDelay = ((GlobalSettings)s).RecordDelay;
                    break;
                case nameof(GlobalSettings.ClearInputDelay):
                    ClearInputDelay = ((GlobalSettings)s).ClearInputDelay;
                    break;
            }
        };

        AuthenticatorVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Authenticator_ViewModel.NowUser))
            {
                User = ((Authenticator_ViewModel)s).NowUser;
                Extensions.IsGodMode = User?.Level >= UserLevel.Administrator;
            }
        };

        AuthenticatorVM.Settings.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(AuthenticatorVM.Settings.UseHeart))
            {
                MainVM.UseHeart = AuthenticatorVM.Settings.UseHeart ? Visibility.Visible : Visibility.Hidden;
            }
        };

        //開啟心跳
        AuthenticatorVM.BtnHeartBeatEvent += async (e) =>
        {
            IsHeartbeat = UseHeartbeat = e;
            if (e)
            {
                GPServiceHostFunc();
                IsNotHeartbeat = false;
            }
            else
            {
                IsNotHeartbeat = true;
            }
        };

        AuthenticatorVM.BtnSaveEvent += async () =>
        {
            DialogVM.Show(new Dictionary<Language, string>
                                                     {
                                                         { Language.TW, "MES設定存檔成功" },
                                                         { Language.CHS, "MES设定存档成功" },
                                                     });
        };
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

        //! 當配方列表更新時，依據使用站別發佈配方
        RecipeVM.ListUpdatedEvent += async e =>
        {
            var (list, added, removed, updated, showtip) = e;
            if (list != null)
            {
                TotalVM.SetRecipeNames(list.Select(x => x.RecipeName).ToList());

                //var path = $"{TotalVM.SecsGemEquipment.BasePath}\\ProcessJob";
                var path = $"\\ProcessJob";

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
                    //      TotalVM.InvokeRecipe(add.RecipeName, PPStatus.Create);
                }
            }

            if (removed != null)
            {
                foreach (var remove in removed)
                {
                    //    TotalVM.InvokeRecipe(remove.RecipeName, PPStatus.Delete);
                }
            }

            if (updated != null)
            {
                foreach (var update in updated)
                {
                    //TotalVM.InvokeRecipe(update.RecipeName, PPStatus.Change); 
                }
            }
        };

        TotalVM.CheckUser += op => AuthenticatorVM.UserList.List.Exists(x => x.Name.ToUpper() == op);

        //! 當某站烤箱要求配方時，自資料庫讀取配方並發送
        TotalVM.GetRecipe += recipename => string.IsNullOrEmpty(recipename) ? null : RecipeVM.GetRecipe(recipename);

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
                await CsvCreator.AddInfo(info, AuthenticatorVM.Settings.DataOutputPath);
            }

            return await TraceVM.CheckProductions(stationIndex);
        };

        TotalVM.EventHappened += e =>
        {
            var (stationIndex, type, time, note, tag, value) = e;
            if (type == EventType.Alarm)
            {
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

                #region 發生事件 傳送AlarmUpload
                try
                {
                    var methodInvoke = "AlarmUpload";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var alarmCode = logevent.TagCode;
                    var alarmDesc = logevent.Description;
                    var input = GetAlarmXml(methodInvoke,macCode,alarmCode,alarmDesc);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] ,ResultData:[{ResultData}] ");
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.Message);
                }
                #endregion
            }

        };

        TotalVM.UpsertRecipe += recipe => RecipeVM.Upsert(recipe).Result;

        TotalVM.DeleteRecipe += recipe => RecipeVM.Delete(recipe).Result;

        #region WebService
        #region ---------Top---------
        #region 叫料
        TotalVM.PLC_All[0].TopAddAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"In_{AuthenticatorVM.Settings.CallCarrierID}" ;
                    var wipEntity = "";
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        //開啟作業管控功能
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopIngredientsButtonEnabled = true;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcodeEnabled = true;
                        Thread.Sleep(AuthenticatorVM.Settings.AVGTime * 1000);
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  当前没有计划工单 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                    }
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopAddEnabled = true;

                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.Message;
                    Log.Debug(ex.Message);
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopAddEnabled = true;
                }
            });
        };
        TotalVM.PLC_All[1].TopAddAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"In_{AuthenticatorVM.Settings.CallCarrierID}" ;
                    var wipEntity = "";
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        //開啟作業管控功能
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopIngredientsButtonEnabled = true;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcodeEnabled = true;
                        Thread.Sleep(AuthenticatorVM.Settings.AVGTime * 1000);
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  当前没有计划工单 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                    }
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopAddEnabled = true;

                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.Message;
                    Log.Debug(ex.Message);
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopAddEnabled = true;
                }
            });
        };
        #endregion
        #region 出料
        TotalVM.PLC_All[0].TopOutAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"Out_{AuthenticatorVM.Settings.OutCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode; //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopNGOutEnabled = false;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcodeEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopOutEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.ToString();
                    Log.Debug(ex.Message);
                }
            });
        };
        TotalVM.PLC_All[1].TopOutAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"Out_{AuthenticatorVM.Settings.OutCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode; //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopNGOutEnabled = false;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcodeEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopOutEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.ToString();
                    Log.Debug(ex.Message);
                }
            });
        };
        #endregion
        #region NG出料
        TotalVM.PLC_All[0].TopNGOutAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"NGOut_{AuthenticatorVM.Settings.NGCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode; //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopOutEnabled = false;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopNGOutEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        TotalVM.PLC_All[1].TopNGOutAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"NGOut_{AuthenticatorVM.Settings.NGCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode; //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopOutEnabled = false;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopNGOutEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        #endregion
        #region 退料
        TotalVM.PLC_All[0].TopRetAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"Ret_{AuthenticatorVM.Settings.CallCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode;  //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                    }
                    else if (ErrorCode is "-1")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopRetEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  {ErrorMsg} {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                    }
                    Log.Debug($"methodInvoke:[{methodInvoke}], berthCode:[{input}]");
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        TotalVM.PLC_All[1].TopRetAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"Ret_{AuthenticatorVM.Settings.CallCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode;  //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                    }
                    else if (ErrorCode is "-1")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopRetEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  {ErrorMsg} {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                    }
                    Log.Debug($"methodInvoke:[{methodInvoke}], berthCode:[{input}]");
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        #endregion
        #region 上傳加工紀錄
        TotalVM.PLC_All[0].TopDataUploadevent += async e =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "DataUpload";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode;  //工單ID
                    string[] sItem = new string[]
                    {
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].TopPartID}" ,
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].TopProcessID}",
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].TopPanelCount}",
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].TopCheckin.ToString("yyyy-MM-dd HH:mm:ss")}" ,
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                        $"{User.Name}"
                    };
                    var input = GetXml(methodInvoke,macCode,sItem);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopOutEnabled = true;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopNGOutEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  加工数据上传失败 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        Thread.Sleep(AuthenticatorVM.Settings.AVGTime * 1000);
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopCheckButtonEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        TotalVM.PLC_All[1].TopDataUploadevent += async e =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "DataUpload";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode;  //工單ID
                    string[] sItem = new string[]
                    {
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].TopPartID}" ,
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].TopProcessID}",
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].TopPanelCount}",
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].TopCheckin.ToString("yyyy-MM-dd HH:mm:ss")}" ,
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                        $"{User.Name}"
                    };
                    var input = GetXml(methodInvoke,macCode,sItem);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopOutEnabled = true;
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopNGOutEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  加工数据上传失败 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                        Thread.Sleep(AuthenticatorVM.Settings.AVGTime * 1000);
                        TotalVM.PLC_All[TotalVM.PLCIndex].TopCheckButtonEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        #endregion
        #region Local
        TotalVM.PLC_All[0].TopLocalIngredientsevent += async () =>
        {
            TotalVM.PLC_All[TotalVM.PLCIndex].TopWorkOrder = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalLot;
            TotalVM.PLC_All[TotalVM.PLCIndex].TopPartID = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalPartID;
            TotalVM.PLC_All[TotalVM.PLCIndex].TopProcessID = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalProcessID;
            TotalVM.PLC_All[TotalVM.PLCIndex].TopPanelCount = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalPanelCount;
            TotalVM.PLC_All[TotalVM.PLCIndex].TopRecipeID = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalRecipe;
        };
        TotalVM.PLC_All[1].TopLocalIngredientsevent += async () =>
        {
            TotalVM.PLC_All[TotalVM.PLCIndex].TopWorkOrder = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalLot;
            TotalVM.PLC_All[TotalVM.PLCIndex].TopPartID = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalPartID;
            TotalVM.PLC_All[TotalVM.PLCIndex].TopProcessID = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalProcessID;
            TotalVM.PLC_All[TotalVM.PLCIndex].TopPanelCount = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalPanelCount;
            TotalVM.PLC_All[TotalVM.PLCIndex].TopRecipeID = TotalVM.PLC_All[TotalVM.PLCIndex].TopLocalRecipe;
        };
        #endregion
        #region 掃碼工單號
        TotalVM.PLC_All[0].TopIngredientsevent += async () =>
        {
            var methodInvoke = "Ingredients";
            var macCode = AuthenticatorVM.Settings.EquipmentID;
            var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode;
            if (wipEntity == null)
            {
                TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  扫码不能为空 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                return;
            }
            var input = GetXml(methodInvoke,macCode,wipEntity);
            var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
            {
                methodInvoke = methodInvoke,
                input = input
            });
            var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
            var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
            var ResultData = Result.macIntfResult.resultDatak__BackingField;
            Log.Debug($"Request : methodInvoke:[{methodInvoke}], berthCode:[{input}]");
            Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
            ////Test 之後要打開
            //if (ErrorCode is "0")
            //{
            //    GetResultData(ResultData, TotalVM.PLCIndex, "Top");
            //    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  配方下发完成 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
            //    TotalVM.PLC_All[TotalVM.PLCIndex].TopAddEnabled = false;
            //    TotalVM.PLC_All[TotalVM.PLCIndex].TopIngredientsButtonEnabled = false;
            //}
            //else
            //{
            //    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  配方下发失败 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;

            //}
            GetResultData(ResultData, TotalVM.PLCIndex, "Top");
            TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"單純測試用";
            TotalVM.PLC_All[TotalVM.PLCIndex].TopIngredientsButtonEnabled = false;
        };
        TotalVM.PLC_All[1].TopIngredientsevent += async () =>
        {
            var methodInvoke = "Ingredients";
            var macCode = AuthenticatorVM.Settings.EquipmentID;
            var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].TopBarcode;
            if (wipEntity == null)
            {
                TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  扫码不能为空 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage;
                return;
            }
            var input = GetXml(methodInvoke,macCode,wipEntity);
            var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
            {
                methodInvoke = methodInvoke,
                input = input
            });
            var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
            var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
            var ResultData = Result.macIntfResult.resultDatak__BackingField;
            Log.Debug($"Request : methodInvoke:[{methodInvoke}], berthCode:[{input}]");
            Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
            ////Test 之後要打開
            //if (ErrorCode is "0")
            //{
            //    GetResultData(ResultData, TotalVM.PLCIndex, "Top");
            //    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  配方下发完成 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.Index].TopMESMessage;
            //    TotalVM.PLC_All[TotalVM.PLCIndex].TopAddEnabled = false;
            //    TotalVM.PLC_All[TotalVM.PLCIndex].TopIngredientsButtonEnabled = false;
            //}
            //else
            //{
            //    TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  配方下发失败 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.Index].TopMESMessage;

            //}
            GetResultData(ResultData, TotalVM.PLCIndex, "Top");
            TotalVM.PLC_All[TotalVM.PLCIndex].TopMESMessage = $"單純測試用";
            TotalVM.PLC_All[TotalVM.PLCIndex].TopIngredientsButtonEnabled = false;
        };

        #endregion
        #endregion
        #region ---------Bottom---------
        #region 叫料
        TotalVM.PLC_All[0].AddAGVevent += async () =>
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var methodInvoke = "CallAgv";
                        var macCode = AuthenticatorVM.Settings.EquipmentID;
                        var berthCode = $"In_{AuthenticatorVM.Settings.CallCarrierID}" ;
                        var wipEntity = "";
                        var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                        var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                        {
                            methodInvoke = methodInvoke,
                            input = input
                        });
                        var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                        var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                        var ResultData = Result.macIntfResult.resultDatak__BackingField;
                        Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                        Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                        if (ErrorCode is "0")
                        {
                            TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                            //開啟作業管控功能
                            TotalVM.PLC_All[TotalVM.PLCIndex].IngredientsButtonEnabled = true;
                            TotalVM.PLC_All[TotalVM.PLCIndex].BarcodeEnabled = true;
                            Thread.Sleep(AuthenticatorVM.Settings.AVGTime * 1000);
                        }
                        else
                        {
                            TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  当前没有计划工单 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        }
                        TotalVM.PLC_All[TotalVM.PLCIndex].AddEnabled = true;
                    }
                    catch (Exception ex)
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.Message;
                        Log.Debug(ex.Message);
                        TotalVM.PLC_All[TotalVM.PLCIndex].AddEnabled = true;
                    }
                });
            };
        TotalVM.PLC_All[1].AddAGVevent += async () =>
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                var methodInvoke = "CallAgv";
                                var macCode = AuthenticatorVM.Settings.EquipmentID;
                                var berthCode = $"In_{AuthenticatorVM.Settings.CallCarrierID}" ;
                                var wipEntity = "";
                                var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                                var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                                {
                                    methodInvoke = methodInvoke,
                                    input = input
                                });
                                var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                                var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                                var ResultData = Result.macIntfResult.resultDatak__BackingField;
                                Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                                Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                                if (ErrorCode is "0")
                                {
                                    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                                    //開啟作業管控功能
                                    TotalVM.PLC_All[TotalVM.PLCIndex].IngredientsButtonEnabled = true;
                                    TotalVM.PLC_All[TotalVM.PLCIndex].BarcodeEnabled = true;
                                    Thread.Sleep(AuthenticatorVM.Settings.AVGTime * 1000);
                                }
                                else
                                {
                                    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  当前没有计划工单 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                                }
                                TotalVM.PLC_All[TotalVM.PLCIndex].AddEnabled = true;
                            }
                            catch (Exception ex)
                            {
                                TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.Message;
                                Log.Debug(ex.Message);
                                TotalVM.PLC_All[TotalVM.PLCIndex].AddEnabled = true;
                            }
                        });
                    };
        #endregion
        #region 出料
        TotalVM.PLC_All[0].OutAGVevent += async () =>
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var methodInvoke = "CallAgv";
                        var macCode = AuthenticatorVM.Settings.EquipmentID;
                        var berthCode = $"Out_{AuthenticatorVM.Settings.OutCarrierID}";
                        var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode; //板件2D??
                        var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                        var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                        {
                            methodInvoke = methodInvoke,
                            input = input
                        });
                        var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                        var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                        var ResultData = Result.macIntfResult.resultDatak__BackingField;
                        Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                        Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                        if (ErrorCode is "0")
                        {
                            TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                            TotalVM.PLC_All[TotalVM.PLCIndex].NGOutEnabled = false;
                            TotalVM.PLC_All[TotalVM.PLCIndex].BarcodeEnabled = true;
                        }
                        else
                        {
                            TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                            TotalVM.PLC_All[TotalVM.PLCIndex].OutEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.ToString();
                        Log.Debug(ex.Message);
                    }
                });
            };
        TotalVM.PLC_All[1].OutAGVevent += async () =>
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var methodInvoke = "CallAgv";
                        var macCode = AuthenticatorVM.Settings.EquipmentID;
                        var berthCode = $"Out_{AuthenticatorVM.Settings.OutCarrierID}";
                        var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode; //板件2D??
                        var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                        var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                        {
                            methodInvoke = methodInvoke,
                            input = input
                        });
                        var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                        var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                        var ResultData = Result.macIntfResult.resultDatak__BackingField;
                        Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                        Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                        if (ErrorCode is "0")
                        {
                            TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                            TotalVM.PLC_All[TotalVM.PLCIndex].NGOutEnabled = false;
                            TotalVM.PLC_All[TotalVM.PLCIndex].BarcodeEnabled = true;
                        }
                        else
                        {
                            TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                            TotalVM.PLC_All[TotalVM.PLCIndex].OutEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.ToString();
                        Log.Debug(ex.Message);
                    }
                });
            };
        #endregion
        #region NG出料
        TotalVM.PLC_All[0].NGOutAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"NGOut_{AuthenticatorVM.Settings.NGCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode; //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].OutEnabled = false;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].NGOutEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        TotalVM.PLC_All[1].NGOutAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"NGOut_{AuthenticatorVM.Settings.NGCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode; //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].OutEnabled = false;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].NGOutEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        #endregion
        #region 退料
        TotalVM.PLC_All[0].RetAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"Ret_{AuthenticatorVM.Settings.CallCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode;  //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                    }
                    else if (ErrorCode is "-1")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].RetEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  {ErrorMsg} {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                    }
                    Log.Debug($"methodInvoke:[{methodInvoke}], berthCode:[{input}]");
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        TotalVM.PLC_All[1].RetAGVevent += async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "CallAgv";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var berthCode = $"Ret_{AuthenticatorVM.Settings.CallCarrierID}";
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode;  //板件2D??
                    var input = GetXml(methodInvoke,macCode,berthCode,wipEntity);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                    }
                    else if (ErrorCode is "-1")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  没有空载位 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].RetEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  {ErrorMsg} {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                    }
                    Log.Debug($"methodInvoke:[{methodInvoke}], berthCode:[{input}]");
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        #endregion
        #region 上傳加工紀錄
        TotalVM.PLC_All[0].DataUploadevent += async e =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "DataUpload";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode;  //工單ID
                    string[] sItem = new string[]
                    {
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].PartID}" ,
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].ProcessID}",
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].PanelCount}",
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].Checkin:yyyy-MM-dd HH:mm:ss}" ,
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                        $"{User.Name}"
                    };
                    var input = GetXml(methodInvoke,macCode,sItem);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].OutEnabled = true;
                        TotalVM.PLC_All[TotalVM.PLCIndex].NGOutEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  加工数据上传失败 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        Thread.Sleep(AuthenticatorVM.Settings.AVGTime * 1000);
                        TotalVM.PLC_All[TotalVM.PLCIndex].CheckButtonEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        TotalVM.PLC_All[1].DataUploadevent += async e =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "DataUpload";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode;  //工單ID
                    string[] sItem = new string[]
                    {
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].PartID}" ,
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].ProcessID}",
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].PanelCount}",
                        $"{TotalVM.PLC_All[TotalVM.PLCIndex].Checkin:yyyy-MM-dd HH:mm:ss}" ,
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                        $"{User.Name}"
                    };
                    var input = GetXml(methodInvoke,macCode,sItem);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                    if (ErrorCode is "0")
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  OK {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        TotalVM.PLC_All[TotalVM.PLCIndex].OutEnabled = true;
                        TotalVM.PLC_All[TotalVM.PLCIndex].NGOutEnabled = true;
                    }
                    else
                    {
                        TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  加工数据上传失败 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                        Thread.Sleep(AuthenticatorVM.Settings.AVGTime * 1000);
                        TotalVM.PLC_All[TotalVM.PLCIndex].CheckButtonEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = ex.Message;
                    Log.Debug(ex.Message);
                }
            });
        };
        #endregion
        #region Local
        TotalVM.PLC_All[0].LocalIngredientsevent += async () =>
        {
            TotalVM.PLC_All[TotalVM.PLCIndex].WorkOrder = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalLot;
            TotalVM.PLC_All[TotalVM.PLCIndex].PartID = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalPartID;
            TotalVM.PLC_All[TotalVM.PLCIndex].ProcessID = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalProcessID;
            TotalVM.PLC_All[TotalVM.PLCIndex].PanelCount = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalPanelCount;
            TotalVM.PLC_All[TotalVM.PLCIndex].RecipeID = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalRecipe;
        };
        TotalVM.PLC_All[1].LocalIngredientsevent += async () =>
        {
            TotalVM.PLC_All[TotalVM.PLCIndex].WorkOrder = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalLot;
            TotalVM.PLC_All[TotalVM.PLCIndex].PartID = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalPartID;
            TotalVM.PLC_All[TotalVM.PLCIndex].ProcessID = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalProcessID;
            TotalVM.PLC_All[TotalVM.PLCIndex].PanelCount = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalPanelCount;
            TotalVM.PLC_All[TotalVM.PLCIndex].RecipeID = TotalVM.PLC_All[TotalVM.PLCIndex].BottomLocalRecipe;
        };
        #endregion
        #region 掃碼工單號
        TotalVM.PLC_All[0].Ingredientsevent += async () =>
        {
            var methodInvoke = "Ingredients";
            var macCode = AuthenticatorVM.Settings.EquipmentID;
            var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode;
            if (wipEntity == null)
            {
                TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  扫码不能为空 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                return;
            }
            var input = GetXml(methodInvoke,macCode,wipEntity);
            var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
            {
                methodInvoke = methodInvoke,
                input = input
            });
            var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
            var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
            var ResultData = Result.macIntfResult.resultDatak__BackingField;
            Log.Debug($"Request : methodInvoke:[{methodInvoke}], berthCode:[{input}]");
            Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
            ////Test 之後要打開
            //if (ErrorCode is "0")
            //{
            //    GetResultData(ResultData, TotalVM.Index, "Bottom");
            //    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  配方下发完成 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
            //    TotalVM.PLC_All[TotalVM.PLCIndex].AddEnabled = false;
            //    TotalVM.PLC_All[TotalVM.PLCIndex].IngredientsButtonEnabled = false;
            //}
            //else
            //{
            //    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  配方下发失败 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
            //}
            GetResultData(ResultData, TotalVM.Index, "Bottom");
            TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"單純測試用";
            TotalVM.PLC_All[TotalVM.PLCIndex].IngredientsButtonEnabled = false;
        };
        TotalVM.PLC_All[1].Ingredientsevent += async () =>
        {
            var methodInvoke = "Ingredients";
            var macCode = AuthenticatorVM.Settings.EquipmentID;
            var wipEntity = TotalVM.PLC_All[TotalVM.PLCIndex].Barcode;
            if (wipEntity == null)
            {
                TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  扫码不能为空 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
                return;
            }
            var input = GetXml(methodInvoke,macCode,wipEntity);
            var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
            {
                methodInvoke = methodInvoke,
                input = input
            });
            var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
            var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
            var ResultData = Result.macIntfResult.resultDatak__BackingField;
            Log.Debug($"Request : methodInvoke:[{methodInvoke}], berthCode:[{input}]");
            Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
            //// Test 之後要打開
            //if (ErrorCode is "0")
            //{
            //    GetResultData(ResultData, TotalVM.Index, "Bottom");
            //    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  配方下发完成 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
            //    TotalVM.PLC_All[TotalVM.PLCIndex].AddEnabled = false;
            //    TotalVM.PLC_All[TotalVM.PLCIndex].IngredientsButtonEnabled = false;
            //}
            //else
            //{
            //    TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:FFF}  [{methodInvoke}]  配方下发失败 {Environment.NewLine}" + TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage;
            //}
            GetResultData(ResultData, TotalVM.Index, "Bottom");
            TotalVM.PLC_All[TotalVM.PLCIndex].MESMessage = $"單純測試用";
            TotalVM.PLC_All[TotalVM.PLCIndex].IngredientsButtonEnabled = false;
        };
        #endregion
        #endregion
        #region 智能單元狀態
        TotalVM.ChangeStatusevent += async e =>
        {
            await Task.Run(() =>
            {
                try
                {
                    var methodInvoke = "DataUpload";
                    var macCode = AuthenticatorVM.Settings.EquipmentID;
                    //var wipEntity = TotalVM.PLC_All[i].Barcode;  //工單ID
                    var input = GetXml(methodInvoke,macCode,e);
                    var Result = Web.macIntf(new SCC_ServerSideRef.macIntfRequest()
                    {
                        methodInvoke = methodInvoke,
                        input = input
                    });
                    var ErrorCode = Result.macIntfResult.errorCodek__BackingField;
                    var ErrorMsg = Result.macIntfResult.errorMsgk__BackingField;
                    var ResultData = Result.macIntfResult.resultDatak__BackingField;
                    Log.Debug($"Request : methodInvoke:[{methodInvoke}], input:[{input}]");
                    Log.Debug($"Resqponse : ErrorCode:[{ErrorCode}], ErrorMsg:[{ErrorMsg}] , ResultData:[{ResultData}]");
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.Message);
                }
            });
        };
        #endregion

        #endregion

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

        //切換上下爐
        TotalVM.OvenTopBottomChangeevent += e =>
        {
            if (e is 0)
            {
                DetailTopVisibility = EditTopVisibility = Visibility.Visible;
                DetailBottomVisibility = EditBottomVisibility = Visibility.Hidden;
            }
            else if (e is 1)
            {
                DetailTopVisibility = EditTopVisibility = Visibility.Hidden;
                DetailBottomVisibility = EditBottomVisibility = Visibility.Visible;
            }
        };

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

        GPServiceHostFunc();
    }

    private string GetXml(string sTitle, string sMacCode, string sBerthCode, string sWipEntity)
    {
        //入料出料退料 CallAgv
        /*
          <? xml version = "1.0" encoding = "UTF-8" ?>
          < CallAgv
          xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance"
          xmlns:xsd = "http://www.w3.org/2001/XMLSchema"
          macCode = "{macCode}"
          berthCode = "{berthCode}"
          wipEntity = "{wipEntity}" >
          </ CallAgv >
        */
        var doc = new XmlDocument();
        var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.AppendChild(declaration);

        var Title = doc.CreateElement(sTitle);
        Title.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        Title.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
        Title.SetAttribute("macCode", sMacCode);
        Title.SetAttribute("berthCode", sBerthCode);
        Title.SetAttribute("wipEntity", sWipEntity);
        doc.AppendChild(Title);

        return doc.InnerXml.ToString();
    }

    private string GetXml(string sTitle, string sMacCode, string sWipEntity)
    {
        //配方下達 Ingredients
        /*    
        <?xml version="1.0" encoding="UTF-8"?>
            <Ingredients
            xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance"
            xmlns:xsd = "http://www.w3.org/2001/XMLSchema"
            macCode = "{macCode}"
            wipEntity = "{wipEntity}">                              
            </Ingredients>
        */
        var doc = new XmlDocument();
        var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.AppendChild(declaration);

        var Title = doc.CreateElement(sTitle);
        Title.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        Title.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
        Title.SetAttribute("macCode", sMacCode);
        Title.SetAttribute("wipEntity", sWipEntity);
        doc.AppendChild(Title);

        return doc.InnerXml.ToString();
    }

    private string GetXml(string sTitle, string sMacCode, string[] sItem)
    {
        //資料上傳 DataUpload
        /*
        <?xml version="1.0" encoding="UTF-8"?>
        <DataUpload
           xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance"
           xmlns:xsd = "http://www.w3.org/2001/XMLSchema"
            macCode = "{macCode}">
           <item tagCode="{macCode}_1001" tagValue="{TotalVM.PLC_All[TotalVM.PLCIndex].TopPartID}" timeStamp="" />
           <item tagCode="{macCode}_1002" tagValue="{TotalVM.PLC_All[TotalVM.PLCIndex].TopProcessID}" timeStamp="" />
           <item tagCode="{macCode}_1003" tagValue="{TotalVM.PLC_All[TotalVM.PLCIndex].TopPanelCount}" timeStamp="" />
           <item tagCode="{macCode}_1004" tagValue="{TotalVM.PLC_All[TotalVM.PLCIndex].TopCheckin}" timeStamp="" />
           <item tagCode="{macCode}_1005" tagValue="{DateTime.Now}" timeStamp="" />
           <item tagCode="{macCode}_1006" tagValue="{User.Name}" timeStamp="" />
        </DataUpload> 
         */
        var doc = new XmlDocument();
        var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.AppendChild(declaration);

        var Title = doc.CreateElement(sTitle);
        Title.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        Title.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
        Title.SetAttribute("macCode", sMacCode);
        doc.AppendChild(Title);

        for (int i = 0; i < sItem.Length; i++)
        {
            var Item = doc.CreateElement("item");
            Item.SetAttribute("tagCode", sMacCode + "_" + $"1{(i + 1).ToString().PadLeft(3, '0')}");
            Item.SetAttribute("tagValue", sItem[i]);
            Item.SetAttribute("timeStamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Title.AppendChild(Item);
        }

        return doc.InnerXml.ToString();
    }
    private string GetXml(string sTitle, string sMacCode, int i)
    {
        //智能單元狀態 DataUpload
        /*
          <?xml version="1.0" encoding="utf-8"?> 
          <DataUpload   
             xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"   
             xmlns:xsd="http://www.w3.org/2001/XMLSchema"   
             macCode="MAC001" 
             wipEntity=”793058”> 
                  <item tagCode="MAC001_018" tagValue="10" timeStamp="2017-11-09 00:00:00" /> 
          </DataUpload> 
        */
        var doc = new XmlDocument();
        var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.AppendChild(declaration);

        var Title = doc.CreateElement(sTitle);
        Title.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        Title.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
        Title.SetAttribute("macCode", sMacCode);
        doc.AppendChild(Title);

        var Item = doc.CreateElement("item");
        Item.SetAttribute("tagCode", sMacCode + "_018");
        Item.SetAttribute("tagValue", i.ToString());
        Item.SetAttribute("timeStamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        Title.AppendChild(Item);

        return doc.InnerXml.ToString();
    }

    private string GetAlarmXml(string sTitle, string sMacCode, string sAlarmCode, string sAlarmDesc)
    {
        //警報訊息上傳 ALarmUpload
        /*<? xml version = "1.0" encoding = "UTF-8" ?>
         <AlarmUpload
           xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance"
           xmlns:xsd = "http://www.w3.org/2001/XMLSchema"
           macCode="{macCode}"
           alarmCode="{alarmCode}"
           alarmDesc="{alarmDesc}"
           timeStamp="{DateTime.Now:yyyy-MM-dd HH:mm:ss}">
           <item tagCode="{macCode}_3000" tagValue="1" timeStamp="{DateTime.Now:yyyy-MM-dd HH:mm:ss}"/>
          </AlarmUpload> 
        */
        var doc = new XmlDocument();
        var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.AppendChild(declaration);

        var Title = doc.CreateElement(sTitle);
        Title.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        Title.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
        Title.SetAttribute("macCode", sMacCode);
        Title.SetAttribute("alarmCode", sAlarmCode);
        Title.SetAttribute("alarmDesc", sAlarmDesc);
        doc.AppendChild(Title);

        var Item = doc.CreateElement("item");
        Item.SetAttribute("tagCode", sMacCode + "_3000");
        Item.SetAttribute("tagValue", "1");
        Item.SetAttribute("timeStamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        Title.AppendChild(Item);

        return doc.InnerXml.ToString();
    }

    private void HeartbeatRun()
    {
        try
        {
            while (true)
            {
                if (ToConnect && !closed && AuthenticatorVM.Settings.UseHeart)
                {
                    var Msg = Encoding.UTF8.GetBytes(AuthenticatorVM.Settings.EquipmentID+"[E]\n\r");
                    Send_Msg(Msg);
                    Thread.Sleep(AuthenticatorVM.Settings.HeartTime * 1000);
                    UseHeartbeat = UseHeartbeat ? !UseHeartbeat : true;
                    IsHeartbeat = true;
                }
                else
                {
                    UseHeartbeat = IsHeartbeat = false;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            HeartbeatRun();
        }
    }

    public void GPServiceHostFunc()
    {
        try
        {
            var Uri = AuthenticatorVM.Settings.iMESURL;
            webServiceHost = new ServiceHost(typeof(SCC_Service), new Uri(Uri));
            var smb = new ServiceMetadataBehavior
            {
                HttpGetEnabled   = true,
                MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 }
            };
            webServiceHost.Description.Behaviors.Add(smb);
            webServiceHost.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
    }

    private void TcpToConnect()
    {
        var hostIP = AuthenticatorVM.Settings.HeartService;
        var port = AuthenticatorVM.Settings.HeartPort;
        mTcpClient = new TcpClient();
        mTcpClient.ReceiveTimeout = 1000;
        mTcpClient.SendTimeout = 1000;
        mTcpClient.BeginConnect(IPAddress.Parse(hostIP), port, mCallBackMsgFun, null);
    }

    private void ReadMessage()
    {
        var testingByte = new byte[1];
        while (true)
        {
            Thread.Sleep(1000);
            if (!ToConnect && closed)
            {
                UseHeartbeat = false;
                closed = false;
                IsHeartbeat = false;
                TcpToConnect();
            }
            if (mTcpClient.Connected)
            {
                if (mTcpClient.Available > 0)
                {
                    var logstr = string.Format("MsgReceivedStarts at [{0}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    _streamFromServer = mTcpClient.GetStream();
                    var buff = new byte[mTcpClient.ReceiveBufferSize];
                    _streamFromServer.Read(buff, 0, buff.Length);
                }
            }
            try
            {
                if (mTcpClient.Connected && mTcpClient.Client.Poll(0, SelectMode.SelectRead))
                    closed = mTcpClient.Client.Receive(testingByte, SocketFlags.Peek) == 0;
                if (closed)
                {
                    ToConnect = false;
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }
    }

    private void mCallBackMsgFun(IAsyncResult ar)
    {
        try
        {
            ToConnect = ar.AsyncWaitHandle.WaitOne(100, true);
            if (mTcpClient.Connected && ToConnect)
            {
                closed = false;
                ToConnect = true;
                string logstr = string.Format("{0} : {1} ConnectionStatus: {2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Connected", ToConnect.ToString());
            }
            else
            {
                ToConnect = false;
                closed = true;
                string logstr = string.Format("{0} : {1} ConnectionStatus: {2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Disconnected", ToConnect.ToString());
                _streamFromServer?.Close();
            }
        }
        catch (Exception e)
        {
            string logstr = string.Format("ConnectionStatus: {0} : {1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), e.Message.ToString());
        }
    }

    public void Send_Msg(byte[] dataOutStream)
    {
        _streamFromServer = mTcpClient.GetStream();
        _streamFromServer.WriteAsync(dataOutStream, 0, dataOutStream.Length);
        _streamFromServer.Flush();
    }

    public void GetResultData(string Data, int PLCindex, string IsToporBottom)
    {
        //測試用
        Data = $"""
           <?xml version="1.0" encoding="utf-8"?> 
            <Ingredients
                    xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance"
                    xmlns:xsd = "http://www.w3.org/2001/XMLSchema"
                    macCode = "MAC001"
                    wipEntity = "12345678" >
                    <item tagCode = "MAC001_1000" tagValue = "Test4321" timeStamp = "" />
                    <item tagCode = "MAC001_1001" tagValue = "Part1234" timeStamp = "" />
                    <item tagCode = "MAC001_1002" tagValue = "Process1234" timeStamp = "" />
                    <item tagCode = "MAC001_1003" tagValue = "1234" timeStamp = "" />
                    <item tagCode = "MAC001_1004" tagValue = "OMG" timeStamp = "" />
           </Ingredients >         
           """;

        var reader = new StringReader(Data);
        var serializer = new XmlSerializer(typeof(Ingredients));
        var instance  = (Ingredients)serializer.Deserialize(reader);
        if (IsToporBottom == "Top")
        {
            TotalVM.PLC_All[TotalVM.PLCIndex].TopOPID = User.Name;
            TotalVM.PLC_All[TotalVM.PLCIndex].TopCheckin = DateTime.Now;

            foreach (var item in instance.item)
            {
                if (item.tagCode.Contains("1000"))  //工序
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopWorkOrder = item.tagValue;
                }
                else if (item.tagCode.Contains("1001")) //物資編碼
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopPartID = item.tagValue;
                }
                else if (item.tagCode.Contains("1002")) //工單ID
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopProcessID = item.tagValue;
                }
                else if (item.tagCode.Contains("1003")) //數量
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopPanelCount = item.tagValue;
                }
                else if (item.tagCode.Contains("1004")) //配方名稱
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].TopRecipeID = item.tagValue;
                }
            }
        }
        else
        {
            TotalVM.PLC_All[TotalVM.PLCIndex].OPID = User.Name;
            TotalVM.PLC_All[TotalVM.PLCIndex].Checkin = DateTime.Now;

            foreach (var item in instance.item)
            {
                if (item.tagCode.Contains("1000"))
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].WorkOrder = item.tagValue;
                }
                else if (item.tagCode.Contains("1001"))
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].PartID = item.tagValue;
                }
                else if (item.tagCode.Contains("1002"))
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].ProcessID = item.tagValue;
                }
                else if (item.tagCode.Contains("1003"))
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].PanelCount = item.tagValue;
                }
                else if (item.tagCode.Contains("1004"))
                {
                    TotalVM.PLC_All[TotalVM.PLCIndex].RecipeID = item.tagValue;
                }
            }
        }
    }

    #region 產生測試資料
    /// <summary>產生測試資料至資料庫</summary>
    /// <param name="PLC_Count"></param>
    //public void MakeTestData(int PLC_Count)
    //{
    //    var partnum = new[]
    //                  {
    //                      "ooxx",
    //                      "abc",
    //                      "zzz",
    //                      "qoo",
    //                      "boom",
    //                      "xxx",
    //                      "wunmao"
    //                  };

    //    var lotid = new[]
    //                {
    //                    "111",
    //                    "222",
    //                    "333",
    //                    "444",
    //                    "555",
    //                    "666",
    //                    "777"
    //                };

    //    var time = DateTime.Now;

    //    for (var j = 1; j <= DateTime.DaysInMonth(time.Year, time.Month); j++) //! 產生一個月的資料
    //    {
    //        for (var i = 0; i < PLC_Count; i++)
    //        {
    //            var rn = new Random(i + j);
    //            var st = new DateTime(time.Year,
    //                                  time.Month,
    //                                  j,
    //                                  8,
    //                                  i + rn.Next(0, 10),
    //                                  rn.Next(0, 60)); //! 早上8點開始

    //            for (var k = 0; k < 10; k++) //! 每天每烤箱8筆
    //            {
    //                var info = new ProcessInfo
    //                {
    //                    StartTime  = st,
    //                    RackID     = rn.Next(1, 10000).ToString("00000"),
    //                    OperatorID = rn.Next(1, 10).ToString("000"),
    //                    Recipe     = RecipeVM.Recipes == null || RecipeVM.Recipes.Count == 0 ? new PLC_Recipe { RecipeName = "NoName" } : RecipeVM.Recipes[new Random().Next(0, RecipeVM.Recipes.Count)]
    //                };

    //                var ttime = new TimeSpan(0, 0, 1);
    //                var cc    = 0;

    //                for (var m = 0; m < 100; m++) //! 產生100筆溫度資料，間隔1分鐘
    //                {
    //                    if (m % 10 == 0) //! 每10分鐘產生一筆事件
    //                    {
    //                        var ev1 = new LogEvent
    //                        {
    //                            StationNumber = i  + 1,
    //                            AddedTime     = st + ttime,
    //                            Description   = $"{i}{j}{m}",
    //                            TagCode       = $"ooxx{m}",
    //                            Type          = (EventType)new Random(DateTime.Now.Millisecond + m).Next(0, 6),
    //                            Value         = new Random(DateTime.Now.Millisecond            + m + 1).Next(2) > 0
    //                        };

    //                        LogVM.DataCollection.Add(ev1);
    //                        info.EventList.Add(ev1);
    //                    }

    //                    var tempt = 30 * (1 + 5 / (1 + Math.Exp(-0.12 * cc + 3)));
    //                    var vals = new RecordTemperatures
    //                    {
    //                        AddedTime                = st + ttime,
    //                        PV_ThermostatTemperature = Math.Round(tempt,                                          1),
    //                        OvenTemperatures_1       = Math.Round(tempt + rn.Next(-5, 5),                         1),
    //                        OvenTemperatures_2       = Math.Round(tempt + rn.Next(-5, 5),                         1),
    //                        OvenTemperatures_3       = Math.Round(tempt + rn.Next(-5, 5),                         1),
    //                        OvenTemperatures_4       = Math.Round(tempt + rn.Next(-5, 5),                         1),
    //                        OvenTemperatures_5       = Math.Round(tempt + rn.Next(-5, 5),                         1),
    //                        OvenTemperatures_6       = Math.Round(tempt + rn.Next(-5, 5),                         1),
    //                        OvenTemperatures_7       = Math.Round(tempt + rn.Next(-5, 5),                         1),
    //                        OvenTemperatures_8       = Math.Round(tempt + rn.Next(-5, 5),                         1),
    //                        OxygenContent            = Math.Round(new Random(i + j + k + m).NextDouble() * 100.0, 1)
    //                    };

    //                    cc += 1;
    //                    info.RecordTemperatures.Add(vals);

    //                    ttime = ttime.Add(TimeSpan.FromMinutes(1)); //! 間隔1分鐘
    //                }

    //                info.EndTime = info.StartTime + ttime;
    //                info.IsFinished = new Random().NextDouble() > 0.5;
    //                info.TotalRampTime = (info.EndTime - info.StartTime).Minutes;

    //                st = info.EndTime + TimeSpan.FromHours(2);

    //                var n = rn.Next(0, 8) + 1; //! 階層
    //                for (var p = 1; p <= n; p++)
    //                {
    //                    var product = new ProductInfo
    //                    {
    //                        PartID   = partnum[rn.Next(0, partnum.Length)],
    //                        LotID    = lotid[rn.Next(0,   lotid.Length)],
    //                        Layer    = p,
    //                        Quantity = rn.Next(10, 20)
    //                    };

    //                    info.Products.Add(product);
    //                }

    //                info.StationNumber = i + 1;
    //                info.AddedTime = info.EndTime.AddSeconds(10);

    //                TraceVM.DataCollection.Add(info);
    //            }
    //        }
    //    }
    //}

    //public ProcessInfo MakeSingleTest(DateTime st, int stationNumber, int tempcount)
    //{
    //    var partnum = new[]
    //                  {
    //                      "ooxx",
    //                      "abc",
    //                      "zzz",
    //                      "qoo",
    //                      "boom",
    //                      "xxx",
    //                      "wunmao"
    //                  };

    //    var lotid = new[]
    //                {
    //                    "111",
    //                    "222",
    //                    "333",
    //                    "444",
    //                    "555",
    //                    "666",
    //                    "777"
    //                };

    //    var rn = new Random((int)st.Ticks);
    //    var info = new ProcessInfo
    //    {
    //        StartTime  = st,
    //        RackID     = rn.Next(1, 10000).ToString("00000"),
    //        OperatorID = rn.Next(1, 10).ToString("000"),
    //        Recipe     = RecipeVM.Recipes == null || RecipeVM.Recipes.Count == 0 ? new PLC_Recipe { RecipeName = "NoName" } : RecipeVM.Recipes[new Random().Next(0, RecipeVM.Recipes.Count)]
    //    };

    //    var ttime = new TimeSpan(0, 0, 1);
    //    var cc    = 0;

    //    for (var m = 0; m < tempcount; m++) //! 產生100筆溫度資料，間隔1分鐘
    //    {
    //        if (m % 10 == 0) //! 每10分鐘產生一筆事件
    //        {
    //            var ev1 = new LogEvent
    //            {
    //                StationNumber = stationNumber,
    //                AddedTime     = st + ttime,
    //                Description   = $"{stationNumber}-{m}",
    //                TagCode       = $"ooxx{m}",
    //                Type          = (EventType)new Random(DateTime.Now.Millisecond + m).Next(0, 6),
    //                Value         = new Random(DateTime.Now.Millisecond            + m + 1).Next(2) > 0
    //            };

    //            LogVM.DataCollection.Add(ev1);
    //            info.EventList.Add(ev1);
    //        }

    //        var tempt = 30 * (1 + 5 / (1 + Math.Exp(-0.12 * cc + 3)));
    //        var vals = new RecordTemperatures
    //        {
    //            AddedTime                = st + ttime,
    //            PV_ThermostatTemperature = Math.Round(tempt,                   1),
    //            OvenTemperatures_1       = Math.Round(tempt + rn.Next(-5, 5),  1),
    //            OvenTemperatures_2       = Math.Round(tempt + rn.Next(-5, 5),  1),
    //            OvenTemperatures_3       = Math.Round(tempt + rn.Next(-5, 5),  1),
    //            OvenTemperatures_4       = Math.Round(tempt + rn.Next(-5, 5),  1),
    //            OvenTemperatures_5       = Math.Round(tempt + rn.Next(-5, 5),  1),
    //            OvenTemperatures_6       = Math.Round(tempt + rn.Next(-5, 5),  1),
    //            OvenTemperatures_7       = Math.Round(tempt + rn.Next(-5, 5),  1),
    //            OvenTemperatures_8       = Math.Round(tempt + rn.Next(-5, 5),  1),
    //            OxygenContent            = Math.Round(rn.NextDouble() * 100.0, 1)
    //        };

    //        cc += 1;
    //        info.RecordTemperatures.Add(vals);

    //        ttime = ttime.Add(TimeSpan.FromSeconds(1)); //! 間隔1秒
    //    }

    //    info.EndTime = info.StartTime + ttime;
    //    info.IsFinished = new Random().NextDouble() > 0.5;
    //    info.TotalRampTime = (info.EndTime - info.StartTime).Minutes;

    //    var n = rn.Next(0, 8) + 1; //! 階層
    //    for (var p = 1; p <= n; p++)
    //    {
    //        var product = new ProductInfo
    //        {
    //            PartID   = partnum[rn.Next(0, partnum.Length)],
    //            LotID    = lotid[rn.Next(0,   lotid.Length)],
    //            Layer    = p,
    //            Quantity = rn.Next(10, 20)
    //        };

    //        info.Products.Add(product);
    //    }

    //    info.StationNumber = stationNumber;
    //    info.AddedTime = info.EndTime.AddSeconds(10);

    //    return info;
    //}
    #endregion
}