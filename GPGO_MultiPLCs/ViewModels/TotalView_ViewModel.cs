using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GPGO_MultiPLCs.GP_PLCs;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>
    /// 生產總覽
    /// </summary>
    public class TotalView_ViewModel : ViewModelBase, IGPServiceCallback
    {
        /// <summary>
        /// PLC Gate通知PLC資訊更新
        /// </summary>
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

        /// <summary>
        /// PLC連線狀態
        /// </summary>
        /// <param name="index">PLC序號</param>
        /// <param name="val">是否連線</param>
        void IGPServiceCallback.Status_Changed(int index, bool val)
        {
            if (index < PLC_All.Length && index > -1)
            {
                PLC_All[index].OnlineStatus = val;
            }
        }

        public delegate void AddRecordToDBHandler(int index, ProcessInfo info);

        public delegate void WantRecipeHandler(int index, string recipe, AutoResetEvent LockObj = null);

        /// <summary>
        ///     心跳信號位置
        /// </summary>
        private const int Check_Dev = 21;

        /// <summary>
        /// 保持PLC Gate連線
        /// </summary>
        private readonly Timer Checker;

        private readonly InstanceContext site;
        private bool _Gate_Status;

        /// <summary>
        ///     生產Tab頁面的Index
        /// </summary>
        private int _Index;

        /// <summary>
        ///     選取PLC的Index
        /// </summary>
        private int _ViewIndex = -1;

        private GPServiceClient PLC_Client;

        /// <summary>
        /// 回到總覽頁
        /// </summary>
        public RelayCommand BackCommand { get; }

        /// <summary>
        /// 所有PLC
        /// </summary>
        public PLC_DataProvider[] PLC_All { get; }

        /// <summary>
        /// 檢視詳細資訊的PLC
        /// </summary>
        public PLC_DataProvider PLC_In_Focused => _ViewIndex > -1 ? PLC_All[_ViewIndex] : null;

        /// <summary>
        /// 產量統計
        /// </summary>
        public ObservableConcurrentDictionary<int, int> TotalProduction { get; }
        public int TotalProductionCount => TotalProduction.Sum(x => x.Value);

        /// <summary>
        /// PLC Gate連線狀態
        /// </summary>
        public bool Gate_Status
        {
            get => _Gate_Status;
            set
            {
                _Gate_Status = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 烤箱總覽和詳細資訊檢視頁面切換index
        /// </summary>
        public int Index
        {
            get => _Index;
            set
            {
                _Index = value;
                NotifyPropertyChanged();
                if (value == 0)
                {
                    ViewIndex = -1;
                }
            }
        }

        /// <summary>
        /// PLC詳細資訊檢視index
        /// </summary>
        public int ViewIndex
        {
            get => _ViewIndex;
            set
            {
                _ViewIndex = value;
                NotifyPropertyChanged();
                if (value > -1)
                {
                    NotifyPropertyChanged(nameof(PLC_In_Focused));
                    Index = 1;
                }
            }
        }

        public event AddRecordToDBHandler AddRecordToDB;

        public event WantRecipeHandler WantRecipe;

        /// <summary>
        /// 讀取設備碼
        /// </summary>
        public void LoadMachineCodes()
        {
            if (File.Exists("MachineCodes.json"))
            {
                try
                {
                    var vals = JsonConvert.DeserializeObject<string[]>(File.ReadAllText("MachineCodes.json", Encoding.UTF8));

                    for (var i = 0; i < Math.Min(vals.Length, PLC_All.Length); i++)
                    {
                        PLC_All[i].Process_Info.MachineCode = vals[i];
                    }

                    return;
                }
                catch
                {
                }
            }

            for (var i = 0; i < PLC_All.Length; i++)
            {
                PLC_All[i].Process_Info.MachineCode = "Machine" + (i + 1).ToString("00");
            }
        }

        /// <summary>
        /// 儲存設備碼
        /// </summary>
        public void SaveMachineCodes()
        {
            try
            {
                var MachineCodes = PLC_All.Select(x => x.Process_Info.MachineCode).ToArray();
                File.WriteAllText("MachineCodes.json", JsonConvert.SerializeObject(MachineCodes), Encoding.UTF8);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 設定PLC監控讀取列表
        /// </summary>
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
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 將配方寫入PLC
        /// </summary>
        /// <param name="index">PLC序號</param>
        /// <param name="recipe">配方</param>
        /// <returns></returns>
        public async Task SetRecipe(int index, PLC_Recipe recipe)
        {
            if (recipe == null)
            {
                return;
            }

            PLC_All[index].RecipeName = recipe.RecipeName;
            PLC_All[index].TargetTemperature_1 = recipe.TargetTemperature_1;
            PLC_All[index].TargetTemperature_2 = recipe.TargetTemperature_2;
            PLC_All[index].TargetTemperature_3 = recipe.TargetTemperature_3;
            PLC_All[index].TargetTemperature_4 = recipe.TargetTemperature_4;
            PLC_All[index].TargetTemperature_5 = recipe.TargetTemperature_5;
            PLC_All[index].TargetTemperature_6 = recipe.TargetTemperature_6;
            PLC_All[index].TargetTemperature_7 = recipe.TargetTemperature_7;
            PLC_All[index].TargetTemperature_8 = recipe.TargetTemperature_8;
            PLC_All[index].HeatingTime_1 = recipe.HeatingTime_1;
            PLC_All[index].HeatingTime_2 = recipe.HeatingTime_2;
            PLC_All[index].HeatingTime_3 = recipe.HeatingTime_3;
            PLC_All[index].HeatingTime_4 = recipe.HeatingTime_4;
            PLC_All[index].HeatingTime_5 = recipe.HeatingTime_5;
            PLC_All[index].HeatingTime_6 = recipe.HeatingTime_6;
            PLC_All[index].HeatingTime_7 = recipe.HeatingTime_7;
            PLC_All[index].HeatingTime_8 = recipe.HeatingTime_8;
            PLC_All[index].ThermostaticTemperature_1 = recipe.ThermostaticTemperature_1;
            PLC_All[index].ThermostaticTemperature_2 = recipe.ThermostaticTemperature_2;
            PLC_All[index].ThermostaticTemperature_3 = recipe.ThermostaticTemperature_3;
            PLC_All[index].ThermostaticTemperature_4 = recipe.ThermostaticTemperature_4;
            PLC_All[index].ThermostaticTemperature_5 = recipe.ThermostaticTemperature_5;
            PLC_All[index].ThermostaticTemperature_6 = recipe.ThermostaticTemperature_6;
            PLC_All[index].ThermostaticTemperature_7 = recipe.ThermostaticTemperature_7;
            PLC_All[index].ThermostaticTemperature_8 = recipe.ThermostaticTemperature_8;
            PLC_All[index].WarmingTime_1 = recipe.WarmingTime_1;
            PLC_All[index].WarmingTime_2 = recipe.WarmingTime_2;
            PLC_All[index].WarmingTime_3 = recipe.WarmingTime_3;
            PLC_All[index].WarmingTime_4 = recipe.WarmingTime_4;
            PLC_All[index].WarmingTime_5 = recipe.WarmingTime_5;
            PLC_All[index].WarmingTime_6 = recipe.WarmingTime_6;
            PLC_All[index].WarmingTime_7 = recipe.WarmingTime_7;
            PLC_All[index].WarmingTime_8 = recipe.WarmingTime_8;
            PLC_All[index].CoolingTemperature = recipe.CoolingTemperature;
            PLC_All[index].InflatingTime = recipe.InflatingTime;
            PLC_All[index].UsedSegmentCounts = recipe.UsedSegmentCounts;

            if (PLC_Client?.State == CommunicationState.Opened && !PLC_All[index].IsRecording)
            {
                await PLC_Client.Set_DataAsync(DataType.D, index, PLC_All[index].Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key, x => x.Value));
            }
        }

        /// <summary>
        /// 設定使用的PLC配方(透過配方名)
        /// </summary>
        /// <param name="names">配方名</param>
        public void SetRecipeNames(ICollection<string> names)
        {
            foreach (var plc in PLC_All)
            {
                plc.Recipe_Names = names;
            }
        }

        /// <summary>
        /// 發送PC和PLC間的檢查信號
        /// </summary>
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
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 和PLC Gate連線
        /// </summary>
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
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 剛連線並初始化參數
        /// </summary>
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
            catch (Exception)
            {
                return false;
            }
        }

        public TotalView_ViewModel(int PLC_Count, IDialogService<string> dialog)
        {
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

            //!PLC的M區位置
            var M_List = new Dictionary<SignalNames, int>
                         {
                             { SignalNames.PC_ByPass, 20 },
                             { SignalNames.自動模式, 50 },
                             { SignalNames.自動啟動, 51 },
                             { SignalNames.自動停止, 52 },
                             { SignalNames.手動模式, 60 },
                             { SignalNames.降溫中, 208 },
                             { SignalNames.程式結束, 209 },
                             { SignalNames.加熱門未關, 250 },
                             { SignalNames.緊急停止, 700 },
                             { SignalNames.溫控器低溫異常, 701 },
                             { SignalNames.電源反相, 702 },
                             { SignalNames.OTP超溫異常, 703 },
                             { SignalNames.循環風車過載, 704 },
                             { SignalNames.冷卻進氣風車異常, 705 },
                             { SignalNames.超溫警報, 710 },
                             { SignalNames.停止後未開門, 715 },
                             { SignalNames.循環風車INV異常, 718 },
                             { SignalNames.充氮氣逾時, 721 },
                             { SignalNames.門未關定位異常, 722 },
                             { SignalNames.升恆溫逾時, 723 }
                         };

            //!PLC的D區位置
            var D_List = new Dictionary<DataNames, int>
                         {
                             { DataNames.溫控器溫度, 130 },
                             { DataNames.片段剩餘時間, 132 },
                             { DataNames.總剩餘時間, 134 },
                             { DataNames.目前段數, 140 },
                             { DataNames.爐內溫度_1, 380 },
                             { DataNames.爐內溫度_2, 381 },
                             { DataNames.爐內溫度_3, 382 },
                             { DataNames.爐內溫度_4, 383 },
                             { DataNames.爐內溫度_5, 384 },
                             { DataNames.爐內溫度_6, 385 },
                             { DataNames.爐內溫度_7, 386 },
                             { DataNames.爐內溫度_8, 387 }
                         };

            //!PLC的配方參數位置
            var Recipe_List = new Dictionary<DataNames, int>
                              {
                                  { DataNames.目標溫度_1, 712 },
                                  { DataNames.升溫時間_1, 713 },
                                  { DataNames.恆溫溫度_1, 714 },
                                  { DataNames.恆溫時間_1, 715 },
                                  { DataNames.目標溫度_2, 716 },
                                  { DataNames.升溫時間_2, 717 },
                                  { DataNames.恆溫溫度_2, 718 },
                                  { DataNames.恆溫時間_2, 719 },
                                  { DataNames.目標溫度_3, 720 },
                                  { DataNames.升溫時間_3, 721 },
                                  { DataNames.恆溫溫度_3, 722 },
                                  { DataNames.恆溫時間_3, 723 },
                                  { DataNames.目標溫度_4, 724 },
                                  { DataNames.升溫時間_4, 725 },
                                  { DataNames.恆溫溫度_4, 726 },
                                  { DataNames.恆溫時間_4, 727 },
                                  { DataNames.目標溫度_5, 728 },
                                  { DataNames.升溫時間_5, 729 },
                                  { DataNames.恆溫溫度_5, 730 },
                                  { DataNames.恆溫時間_5, 731 },
                                  { DataNames.目標溫度_6, 732 },
                                  { DataNames.升溫時間_6, 733 },
                                  { DataNames.恆溫溫度_6, 734 },
                                  { DataNames.恆溫時間_6, 735 },
                                  { DataNames.目標溫度_7, 736 },
                                  { DataNames.升溫時間_7, 737 },
                                  { DataNames.恆溫溫度_7, 738 },
                                  { DataNames.恆溫時間_7, 739 },
                                  { DataNames.目標溫度_8, 740 },
                                  { DataNames.升溫時間_8, 741 },
                                  { DataNames.恆溫溫度_8, 742 },
                                  { DataNames.恆溫時間_8, 743 },
                                  { DataNames.降溫溫度, 745 },
                                  { DataNames.充氣時間, 747 },
                                  { DataNames.使用段數, 749 },
                                  { DataNames.配方名稱_01, 750 },
                                  { DataNames.配方名稱_02, 751 },
                                  { DataNames.配方名稱_03, 752 },
                                  { DataNames.配方名稱_04, 753 },
                                  { DataNames.配方名稱_05, 754 },
                                  { DataNames.配方名稱_06, 755 },
                                  { DataNames.配方名稱_07, 756 },
                                  { DataNames.配方名稱_08, 757 },
                                  { DataNames.配方名稱_09, 758 },
                                  { DataNames.配方名稱_10, 759 },
                                  { DataNames.配方名稱_11, 760 },
                                  { DataNames.配方名稱_12, 761 },
                                  { DataNames.配方名稱_13, 762 }
                              };

            //!註冊PLC事件需引發的動作
            for (var i = 0; i < PLC_Count; i++)
            {
                TotalProduction.Add(i, 0);

                PLC_All[i] = new PLC_DataProvider(M_List, D_List, Recipe_List, dialog);
                var index = i;

                //!PLC由OP指定變更配方時
                PLC_All[i].SwitchRecipeEvent += recipe =>
                                                {
                                                    WantRecipe?.Invoke(index, recipe);
                                                };

                //!烤箱自動啟動時，開始紀錄
                PLC_All[i].StartRecording += (recipe, obj) =>
                                             {
                                                 WantRecipe?.Invoke(index, recipe, obj);
                                             };

                //!烘烤流程結束時
                PLC_All[i].RecordingFinished += info =>
                                                {
                                                    //if (info.ProcessCount > 0)
                                                    //{
                                                    //!寫入資料庫，上傳
                                                    AddRecordToDB?.Invoke(index, info);

                                                    //!完成上傳後，清空生產資訊
                                                    info.Clear();

                                                    TotalProduction[index] = TotalProduction[index] + info.ProcessCount;
                                                    dialog?.Show(new Dictionary<Language, string>
                                                                 {
                                                                     {Language.TW, "第" + (index + 1) + "站已完成烘烤!"},
                                                                     {Language.CHS, "第" + (index + 1) + "站已完成烘烤!"},
                                                                     {Language.EN, "Oven No" + (index + 1) + "has been finished!"},
                                                                 }, TimeSpan.FromSeconds(2));
                                                    //}
                                                };

                //!由OP變更設備代碼時
                PLC_All[i].MachineCodeChanged += code =>
                                                 {
                                                     SaveMachineCodes();
                                                 };

                //!PLC配方輸入錯誤時
                PLC_All[i].RecipeKeyInError += () =>
                                               {
                                                   dialog?.Show(new Dictionary<Language, string>
                                                                {
                                                                    {Language.TW, "第" + (index + 1) + "站配方輸入錯誤!"},
                                                                    {Language.CHS, "第" + (index + 1) + "站配方输入错误!"},
                                                                    {Language.EN, "Oven No" + (index + 1) + " recipe input error!"}
                                                                }, TimeSpan.FromSeconds(1), DialogMsgType.Alarm);
                                               };
            }

            LoadMachineCodes();

            //!產生PLC位置訂閱列表，M、D為10進制位置，B、X、Y、W為16進制
            var namelists = M_List.Values.OrderBy(x => x)
                                  .Select(x => BitType.M.ToString() + x.ToString())
                                  .Concat(D_List.Values.OrderBy(x => x).Select(x => DataType.D.ToString() + x.ToString()))
                                  .ToArray();

            var namearray = new[]
                            {
                                namelists, //1
                                namelists, //2
                                namelists, //3
                                namelists, //4
                                namelists, //5
                                namelists, //6
                                namelists, //7
                                namelists, //8
                                namelists, //9
                                namelists, //10
                                namelists, //11
                                namelists, //12
                                namelists, //13
                                namelists, //14
                                namelists, //15
                                namelists, //16
                                namelists, //17
                                namelists, //18
                                namelists, //19
                                namelists //20
                            };

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