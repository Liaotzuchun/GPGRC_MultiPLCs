using System;
using System.Collections.Generic;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    [BsonIgnoreExtraElements]
    public class ProcessInfo : ViewModelBase
    {

        #region 此區由TraceabilityView_ViewModel新增至資料庫時填入
        /// <summary>
        /// 新增至資料庫的時間
        /// </summary>
        [BsonId]
        public DateTime AddedTime;

        /// <summary>
        /// PLC站號
        /// </summary>
        public int StationNumber;
        #endregion

        private Dictionary<TimeSpan, string> _AlarmList = new Dictionary<TimeSpan, string>();
        private DateTime _EndTime;
        private bool _FirstPanel;
        private short _HeatingTime;
        private string _JigCode;
        private string _MachineCode;
        private string _OperatorID;
        private string _OrderCode;
        private int _OrderCount;
        private int _PCS_Number;
        private int _ProcessCount;
        private int _ProcessNumber;
        private string _ProduceCode;
        private List<RecordTemperatures> _RecordTemperatures = new List<RecordTemperatures>();
        private string _Side;
        private DateTime _StartTime;
        private double _TargetOvenTemperature;
        private short _TotalHeatingTime;
        private string _TrolleyCode;
        private short _WarmingTime;

        public Dictionary<TimeSpan, string> AlarmList
        {
            get => _AlarmList;
            set
            {
                _AlarmList = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime EndTime
        {
            get => _EndTime;
            set
            {
                _EndTime = value;
                NotifyPropertyChanged();
            }
        }

        public bool FirstPanel
        {
            get => _FirstPanel;
            set
            {
                _FirstPanel = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime
        {
            get => _HeatingTime;
            set
            {
                _HeatingTime = value;
                NotifyPropertyChanged();
            }
        }

        public string JigCode
        {
            get => _JigCode;
            set
            {
                _JigCode = value;
                NotifyPropertyChanged();
            }
        }

        public string MachineCode
        {
            get => _MachineCode;
            set
            {
                _MachineCode = value;
                NotifyPropertyChanged();
            }
        }

        public string OperatorID
        {
            get => _OperatorID;
            set
            {
                _OperatorID = value;
                NotifyPropertyChanged();
            }
        }

        public string OrderCode
        {
            get => _OrderCode;
            set
            {
                _OrderCode = value;
                NotifyPropertyChanged();
            }
        }

        public int OrderCount
        {
            get => _OrderCount;
            set
            {
                _OrderCount = value;
                NotifyPropertyChanged();
            }
        }

        public int PCS_Number
        {
            get => _PCS_Number;
            set
            {
                _PCS_Number = value;
                NotifyPropertyChanged();
            }
        }

        public int ProcessCount
        {
            get => _ProcessCount;
            set
            {
                _ProcessCount = value;
                NotifyPropertyChanged();
            }
        }

        public int ProcessNumber
        {
            get => _ProcessNumber;
            set
            {
                _ProcessNumber = value;
                NotifyPropertyChanged();
            }
        }

        public string ProduceCode
        {
            get => _ProduceCode;
            set
            {
                _ProduceCode = value;
                NotifyPropertyChanged();
            }
        }

        public List<RecordTemperatures> RecordTemperatures
        {
            get => _RecordTemperatures;
            set
            {
                _RecordTemperatures = value;
                NotifyPropertyChanged();
            }
        }

        public string Side
        {
            get => _Side;
            set
            {
                _Side = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime StartTime
        {
            get => _StartTime;
            set
            {
                _StartTime = value;
                NotifyPropertyChanged();
            }
        }

        public double TargetOvenTemperature
        {
            get => _TargetOvenTemperature;
            set
            {
                _TargetOvenTemperature = value;
                NotifyPropertyChanged();
            }
        }

        public short TotalHeatingTime
        {
            get => _TotalHeatingTime;
            set
            {
                _TotalHeatingTime = value;
                NotifyPropertyChanged();
            }
        }

        public string TrolleyCode
        {
            get => _TrolleyCode;
            set
            {
                _TrolleyCode = value;
                NotifyPropertyChanged();
            }
        }

        public short WarmingTime
        {
            get => _WarmingTime;
            set
            {
                _WarmingTime = value;
                NotifyPropertyChanged();
            }
        }
    }
}