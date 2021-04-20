using GPGO_MultiPLCs.Models;
using GPMVVM.Helpers;
using GPMVVM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GP_SECS_GEM;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>所有烤箱的生產總覽</summary>
    public sealed class TotalView_ViewModel : PLCGate
    {
        public new void Dispose()
        {
            Checker.Dispose();
            PLC_Client.Close();

            foreach (var plc in PLC_All)
            {
                plc.Dispose();
            }
        }

        private readonly IDialogService Dialog;
        private readonly SECSThread     secsGem;

        /// <summary>財產編號儲存位置</summary>
        private const string AssetNumbersPath = "AssetNumbers.json";

        /// <summary>設備碼儲存位置</summary>
        private const string MachineCodesPath = "MachineCodes.json";

        /// <summary>保持PLC Gate連線</summary>
        private readonly Timer Checker;

        public Language Language = Language.TW;

        /// <summary>回到總覽頁</summary>
        public RelayCommand BackCommand { get; }

        /// <summary>所有PLC</summary>
        public IList<PLC_DataProvider> PLC_All { get; }

        public IEnumerable<PLC_DataProvider> PLC_All_View => OvenCount > PLC_All.Count ? PLC_All : PLC_All.Take(OvenCount);

        /// <summary>檢視詳細資訊的PLC</summary>
        public PLC_DataProvider PLC_In_Focused => ViewIndex > -1 ? PLC_All[ViewIndex] : null;

        /// <summary>產量統計</summary>
        public ObservableConcurrentDictionary<int, int> TotalProduction { get; }

        public int TotalProductionCount => TotalProduction_View?.Sum(x => x.Value) ?? 0;

        public int OvenCount
        {
            get => Get<int>();
            set
            {
                Set(value);
                NotifyPropertyChanged(nameof(PLC_All_View));
                NotifyPropertyChanged(nameof(TotalProduction_View));
                NotifyPropertyChanged(nameof(TotalProductionCount));
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

        public IEnumerable<KeyValuePair<int, int>> TotalProduction_View => OvenCount > TotalProduction.Count ? TotalProduction : TotalProduction.Take(OvenCount);

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
                Set(value);

                if (!secsGem.Enable(value))
                {
                    Dialog?.Show(new Dictionary<Language, string>
                                 {
                                     {Language.TW, "無法啟用連線"},
                                     {Language.CHS, "无法启用联机"},
                                     {Language.EN, "Unable to enable connection"}
                                 });

                    Set(!value, nameof(SECS_ENABLE));
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
                    plc.LocalMode  = !plc.RemoteMode;
                }

                if (!secsGem.Remote(value))
                {
                    Set(!value, nameof(SECS_REMOTE));

                    foreach (var plc in PLC_All)
                    {
                        plc.RemoteMode = !value;
                        plc.LocalMode  = !plc.RemoteMode;
                    }
                }
            }
        }

        public event Func<(int StationIndex, ICollection<ProcessInfo> Infos), ValueTask<int>>                         AddRecordToDB;
        public event Action<(int StationIndex, string RackID)>                                                        CancelCheckIn;
        public event Action<(int StationIndex, EventType type, DateTime time, string note, string tag, object value)> EventHappened;
        public event Func<(int StationIndex, string RecipeName), PLC_Recipe>                                          GetRecipe;
        public event Action<(int StationIndex, string RecipeName)>                                                    RecipeUsed;
        public event Func<(int StationIndex, string RackID), ValueTask<ICollection<ProductInfo>>>                     WantFrontData;
        public event Func<User>                                                                                       GetUser;
        public event Func<PLC_Recipe, bool>                                                                           UpsertRecipe;
        public event Action<string>                                                                                   DeleteRecipe;

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

            for (var i = 0; i < PLC_All.Count; i++)
            {
                PLC_All[i].OvenInfo.MachineCode = $"Machine{i + 1:00}";
            }
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
        public bool SetRecipe(int index, PLC_Recipe recipe)
        {
            return PLC_All[index].SetRecipe(recipe);
        }

        /// <summary>設定使用的PLC配方(透過配方名)</summary>
        /// <param name="names">配方名列表</param>
        public void SetRecipeNames(ICollection<string> names)
        {
            foreach (var plc in PLC_All)
            {
                plc.Recipe_Names = names;
            }
        }

        public void InvokeRecipe(string name, SECSThread.PPStatus status)
        {
            secsGem?.UpdateDV("GemPPChangeName", name);
            secsGem?.UpdateDV("GemPPChangeStatus", (int)status);
            secsGem?.InvokeEvent("GemProcessProgramChange");
        }

        public TotalView_ViewModel(int count, IDialogService dialog)
        {
            Dialog    = dialog;
            OvenCount = count;
            PLC_All   = new PLC_DataProvider[OvenCount];
            ViewIndex = -1;

            BackCommand = new RelayCommand(o =>
                                           {
                                               Index = int.TryParse(o.ToString(), out var i) ? i : 0;
                                           });

            TotalProduction = new ObservableConcurrentDictionary<int, int>();

            //!當各PLC產量變更時更新總量顯示
            TotalProduction.CollectionChanged += (_, _) =>
                                                 {
                                                     NotifyPropertyChanged(nameof(TotalProduction_View));
                                                     NotifyPropertyChanged(nameof(TotalProductionCount));
                                                 };

            secsGem = new SECSThread(0);
            secsGem.TerminalMessage += async message =>
                                       {
                                           if (dialog == null)
                                           {
                                               return;
                                           }

                                           var (result, input) = await dialog.ShowWithInput(new Dictionary<Language, string>
                                                                                            {
                                                                                                {Language.TW, message},
                                                                                                {Language.CHS, message},
                                                                                                {Language.EN, message}
                                                                                            }, new Dictionary<Language, string>
                                                                                               {
                                                                                                   {Language.TW, "終端訊息"},
                                                                                                   {Language.CHS, "终端讯息"},
                                                                                                   {Language.EN, "TerminalMessage"}
                                                                                               });

                                           if (result)
                                           {
                                               secsGem.GemCore.SendTerminalMessage(input == null || input.ToString() == "" ? "Confirmed." : $"{input}");
                                           }
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
                                        UpsertRecipe?.Invoke(recipe);

                                        return true;
                                    };

            secsGem.DeleteRecipe += recipeName =>
                                    {
                                        DeleteRecipe?.Invoke(recipeName);
                                    };

            secsGem.Start += index =>
                             {
                                 PLC_All[index].RemoteCommandStart = true;

                                 return HCACKValule.Acknowledge;
                             };

            secsGem.Stop += index =>
                            {
                                PLC_All[index].RemoteCommandStop = true;

                                return HCACKValule.Acknowledge;
                            };

            secsGem.SetRecipe += (index, name) => PLC_All[index].SetRecipe(name) ? HCACKValule.Acknowledge : HCACKValule.ParameterInvalid;

            secsGem.AddLOT += (index, o) =>
                              {
                                  var (lotID, partID, panels) = o;

                                  var info = new ProductInfo
                                             {
                                                 PartID = partID,
                                                 LotID  = lotID
                                             };

                                  foreach (var panel in panels)
                                  {
                                      info.PanelIDs.Add(panel);
                                  }

                                  PLC_All[index].Ext_Info.Add(info);
                                  Task.Run(() =>
                                  {
                                      Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                                      secsGem.InvokeEvent($"Oven{index + 1}_LotAdded");
                                  }
                                  );
                                  return HCACKValule.Acknowledge;
                              };
            secsGem.CANCEL += index =>
            {
                PLC_All[index].Ext_Info.Clear();
                if (PLC_All[index].ExecutingTask != null && PLC_All[index].IsExecuting)
                {
                    PLC_All[index].CTS?.Cancel();

                    PLC_All[index].ExecutingTask.Wait();
                }

                CancelCheckIn?.Invoke((index, PLC_All[index].OvenInfo.RackID));
                PLC_All[index].OvenInfo.Clear();
                PLC_All[index].Ext_Info.Clear();
                secsGem.InvokeEvent($"Oven{index + 1}_LotRemoved");
                return HCACKValule.Acknowledge;
            };

            secsGem.CommEnable_Changed += e =>
                                          {
                                              Set(e, nameof(SECS_ENABLE));
                                          };

            secsGem.Communicating_Changed += e =>
                                          {
                                              Set(e, nameof(SECS_Communicating));
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

            //!註冊PLC事件需引發的動作
            for (var i = 0; i < OvenCount; i++)
            {
                var j = i + 1;

                TotalProduction.Add(i, 0);
                var plc = new PLC_DataProvider(dialog);
                PLC_All[i] = plc;
                var index = i;

                #region DataModel新值寫入PLC
                plc.SetBit   += async (type, dev, val) => await SetBit(index, type, dev, val);
                plc.SetData  += async (type, dev, val) => await SetData(index, type, dev, val);
                plc.SetDatas += async (type, vals) => await SetDatas(index, type, vals);
                #endregion

                //plc.ValueChanged += async (LogType, data) => await ValueChanged(LogType, data);

                plc.TracedDataChanged += data =>
                                                {
                                                };

                plc.GetLanguage += () => Language;

                //!PLC讀取配方內容時
                plc.GetRecipe += recipeName => string.IsNullOrEmpty(recipeName) ? null : GetRecipe?.Invoke((index, recipeName));

                //!PLC由OP指定變更配方時
                plc.RecipeUsed += recipeName => RecipeUsed?.Invoke((index, recipeName));

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
                                                        secsGem?.UpdateDV($"Oven{j}_ProcessData", JsonConvert.SerializeObject(products));
                                                    }
                                                    catch
                                                    {
                                                        // ignored
                                                    }

                                                    if (!baseInfo.IsFinished)
                                                    {
                                                        secsGem?.InvokeEvent($"Oven{j}_ProcessAborted");

                                                        dialog?.Show(new Dictionary<Language, string>
                                                                     {
                                                                         {Language.TW, $"第{index + 1}站已取消烘烤！"},
                                                                         {Language.CHS, $"第{index + 1}站已取消烘烤！"},
                                                                         {Language.EN, $"Oven No{index + 1}has been canceled!"}
                                                                     },
                                                                     TimeSpan.FromSeconds(2));

                                                        return;
                                                    }

                                                    if (AddRecordToDB != null && index < TotalProduction.Count)
                                                    {
                                                        TotalProduction[index] = await AddRecordToDB.Invoke((index, products));
                                                    }

                                                    secsGem?.InvokeEvent($"Oven{j}_ProcessComplete");

                                                    //!完成上傳後，清空生產資訊
                                                    dialog?.Show(new Dictionary<Language, string>
                                                                 {
                                                                     {Language.TW, $"第{index + 1}站已完成烘烤！"},
                                                                     {Language.CHS, $"第{index + 1}站已完成烘烤！"},
                                                                     {Language.EN, $"Oven No{index + 1}has been finished!"}
                                                                 },
                                                                 TimeSpan.FromSeconds(2));
                                                };

                //!由板架code取得前端生產資訊
                plc.WantFrontData += async e =>
                                            {
                                                if (WantFrontData != null)
                                                {
                                                    return await WantFrontData.Invoke((index, e));
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
                                                                    {Language.TW, $"第{index + 1}站配方輸入錯誤！"},
                                                                    {Language.CHS, $"第{index + 1}站配方输入错误！"},
                                                                    {Language.EN, $"Oven No{index + 1} recipe input error!"}
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
                                                  secsGem.InvokeEvent($"Oven{j}_{EventName}");
                                              };

                plc.InvokeSECSAlarm += (AlarmName, val) =>
                                              {
                                                  secsGem.InvokeAlarm($"Oven{j}_{AlarmName}", val);
                                              };

                plc.SV_Changed += (name, value) =>
                                         {
                                             if (name == "RackID")
                                             {
                                                 value = value.ToString().Trim();
                                             }
                                             
                                             secsGem.UpdateSV($"Oven{j}_{name}", value);
                                         };

                plc.RecipeChangedbyPLC += recipe =>
                                          {
                                              UpsertRecipe?.Invoke(recipe);
                                          };
            }

            LoadMachineCodes();
            LoadAssetNumbers();

            #region PLCGate事件通知
            MessagesSent += (i, v) =>
                             {
                                 try
                                 {
                                     if (i <= -1)
                                     {
                                         return;
                                     }

                                     var plc = PLC_All[i];

                                     plc.SetValues(v);
                                 }
                                 catch
                                 {
                                     // ignored
                                 }
                             };

            StatusChanged += (i, v) =>
                              {
                                  try
                                  {
                                      if (i < PLC_All.Count && i > -1 && PLC_All[i].OnlineStatus != v)
                                      {
                                          PLC_All[i].OnlineStatus = v;
                                      }
                                  }
                                  catch (Exception)
                                  {
                                      // ignored
                                  }
                              };

            GateOffline += () =>
                           {
                               EventHappened?.Invoke((-1, EventType.Alarm, DateTime.Now, "PLC Gate Offline!", string.Empty, true));

                               foreach (var plc in PLC_All)
                               {
                                   plc.OnlineStatus = false;
                               }
                           };
            #endregion

            Checker = new Timer(_ =>
                                {
                                    if (!Gate_Status)
                                    {
                                        if (Connect() && SetReadLists(PLC_All.Select(x => x.GetNameArray()).ToArray())) //!連線並發送訂閱列表
                                        {
                                            Gate_Status = true;
                                        }
                                    }

                                    foreach (var plc in PLC_All)
                                    {
                                        plc.Check = !plc.Check;
                                    }

                                    Checker.Change(150, Timeout.Infinite);
                                },
                                null,
                                0,
                                Timeout.Infinite);
        }
    }
}