﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GPGO_MultiPLCs.Models;
using GPMVVM.Core.Helpers;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PooledCollections;
using PLCService;
using Serilog;
#pragma warning disable VSTHRD110

namespace GPGO_MultiPLCs.ViewModels;

public sealed class PLC_ViewModel : GOL_DataModel, IDisposable
{
    public event Action?                                                                         ExecutingStarted;
    public event Action?                                                                         RecipeKeyInError;
    public event Func<bool>?                                                                     CheckRbReady;
    public event Action<(double,string)>?                                                        WriteToRB;
    public event Action?                                                                         WantDetail;
    public event Action<(EventType type, DateTime time, string note, string tag, object value)>? EventHappened;
    public event Action<(string opid, string rackid)>?                                           CheckIn;
    public event Action<string, bool>?                                                           InvokeSECSAlarm;
    public event Action<string, object>?                                                         SV_Changed;
    public event Action<string>?                                                                 AssetNumberChanged;
    public event Action<string>?                                                                 CancelCheckIn;
    public event Action<string>?                                                                 CheckOut;
    public event Action<string>?                                                                 InvokeSECSEvent;
    public event Action<string>?                                                                 LotAdded;
    public event Action<string>?                                                                 LotRemoved;
    public event Action<string>?                                                                 MachineCodeChanged;
    public event Func<BaseInfo, Task>?                                                           ExecutingFinished;
    public event Func<string, bool>?                                                             CheckUser;
    public event Func<string, PLC_Recipe?>?                                                      GetRecipe;

    private readonly CountDownTimer          countDownTimer = new();
    private readonly IDialogService          Dialog;
    private readonly TaskFactory             OneScheduler = new(new StaTaskScheduler(1));
    private          bool                    isCheckin;
    private          bool                    ManualRecord;
    private          CancellationTokenSource CheckRecipeCTS = new();
    private          CancellationTokenSource ppCTS          = new();
    private          CancellationTokenSource BottomppCTS          = new();
    private          DateTime                OfflineTime    = DateTime.MaxValue;
    private          TextBox?                inputFocusTB;

    #region webservice 功能
    #region Top
    public RelayCommand TopAddAGV { get; }
    public RelayCommand TopRetAGV { get; }
    public RelayCommand TopOutAGV { get; }
    public RelayCommand TopNGOutAGV { get; }
    public RelayCommand TopCheckButton { get; }
    public RelayCommand TopTaskControl { get; }
    public RelayCommand TopIngredients { get; }
    public RelayCommand AutoStart { get; }
    public RelayCommand TopLocalIngredients { get; }
    public RelayCommand TopLocalCleanWO { get; }
    public RelayCommand TopCleanWO { get; }
    public RelayCommand TopDataUpload { get; }

    public event Func<Task> TopAddAGVevent;
    public event Func<Task> TopOutAGVevent;
    public event Func<Task> TopNGOutAGVevent;
    public event Func<Task> TopRetAGVevent;
    public event Func<Task> TopLocalIngredientsevent;
    public event Action<int> TopDataUploadevent;
    public event Func<Task> TopIngredientsevent;
    public event Func<Task> TopTaskControlevent;
    public DateTime TopCheckin
    {
        get => Get<DateTime>();
        set => Set(value);
    }
    public DateTime Checkin
    {
        get => Get<DateTime>();
        set => Set(value);
    }
    public bool TopAddEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool TopRetEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool TopOutEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool TopNGOutEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool TopTaskControlButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool TopDataUploadButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool TopIngredientsButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public string TopBarcode
    {
        get => Get<string>();
        set => Set(value);
    }
    public bool TopBarcodeEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool TopCheckButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public string TopOPID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopWorkOrder
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopPartID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopLocalPartID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopLocalProcessID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopProcessID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopPanelCount
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopLocalPanelCount
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopRecipeID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopMESMessage
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopLocalLot
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopLocalRecipe
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string TopLocalUser
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string BottomLocalUser
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string BottomLocalLot
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string BottomLocalPartID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string BottomLocalProcessID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string BottomLocalPanelCount
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string BottomLocalRecipe
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    #endregion
    #region Bottom
    public RelayCommand AddAGV { get; }
    public RelayCommand OutAGV { get; }
    public RelayCommand NGOutAGV { get; }
    public RelayCommand RetAGV { get; }
    public RelayCommand TaskControl { get; }
    public RelayCommand Ingredients { get; }
    public RelayCommand CleanWO { get; }
    public RelayCommand BottomLocalIngredients { get; }
    public RelayCommand BottomLocalCleanWO { get; }
    public RelayCommand CheckButton { get; }
    public RelayCommand DataUpload { get; }
    public event Func<Task> AddAGVevent;
    public event Func<Task> OutAGVevent;
    public event Func<Task> NGOutAGVevent;
    public event Func<Task> RetAGVevent;
    public event Action<int> DataUploadevent;
    public event Func<Task> Ingredientsevent;
    public event Func<Task> LocalIngredientsevent;
    public event Func<Task> TaskControlevent;
    public bool AddEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool RetEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool OutEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool NGOutEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool TaskControlButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool DataUploadButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public bool IngredientsButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public string Barcode
    {
        get => Get<string>();
        set => Set(value);
    }
    public bool BarcodeEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool CheckButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public string OPID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string WorkOrder
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string PartID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string ProcessID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string PanelCount
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string RecipeID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string MESMessage
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    #endregion
    #endregion
    public int InputQuantityMin => 0;
    public int InputQuantityMax => 99999;
    public int InputLayerMin => 1;
    public int InputLayerMax => 8;
    //public event Action<PLC_Recipe> RecipeChangedbyPLC;
    public int RecordDelay { get; set; } = 1;
    public int ClearInputDelay { get; set; } = 60;
    public RelayCommand InputFocusCommand { get; }
    //public AsyncCommand StartCommand { get; }
    //public AsyncCommand StopCommand { get; }
    //public RelayCommand SilinceCommand { get; }
    /// <summary>取消投產</summary>
    public RelayCommand CancelCheckInCommand { get; }
    /// <summary>投產</summary>
    public RelayCommand CheckInCommand { get; }
    public CommandWithResult<bool> CheckInDialogCommand { get; }
    public RelayCommand CheckRecipeCommand_KeyIn { get; }
    public RelayCommand CheckRecipeCommand_KeyLeave { get; }
    public RelayCommand AddLotCommand { get; }
    public RelayCommand DeleteLotCommand { get; }
    public RelayCommand GoDetailCommand { get; }
    public RelayCommand ClearOPTextCommand { get; }
    public RelayCommand ClearPartTextCommand { get; }
    public RelayCommand ClearLotTextCommand { get; }
    public RelayCommand ClearRecipeTextCommand { get; }
    public RelayCommand ClearQuantityCommand { get; }
    public RelayCommand CheckIsExecutingCommand { get; }

