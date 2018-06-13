using System;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    [BsonIgnoreExtraElements]
    public class PLC_Recipe : ViewModelBase
    {
        private short _ConstantTime_1;
        private short _ConstantTime_2;
        private short _ConstantTime_3;
        private short _ConstantTime_4;
        private short _ConstantTime_5;
        private short _ConstantTime_6;
        private short _ConstantTime_7;
        private short _ConstantTime_8;
        private short _CoolingTemperature;
        private short _HeatingTime_1;
        private short _HeatingTime_2;
        private short _HeatingTime_3;
        private short _HeatingTime_4;
        private short _HeatingTime_5;
        private short _HeatingTime_6;
        private short _HeatingTime_7;
        private short _HeatingTime_8;
        private short _InflatingTime;
        private string _RecipeName;
        private double _TargetTemperature_1;
        private double _TargetTemperature_2;
        private double _TargetTemperature_3;
        private double _TargetTemperature_4;
        private double _TargetTemperature_5;
        private double _TargetTemperature_6;
        private double _TargetTemperature_7;
        private double _TargetTemperature_8;
        private double _ThermostaticTemperature_1;
        private double _ThermostaticTemperature_2;
        private double _ThermostaticTemperature_3;
        private double _ThermostaticTemperature_4;
        private double _ThermostaticTemperature_5;
        private double _ThermostaticTemperature_6;
        private double _ThermostaticTemperature_7;
        private double _ThermostaticTemperature_8;
        private DateTime _Updated;
        private bool[] _Used_Stations = new bool[Connector.PLC_Count];
        private short _UsedSegmentCounts;

        public short SegmentCounts_Max => 8;
        public short SegmentCounts_Min => 1;

        public double Temperature_Max => 240.0;
        public double Temperature_Min => 40.0;

        public short Time_Max => 600;
        public short Time_Min => 1;

        public short ConstantTime_1
        {
            get => _ConstantTime_1;
            set
            {
                _ConstantTime_1 = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_2
        {
            get => _ConstantTime_2;
            set
            {
                _ConstantTime_2 = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_3
        {
            get => _ConstantTime_3;
            set
            {
                _ConstantTime_3 = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_4
        {
            get => _ConstantTime_4;
            set
            {
                _ConstantTime_4 = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_5
        {
            get => _ConstantTime_5;
            set
            {
                _ConstantTime_5 = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_6
        {
            get => _ConstantTime_6;
            set
            {
                _ConstantTime_6 = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_7
        {
            get => _ConstantTime_7;
            set
            {
                _ConstantTime_7 = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_8
        {
            get => _ConstantTime_8;
            set
            {
                _ConstantTime_8 = value;
                NotifyPropertyChanged();
            }
        }

        public short CoolingTemperature
        {
            get => _CoolingTemperature;
            set
            {
                _CoolingTemperature = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_1
        {
            get => _HeatingTime_1;
            set
            {
                _HeatingTime_1 = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_2
        {
            get => _HeatingTime_2;
            set
            {
                _HeatingTime_2 = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_3
        {
            get => _HeatingTime_3;
            set
            {
                _HeatingTime_3 = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_4
        {
            get => _HeatingTime_4;
            set
            {
                _HeatingTime_4 = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_5
        {
            get => _HeatingTime_5;
            set
            {
                _HeatingTime_5 = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_6
        {
            get => _HeatingTime_6;
            set
            {
                _HeatingTime_6 = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_7
        {
            get => _HeatingTime_7;
            set
            {
                _HeatingTime_7 = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_8
        {
            get => _HeatingTime_8;
            set
            {
                _HeatingTime_8 = value;
                NotifyPropertyChanged();
            }
        }

        public short InflatingTime
        {
            get => _InflatingTime;
            set
            {
                _InflatingTime = value;
                NotifyPropertyChanged();
            }
        }

        [BsonId]
        public string RecipeName
        {
            get => _RecipeName;
            set
            {
                _RecipeName = value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_1
        {
            get => _TargetTemperature_1;
            set
            {
                _TargetTemperature_1 = value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_2
        {
            get => _TargetTemperature_2;
            set
            {
                _TargetTemperature_2 = value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_3
        {
            get => _TargetTemperature_3;
            set
            {
                _TargetTemperature_3 = value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_4
        {
            get => _TargetTemperature_4;
            set
            {
                _TargetTemperature_4 = value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_5
        {
            get => _TargetTemperature_5;
            set
            {
                _TargetTemperature_5 = value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_6
        {
            get => _TargetTemperature_6;
            set
            {
                _TargetTemperature_6 = value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_7
        {
            get => _TargetTemperature_7;
            set
            {
                _TargetTemperature_7 = value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_8
        {
            get => _TargetTemperature_8;
            set
            {
                _TargetTemperature_8 = value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_1
        {
            get => _ThermostaticTemperature_1;
            set
            {
                _ThermostaticTemperature_1 = value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_2
        {
            get => _ThermostaticTemperature_2;
            set
            {
                _ThermostaticTemperature_2 = value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_3
        {
            get => _ThermostaticTemperature_3;
            set
            {
                _ThermostaticTemperature_3 = value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_4
        {
            get => _ThermostaticTemperature_4;
            set
            {
                _ThermostaticTemperature_4 = value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_5
        {
            get => _ThermostaticTemperature_5;
            set
            {
                _ThermostaticTemperature_5 = value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_6
        {
            get => _ThermostaticTemperature_6;
            set
            {
                _ThermostaticTemperature_6 = value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_7
        {
            get => _ThermostaticTemperature_7;
            set
            {
                _ThermostaticTemperature_7 = value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_8
        {
            get => _ThermostaticTemperature_8;
            set
            {
                _ThermostaticTemperature_8 = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime Updated
        {
            get => _Updated;
            set
            {
                _Updated = value;
                NotifyPropertyChanged();
            }
        }

        public bool[] Used_Stations
        {
            get => _Used_Stations;
            set
            {
                _Used_Stations = value;
                NotifyPropertyChanged();
            }
        }

        public short UsedSegmentCounts
        {
            get => _UsedSegmentCounts;
            set
            {
                _UsedSegmentCounts = value;
                NotifyPropertyChanged();
            }
        }

        public PLC_Recipe(string name = "")
        {
            Updated = DateTime.Now;
            RecipeName = string.IsNullOrEmpty(name) ? Updated.Ticks.ToString() : name;
            ThermostaticTemperature_1 = 200;
            ThermostaticTemperature_2 = 200;
            ThermostaticTemperature_3 = 200;
            ThermostaticTemperature_4 = 200;
            ThermostaticTemperature_5 = 200;
            ThermostaticTemperature_6 = 200;
            ThermostaticTemperature_7 = 200;
            ThermostaticTemperature_8 = 200;
            ConstantTime_1 = 10;
            ConstantTime_2 = 10;
            ConstantTime_3 = 10;
            ConstantTime_4 = 10;
            ConstantTime_5 = 10;
            ConstantTime_6 = 10;
            ConstantTime_7 = 10;
            ConstantTime_8 = 10;
            CoolingTemperature = 40;
            HeatingTime_1 = 10;
            HeatingTime_2 = 10;
            HeatingTime_3 = 10;
            HeatingTime_4 = 10;
            HeatingTime_5 = 10;
            HeatingTime_6 = 10;
            HeatingTime_7 = 10;
            HeatingTime_8 = 10;
            InflatingTime = 10;
            TargetTemperature_1 = 200;
            TargetTemperature_2 = 200;
            TargetTemperature_3 = 200;
            TargetTemperature_4 = 200;
            TargetTemperature_5 = 200;
            TargetTemperature_6 = 200;
            TargetTemperature_7 = 200;
            TargetTemperature_8 = 200;
            UsedSegmentCounts = 8;
        }

        public PLC_Recipe Copy()
        {
            return new PLC_Recipe
                   {
                       Updated = DateTime.Now,
                       RecipeName = _RecipeName,
                       ThermostaticTemperature_1 = _ThermostaticTemperature_1,
                       ThermostaticTemperature_2 = _ThermostaticTemperature_2,
                       ThermostaticTemperature_3 = _ThermostaticTemperature_3,
                       ThermostaticTemperature_4 = _ThermostaticTemperature_4,
                       ThermostaticTemperature_5 = _ThermostaticTemperature_5,
                       ThermostaticTemperature_6 = _ThermostaticTemperature_6,
                       ThermostaticTemperature_7 = _ThermostaticTemperature_7,
                       ThermostaticTemperature_8 = _ThermostaticTemperature_8,
                       ConstantTime_1 = _ConstantTime_1,
                       ConstantTime_2 = _ConstantTime_2,
                       ConstantTime_3 = _ConstantTime_3,
                       ConstantTime_4 = _ConstantTime_4,
                       ConstantTime_5 = _ConstantTime_5,
                       ConstantTime_6 = _ConstantTime_6,
                       ConstantTime_7 = _ConstantTime_7,
                       ConstantTime_8 = _ConstantTime_8,
                       CoolingTemperature = _CoolingTemperature,
                       HeatingTime_1 = _HeatingTime_1,
                       HeatingTime_2 = _HeatingTime_2,
                       HeatingTime_3 = _HeatingTime_3,
                       HeatingTime_4 = _HeatingTime_4,
                       HeatingTime_5 = _HeatingTime_5,
                       HeatingTime_6 = _HeatingTime_6,
                       HeatingTime_7 = _HeatingTime_7,
                       HeatingTime_8 = _HeatingTime_8,
                       InflatingTime = _InflatingTime,
                       TargetTemperature_1 = _TargetTemperature_1,
                       TargetTemperature_2 = _TargetTemperature_2,
                       TargetTemperature_3 = _TargetTemperature_3,
                       TargetTemperature_4 = _TargetTemperature_4,
                       TargetTemperature_5 = _TargetTemperature_5,
                       TargetTemperature_6 = _TargetTemperature_6,
                       TargetTemperature_7 = _TargetTemperature_7,
                       TargetTemperature_8 = _TargetTemperature_8,
                       UsedSegmentCounts = _UsedSegmentCounts,
                       Used_Stations = _Used_Stations
                   };
        }
    }
}