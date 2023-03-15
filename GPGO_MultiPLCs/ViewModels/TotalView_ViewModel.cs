using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Models;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.Models.SECS;
using GPMVVM.PooledCollections;
using GPMVVM.SECSGEM;
using Newtonsoft.Json;
using PLCService;

namespace GPGO_MultiPLCs.ViewModels;

/// <summary>所有烤箱的生產總覽</summary>
public sealed class TotalView_ViewModel : ObservableObject
{
    public event Action                                                                                           WantLogin;
    public event Action<(int StationIndex, EventType type, DateTime time, string note, string tag, object value)> EventHappened;
    public event Action<(int StationIndex, string RackID)>                                                        CancelCheckIn;
    public event Action<(int StationIndex, string RecipeName)>                                                    RecipeUsed;
    public event Func<(int StationIndex, ProcessInfo Info), Task<int>>                                            AddRecordToDB;
    public event Func<(int StationIndex, string RecipeName), PLC_Recipe?>                                         GetRecipe;
    public event Func<PLC_Recipe, bool>                                                                           UpsertRecipe;
    public event Func<string, bool>                                                                               DeleteRecipe;

    /// <summary>財產編號儲存位置</summary>
    private const string AssetNumbersPath = "AssetNumbers";

    /// <summary>設備碼儲存位置</summary>
    private const string MachineCodesPath = "MachineCodes";
    private readonly IDialogService? Dialog;

    /// <summary>保持PLC Gate連線</summary>
    private readonly Timer Checker;

    public Language Language = Language.TW;

    public GOL_SecsGem                         SecsGemEquipment { get; }
    public HSMSParameters                      HSMSParameters   { get; }
    public IGate                               Gate             { get; }
    public ObservableConcurrentQueue<LogEvent> QueueMessages    { get; } = new();
    public RelayCommand                        WantLoginCommand { get; }

    /// <summary>回到總覽頁</summary>
    public RelayCommand BackCommand { get; }

    public AsyncCommand SendTerminalMessageCommand { get; }

    /// <summary>所有PLC</summary>
    public IList<PLC_ViewModel> PLC_All { get; }

    public IList<PLC_ViewModel> PLC_All_View => OvenCount > PLC_All.Count ? PLC_All : PLC_All.Take(OvenCount).ToList();

