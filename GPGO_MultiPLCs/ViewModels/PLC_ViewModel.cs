
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GPGRC_MultiPLCs.Models;
using GPMVVM.Helpers;
using GPMVVM.Models;
using MongoDB.Driver;
using PLCService;
#pragma warning disable VSTHRD110

namespace GPGRC_MultiPLCs.ViewModels;

public sealed class PLC_ViewModel : GRC_DataModel, IDisposable
{
    public event Action?                                                                         ExecutingStarted;
    public event Action?                                                                         RecipeKeyInError;
    public event Func<bool>?                                                                     CheckRbReady;
    public event Action<(double,string)>?                                                        WriteToRB;
    public event Action?                                                                         WantDetail;
    public event Action<(EventType type, DateTime time, string note, string tag, object value)>? EventHappened;
    public event Action<(int,string)>? PanelMoveHappened;
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
    public event Func<bool>                                                                      bChangeStatusevent;
    public event Func<(int StationIndex, ProcessInfo Info), Task<int>>?                          AddRecordToDB;

    private readonly CountDownTimer          countDownTimer = new();
    private readonly IDialogService          Dialog;
    private readonly TaskFactory             OneScheduler = new(new StaTaskScheduler(1));
    private          bool                    isCheckin;
    private          bool                    ManualRecord;
    private          DateTime                OfflineTime    = DateTime.MaxValue;
    private          TextBox?                inputFocusTB;
    public ObservableCollection<Item> RecipeItem { get; set; }
    public ObservableCollection<TemperatureItem> TemperatureItems { get; set; }

    public int InputQuantityMin => 0;
    public int InputQuantityMax => 99999;
    public int InputLayerMin => 1;
    public int InputLayerMax => 8;
    //public event Action<PLC_Recipe> RecipeChangedbyPLC;
    public int RecordDelay { get; set; } = 1;
    public int ClearInputDelay { get; set; } = 60;
    public RelayCommand InputFocusCommand { get; }
    public AsyncCommand StartCommand { get; }
    public AsyncCommand StopCommand { get; }
    public RelayCommand SilinceCommand { get; }
    /// <summary>取消投產</summary>
    public RelayCommand CancelCheckInCommand { get; }
    /// <summary>投產</summary>
    public RelayCommand CheckInCommand { get; }
    public CommandWithResult<bool> CheckInDialogCommand { get; }
    public RelayCommand GetRecipeCommand { get; }
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
    public RelayCommand CheckCommand { get; }

    /// <summary>機台資訊</summary>
    public BaseInfoWithChart OvenInfo { get; }