    /// <summary>機台資訊</summary>
    public BaseInfoWithChart OvenInfo { get; }

    /// <summary>取得是否正在紀錄溫度</summary>
    public bool IsExecuting => ExecutingTask?.Status is TaskStatus.Running or TaskStatus.WaitingForActivation or TaskStatus.WaitingToRun;
    public bool BottomIsExecuting => BottomExecutingTask?.Status is TaskStatus.Running or TaskStatus.WaitingForActivation or TaskStatus.WaitingToRun;

    /// <summary>生產進度</summary>
    public double Progress
    {
        get
        {
            if (!ConnectionStatus.CurrentValue || !IsExecuting)
            {
                return 0.0;
            }

            var val = 1.0 - RemainTime / TotalTime;
            return double.IsNaN(val) || double.IsInfinity(val) || val <= 0.0 ? 0.0 :
                   val >= 1.0 ? 1.0 : val;
        }
    }

    /// <summary>進度狀態</summary>
    public Status EquipmentStatus => !ConnectionStatus.CurrentValue ?
                                         Status.離線 :
                                         TopEquipmentState switch
                                         {
                                             0 => Status.待命,
                                             1 => Status.運轉中,
                                             2 => Status.停止,
                                             3 => Status.錯誤,
                                             _ => Status.未知
                                         };

    /// <summary>OP輸入的配方名稱</summary>
    public string InputRecipeName
    {
        get => Get<string>();
        set => Set(value);
    }

    public ICollection<string>? Recipe_Names
    {
        get => Get<ICollection<string>>();
        set => Set(value);
    }

    /// <summary>用來紀錄的任務，可追蹤狀態</summary>
    public Task? ExecutingTask
    {
        get => Get<Task>();
        private set => Set(value);
    }
    /// <summary>用來紀錄的任務，可追蹤狀態</summary>
    public Task? BottomExecutingTask
    {
        get => Get<Task>();
        private set => Set(value);
    }

    /// <summary>輸入人員ID</summary>
    public string InputOperatorID
    {
        get => Get<string>();
        set
        {
            value = value.Trim().ToUpper();

            if (value.Length > 12)
            {
                value = value.Substring(0, 12);
            }

            if (CheckUser != null && !CheckUser.Invoke(value))
            {
                Set(string.Empty);
                Dialog.Show(new Dictionary<Language, string>
                            {
                                { Language.TW, "OP權限不符" },
                                { Language.CHS, "OP权限不符" },
                                { Language.EN, "OP permissions error." }
                            },
                            DialogMsgType.Alert);

                InputReFocus();
            }
            else
            {
                Set(value);
                DelayClean();
            }
        }
    }

    /// <summary>輸入料號</summary>
    public string InputPartID
    {
        get => Get<string>();
        set
        {
            value = value.Trim().ToUpper();
            Set(value.Length > 16 ? value.Substring(0, 16) : value);
            DelayClean();
        }
    }

    /// <summary>輸入批號</summary>
    public string InputLotID
    {
        get => Get<string>();
        set
        {
            value = value.Trim().ToUpper();
            //if (value.Length < 10)
            //{
            //    Set(string.Empty);
            //    Dialog.Show(new Dictionary<Language, string>
            //                {
            //                    { Language.TW, "需至少10個字元" },
            //                    { Language.CHS, "需至少10个字符" },
            //                    { Language.EN, "At least 10 chars" }
            //                },
            //                DialogMsgType.Alert);

            //    InputReFocus();
            //}
            //else
            //{
            //    //! 欣興要求批號只取14個字
            //    Set(value.Length > 14 ? value.Substring(0, 14) : value);
            //    DelayClean();
            //}
            Set(value);
        }
    }

    /// <summary>輸入數量</summary>
    public int InputQuantity
    {
        get => Get<int>();
        set
        {
            if (value < InputQuantityMin)
            {
                value = InputQuantityMin;
            }
            else if (value > InputQuantityMax)
            {
                value = InputQuantityMax;
            }

            Set(value);
            DelayClean();
        }
    }

    public int InputLayer
    {
        get => Get<int>();
        set
        {
            if (value < InputLayerMin)
            {
                value = InputLayerMin;
            }
            else if (value > InputLayerMax)
            {
                value = InputLayerMax;
            }

            Set(value);
            DelayClean();
        }
    }

