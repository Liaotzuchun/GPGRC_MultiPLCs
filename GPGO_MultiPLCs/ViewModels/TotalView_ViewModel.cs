using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using GPGO_MultiPLCs.GP_PLCs;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.ViewModels
{
    public class TotalView_ViewModel : ViewModelBase, IGPServiceCallback
    {
        void IGPServiceCallback.Status_Changed(int index, bool val)
        {
            if (index < PLC_Count)
            {
                PLC_In_All[index].OnlineStatus = val;
            }
        }

        void IGPServiceCallback.M_Changed(int index, Dictionary<int, bool> val)
        {
            if (index < PLC_Count)
            {
                foreach (var v in val)
                {
                    PLC_In_All[index].M_Values[v.Key] = v.Value;
                }
            }
        }

        void IGPServiceCallback.D_Changed(int index, Dictionary<int, short> val)
        {
            if (index < PLC_Count)
            {
                foreach (var v in val)
                {
                    PLC_In_All[index].D_Values[v.Key] = v.Value;
                }
            }
        }

        private const int PLC_Count = 20;
        private const int Check_Dev = 21; //心跳信號位置
        private readonly Timer Checker;
        private readonly InstanceContext site;
        private bool _Gate_Status;

        private int _ViewIndex;
        private GPServiceClient PLC_Client;

        public PLC_Data[] PLC_In_All;

        public int ViewIndex
        {
            get => _ViewIndex;
            set
            {
                _ViewIndex = value;
                NotifyPropertyChanged();
            }
        }

        public bool Gate_Status
        {
            get => _Gate_Status;
            set
            {
                _Gate_Status = value;
                NotifyPropertyChanged();
            }
        }

        public PLC_Data PLC_In_Focused => _ViewIndex > -1 ? PLC_In_All[_ViewIndex] : null;

        public TotalView_ViewModel()
        {
            site = new InstanceContext(this);

            PLC_In_All = new PLC_Data[PLC_Count];

            var M_List = new Dictionary<SignalNames, int>
                         {
                             { SignalNames.PC_ByPass, 20 },
                             { SignalNames.自動模式, 50 },
                             { SignalNames.自動啟動, 51 },
                             { SignalNames.自動停止, 52 },
                             { SignalNames.手動模式, 60 },
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

            var D_List = new Dictionary<DataNames, int>
                         {
                             { DataNames.溫控器溫度, 130 },
                             { DataNames.片段剩餘時間, 132 },
                             { DataNames.總剩餘時間, 134 },
                             { DataNames.爐內溫度_1, 380 },
                             { DataNames.爐內溫度_2, 381 },
                             { DataNames.爐內溫度_3, 382 },
                             { DataNames.爐內溫度_4, 383 },
                             { DataNames.爐內溫度_5, 384 },
                             { DataNames.爐內溫度_6, 385 },
                             { DataNames.爐內溫度_7, 386 },
                             { DataNames.爐內溫度_8, 387 }
                         };

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

            for (var i = 0; i < PLC_Count; i++)
            {
                PLC_In_All[i] = new PLC_Data(M_List, D_List, Recipe_List);
            }

            var namelists = M_List.Values.OrderBy(x => x).Select(x => "M" + x.ToString()).Concat(D_List.Values.OrderBy(x => x).Select(x => "D" + x.ToString())).ToList();
            var namearray = new[]
                            {
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray()
                            };

            Checker = new Timer(o =>
                                {
                                    if (!Gate_Status)
                                    {
                                        if (Connect() && Initial() && SetReadLists(namearray))
                                        {
                                            Gate_Status = true;
                                        }
                                    }
                                    else if (!Check())
                                    {
                                        Gate_Status = false;
                                    }

                                    Checker.Change(150, Timeout.Infinite);
                                },
                                null,
                                0,
                                Timeout.Infinite);
        }

        public bool SetReadLists(string[][] list)
        {
            try
            {
                if (PLC_Client.State != CommunicationState.Opened)
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

        private bool Connect()
        {
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

        private bool Initial()
        {
            try
            {
                if (PLC_Client.State != CommunicationState.Opened)
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

        private bool Check()
        {
            try
            {
                if (PLC_Client.State != CommunicationState.Opened)
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
    }
}