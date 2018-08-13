using System;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>PLC配方</summary>
    [BsonIgnoreExtraElements]
    public class PLC_Recipe : ObservableObject
    {
        public short SegmentCounts_Max => 8;
        public short SegmentCounts_Min => 1;

        public double Temperature_Max => 240.0;
        public double Temperature_Min => 40.0;

        public short Time_Max => 600;
        public short Time_Min => 1;

        public short CoolingTemperature
        {
            get => Get<short>();
            set => Set(value);
        }

        public short HeatingTime_1
        {
            get => Get<short>();
            set => Set(value);
        }

        public short HeatingTime_2
        {
            get => Get<short>();
            set => Set(value);
        }

        public short HeatingTime_3
        {
            get => Get<short>();
            set => Set(value);
        }

        public short HeatingTime_4
        {
            get => Get<short>();
            set => Set(value);
        }

        public short HeatingTime_5
        {
            get => Get<short>();
            set => Set(value);
        }

        public short HeatingTime_6
        {
            get => Get<short>();
            set => Set(value);
        }

        public short HeatingTime_7
        {
            get => Get<short>();
            set => Set(value);
        }

        public short HeatingTime_8
        {
            get => Get<short>();
            set => Set(value);
        }

        public short InflatingTime
        {
            get => Get<short>();
            set => Set(value);
        }

        [BsonId]
        public string RecipeName
        {
            get => Get<string>();
            set => Set(value);
        }

        public double TargetTemperature_1
        {
            get => Get<double>();
            set => Set(value);
        }

        public double TargetTemperature_2
        {
            get => Get<double>();
            set => Set(value);
        }

        public double TargetTemperature_3
        {
            get => Get<double>();
            set => Set(value);
        }

        public double TargetTemperature_4
        {
            get => Get<double>();
            set => Set(value);
        }

        public double TargetTemperature_5
        {
            get => Get<double>();
            set => Set(value);
        }

        public double TargetTemperature_6
        {
            get => Get<double>();
            set => Set(value);
        }

        public double TargetTemperature_7
        {
            get => Get<double>();
            set => Set(value);
        }

        public double TargetTemperature_8
        {
            get => Get<double>();
            set => Set(value);
        }

        public double ThermostaticTemperature_1
        {
            get => Get<double>();
            set => Set(value);
        }

        public double ThermostaticTemperature_2
        {
            get => Get<double>();
            set => Set(value);
        }

        public double ThermostaticTemperature_3
        {
            get => Get<double>();
            set => Set(value);
        }

        public double ThermostaticTemperature_4
        {
            get => Get<double>();
            set => Set(value);
        }

        public double ThermostaticTemperature_5
        {
            get => Get<double>();
            set => Set(value);
        }

        public double ThermostaticTemperature_6
        {
            get => Get<double>();
            set => Set(value);
        }

        public double ThermostaticTemperature_7
        {
            get => Get<double>();
            set => Set(value);
        }

        public double ThermostaticTemperature_8
        {
            get => Get<double>();
            set => Set(value);
        }

        public DateTime Updated
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        public bool[] Used_Stations
        {
            get => Get<bool[]>();
            set => Set(value);
        }

        public short UsedSegmentCounts
        {
            get => Get<short>();
            set => Set(value);
        }

        public short WarmingTime_1
        {
            get => Get<short>();
            set => Set(value);
        }

        public short WarmingTime_2
        {
            get => Get<short>();
            set => Set(value);
        }

        public short WarmingTime_3
        {
            get => Get<short>();
            set => Set(value);
        }

        public short WarmingTime_4
        {
            get => Get<short>();
            set => Set(value);
        }

        public short WarmingTime_5
        {
            get => Get<short>();
            set => Set(value);
        }

        public short WarmingTime_6
        {
            get => Get<short>();
            set => Set(value);
        }

        public short WarmingTime_7
        {
            get => Get<short>();
            set => Set(value);
        }

        public short WarmingTime_8
        {
            get => Get<short>();
            set => Set(value);
        }

        public PLC_Recipe Copy()
        {
            return new PLC_Recipe
                   {
                       Updated = DateTime.Now,
                       RecipeName = RecipeName,
                       ThermostaticTemperature_1 = ThermostaticTemperature_1,
                       ThermostaticTemperature_2 = ThermostaticTemperature_2,
                       ThermostaticTemperature_3 = ThermostaticTemperature_3,
                       ThermostaticTemperature_4 = ThermostaticTemperature_4,
                       ThermostaticTemperature_5 = ThermostaticTemperature_5,
                       ThermostaticTemperature_6 = ThermostaticTemperature_6,
                       ThermostaticTemperature_7 = ThermostaticTemperature_7,
                       ThermostaticTemperature_8 = ThermostaticTemperature_8,
                       WarmingTime_1 = WarmingTime_1,
                       WarmingTime_2 = WarmingTime_2,
                       WarmingTime_3 = WarmingTime_3,
                       WarmingTime_4 = WarmingTime_4,
                       WarmingTime_5 = WarmingTime_5,
                       WarmingTime_6 = WarmingTime_6,
                       WarmingTime_7 = WarmingTime_7,
                       WarmingTime_8 = WarmingTime_8,
                       CoolingTemperature = CoolingTemperature,
                       HeatingTime_1 = HeatingTime_1,
                       HeatingTime_2 = HeatingTime_2,
                       HeatingTime_3 = HeatingTime_3,
                       HeatingTime_4 = HeatingTime_4,
                       HeatingTime_5 = HeatingTime_5,
                       HeatingTime_6 = HeatingTime_6,
                       HeatingTime_7 = HeatingTime_7,
                       HeatingTime_8 = HeatingTime_8,
                       InflatingTime = InflatingTime,
                       TargetTemperature_1 = TargetTemperature_1,
                       TargetTemperature_2 = TargetTemperature_2,
                       TargetTemperature_3 = TargetTemperature_3,
                       TargetTemperature_4 = TargetTemperature_4,
                       TargetTemperature_5 = TargetTemperature_5,
                       TargetTemperature_6 = TargetTemperature_6,
                       TargetTemperature_7 = TargetTemperature_7,
                       TargetTemperature_8 = TargetTemperature_8,
                       UsedSegmentCounts = UsedSegmentCounts,
                       Used_Stations = Used_Stations
                   };
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
            WarmingTime_1 = 10;
            WarmingTime_2 = 10;
            WarmingTime_3 = 10;
            WarmingTime_4 = 10;
            WarmingTime_5 = 10;
            WarmingTime_6 = 10;
            WarmingTime_7 = 10;
            WarmingTime_8 = 10;
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

        public PLC_Recipe()
        {
            Used_Stations = new bool[20];
        }
    }
}