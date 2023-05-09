using System;
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

namespace GPGO_MultiPLCs.ViewModels;

public sealed class PLC_ViewModel : GOL_DataModel, IDisposable
{
    public event Action?                                                                         ExecutingStarted;
    public event Action?                                                                         RecipeKeyInError;
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
    private          DateTime                OfflineTime    = DateTime.MaxValue;
    private          TextBox?                inputFocusTB;

    public int InputQuantityMin => 0;
    public int InputQuantityMax => 99999;
    public int InputLayerMin    => 1;
    public int InputLayerMax    => 8;
    //public event Action<PLC_Recipe> RecipeChangedbyPLC;
    public int          RecordDelay       { get; set; } = 1;
    public int          ClearInputDelay   { get; set; } = 60;
    public RelayCommand LoadedCommand     { get; }
    public RelayCommand InputFocusCommand { get; }
    public AsyncCommand StartCommand      { get; }
    public AsyncCommand StopCommand       { get; }

    public RelayCommand SilinceCommand { get; }
    /// <summary>取消投產</summary>
    public RelayCommand CancelCheckInCommand { get; }
    /// <summary>投產</summary>
    public RelayCommand CheckInCommand { get; }
    public RelayCommand CheckRecipeCommand_KeyIn    { get; }
    public RelayCommand CheckRecipeCommand_KeyLeave { get; }
    public RelayCommand AddLotCommand               { get; }
    public RelayCommand DeleteLotCommand            { get; }
    public RelayCommand GoDetailCommand             { get; }
    public RelayCommand ClearOPTextCommand          { get; }
    public RelayCommand ClearPartTextCommand        { get; }
    public RelayCommand ClearLotTextCommand         { get; }
    public RelayCommand ClearRecipeTextCommand      { get; }
    public RelayCommand ClearQuantityCommand        { get; }

    public RelayCommand CheckIsExecutingCommand { get; }

    /// <summary>機台資訊</summary>
    public BaseInfoWithChart OvenInfo { get; }

