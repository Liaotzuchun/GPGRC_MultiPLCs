using System;
using System.Collections.Generic;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>PLC配方</summary>
    [OrderedObject]
    [BsonIgnoreExtraElements]
    public class PLC_Recipe : ObservableObject
    {
        public short SegmentCounts_Max => 8;
        public short SegmentCounts_Min => 1;

        public double Temperature_Max => 240.0;
        public double Temperature_Min => 40.0;

        public short Time_Max => 600;
        public short Time_Min => 1;

        [LanguageTranslator("Cooling Temperature", "降溫溫度", "降温温度")]
        public short CoolingTemperature
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(12)]
        [LanguageTranslator("Heating Time 1", "升溫時間 1", "升温时间 1")]
        public short HeatingTime_1
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(13)]
        [LanguageTranslator("Heating Time 2", "升溫時間 2", "升温时间 2")]
        public short HeatingTime_2
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(14)]
        [LanguageTranslator("Heating Time 3", "升溫時間 3", "升温时间 3")]
        public short HeatingTime_3
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(15)]
        [LanguageTranslator("Heating Time 4", "升溫時間 4", "升温时间 4")]
        public short HeatingTime_4
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(16)]
        [LanguageTranslator("Heating Time 5", "升溫時間 5", "升温时间 5")]
        public short HeatingTime_5
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(17)]
        [LanguageTranslator("Heating Time 6", "升溫時間 6", "升温时间 6")]
        public short HeatingTime_6
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(18)]
        [LanguageTranslator("Heating Time 7", "升溫時間 7", "升温时间 7")]
        public short HeatingTime_7
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(19)]
        [LanguageTranslator("Heating Time 8", "升溫時間 8", "升温时间 8")]
        public short HeatingTime_8
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(3)]
        [LanguageTranslator("Inflating Time", "充氣時間", "充气时间")]
        public short InflatingTime
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(0)]
        [LanguageTranslator("Recipe Name", "配方名稱", "配方名称")]
        [BsonId]
        public string RecipeName
        {
            get => Get<string>();
            set => Set(value);
        }

        [OrderIndex(4)]
        [LanguageTranslator("Target Temperature 1", "目標溫度 1", "目标温度 1")]
        public double TargetTemperature_1
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(5)]
        [LanguageTranslator("Target Temperature 2", "目標溫度 2", "目标温度 2")]
        public double TargetTemperature_2
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(6)]
        [LanguageTranslator("Target Temperature 3", "目標溫度 3", "目标温度 3")]
        public double TargetTemperature_3
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(7)]
        [LanguageTranslator("Target Temperature 4", "目標溫度 4", "目标温度 4")]
        public double TargetTemperature_4
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(8)]
        [LanguageTranslator("Target Temperature 5", "目標溫度 5", "目标温度 5")]
        public double TargetTemperature_5
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(9)]
        [LanguageTranslator("Target Temperature 6", "目標溫度 6", "目标温度 6")]
        public double TargetTemperature_6
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(10)]
        [LanguageTranslator("Target Temperature 7", "目標溫度 7", "目标温度 7")]
        public double TargetTemperature_7
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(11)]
        [LanguageTranslator("Target Temperature 8", "目標溫度 8", "目标温度 8")]
        public double TargetTemperature_8
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(20)]
        [LanguageTranslator("Thermostatic Temperature 1", "恆溫溫度 1", "恒温温度 1")]
        public double ThermostaticTemperature_1
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(21)]
        [LanguageTranslator("Thermostatic Temperature 2", "恆溫溫度 2", "恒温温度 2")]
        public double ThermostaticTemperature_2
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(22)]
        [LanguageTranslator("Thermostatic Temperature 3", "恆溫溫度 3", "恒温温度 3")]
        public double ThermostaticTemperature_3
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(23)]
        [LanguageTranslator("Thermostatic Temperature 4", "恆溫溫度 4", "恒温温度 4")]
        public double ThermostaticTemperature_4
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(24)]
        [LanguageTranslator("Thermostatic Temperature 5", "恆溫溫度 5", "恒温温度 5")]
        public double ThermostaticTemperature_5
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(25)]
        [LanguageTranslator("Thermostatic Temperature 6", "恆溫溫度 6", "恒温温度 6")]
        public double ThermostaticTemperature_6
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(26)]
        [LanguageTranslator("Thermostatic Temperature 7", "恆溫溫度 7", "恒温温度 7")]
        public double ThermostaticTemperature_7
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(27)]
        [LanguageTranslator("Thermostatic Temperature 8", "恆溫溫度 8", "恒温温度 8")]
        public double ThermostaticTemperature_8
        {
            get => Get<double>();
            set => Set(value);
        }

        [OrderIndex(1)]
        [LanguageTranslator("Updated Time", "更新時間", "更新时间")]
        public DateTime Updated
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        [LanguageTranslator("Used Stations", "使用站點", "使用站点")]
        public IList<bool> Used_Stations
        {
            get => Get <IList<bool>>();
            set => Set(value);
        }

        [OrderIndex(2)]
        [LanguageTranslator("Used Segment Counts", "使用段數", "使用段数")]
        public short UsedSegmentCounts
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(28)]
        [LanguageTranslator("Warning Time 1", "恆溫時間 1", "恒温时间 1")]
        public short WarmingTime_1
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(29)]
        [LanguageTranslator("Warning Time 2", "恆溫時間 2", "恒温时间 2")]
        public short WarmingTime_2
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(30)]
        [LanguageTranslator("Warning Time 3", "恆溫時間 3", "恒温时间 3")]
        public short WarmingTime_3
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(31)]
        [LanguageTranslator("Warning Time 4", "恆溫時間 4", "恒温时间 4")]
        public short WarmingTime_4
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(32)]
        [LanguageTranslator("Warning Time 5", "恆溫時間 5", "恒温时间 5")]
        public short WarmingTime_5
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(33)]
        [LanguageTranslator("Warning Time 6", "恆溫時間 6", "恒温时间 6")]
        public short WarmingTime_6
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(34)]
        [LanguageTranslator("Warning Time 7", "恆溫時間 7", "恒温时间 7")]
        public short WarmingTime_7
        {
            get => Get<short>();
            set => Set(value);
        }

        [OrderIndex(35)]
        [LanguageTranslator("Warning Time 8", "恆溫時間 8", "恒温时间 8")]
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
            Used_Stations = new bool[20];
        }

        public PLC_Recipe()
        {
            Used_Stations = new bool[20];
        }
    }
}