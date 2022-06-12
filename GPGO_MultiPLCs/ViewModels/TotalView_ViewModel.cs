using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GP_SECS_GEM;
using GPGO_MultiPLCs.Models;
using GPMVVM.Helpers;
using GPMVVM.Models;
using Mapster.Adapters;
using Newtonsoft.Json;
using PLCService;

namespace GPGO_MultiPLCs.ViewModels;

/// <summary>所有烤箱的生產總覽</summary>
public sealed class TotalView_ViewModel : ObservableObject
{
    private readonly IDialogService Dialog;
    private readonly SECSThread     secsGem;

    /// <summary>財產編號儲存位置</summary>
    private const string AssetNumbersPath = "AssetNumbers";

    /// <summary>設備碼儲存位置</summary>
    private const string MachineCodesPath = "MachineCodes";

    /// <summary>保持PLC Gate連線</summary>
    private readonly Timer Checker;

    public event Action                                                                                           WantLogin;
    public event Func<(int StationIndex, ICollection<ProcessInfo> Infos), ValueTask<int>>                         AddRecordToDB;
    public event Action<(int StationIndex, string RackID)>                                                        CancelCheckIn;
    public event Action<(int StationIndex, EventType type, DateTime time, string note, string tag, object value)> EventHappened;
    public event Func<(int StationIndex, string RecipeName), PLC_Recipe>                                          GetRecipe;
    public event Action<(int StationIndex, string RecipeName)>                                                    RecipeUsed;
    public event Func<(int StationIndex, string RackID), ValueTask<ICollection<ProductInfo>>>                     WantFrontData;
    public event Func<User>                                                                                       GetUser;
    public event Func<PLC_Recipe, ValueTask<bool>>                                                                UpsertRecipe;
    public event Func<string, ValueTask<bool>>                                                                    DeleteRecipe;
    public event Func<string, ValueTask<ProcessInfo>>                                                             RetrieveLotData;

    public Language Language = Language.TW;

    public IGate Gate { get; }

    public ObservableConcurrentQueue<LogEvent> QueueMessages { get; } = new();

    public RelayCommand WantLoginCommand { get; }

    /// <summary>回到總覽頁</summary>
    public RelayCommand BackCommand { get; }

    /// <summary>所有PLC</summary>
    public IList<PLC_ViewModel> PLC_All { get; }

    public IEnumerable<PLC_ViewModel> PLC_All_View => OvenCount > PLC_All.Count ? PLC_All : PLC_All.Take(OvenCount);

    /// <summary>檢視詳細資訊的PLC</summary>
    public PLC_ViewModel PLC_In_Focused => ViewIndex > -1 ? PLC_All[ViewIndex] : null;

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