    public bool SecsIsOnline
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SecsIsRemoteOnline
    {
        get => Get<bool>();
        set => Set(value);
    }
    public int EditOvenChange
    {
        get => Get<int>();
        set
        {
            Set(value);
            if (value is 0)
            {
                EditTopVisibility = Visibility.Visible;
                EditBottomVisibility = Visibility.Hidden;
            }
            else if (value is 1)
            {
                EditTopVisibility = Visibility.Hidden;
                EditBottomVisibility = Visibility.Visible;
            }
        }
    }
    public int DetailOvenChange
    {
        get => Get<int>();
        set
        {
            Set(value);
            if (value is 0)
            {
                DetailTopVisibility = Visibility.Visible;
                DetailBottomVisibility = Visibility.Hidden;
            }

            else if (value is 1)
            {
                DetailTopVisibility = Visibility.Hidden;
                DetailBottomVisibility = Visibility.Visible;
            }
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
    public Visibility DetailBottomVisibility
    {
        get => Get<Visibility>();
        set
        {
            Set(value);
            NotifyPropertyChanged();
        }
    }

    public LogEvent SelectedLogEvent
    {
        set => OvenInfo.ChartModel.SetAnnotation(value);
    }
    public Func<object, object> START_Command { get; internal set; }

    public PLC_ViewModel(IDialogService dialog,
                         IGate gate,
                         int plcindex,
                         string plctag,
                         (Dictionary<BitType, int> bits_shift, Dictionary<DataType, int> datas_shift) shift = new()) : base(gate, plcindex, plctag, shift)
    {
        #region WebService功能
        #region Top
        TopAddEnabled = true;
        TopRetEnabled = false;
        TopOutEnabled = false;
        TopNGOutEnabled = false;
        TopCheckButtonEnabled = true;
        TopIngredientsButtonEnabled = false;
        TopAddAGV = new RelayCommand(_ =>
        {
            TopAddEnabled = false;
            TopAddAGVevent?.Invoke();
        });
        TopOutAGV = new RelayCommand(_ =>
        {
            TopOutEnabled = false;
            TopOutAGVevent?.Invoke();
        });
        TopNGOutAGV = new RelayCommand(_ =>
        {
            TopNGOutEnabled = false;
            TopNGOutAGVevent?.Invoke();
        });
        TopRetAGV = new RelayCommand(_ =>
        {
            TopRetEnabled = false;
            TopRetAGVevent?.Invoke();
        });

        TopTaskControl = new RelayCommand(_ =>
        {
            TopTaskControlevent?.Invoke();
        });

        AutoStart = new RelayCommand(async _ =>
        {
            if (AutoStatus)
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW, "烤箱正在烘烤中" },
                                                                                { Language.CHS, "烤箱正在烘烤中" }
                                                                            });
                return;
            }
            var a = await dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW, "請確認是否啟動" },
                                                                                { Language.CHS, "请确认是否啟動" }
                                                                            },
                                                                            true);
            if (!a)
            {
                return;
            }
            Set(true, nameof(AutoStatus));

            Set(true, nameof(TopAutoMode_Start));
        });


        TopIngredients = new RelayCommand(async _ =>
        {

            if (TopBarcode is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請刷入工單號！" },
                                                                                { Language.CHS, "请刷入工单号！" }
                                                                            });
                return;
            }
            if (OvenInfo.TopTempProducts.Count > 0)
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                               { Language.TW, "列表已有刷入工單" },
                                                                                 { Language.CHS, "列表已有刷入工单" }
                                                                            });
                return;
            }
            //bool CheckRB = (bool)(CheckRbReady?.Invoke());
            //if (!CheckRB)
            //    return;

            //if (!RemoteStatus)
            //{
            //    dialog.Show(new Dictionary<Language, string>
            //                                                                {
            //                                                                    { Language.TW,  "請切換為手動模式" },
            //                                                                    { Language.CHS, "請切換為手動模式" }
            //                                                                });
            //    return;
            //}
            //if (AutoStatus)
            //{
            //    dialog.Show(new Dictionary<Language, string>
            //                                                                {
            //                                                                    { Language.TW,  "請切換為手動模式" },
            //                                                                    { Language.CHS, "請切換為手動模式" }
            //                                                                });
            //    return;
            //}

            if (TopAutoMode_Start)
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "正在烘烤中！" },
                                                                                { Language.CHS, "正在烘烤中！" }
                                                                            });
                return;
            }
            //if (TopPCtoPLC == 0)
            //{
            //    dialog.Show(new Dictiona` ry<Language, string>
            //                                                                {
            //                                                                    { Language.TW,  "PC與上爐斷線" },
            //                                                                    { Language.CHS, "PC與上爐斷線" }
            //                                                                });
            //    return;
            //}
            //if (TopStatus != 1)
            //{
            //    dialog.Show(new Dictionary<Language, string>
            //                                                                {
            //                                                                    { Language.TW,  "爐內有框" },
            //                                                                    { Language.CHS, "爐內有框" }
            //                                                                });
            //    return;
            //}
            var a = await dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW, "請確認是否寫入配方" },
                                                                                { Language.CHS, "请确认是否寫入配方" }
                                                                            },
                                                                            true);
            if (!a)
            {
                return;
            }
            TopIngredientsevent?.Invoke();



            var recipe = TopRecipeID;
            var part = TopPartID;
            var panelcount = Convert.ToInt32(TopPanelCount);
            var lot = TopWorkOrder;
            var bCheck = TopWebRecipetoPLC(recipe, part, lot, panelcount);
            if (!bCheck)
            {
                return;
            }

        });

        TopLocalIngredients = new RelayCommand(async _ =>
        {
            if (TopLocalLot is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請刷入工單號！" },
                                                                                { Language.CHS, "请刷入工单号！" }
                                                                            });
                return;
            }
            if (TopLocalRecipe is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請輸入配方！" },
                                                                                { Language.CHS, "请输入配方！" }
                                                                            });
                return;
            }
            if (TopLocalPartID is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請輸入物资编码！" },
                                                                                { Language.CHS, "请输入物资编码！" }
                                                                            });
                return;
            }
            if (TopLocalPanelCount is null or "")
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請輸入計畫加工板數！" },
                                                                                { Language.CHS, "请输入計畫加工板數！" }
                                                                            });
                return;
            }
            if (TopLocalUser is null or "")
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
                var check = Convert.ToInt32(TopLocalPanelCount);
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
            if (OvenInfo.TopTempProducts.Count > 0)
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                               { Language.TW, "列表已有刷入工單" },
                                                                                 { Language.CHS, "列表已有刷入工单" }
                                                                            });
                return;
            }
            if (!RemoteStatus)
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請切換為手動模式" },
                                                                                { Language.CHS, "請切換為手動模式" }
                                                                            });
                return;
            }
            if (AutoStatus)
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "請切換為手動模式" },
                                                                                { Language.CHS, "請切換為手動模式" }
                                                                            });
                return;
            }
            if (TopAutoMode_Start)
            {
                dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW,  "正在烘烤中！" },
                                                                                { Language.CHS, "正在烘烤中！" }
                                                                            });
                return;
            }
            //bool CheckRB = (bool)(CheckRbReady?.Invoke());
            //if (!CheckRB)
            //    return;

            //if (TopPCtoPLC == 0)
            //{
            //    dialog.Show(new Dictionary<Language, string>
            //                                                                {
            //                                                                    { Language.TW,  "PC與上爐斷線" },
            //                                                                    { Language.CHS, "PC與上爐斷線" }
            //                                                                });
            //    return;
            //}
            //if (IsRbRun == 1)
            //{
            //    dialog.Show(new Dictionary<Language, string>
            //                                                                {
            //                                                                    { Language.TW,  "機器人搬運中，請稍後" },
            //                                                                    { Language.CHS, "机器人搬运中，请稍后" }
            //                                                                });
            //    return;
            //}
            //if (TopStatus != 1)
            //{
            //    dialog.Show(new Dictionary<Language, string>
            //                                                                {
            //                                                                    { Language.TW,  "爐內有框" },
            //                                                                    { Language.CHS, "爐內有框" }
            //                                                                });
            //    return;
            //}
            var a = await dialog.Show(new Dictionary<Language, string>
                                                                            {
                                                                                { Language.TW, "請確認是否執行" },
                                                                                { Language.CHS, "请确认是否执行" }
                                                                            },
                                                                            true);
            if (!a)
            {
                return;
            }
            var recipe = TopLocalRecipe;
            var lot = TopLocalLot;
            var part = TopLocalPartID;
            var panelcount = Convert.ToInt32(TopLocalPanelCount);
            var bCheck = TopWebRecipetoPLC(recipe, part, lot, panelcount);
            if (!bCheck)
                return;
            TopLocalIngredientsevent?.Invoke();
        });

        TopCleanWO = new RelayCommand(_ =>
        {
            TopBarcode = "";
            TopAddEnabled = true;
            //OvenInfo.TopTempProducts.Remove(1);
        });

        TopLocalCleanWO = new RelayCommand(_ =>
        {
            TopLocalLot = "";
            TopLocalRecipe = "";
            TopLocalProcessID = "";
            TopLocalPartID = "";
            TopLocalPanelCount = "";
            TopLocalUser = "";
        });

        TopCheckButton = new RelayCommand(_ =>
        {
            TopCheckButtonEnabled = false;
            TopDataUploadevent?.Invoke(0);
        });
        #endregion

        #endregion
        InputLayer = InputLayerMin;
        Dialog = dialog;
        EditOvenChange = 0;
        DetailOvenChange = 0;
        ConnectionStatus.ValueChanged += status =>
                                         {
                                             NotifyPropertyChanged(nameof(EquipmentStatus));
                                             NotifyPropertyChanged(nameof(Progress));

                                             //EventHappened?.Invoke((status ? EventType.StatusChanged : EventType.Alarm, DateTime.Now, "Connection Status", string.Empty, status));
                                             //if (IsExecuting)
                                             //{
                                             //    AddProcessEvent((status ? EventType.StatusChanged : EventType.Alarm, DateTime.Now, "Connection Status", string.Empty, status));
                                             //}

                                             SV_Changed?.Invoke("OnlineStatus", status);
                                             InvokeSECSEvent?.Invoke("OnlineStatusChanged");
                                             OfflineTime = status ? DateTime.MaxValue : DateTime.Now;
                                         };

        OvenInfo = new BaseInfoWithChart();
        OvenInfo.PropertyChanged += (s, e) =>
                                    {
                                        //! 在機台編號或財產編號變更時需通知儲存
                                        if (s is BaseInfo bi)
                                        {
                                            if (e.PropertyName == nameof(BaseInfo.MachineCode))
                                            {
                                                MachineCodeChanged?.Invoke(bi.MachineCode);
                                            }
                                            else if (e.PropertyName == nameof(BaseInfo.AssetNumber))
                                            {
                                                AssetNumberChanged?.Invoke(bi.AssetNumber);
                                            }
                                        }
                                    };


        InputFocusCommand = new RelayCommand(e => inputFocusTB = e as TextBox);

        CheckRecipeCommand_KeyLeave = new RelayCommand(_ =>
                                                       {
                                                       });

        CheckInDialogCommand = new CommandWithResult<bool>(_ => false);

        CheckInCommand = new RelayCommand(_ =>
                                          {
                                              isCheckin = true;

                                              OvenInfo.OperatorID = InputOperatorID;
                                              RackID = OvenInfo.TempProducts.FirstOrDefault()?.LotID ?? string.Empty;
                                              DoorLock = true;
                                              CheckIn?.Invoke((opid: OvenInfo.OperatorID, rackid: RackID));
                                          });

        CheckIsExecutingCommand = new RelayCommand(e =>
                                                   {
                                                       if (e is MouseButtonEventArgs { Source: ToggleButton tb } args)
                                                       {
                                                           //! 避免烘烤中意外中止
                                                           if (IsExecuting && tb.IsChecked == true)
                                                           {
                                                               args.Handled = true;
                                                           }

                                                           //! 避免CheckIn的OP權限不符
                                                           if (tb.IsChecked == false && CheckUser != null && !CheckUser.Invoke(InputOperatorID))
                                                           {
                                                               dialog.Show(new Dictionary<Language, string>
                                                                           {
                                                                               { Language.TW, "OP權限不符" },
                                                                               { Language.CHS, "OP权限不符" },
                                                                               { Language.EN, "OP permissions error." }
                                                                           },
                                                                           DialogMsgType.Alert);

                                                               args.Handled = true;
                                                           }
                                                       }
                                                   });

        GoDetailCommand = new RelayCommand(_ => WantDetail?.Invoke());

        ClearOPTextCommand = new RelayCommand(e =>
                                              {
                                                  Set(string.Empty, nameof(InputOperatorID));
                                                  if (e is TextBox tb)
                                                  {
                                                      Keyboard.Focus(tb);
                                                  }
                                              });

        ClearPartTextCommand = new RelayCommand(e =>
                                                {
                                                    Set(string.Empty, nameof(InputPartID));
                                                    if (e is TextBox tb)
                                                    {
                                                        Keyboard.Focus(tb);
                                                    }
                                                });

        ClearLotTextCommand = new RelayCommand(e =>
                                               {
                                                   Set(string.Empty, nameof(InputLotID));
                                                   if (e is TextBox tb)
                                                   {
                                                       Keyboard.Focus(tb);
                                                   }
                                               });

        ClearRecipeTextCommand = new RelayCommand(e =>
                                                  {
                                                      Set(string.Empty, nameof(InputRecipeName));
                                                      if (e is TextBox tb)
                                                      {
                                                          Keyboard.Focus(tb);
                                                      }
                                                  });

        ClearQuantityCommand = new RelayCommand(e =>
                                                {
                                                    Set(0, nameof(InputQuantity));
                                                    if (e is TextBox tb)
                                                    {
                                                        Keyboard.Focus(tb);
                                                    }
                                                });

        PropertyChanged += (s, e) =>
                           {
                               if (e.PropertyName == nameof(ProgramStop))
                               {
                                   if (ProgramStop)
                                   {
                                       TopAutoMode_Start = false;
                                       _ = StopPP();
                                   }

                                   SV_Changed?.Invoke(nameof(ProgramStop), ProgramStop);
                               }
                               else if (e.PropertyName == nameof(IsRamp))
                               {
                                   ManualRecord = true; //! 狀態變更強制紀錄溫度
                                   SV_Changed?.Invoke(nameof(IsRamp), IsRamp);
                               }
                               else if (e.PropertyName == nameof(IsDwell))
                               {
                                   ManualRecord = true; //! 狀態變更強制紀錄溫度
                                   SV_Changed?.Invoke(nameof(IsDwell), IsDwell);
                               }
                               else if (e.PropertyName == nameof(IsCooling))
                               {
                                   ManualRecord = true; //! 狀態變更強制紀錄溫度
                                   SV_Changed?.Invoke(nameof(IsCooling), IsCooling);
                               }
                               else if (e.PropertyName == nameof(Inflating))
                               {
                                   SV_Changed?.Invoke(nameof(Inflating), Inflating);
                               }
                               else if (e.PropertyName is nameof(RemainTime) or nameof(TotalTime))
                               {
                                   NotifyPropertyChanged(nameof(Progress));
                                   NotifyPropertyChanged(nameof(Progress));
                               }
                           };

        #region 註冊PLC事件
        //! 只有有註冊PLC點位的值變事件
        ValueChanged += (LogType, data) =>
                        {
                            var (name, value, oldvalue, type, Subscriptions, SubPosition) = data;

                            var nowtime = DateTime.Now;

                            if (LogType == LogType.StatusVariables)
                            {
                                var eventval = (EventType.StatusChanged, nowtime, name, $"{(DataType)type!}{Subscriptions!.First()}{(SubPosition > -1 ? $"-{SubPosition:X}" : string.Empty)}", value);

                                SV_Changed?.Invoke(name, value!);

                                if (value is bool val)
                                {
                                    EventHappened?.Invoke(eventval!);
                                    if (IsExecuting)
                                    {
                                        AddProcessEvent(eventval!);
                                    }

                                    if (name == nameof(TopAutoMode_Start))
                                    {
                                        if (!val)
                                        {
                                            return;
                                        }
                                        TopAutoMode_Stop = false;
                                        _ = StartPP();
                                    }
                                    else if (name == nameof(TopProcessComplete))
                                    {
                                        if (!val)
                                        {
                                            return;
                                        }
                                        OvenInfo.TopIsFinished = true;
                                        TopAutoMode_Start = false;
                                        _ = StopPP();
                                    }
                                    else if (name == nameof(TopAutoMode_Stop))
                                    {
                                        if (!val)
                                        {
                                            return;
                                        }
                                        TopAutoMode_Start = false;
                                        _ = StopPP();
                                    }
                                }
                                else if (value is short sv)
                                {
                                    if (name == nameof(TopEquipmentState))
                                    {
                                        SV_Changed?.Invoke($"Previous{name}", oldvalue!);

                                        EventHappened?.Invoke(eventval!);
                                        if (IsExecuting)
                                        {
                                            AddProcessEvent(eventval!);
                                        }
                                        NotifyPropertyChanged(nameof(EquipmentStatus));
                                    }
                                    else if (name == nameof(TopProcessState))
                                    {
                                        EventHappened?.Invoke(eventval!);
                                        if (IsExecuting)
                                        {
                                            AddProcessEvent(eventval!);
                                        }

                                        //SetWithOutNotifyWhenEquals(sv == 0, nameof(ManualMode));
                                        SetWithOutNotifyWhenEquals(sv == 1, nameof(IsRamp));
                                        SetWithOutNotifyWhenEquals(sv == 2, nameof(IsDwell));
                                        SetWithOutNotifyWhenEquals(sv == 7, nameof(IsCooling));
                                        SetWithOutNotifyWhenEquals(sv == 8, nameof(ProgramStop));
                                        //SetWithOutNotifyWhenEquals(sv == 9, nameof(AutoMode));
                                        SetWithOutNotifyWhenEquals(sv == 10, nameof(Inflating));
                                    }
                                }
                            }
                            else if (LogType == LogType.Alert)
                            {
                                var eventval = (EventType.Alert, nowtime, name, $"{(BitType)type!}{Subscriptions!.First()}{(SubPosition > -1 ? $"-{SubPosition:X}" : string.Empty)}", value);
                                EventHappened?.Invoke(eventval!);
                                if (IsExecuting)
                                {
                                    AddProcessEvent(eventval!);
                                }
                                if (IsExecuting)
                                {
                                    AddProcessEvent(eventval!);
                                }

                                if (value is bool boolval)
                                {
                                    InvokeSECSAlarm?.Invoke(name, boolval);
                                }
                            }
                            else if (LogType == LogType.Alarm)
                            {
                                var eventval = (EventType.Alarm, nowtime, name, $"{(BitType)type!}{Subscriptions!.First()}{(SubPosition > -1 ? $"-{SubPosition:X}" : string.Empty)}", value);
                                EventHappened?.Invoke(eventval!);
                                if (IsExecuting)
                                {
                                    AddProcessEvent(eventval!);
                                }

                                if (value is bool boolval)
                                {
                                    InvokeSECSAlarm?.Invoke(name, boolval);
                                }
                            }
                            else if (LogType == LogType.RecipeSet) //PLC配方"設定值"改變時
                            {
                                SV_Changed?.Invoke(name, value!);
                            }
                            else if (LogType == LogType.Trigger)
                            {
                            }
                        };

        OvenInfo.Products.CollectionChanged += (_, _) =>
                                               {
                                                   using var lots   = OvenInfo.Products.Select(x => x.LotID).Distinct().ToPooledList();
                                                   using var parts  = OvenInfo.Products.Select(x => x.PartID).Distinct().ToPooledList();
                                                   var       panels = OvenInfo.Products.Sum(x => x.Quantity);

                                                   SV_Changed?.Invoke("LotIDs", lots.Count > 0 ? string.Join(",", lots) : string.Empty);
                                                   SV_Changed?.Invoke("PartIDs", parts.Count > 0 ? string.Join(",", parts) : string.Empty);
                                                   SV_Changed?.Invoke("PanelIDs", panels);
                                               };
        #endregion 註冊PLC事件
    }

    private async void InputReFocus()
    {
        if (inputFocusTB != null)
        {
            await Task.Delay(60);
            Keyboard.Focus(inputFocusTB);
        }
    }

    private async void DelayClean()
    {
        if (await countDownTimer.WaitAsync(TimeSpan.FromSeconds(ClearInputDelay)))
        {
            inputFocusTB = null;
            Set(string.Empty, nameof(InputOperatorID));
            Set(string.Empty, nameof(InputPartID));
            Set(string.Empty, nameof(InputLotID));
            Set(string.Empty, nameof(InputRecipeName));
            Set(InputQuantityMin, nameof(InputQuantity));
            Set(InputLayerMin, nameof(InputLayer));
        }
    }

    public async Task<SetRecipeResult> WriteRecipeToPlcAsync(PLC_Recipe recipe)
    {
        if (AutoMode)
        {
            AutoMode = false;
            await Task.Delay(900).ConfigureAwait(false);
        }

        if (RecipeCompare(recipe))
        {
            AutoMode = true;
            return SetRecipeResult.無需變更;
        }
        var errs = await ManualSetByPropertiesWithCheck(recipe.ToDictionary()).ConfigureAwait(false);

        var result = errs.Count == 0 ? SetRecipeResult.成功 : SetRecipeResult.比對不相符;
        if (result == SetRecipeResult.成功)
        {
            AutoMode = true;
        }
        return result;
    }
    private bool RecipeCompare(PLC_Recipe recipe) => NitrogenMode == recipe.NitrogenMode &&
                                                     OxygenContentSet.ToString("0.0") == recipe.OxygenContentSet.ToString("0.0") &&
                                                     (RecipeName.Length > 16 ? RecipeName.Substring(0, 16) : RecipeName) == (recipe.RecipeName.Length > 16 ? recipe.RecipeName.Substring(0, 16) : recipe.RecipeName) && //! 只最多比對16個字(PLC的配方名長度)
                                                     DwellTime_1.ToString("0.0") == recipe.DwellTime_1.ToString("0.0") &&
                                                     DwellTime_2.ToString("0.0") == recipe.DwellTime_2.ToString("0.0") &&
                                                     DwellTime_3.ToString("0.0") == recipe.DwellTime_3.ToString("0.0") &&
                                                     DwellTime_4.ToString("0.0") == recipe.DwellTime_4.ToString("0.0") &&
                                                     DwellTime_5.ToString("0.0") == recipe.DwellTime_5.ToString("0.0") &&
                                                     DwellTime_6.ToString("0.0") == recipe.DwellTime_6.ToString("0.0") &&
                                                     DwellAlarm_1.ToString("0.0") == recipe.DwellAlarm_1.ToString("0.0") &&
                                                     DwellAlarm_2.ToString("0.0") == recipe.DwellAlarm_2.ToString("0.0") &&
                                                     DwellAlarm_3.ToString("0.0") == recipe.DwellAlarm_3.ToString("0.0") &&
                                                     DwellAlarm_4.ToString("0.0") == recipe.DwellAlarm_4.ToString("0.0") &&
                                                     DwellAlarm_5.ToString("0.0") == recipe.DwellAlarm_5.ToString("0.0") &&
                                                     DwellAlarm_6.ToString("0.0") == recipe.DwellAlarm_6.ToString("0.0") &&
                                                     CoolingTime.ToString("0.0") == recipe.CoolingTime.ToString("0.0") &&
                                                     CoolingTemperature.ToString("0.0") == recipe.CoolingTemperature.ToString("0.0") &&
                                                     RampTime_1.ToString("0.0") == recipe.RampTime_1.ToString("0.0") &&
                                                     RampTime_2.ToString("0.0") == recipe.RampTime_2.ToString("0.0") &&
                                                     RampTime_3.ToString("0.0") == recipe.RampTime_3.ToString("0.0") &&
                                                     RampTime_4.ToString("0.0") == recipe.RampTime_4.ToString("0.0") &&
                                                     RampTime_5.ToString("0.0") == recipe.RampTime_5.ToString("0.0") &&
                                                     RampTime_6.ToString("0.0") == recipe.RampTime_6.ToString("0.0") &&
                                                     RampAlarm_1.ToString("0.0") == recipe.RampAlarm_1.ToString("0.0") &&
                                                     RampAlarm_2.ToString("0.0") == recipe.RampAlarm_2.ToString("0.0") &&
                                                     RampAlarm_3.ToString("0.0") == recipe.RampAlarm_3.ToString("0.0") &&
                                                     RampAlarm_4.ToString("0.0") == recipe.RampAlarm_4.ToString("0.0") &&
                                                     RampAlarm_5.ToString("0.0") == recipe.RampAlarm_5.ToString("0.0") &&
                                                     RampAlarm_6.ToString("0.0") == recipe.RampAlarm_6.ToString("0.0") &&
                                                     InflatingTime.ToString("0") == recipe.InflatingTime.ToString("0") &&
                                                     TemperatureSetpoint_1.ToString("0.0") == recipe.TemperatureSetpoint_1.ToString("0.0") &&
                                                     TemperatureSetpoint_2.ToString("0.0") == recipe.TemperatureSetpoint_2.ToString("0.0") &&
                                                     TemperatureSetpoint_3.ToString("0.0") == recipe.TemperatureSetpoint_3.ToString("0.0") &&
                                                     TemperatureSetpoint_4.ToString("0.0") == recipe.TemperatureSetpoint_4.ToString("0.0") &&
                                                     TemperatureSetpoint_5.ToString("0.0") == recipe.TemperatureSetpoint_5.ToString("0.0") &&
                                                     TemperatureSetpoint_6.ToString("0.0") == recipe.TemperatureSetpoint_6.ToString("0.0") &&
                                                     SegmentCounts == recipe.SegmentCounts;

    private void AddProcessEvent((EventType type, DateTime addtime, string note, string tag, object value) eventdata)
    {
        if (!IsExecuting)
        {
            return;
        }

        ManualRecord = true;
        var (type, addtime, note, tag, value) = eventdata;
        OvenInfo.EventList.Add(new LogEvent
        {
            Type = type,
            AddedTime = addtime,
            Description = note,
            TagCode = tag,
            Value = value
        });
    }

    /// <summary>重設CancellationTokenSource狀態</summary>
    /// <param name="act">取消動作時執行的委派</param>
    private void ResetStopTokenSource(Action? act = null)
    {
        ppCTS.Dispose();

        ppCTS = new CancellationTokenSource();

        if (act != null)
        {
            ppCTS.Token.Register(act);
        }
    }
    /// <summary>開始記錄</summary>
    /// <param name="ct">取消任務的token</param>
    /// <returns></returns>
    private async Task StartRecoder(CancellationToken ct)
    {
        OvenInfo.Clear();
        CheckRecipeCTS.Cancel();

        foreach (var product in OvenInfo.TempProducts)
        {
            OvenInfo.Products.Add(product);
        }

        void AddTemperatures(bool keypoint,
                             DateTime addtime,
                             double t0,
                             //double t1,
                             //double t2,
                             //double t3,
                             //double t4,
                             //double t5,
                             //double t6,
                             //double t7,
                             //double t8,
                             double oxy)
        {
            var record = new RecordTemperatures
            {
                KeyPoint                 = keypoint,
                AddedTime                = addtime,
                PV_ThermostatTemperature = t0,
                //OvenTemperatures_1       = t1,
                //OvenTemperatures_2       = t2,
                //OvenTemperatures_3       = t3,
                //OvenTemperatures_4       = t4,
                //OvenTemperatures_5       = t5,
                //OvenTemperatures_6       = t6,
                //OvenTemperatures_7       = t7,
                //OvenTemperatures_8       = t8,
                OxygenContent            = oxy
            };

            OvenInfo.RecordTemperatures.Add(record);
            OvenInfo.ChartModel.AddData(record);
        }

        OvenInfo.StartTime = DateTime.Now;
        var nt                     = OvenInfo.StartTime;
        var n                      = TimeSpan.FromSeconds(RecordDelay); //! 每delay週期紀錄一次
        var _ThermostatTemperature = PV_TopThermostatTemperature;
        //var _OvenTemperature_1     = OvenTemperature_1;
        //var _OvenTemperature_2     = OvenTemperature_2;
        //var _OvenTemperature_3     = OvenTemperature_3;
        //var _OvenTemperature_4     = OvenTemperature_4;
        //var _OvenTemperature_5     = OvenTemperature_5;
        //var _OvenTemperature_6     = OvenTemperature_6;
        //var _OvenTemperature_7     = OvenTemperature_7;
        //var _OvenTemperature_8     = OvenTemperature_8;
        var _OxygenContent         = OxygenContent;

        AddTemperatures(true,
                        OvenInfo.StartTime,
                        _ThermostatTemperature,
                        //_OvenTemperature_1,
                        //_OvenTemperature_2,
                        //_OvenTemperature_3,
                        //_OvenTemperature_4,
                        //_OvenTemperature_5,
                        //_OvenTemperature_6,
                        //_OvenTemperature_7,
                        //_OvenTemperature_8,
                        OxygenContent);

        await OneScheduler.StartNew(() =>
                                    {
                                        while (!ct.IsCancellationRequested)
                                        {
                                            if ((DateTime.Now - OfflineTime).TotalSeconds > 30.0)
                                            {
                                                ppCTS.Cancel();
                                                var eventval = (EventType.Alarm, DateTime.Now, "OffLine. The recoding has been aborted.", string.Empty, true);
                                                EventHappened?.Invoke(eventval);
                                                AddProcessEvent(eventval);
                                            }

                                            _ThermostatTemperature = PV_TopThermostatTemperature <= 0 ? _ThermostatTemperature : PV_TopThermostatTemperature;
                                            //_OvenTemperature_1 = OvenTemperature_1 <= 0 ? _OvenTemperature_1 : OvenTemperature_1;
                                            //_OvenTemperature_2 = OvenTemperature_2 <= 0 ? _OvenTemperature_2 : OvenTemperature_2;
                                            //_OvenTemperature_3 = OvenTemperature_3 <= 0 ? _OvenTemperature_3 : OvenTemperature_3;
                                            //_OvenTemperature_4 = OvenTemperature_4 <= 0 ? _OvenTemperature_4 : OvenTemperature_4;
                                            //_OvenTemperature_5 = OvenTemperature_5 <= 0 ? _OvenTemperature_5 : OvenTemperature_5;
                                            //_OvenTemperature_6 = OvenTemperature_6 <= 0 ? _OvenTemperature_6 : OvenTemperature_6;
                                            //_OvenTemperature_7 = OvenTemperature_7 <= 0 ? _OvenTemperature_7 : OvenTemperature_7;
                                            //_OvenTemperature_8 = OvenTemperature_8 <= 0 ? _OvenTemperature_8 : OvenTemperature_8;
                                            _OxygenContent = OxygenContent <= 0 ? _OxygenContent : OxygenContent;

                                            if (DateTime.Now - nt >= n && ConnectionStatus.CurrentValue)
                                            {
                                                nt = DateTime.Now;
                                                AddTemperatures(false,
                                                                nt,
                                                                _ThermostatTemperature,
                                                                //_OvenTemperature_1,
                                                                //_OvenTemperature_2,
                                                                //_OvenTemperature_3,
                                                                //_OvenTemperature_4,
                                                                //_OvenTemperature_5,
                                                                //_OvenTemperature_6,
                                                                //_OvenTemperature_7,
                                                                //_OvenTemperature_8,
                                                                _OxygenContent);
                                            }
                                            else if (ManualRecord)
                                            {
                                                ManualRecord = false;
                                                AddTemperatures(true,
                                                                DateTime.Now,
                                                                _ThermostatTemperature,
                                                                //_OvenTemperature_1,
                                                                //_OvenTemperature_2,
                                                                //_OvenTemperature_3,
                                                                //_OvenTemperature_4,
                                                                //_OvenTemperature_5,
                                                                //_OvenTemperature_6,
                                                                //_OvenTemperature_7,
                                                                //_OvenTemperature_8,
                                                                _OxygenContent);
                                            }
                                            else
                                            {
                                                SpinWait.SpinUntil(() => false, 15);
                                            }
                                        }

                                        _ThermostatTemperature = PV_TopThermostatTemperature <= 0 ? _ThermostatTemperature : PV_TopThermostatTemperature;
                                        //_OvenTemperature_1 = OvenTemperature_1 <= 0 ? _OvenTemperature_1 : OvenTemperature_1;
                                        //_OvenTemperature_2 = OvenTemperature_2 <= 0 ? _OvenTemperature_2 : OvenTemperature_2;
                                        //_OvenTemperature_3 = OvenTemperature_3 <= 0 ? _OvenTemperature_3 : OvenTemperature_3;
                                        //_OvenTemperature_4 = OvenTemperature_4 <= 0 ? _OvenTemperature_4 : OvenTemperature_4;
                                        //_OvenTemperature_5 = OvenTemperature_5 <= 0 ? _OvenTemperature_5 : OvenTemperature_5;
                                        //_OvenTemperature_6 = OvenTemperature_6 <= 0 ? _OvenTemperature_6 : OvenTemperature_6;
                                        //_OvenTemperature_7 = OvenTemperature_7 <= 0 ? _OvenTemperature_7 : OvenTemperature_7;
                                        //_OvenTemperature_8 = OvenTemperature_8 <= 0 ? _OvenTemperature_8 : OvenTemperature_8;
                                        _OxygenContent = OxygenContent <= 0 ? _OxygenContent : OxygenContent;

                                        AddTemperatures(true,
                                                        DateTime.Now,
                                                        _ThermostatTemperature,
                                                        //_OvenTemperature_1,
                                                        //_OvenTemperature_2,
                                                        //_OvenTemperature_3,
                                                        //_OvenTemperature_4,
                                                        //_OvenTemperature_5,
                                                        //_OvenTemperature_6,
                                                        //_OvenTemperature_7,
                                                        //_OvenTemperature_8,
                                                        _OxygenContent);
                                    },
                                    ct);
    }
    public bool TopWebRecipetoPLC(string RecipeName, string part, string lot, int panelcount)
    {
        try
        {
            var recipe = GetRecipe(RecipeName);
            if (recipe is null)
            {
                Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, "找不到配方，請確認是否創建該配方" },
                            { Language.CHS, "找不到配方，请确认是否创建该配方" }
                        });
                return false;
            }
            var a  = SetRecipeAsync(recipe).Result;
            if (a != SetRecipeResult.成功)
            {
                return false;
            }
            TopAddLOT(part, lot, panelcount);

            Set(lot, nameof(TopLotID));
            Set(panelcount, nameof(TopQuantity));
            return true;
        }
        catch (Exception ex)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, ex.Message },
                            { Language.CHS, ex.Message }
                        });
            Log.Debug(ex.Message);
            return false;
        }
    }
    private async Task StartPP()
    {
        //await StopPP(); //! 需先確認PP已停止
        ResetStopTokenSource();
        ExecutingTask = StartRecoder(ppCTS.Token);
        //_ = ExecutingTask.ContinueWith(x =>
        //                               {
        //                                   x.Dispose();
        //                                   //! 結束生產，填入資料
        //                                   OvenInfo.EndTime = DateTime.Now;
        //                                   OvenInfo.Recipe = GetRecipeTop();
        //                                   OvenInfo.Qty = OvenInfo.TopTempQuantity;
        //                                   OvenInfo.LotID = TopWorkOrder;
        //                                   OvenInfo.Part = TopPartID;
        //                                   OvenInfo.TopOrBottom = "Top";
        //                                   OvenInfo.OperatorID = TopOPID;
        //                                   OvenInfo.TotalRampTime = (OvenInfo.EndTime - OvenInfo.StartTime).Minutes;
        //                                   if (!isCheckin)
        //                                   {
        //                                       ClearInput("Top");
        //                                   }

        //                                   _ = ExecutingFinished?.Invoke(OvenInfo.Copy()!);

        //                                   NotifyPropertyChanged(nameof(IsExecuting));
        //                               });
        TopCheckin = DateTime.Now;
        NotifyPropertyChanged(nameof(IsExecuting));
    }
    private async Task StopPP()
    {
        if (ExecutingTask != null && IsExecuting)
        {
            ppCTS.Cancel();
            //! 結束生產，填入資料
            OvenInfo.StartTime = TopCheckin;
            OvenInfo.EndTime = DateTime.Now;
            OvenInfo.Recipe = GetRecipeTop();
            OvenInfo.Qty = OvenInfo.TopTempQuantity;
            OvenInfo.LotID = TopWorkOrder;
            OvenInfo.Part = TopPartID;
            OvenInfo.TopOrBottom = "Top";
            OvenInfo.OperatorID = TopOPID;
            OvenInfo.TotalRampTime = (OvenInfo.EndTime - OvenInfo.StartTime).Minutes;
            if (!isCheckin)
            {
                ClearInput();
            }
            _ = ExecutingFinished?.Invoke(OvenInfo.Copy()!);
            NotifyPropertyChanged(nameof(IsExecuting));
            await ExecutingTask;
        }
    }

    public PLC_Recipe GetRecipeTop() => new PLC_Recipe
    {
        NitrogenMode = NitrogenMode,
        OxygenContentSet = OxygenContentSet,
        RecipeName = RecipeName,
        DwellTime_1 = DwellTime_1,
        DwellTime_2 = DwellTime_2,
        DwellTime_3 = DwellTime_3,
        DwellTime_4 = DwellTime_4,
        DwellTime_5 = DwellTime_5,
        DwellTime_6 = DwellTime_6,
        DwellAlarm_1 = DwellAlarm_1,
        DwellAlarm_2 = DwellAlarm_2,
        DwellAlarm_3 = DwellAlarm_3,
        DwellAlarm_4 = DwellAlarm_4,
        DwellAlarm_5 = DwellAlarm_5,
        DwellAlarm_6 = DwellAlarm_6,
        CoolingTime = CoolingTime,
        CoolingTemperature = CoolingTemperature,
        RampTime_1 = RampTime_1,
        RampTime_2 = RampTime_2,
        RampTime_3 = RampTime_3,
        RampTime_4 = RampTime_4,
        RampTime_5 = RampTime_5,
        RampTime_6 = RampTime_6,
        RampAlarm_1 = RampAlarm_1,
        RampAlarm_2 = RampAlarm_2,
        RampAlarm_3 = RampAlarm_3,
        RampAlarm_4 = RampAlarm_4,
        RampAlarm_5 = RampAlarm_5,
        RampAlarm_6 = RampAlarm_6,
        InflatingTime = InflatingTime,
        TemperatureSetpoint_1 = TemperatureSetpoint_1,
        TemperatureSetpoint_2 = TemperatureSetpoint_2,
        TemperatureSetpoint_3 = TemperatureSetpoint_3,
        TemperatureSetpoint_4 = TemperatureSetpoint_4,
        TemperatureSetpoint_5 = TemperatureSetpoint_5,
        TemperatureSetpoint_6 = TemperatureSetpoint_6,
        SegmentCounts = SegmentCounts
    };

    public async Task<SetRecipeResult> SetRecipeAsync(PLC_Recipe? recipe)
    {
        if (await WriteRecipeToPlcAsync(recipe).ConfigureAwait(false) == SetRecipeResult.比對不相符)
        {
            RecipeChangeError = true;
        }

        if (RecipeChangeError)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, "配方切換失敗" },
                            { Language.CHS, "配方切换失敗" }
                        });
            return SetRecipeResult.比對不相符;
        }
        Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, "配方切換完成" },
                            { Language.CHS, "配方切换完成" }
                        });
        return SetRecipeResult.成功;
    }

    public void ClearInput()
    {
        Set(string.Empty, nameof(InputOperatorID));
        ClearInputTop();
        OvenInfo.TopTempProducts.Clear();
    }

    public void ClearInputTop()
    {
        inputFocusTB = null;
        Set(string.Empty, nameof(TopPartID));
        Set(string.Empty, nameof(TopProcessID));
        Set(string.Empty, nameof(TopRecipeID));
        Set(string.Empty, nameof(TopPanelCount));
        Set(string.Empty, nameof(TopWorkOrder));
        Set(string.Empty, nameof(TopBarcode));
        Set(string.Empty, nameof(TopOPID));
    }

    public void AddLOT(string PartID, string LotID, int quantity)
    {
        var info = new ProductInfo
        {
            PartID   = PartID.Trim(),
            LotID    = LotID.Trim(),
            Quantity = quantity
        };

        OvenInfo.TempProducts.Add(info);
    }

    public void TopAddLOT(string PartID, string LotID, int quantity)
    {
        var info = new ProductInfo
        {
            PartID   = PartID.Trim(),
            LotID    = LotID.Trim(),
            Quantity = quantity
        };

        OvenInfo.TopTempProducts.Add(info);
    }

    #region Interface Implementations
    public void Dispose() => ppCTS.Dispose();
    #endregion

}