    /// <summary>取得是否正在紀錄溫度</summary>
    public bool IsExecuting => ExecutingTask?.Status is TaskStatus.Running or TaskStatus.WaitingForActivation or TaskStatus.WaitingToRun;

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
                   val >= 1.0                                                ? 1.0 : val;
        }
    }

    /// <summary>進度狀態</summary>
    public Status EquipmentStatus => !ConnectionStatus.CurrentValue ?
                                         Status.離線 :
                                         EquipmentState switch
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

    public ICollection<string> Recipe_Names
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
            if (value.Length < 10)
            {
                Set(string.Empty);
                Dialog.Show(new Dictionary<Language, string>
                            {
                                { Language.TW, "需至少10個字元" },
                                { Language.CHS, "需至少10个字符" },
                                { Language.EN, "At least 10 chars" }
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

    public LogEvent SelectedLogEvent
    {
        set => OvenInfo.ChartModel.SetAnnotation(value);
    }

    public PLC_ViewModel(IDialogService                                                               dialog,
                         IGate                                                                        gate,
                         int                                                                          plcindex,
                         string                                                                       plctag,
                         (Dictionary<BitType, int> bits_shift, Dictionary<DataType, int> datas_shift) shift = new()) : base(gate, plcindex, plctag, shift)
    {
        InputLayer = InputLayerMin;
        Dialog     = dialog;

        ConnectionStatus.ValueChanged += status =>
                                         {
                                             NotifyPropertyChanged(nameof(EquipmentStatus));
                                             NotifyPropertyChanged(nameof(Progress));

                                             EventHappened?.Invoke((status ? EventType.StatusChanged : EventType.Alarm, DateTime.Now, "Connection Status", string.Empty, status));
                                             if (IsExecuting)
                                             {
                                                 AddProcessEvent((status ? EventType.StatusChanged : EventType.Alarm, DateTime.Now, "Connection Status", string.Empty, status));
                                             }

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

        LoadedCommand = new RelayCommand(e =>
                                         {
                                             if (e is FrameworkElement el)
                                             {
                                                 OvenInfo.ChartModel.SetFrameworkElement(el);
                                             }
                                         });

        InputFocusCommand = new RelayCommand(e => inputFocusTB = e as TextBox);

        CheckRecipeCommand_KeyIn = new RelayCommand(async text =>
                                                    {
                                                        if (text is string name && name != string.Empty)
                                                        {
                                                            name = name.Trim();
                                                            using var matches = new PooledList<string>();

                                                            foreach (var r in Recipe_Names)
                                                            {
                                                                if (r == name) //! 100%符合的優先
                                                                {
                                                                    _ = await SetRecipeDialogAsync(name);
                                                                    return;
                                                                }

                                                                if (r.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                                                                {
                                                                    matches.Add(r);
                                                                }
                                                            }

                                                            if (matches.Count > 0)
                                                            {
                                                                _ = await SetRecipeDialogAsync(matches[0]);
                                                            }
                                                            else
                                                            {
                                                                InputRecipeName = string.Empty;
                                                                RecipeKeyInError?.Invoke();
                                                            }
                                                        }
                                                    });

        CheckRecipeCommand_KeyLeave = new RelayCommand(_ =>
                                                       {
                                                       });

        AddLotCommand = new RelayCommand(_ =>
                                         {
                                             if (InputQuantity <= 0 || string.IsNullOrEmpty(InputPartID) || string.IsNullOrEmpty(InputLotID))
                                             {
                                                 return;
                                             }

                                             OvenInfo.OperatorID = InputOperatorID;

                                             //! 當PartID、LotID和Layer都相等，數量則直接覆蓋
                                             var same = OvenInfo.TempProducts.FirstOrDefault(x => x.PartID == InputPartID && x.LotID == InputLotID && x.Layer == InputLayer);
                                             if (same != null)
                                             {
                                                 same.Quantity += InputQuantity;
                                                 Quantity      =  (short)same.Quantity;
                                             }
                                             else
                                             {
                                                 PartID   = InputPartID;
                                                 LotID    = InputLotID;
                                                 Quantity = (short)InputQuantity;

                                                 var info = new ProductInfo
                                                            {
                                                                PartID   = InputPartID,
                                                                LotID    = InputLotID,
                                                                Layer    = InputLayer,
                                                                Quantity = InputQuantity
                                                            };

                                                 OvenInfo.TempProducts.Add(info);
                                             }

                                             LotAdded?.Invoke(string.Join(",", OvenInfo.TempProducts.Select(x => x.LotID)));
                                         });

        DeleteLotCommand = new RelayCommand(lot =>
                                            {
                                                if (lot is ProductInfo info)
                                                {
                                                    using var list = OvenInfo.TempProducts.ToPooledList();
                                                    list.Remove(info);

                                                    LotRemoved?.Invoke(string.Join(",", OvenInfo.TempProducts.Select(x => x.LotID)));

                                                    ClearInput();
                                                    list.ForEach(x => OvenInfo.TempProducts.Add(x));
                                                }
                                            });

        CheckInCommand = new RelayCommand(_ =>
                                          {
                                              isCheckin = true;

                                              OvenInfo.OperatorID = InputOperatorID;
                                              RackID              = OvenInfo.TempProducts.FirstOrDefault()?.LotID ?? string.Empty;
                                              DoorLock            = true;
                                              CheckIn?.Invoke((opid: OvenInfo.OperatorID, rackid: RackID));
                                          });

        CancelCheckInCommand = new RelayCommand(e =>
                                                {
                                                    if (e != null)
                                                    {
                                                        CheckOut?.Invoke(OvenInfo.RackID);
                                                        ClearInput();
                                                        BeepSilince = true;

                                                        AutoMode   = false;
                                                        ManualMode = true; //! 結批後切回手動模式
                                                    }
                                                    else
                                                    {
                                                        CancelCheckIn?.Invoke(OvenInfo.RackID);
                                                        ClearInput();
                                                        LotRemoved?.Invoke(string.Join(",", OvenInfo.TempProducts.Select(x => x.LotID)));
                                                    }

                                                    isCheckin = false;
                                                    DoorLock  = false;
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

        SilinceCommand = new RelayCommand(_ => BeepSilince = true);

        StartCommand = new AsyncCommand(async _ =>
                                        {
                                            if (CheckUser != null && !CheckUser.Invoke(InputOperatorID))
                                            {
                                                dialog.Show(new Dictionary<Language, string>
                                                            {
                                                                { Language.TW, "OP權限不符" },
                                                                { Language.CHS, "OP权限不符" },
                                                                { Language.EN, "OP permissions error." }
                                                            },
                                                            DialogMsgType.Alert);

                                                return;
                                            }

                                            if (!AutoMode)
                                            {
                                                AutoMode = true;
                                            }

                                            OvenInfo.OperatorID = InputOperatorID;
                                            RackID              = OvenInfo.TempProducts.FirstOrDefault()?.LotID ?? string.Empty;
                                            DoorLock            = true;

                                            if (!RecipeCompareSV())
                                            {
                                                if (!await dialog.Show(new Dictionary<Language, string>
                                                                       {
                                                                           { Language.TW, "執行配方比對異常！\n確定啟動烘烤？" },
                                                                           { Language.CHS, "执行配方比对异常！\n确定启动烘烤？" },
                                                                           { Language.EN, "Executing recipe is abnormal!\nAre you sure you want to start baking?" }
                                                                       },
                                                                       true,
                                                                       TimeSpan.FromSeconds(15),
                                                                       DialogMsgType.Alarm))
                                                {
                                                    AutoMode = false;
                                                    return;
                                                }
                                            }
                                            else if (!await dialog.Show(new Dictionary<Language, string>
                                                                        {
                                                                            { Language.TW, "確定啟動烘烤？" },
                                                                            { Language.CHS, "确定启动烘烤？" },
                                                                            { Language.EN, "Are you sure you want to start baking?" }
                                                                        },
                                                                        true,
                                                                        TimeSpan.FromSeconds(15),
                                                                        DialogMsgType.Alert))
                                            {
                                                AutoMode = false;
                                                return;
                                            }

                                            AutoMode_Stop  = false;
                                            AutoMode_Start = true;
                                        },
                                        null);

        StopCommand = new AsyncCommand(async _ =>
                                       {
                                           if (!AllowStop)
                                           {
                                               dialog.Show(new Dictionary<Language, string>
                                                           {
                                                               { Language.TW, "不允許停止" },
                                                               { Language.CHS, "不允许停止" },
                                                               { Language.EN, "Not allowed to stop." }
                                                           },
                                                           DialogMsgType.Alert);

                                               return;
                                           }

                                           if (!await dialog.Show(new Dictionary<Language, string>
                                                                  {
                                                                      { Language.TW, "確定停止烘烤？" },
                                                                      { Language.CHS, "确定停止烘烤？" },
                                                                      { Language.EN, "Are you sure you want to stop baking?" }
                                                                  },
                                                                  true,
                                                                  TimeSpan.FromSeconds(15),
                                                                  DialogMsgType.Alert))
                                           {
                                               return;
                                           }

                                           AutoMode_Start = false;
                                           AutoMode_Stop  = true;
                                       },
                                       null);

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
                                       AutoMode_Start = false;
                                       _              = StopPP();
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

                                    if (name == nameof(AutoMode_Start))
                                    {
                                        if (!val)
                                        {
                                            return;
                                        }

                                        InvokeSECSEvent?.Invoke("ProcessStarted");
                                        AutoMode_Stop = false;
                                        _             = StartPP();
                                    }
                                    else if (name == nameof(ProcessComplete))
                                    {
                                        if (!val)
                                        {
                                            return;
                                        }

                                        InvokeSECSEvent?.Invoke("ProcessComplete");
                                        OvenInfo.IsFinished = true;
                                        AutoMode_Start      = false;
                                        _                   = StopPP();
                                    }
                                    else if (name == nameof(AutoMode_Stop))
                                    {
                                        if (!val)
                                        {
                                            return;
                                        }

                                        InvokeSECSEvent?.Invoke("ProcessStopped");
                                        AutoMode_Start = false;
                                        _              = StopPP();
                                    }
                                }
                                else if (value is short sv)
                                {
                                    if (name is nameof(CurrentSegment))
                                    {
                                        EventHappened?.Invoke(eventval!);
                                        if (IsExecuting)
                                        {
                                            AddProcessEvent(eventval!);
                                        }
                                    }
                                    else if (name == nameof(EquipmentState))
                                    {
                                        InvokeSECSEvent?.Invoke("EqpStatusChanged");
                                        SV_Changed?.Invoke($"Previous{name}", oldvalue!);

                                        EventHappened?.Invoke(eventval!);
                                        if (IsExecuting)
                                        {
                                            AddProcessEvent(eventval!);
                                        }

                                        NotifyPropertyChanged(nameof(EquipmentStatus));
                                    }
                                    else if (name == nameof(ProcessState))
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

                                                   SV_Changed?.Invoke("LotIDs",   lots.Count  > 0 ? string.Join(",", lots) : string.Empty);
                                                   SV_Changed?.Invoke("PartIDs",  parts.Count > 0 ? string.Join(",", parts) : string.Empty);
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
            Set(string.Empty,     nameof(InputOperatorID));
            Set(string.Empty,     nameof(InputPartID));
            Set(string.Empty,     nameof(InputLotID));
            Set(string.Empty,     nameof(InputRecipeName));
            Set(InputQuantityMin, nameof(InputQuantity));
            Set(InputLayerMin,    nameof(InputLayer));
        }
    }

    private async Task<SetRecipeResult> WriteRecipeToPlcAsync(PLC_Recipe recipe)
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

        var errs = await ManualSetByPropertiesWithCheck(recipe.ToDictionary());

        InvokeSECSEvent?.Invoke("RecipeChanged");

        var result = errs.Count == 0 ? SetRecipeResult.成功 : SetRecipeResult.比對不相符;
        if (result == SetRecipeResult.成功)
        {
            AutoMode = true;
        }
        return result;
    }

    private bool RecipeCompare(PLC_Recipe recipe) => NitrogenMode                          == recipe.NitrogenMode                          &&
                                                     OxygenContentSet.ToString("0.0")      == recipe.OxygenContentSet.ToString("0.0")      &&
                                                     RecipeName.Substring(0, 16)           == recipe.RecipeName.Substring(0, 16)           &&
                                                     DwellTime_1.ToString("0.0")           == recipe.DwellTime_1.ToString("0.0")           &&
                                                     DwellTime_2.ToString("0.0")           == recipe.DwellTime_2.ToString("0.0")           &&
                                                     DwellTime_3.ToString("0.0")           == recipe.DwellTime_3.ToString("0.0")           &&
                                                     DwellTime_4.ToString("0.0")           == recipe.DwellTime_4.ToString("0.0")           &&
                                                     DwellTime_5.ToString("0.0")           == recipe.DwellTime_5.ToString("0.0")           &&
                                                     DwellTime_6.ToString("0.0")           == recipe.DwellTime_6.ToString("0.0")           &&
                                                     DwellAlarm_1.ToString("0.0")          == recipe.DwellAlarm_1.ToString("0.0")          &&
                                                     DwellAlarm_2.ToString("0.0")          == recipe.DwellAlarm_2.ToString("0.0")          &&
                                                     DwellAlarm_3.ToString("0.0")          == recipe.DwellAlarm_3.ToString("0.0")          &&
                                                     DwellAlarm_4.ToString("0.0")          == recipe.DwellAlarm_4.ToString("0.0")          &&
                                                     DwellAlarm_5.ToString("0.0")          == recipe.DwellAlarm_5.ToString("0.0")          &&
                                                     DwellAlarm_6.ToString("0.0")          == recipe.DwellAlarm_6.ToString("0.0")          &&
                                                     CoolingTime.ToString("0.0")           == recipe.CoolingTime.ToString("0.0")           &&
                                                     CoolingTemperature.ToString("0.0")    == recipe.CoolingTemperature.ToString("0.0")    &&
                                                     RampTime_1.ToString("0.0")            == recipe.RampTime_1.ToString("0.0")            &&
                                                     RampTime_2.ToString("0.0")            == recipe.RampTime_2.ToString("0.0")            &&
                                                     RampTime_3.ToString("0.0")            == recipe.RampTime_3.ToString("0.0")            &&
                                                     RampTime_4.ToString("0.0")            == recipe.RampTime_4.ToString("0.0")            &&
                                                     RampTime_5.ToString("0.0")            == recipe.RampTime_5.ToString("0.0")            &&
                                                     RampTime_6.ToString("0.0")            == recipe.RampTime_6.ToString("0.0")            &&
                                                     RampAlarm_1.ToString("0.0")           == recipe.RampAlarm_1.ToString("0.0")           &&
                                                     RampAlarm_2.ToString("0.0")           == recipe.RampAlarm_2.ToString("0.0")           &&
                                                     RampAlarm_3.ToString("0.0")           == recipe.RampAlarm_3.ToString("0.0")           &&
                                                     RampAlarm_4.ToString("0.0")           == recipe.RampAlarm_4.ToString("0.0")           &&
                                                     RampAlarm_5.ToString("0.0")           == recipe.RampAlarm_5.ToString("0.0")           &&
                                                     RampAlarm_6.ToString("0.0")           == recipe.RampAlarm_6.ToString("0.0")           &&
                                                     InflatingTime.ToString("0")           == recipe.InflatingTime.ToString("0")           &&
                                                     TemperatureSetpoint_1.ToString("0.0") == recipe.TemperatureSetpoint_1.ToString("0.0") &&
                                                     TemperatureSetpoint_2.ToString("0.0") == recipe.TemperatureSetpoint_2.ToString("0.0") &&
                                                     TemperatureSetpoint_3.ToString("0.0") == recipe.TemperatureSetpoint_3.ToString("0.0") &&
                                                     TemperatureSetpoint_4.ToString("0.0") == recipe.TemperatureSetpoint_4.ToString("0.0") &&
                                                     TemperatureSetpoint_5.ToString("0.0") == recipe.TemperatureSetpoint_5.ToString("0.0") &&
                                                     TemperatureSetpoint_6.ToString("0.0") == recipe.TemperatureSetpoint_6.ToString("0.0") &&
                                                     SegmentCounts                         == recipe.SegmentCounts;

    private void AddProcessEvent((EventType type, DateTime addtime, string note, string tag, object value) eventdata)
    {
        if (!IsExecuting)
        {
            return;
        }

        ManualRecord                          = true;
        var (type, addtime, note, tag, value) = eventdata;
        OvenInfo.EventList.Add(new LogEvent
                               {
                                   Type        = type,
                                   AddedTime   = addtime,
                                   Description = note,
                                   TagCode     = tag,
                                   Value       = value
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

    private async Task<SetRecipeResult> SetRecipeDialogAsync(string recipeName)
    {
        if (GetRecipe?.Invoke(recipeName) is not { } recipe)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, "配方讀取錯誤" },
                            { Language.CHS, "配方读取错误" },
                            { Language.EN, "Recipe loaded Fail" }
                        });

            return SetRecipeResult.條件不允許;
        }

        if (IsExecuting)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, "烤箱仍在烘烤`" },
                            { Language.CHS, "烤箱仍在烘烤" },
                            { Language.EN, "Oven is still executing" }
                        });

            return SetRecipeResult.條件不允許;
        }

        if (!RemoteMode)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, "烤箱未在Remote模式" },
                            { Language.CHS, "烤箱未在Remote模式" },
                            { Language.EN, "Oven is not in Remote Mode" }
                        });

            return SetRecipeResult.條件不允許;
        }

        if (RecipeCompare(recipe)) //! 配方相同就不再確認
        {
            if (AutoMode)
            {
                AutoMode = false;
                await Task.Delay(900).ConfigureAwait(false);
            }

            AutoMode = true;
            return SetRecipeResult.無需變更;
        }

        CheckRecipeCTS.Dispose();
        CheckRecipeCTS = new CancellationTokenSource();
        if (!await Dialog.Show(new Dictionary<Language, string>
                               {
                                   { Language.TW, "請確認配方內容：" },
                                   { Language.CHS, "请确认配方内容：" },
                                   { Language.EN, "Please confirm this recipe:" }
                               },
                               recipe,
                               true,
                               TimeSpan.FromMilliseconds(int.MaxValue),
                               DialogMsgType.Alert,
                               CheckRecipeCTS.Token))
        {
            return SetRecipeResult.條件不允許;
        }

        if (await WriteRecipeToPlcAsync(recipe) == SetRecipeResult.比對不相符)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, "配方比對錯誤！" },
                            { Language.CHS, "配方比对错误！" },
                            { Language.EN, "Recipe comparison error!" }
                        },
                        TimeSpan.FromSeconds(3),
                        DialogMsgType.Alarm);

            RecipeChangeError = true;
            return SetRecipeResult.比對不相符;
        }

        return SetRecipeResult.成功;
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

        void AddTemperatures(bool     keypoint,
                             DateTime addtime,
                             double   t0,
                             double   t1,
                             double   t2,
                             double   t3,
                             double   t4,
                             double   t5,
                             double   t6,
                             double   t7,
                             double   t8,
                             double   oxy)
        {
            var record = new RecordTemperatures
                         {
                             AddedTime                = addtime,
                             PV_ThermostatTemperature = t0,
                             OvenTemperatures_1       = t1,
                             OvenTemperatures_2       = t2,
                             OvenTemperatures_3       = t3,
                             OvenTemperatures_4       = t4,
                             OvenTemperatures_5       = t5,
                             OvenTemperatures_6       = t6,
                             OvenTemperatures_7       = t7,
                             OvenTemperatures_8       = t8,
                             OxygenContent            = oxy
                         };

            OvenInfo.RecordTemperatures.Add(record);
            OvenInfo.ChartModel.AddData(record);
        }

        OvenInfo.StartTime = DateTime.Now;
        var nt                     = OvenInfo.StartTime;
        var n                      = TimeSpan.FromSeconds(RecordDelay); //! 每delay週期紀錄一次
        var _ThermostatTemperature = PV_ThermostatTemperature;
        var _OvenTemperature_1     = OvenTemperature_1;
        var _OvenTemperature_2     = OvenTemperature_2;
        var _OvenTemperature_3     = OvenTemperature_3;
        var _OvenTemperature_4     = OvenTemperature_4;
        var _OvenTemperature_5     = OvenTemperature_5;
        var _OvenTemperature_6     = OvenTemperature_6;
        var _OvenTemperature_7     = OvenTemperature_7;
        var _OvenTemperature_8     = OvenTemperature_8;
        var _OxygenContent         = OxygenContent;

        AddTemperatures(true,
                        OvenInfo.StartTime,
                        _ThermostatTemperature,
                        _OvenTemperature_1,
                        _OvenTemperature_2,
                        _OvenTemperature_3,
                        _OvenTemperature_4,
                        _OvenTemperature_5,
                        _OvenTemperature_6,
                        _OvenTemperature_7,
                        _OvenTemperature_8,
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

                                            _ThermostatTemperature = PV_ThermostatTemperature <= 0 ? _ThermostatTemperature : PV_ThermostatTemperature;
                                            _OvenTemperature_1     = OvenTemperature_1        <= 0 ? _OvenTemperature_1 : OvenTemperature_1;
                                            _OvenTemperature_2     = OvenTemperature_2        <= 0 ? _OvenTemperature_2 : OvenTemperature_2;
                                            _OvenTemperature_3     = OvenTemperature_3        <= 0 ? _OvenTemperature_3 : OvenTemperature_3;
                                            _OvenTemperature_4     = OvenTemperature_4        <= 0 ? _OvenTemperature_4 : OvenTemperature_4;
                                            _OvenTemperature_5     = OvenTemperature_5        <= 0 ? _OvenTemperature_5 : OvenTemperature_5;
                                            _OvenTemperature_6     = OvenTemperature_6        <= 0 ? _OvenTemperature_6 : OvenTemperature_6;
                                            _OvenTemperature_7     = OvenTemperature_7        <= 0 ? _OvenTemperature_7 : OvenTemperature_7;
                                            _OvenTemperature_8     = OvenTemperature_8        <= 0 ? _OvenTemperature_8 : OvenTemperature_8;
                                            _OxygenContent         = OxygenContent            <= 0 ? _OxygenContent : OxygenContent;

                                            if (DateTime.Now - nt >= n && ConnectionStatus.CurrentValue)
                                            {
                                                nt = DateTime.Now;
                                                AddTemperatures(false,
                                                                nt,
                                                                _ThermostatTemperature,
                                                                _OvenTemperature_1,
                                                                _OvenTemperature_2,
                                                                _OvenTemperature_3,
                                                                _OvenTemperature_4,
                                                                _OvenTemperature_5,
                                                                _OvenTemperature_6,
                                                                _OvenTemperature_7,
                                                                _OvenTemperature_8,
                                                                _OxygenContent);
                                            }
                                            else if (ManualRecord)
                                            {
                                                ManualRecord = false;
                                                AddTemperatures(true,
                                                                DateTime.Now,
                                                                _ThermostatTemperature,
                                                                _OvenTemperature_1,
                                                                _OvenTemperature_2,
                                                                _OvenTemperature_3,
                                                                _OvenTemperature_4,
                                                                _OvenTemperature_5,
                                                                _OvenTemperature_6,
                                                                _OvenTemperature_7,
                                                                _OvenTemperature_8,
                                                                _OxygenContent);
                                            }
                                            else
                                            {
                                                SpinWait.SpinUntil(() => false, 15);
                                            }
                                        }

                                        _ThermostatTemperature = PV_ThermostatTemperature <= 0 ? _ThermostatTemperature : PV_ThermostatTemperature;
                                        _OvenTemperature_1     = OvenTemperature_1        <= 0 ? _OvenTemperature_1 : OvenTemperature_1;
                                        _OvenTemperature_2     = OvenTemperature_2        <= 0 ? _OvenTemperature_2 : OvenTemperature_2;
                                        _OvenTemperature_3     = OvenTemperature_3        <= 0 ? _OvenTemperature_3 : OvenTemperature_3;
                                        _OvenTemperature_4     = OvenTemperature_4        <= 0 ? _OvenTemperature_4 : OvenTemperature_4;
                                        _OvenTemperature_5     = OvenTemperature_5        <= 0 ? _OvenTemperature_5 : OvenTemperature_5;
                                        _OvenTemperature_6     = OvenTemperature_6        <= 0 ? _OvenTemperature_6 : OvenTemperature_6;
                                        _OvenTemperature_7     = OvenTemperature_7        <= 0 ? _OvenTemperature_7 : OvenTemperature_7;
                                        _OvenTemperature_8     = OvenTemperature_8        <= 0 ? _OvenTemperature_8 : OvenTemperature_8;
                                        _OxygenContent         = OxygenContent            <= 0 ? _OxygenContent : OxygenContent;

                                        AddTemperatures(true,
                                                        DateTime.Now,
                                                        _ThermostatTemperature,
                                                        _OvenTemperature_1,
                                                        _OvenTemperature_2,
                                                        _OvenTemperature_3,
                                                        _OvenTemperature_4,
                                                        _OvenTemperature_5,
                                                        _OvenTemperature_6,
                                                        _OvenTemperature_7,
                                                        _OvenTemperature_8,
                                                        _OxygenContent);
                                    },
                                    ct);
    }

    private async Task StartPP()
    {
        await StopPP(); //! 需先確認PP已停止

        ResetStopTokenSource();
        ExecutingTask = StartRecoder(ppCTS.Token);
        _ = ExecutingTask.ContinueWith(x =>
                                       {
                                           x.Dispose();

                                           DoorLock    = false;
                                           BeepSilince = false;
                                           //! 結束生產，填入資料
                                           OvenInfo.EndTime       = DateTime.Now;
                                           OvenInfo.Recipe        = GetRecipeSV();
                                           OvenInfo.TotalRampTime = (OvenInfo.EndTime - OvenInfo.StartTime).Minutes;

                                           if (!isCheckin)
                                           {
                                               ClearInput();
                                           }

                                           _ = ExecutingFinished?.Invoke(OvenInfo.Copy()!);

                                           NotifyPropertyChanged(nameof(IsExecuting));
                                       });

        NotifyPropertyChanged(nameof(IsExecuting));
        ExecutingStarted?.Invoke();
    }

    private async Task StopPP()
    {
        if (ExecutingTask != null && IsExecuting)
        {
            ppCTS.Cancel();

            await ExecutingTask;
        }
    }

    public bool RecipeCompareSV() => RecipeCompare(GetRecipeSV());

    public PLC_Recipe GetRecipeSV() => new()
                                       {
                                           NitrogenMode          = SV_NitrogenMode,
                                           OxygenContentSet      = SV_OxygenContentSet,
                                           RecipeName            = SV_RecipeName,
                                           DwellTemperature_1    = SV_DwellTemperature_1,
                                           DwellTemperature_2    = SV_DwellTemperature_2,
                                           DwellTemperature_3    = SV_DwellTemperature_3,
                                           DwellTemperature_4    = SV_DwellTemperature_4,
                                           DwellTemperature_5    = SV_DwellTemperature_5,
                                           DwellTemperature_6    = SV_DwellTemperature_6,
                                           DwellTemperature_7    = SV_DwellTemperature_7,
                                           DwellTemperature_8    = SV_DwellTemperature_8,
                                           DwellTime_1           = SV_DwellTime_1,
                                           DwellTime_2           = SV_DwellTime_2,
                                           DwellTime_3           = SV_DwellTime_3,
                                           DwellTime_4           = SV_DwellTime_4,
                                           DwellTime_5           = SV_DwellTime_5,
                                           DwellTime_6           = SV_DwellTime_6,
                                           DwellTime_7           = SV_DwellTime_7,
                                           DwellTime_8           = SV_DwellTime_8,
                                           DwellAlarm_1          = SV_DwellAlarm_1,
                                           DwellAlarm_2          = SV_DwellAlarm_2,
                                           DwellAlarm_3          = SV_DwellAlarm_3,
                                           DwellAlarm_4          = SV_DwellAlarm_4,
                                           DwellAlarm_5          = SV_DwellAlarm_5,
                                           DwellAlarm_6          = SV_DwellAlarm_6,
                                           DwellAlarm_7          = SV_DwellAlarm_7,
                                           DwellAlarm_8          = SV_DwellAlarm_8,
                                           CoolingTime           = SV_CoolingTime,
                                           CoolingTemperature    = SV_CoolingTemperature,
                                           RampTime_1            = SV_RampTime_1,
                                           RampTime_2            = SV_RampTime_2,
                                           RampTime_3            = SV_RampTime_3,
                                           RampTime_4            = SV_RampTime_4,
                                           RampTime_5            = SV_RampTime_5,
                                           RampTime_6            = SV_RampTime_6,
                                           RampTime_7            = SV_RampTime_7,
                                           RampTime_8            = SV_RampTime_8,
                                           RampAlarm_1           = SV_RampAlarm_1,
                                           RampAlarm_2           = SV_RampAlarm_2,
                                           RampAlarm_3           = SV_RampAlarm_3,
                                           RampAlarm_4           = SV_RampAlarm_4,
                                           RampAlarm_5           = SV_RampAlarm_5,
                                           RampAlarm_6           = SV_RampAlarm_6,
                                           RampAlarm_7           = SV_RampAlarm_7,
                                           RampAlarm_8           = SV_RampAlarm_8,
                                           InflatingTime         = SV_InflatingTime,
                                           TemperatureSetpoint_1 = SV_TemperatureSetpoint_1,
                                           TemperatureSetpoint_2 = SV_TemperatureSetpoint_2,
                                           TemperatureSetpoint_3 = SV_TemperatureSetpoint_3,
                                           TemperatureSetpoint_4 = SV_TemperatureSetpoint_4,
                                           TemperatureSetpoint_5 = SV_TemperatureSetpoint_5,
                                           TemperatureSetpoint_6 = SV_TemperatureSetpoint_6,
                                           TemperatureSetpoint_7 = SV_TemperatureSetpoint_7,
                                           TemperatureSetpoint_8 = SV_TemperatureSetpoint_8,
                                           SegmentCounts         = SV_SegmentCounts
                                       };

    public async Task<SetRecipeResult> SetRecipeAsync(PLC_Recipe? recipe)
    {
        if (recipe == null || IsExecuting || !RemoteMode)
        {
            return SetRecipeResult.條件不允許;
        }

        if (await WriteRecipeToPlcAsync(recipe).ConfigureAwait(false) == SetRecipeResult.比對不相符)
        {
            RecipeChangeError = true;
            return SetRecipeResult.比對不相符;
        }

        return SetRecipeResult.成功;
    }

    public void ClearInput()
    {
        inputFocusTB = null;
        OvenInfo.TempProducts.Clear();
        Set(string.Empty,     nameof(InputOperatorID));
        Set(string.Empty,     nameof(InputPartID));
        Set(string.Empty,     nameof(InputLotID));
        Set(string.Empty,     nameof(InputRecipeName));
        Set(InputQuantityMin, nameof(InputQuantity));
        Set(InputLayerMin,    nameof(InputLayer));
    }

    public void AddLOT(string PartID, string LotID, int layer, int quantity)
    {
        if (OvenInfo.TempProducts.FirstOrDefault(x => x.PartID == PartID.Trim() && x.LotID == LotID.Trim() && x.Layer == layer) is { } product)
        {
            product.Quantity += quantity;
        }
        else
        {
            var info = new ProductInfo
                       {
                           PartID   = PartID.Trim(),
                           LotID    = LotID.Trim(),
                           Layer    = layer,
                           Quantity = quantity
                       };

            OvenInfo.TempProducts.Add(info);
        }
    }

    #region Interface Implementations
    public void Dispose() => ppCTS.Dispose();
    #endregion
}