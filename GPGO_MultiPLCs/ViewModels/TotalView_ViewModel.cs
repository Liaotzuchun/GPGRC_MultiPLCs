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
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.Models.SECS;
using GPMVVM.Models.SECS.ITRISecs;
using GPMVVM.SECSGEM;
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
    public event Func<List<string>>?                                                                               GetRecipeList;
    public event Action<string>?                                                                               CheckRecipeCommand_KeyIn;

    /// <summary>財產編號儲存位置</summary>
    private const string AssetNumbersPath = "AssetNumbers";

    /// <summary>設備碼儲存位置</summary>
    private const string MachineCodesPath = "MachineCodes";
    private readonly IDialogService? Dialog;

    /// <summary>保持PLC Gate連線</summary>
    private readonly Timer Checker;
    private readonly AsyncOperation? asyncOperation;
    private readonly int             threadid;
    public GRC_SecsGem SecsGemEquipment { get; }
    public Language Language = Language.TW;
    public IGate Gate { get; }
    public ObservableConcurrentQueue<LogEvent> QueueMessages { get; } = new();

    /// <summary>回到總覽頁</summary>
    public RelayCommand BackCommand { get; }
    public RelayCommand TestPanelCommand { get; }
    public RelayCommand InitialPanelCommand { get; }
    public RelayCommand GoDetailCommand { get; }
    public RelayCommand LoadedCommand { get; }
    public AsyncCommand SendTerminalMessageCommand { get; }
    public RelayCommand SecsReStartCommand { get; }
    public RelayCommand GetRecipeCommand { get; }
    public RelayCommand CheckRecipeCommand { get; }

    public List<string> PPNameList
    {
        get => GetRecipeList?.Invoke();
    }
    public string LocalRecipe
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public List<CoaterItem> Coater1Panel
    {
        get => Get<List<CoaterItem>>();
        set => Set(value);
    }
    public List<CoaterItem> Coater2Panel
    {
        get => Get<List<CoaterItem>>();
        set => Set(value);
    }
    public List<CoaterItem> Coater3Panel
    {
        get => Get<List<CoaterItem>>();
        set => Set(value);
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
    public bool[] PLCIsBusy { get; }
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
    public bool SECS_ENABLE
    {
        get => Get<bool>();
        set
        {
            if (!SecsGemEquipment.Enable(value))
            {
                Dialog?.Show(value ?
                                 new Dictionary<Language, string>
                                 {
                                     { Language.TW, "無法啟用連線" },
                                     { Language.CHS, "无法启用联机" },
                                     { Language.EN, "Unable to enable connection" }
                                 } :
                                 new Dictionary<Language, string>
                                 {
                                     { Language.TW, "無法中止連線" },
                                     { Language.CHS, "无法中止联机" },
                                     { Language.EN, "Unable to disable connection" }
                                 });
            }
        }
    }

    public bool SECS_Communicating
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SECS_ONLINE
    {
        get => Get<bool>();
        set
        {
            if (!SECS_ENABLE)
            {
                return;
            }

            SecsGemEquipment.Online(value);
        }
    }

    public bool SECS_REMOTE
    {
        get => Get<bool>();
        set
        {
            if (!SECS_ENABLE || !SECS_ONLINE)
            {
                return;
            }

            SecsGemEquipment.Remote(value);
        }
    }

    public TotalView_ViewModel(int count, IGate gate, IPAddress plcaddress, IDialogService dialog)
    {
        asyncOperation = AsyncOperationManager.CreateOperation(null);
        Gate = gate;
        Dialog = dialog;
        OvenCount = count;
        PLC_All = new PLC_ViewModel[count];
        PLCIndex = 0;
        var v = Assembly.GetExecutingAssembly().GetName().Version;
        SecsGemEquipment = new GRC_SecsGem("0", "GPGRC", $"{v.Major}.{v.Minor}.{v.Build}");
        SecsGemEquipment.HSMSParameters.RegisterChanged();

        SecsGemEquipment.TerminalMessageRecived += async message =>
        {
            if (Dialog == null)
            {
                return;
            }

            var eventval = (-1, EventType.SECSCommand, DateTime.Now, nameof(SECSGEM.TerminalMessageRecived), "", message);
            EventHappened?.Invoke(eventval);

            if (await dialog.Show(new Dictionary<Language, string>
                                                                             {
                                                                                 { Language.TW, $"{DateTime.Now:M/d HH:mm:ss} 終端訊息：\n{message}" },
                                                                                 { Language.CHS, $"{DateTime.Now:M/d HH:mm:ss} 终端讯息：\n{message}" },
                                                                                 { Language.EN, $"{DateTime.Now:M/d HH:mm:ss} TerminalMessage：\n{message}" }
                                                                             },
                                  false,
                                  TimeSpan.FromDays(1),
                                  DialogMsgType.Alert))
            {
                SecsGemEquipment.TerminalMessageConfirm();

                var (result1, input1) = await dialog.ShowWithInput(new Dictionary<Language, string>
                                                                                                              {
                                                                                                                  { Language.TW, "欲回覆之訊息：" },
                                                                                                                  { Language.CHS, "欲回复之讯息：" },
                                                                                                                  { Language.EN, "Please enter the message you want to reply：" }
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
                    SecsGemEquipment.SendTerminalMessage(msg);
                }
            }
        };

        SecsGemEquipment.ECChange += _ =>
        {
        };

        SecsGemEquipment.UpsertUnformattedPP += e =>
        {
            var (ppid, ppbody) = e;
            var eventval = (-1, EventType.SECSCommand, DateTime.Now, nameof(SECSGEM.UpsertUnformattedPP), "", ppid);
            EventHappened?.Invoke(eventval);
            if (ppbody.JsonToObject<PLC_Recipe>() is { } recipe)
            {
                recipe.RecipeName = ppid;
                recipe.Editor = "SECSGEM-HOST";
                recipe.EditorLevel = UserLevel.Manager;

                return UpsertRecipe != null && UpsertRecipe.Invoke(recipe);
            }

            return false;
        };

        SecsGemEquipment.UpsertFormattedPP += e =>
        {
            var (ppid, _, recipedic) = e;
            var recipe   = new PLC_Recipe(ppid, "SECSGEM-HOST", UserLevel.Manager);
            var eventval = (-1, EventType.SECSCommand, DateTime.Now, nameof(SECSGEM.UpsertFormattedPP), "", recipe.RecipeName);
            EventHappened?.Invoke(eventval);

            return recipe.SetByDictionary(recipedic) && UpsertRecipe != null && UpsertRecipe.Invoke(recipe);
        };

        SecsGemEquipment.DeletePP += recipeName =>
        {
            var eventval = (-1, EventType.SECSCommand, DateTime.Now, nameof(SECSGEM.DeletePP), "", recipeName);
            EventHappened?.Invoke(eventval);

            return DeleteRecipe != null && DeleteRecipe.Invoke(recipeName);
        };

        SecsGemEquipment.START_Command += index =>
        {
            if (!SECS_REMOTE)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "不在SECS_REMOTE", index);
                EventHappened?.Invoke(eventval);
                return HCACKValule.CantPerform;
            }

            if (index >= PLC_All.Count)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "Index超過PLC總數", index);
                EventHappened?.Invoke(eventval);
                return HCACKValule.ParameterInvalid;
            }

            if (PLCIsBusy[index])
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "正在執行其他Command", index);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, "START: 正在執行其他Command" },
                                                                  { Language.CHS, "START: 正在执行其他Command" },
                                                                  { Language.EN, "START: Other command is being executed" }
                                                              },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            var plc = PLC_All[index];

            if (!Gate.GateStatus.CurrentValue || !plc.ConnectionStatus.CurrentValue)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "PLC離線", index);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, "START: PLC離線" },
                                                                  { Language.CHS, "START: PLC脱机" },
                                                                  { Language.EN, "START: PLC is offline." }
                                                              },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            if (plc.IsExecuting)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "仍在烘烤中", index);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, "START: 仍在烘烤中" },
                                                                  { Language.CHS, "START: 仍在烘烤中" },
                                                                  { Language.EN, "START: Oven is executing." }
                                                              },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            if (plc.EmergencyStop || plc.PowerPhaseError || plc.OTPTemperatureError || plc.CirculatingFanCurrentError || plc.ELBtrip)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "烤箱狀態異常", index);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, "START: 烤箱狀態異常" },
                                                                  { Language.CHS, "START: 烤箱状态异常" },
                                                                  { Language.EN, "START: Oven status abnormal." }
                                                              },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            if (plc.DoorNotOpen)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "停止後未開門", index);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, "START: 停止後未開門" },
                                                                  { Language.CHS, "START: 停止后未开门" },
                                                                  { Language.EN, "START: The door did not open after stopped." }
                                                              },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            PLCIsBusy[index] = true;

            if (!plc.AutoMode && plc.ManualSetByPropertiesWithCheck(new Dictionary<string, object> { { nameof(PLC_ViewModel.AutoMode), true } }).Result.Count > 0)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "無法切換自動模式", index);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, "START: 無法切換自動模式" },
                                                                  { Language.CHS, "START: 无法切换自动模式" },
                                                                  { Language.EN, "START: Unable switch to AutoMode." }
                                                              },
                            DialogMsgType.Alert);

                PLCIsBusy[index] = false;
                return HCACKValule.CantPerform;
            }

            if (plc.ManualSetByPropertiesWithCheck(new Dictionary<string, object> { { nameof(PLC_ViewModel.AutoMode_Start), true } }).Result.Count > 0)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "AutoMode_Start設定失敗", index);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, "START: AutoMode_Start設定失敗" },
                                                                  { Language.CHS, "START: AutoMode_Start设定失败" },
                                                                  { Language.EN, "START: Set \"AutoMode_Start\" failed." }
                                                              },
                            DialogMsgType.Alert);

                PLCIsBusy[index] = false;
                return HCACKValule.CantPerform;
            }

            var eventval1 = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.START_Command), "啟動成功", index);
            EventHappened?.Invoke(eventval1);

            PLCIsBusy[index] = false;
            return HCACKValule.Acknowledge;
        };

        SecsGemEquipment.STOP_Command += index =>
        {
            if (!SECS_REMOTE)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.STOP_Command), "不在SECS_REMOTE", index);
                EventHappened?.Invoke(eventval);
                return HCACKValule.CantPerform;
            }

            if (index >= PLC_All.Count)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.STOP_Command), "Index超過PLC總數", index);
                EventHappened?.Invoke(eventval);
                return HCACKValule.ParameterInvalid;
            }

            if (!Gate.GateStatus.CurrentValue || !PLC_All[index].ConnectionStatus.CurrentValue)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.STOP_Command), "PLC離線", index);
                EventHappened?.Invoke(eventval);
                return HCACKValule.CantPerform;
            }

            if (!PLC_All[index].ProcessComplete)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.STOP_Command), "烘烤程序未完成", index);
                EventHappened?.Invoke(eventval);
                return HCACKValule.CantPerform;
            }

            var eventval1 = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.STOP_Command), "", index);
            EventHappened?.Invoke(eventval1);
            PLC_All[index].AutoMode_Start = false;

            return HCACKValule.Acknowledge;
        };

        SecsGemEquipment.PPSELECT_Command += (index, name) =>
        {
            var msg = $"{index}:{name}";

            if (!SECS_REMOTE)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.PPSELECT_Command), "不在SECS_REMOTE", msg);
                EventHappened?.Invoke(eventval);
                return HCACKValule.CantPerform;
            }

            if (index >= PLC_All.Count)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.PPSELECT_Command), "Index超過PLC總數", msg);
                EventHappened?.Invoke(eventval);
                return HCACKValule.ParameterInvalid;
            }

            if (PLCIsBusy[index])
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.PPSELECT_Command), "正在執行其他Command", msg);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                                 {
                                                                     { Language.TW, "PPSELECT: 正在執行其他Command" },
                                                                     { Language.CHS, "PPSELECT: 正在执行其他Command" },
                                                                     { Language.EN, "PPSELECT: Other command is being executed" }
                                                                 },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            var plc = PLC_All[index];

            if (!Gate.GateStatus.CurrentValue || !plc.ConnectionStatus.CurrentValue)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.PPSELECT_Command), "PLC離線", msg);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                                 {
                                                                     { Language.TW, "PPSELECT: PLC離線" },
                                                                     { Language.CHS, "PPSELECT: PLC脱机" },
                                                                     { Language.EN, "PPSELECT: PLC is offline." }
                                                                 },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            if (plc.IsExecuting)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.PPSELECT_Command), "仍在烘烤中", msg);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                                 {
                                                                     { Language.TW, "PPSELECT: 仍在烘烤中" },
                                                                     { Language.CHS, "PPSELECT: 仍在烘烤中" },
                                                                     { Language.EN, "PPSELECT: Oven is executing." }
                                                                 },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            PLCIsBusy[index] = true;

            if (GetRecipe?.Invoke(name) is not { } recipe)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.PPSELECT_Command), "配方不存在", msg);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                                 {
                                                                     { Language.TW, "PPSELECT: 配方不存在" },
                                                                     { Language.CHS, "PPSELECT: 配方不存在" },
                                                                     { Language.EN, "PPSELECT: The PP does not exist." }
                                                                 },
                            DialogMsgType.Alert);

                PLCIsBusy[Index] = false;
                return HCACKValule.NoObjectExists;
            }

            var eventval1 = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.PPSELECT_Command), "", msg);
            EventHappened?.Invoke(eventval1);

            var result = plc.WriteRecipeToPlcAsync(recipe).Result;

            PLCIsBusy[Index] = false;
            return result == SetRecipeResult.成功 ? HCACKValule.Acknowledge : HCACKValule.CantPerform;
        };

        SecsGemEquipment.ADDLOT_Command += (index, lot) =>
        {
            var (lotID, partID, layer, quantity) = lot;
            var unit = quantity > 1 ? "pcs" : "pc";
            var msg  = $"{index}:{lotID}-{partID}-{layer}-{quantity}{unit}";

            if (!SECS_REMOTE)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.ADDLOT_Command), "不在SECS_REMOTE", msg);
                EventHappened?.Invoke(eventval);
                return HCACKValule.CantPerform;
            }

            if (index >= PLC_All.Count || lot is { layer: <= 0 or > 8 } or { quantity: <= 0 })
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.ADDLOT_Command), "參數不正確", msg);
                EventHappened?.Invoke(eventval);
                return HCACKValule.ParameterInvalid;
            }

            if (!Gate.GateStatus.CurrentValue || !PLC_All[index].ConnectionStatus.CurrentValue)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.ADDLOT_Command), "PLC離線", msg);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                               {
                                                                   { Language.TW, "ADDLOT: PLC離線" },
                                                                   { Language.CHS, "ADDLOT: PLC脱机" },
                                                                   { Language.EN, "ADDLOT: PLC is offline." }
                                                               },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            if (PLC_All[index].IsExecuting)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.ADDLOT_Command), "仍在烘烤中", msg);
                EventHappened?.Invoke(eventval);

                dialog.Show(new Dictionary<Language, string>
                                                               {
                                                                   { Language.TW, "ADDLOT: 仍在烘烤中" },
                                                                   { Language.CHS, "ADDLOT: 仍在烘烤中" },
                                                                   { Language.EN, "ADDLOT: Oven is executing." }
                                                               },
                            DialogMsgType.Alert);

                return HCACKValule.CantPerform;
            }

            PLC_All[index].AddLOT(lotID, partID, layer, quantity);
            var eventval1 = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.ADDLOT_Command), "", msg);
            EventHappened?.Invoke(eventval1);

            SecsGemEquipment.InvokeEvent($"Oven{index + 1}_LotAdded");
            return HCACKValule.Acknowledge;
        };

        SecsGemEquipment.CANCEL_Command += index =>
        {
            if (!SECS_REMOTE)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.CANCEL_Command), "不在SECS_REMOTE", index);
                EventHappened?.Invoke(eventval);
                return HCACKValule.CantPerform;
            }

            if (index >= PLC_All.Count)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.CANCEL_Command), "Index超過PLC總數", index);
                EventHappened?.Invoke(eventval);
                return HCACKValule.ParameterInvalid;
            }

            if (PLC_All[index].IsExecuting)
            {
                var eventval = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.CANCEL_Command), "仍在烘烤中", index);
                EventHappened?.Invoke(eventval);
                return HCACKValule.CantPerform;
            }

            var eventval1 = (index, EventType.SECSCommand, DateTime.Now, nameof(GRC_SecsGem.CANCEL_Command), "", index);
            EventHappened?.Invoke(eventval1);

            PLC_All[index].ClearInput();
            SecsGemEquipment.InvokeEvent($"Oven{index + 1}_LotRemoved");
            SecsGemEquipment.InvokeEvent($"Oven{index + 1}_CancelCheckIn");
            return HCACKValule.Acknowledge;
        };

        SecsGemEquipment.CommEnable_Changed += boolval =>
        {
            if (SECS_ENABLE == boolval)
            {
                return; //! 確定值變
            }
            Set(boolval, nameof(SECS_ENABLE)); //! 避免直接設定值觸發動作（直接設定值是給OP操作界面用的）

            var eventval = (-1, EventType.StatusChanged, DateTime.Now, nameof(SECS_ENABLE), "", boolval);
            EventHappened?.Invoke(eventval);
        };

        SecsGemEquipment.Communicating_Changed += boolval =>
        {
            if (SECS_Communicating == boolval)
            {
                return; //! 確定值變
            }
            Set(boolval, nameof(SECS_Communicating)); //! 避免直接設定值觸發動作（直接設定值是給OP操作界面用的）

            var eventval = (-1, EventType.StatusChanged, DateTime.Now, nameof(SECS_Communicating), "", boolval);
            EventHappened?.Invoke(eventval);
        };

        SecsGemEquipment.ONLINE_Changed += online =>
        {
            if (SECS_ONLINE == online)
            {
                return; //! 確定值變
            }
            Set(online, nameof(SECS_ONLINE)); //! 避免直接設定值觸發動作（直接設定值是給OP操作界面用的）

            var eventval = (-1, EventType.StatusChanged, DateTime.Now, nameof(SECS_ONLINE), "", online);
            EventHappened?.Invoke(eventval);
        };

        SecsGemEquipment.GO_Local += () =>
        {
            if (!SECS_REMOTE)
            {
                return; //! 確定值變
            }
            Set(false, nameof(SECS_REMOTE)); //! 避免直接設定值觸發動作（直接設定值是給OP操作界面用的）
            foreach (var plc in PLC_All)
            {
                plc.RemoteMode = true;
            }

            var eventval = (-1, EventType.StatusChanged, DateTime.Now, "SECS_LOCAL", "", true);
            EventHappened?.Invoke(eventval);
        };

        SecsGemEquipment.GO_Remote += () =>
        {
            if (SECS_REMOTE)
            {
                return; //! 確定值變
            }
            Set(true, nameof(SECS_REMOTE)); //! 避免直接設定值觸發動作（直接設定值是給OP操作界面用的）
            foreach (var plc in PLC_All)
            {
                plc.RemoteMode = true;
            }

            var eventval = (-1, EventType.StatusChanged, DateTime.Now, nameof(SECS_REMOTE), "", true);
            EventHappened?.Invoke(eventval);
        };

        Mode = 0;
        EqpState = 1;
        InitialStringItem();
        Status = -1;
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

        GetRecipeCommand = new RelayCommand(_ =>
        {
            NotifyPropertyChanged(nameof(PPNameList));
            NotifyPropertyChanged(nameof(LocalRecipe));
        });
        CheckRecipeCommand = new RelayCommand(_ =>
        {
            CheckRecipeCommand_KeyIn?.Invoke(LocalRecipe);
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
                SecsGemEquipment.SendTerminalMessage(msg);
            }
        },
                                                     null);

        SecsReStartCommand = new RelayCommand(_ =>
        {
            SecsGemEquipment.ReStartSecsgem();
            SecsGemEquipment.Enable(true);
            SecsGemEquipment.Online(true);
        });

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(SECS_ENABLE) or nameof(SECS_Communicating) or nameof(SECS_ONLINE) or nameof(SECS_REMOTE))
            {
                var val  = SECS_ENABLE && SECS_Communicating && SECS_ONLINE;
                var val2 = val         && SECS_REMOTE;
                foreach (var plc in PLC_All)
                {
                    plc.SecsIsOnline = val;
                    plc.SecsIsRemoteOnline = val2;
                }
            }
        };
        //var address = plcaddress.GetAddressBytes();

        //! 註冊PLC事件需引發的動作
        for (var i = 0; i < count; i++)
        {
            var plc = new PLC_ViewModel(dialog,
                                        Gate,
                                        i,
                                        "GRC",
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
                SecsGemEquipment.UpdateSV($"Coater{index + 1}_OperatorID", opid);
                SecsGemEquipment.UpdateSV($"Coater{index + 1}_RackID", rackid);
                SecsGemEquipment.InvokeEvent($"Coater{index + 1}_RackInput");
            };

            //! 取消投產
            plc.CancelCheckIn += _ =>
            {
                SecsGemEquipment.UpdateSV($"Coater{index + 1}_RackID", string.Empty);
                SecsGemEquipment.InvokeEvent($"Coater{index + 1}_CancelCheckIn");
            };

            plc.CheckOut += _ => SecsGemEquipment.InvokeEvent($"Coater{index + 1}_RackOutput");


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

                                    };


            //! 烘烤流程結束時
            plc.ExecutingFinished += async baseInfo =>
                                     {
                                         var product = new ProcessInfo(baseInfo);

                                         try
                                         {
                                             SecsGemEquipment.UpdateDV($"Coater{index + 1}_ProcessData", baseInfo.ToJson());
                                         }
                                         catch
                                         {
                                             // ignored
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

            plc.InvokeSECSEvent += EventName => SecsGemEquipment.InvokeEvent($"Oven{index + 1}_{EventName}");

            plc.InvokeSECSAlarm += (AlarmName, val) => SecsGemEquipment.InvokeAlarm($"Oven{index + 1}_{AlarmName}", val);

            plc.SV_Changed += (name, value) =>
            {
                if (name == nameof(PLC_ViewModel.EquipmentState))
                {
                    SecsGemEquipment.UpdateITRISV(ITRI_SV.GEM_PROCESS_STATE, value);
                }
                else if (name == $"Previous{nameof(PLC_ViewModel.EquipmentState)}")
                {
                    SecsGemEquipment.UpdateITRISV(ITRI_SV.GEM_PREVIOUS_PROCESS_STATE, value);
                }
                else if (name == nameof(PLC_ViewModel.RecipeName))
                {
                    SecsGemEquipment.UpdateITRISV(ITRI_SV.GEM_PP_EXEC_NAME, value);
                }

                SecsGemEquipment.UpdateSV($"Coater{index + 1}_{name}", value);
            };

            plc.PanelMoveHappened += e =>
            {
                var panelIndexMap = new Dictionary<string, int>
                                                   {
                                                       { nameof(plc.FeedInlet), 0 },
                                                       { nameof(plc.FeedToWait), 1 },
                                                       { nameof(plc.WaitToFrontWeight), 2 },
                                                       { nameof(plc.FrontWeightToCoater), 3 },
                                                       { nameof(plc.CoaterToBackWeight), 4 },
                                                   };
                try
                {
                    if (e.Item1 == 0)
                    {
                        if (panelIndexMap.ContainsKey(e.Item2))
                        {
                            var Coaterindex = panelIndexMap[e.Item2];

                            if (Coaterindex == 0)
                            {
                                Coater1Panel[0] = new CoaterItem { Num = 0, PanelName = plc.PanelID };
                            }
                            else
                            {
                                Coater1Panel[Coaterindex] = Coater1Panel[Coaterindex - 1];
                                Coater1Panel[Coaterindex - 1] = null;
                            }
                        }
                        else if (e.Item2 == nameof(plc.BackWeightToOven) && Coater1Panel[4] is not null)
                        {
                            _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                Coater1Items.Add(new CoaterItem { Num = Coater1Items.Count + 1, PanelName = Coater1Panel[4].PanelName });
                                Coater1Panel[4] = null;
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
                                        Coater2Panel[0] = new CoaterItem { Num = 0, PanelName = Coater1Items[0].PanelName };

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
                                Coater2Panel[Coaterindex - 2] = null;
                            }
                        }
                        else if (e.Item2 == nameof(plc.BackWeightToOven) && Coater2Panel[3] is not null)
                        {
                            _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                Coater2Items.Add(new CoaterItem { Num = Coater2Items.Count + 1, PanelName = Coater2Panel[3].PanelName });
                                Coater2Panel[3] = null;
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
                            else if (Coaterindex == 1 && !string.IsNullOrEmpty(Coater2Items[0].PanelName))
                            {
                                _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                                    {
                                        Coater3Panel[0] = new CoaterItem { Num = 0, PanelName = Coater2Items[0].PanelName };

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
                                Coater3Panel[Coaterindex - 2] = null;
                            }
                        }
                        else if (e.Item2 == nameof(plc.BackWeightToOven) && Coater3Panel[3] is not null)
                        {
                            _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                Coater3Items.Add(new CoaterItem { Num = Coater3Items.Count + 1, PanelName = Coater3Panel[3].PanelName });
                                Coater3Panel[3] = null;
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

        SecsGemEquipment.Enable(true);
        SecsGemEquipment.Online(true);

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
                                    foreach (var plc in PLC_All)
                                    {
                                        plc.Check = !plc.Check;
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
        Coater1Items = [];
        Coater2Items = [];
        Coater3Items = [];
        Coater1Panel = [];
        Coater2Panel = [];
        Coater3Panel = [];
        Coater1Panel.Add(null);
        Coater1Panel.Add(null);
        Coater1Panel.Add(null);
        Coater1Panel.Add(null);
        Coater1Panel.Add(null);
        Coater2Panel.Add(null);
        Coater2Panel.Add(null);
        Coater2Panel.Add(null);
        Coater2Panel.Add(null);
        Coater3Panel.Add(null);
        Coater3Panel.Add(null);
        Coater3Panel.Add(null);
        Coater3Panel.Add(null);
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
                        PLC_All[i].OvenInfo.MachineCode = $"Coater{i + 1}";
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
            PLC_All[i].OvenInfo.MachineCode = $"Coater{i + 1}";
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
    public void InvokeRecipe(string name, PPStatus status)
    {
        SecsGemEquipment.UpdateDV("GemPPChangeName", name);
        SecsGemEquipment.UpdateDV("GemPPChangeStatus", (int)status);
        SecsGemEquipment.InvokeEvent("GemProcessProgramChange");
    }
}