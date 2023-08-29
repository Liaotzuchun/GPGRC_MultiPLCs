using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GPGO_MultiPLCs.Models;
//using GPMVVM.Core.Models.SECS;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PooledCollections;
using PLCService;
#pragma warning disable VSTHRD101

namespace GPGO_MultiPLCs.ViewModels;

/// <summary>所有烤箱的生產總覽</summary>
public sealed class TotalView_ViewModel : ObservableObject
{
    public event Action<(int StationIndex, EventType type, DateTime time, string note, string tag, object value)>? EventHappened;
    public event Func<(int StationIndex, ProcessInfo Info), Task<int>>?                                            AddRecordToDB;
    public event Func<PLC_Recipe, bool>?                                                                           UpsertRecipe;
    public event Func<string, bool>                                                                                CheckUser;
    public event Func<string, bool>?                                                                               DeleteRecipe;
    public event Func<string, PLC_Recipe?>?                                                                        GetRecipe;

    /// <summary>財產編號儲存位置</summary>
    private const string AssetNumbersPath = "AssetNumbers";

    /// <summary>設備碼儲存位置</summary>
    private const string MachineCodesPath = "MachineCodes";
    private readonly IDialogService? Dialog;

    /// <summary>保持PLC Gate連線</summary>
    private readonly Timer Checker;
    private readonly AsyncOperation? asyncOperation;
    private readonly int             threadid;

    public Language Language = Language.TW;
    public IGate Gate { get; }
    public ObservableConcurrentQueue<LogEvent> QueueMessages { get; } = new();

    /// <summary>回到總覽頁</summary>
    public RelayCommand BackCommand { get; }

    public RelayCommand GoDetailCommand { get; }

    public RelayCommand LoadedCommand { get; }

    public AsyncCommand SendTerminalMessageCommand { get; }

    public RelayCommand SecsReStartCommand { get; }

    public RelayCommand AddA { get; }
    public RelayCommand AddB { get; }
    public RelayCommand OutAGV { get; }
    public RelayCommand RetAGV { get; }
    public RelayCommand TaskControl { get; }
    public RelayCommand DataUpload { get; }
    public RelayCommand Ingredients { get; }

    /// <summary>所有PLC</summary>
    public IList<PLC_ViewModel> PLC_All { get; }

    public IList<PLC_ViewModel> PLC_All_View => OvenCount > PLC_All.Count ? PLC_All : PLC_All.Take(OvenCount).ToList();

    /// <summary>檢視詳細資訊的PLC</summary>
    public PLC_ViewModel PLC_In_Focused => PLCIndex > -1 ? PLC_All[PLCIndex] : PLC_All[0];

    public int OvenCount
    {
        get => Get<int>();
        set
        {
            if (Get<int>() == value)
            {
                return;
            }

            Set(value);
            NotifyPropertyChanged(nameof(PLC_All_View));

            Gate.GateStatus.CurrentValue = false; //! 重新連線並發送通訊列表
        }
    }

    /// <summary>烤箱總覽和詳細資訊檢視頁面切換index</summary>
    public int Index
    {
        get => Get<int>();
        set => Set(value);
    }

    /// <summary>選擇PLC，並切換至該PLC詳細資訊檢視</summary>
    public int PLCIndex
    {
        get => Get<int>();
        set
        {
            Set(value);
            NotifyPropertyChanged(nameof(PLC_In_Focused));
        }
    }

    public int Mode
    {
        get => Get<int>();
        set => Set(value);
    }

