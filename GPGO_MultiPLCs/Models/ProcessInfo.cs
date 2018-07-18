using System;
using System.Text;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    public enum CodeType
    {
        Panel,
        SubPanel,
        JobNo
    }

    [BsonIgnoreExtraElements]
    public class ProcessInfo : ViewModelBase
    {
        private CodeType _CodeType = CodeType.Panel;
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

        public CodeType CodeType
        {
            get => _CodeType;
            set
            {
                _CodeType = value;
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

        public new string ToString()
        {
            var stb = new StringBuilder();
            stb.Append("General1=");
            stb.AppendLine(_OrderCode);
            stb.Append("General2=");
            stb.AppendLine(_ProcessNumber.ToString("000"));
            stb.Append("General3=");
            stb.AppendLine(_StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            stb.Append("General4=");
            stb.AppendLine(_EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            stb.Append("General5=");
            stb.AppendLine(_MachineCode);
            stb.Append("General6=");
            stb.AppendLine(_CodeType.ToString());
            stb.Append("General7=");
            stb.AppendLine(_ProduceCode);
            stb.Append("General8=");
            stb.AppendLine(_RecipeName);
            stb.Append("General9=");
            stb.AppendLine(_ProcessCount.ToString());
            stb.Append("General10=");
            stb.AppendLine(_OrderCount.ToString());
            stb.Append("General11=");
            stb.AppendLine(_OperatorID);
            stb.Append("General12=");
            stb.AppendLine(""); //!治具編號
            stb.Append("General13=");
            stb.AppendLine(_Side);
            stb.Append("General14=");
            stb.AppendLine(""); //!PCS序號
            stb.Append("General15=");
            stb.AppendLine(_FirstPanel ? "Y" : "N");
            stb.Append("Machine1=");
            stb.AppendLine(_TrolleyCode);
            stb.Append("Machine2=");
            stb.AppendLine(_TargetOvenTemperature.ToString("0"));
            stb.Append("Machine3=");
            stb.AppendLine(_WarmingTime.ToString());
            stb.Append("Machine4=");
            stb.AppendLine(_HeatingTime.ToString());
            stb.Append("Machine5=");
            stb.AppendLine(_TotalHeatingTime.ToString());
            stb.Append("Machine6=");
            stb.AppendLine("");

            return stb.ToString();
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
    }
}