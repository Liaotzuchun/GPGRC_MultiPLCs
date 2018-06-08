using System;
using System.Collections.Generic;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    [BsonIgnoreExtraElements]
    public class ProcessInfo : ViewModelBase
    {
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
        private List<Record_Temperatures> _RecordTemperatures = new List<Record_Temperatures>();
        private string _Side;
        private DateTime _StartTime;
        private double _TargetOvenTemperature;
        private short _TotalHeatingTime;
        private string _TrolleyCode;
        private short _WarmingTime;

        [BsonId]
        public DateTime AddedTime;

        public int StationNumber;

        public List<Record_Temperatures> RecordTemperatures
        {
            get => _RecordTemperatures;
            set
            {
                _RecordTemperatures = value;
                NotifyPropertyChanged();
            }
        }

        public Dictionary<TimeSpan, string> AlarmList
        {
            get => _AlarmList;
            set
            {
                _AlarmList = value;
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

        public int ProcessNumber
        {
            get => _ProcessNumber;
            set
            {
                _ProcessNumber = value;
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

        public DateTime EndTime
        {
            get => _EndTime;
            set
            {
                _EndTime = value;
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

        public string ProduceCode
        {
            get => _ProduceCode;
            set
            {
                _ProduceCode = value;
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

        public int ProcessCount
        {
            get => _ProcessCount;
            set
            {
                _ProcessCount = value;
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

        public string JigCode
        {
            get => _JigCode;
            set
            {
                _JigCode = value;
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

        public int PCS_Number
        {
            get => _PCS_Number;
            set
            {
                _PCS_Number = value;
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

        public string TrolleyCode
        {
            get => _TrolleyCode;
            set
            {
                _TrolleyCode = value;
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

        public short HeatingTime
        {
            get => _HeatingTime;
            set
            {
                _HeatingTime = value;
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

        public short TotalHeatingTime
        {
            get => _TotalHeatingTime;
            set
            {
                _TotalHeatingTime = value;
                NotifyPropertyChanged();
            }
        }
    }
}