    public TotalView_ViewModel(int count, IGate gate, IPAddress plcaddress, IDialogService dialog)
    {
        asyncOperation = AsyncOperationManager.CreateOperation(null);
        Gate = gate;
        Dialog = dialog;
        OvenCount = count;
        PLC_All = new PLC_ViewModel[count];
        PLCIndex = 0;
        Mode = 0;
        var v = Assembly.GetExecutingAssembly().GetName().Version;
        threadid = Thread.CurrentThread.ManagedThreadId;
        BackCommand = new RelayCommand(index => Index = index != null && int.TryParse(index.ToString(), out var i) ? i : 0);

        GoDetailCommand = new RelayCommand(_ => Index = 1);

        LoadedCommand = new RelayCommand(e =>
                                         {
                                             if (e is FrameworkElement el)
                                             {
                                                 foreach (var plc in PLC_All)
                                                 {
                                                     plc.OvenInfo.ChartModel.SetFrameworkElement(el);
                                                 }
                                             }
                                         });

        SendTerminalMessageCommand = new AsyncCommand(async _ =>
                                                      {
                                                          var (result1, input1) = await dialog.ShowWithInput(new Dictionary<Language, string>
                                                                                                             {
                                                                                                                 { Language.TW, "請輸入欲發送之訊息：" },
                                                                                                                 { Language.CHS, "请输入欲发送之讯息：" },
                                                                                                                 { Language.EN, "Please enter the message you want to send：" }
                                                                                                             },
                                                                                                             new Dictionary<Language, string>
                                                                                                             {
                                                                                                                 { Language.TW, "終端訊息" },
                                                                                                                 { Language.CHS, "终端讯息" },
                                                                                                                 { Language.EN, "Terminal Message" }
                                                                                                             },
                                                                                                             true);

                                                          if (result1 && input1 is string msg)
                                                          {
                                                              //SecsGemEquipment.SendTerminalMessage(msg);
                                                          }
                                                      },
                                                      null);

        SecsReStartCommand = new RelayCommand(_ =>
                                              {
                                                  asyncOperation.Post(_ =>
                                                                      {
                                                                          var tid = Thread.CurrentThread.ManagedThreadId;
                                                                          if (threadid == tid)
                                                                          {
                                                                              //SecsGemEquipment.ReStartSecsgem();
                                                                              //SecsGemEquipment.Enable(true);
                                                                              //SecsGemEquipment.Online(true);
                                                                          }
                                                                      },
                                                                      null);
                                              });

        AddA = new RelayCommand(_ =>
        {
            MessageBox.Show("AddA");

        });

        AddB = new RelayCommand(_ =>
        {
            MessageBox.Show("AddB");

        });

        OutAGV = new RelayCommand(_ =>
        {
            MessageBox.Show("OutAGV");

        });

        RetAGV = new RelayCommand(_ =>
        {
            MessageBox.Show("RetAGV");

        });

        TaskControl = new RelayCommand(_ =>
        {
            MessageBox.Show("TaskControl");
        });

        DataUpload = new RelayCommand(_ =>
        {
            MessageBox.Show("DataUpload");
        });

        Ingredients = new RelayCommand(_ =>
        {
            MessageBox.Show("Ingredients");
        });

        PropertyChanged += (_, e) =>
               {
                   //if (e.PropertyName is nameof(SECS_ENABLE) or nameof(SECS_Communicating) or nameof(SECS_ONLINE) or nameof(SECS_REMOTE))
                   //{
                   //    //var val  = SECS_ENABLE && SECS_Communicating && SECS_ONLINE;
                   //    //var val2 = val         && SECS_REMOTE;
                   //    foreach (var plc in PLC_All)
                   //    {
                   //        plc.SecsIsOnline       = val;
                   //        plc.SecsIsRemoteOnline = val2;
                   //    }
                   //}
               };
        //var address = plcaddress.GetAddressBytes();

        //! 註冊PLC事件需引發的動作
        for (var i = 0; i < count; i++)
        {
            var plc = new PLC_ViewModel(dialog,
                                        Gate,
                                        //BitConverter.ToInt32(new[] { address[0], address[1], address[2], (byte)(address[3] + i) }, 0),
                                        i,
                                        "GOL",
                                        (bits_shift: new Dictionary<BitType, int>
                                                     {
                                                         { BitType.B, 0 },
                                                         { BitType.M, 0 },
                                                         { BitType.S, 0 },
                                                         { BitType.X, 0 },
                                                         { BitType.Y, 0 }
                                                     },
                                         datas_shift: new Dictionary<DataType, int>
                                                      {
                                                          { DataType.D, 0 },
                                                          { DataType.W, 0 }
                                                      })); //! 可指定PLC點位位移

            plc.OvenInfo.OvenCode = $"Oven{i + 1}";

            PLC_All[i] = plc;
            var index = i;

            plc.WantDetail += () =>
                              {
                                  PLCIndex = index;
                                  Index = 1;
                              };

            plc.CheckUser += op => CheckUser != null && CheckUser.Invoke(op);

            plc.CheckIn += e =>
                           {
                               var (opid, rackid) = e;
                           };

            //! 取消投產
            plc.CancelCheckIn += _ =>
                                 {
                                 };

            plc.CheckOut += _ =>
            {

            };

            plc.LotAdded += lotid =>
                            {

                            };

            plc.LotRemoved += lotid =>
                              {
                              };

            //! PLC讀取配方內容時
            plc.GetRecipe += recipeName => string.IsNullOrEmpty(recipeName) ? null : GetRecipe?.Invoke(recipeName);

            plc.ExecutingStarted += () =>
                                    {
                                        PLCIndex = index;
                                        Index = 1;
                                    };

            //! 烘烤流程結束時
            plc.ExecutingFinished += async baseInfo =>
                                     {
                                         var product = new ProcessInfo(baseInfo);

                                         //! 更新ProcessData以供上報
                                         try
                                         {
                                             // SecsGemEquipment.UpdateDV($"Oven{index + 1}_ProcessData", JsonConvert.SerializeObject(baseInfo));
                                         }
                                         catch
                                         {
                                             // ignored
                                         }

                                         if (baseInfo.IsFinished)
                                         {
                                             //SecsGemEquipment.InvokeEvent($"Oven{index + 1}_ProcessComplete");
                                             dialog.Show(new Dictionary<Language, string>
                                                         {
                                                             { Language.TW, "已完成烘烤！" },
                                                             { Language.CHS, "已完成烘烤！" },
                                                             { Language.EN, "Finished!" }
                                                         });
                                         }
                                         else
                                         {
                                             //SecsGemEquipment.InvokeEvent($"Oven{index + 1}_ProcessAborted");
                                             dialog.Show(new Dictionary<Language, string>
                                                         {
                                                             { Language.TW, "已取消烘烤！" },
                                                             { Language.CHS, "已取消烘烤！" },
                                                             { Language.EN, "Canceled!" }
                                                         });
                                         }

                                         if (AddRecordToDB != null)
                                         {
                                             await AddRecordToDB.Invoke((index, product));
                                         }

                                         Index = 0; //! 烘烤完成，切回投產頁面
                                     };

            //! 由OP變更設備代碼時
            plc.MachineCodeChanged += _ => SaveMachineCodes(MachineCodesPath);

            //! 由OP變更財產編號時
            plc.AssetNumberChanged += _ => SaveAssetNumbers(AssetNumbersPath);

            //! PLC配方輸入錯誤時
            plc.RecipeKeyInError += () =>
                                    {
                                        dialog.Show(new Dictionary<Language, string>
                                                    {
                                                        { Language.TW, "配方輸入錯誤！" },
                                                        { Language.CHS, "配方输入错误！" },
                                                        { Language.EN, "Recipe input error!" }
                                                    },
                                                    TimeSpan.FromSeconds(1),
                                                    DialogMsgType.Alarm);
                                    };

            //! PLC事件紀錄
            plc.EventHappened += e => EventHappened?.Invoke((index, e.type, e.time, e.note, e.tag, e.value));

            //plc.InvokeSECSEvent += EventName => SecsGemEquipment.InvokeEvent($"Oven{index + 1}_{EventName}");

            //plc.InvokeSECSAlarm += (AlarmName, val) => SecsGemEquipment.InvokeAlarm($"Oven{index + 1}_{AlarmName}", val);

            plc.SV_Changed += (name, value) =>
                              {
                                  if (name == nameof(PLC_ViewModel.EquipmentState))
                                  {
                                      //     SecsGemEquipment.UpdateITRISV(ITRI_SV.GEM_PROCESS_STATE, value);
                                  }
                                  else if (name == $"Previous{nameof(PLC_ViewModel.EquipmentState)}")
                                  {
                                      //   SecsGemEquipment.UpdateITRISV(ITRI_SV.GEM_PREVIOUS_PROCESS_STATE, value);
                                  }
                                  else if (name == nameof(PLC_ViewModel.SV_RecipeName))
                                  {
                                      // SecsGemEquipment.UpdateITRISV(ITRI_SV.GEM_PP_EXEC_NAME, value);
                                  }

                                  //SecsGemEquipment.UpdateSV($"Oven{index + 1}_{name}", value);
                              };

            //plc.RecipeChangedbyPLC += recipe =>
            //                          {
            //                              UpsertRecipe?.Invoke(recipe);
            //                          };
        }

        #region PLCGate事件通知
        Gate.GateStatus.ValueChanged += status =>
                                        {
                                            if (!status)
                                            {
                                                EventHappened?.Invoke((-1, EventType.Alarm, DateTime.Now, "PLC Gate Offline!", string.Empty, true));
                                            }
                                        };
        #endregion

        LoadMachineCodes();
        LoadAssetNumbers();

        Checker = new Timer(_ =>
                            {
                                if (Gate.GateStatus.CurrentValue)
                                {
                                    for (var i = 0; i < OvenCount; i++)
                                    {
                                        PLC_All[i].Check = !PLC_All[i].Check;
                                    }
                                }
                                else if (Gate.Connect(new Dictionary<string, string>
                                                      {
                                                          { "port", "5010" },
                                                          { "frame", "MC3E" }
                                                      }))
                                {
                                    for (var i = 0; i < OvenCount; i++)
                                    {
                                        Gate.SetReadListsByDataModels(PLC_All[i]); //! 連線並發送訂閱列表
                                    }
                                }

                                Checker?.Change(150, Timeout.Infinite);
                            },
                            null,
                            Timeout.Infinite,
                            Timeout.Infinite);
    }

