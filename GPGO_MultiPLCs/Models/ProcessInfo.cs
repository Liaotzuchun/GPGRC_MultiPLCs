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
        private int _HeatingTime;
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
        private int _TotalHeatingTime;
        private string _TrolleyCode;
        private int _WarmingTime;

        /// <summary>
        /// 條碼類型
        /// </summary>
        public CodeType CodeType
        {
            get => _CodeType;
            set
            {
                _CodeType = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime EndTime
        {
            get => _EndTime;
            set
            {
                _EndTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 事件紀錄
        /// </summary>
        public ObservableConcurrentCollection<RecordEvent> EventList
        {
            get => _EventList;
            set
            {
                _EventList = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 是否為首件
        /// </summary>
        public bool FirstPanel
        {
            get => _FirstPanel;
            set
            {
                _FirstPanel = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 加熱時間(升溫至目標溫度)
        /// </summary>
        public int HeatingTime
        {
            get => _HeatingTime;
            set
            {
                _HeatingTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 機台編號
        /// </summary>
        public string MachineCode
        {
            get => _MachineCode;
            set
            {
                _MachineCode = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 操作人員ID
        /// </summary>
        public string OperatorID
        {
            get => _OperatorID;
            set
            {
                _OperatorID = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 工單號
        /// </summary>
        public string OrderCode
        {
            get => _OrderCode;
            set
            {
                _OrderCode = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 工單材料總量
        /// </summary>
        public int OrderCount
        {
            get => _OrderCount;
            set
            {
                _OrderCount = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 單一製造序材料數量
        /// </summary>
        public int ProcessCount
        {
            get => _ProcessCount;
            set
            {
                _ProcessCount = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 製造序
        /// </summary>
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
        /// 條碼 = OrderCode + ProcessNumber
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

        /// <summary>
        /// 配方名
        /// </summary>
        public string RecipeName
        {
            get => _RecipeName;
            set
            {
                _RecipeName = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 紀錄溫度
        /// </summary>
        public ObservableConcurrentCollection<RecordTemperatures> RecordTemperatures
        {
            get => _RecordTemperatures;
            set
            {
                _RecordTemperatures = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 正反面
        /// </summary>
        public string Side
        {
            get => _Side;
            set
            {
                _Side = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartTime
        {
            get => _StartTime;
            set
            {
                _StartTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 目標溫度
        /// </summary>
        public double TargetOvenTemperature
        {
            get => _TargetOvenTemperature;
            set
            {
                _TargetOvenTemperature = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 總烘烤時間
        /// </summary>
        public int TotalHeatingTime
        {
            get => _TotalHeatingTime;
            set
            {
                _TotalHeatingTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 台車編號
        /// </summary>
        public string TrolleyCode
        {
            get => _TrolleyCode;
            set
            {
                _TrolleyCode = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 恆溫時間
        /// </summary>
        public int WarmingTime
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
            stb.AppendLine("");
            stb.Append("General13=");
            stb.AppendLine(_Side);
            stb.Append("General14=");
            stb.AppendLine("");
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