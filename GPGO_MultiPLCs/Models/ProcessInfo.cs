using System;
using System.Collections.Generic;
using System.Linq;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    public class TimeWithTemperature
    {
        public double Temperature { get; set; }
        public TimeSpan Time { get; set; }

        public override string ToString()
        {
            return $"Time:{Time:hh\\:mm\\:ss}, Temperature:{Temperature:F1}°C";
        }
    }

    [BsonIgnoreExtraElements]
    public class ProcessInfo : ViewModelBase
    {
        private List<RecordEvent> _EventList = new List<RecordEvent>();
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
        private string _RecipeName;
        private List<RecordTemperatures> _RecordTemperatures = new List<RecordTemperatures>();
        private string _Side;
        private DateTime _StartTime;
        private double _TargetOvenTemperature;
        private short _TotalHeatingTime;
        private string _TrolleyCode;
        private short _WarmingTime;
        public IEnumerable<TimeWithTemperature> OvenTemperatures_0 => RecordTemperatures.Select(x => new TimeWithTemperature { Time = x.Time, Temperature = x.OvenTemperatures[0] });
        public IEnumerable<TimeWithTemperature> OvenTemperatures_1 => RecordTemperatures.Select(x => new TimeWithTemperature { Time = x.Time, Temperature = x.OvenTemperatures[1] });
        public IEnumerable<TimeWithTemperature> OvenTemperatures_2 => RecordTemperatures.Select(x => new TimeWithTemperature { Time = x.Time, Temperature = x.OvenTemperatures[2] });
        public IEnumerable<TimeWithTemperature> OvenTemperatures_3 => RecordTemperatures.Select(x => new TimeWithTemperature { Time = x.Time, Temperature = x.OvenTemperatures[3] });
        public IEnumerable<TimeWithTemperature> OvenTemperatures_4 => RecordTemperatures.Select(x => new TimeWithTemperature { Time = x.Time, Temperature = x.OvenTemperatures[4] });
        public IEnumerable<TimeWithTemperature> OvenTemperatures_5 => RecordTemperatures.Select(x => new TimeWithTemperature { Time = x.Time, Temperature = x.OvenTemperatures[5] });
        public IEnumerable<TimeWithTemperature> OvenTemperatures_6 => RecordTemperatures.Select(x => new TimeWithTemperature { Time = x.Time, Temperature = x.OvenTemperatures[6] });
        public IEnumerable<TimeWithTemperature> OvenTemperatures_7 => RecordTemperatures.Select(x => new TimeWithTemperature { Time = x.Time, Temperature = x.OvenTemperatures[7] });
        public IEnumerable<TimeWithTemperature> ThermostatTemperature => RecordTemperatures.Select(x => new TimeWithTemperature { Time = x.Time, Temperature = x.ThermostatTemperature });

        public List<RecordEvent> EventList
        {
            get => _EventList;
            set
            {
                _EventList = value;
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