    /// <summary>讀取財產編號</summary>
    public void LoadAssetNumbers()
    {
        if (File.Exists(AssetNumbersPath))
        {
            try
            {
                var vals = AssetNumbersPath.ReadFromJsonFile<string[]>();

                if (vals != null)
                {
                    for (var i = 0; i < Math.Min(vals.Length, PLC_All.Count); i++)
                    {
                        PLC_All[i].OvenInfo.AssetNumber = vals[i];
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        //foreach (var plc in PLC_All)
        //{
        //    plc.OvenInfo.AssetNumber = "";
        //}
    }

    /// <summary>讀取設備碼</summary>
    public void LoadMachineCodes()
    {
        if (File.Exists(MachineCodesPath))
        {
            try
            {
                var vals = MachineCodesPath.ReadFromJsonFile<string[]>();

                if (vals != null)
                {
                    for (var i = 0; i < vals.Length; i++)
                    {
                        PLC_All[i].OvenInfo.MachineCode = vals[i];
                    }

                    for (var i = vals.Length; i < PLC_All.Count; i++)
                    {
                        PLC_All[i].OvenInfo.MachineCode = $"Oven{i + 1}";
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        for (var i = 0; i < PLC_All.Count; i++)
        {
            PLC_All[i].OvenInfo.MachineCode = $"Oven{i + 1}";
        }
    }

    /// <summary>儲存財產編號</summary>
    public void SaveAssetNumbers(string path)
    {
        try
        {
            using var AssetNumbers = PLC_All.Select(x => x.OvenInfo.AssetNumber).ToPooledList();
            AssetNumbers.WriteToJsonFile(path);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>儲存設備碼</summary>
    public void SaveMachineCodes(string path)
    {
        try
        {
            using var MachineCodes = PLC_All.Select(x => x.OvenInfo.MachineCode).ToPooledList();
            MachineCodes.WriteToJsonFile(path);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>設定使用的PLC配方(透過配方名)</summary>
    /// <param name="names">配方名列表</param>
    public void SetRecipeNames(ICollection<string> names)
    {
        foreach (var plc in PLC_All)
        {
            if (plc.Recipe_Names == null || plc.Recipe_Names.Count != names.Count || plc.Recipe_Names.Except(names).Any())
            {
                plc.Recipe_Names = names;
            }
        }
    }



    public void InsertMessage(params LogEvent[] evs)
    {
        if (evs.Length == 0)
        {
            return;
        }

        evs.OrderBy(x => x.AddedTime)
           .ForEach(ev =>
                    {
                        if (ev.Type > EventType.StatusChanged)
                        {
                            try
                            {
                                QueueMessages.Enqueue(ev);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    });

        while (QueueMessages.Count > 50)
        {
            QueueMessages.TryDequeue(out _);
        }
    }

    public void InsertMessage(IList<LogEvent> evs)
    {
        if (evs.Count == 0)
        {
            return;
        }

        evs.OrderBy(x => x.AddedTime)
           .ForEach(ev =>
                    {
                        if (ev.Type > EventType.StatusChanged)
                        {
                            try
                            {
                                QueueMessages.Enqueue(ev);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    });

        while (QueueMessages.Count > 50)
        {
            QueueMessages.TryDequeue(out _);
        }
    }

    public void StartPLCGate() => Checker.Change(0, Timeout.Infinite);
}