using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GP_GRC.Models;
using GPGRC_MultiPLCs.Models;
//using GPMVVM.Core.Models.SECS;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PooledCollections;
using PLCService;
#pragma warning disable VSTHRD101

namespace GPGRC_MultiPLCs.ViewModels;

/// <summary>所有烤箱的生產總覽</summary>
public sealed class TotalView_ViewModel : ObservableObject, INotifyPropertyChanged
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
    public RelayCommand TestPanelCommand { get; }
    public RelayCommand InitialPanelCommand { get; }
    public RelayCommand GoDetailCommand { get; }
    public RelayCommand GoMesCommand { get; }
    public RelayCommand LoadedCommand { get; }
    public RelayCommand DataUpload10Command { get; }
    public RelayCommand DataUpload20Command { get; }
    public RelayCommand DataUpload30Command { get; }
    public RelayCommand DataUpload40Command { get; }
    public RelayCommand DataUpload50Command { get; }
    public RelayCommand DataUpload60Command { get; }
    public RelayCommand DataUpload70Command { get; }
    public RelayCommand DataUpload80Command { get; }
    public RelayCommand DataUpload90Command { get; }
    public RelayCommand DataUpload100Command { get; }
    public RelayCommand DataUpload110Command { get; }
    public RelayCommand DataUpload120Command { get; }
    public RelayCommand DataUpload130Command { get; }
    public RelayCommand DataUpload140Command { get; }
    public RelayCommand DataUpload150Command { get; }
    public RelayCommand DataUpload160Command { get; }
    public RelayCommand DataUpload170Command { get; }
    public RelayCommand DataUpload180Command { get; }
    public RelayCommand DataUpload190Command { get; }
    public RelayCommand DataUpload200Command { get; }
    public RelayCommand LocalIngredients { get; }

    public event Func<Task> LocalIngredientsevent;

    public event Action<int> ChangeStatusevent;

    public bool RadioButton90Check
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool RadioButton100Check
    {
        get => Get<bool>();
        set => Set(value);
    }
    public string[] Coater1Panel
    {
        get => Get<string[]>();
        set
        {
            Set(value);
            NotifyPropertyChanged(nameof(Coater1Panel));
        }
    }
    public string[] Coater2Panel
    {
        get => Get<string[]>();
        set
        {
            Set(value);
            NotifyPropertyChanged(nameof(Coater2Panel));
        }
    }
    public string[] Coater3Panel
    {
        get => Get<string[]>();
        set
        {
            Set(value);
            NotifyPropertyChanged(nameof(Coater3Panel));
        }
    }
    public ObservableCollection<CoaterItem> Coater1Items
    {
        get => Get<ObservableCollection<CoaterItem>>();
        set => Set(value);
    }
    public ObservableCollection<CoaterItem> Coater2Items
    {
        get => Get<ObservableCollection<CoaterItem>>();
        set => Set(value);
    }
    public ObservableCollection<CoaterItem> Coater3Items
    {
        get => Get<ObservableCollection<CoaterItem>>();
        set => Set(value);
    }
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

    public int CarrierIndex
    {
        get => Get<int>();
        set => Set(value);
    }
    public int EqpState
    {
        get => Get<int>();
        set => Set(value);
    }
    public int Status
    {
        get => Get<int>();
        set => Set(value);
    }
    #region 本地下配方輸入資料
    public string LocalLot
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string LocalRecipe
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string LocalUser
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string LocalPartID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string LocalPanelCount
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string LocalProcessID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    #endregion
    #region 顯示配方訊息
    public string ShowOPID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string ShowLot
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string ShowPartID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public string ShowProcessID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string ShowPanelCount
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public string ShowRecipeID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public DateTime ShowCheckin
    {
        get => Get<DateTime>();
        set => Set(value);
    }
    #endregion
    public TotalView_ViewModel(int count, IGate gate, IPAddress plcaddress, IDialogService dialog)
    {
        asyncOperation = AsyncOperationManager.CreateOperation(null);
        Gate = gate;
        Dialog = dialog;
        OvenCount = count;
        PLC_All = new PLC_ViewModel[count];
        PLCIndex = 0;
        Mode = 0;
        EqpState = 1;
        InitialStringItem();
        Status = -1;
        var v = Assembly.GetExecutingAssembly().GetName().Version;
        threadid = Thread.CurrentThread.ManagedThreadId;
        BackCommand = new RelayCommand(index => Index = index != null && int.TryParse(index.ToString(), out var i) ? i : 0);

        GoDetailCommand = new RelayCommand(_ => Index = 1);

        GoMesCommand = new RelayCommand(_ => Index = 2);

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
        #region 智能狀態
        DataUpload10Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(10);
        });
        DataUpload120Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(20);
        });
        DataUpload30Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(30);
        });
        DataUpload40Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(40);
        });
        DataUpload50Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(50);
        });
        DataUpload60Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(60);
        });
        DataUpload70Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(70);
        });
        DataUpload80Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(80);
        });
        DataUpload90Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(90);
        });
        DataUpload100Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(100);
        });
        DataUpload110Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(110);
        });
        DataUpload120Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(120);
        });
        DataUpload130Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(130);
        });
        DataUpload140Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(140);
        });
        DataUpload150Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(150);
        });
        DataUpload160Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(160);
        });
        DataUpload170Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(170);
        });
        DataUpload180Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(180);
        });
        DataUpload190Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(190);
        });
        DataUpload200Command = new RelayCommand(_ =>
        {
            ChangeStatusevent?.Invoke(200);
        });
        #endregion
        PropertyChanged += (_, e) =>
               {
               };
        //var address = plcaddress.GetAddressBytes();
        #region 遠端下配方 還在改
        //TopIngredients = new RelayCommand(async _ =>
        //{

        //    if (TopBarcode is null or "")
        //    {
        //        dialog.Show(new Dictionary<Language, string>
        //                                                                    {
        //                                                                        { Language.TW,  "請刷入工單號！" },
        //                                                                        { Language.CHS, "请刷入工单号！" }
        //                                                                    });
        //        return;
        //    }
        //    if (TopPCtoPLC != 1)
        //    {
        //        dialog.Show(new Dictionary<Language, string>
        //                                                                    {
        //                                                                        { Language.TW,  "未在PC連線模式" },
        //                                                                        { Language.CHS, "未在PC連線模式" }
        //    });
        //        return;
        //    }
        //    if (!bChangeStatusevent.Invoke())
        //    {
        //        dialog.Show(new Dictionary<Language, string>
        //                                                                    {
        //                                                                        { Language.TW,  "智能单元状态不在可下方配方状态" },
        //                                                                        { Language.CHS, "智能单元状态不在可下方配方状态" }
        //    });
        //        return;
        //    }
        //    var result = TopTaskControlevent?.Invoke();

        //    if (result == "-1")
        //    {
        //        dialog.Show(new Dictionary<Language, string>
        //                                                                        {
        //                                                                            { Language.TW,  "工单品质暂停" },
        //                                                                            { Language.CHS, "工单品质暂停" }
        //                                                                        });
        //        return;
        //    }
        //    else if (result == "0")
        //    {
        //        await Task.Run(() => TopIngredientsevent?.Invoke());

        //        if (GetRecipe?.Invoke(TopRecipeID) is not { } recipe1)
        //        {
        //            Dialog.Show(new Dictionary<Language, string>
        //                {
        //                    { Language.TW, "配方讀取錯誤" },
        //                    { Language.CHS, "配方读取错误" },
        //                    { Language.EN, "Recipe loaded Fail" }
        //                });
        //            return;
        //        }
        //        if (!await Dialog.Show(new Dictionary<Language, string>
        //                       {
        //                           { Language.TW, "請確認配方內容：" },
        //                           { Language.CHS, "请确认配方内容：" }
        //                       },
        //                               recipe1.ToShowDictionary(),
        //                               true,
        //                               TimeSpan.FromMilliseconds(int.MaxValue),
        //                               DialogMsgType.Alert))
        //            return;
        //        var recipe = TopRecipeID;
        //        var part = TopPartID;
        //        var panelcount = Convert.ToInt32(TopPanelCount);
        //        var lot = TopWorkOrder;
        //        if (!WebRecipetoPLC(recipe, part))
        //            return;
        //    }
        //});

        #endregion
        LocalIngredients = new RelayCommand(async _ =>
        {
            if (LocalLot is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請刷入工單號！" },
                                                                                { Language.CHS, "请刷入工单号！" }
                                                                            });
                return;
            }
            if (LocalRecipe is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請輸入配方！" },
                                                                                { Language.CHS, "请输入配方！" }
                                                                            });
                return;
            }
            if (LocalPartID is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請輸入物资编码！" },
                                                                                { Language.CHS, "请输入物资编码！" }
                                                                            });
                return;
            }
            if (LocalProcessID is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請輸入工序编码！" },
                                                                                { Language.CHS, "请输入工序编码！" }
                                                                            });
                return;
            }
            if (LocalPanelCount is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請輸入計畫加工板數！" },
                                                                                { Language.CHS, "请输入計畫加工板數！" }
                                                                            });
                return;
            }
            if (LocalUser is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請刷入操作人員工號！" },
                                                                                { Language.CHS, "請刷入操作人員工号！" }
                                                                            });
                return;
            }

            try
            {
                var check = Convert.ToInt32(LocalPanelCount);
            }
            catch
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "板數輸入錯誤！" },
                                                                                { Language.CHS, "板数输入错误！" }
                                                                            });
                return;
            }
            //if (TopPCtoPLC == 0)
            //{
            //    dialog.Show(new Dictionary<Language, string>
            //                                                                {
            //                                                                    { Language.TW,  "未在PC連線模式" },
            //                                                                    { Language.CHS, "未在PC連線模式"  }
            //                                                                });
            //    return;
            //}
            if (GetRecipe?.Invoke(LocalRecipe) is not { } recipe1)
            {
                Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, "配方讀取錯誤" },
                            { Language.CHS, "配方读取错误" },
                            { Language.EN, "Recipe loaded Fail" }
                        });

                return;
            }
            var abb = recipe1.ToShowDictionary();
            if (!await Dialog.Show(new Dictionary<Language, string>
                               {
                                   { Language.TW, "請確認配方內容：" },
                                   { Language.CHS, "请确认配方内容：" }
                               },
                                   recipe1.ToShowDictionary(),
                                   true,
                                   TimeSpan.FromMilliseconds(int.MaxValue),
                                   DialogMsgType.Alert))
            {
                return;
            }
            var recipe = LocalRecipe;
            var lot = LocalLot;
            var part = LocalPartID;
            var panelcount = Convert.ToInt32(LocalPanelCount);
            PLC_All[0].WebRecipetoPLC(recipe, lot);
            PLC_All[1].WebRecipetoPLC(recipe, lot);
            PLC_All[2].WebRecipetoPLC(recipe, lot);

            LocalIngredientsevent?.Invoke();
        });

        //! 註冊PLC事件需引發的動作
        for (var i = 0; i < count; i++)
        {
            var plc = new PLC_ViewModel(dialog,
                                        Gate,
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
            //if (i == 0)
            plc.OvenInfo.OvenCode = $"Coater" + (i + 1);
            //else
            //    plc.OvenInfo.OvenCode = $"右炉";

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

            //寫入手臂 PLC[0]
            plc.WriteToRB += e =>
            {
            };

            //! PLC讀取配方內容時
            plc.GetRecipe += recipeName => string.IsNullOrEmpty(recipeName) ? null : GetRecipe?.Invoke(recipeName);

            plc.ExecutingStarted += () =>
                                    {

                                    };


            //! 烘烤流程結束時
            plc.ExecutingFinished += async baseInfo =>
                                     {
                                         var product = new ProcessInfo(baseInfo);

                                         if (baseInfo.TopIsFinished)
                                         {
                                             dialog.Show(new Dictionary<Language, string>
                                                         {
                                                             { Language.TW, $"已完成烘烤！" },
                                                             { Language.CHS, $"已完成烘烤！" },
                                                             { Language.EN, $"Oven No{index + 1}has been finished!" }
                                                         },
                                             TimeSpan.FromSeconds(2));
                                         }
                                         else
                                         {
                                             dialog.Show(new Dictionary<Language, string>
                                                         {
                                                             { Language.TW, $"已完成烘烤！" },
                                                             { Language.CHS, $"已完成烘烤！" },
                                                             { Language.EN, $"Oven No{index + 1}has been finished!" }
                                                         },
                                              TimeSpan.FromSeconds(2));
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

            var panelIndexMap = new Dictionary<string, int>
                                                   {
                                                       { nameof(plc.FeedInlet), 0 },
                                                       { nameof(plc.FeedToWait), 1 },
                                                       { nameof(plc.WaitToFrontWeight), 2 },
                                                       { nameof(plc.FrontWeightToCoater), 3 },
                                                       { nameof(plc.CoaterToBackWeight), 4 },
                                                   };

            plc.PanelMoveHappened += e =>
            {
                try
                {
                    if (e.Item1 == 0)
                    {
                        if (panelIndexMap.ContainsKey(e.Item2))
                        {
                            var Coaterindex = panelIndexMap[e.Item2];

                            if (Coaterindex == 0)
                            {
                                Coater1Panel[0] = plc.PanelID;
                            }
                            else
                            {
                                Coater1Panel[Coaterindex] = Coater1Panel[Coaterindex - 1];
                                Coater1Panel[Coaterindex - 1] = "";
                            }
                        }
                        else if (e.Item2 == nameof(plc.BackWeightToOven) && !string.IsNullOrEmpty(Coater1Panel[4]))
                        {
                            _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                Coater1Items.Add(new CoaterItem { Num = Coater1Items.Count + 1, PanelName = Coater1Panel[4] });
                                Coater1Panel[4] = "";
                            });
                        }
                    }
                    else if (e.Item1 == 1)
                    {
                        if (panelIndexMap.ContainsKey(e.Item2))
                        {
                            var Coaterindex = panelIndexMap[e.Item2];
                            if (Coaterindex == 0)
                            {

                            }
                            else if (Coaterindex == 1)
                            {
                                _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                                    {
                                        Coater2Panel[0] = Coater1Items[0].PanelName;

                                        for (var i = 0; i < Coater1Items.Count - 1; i++)
                                        {
                                            Coater1Items[i] = Coater1Items[i + 1];
                                            Coater1Items[i].Num = i + 1;
                                        }
                                        Coater1Items.RemoveAt(Coater1Items.Count - 1);
                                    });
                            }
                            else
                            {
                                Coater2Panel[Coaterindex - 1] = Coater2Panel[Coaterindex - 2];
                                Coater2Panel[Coaterindex - 2] = "";
                            }
                        }
                        else if (e.Item2 == nameof(plc.BackWeightToOven) && !string.IsNullOrEmpty(Coater2Panel[3]))
                        {
                            _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                Coater2Items.Add(new CoaterItem { Num = Coater2Items.Count + 1, PanelName = Coater2Panel[3] });
                                Coater2Panel[3] = "";
                            });
                        }
                    }
                    else if (e.Item1 == 2)
                    {
                        if (panelIndexMap.ContainsKey(e.Item2))
                        {
                            var Coaterindex = panelIndexMap[e.Item2];
                            if (Coaterindex == 0)
                            {

                            }
                            else if (Coaterindex == 1)
                            {
                                _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                                    {
                                        Coater3Panel[0] = Coater2Items[0].PanelName;

                                        for (var i = 0; i < Coater2Items.Count - 1; i++)
                                        {
                                            Coater2Items[i] = Coater2Items[i + 1];
                                            Coater2Items[i].Num = i + 1;
                                        }
                                        Coater2Items.RemoveAt(Coater2Items.Count - 1);
                                    });
                            }
                            else
                            {
                                Coater3Panel[Coaterindex - 1] = Coater3Panel[Coaterindex - 2];
                                Coater3Panel[Coaterindex - 2] = "";
                            }
                        }
                        else if (e.Item2 == nameof(plc.BackWeightToOven) && !string.IsNullOrEmpty(Coater3Panel[3]))
                        {
                            _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                Coater3Items.Add(new CoaterItem { Num = Coater3Items.Count + 1, PanelName = Coater3Panel[3] });
                                Coater3Panel[3] = "";
                            });
                        }
                    }
                    NotifyPropertyChanged(nameof(Coater1Panel));
                    NotifyPropertyChanged(nameof(Coater2Panel));
                    NotifyPropertyChanged(nameof(Coater3Panel));
                }
                catch
                {

                }
            };
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
                                        PLC_All[i].Check = PLC_All[i].Check == 1 ? (short)0 : (short)1;
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

    private void InitialStringItem()
    {
        Coater1Panel = new string[5];
        Coater2Panel = new string[4];
        Coater3Panel = new string[4];
        Coater1Items = [];
        Coater2Items = [];
        Coater3Items = [];
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

        foreach (var plc in PLC_All)
        {
            plc.OvenInfo.AssetNumber = "";
        }
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
            PLC_All.Select(x => x.OvenInfo.MachineCode).ToArray().WriteToJsonFile(path);
        }
        catch
        {
            // ignored
        }
    }
    public void StartPLCGate() => Checker.Change(0, Timeout.Infinite);

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
}