    /// <summary>取得是否正在紀錄溫度</summary>
    public bool IsExecuting => ExecutingTask?.Status is TaskStatus.Running or TaskStatus.WaitingForActivation or TaskStatus.WaitingToRun;

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
    public bool IsCheckin
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool IsCheckOut
    {
        get => Get<bool>();
        set => Set(value);
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
    public Func<object, object> START_Command { get; internal set; }

    public PLC_ViewModel(IDialogService dialog,
                         IGate gate,
                         int plcindex,
                         string plctag,
                         (Dictionary<BitType, int> bits_shift, Dictionary<DataType, int> datas_shift) shift = new()) : base(gate, plcindex, plctag, shift)
    {
        InputLayer = InputLayerMin;
        Dialog = dialog;
        ConnectionStatus.ValueChanged += status =>
                                         {
                                             NotifyPropertyChanged(nameof(EquipmentStatus));

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

        InputFocusCommand = new RelayCommand(e => inputFocusTB = e as TextBox);

        CheckRecipeCommand_KeyIn = new RelayCommand(async text =>
        {
            if (text is string name && name != string.Empty)
            {
                name = name.Trim();

                if (Recipe_Names != null)
                {
                    using var matches = new PooledList<string>();
                    foreach (var r in Recipe_Names)
                    {
                        if (r == name) //! 100%符合的優先
                        {
                            var result   = await SetRecipeDialogAsync(name,plcindex);
                            var eventval = (EventType.Operator, DateTime.Now, "SetRecipe", "", $"{name}:{result}");
                            EventHappened?.Invoke(eventval);
                            return;
                        }

                        if (r.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            matches.Add(r);
                        }
                    }

                    if (matches.Count > 0)
                    {
                        var ppname   = matches[0];
                        var result   = await SetRecipeDialogAsync(ppname,plcindex);
                        var eventval = (EventType.Operator, DateTime.Now, "SetRecipe", "", $"{ppname}:{result}");
                        EventHappened?.Invoke(eventval);
                        return;
                    }
                }

                InputRecipeName = string.Empty;
                RecipeKeyInError?.Invoke();
            }
        });

        CheckInDialogCommand = new CommandWithResult<bool>(_ => false);

        CheckInCommand = new RelayCommand(_ =>
        {
            if (SecsIsRemoteOnline)
            {
                if (IsCheckin)
                {
                    CancelCheckInCommand?.Execute(null);
                }
                else
                {
                    IsCheckin = true;
                    OvenInfo.OperatorID = InputOperatorID;
                    RackID = OvenInfo.TempProducts.FirstOrDefault()?.LotID ?? string.Empty;
                    DoorLock = true;
                    CheckIn?.Invoke((opid: OvenInfo.OperatorID, rackid: RackID));

                    var eventval = (EventType.Operator, DateTime.Now, nameof(CheckInCommand), "", "");
                    EventHappened?.Invoke(eventval);
                }
            }
            else
            {
                CancelCheckIn?.Invoke(OvenInfo.RackID);
                ClearInput();
                InvokeSECSEvent?.Invoke("LotRemoved");

                var eventval = (EventType.Operator, DateTime.Now, nameof(CancelCheckIn), "", "");
                EventHappened?.Invoke(eventval);
            }
        });

        CancelCheckInCommand = new RelayCommand(_ =>
        {
            CheckOut?.Invoke(OvenInfo.RackID);
            ClearInput();
            BeepSilince = true;
            AutoMode = false;

            var eventval = (EventType.Operator, DateTime.Now, nameof(CheckOut), "", "");
            EventHappened?.Invoke(eventval);

            IsCheckOut = false;
            IsCheckin = false;
            DoorLock = false;
        });

        CheckCommand = new RelayCommand(e =>
        {
            if (e is MouseButtonEventArgs args)
            {
                //! 避免烘烤中意外中止
                if (IsExecuting && IsCheckin)
                {
                    args.Handled = true;
                }
                //! 避免CheckIn的OP權限不符
                else if (!IsCheckin)
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

                        args.Handled = true;
                    }
                    else if (DoorNotOpen)
                    {
                        dialog.Show(new Dictionary<Language, string>
                                                                    {
                                                                        { Language.TW, "停止後未開門" },
                                                                        { Language.CHS, "停止后未开门" },
                                                                        { Language.EN, "The door did not open after stopped." }
                                                                    },
                                    DialogMsgType.Alert);

                        args.Handled = true;
                    }
                }
            }
        });

        SilinceCommand = new RelayCommand(_ => BeepSilince = true);

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

        AddLotCommand = new RelayCommand(_ =>
        {
            if (InputQuantity <= 0 || string.IsNullOrEmpty(InputPartID) || string.IsNullOrEmpty(InputLotID))
            {
                return;
            }

            OvenInfo.OperatorID = InputOperatorID;
            AddLOT(InputLotID, InputPartID, InputLayer, InputQuantity);

            var unit     = InputQuantity > 1 ? "pcs" : "pc";
            var eventval = (EventType.Operator, DateTime.Now, nameof(AddLotCommand), "", $"{InputLotID}-{InputPartID}-{InputLayer}-{InputQuantity}{unit}");
            EventHappened?.Invoke(eventval);

            ClearInput2();
        });

        DeleteLotCommand = new RelayCommand(lot =>
        {
            if (lot is ProductInfo info)
            {
                using var list = OvenInfo.TempProducts.ToPooledList();
                list.Remove(info);

                InvokeSECSEvent?.Invoke("LotRemoved");

                var unit     = info.Quantity > 1 ? "pcs" : "pc";
                var eventval = (EventType.Operator, DateTime.Now, nameof(DeleteLotCommand), "", $"{info.LotID}-{info.PartID}-{info.Layer}-{info.Quantity}{unit}");
                EventHappened?.Invoke(eventval);

                ClearInput();
                list.ForEach(x => OvenInfo.TempProducts.Add(x));
            }
        });

        //GetRecipeCommand = new RelayCommand(_ =>
        //{
        //    NotifyPropertyChanged(nameof(PPNameList));
        //    NotifyPropertyChanged(nameof(LocalRecipe));
        //    //TopLocalRecipe = PPNameList.
        //});
        #region 註冊PLC事件
        //! 只有有註冊PLC點位的值變事件
        ValueChanged += (LogType, data) =>
                        {
                            var (name, value, oldvalue, type, Subscriptions, SubPosition) = data;

                            var nowtime = DateTime.Now;

                            if (LogType == LogType.StatusVariables)
                            {
                                var eventval = (EventType.StatusChanged, nowtime, name, $"{type!}{Subscriptions!.First()}{(SubPosition > -1 ? $"-{SubPosition:X}" : string.Empty)}", value);

                                SV_Changed?.Invoke(name, value!);

                                if (value is bool val)
                                {
                                    EventHappened?.Invoke(eventval!);
                                }
                                else if (value is short sv)
                                {

                                }
                            }
                            else if (LogType == LogType.Alert)
                            {
                                var eventval = (EventType.Alert, nowtime, name, $"{(BitType)type!}{Subscriptions!.First()}{(SubPosition > -1 ? $"-{SubPosition:X}" : string.Empty)}", value);
                                EventHappened?.Invoke(eventval!);

                                if (value is bool boolval)
                                {
                                }
                            }
                            else if (LogType == LogType.Alarm)
                            {
                                var eventval = (EventType.Alarm, nowtime, name, $"{(BitType)type!}{Subscriptions!.First()}{(SubPosition > -1 ? $"-{SubPosition:X}" : string.Empty)}", value);
                                EventHappened?.Invoke(eventval!);

                                if (value is bool boolval)
                                {
                                }
                            }
                            else if (LogType == LogType.RecipeSet) //PLC配方"設定值"改變時
                            {
                                RecipeItem = InitalItems(plcindex);
                                TemperatureItems = InitalTemperatureItem(plcindex);
                                SV_Changed?.Invoke(name, value!);
                            }
                            else if (LogType == LogType.Trigger)
                            {
                            }
                            else if (LogType == LogType.CustomData)
                            {
                                if (value is bool val)
                                {
                                    if (val)
                                    {
                                        PanelMoveHappened?.Invoke((plcindex, name));
                                        if (name == nameof(BackWeightToOven))
                                        {
                                            OvenInfo.CoaterAfterCoaterWeight = 0;
                                            OvenInfo.CoaterEmptyPanelWeight = 0;
                                            OvenInfo.CoaterOilWeight = 0;
                                            OvenInfo.Station = plcindex + 1;
                                            _ = ExecutingFinished?.Invoke(OvenInfo.Copy()!);
                                        }
                                    }
                                }
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
        RecipeItem = InitalItems(plcindex);
        TemperatureItems = InitalTemperatureItem(plcindex);
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
        var errs = await ManualSetByPropertiesWithCheck(recipe.ToDictionary()).ConfigureAwait(false);

        var result = errs.Count == 0 ? SetRecipeResult.成功 : SetRecipeResult.比對不相符;
        if (result == SetRecipeResult.成功)
        {
            AutoMode = true;
        }
        return result;
    }

    //多顆PLC配方寫入
    public async Task<SetRecipeResult> WriteRecipeToPlcAsync(PLC_Recipe recipe, int plcindex)
    {
        var errs = await ManualSetByPropertiesWithCheck(recipe.ToDictionary(plcindex)).ConfigureAwait(false);

        var result = errs.Count == 0 ? SetRecipeResult.成功 : SetRecipeResult.比對不相符;
        if (result == SetRecipeResult.成功)
        {
            AutoMode = true;
        }
        return result;
    }
    public PLC_Recipe GetRecipeCoater() => new PLC_Recipe
    {
        RecipeName = RecipeName,
    };

    public async Task<SetRecipeResult> SetRecipeAsync(PLC_Recipe? recipe)
    {
        if (await WriteRecipeToPlcAsync(recipe).ConfigureAwait(false) == SetRecipeResult.比對不相符)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, $"Coater{PLCIndex+1} 配方切換失敗" },
                            { Language.CHS, $"Coater{PLCIndex+1} 配方切换失敗" }
                        });
            return SetRecipeResult.比對不相符;
        }
        Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, $"Coater{PLCIndex+1} 配方切換完成" },
                            { Language.CHS, $"Coater{PLCIndex+1} 配方切換完成"}
                        });
        return SetRecipeResult.成功;
    }
    //private bool RecipeCompare(PLC_Recipe recipe) => NitrogenMode == recipe.NitrogenMode &&
    //                                                OxygenContentSet.ToString("0.0") == recipe.OxygenContentSet.ToString("0.0") &&
    //                                                (RecipeName.Length > 16 ? RecipeName[..16] : RecipeName) == (recipe.RecipeName.Length > 16 ? recipe.RecipeName[..16] : recipe.RecipeName) && //! 只最多比對16個字(PLC的配方名長度)
    //                                                DwellTime_1.ToString("0.0") == recipe.DwellTime_1.ToString("0.0") &&
    //                                                DwellTime_2.ToString("0.0") == recipe.DwellTime_2.ToString("0.0") &&
    //                                                DwellTime_3.ToString("0.0") == recipe.DwellTime_3.ToString("0.0") &&
    //                                                DwellTime_4.ToString("0.0") == recipe.DwellTime_4.ToString("0.0") &&
    //                                                DwellTime_5.ToString("0.0") == recipe.DwellTime_5.ToString("0.0") &&
    //                                                DwellTime_6.ToString("0.0") == recipe.DwellTime_6.ToString("0.0") &&
    //                                                DwellAlarm_1.ToString("0.0") == recipe.DwellAlarm_1.ToString("0.0") &&
    //                                                DwellAlarm_2.ToString("0.0") == recipe.DwellAlarm_2.ToString("0.0") &&
    //                                                DwellAlarm_3.ToString("0.0") == recipe.DwellAlarm_3.ToString("0.0") &&
    //                                                DwellAlarm_4.ToString("0.0") == recipe.DwellAlarm_4.ToString("0.0") &&
    //                                                DwellAlarm_5.ToString("0.0") == recipe.DwellAlarm_5.ToString("0.0") &&
    //                                                DwellAlarm_6.ToString("0.0") == recipe.DwellAlarm_6.ToString("0.0") &&
    //                                                CoolingTime.ToString("0.0") == recipe.CoolingTime.ToString("0.0") &&
    //                                                CoolingTemperature.ToString("0.0") == recipe.CoolingTemperature.ToString("0.0") &&
    //                                                RampTime_1.ToString("0.0") == recipe.RampTime_1.ToString("0.0") &&
    //                                                RampTime_2.ToString("0.0") == recipe.RampTime_2.ToString("0.0") &&
    //                                                RampTime_3.ToString("0.0") == recipe.RampTime_3.ToString("0.0") &&
    //                                                RampTime_4.ToString("0.0") == recipe.RampTime_4.ToString("0.0") &&
    //                                                RampTime_5.ToString("0.0") == recipe.RampTime_5.ToString("0.0") &&
    //                                                RampTime_6.ToString("0.0") == recipe.RampTime_6.ToString("0.0") &&
    //                                                RampAlarm_1.ToString("0.0") == recipe.RampAlarm_1.ToString("0.0") &&
    //                                                RampAlarm_2.ToString("0.0") == recipe.RampAlarm_2.ToString("0.0") &&
    //                                                RampAlarm_3.ToString("0.0") == recipe.RampAlarm_3.ToString("0.0") &&
    //                                                RampAlarm_4.ToString("0.0") == recipe.RampAlarm_4.ToString("0.0") &&
    //                                                RampAlarm_5.ToString("0.0") == recipe.RampAlarm_5.ToString("0.0") &&
    //                                                RampAlarm_6.ToString("0.0") == recipe.RampAlarm_6.ToString("0.0") &&
    //                                                InflatingTime.ToString("0") == recipe.InflatingTime.ToString("0") &&
    //                                                TemperatureSetpoint_1.ToString("0.0") == recipe.TemperatureSetpoint_1.ToString("0.0") &&
    //                                                TemperatureSetpoint_2.ToString("0.0") == recipe.TemperatureSetpoint_2.ToString("0.0") &&
    //                                                TemperatureSetpoint_3.ToString("0.0") == recipe.TemperatureSetpoint_3.ToString("0.0") &&
    //                                                TemperatureSetpoint_4.ToString("0.0") == recipe.TemperatureSetpoint_4.ToString("0.0") &&
    //                                                TemperatureSetpoint_5.ToString("0.0") == recipe.TemperatureSetpoint_5.ToString("0.0") &&
    //                                                TemperatureSetpoint_6.ToString("0.0") == recipe.TemperatureSetpoint_6.ToString("0.0") &&
    //                                                SegmentCounts == recipe.SegmentCounts;
    public class Item : ObservableObject
    {
        public string RecipeDESC
        {
            get => Get<string>();
            set => Set(value);
        }

        public string RecipeValue
        {
            get => Get<string>();
            set => Set(value);
        }
    }
    public class TemperatureItem : ObservableObject
    {
        public string TemperatureDESC
        {
            get => Get<string>();
            set => Set(value);
        }

        public string TemperatureValue
        {
            get => Get<string>();
            set => Set(value);
        }
    }

    /// <summary>
    /// 資料蒐集 顯示UI
    /// </summary>
    /// <param name="plcindex"></param>
    /// <returns></returns>
    private ObservableCollection<Item> InitalItems(int plcindex)
    {
        var items = new ObservableCollection<Item>();

        //三台PLC配方點位都是一樣，但客戶某幾段沒有要全看
        var recipeData = new Dictionary<string, string>
        {
            { "塗佈使用", UseCoating == 0 ? "使用" : "不使用"  },
            { "塞孔使用", UsePlug == 0 ? "使用" : "不使用"  },
            { "塗佈次數", Coatingoftimes.ToString() },
            { "塞孔次數設定", Plugoftimes.ToString() },
            { "塗佈速度設定", CoatingSpeedSetting.ToString() },
            { "塞孔刮刀壓力設定", Blade_Pressure.ToString() },
            { "塗佈壓力設定", CoatingPressureSetting.ToString() },
            { "基板厚度設定", PanelThicknessSetting.ToString() },
            { "入料下降位置設定", LocationOfDrop.ToString() },
            { "板面夾持距離設定", BoardClampingDistance.ToString() },
            { "左前D.BAR壓力設定", D_BarPressureSetting1.ToString() },
            { "右前D.BAR壓力設定", D_BarPressureSetting2.ToString() },
            { "左後D.BAR壓力設定", D_BarPressureSetting3.ToString() },
            { "右後D.BAR壓力設定", D_BarPressureSetting4.ToString() },
            { "標準墨重", StandardInk.ToString() },
            { "墨重誤差值", DifferenceOfInk.ToString() },
            { "烘烤時間設定", BakingTimeSetting.ToString() },
            { "第1段溫度設定值", TemperatureSV1.ToString() },
            { "第2段溫度設定值", TemperatureSV2.ToString() },
        };
        if (plcindex == 0)
        {
            foreach (var kvp in recipeData)
            {
                items.Add(new Item { RecipeDESC = kvp.Key, RecipeValue = kvp.Value.ToString() });
            }
        }
        else if (plcindex == 1)
        {
            foreach (var kvp in recipeData)
            {
                items.Add(new Item { RecipeDESC = kvp.Key, RecipeValue = kvp.Value.ToString() });
            }
        }
        else if (plcindex == 2)
        {
            foreach (var kvp in recipeData)
            {
                items.Add(new Item { RecipeDESC = kvp.Key, RecipeValue = kvp.Value.ToString() });
            }
            items.Add(new Item { RecipeDESC = "第3段溫度設定值", RecipeValue = TemperatureSV3.ToString() });
            items.Add(new Item { RecipeDESC = "第4段溫度設定值", RecipeValue = TemperatureSV4.ToString() });
            items.Add(new Item { RecipeDESC = "第5段溫度設定值", RecipeValue = TemperatureSV5.ToString() });

        }
        NotifyPropertyChanged(nameof(RecipeItem));
        return items;
    }
    private ObservableCollection<TemperatureItem> InitalTemperatureItem(int plcindex)
    {
        var items = new ObservableCollection<TemperatureItem>();
        var TemperatureData = new Dictionary<string, double>
        {
            { "第1段溫度實際值", TemperaturePV1 },
            { "第2段溫度實際值", TemperaturePV2 },
        };
        if (plcindex == 0)
        {
            foreach (var kvp in TemperatureData)
            {
                items.Add(new TemperatureItem { TemperatureDESC = kvp.Key, TemperatureValue = kvp.Value.ToString() });
            }
        }
        else if (plcindex == 1)
        {
            foreach (var kvp in TemperatureData)
            {
                items.Add(new TemperatureItem { TemperatureDESC = kvp.Key, TemperatureValue = kvp.Value.ToString() });
            }
        }
        else if (plcindex == 2)
        {
            foreach (var kvp in TemperatureData)
            {
                items.Add(new TemperatureItem { TemperatureDESC = kvp.Key, TemperatureValue = kvp.Value.ToString() });
            }
            items.Add(new TemperatureItem { TemperatureDESC = "第3段溫度實際值", TemperatureValue = TemperaturePV3.ToString() });
            items.Add(new TemperatureItem { TemperatureDESC = "第4段溫度實際值", TemperatureValue = TemperaturePV4.ToString() });
            items.Add(new TemperatureItem { TemperatureDESC = "第5段溫度實際值", TemperatureValue = TemperaturePV5.ToString() });
        }
        NotifyPropertyChanged(nameof(TemperatureItems));
        return items;
    }
    public void ClearInput()
    {
        OvenInfo.TempProducts.Clear();
        Set(string.Empty, nameof(InputOperatorID));
        ClearInput2();
    }
    public void ClearInput2()
    {
        inputFocusTB = null;
        Set(string.Empty, nameof(InputPartID));
        Set(string.Empty, nameof(InputLotID));
        Set(string.Empty, nameof(InputRecipeName));
        Set(InputQuantityMin, nameof(InputQuantity));
        Set(InputLayerMin, nameof(InputLayer));
    }
    public void AddLOT(string lotid, string partid, int layer, int quantity)
    {
        if (OvenInfo.TempProducts.FirstOrDefault(x => x.PartID == partid.Trim() && x.LotID == lotid.Trim() && x.Layer == layer) is { } product)
        {
            product.Quantity = quantity;
            Quantity = (short)product.Quantity;
        }
        else
        {
            var info = new ProductInfo
            {
                PartID   = partid.Trim(),
                LotID    = lotid.Trim(),
                Layer    = layer,
                Quantity = quantity
            };

            PartID = info.PartID;
            LotID = info.LotID;
            Quantity = (short)info.Quantity;

            OvenInfo.TempProducts.Add(info);
        }

        InvokeSECSEvent?.Invoke("LotAdded");
    }
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

    private async Task<SetRecipeResult> SetRecipeDialogAsync(string recipeName, int plcindex)
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

        if (!RemoteMode)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW,  $"Coater{plcindex}未在Remote模式" },
                            { Language.CHS, $"Coater{plcindex}未在Remote模式" },
                            { Language.EN,  $"Coater{plcindex} is not in Remote Mode" }
                        });

            return SetRecipeResult.條件不允許;
        }
        var a =  await WriteRecipeToPlcAsync(recipe, plcindex).ConfigureAwait(false);
        return await WriteRecipeToPlcAsync(recipe, plcindex).ConfigureAwait(false);
    }

    #region Interface Implementations
    public void Dispose() => Dispose();
    #endregion

}