﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using GPGO_MultiPLCs.GP_PLCs;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;

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
            if (index < PLC_All.Length && index > -1)
            {
                //! short data先，bit bool後

                foreach (var D in val.D)
                {
                    PLC_All[index].D_Values[D.Key] = D.Value;
                }

                foreach (var M in val.M)
                {
                    PLC_All[index].M_Values[M.Key] = M.Value;
                }
            }
        }

        /// <summary>PLC連線狀態</summary>
        /// <param name="index">PLC序號</param>
        /// <param name="val">是否連線</param>
        void IGPServiceCallback.Status_Changed(int index, bool val)
        {
            if (index < PLC_All.Length && index > -1)
            {
                PLC_All[index].OnlineStatus = val;
            }
        }

        /// <summary>心跳信號位置</summary>
        private const int Check_Dev = 21;

        /// <summary>保持PLC Gate連線</summary>
        private readonly Timer Checker;

        private const string MachineCodesPath = "MachineCodes.json";

        private readonly InstanceContext site;

        /// <summary>wcf連線client</summary>
        private GPServiceClient PLC_Client;

        /// <summary>回到總覽頁</summary>
        public RelayCommand BackCommand { get; }

        /// <summary>所有PLC</summary>
        public PLC_DataProvider[] PLC_All { get; }

        /// <summary>檢視詳細資訊的PLC</summary>
        public PLC_DataProvider PLC_In_Focused => ViewIndex > -1 ? PLC_All[ViewIndex] : null;

        /// <summary>產量統計</summary>
        public ObservableConcurrentDictionary<int, int> TotalProduction { get; }

        public int TotalProductionCount => TotalProduction.Sum(x => x.Value);

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

        public event Action<(int StationIndex, ICollection<ProcessInfo> Infos)> AddRecordToDB;
        public event Action<(int StationIndex, EventType type, DateTime time, string note)> EventHappened;
        public event Func<(int StationIndex, string RecipeName), ValueTask<PLC_Recipe>> WantRecipe;
        public event Func<string, ValueTask<ICollection<ProductInfo>>> WantFrontData;
        public event Action<(int StationIndex, string TrolleyCode)> CancelCheckIn;

        /// <summary>讀取設備碼</summary>
        public void LoadMachineCodes()
        {
            if (File.Exists(MachineCodesPath))
            {
                try
                {
                    var vals = MachineCodesPath.ReadFromJsonFile<string[]>();

                    for (var i = 0; i < Math.Min(vals.Length, PLC_All.Length); i++)
                    {
                        PLC_All[i].OvenInfo.MachineCode = vals[i];
                    }

                    return;
                }
                catch
                {
                }
            }

            for (var i = 0; i < PLC_All.Length; i++)
            {
                PLC_All[i].OvenInfo.MachineCode = "Machine" + (i + 1).ToString("00");
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
        /// <returns></returns>
        public async Task SetRecipe(int index, PLC_Recipe recipe, bool SetToPLC)
        {
            if (recipe == null)
            {
                return;
            }

            recipe.CopyTo(PLC_All[index]);

            if (SetToPLC && PLC_Client?.State == CommunicationState.Opened && !PLC_All[index].IsRecording)
            {
                await PLC_Client.Set_DataAsync(DataType.D, index, PLC_All[index].Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key, x => x.Value));
            }
        }

        /// <summary>設定使用的PLC配方(透過配方名)</summary>
        /// <param name="names">配方名</param>
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
                                                     NotifyPropertyChanged(nameof(TotalProductionCount));
                                                 };

            //!註冊PLC事件需引發的動作
            for (var i = 0; i < PLC_Count; i++)
            {
                TotalProduction.Add(i, 0);
                PLC_All[i] = new PLC_DataProvider(plc_maps.ElementAt(i), dialog);
                var index = i;

                //!PLC由OP指定變更配方時
                PLC_All[i].SwitchRecipeEvent += async e =>
                                                {
                                                    var recipe = WantRecipe == null ? null : await WantRecipe.Invoke((index, e.RecipeName));

                                                    if (e.UpdateToPLC && PLC_Client?.State == CommunicationState.Opened && !PLC_All[index].IsRecording)
                                                    {
                                                        await PLC_Client.Set_DataAsync(DataType.D, index, PLC_All[index].Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key, x => x.Value));
                                                    }

                                                    return recipe;
                                                };

                //!烤箱自動啟動時，開始紀錄
                PLC_All[i].StartRecording += async recipeName =>
                                             {
                                                 var recipe = WantRecipe == null ? null : await WantRecipe.Invoke((index, recipeName));

                                                 if (PLC_Client?.State == CommunicationState.Opened && !PLC_All[index].IsRecording)
                                                 {
                                                     await PLC_Client.Set_DataAsync(DataType.D, index, PLC_All[index].Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key, x => x.Value));
                                                 }

                                                 return recipe;
                                             };

                //!烘烤流程結束時
                PLC_All[i].RecordingFinished += e =>
                                                {
                                                    if (e.productInfo.Count > 0)
                                                    {
                                                        //!寫入資料庫，上傳
                                                        var infos = new List<ProcessInfo>();

                                                        foreach (var info in e.productInfo)
                                                        {
                                                            infos.Add(new ProcessInfo(e.baseInfo, info));
                                                            TotalProduction[index] = TotalProduction[index] + info.PanelCodes.Count;
                                                        }

                                                        AddRecordToDB?.Invoke((index, infos));

                                                        //!完成上傳後，清空生產資訊
                                                        dialog?.Show(new Dictionary<Language, string>
                                                                     {
                                                                         { Language.TW, "第" + (index + 1) + "站已完成烘烤!" },
                                                                         { Language.CHS, "第" + (index + 1) + "站已完成烘烤!" },
                                                                         { Language.EN, "Oven No" + (index + 1) + "has been finished!" }
                                                                     },
                                                                     TimeSpan.FromSeconds(2));
                                                    }
                                                };

                //!由台車code取得前端生產資訊
                PLC_All[i].WantFrontData += async TrolleyCode =>
                                           {
                                               if (WantFrontData != null)
                                               {
                                                   return await WantFrontData.Invoke(TrolleyCode);
                                               }

                                               return null;
                                           };

                //!由OP變更設備代碼時
                PLC_All[i].MachineCodeChanged += code =>
                                                 {
                                                     SaveMachineCodes(MachineCodesPath);
                                                 };

                //!PLC配方輸入錯誤時
                PLC_All[i].RecipeKeyInError += () =>
                                               {
                                                   dialog?.Show(new Dictionary<Language, string>
                                                                {
                                                                    { Language.TW, "第" + (index + 1) + "站配方輸入錯誤!" },
                                                                    { Language.CHS, "第" + (index + 1) + "站配方输入错误!" },
                                                                    { Language.EN, "Oven No" + (index + 1) + " recipe input error!" }
                                                                },
                                                                TimeSpan.FromSeconds(1),
                                                                DialogMsgType.Alarm);
                                               };

                //!PLC事件紀錄
                PLC_All[i].EventHappened += e =>
                                            {
                                                EventHappened?.Invoke((index, e.type, e.time, e.note));
                                            };

                //!取消投產
                PLC_All[i].CancelCheckIn += TrolleyCode =>
                                            {
                                                CancelCheckIn?.Invoke((index, TrolleyCode));
                                            };
            }

            LoadMachineCodes();

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
                                    else if (!Check())
                                    {
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