            Gate.GateStatus.CurrentValue = false; //!重新連線並發送通訊列表
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
                ViewIndex = -1;
            }
        }
    }

    /// <summary>PLC詳細資訊檢視index</summary>
    public int ViewIndex
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
            if (!secsGem.Enable(value))
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

                Set(!value, nameof(SECS_ENABLE));
            }
            else
            {
                Set(value);
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

            Set(value);

            if (!secsGem.Online(value))
            {
                Set(!value, nameof(SECS_ONLINE));
            }
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

            Set(value);

            foreach (var plc in PLC_All)
            {
                plc.RemoteMode = value;
            }

            if (!secsGem.Remote(value))
            {
                Set(!value, nameof(SECS_REMOTE));

                foreach (var plc in PLC_All)
                {
                    plc.RemoteMode = !value;
                }
            }
        }
    }

    /// <summary>讀取財產編號</summary>
    public void LoadAssetNumbers()
    {
        if (File.Exists(AssetNumbersPath))
        {
            try
            {
                var vals = AssetNumbersPath.ReadFromJsonFile<string[]>();

                for (var i = 0; i < Math.Min(vals.Length, PLC_All.Count); i++)
                {
                    PLC_All[i].OvenInfo.AssetNumber = vals[i];
                }

                return;
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

                for (var i = 0; i < Math.Min(vals.Length, PLC_All.Count); i++)
                {
                    PLC_All[i].OvenInfo.MachineCode = vals[i];
                }

                return;
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
            PLC_All.Select(x => x.OvenInfo.AssetNumber).ToArray().WriteToJsonFile(path);
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

    public void InvokeRecipe(string name, SECSThread.PPStatus status)
    {
        secsGem?.UpdateDV("GemPPChangeName",   name);
        secsGem?.UpdateDV("GemPPChangeStatus", (int)status);
        secsGem?.InvokeEvent("GemProcessProgramChange");
    }

    public void InsertMessage(params LogEvent[] evs)
    {
        if (evs.Length == 0) return;

        var _evs = evs.OrderBy(x => x.AddedTime).ToArray();

        if (QueueMessages.IsEmpty || (_evs.FirstOrDefault() is {} _ev && _ev.AddedTime >= QueueMessages.Last().AddedTime && (DateTime.Now - _ev.AddedTime).TotalDays <= 1.0))
        {
            foreach (var ev in _evs)
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

            while (QueueMessages.Count > 50)
            {
                QueueMessages.TryDequeue(out _);
            }
        }
    }

    public void StartPLCGate()
    {
        Checker?.Change(0, Timeout.Infinite);
    }

    public TotalView_ViewModel(int count, IGate gate, IPAddress plcaddress, IDialogService dialog)
    {
        Gate      = gate;
        Dialog    = dialog;
        OvenCount = count;
        PLC_All   = new PLC_ViewModel[count];
        ViewIndex = -1;

        WantLoginCommand = new RelayCommand(_ =>
                                            {
                                                WantLogin?.Invoke();
                                            });

        BackCommand = new RelayCommand(index =>
                                       {
                                           Index = int.TryParse(index.ToString(), out var i) ? i : 0;
                                       });

        secsGem = new SECSThread(0);
        secsGem.TerminalMessage += message =>
                                   {
                                       if (dialog == null)
                                       {
                                           return;
                                       }

                                       _ = dialog.Show(new Dictionary<Language, string>
                                                       {
                                                           { Language.TW, $"{DateTime.Now:M/d HH:mm:ss} 終端訊息：\n{message}" },
                                                           { Language.CHS, $"{DateTime.Now:M/d HH:mm:ss} 终端讯息：\n{message}" },
                                                           { Language.EN, $"{DateTime.Now:M/d HH:mm:ss} TerminalMessage：\n{message}" }
                                                       },
                                                       false,
                                                       TimeSpan.FromDays(1));
                                   };

        secsGem.ECChange += (index, ecid, value) =>
                            {
                                switch (ecid)
                                {
                                    case "EqpName":

                                        break;
                                    case "ReAlarmInterval":

                                        break;
                                }
                            };

        secsGem.UpsertRecipe += recipe =>
                                {
                                    var _ = UpsertRecipe != null && UpsertRecipe.Invoke(recipe).Result;

                                    return true;
                                };

        secsGem.DeleteRecipe += async recipeName =>
                                {
                                    var _ = DeleteRecipe != null && await DeleteRecipe.Invoke(recipeName);

                                    return true;
                                };

        secsGem.Start += index =>
                         {
                             if (!Gate.GateStatus.CurrentValue || !PLC_All[index].ConnectionStatus.CurrentValue || !PLC_All[index].AllowStart)
                             {
                                 return HCACKValule.CantPerform;
                             }

                             PLC_All[index].RemoteCommandStart = true;

                             return HCACKValule.Acknowledge;
                         };

        secsGem.Stop += index =>
                        {
                            if (!Gate.GateStatus.CurrentValue || !PLC_All[index].ConnectionStatus.CurrentValue || !PLC_All[index].AllowStop)
                            {
                                return HCACKValule.CantPerform;
                            }

                            PLC_All[index].RemoteCommandStop = true;

                            return HCACKValule.Acknowledge;
                        };

        secsGem.SetRecipe += (index, name) =>
                             {
                                 if (!Gate.GateStatus.CurrentValue || !PLC_All[index].ConnectionStatus.CurrentValue)
                                 {
                                     return HCACKValule.CantPerform;
                                 }

                                 return PLC_All[index].SetRecipe(name).Result == SetRecipeResult.成功 ? HCACKValule.Acknowledge : HCACKValule.CantPerform;
                             };

        secsGem.AddLOT += (index, lot) =>
                          {
                              var (lotID, partID, panels) = lot;

                              PLC_All[index].AddLOT(lotID, partID, panels);

                              Task.Run(() =>
                                       {
                                           Thread.Sleep(100);
                                           secsGem.InvokeEvent($"Oven{index + 1}_LotAdded");
                                       });
                              return HCACKValule.Acknowledge;
                          };

        secsGem.CANCEL += index =>
                          {
                              PLC_All[index].ProductInfos.Clear();
                              if (PLC_All[index].ExecutingTask != null && PLC_All[index].IsExecuting)
                              {
                                  PLC_All[index].CTS?.Cancel();

                                  PLC_All[index].ExecutingTask.Wait();
                              }

                              CancelCheckIn?.Invoke((index, PLC_All[index].OvenInfo.RackID));
                              PLC_All[index].OvenInfo.Clear();
                              PLC_All[index].ProductInfos.Clear();
                              secsGem.InvokeEvent($"Oven{index + 1}_LotRemoved");
                              return HCACKValule.Acknowledge;
                          };

        secsGem.CommEnable_Changed += boolval =>
                                      {
                                          Set(boolval, nameof(SECS_ENABLE));
                                      };

        secsGem.Communicating_Changed += boolval =>
                                         {
                                             Set(boolval, nameof(SECS_Communicating));
                                         };

        secsGem.ONLINE_Changed += online =>
                                  {
                                      Set(online, nameof(SECS_ONLINE));
                                  };

        secsGem.GO_Local += () =>
                            {
                                Set(false, nameof(SECS_REMOTE));
                            };

        secsGem.GO_Remote += () =>
                             {
                                 Set(true, nameof(SECS_REMOTE));
                             };

        //!依LotID查詢最近一筆資料
        secsGem.RetrieveLotData += lotid =>
                                   {
                                       var info = RetrieveLotData?.Invoke(lotid).Result;

                                       if (info != null)
                                       {
                                           try
                                           {
                                               secsGem?.UpdateDV("RetrieveLotData", JsonConvert.SerializeObject(info));
                                           }
                                           catch
                                           {
                                               // ignored
                                           }
                                       }
                                       else
                                       {
                                           return HCACKValule.NoObjectExists;
                                       }

                                       return HCACKValule.Acknowledge;
                                   };

        var address = plcaddress.GetAddressBytes();

        //!註冊PLC事件需引發的動作
        for (var i = 0; i < count; i++)
        {
            var plc = new PLC_ViewModel(dialog,
                                        Gate,
                                        BitConverter.ToInt32(new[] { address[0], address[1], address[2], (byte)(address[3] + i) }, 0),
                                        //i,
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
                                                      })); //!可指定PLC點位位移

            PLC_All[i] = plc;
            var index = i;

            plc.GetLanguage += () => Language;

            //!PLC讀取配方內容時
            plc.GetRecipe += recipeName => string.IsNullOrEmpty(recipeName) ? null : GetRecipe?.Invoke((index, recipeName));

            //!PLC由OP指定變更配方時
            plc.RecipeUsed += recipeName => RecipeUsed?.Invoke((index, recipeName));

            plc.ExecutingStarted += () =>
                                    {
                                        ViewIndex = 0;
                                        Index     = 1;
                                    };

            //!烘烤流程結束時
            plc.ExecutingFinished += async e =>
                                     {
                                         var (baseInfo, productInfo) = e;

                                         var products = productInfo.Count > 0 ?
                                                            productInfo.Select(info => new ProcessInfo(baseInfo, info)).ToList() :
                                                            new List<ProcessInfo>
                                                            {
                                                                new(baseInfo, new ProductInfo())
                                                            };

                                         //! 更新ProcessData以供上報
                                         try
                                         {
                                             secsGem?.UpdateDV($"Oven{index + 1}_ProcessData", JsonConvert.SerializeObject(products));
                                         }
                                         catch
                                         {
                                             // ignored
                                         }

                                         if (baseInfo.IsFinished)
                                         {
                                             secsGem?.InvokeEvent($"Oven{index + 1}_ProcessComplete");

                                             dialog?.Show(new Dictionary<Language, string>
                                                          {
                                                              { Language.TW, $"第{index       + 1}站已完成烘烤！" },
                                                              { Language.CHS, $"第{index      + 1}站已完成烘烤！" },
                                                              { Language.EN, $"Oven No{index + 1}has been finished!" }
                                                          },
                                                          TimeSpan.FromSeconds(2));
                                         }
                                         else
                                         {
                                             secsGem?.InvokeEvent($"Oven{index + 1}_ProcessAborted");

                                             dialog?.Show(new Dictionary<Language, string>
                                                          {
                                                              { Language.TW, $"第{index       + 1}站已取消烘烤！" },
                                                              { Language.CHS, $"第{index      + 1}站已取消烘烤！" },
                                                              { Language.EN, $"Oven No{index + 1}has been canceled!" }
                                                          },
                                                          TimeSpan.FromSeconds(2));
                                         }

                                         if (AddRecordToDB != null)
                                         {
                                             await AddRecordToDB.Invoke((index, products));
                                         }
                                     };

            //!由板架code取得前端生產資訊
            plc.WantFrontData += async code =>
                                 {
                                     if (WantFrontData != null)
                                     {
                                         return await WantFrontData.Invoke((index, code));
                                     }

                                     return null;
                                 };

            //!由OP變更設備代碼時
            plc.MachineCodeChanged += code =>
                                      {
                                          SaveMachineCodes(MachineCodesPath);
                                      };

            //!由OP變更財產編號時
            plc.AssetNumberChanged += code =>
                                      {
                                          SaveAssetNumbers(AssetNumbersPath);
                                      };

            //!PLC配方輸入錯誤時
            plc.RecipeKeyInError += () =>
                                    {
                                        dialog?.Show(new Dictionary<Language, string>
                                                     {
                                                         { Language.TW, $"第{index       + 1}站配方輸入錯誤！" },
                                                         { Language.CHS, $"第{index      + 1}站配方输入错误！" },
                                                         { Language.EN, $"Oven No{index + 1} recipe input error!" }
                                                     },
                                                     TimeSpan.FromSeconds(1),
                                                     DialogMsgType.Alarm);
                                    };

            //!PLC事件紀錄
            plc.EventHappened += e =>
                                 {
                                     EventHappened?.Invoke((index, e.type, e.time, e.note, e.tag, e.value));
                                 };

            //!取消投產
            plc.CancelCheckIn += RackID =>
                                 {
                                     CancelCheckIn?.Invoke((index, RackID));
                                 };

            plc.GetUser += () => GetUser?.Invoke();

            plc.InvokeSECSEvent += EventName =>
                                   {
                                       secsGem.InvokeEvent($"Oven{index + 1}_{EventName}");
                                   };

            plc.InvokeSECSAlarm += (AlarmName, val) =>
                                   {
                                       secsGem.InvokeAlarm($"Oven{index + 1}_{AlarmName}", val);
                                   };

            plc.SV_Changed += (name, value) =>
                              {
                                  if (name == "RackID")
                                  {
                                      value = value.ToString().Trim();
                                  }

                                  secsGem.UpdateSV($"Oven{index + 1}_{name}", value);
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
                                        Gate.SetReadListsByDataModels(PLC_All[i]); //!連線並發送訂閱列表
                                    }
                                }

                                Checker?.Change(150, Timeout.Infinite);
                            },
                            null,
                            Timeout.Infinite,
                            Timeout.Infinite);
    }
}