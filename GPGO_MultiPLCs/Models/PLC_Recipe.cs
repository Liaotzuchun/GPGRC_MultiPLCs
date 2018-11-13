using System;
using System.Collections.Generic;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>PLC配方</summary>
    [OrderedObject]
    [BsonIgnoreExtraElements]
    public class PLC_Recipe : ObservableObject, IEquatable<PLC_Recipe>
    {
        public bool Equals(PLC_Recipe other)
        {
            return other != null &&
                   RecipeName == other.RecipeName &&
                   ThermostaticTemperature_1 == other.ThermostaticTemperature_1 &&
                   ThermostaticTemperature_2 == other.ThermostaticTemperature_2 &&
                   ThermostaticTemperature_3 == other.ThermostaticTemperature_3 &&
                   ThermostaticTemperature_4 == other.ThermostaticTemperature_4 &&
                   ThermostaticTemperature_5 == other.ThermostaticTemperature_5 &&
                   ThermostaticTemperature_6 == other.ThermostaticTemperature_6 &&
                   ThermostaticTemperature_7 == other.ThermostaticTemperature_7 &&
                   ThermostaticTemperature_8 == other.ThermostaticTemperature_8 &&
                   WarmingTime_1 == other.WarmingTime_1 &&
                   WarmingTime_2 == other.WarmingTime_2 &&
                   WarmingTime_3 == other.WarmingTime_3 &&
                   WarmingTime_4 == other.WarmingTime_4 &&
                   WarmingTime_5 == other.WarmingTime_5 &&
                   WarmingTime_6 == other.WarmingTime_6 &&
                   WarmingTime_7 == other.WarmingTime_7 &&
                   WarmingTime_8 == other.WarmingTime_8 &&
                   CoolingTemperature == other.CoolingTemperature &&
                   HeatingTime_1 == other.HeatingTime_1 &&
                   HeatingTime_2 == other.HeatingTime_2 &&
                   HeatingTime_3 == other.HeatingTime_3 &&
                   HeatingTime_4 == other.HeatingTime_4 &&
                   HeatingTime_5 == other.HeatingTime_5 &&
                   HeatingTime_6 == other.HeatingTime_6 &&
                   HeatingTime_7 == other.HeatingTime_7 &&
                   HeatingTime_8 == other.HeatingTime_8 &&
                   InflatingTime == other.InflatingTime &&
                   TargetTemperature_1 == other.TargetTemperature_1 &&
                   TargetTemperature_2 == other.TargetTemperature_2 &&
                   TargetTemperature_3 == other.TargetTemperature_3 &&
                   TargetTemperature_4 == other.TargetTemperature_4 &&
                   TargetTemperature_5 == other.TargetTemperature_5 &&
                   TargetTemperature_6 == other.TargetTemperature_6 &&
                   TargetTemperature_7 == other.TargetTemperature_7 &&
                   TargetTemperature_8 == other.TargetTemperature_8 &&
                   UsedSegmentCounts == other.UsedSegmentCounts;
        }

        [JsonIgnore]
        public short SegmentCounts_Max => 8;
        [JsonIgnore]
        public short SegmentCounts_Min => 1;
        [JsonIgnore]
        public double Temperature_Max => 240.0;
        [JsonIgnore]
        public double Temperature_Min => 40.0;
        [JsonIgnore]
        public short Time_Max => 600;
        [JsonIgnore]
        public short Time_Min => 1;

        [LanguageTranslator("Cooling Temperature", "降溫溫度", "降温温度")]
        public short CoolingTemperature
        {
            get => Get<short>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = (short)Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = (short)Temperature_Min;
                }

                Set(value);
            }
        }

        [JsonIgnore]
        [LanguageTranslator("Editor", "修改者", "修改者")]
        public string Editor
        {
            get => Get<string>();
            set => Set(value);
        }

        [OrderIndex(12)]
        [LanguageTranslator("Heating Time 1", "升溫時間 1", "升温时间 1")]
        public short HeatingTime_1
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(13)]
        [LanguageTranslator("Heating Time 2", "升溫時間 2", "升温时间 2")]
        public short HeatingTime_2
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(14)]
        [LanguageTranslator("Heating Time 3", "升溫時間 3", "升温时间 3")]
        public short HeatingTime_3
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(15)]
        [LanguageTranslator("Heating Time 4", "升溫時間 4", "升温时间 4")]
        public short HeatingTime_4
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(16)]
        [LanguageTranslator("Heating Time 5", "升溫時間 5", "升温时间 5")]
        public short HeatingTime_5
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(17)]
        [LanguageTranslator("Heating Time 6", "升溫時間 6", "升温时间 6")]
        public short HeatingTime_6
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(18)]
        [LanguageTranslator("Heating Time 7", "升溫時間 7", "升温时间 7")]
        public short HeatingTime_7
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(19)]
        [LanguageTranslator("Heating Time 8", "升溫時間 8", "升温时间 8")]
        public short HeatingTime_8
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(3)]
        [LanguageTranslator("Inflating Time", "充氣時間", "充气时间")]
        public short InflatingTime
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [JsonIgnore]
        [OrderIndex(0)]
        [LanguageTranslator("Recipe Name", "配方名稱", "配方名称")]
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
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(5)]
        [LanguageTranslator("Target Temperature 2", "目標溫度 2", "目标温度 2")]
        public double TargetTemperature_2
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(6)]
        [LanguageTranslator("Target Temperature 3", "目標溫度 3", "目标温度 3")]
        public double TargetTemperature_3
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(7)]
        [LanguageTranslator("Target Temperature 4", "目標溫度 4", "目标温度 4")]
        public double TargetTemperature_4
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(8)]
        [LanguageTranslator("Target Temperature 5", "目標溫度 5", "目标温度 5")]
        public double TargetTemperature_5
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(9)]
        [LanguageTranslator("Target Temperature 6", "目標溫度 6", "目标温度 6")]
        public double TargetTemperature_6
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(10)]
        [LanguageTranslator("Target Temperature 7", "目標溫度 7", "目标温度 7")]
        public double TargetTemperature_7
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(11)]
        [LanguageTranslator("Target Temperature 8", "目標溫度 8", "目标温度 8")]
        public double TargetTemperature_8
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(20)]
        [LanguageTranslator("Thermostatic Temperature 1", "恆溫溫度 1", "恒温温度 1")]
        public double ThermostaticTemperature_1
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(21)]
        [LanguageTranslator("Thermostatic Temperature 2", "恆溫溫度 2", "恒温温度 2")]
        public double ThermostaticTemperature_2
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(22)]
        [LanguageTranslator("Thermostatic Temperature 3", "恆溫溫度 3", "恒温温度 3")]
        public double ThermostaticTemperature_3
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(23)]
        [LanguageTranslator("Thermostatic Temperature 4", "恆溫溫度 4", "恒温温度 4")]
        public double ThermostaticTemperature_4
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(24)]
        [LanguageTranslator("Thermostatic Temperature 5", "恆溫溫度 5", "恒温温度 5")]
        public double ThermostaticTemperature_5
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(25)]
        [LanguageTranslator("Thermostatic Temperature 6", "恆溫溫度 6", "恒温温度 6")]
        public double ThermostaticTemperature_6
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(26)]
        [LanguageTranslator("Thermostatic Temperature 7", "恆溫溫度 7", "恒温温度 7")]
        public double ThermostaticTemperature_7
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(27)]
        [LanguageTranslator("Thermostatic Temperature 8", "恆溫溫度 8", "恒温温度 8")]
        public double ThermostaticTemperature_8
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < Temperature_Min)
                {
                    value = Temperature_Min;
                }

                Set(value);
            }
        }

        [JsonIgnore]
        [OrderIndex(1)]
        [LanguageTranslator("Updated Time", "更新時間", "更新时间")]
        public DateTime Updated
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        [JsonIgnore]
        [LanguageTranslator("Used Stations", "使用站點", "使用站点")]
        public IList<bool> Used_Stations
        {
            get => Get<IList<bool>>();
            set => Set(value);
        }

        [OrderIndex(2)]
        [LanguageTranslator("Used Segment Counts", "使用段數", "使用段数")]
        public short UsedSegmentCounts
        {
            get => Get<short>();
            set
            {
                if (value > SegmentCounts_Max)
                {
                    value = SegmentCounts_Max;
                }
                else if (value < SegmentCounts_Min)
                {
                    value = SegmentCounts_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(28)]
        [LanguageTranslator("Warning Time 1", "恆溫時間 1", "恒温时间 1")]
        public short WarmingTime_1
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(29)]
        [LanguageTranslator("Warning Time 2", "恆溫時間 2", "恒温时间 2")]
        public short WarmingTime_2
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(30)]
        [LanguageTranslator("Warning Time 3", "恆溫時間 3", "恒温时间 3")]
        public short WarmingTime_3
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(31)]
        [LanguageTranslator("Warning Time 4", "恆溫時間 4", "恒温时间 4")]
        public short WarmingTime_4
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(32)]
        [LanguageTranslator("Warning Time 5", "恆溫時間 5", "恒温时间 5")]
        public short WarmingTime_5
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(33)]
        [LanguageTranslator("Warning Time 6", "恆溫時間 6", "恒温时间 6")]
        public short WarmingTime_6
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(34)]
        [LanguageTranslator("Warning Time 7", "恆溫時間 7", "恒温时间 7")]
        public short WarmingTime_7
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        [OrderIndex(35)]
        [LanguageTranslator("Warning Time 8", "恆溫時間 8", "恒温时间 8")]
        public short WarmingTime_8
        {
            get => Get<short>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < Time_Min)
                {
                    value = Time_Min;
                }

                Set(value);
            }
        }

        public PLC_Recipe Copy(string user)
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
                       Used_Stations = Used_Stations,
                       Editor = user
                   };
        }

        public void CopyValue(string user, PLC_Recipe recipe)
        {
            Updated = DateTime.Now;
            RecipeName = recipe.RecipeName;
            ThermostaticTemperature_1 = recipe.ThermostaticTemperature_1;
            ThermostaticTemperature_2 = recipe.ThermostaticTemperature_2;
            ThermostaticTemperature_3 = recipe.ThermostaticTemperature_3;
            ThermostaticTemperature_4 = recipe.ThermostaticTemperature_4;
            ThermostaticTemperature_5 = recipe.ThermostaticTemperature_5;
            ThermostaticTemperature_6 = recipe.ThermostaticTemperature_6;
            ThermostaticTemperature_7 = recipe.ThermostaticTemperature_7;
            ThermostaticTemperature_8 = recipe.ThermostaticTemperature_8;
            WarmingTime_1 = recipe.WarmingTime_1;
            WarmingTime_2 = recipe.WarmingTime_2;
            WarmingTime_3 = recipe.WarmingTime_3;
            WarmingTime_4 = recipe.WarmingTime_4;
            WarmingTime_5 = recipe.WarmingTime_5;
            WarmingTime_6 = recipe.WarmingTime_6;
            WarmingTime_7 = recipe.WarmingTime_7;
            WarmingTime_8 = recipe.WarmingTime_8;
            CoolingTemperature = recipe.CoolingTemperature;
            HeatingTime_1 = recipe.HeatingTime_1;
            HeatingTime_2 = recipe.HeatingTime_2;
            HeatingTime_3 = recipe.HeatingTime_3;
            HeatingTime_4 = recipe.HeatingTime_4;
            HeatingTime_5 = recipe.HeatingTime_5;
            HeatingTime_6 = recipe.HeatingTime_6;
            HeatingTime_7 = recipe.HeatingTime_7;
            HeatingTime_8 = recipe.HeatingTime_8;
            InflatingTime = recipe.InflatingTime;
            TargetTemperature_1 = recipe.TargetTemperature_1;
            TargetTemperature_2 = recipe.TargetTemperature_2;
            TargetTemperature_3 = recipe.TargetTemperature_3;
            TargetTemperature_4 = recipe.TargetTemperature_4;
            TargetTemperature_5 = recipe.TargetTemperature_5;
            TargetTemperature_6 = recipe.TargetTemperature_6;
            TargetTemperature_7 = recipe.TargetTemperature_7;
            TargetTemperature_8 = recipe.TargetTemperature_8;
            UsedSegmentCounts = recipe.UsedSegmentCounts;
            Editor = user;
        }

        public PLC_Recipe(string name = "", string user = "")
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
            Editor = user;
        }

        public PLC_Recipe()
        {
            Used_Stations = new bool[20];
        }
    }
}