    /// <summary>檢視詳細資訊的PLC</summary>
    public PLC_ViewModel PLC_In_Focused => PLCIndex > -1 ? PLC_All[PLCIndex] : null;

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
        set
        {
            Set(value);

            if (value == 0)
            {
                PLCIndex = -1;
            }
        }
    }

    /// <summary>選擇PLC，並切換至該PLC詳細資訊檢視</summary>
    public int PLCIndex
    {
        get => Get<int>();
        set
        {
            Set(value);

            if (value > -1)
            {
                NotifyPropertyChanged(nameof(PLC_In_Focused));
                Index = 1;
            }
        }
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

                value = !value;
            }

            Set(value);
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

            if (!SecsGemEquipment.Online(value))
            {
                value = !value;
            }

            Set(value);
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

            if (!SecsGemEquipment.Remote(value))
            {
                value = !value;
            }

            Set(value);

            foreach (var plc in PLC_All)
            {
                plc.RemoteMode = value;
            }
        }
    }

    public TotalView_ViewModel(int count, IGate gate, IPAddress plcaddress, IDialogService dialog)
    {
        Gate      = gate;
        Dialog    = dialog;
        OvenCount = count;
        PLC_All   = new PLC_ViewModel[count];
        PLCIndex  = -1;
        var v = Assembly.GetExecutingAssembly().GetName().Version;
        SecsGemEquipment = new GOL_SecsGem("0", "GPGO", $"{v.Major}.{v.Minor}.{v.Build}");

        SecsGemEquipment.HSMSParameters.PropertyChanged += (_, _) => SecsGemEquipment.SaveHSMSParameters();

        WantLoginCommand = new RelayCommand(_ => WantLogin?.Invoke());

        BackCommand = new RelayCommand(index => Index = int.TryParse(index.ToString(), out var i) ? i : 0);

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

        SecsGemEquipment.TerminalMessageRecived += async message =>
                                                   {
                                                       if (Dialog == null)
                                                       {
                                                           return;
                                                       }

                                                       var eventval = (-1, EventType.SECSCommnd, DateTime.Now, nameof(SECSGEM.TerminalMessageRecived), "", message);
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

        SecsGemEquipment.ECChange += (ecid, value) =>
                                     {
                                         switch (ecid)
                                         {
                                             case "EqpName":

                                                 break;
                                             case "ReAlarmInterval":

                                                 break;
                                         }
                                     };

        SecsGemEquipment.UpsertRecipe += (name, recipedic) =>
                                         {
                                             var recipe   = new PLC_Recipe(name, "SECSGEM-HOST", UserLevel.Manager);
                                             var eventval = (-1, EventType.SECSCommnd, DateTime.Now, nameof(SECSGEM.UpsertRecipe), "", recipe.RecipeName);
                                             EventHappened?.Invoke(eventval);

                                             return recipe.SetByDictionary(recipedic) && UpsertRecipe != null && UpsertRecipe.Invoke(recipe);
                                         };

        SecsGemEquipment.DeleteRecipe += recipeName =>
                                         {
                                             var eventval = (-1, EventType.SECSCommnd, DateTime.Now, nameof(SECSGEM.DeleteRecipe), "", recipeName);
                                             EventHappened?.Invoke(eventval);

                                             return DeleteRecipe != null && DeleteRecipe.Invoke(recipeName);
                                         };

        SecsGemEquipment.START_Command += index =>
                                          {
                                              if (index >= PLC_All.Count)
                                              {
                                                  return HCACKValule.ParameterInvalid;
                                              }

                                              if (!Gate.GateStatus.CurrentValue || !PLC_All[index].ConnectionStatus.CurrentValue || !PLC_All[index].AutoMode || PLC_All[index].IsExecuting)
                                              {
                                                  return HCACKValule.CantPerform;
                                              }

                                              var eventval = (index, EventType.SECSCommnd, DateTime.Now, nameof(GOL_SecsGem.START_Command), "", index);
                                              EventHappened?.Invoke(eventval);
                                              PLC_All[index].AutoMode_Stop  = false;
                                              PLC_All[index].AutoMode_Start = true;

                                              return HCACKValule.Acknowledge;
                                          };

        SecsGemEquipment.STOP_Command += index =>
                                         {
                                             if (index >= PLC_All.Count)
                                             {
                                                 return HCACKValule.ParameterInvalid;
                                             }

                                             if (!Gate.GateStatus.CurrentValue || !PLC_All[index].ConnectionStatus.CurrentValue || !PLC_All[index].ProcessComplete)
                                             {
                                                 return HCACKValule.CantPerform;
                                             }

                                             var eventval = (index, EventType.SECSCommnd, DateTime.Now, nameof(GOL_SecsGem.STOP_Command), "", index);
                                             EventHappened?.Invoke(eventval);
                                             PLC_All[index].AutoMode_Start = false;
                                             PLC_All[index].AutoMode_Stop  = true;

                                             return HCACKValule.Acknowledge;
                                         };

        SecsGemEquipment.PPSELECT_Command += (index, name) =>
                                             {
                                                 if (index >= PLC_All.Count)
                                                 {
                                                     return HCACKValule.ParameterInvalid;
                                                 }

                                                 if (!Gate.GateStatus.CurrentValue || !PLC_All[index].ConnectionStatus.CurrentValue)
                                                 {
                                                     return HCACKValule.CantPerform;
                                                 }

                                                 var eventval = (index, EventType.SECSCommnd, DateTime.Now, nameof(GOL_SecsGem.PPSELECT_Command), "", $"{index}-{name}");
                                                 EventHappened?.Invoke(eventval);

                                                 return PLC_All[index].SetRecipe(name).Result == SetRecipeResult.成功 ? HCACKValule.Acknowledge : HCACKValule.CantPerform;
                                                 ;
                                             };

        SecsGemEquipment.ADDLOT_Command += (index, lot) =>
                                           {
                                               if (index >= PLC_All.Count)
                                               {
                                                   return HCACKValule.ParameterInvalid;
                                               }

                                               if (!Gate.GateStatus.CurrentValue || !PLC_All[index].ConnectionStatus.CurrentValue || PLC_All[index].IsExecuting)
                                               {
                                                   return HCACKValule.CantPerform;
                                               }

                                               var (lotID, partID, layer, quantity) = lot;

                                               PLC_All[index].AddLOT(lotID, partID, layer, quantity);
                                               var unit     = quantity > 1 ? "pcs" : "pc";
                                               var eventval = (index, EventType.SECSCommnd, DateTime.Now, nameof(GOL_SecsGem.ADDLOT_Command), "", $"{index}-{lotID}-{partID}-{layer}-{quantity}{unit}");
                                               EventHappened?.Invoke(eventval);

                                               SecsGemEquipment.InvokeEvent($"Oven{index + 1}_LotAdded");
                                               return HCACKValule.Acknowledge;
                                           };

        SecsGemEquipment.CANCEL_Command += index =>
                                           {
                                               if (index >= PLC_All.Count)
                                               {
                                                   return HCACKValule.ParameterInvalid;
                                               }

                                               if (PLC_All[index].IsExecuting)
                                               {
                                                   return HCACKValule.CantPerform;
                                               }

                                               var eventval = (index, EventType.SECSCommnd, DateTime.Now, nameof(GOL_SecsGem.CANCEL_Command), "", index);
                                               EventHappened?.Invoke(eventval);
                                               CancelCheckIn?.Invoke((index, PLC_All[index].OvenInfo.RackID));

                                               PLC_All[index].ClearInput();
                                               PLC_All[index].OvenInfo.Clear();
                                               SecsGemEquipment.InvokeEvent($"Oven{index + 1}_LotRemoved");
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

                                          var eventval = (-1, EventType.StatusChanged, DateTime.Now, nameof(SECS_REMOTE), "", true);
                                          EventHappened?.Invoke(eventval);
                                      };

        SecsGemEquipment.Enable(true);
        SecsGemEquipment.Online(true);

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

            plc.WantFocus += () => PLCIndex = index;

            //! PLC讀取配方內容時
            plc.GetRecipe += recipeName => string.IsNullOrEmpty(recipeName) ? null : GetRecipe?.Invoke((index, recipeName));

            //! PLC由OP指定變更配方時
            plc.RecipeUsed += recipeName => RecipeUsed?.Invoke((index, recipeName));

            plc.ExecutingStarted += () =>
                                    {
                                        PLCIndex = index;
                                        Index    = 1;
                                    };

            //! 烘烤流程結束時
            plc.ExecutingFinished += async baseInfo =>
                                     {
                                         var product = new ProcessInfo(baseInfo);

                                         //! 更新ProcessData以供上報
                                         try
                                         {
                                             SecsGemEquipment.UpdateDVbyName($"Oven{index + 1}_ProcessData", JsonConvert.SerializeObject(baseInfo));
                                         }
                                         catch
                                         {
                                             // ignored
                                         }

                                         if (baseInfo.IsFinished)
                                         {
                                             SecsGemEquipment.InvokeEvent($"Oven{index + 1}_ProcessComplete");

                                             dialog?.Show(new Dictionary<Language, string>
                                                          {
                                                              { Language.TW, "已完成烘烤！" },
                                                              { Language.CHS, "已完成烘烤！" },
                                                              { Language.EN, "Finished!" }
                                                          },
                                                          TimeSpan.FromSeconds(2));
                                         }
                                         else
                                         {
                                             SecsGemEquipment.InvokeEvent($"Oven{index + 1}_ProcessAborted");

                                             dialog?.Show(new Dictionary<Language, string>
                                                          {
                                                              { Language.TW, "已取消烘烤！" },
                                                              { Language.CHS, "已取消烘烤！" },
                                                              { Language.EN, "Canceled!" }
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
            plc.MachineCodeChanged += code => SaveMachineCodes(MachineCodesPath);

            //! 由OP變更財產編號時
            plc.AssetNumberChanged += code => SaveAssetNumbers(AssetNumbersPath);

            //! PLC配方輸入錯誤時
            plc.RecipeKeyInError += () =>
                                    {
                                        dialog?.Show(new Dictionary<Language, string>
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

            //! 取消投產
            plc.CancelCheckIn += RackID => CancelCheckIn?.Invoke((index, RackID));

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
                                  else if (name == nameof(PLC_ViewModel.SV_RecipeName))
                                  {
                                      SecsGemEquipment.UpdateITRISV(ITRI_SV.GEM_PP_EXEC_NAME, value);
                                  }

                                  SecsGemEquipment.UpdateSVbyName($"Oven{index + 1}_{name}", value);
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

                for (var i = 0; i < Math.Min(vals?.Length ?? 0, PLC_All.Count); i++)
                {
                    PLC_All[i].OvenInfo.AssetNumber = vals[i];
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
                    for (var i = 0; i < Math.Min(vals.Length, PLC_All.Count); i++)
                    {
                        PLC_All[i].OvenInfo.MachineCode = vals[i];
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        //for (var i = 0; i < PLC_All.Count; i++)
        //{
        //    PLC_All[i].OvenInfo.MachineCode = $"Machine{i + 1:00}";
        //}
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

    /// <summary>將配方寫入PLC</summary>
    /// <param name="index">PLC序號</param>
    /// <param name="recipe">配方</param>
    /// <returns>是否成功寫入PLC</returns>
    public Task<SetRecipeResult> SetRecipe(int index, PLC_Recipe recipe) => PLC_All[index].SetRecipe(recipe);

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
        SecsGemEquipment.UpdateDVbyName("GemPPChangeName",   name);
        SecsGemEquipment.UpdateDVbyName("GemPPChangeStatus", (int)status);
        SecsGemEquipment.InvokeEvent("GemProcessProgramChange");
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

    public void StartPLCGate() => Checker?.Change(0, Timeout.Infinite);
}