using GPGO_MultiPLCs.GP_PLCs;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>所有烤箱的生產總覽</summary>
    public sealed class TotalView_ViewModel : ObservableObject, IGPServiceCallback, IDisposable
    {
        public void Dispose()
        {
            Checker.Dispose();
            PLC_Client.Close();

            foreach (var plc in PLC_All)
            {
                plc.Dispose();
            }
        }

        /// <summary>PLC Gate通知PLC資訊更新</summary>
        /// <param name="index">PLC序號</param>
        /// <param name="val">更新值集合</param>
        void IGPServiceCallback.Messages_Send(int index, PLC_Messages val)
        {
            OneScheduler.StartNew(e =>
                                  {
                                      var args = (ValueTuple<int, PLC_Messages>)e;
                                      try
                                      {
                                          if (args.Item1 < PLC_All.Count && args.Item1 > -1)
                                          {
                                              //! short data先，bit bool後

                                              foreach (var D in args.Item2.D)
                                              {
                                                  PLC_All[args.Item1].D_Values[D.Key] = D.Value;
                                              }

                                              foreach (var M in args.Item2.M)
                                              {
                                                  PLC_All[args.Item1].M_Values[M.Key] = M.Value;
                                              }
                                          }
                                      }
                                      catch (Exception)
                                      {
                                      }
                                  },
                                  (index, val));
        }

        /// <summary>PLC連線狀態</summary>
        /// <param name="index">PLC序號</param>
        /// <param name="val">是否連線</param>
        void IGPServiceCallback.Status_Changed(int index, bool val)
        {
            OneScheduler.StartNew(e =>
                                  {
                                      var args = (ValueTuple<int, bool>)e;
                                      try
                                      {
                                          if (args.Item1 < PLC_All.Count && args.Item1 > -1 && PLC_All[args.Item1].OnlineStatus != args.Item2)
                                          {
                                              PLC_All[args.Item1].OnlineStatus = args.Item2;
                                          }
                                      }
                                      catch (Exception)
                                      {
                                      }
                                  },
                                  (index, val));
        }

        /// <summary>財產編號儲存位置</summary>
        private const string AssetNumbersPath = "AssetNumbers.json";

        /// <summary>心跳信號位置</summary>
        private const int Check_Dev = 21;

        /// <summary>設備碼儲存位置</summary>
        private const string MachineCodesPath = "MachineCodes.json";

        /// <summary>保持PLC Gate連線</summary>
        private readonly Timer Checker;

        private readonly TaskFactory OneScheduler = new TaskFactory(new StaTaskScheduler(1));

        private readonly InstanceContext site;

        /// <summary>wcf連線client</summary>
        private GPServiceClient PLC_Client;

        /// <summary>回到總覽頁</summary>
        public RelayCommand BackCommand { get; }

        /// <summary>所有PLC</summary>
        public IList<PLC_DataProvider> PLC_All { get; }

        /// <summary>檢視詳細資訊的PLC</summary>
        public PLC_DataProvider PLC_In_Focused => ViewIndex > -1 ? PLC_All[ViewIndex] : null;

        /// <summary>產量統計</summary>
        public ObservableConcurrentDictionary<int, int> TotalProduction { get; }

        public int TotalProductionCount => TotalProduction_View?.Sum(x => x.Value) ?? 0;

        /// <summary>PLC Gate連線狀態</summary>
        public bool Gate_Status
        {
            get => Get<bool>();
            set => Set(value);
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

        public IEnumerable<KeyValuePair<int, int>> TotalProduction_View
        {
            get => Get<IEnumerable<KeyValuePair<int, int>>>();
            set
            {
                Set(value);
                NotifyPropertyChanged(nameof(TotalProductionCount));
            }
        }

        public int TotalProduction_ViewCount
        {
            get => Get<int>();
            set
            {
                Set(value);
                TotalProduction_View = TotalProduction.Take(value);
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

        public event Func<(int StationIndex, ICollection<ProcessInfo> Infos), ValueTask<int>> AddRecordToDB;

        public event Action<(int StationIndex, string TrolleyCode)> CancelCheckIn;

        public event Action<(int StationIndex, EventType type, DateTime time, string note, int tag, bool value)> EventHappened;

        public event Func<(int StationIndex, string RecipeName), PLC_Recipe> GetRecipe;

        public event Action<(int StationIndex, string RecipeName)> RecipeUsed;

        public event Func<(int StationIndex, string TrolleyCode), ValueTask<ICollection<ProductInfo>>> WantFrontData;

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
            }
        }

        /// <summary>設定PLC監控讀取列表</summary>
        /// <param name="list">所有PLC的讀取列表</param>
        /// <returns></returns>
        public bool SetReadLists(string[][] list)
        {
            try
            {
                if (PLC_Client?.State != CommunicationState.Opened)
                {
                    return false;
                }

                PLC_Client.SetReadLists(list);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>將配方寫入PLC</summary>
        /// <param name="index">PLC序號</param>
        /// <param name="recipe">配方</param>
        /// <param name="SetToPLC">是否寫入PLC</param>
        /// <returns>是否成功寫入PLC</returns>
        public async Task<bool> SetRecipe(int index, PLC_Recipe recipe, bool SetToPLC)
        {
            if (recipe == null)
            {
                return false;
            }

            if (PLC_All[index].IsRecording)
            {
                PLC_All[index].SetSelectedRecipeName(recipe.RecipeName);
                return false;
            }

            recipe.CopyTo(PLC_All[index]);

            if (SetToPLC && PLC_Client?.State == CommunicationState.Opened && PLC_All[index].OnlineStatus)
            {
                await PLC_Client.Set_DataAsync(DataType.D, index, PLC_All[index].Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key, x => x.Value));

                return true;
            }

            return false;
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

        /// <summary>發送PC和PLC間的檢查信號</summary>
        /// <returns></returns>
        private bool Check()
        {
            try
            {
                if (PLC_Client?.State != CommunicationState.Opened)
                {
                    return false;
                }

                PLC_Client.CheckSignal(Check_Dev);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>和PLC Gate連線</summary>
        /// <returns></returns>
        private bool Connect()
        {
            if (PLC_Client?.State == CommunicationState.Opened)
            {
                PLC_Client.Close();
            }

            try
            {
                PLC_Client = new GPServiceClient(site);
                PLC_Client.Open();

                return PLC_Client.State == CommunicationState.Opened;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>剛連線並初始化參數</summary>
        /// <returns></returns>
        private bool Initial()
        {
            try
            {
                if (PLC_Client?.State != CommunicationState.Opened)
                {
                    return false;
                }

                PLC_Client.Initial();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public TotalView_ViewModel(IReadOnlyCollection<PLC_DevicesMap> plc_maps, IDialogService dialog)
        {
            ViewIndex = -1;
            var PLC_Count = plc_maps.Count;
            site = new InstanceContext(this);

            BackCommand = new RelayCommand(o =>
                                           {
                                               Index = o is int i ? i : 0;
                                           });

            PLC_All = new PLC_DataProvider[PLC_Count];
            TotalProduction = new ObservableConcurrentDictionary<int, int>();

            //!當各PLC產量變更時更新總量顯示
            TotalProduction.CollectionChanged += (obj, args) =>
                                                 {
                                                     TotalProduction_ViewCount = TotalProduction_ViewCount;
                                                 };

            //!註冊PLC事件需引發的動作
            for (var i = 0; i < PLC_Count; i++)
            {
                TotalProduction.Add(i, 0);
                PLC_All[i] = new PLC_DataProvider(plc_maps.ElementAt(i), dialog);
                var index = i;

                //!PLC讀取配方內容時
                PLC_All[i].GetRecipe += recipeName => string.IsNullOrEmpty(recipeName) ? null : GetRecipe?.Invoke((index, recipeName));

                //!PLC由OP指定變更配方時
                PLC_All[i].RecipeUsed += recipeName => RecipeUsed?.Invoke((index, recipeName));

                //!烘烤流程結束時
                PLC_All[i].RecordingFinished += async e =>
                                                {
                                                    if (!e.Pass)
                                                    {
                                                        dialog?.Show(new Dictionary<Language, string>
                                                                     {
                                                                         { Language.TW, $"第{index + 1}站已取消烘烤！" },
                                                                         { Language.CHS, $"第{index + 1}站已取消烘烤！" },
                                                                         { Language.EN, $"Oven No{index + 1}has been canceled!" }
                                                                     },
                                                                     TimeSpan.FromSeconds(2));
                                                    }
                                                    else if (e.productInfo.Count > 0)
                                                    {
                                                        //!寫入資料庫，上傳
                                                        var infos = e.productInfo.Select(info => new ProcessInfo(e.baseInfo, info)).ToList();

                                                        if (AddRecordToDB != null && index < TotalProduction.Count)
                                                        {
                                                            TotalProduction[index] = await AddRecordToDB.Invoke((index, infos));
                                                        }

                                                        //!完成上傳後，清空生產資訊
                                                        dialog?.Show(new Dictionary<Language, string>
                                                                     {
                                                                         { Language.TW, $"第{index + 1}站已完成烘烤！" },
                                                                         { Language.CHS, $"第{index + 1}站已完成烘烤！" },
                                                                         { Language.EN, $"Oven No{index + 1}has been finished!" }
                                                                     },
                                                                     TimeSpan.FromSeconds(2));
                                                    }
                                                };

                //!由台車code取得前端生產資訊
                PLC_All[i].WantFrontData += async e =>
                                            {
                                                if (WantFrontData != null)
                                                {
                                                    return await WantFrontData.Invoke((index, e));
                                                }

                                                return null;
                                            };

                //!由OP變更設備代碼時
                PLC_All[i].MachineCodeChanged += code =>
                                                 {
                                                     SaveMachineCodes(MachineCodesPath);
                                                 };

                //!由OP變更財產編號時
                PLC_All[i].AssetNumberChanged += code =>
                                                 {
                                                     SaveAssetNumbers(AssetNumbersPath);
                                                 };

                //!PLC配方輸入錯誤時
                PLC_All[i].RecipeKeyInError += () =>
                                               {
                                                   dialog?.Show(new Dictionary<Language, string>
                                                                {
                                                                    { Language.TW, $"第{index + 1}站配方輸入錯誤！" },
                                                                    { Language.CHS, $"第{index + 1}站配方输入错误！" },
                                                                    { Language.EN, $"Oven No{index + 1} recipe input error!" }
                                                                },
                                                                TimeSpan.FromSeconds(1),
                                                                DialogMsgType.Alarm);
                                               };

                //!PLC事件紀錄
                PLC_All[i].EventHappened += e =>
                                            {
                                                EventHappened?.Invoke((index, e.type, e.time, e.note, e.tag, e.value));
                                            };

                //!取消投產
                PLC_All[i].CancelCheckIn += TrolleyCode =>
                                            {
                                                CancelCheckIn?.Invoke((index, TrolleyCode));
                                            };

                PLC_All[i].GetPLCParameters += async values => PLC_Client?.State == CommunicationState.Opened ? await PLC_Client.Get_DataAsync(DataType.D, index, values) : null;

                PLC_All[i].SetPLCParameters += async values =>
                                               {
                                                   if (PLC_Client?.State == CommunicationState.Opened)
                                                   {
                                                       await PLC_Client.Set_DataAsync(DataType.D, index, values);
                                                   }
                                               };
            }

            LoadMachineCodes();
            LoadAssetNumbers();

            //!產生PLC位置訂閱列表，M、D為10進制位置，B、X、Y、W為16進制
            var namearray = plc_maps.Select(x =>
                                            {
                                                var list1 = x.SignalList.Values.OrderBy(y => y).Select(y => BitType.M + y.ToString());
                                                var list2 = x.DataList.Values.OrderBy(y => y).Select(y => DataType.D + y.ToString());

                                                return list1.Concat(list2).ToArray();
                                            })
                                    .ToArray();

            Checker = new Timer(o =>
                                {
                                    if (!Gate_Status)
                                    {
                                        if (Connect() && Initial() && SetReadLists(namearray)) //!連線並發送訂閱列表
                                        {
                                            Gate_Status = true;
                                        }
                                    }
                                    else if (!Check() && Gate_Status)
                                    {
                                        EventHappened?.Invoke((-1, EventType.Alarm, DateTime.Now, "PLC Gate Offline!", (int)PCEventCode.PC_Offline, true));

                                        Gate_Status = false;

                                        foreach (var plc in PLC_All)
                                        {
                                            plc.OnlineStatus = false;
                                        }
                                    }

                                    Checker.Change(150, Timeout.Infinite);
                                },
                                null,
                                0,
                                Timeout.Infinite);
        }
    }
}