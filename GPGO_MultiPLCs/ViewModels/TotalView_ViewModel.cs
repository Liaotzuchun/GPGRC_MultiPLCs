using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly AsyncOperation AO = AsyncOperationManager.CreateOperation(null);
        private readonly Timer Checker;
        private readonly InstanceContext site;
        private GPServiceClient PLC_Client;

        public PLC_Data[] PLC_In_All;

        private int _ViewIndex;
        private bool _Gate_Status;

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

            //for (var i = 0; i < PLC_Count; i++)
            //{
            //    PLC_In_All = new PLC_Data();
            //}

            Checker = new Timer(o =>
                                {
                                    if (!Gate_Status)
                                    {
                                        if (Connect() && Initial())
                                        {
                                            Gate_Status = true;
                                        }
                                    }
                                    else if (!Check())
                                    {
                                        Gate_Status = false;
                                    }

                                    Checker.Change(90, Timeout.Infinite);
                                },
                                null,
                                0,
                                Timeout.Infinite);
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

                PLC_Client.CheckSignal();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
    }
}
