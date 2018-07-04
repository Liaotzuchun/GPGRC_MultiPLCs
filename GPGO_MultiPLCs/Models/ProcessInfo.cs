using System;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    [BsonIgnoreExtraElements]
    public class ProcessInfo : ViewModelBase
    {
        private DateTime _EndTime;
        private ObservableConcurrentCollection<RecordEvent> _EventList = new ObservableConcurrentCollection<RecordEvent>();
        private bool _FirstPanel;
        private short _HeatingTime;
        private string _MachineCode;
        private string _OperatorID;
        private string _OrderCode;
        private int _OrderCount;
        private int _ProcessCount;
        private int _ProcessNumber;
        private string _ProduceCode;
        private string _RecipeName;
        private ObservableConcurrentCollection<RecordTemperatures> _RecordTemperatures = new ObservableConcurrentCollection<RecordTemperatures>();
        private string _Side;
        private DateTime _StartTime;
        private double _TargetOvenTemperature;
        private short _TotalHeatingTime;
        private string _TrolleyCode;
        private short _WarmingTime;

        public DateTime EndTime
        {
            get => _EndTime;
            set
            {
                _EndTime = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableConcurrentCollection<RecordEvent> EventList
        {
            get => _EventList;
            set
            {
                _EventList = value;
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

        /// <summary>
        ///     = OrderCode + ProcessNumber
        /// </summary>
        public string ProduceCode
        {
            get => _ProduceCode;
            set
            {
                _ProduceCode = value;
                NotifyPropertyChanged();
            }
        }

        public string RecipeName
        {
            get => _RecipeName;
            set
            {
                _RecipeName = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableConcurrentCollection<RecordTemperatures> RecordTemperatures
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

        #region 此區由TraceabilityView_ViewModel新增至資料庫時填入

        /// <summary>
        ///     新增至資料庫的時間
        /// </summary>
        [BsonId]
        public DateTime AddedTime { get; set; }

        /// <summary>
        ///     PLC站號
        /// </summary>
        public int StationNumber { get; set; }

        #endregion

        public void Clear()
        {
            StartTime = new DateTime();
            EndTime = new DateTime();
            OperatorID = "";
            OrderCode = "";
            OrderCount = 0;
            ProcessCount = 0;
            ProcessNumber = 0;
            ProduceCode = "";
            TrolleyCode = "";
        }
    }
}