using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GPMVVM.Helpers;
using GPMVVM.Models;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>PLC配方</summary>
    [OrderedObject]
    [BsonIgnoreExtraElements]
    public class PLC_Recipe : RecipeBase<PLC_Recipe>
    {
        public override bool Equals(PLC_Recipe other) =>
            other != null &&
            RecipeName == other.RecipeName &&
            ThermostaticTemperature_1.ToString("0.0") == other.ThermostaticTemperature_1.ToString("0.0") &&
            ThermostaticTemperature_2.ToString("0.0") == other.ThermostaticTemperature_2.ToString("0.0") &&
            ThermostaticTemperature_3.ToString("0.0") == other.ThermostaticTemperature_3.ToString("0.0") &&
            ThermostaticTemperature_4.ToString("0.0") == other.ThermostaticTemperature_4.ToString("0.0") &&
            ThermostaticTemperature_5.ToString("0.0") == other.ThermostaticTemperature_5.ToString("0.0") &&
            ThermostaticTemperature_6.ToString("0.0") == other.ThermostaticTemperature_6.ToString("0.0") &&
            ThermostaticTemperature_7.ToString("0.0") == other.ThermostaticTemperature_7.ToString("0.0") &&
            ThermostaticTemperature_8.ToString("0.0") == other.ThermostaticTemperature_8.ToString("0.0") &&
            WarmingTime_1.ToString("0.0") == other.WarmingTime_1.ToString("0.0") &&
            WarmingTime_2.ToString("0.0") == other.WarmingTime_2.ToString("0.0") &&
            WarmingTime_3.ToString("0.0") == other.WarmingTime_3.ToString("0.0") &&
            WarmingTime_4.ToString("0.0") == other.WarmingTime_4.ToString("0.0") &&
            WarmingTime_5.ToString("0.0") == other.WarmingTime_5.ToString("0.0") &&
            WarmingTime_6.ToString("0.0") == other.WarmingTime_6.ToString("0.0") &&
            WarmingTime_7.ToString("0.0") == other.WarmingTime_7.ToString("0.0") &&
            WarmingTime_8.ToString("0.0") == other.WarmingTime_8.ToString("0.0") &&
            WarmingAlarm_1.ToString("0.0") == other.WarmingAlarm_1.ToString("0.0") &&
            WarmingAlarm_2.ToString("0.0") == other.WarmingAlarm_2.ToString("0.0") &&
            WarmingAlarm_3.ToString("0.0") == other.WarmingAlarm_3.ToString("0.0") &&
            WarmingAlarm_4.ToString("0.0") == other.WarmingAlarm_4.ToString("0.0") &&
            WarmingAlarm_5.ToString("0.0") == other.WarmingAlarm_5.ToString("0.0") &&
            WarmingAlarm_6.ToString("0.0") == other.WarmingAlarm_6.ToString("0.0") &&
            WarmingAlarm_7.ToString("0.0") == other.WarmingAlarm_7.ToString("0.0") &&
            WarmingAlarm_8.ToString("0.0") == other.WarmingAlarm_8.ToString("0.0") &&
            CoolingTime.ToString("0.0") == other.CoolingTime.ToString("0.0") &&
            CoolingTemperature.ToString("0.0") == other.CoolingTemperature.ToString("0.0") &&
            HeatingTime_1.ToString("0.0") == other.HeatingTime_1.ToString("0.0") &&
            HeatingTime_2.ToString("0.0") == other.HeatingTime_2.ToString("0.0") &&
            HeatingTime_3.ToString("0.0") == other.HeatingTime_3.ToString("0.0") &&
            HeatingTime_4.ToString("0.0") == other.HeatingTime_4.ToString("0.0") &&
            HeatingTime_5.ToString("0.0") == other.HeatingTime_5.ToString("0.0") &&
            HeatingTime_6.ToString("0.0") == other.HeatingTime_6.ToString("0.0") &&
            HeatingTime_7.ToString("0.0") == other.HeatingTime_7.ToString("0.0") &&
            HeatingTime_8.ToString("0.0") == other.HeatingTime_8.ToString("0.0") &&
            HeatingAlarm_1.ToString("0.0") == other.HeatingAlarm_1.ToString("0.0") &&
            HeatingAlarm_2.ToString("0.0") == other.HeatingAlarm_2.ToString("0.0") &&
            HeatingAlarm_3.ToString("0.0") == other.HeatingAlarm_3.ToString("0.0") &&
            HeatingAlarm_4.ToString("0.0") == other.HeatingAlarm_4.ToString("0.0") &&
            HeatingAlarm_5.ToString("0.0") == other.HeatingAlarm_5.ToString("0.0") &&
            HeatingAlarm_6.ToString("0.0") == other.HeatingAlarm_6.ToString("0.0") &&
            HeatingAlarm_7.ToString("0.0") == other.HeatingAlarm_7.ToString("0.0") &&
            HeatingAlarm_8.ToString("0.0") == other.HeatingAlarm_8.ToString("0.0") &&
            InflatingTime == other.InflatingTime &&
            ProgramStopAlarmTime.ToString("0.0") == other.ProgramStopAlarmTime.ToString("0.0") &&
            TargetTemperature_1.ToString("0.0") == other.TargetTemperature_1.ToString("0.0") &&
            TargetTemperature_2.ToString("0.0") == other.TargetTemperature_2.ToString("0.0") &&
            TargetTemperature_3.ToString("0.0") == other.TargetTemperature_3.ToString("0.0") &&
            TargetTemperature_4.ToString("0.0") == other.TargetTemperature_4.ToString("0.0") &&
            TargetTemperature_5.ToString("0.0") == other.TargetTemperature_5.ToString("0.0") &&
            TargetTemperature_6.ToString("0.0") == other.TargetTemperature_6.ToString("0.0") &&
            TargetTemperature_7.ToString("0.0") == other.TargetTemperature_7.ToString("0.0") &&
            TargetTemperature_8.ToString("0.0") == other.TargetTemperature_8.ToString("0.0") &&
            UsedSegmentCounts == other.UsedSegmentCounts;

        [JsonIgnore] public short SegmentCounts_Max => 6;

        [JsonIgnore] public short SegmentCounts_Min => 1;

        [JsonIgnore] public double Temperature_Max => 200.0;

        [JsonIgnore] public double Temperature_Min => 30.0;

        [JsonIgnore] public double Time_Max => 200;

        [JsonIgnore] public double Time_Min => 5;

        [GPIgnore]
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

        [GPIgnore]
        [LanguageTranslator("Cooling Temp.", "降溫溫度", "降温温度")]
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

        [LanguageTranslator("Cooling Time", "降溫時間", "降温时间")]
        public double CoolingTime
        {
            get => Get<double>();
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

        [LanguageTranslator("ProgramStopAlarmTime", "程式結束警報時間", "程序结束警报时间")]
        public double ProgramStopAlarmTime
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 1)
                {
                    value = 1;
                }

                Set(value);
            }
        }

        [JsonIgnore] //匯出時檔名已是名稱，此處用JsonIgnore是用來做配方比較需忽略此項
        [OrderIndex(0)]
        [LanguageTranslator("Recipe Name", "配方名稱", "配方名称")]
        public override string RecipeName { get; set; }

        [GPIgnore]
        [JsonIgnore]
        [OrderIndex(1)]
        [LanguageTranslator("Updated Time", "更新時間", "更新时间")]
        public override DateTime Updated { get; set; }

        [JsonIgnore]
        [OrderIndex(2)]
        [LanguageTranslator("Editor", "修改者", "修改者")]
        public override string Editor { get; set; }

        [GPIgnore]
        [JsonIgnore]
        [OrderIndex(3)]
        [LanguageTranslator("Editor Level", "權限", "权限")]
        public override UserLevel EditorLevel { get; set; }

        [GPIgnore]
        [OrderIndex(4)]
        [LanguageTranslator("Inflating Time", "充氣時間", "充气时间")]
        public double InflatingTime
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                Set(value);
            }
        }

        [OrderIndex(5)]
        [LanguageTranslator("Target Temp. 1", "目標溫度 1", "目标温度 1")]
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
                TargetTemperature_2 = TargetTemperature_2;
            }
        }

        [OrderIndex(6)]
        [LanguageTranslator("Target Temp. 2", "目標溫度 2", "目标温度 2")]
        public double TargetTemperature_2
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TargetTemperature_1)
                {
                    value = TargetTemperature_1;
                }

                Set(value);
                TargetTemperature_3 = TargetTemperature_3;
            }
        }

        [OrderIndex(7)]
        [LanguageTranslator("Target Temp. 3", "目標溫度 3", "目标温度 3")]
        public double TargetTemperature_3
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TargetTemperature_2)
                {
                    value = TargetTemperature_2;
                }

                Set(value);
                TargetTemperature_4 = TargetTemperature_4;
            }
        }

        [OrderIndex(8)]
        [LanguageTranslator("Target Temp. 4", "目標溫度 4", "目标温度 4")]
        public double TargetTemperature_4
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TargetTemperature_3)
                {
                    value = TargetTemperature_3;
                }

                Set(value);
                TargetTemperature_5 = TargetTemperature_5;
            }
        }

        [OrderIndex(9)]
        [LanguageTranslator("Target Temp. 5", "目標溫度 5", "目标温度 5")]
        public double TargetTemperature_5
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TargetTemperature_4)
                {
                    value = TargetTemperature_4;
                }

                Set(value);
                TargetTemperature_6 = TargetTemperature_6;
            }
        }

        [OrderIndex(10)]
        [LanguageTranslator("Target Temp. 6", "目標溫度 6", "目标温度 6")]
        public double TargetTemperature_6
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TargetTemperature_5)
                {
                    value = TargetTemperature_5;
                }

                Set(value);
                TargetTemperature_7 = TargetTemperature_7;
            }
        }

        [OrderIndex(11)]
        [LanguageTranslator("Target Temp. 7", "目標溫度 7", "目标温度 7")]
        public double TargetTemperature_7
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TargetTemperature_6)
                {
                    value = TargetTemperature_6;
                }

                Set(value);
                TargetTemperature_8 = TargetTemperature_8;
            }
        }

        [OrderIndex(12)]
        [LanguageTranslator("Target Temp. 8", "目標溫度 8", "目标温度 8")]
        public double TargetTemperature_8
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TargetTemperature_7)
                {
                    value = TargetTemperature_7;
                }

                Set(value);
            }
        }

        [OrderIndex(13)]
        [LanguageTranslator("Heating Time 1", "升溫時間 1", "升温时间 1")]
        public double HeatingTime_1
        {
            get => Get<double>();
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
                HeatingAlarm_1 = HeatingAlarm_1;
            }
        }

        [OrderIndex(14)]
        [LanguageTranslator("Heating Time 2", "升溫時間 2", "升温时间 2")]
        public double HeatingTime_2
        {
            get => Get<double>();
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
                HeatingAlarm_2 = HeatingAlarm_2;
            }
        }

        [OrderIndex(15)]
        [LanguageTranslator("Heating Time 3", "升溫時間 3", "升温时间 3")]
        public double HeatingTime_3
        {
            get => Get<double>();
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
                HeatingAlarm_3 = HeatingAlarm_3;
            }
        }

        [OrderIndex(16)]
        [LanguageTranslator("Heating Time 4", "升溫時間 4", "升温时间 4")]
        public double HeatingTime_4
        {
            get => Get<double>();
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
                HeatingAlarm_4 = HeatingAlarm_4;
            }
        }

        [OrderIndex(17)]
        [LanguageTranslator("Heating Time 5", "升溫時間 5", "升温时间 5")]
        public double HeatingTime_5
        {
            get => Get<double>();
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
                HeatingAlarm_5 = HeatingAlarm_5;
            }
        }

        [OrderIndex(18)]
        [LanguageTranslator("Heating Time 6", "升溫時間 6", "升温时间 6")]
        public double HeatingTime_6
        {
            get => Get<double>();
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
                HeatingAlarm_6 = HeatingAlarm_6;
            }
        }

        [GPIgnore]
        [OrderIndex(19)]
        [LanguageTranslator("Heating Time 7", "升溫時間 7", "升温时间 7")]
        public double HeatingTime_7
        {
            get => Get<double>();
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
                HeatingAlarm_7 = HeatingAlarm_7;
            }
        }

        [GPIgnore]
        [OrderIndex(20)]
        [LanguageTranslator("Heating Time 8", "升溫時間 8", "升温时间 8")]
        public double HeatingTime_8
        {
            get => Get<double>();
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
                HeatingAlarm_8 = HeatingAlarm_8;
            }
        }

        [OrderIndex(21)]
        [LanguageTranslator("Heating Alarm 1", "升溫警報 1", "升温警报 1")]
        public double HeatingAlarm_1
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < HeatingTime_1 + 1)
                {
                    value = HeatingTime_1 + 1;
                }

                Set(value);
            }
        }

        [OrderIndex(22)]
        [LanguageTranslator("Heating Alarm 2", "升溫警報 2", "升温警报 2")]
        public double HeatingAlarm_2
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < HeatingTime_2 + 1)
                {
                    value = HeatingTime_2 + 1;
                }

                Set(value);
            }
        }

        [OrderIndex(23)]
        [LanguageTranslator("Heating Alarm 3", "升溫警報 3", "升温警报 3")]
        public double HeatingAlarm_3
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < HeatingTime_3 + 1)
                {
                    value = HeatingTime_3 + 1;
                }

                Set(value);
            }
        }

        [OrderIndex(24)]
        [LanguageTranslator("Heating Alarm 4", "升溫警報 4", "升温警报 4")]
        public double HeatingAlarm_4
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < HeatingTime_4 + 1)
                {
                    value = HeatingTime_4 + 1;
                }

                Set(value);
            }
        }

        [OrderIndex(25)]
        [LanguageTranslator("Heating Alarm 5", "升溫警報 5", "升温警报 5")]
        public double HeatingAlarm_5
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < HeatingTime_5 + 1)
                {
                    value = HeatingTime_5 + 1;
                }

                Set(value);
            }
        }

        [OrderIndex(26)]
        [LanguageTranslator("Heating Alarm 6", "升溫警報 6", "升温警报 6")]
        public double HeatingAlarm_6
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < HeatingTime_6 + 1)
                {
                    value = HeatingTime_6 + 1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(27)]
        [LanguageTranslator("Heating Alarm 7", "升溫警報 7", "升温警报 7")]
        public double HeatingAlarm_7
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < HeatingTime_7 + 1)
                {
                    value = HeatingTime_7 + 1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(28)]
        [LanguageTranslator("Heating Alarm 8", "升溫警報 8", "升温警报 8")]
        public double HeatingAlarm_8
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < HeatingTime_8 + 1)
                {
                    value = HeatingTime_8 + 1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(29)]
        [LanguageTranslator("Thermostatic Temp. 1", "恆溫溫度 1", "恒温温度 1")]
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

        [GPIgnore]
        [OrderIndex(30)]
        [LanguageTranslator("Thermostatic Temp. 2", "恆溫溫度 2", "恒温温度 2")]
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

        [GPIgnore]
        [OrderIndex(31)]
        [LanguageTranslator("Thermostatic Temp. 3", "恆溫溫度 3", "恒温温度 3")]
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

        [GPIgnore]
        [OrderIndex(32)]
        [LanguageTranslator("Thermostatic Temp. 4", "恆溫溫度 4", "恒温温度 4")]
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

        [GPIgnore]
        [OrderIndex(33)]
        [LanguageTranslator("Thermostatic Temp. 5", "恆溫溫度 5", "恒温温度 5")]
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

        [GPIgnore]
        [OrderIndex(34)]
        [LanguageTranslator("Thermostatic Temp. 6", "恆溫溫度 6", "恒温温度 6")]
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

        [GPIgnore]
        [OrderIndex(35)]
        [LanguageTranslator("Thermostatic Temp. 7", "恆溫溫度 7", "恒温温度 7")]
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

        [GPIgnore]
        [OrderIndex(36)]
        [LanguageTranslator("Thermostatic Temp. 8", "恆溫溫度 8", "恒温温度 8")]
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

        [OrderIndex(37)]
        [LanguageTranslator("Warning Time 1", "恆溫時間 1", "恒温时间 1")]
        public double WarmingTime_1
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                Set(value);
                WarmingAlarm_1 = WarmingAlarm_1;
            }
        }

        [OrderIndex(38)]
        [LanguageTranslator("Warning Time 2", "恆溫時間 2", "恒温时间 2")]
        public double WarmingTime_2
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                Set(value);
                WarmingAlarm_2 = WarmingAlarm_2;
            }
        }

        [OrderIndex(39)]
        [LanguageTranslator("Warning Time 3", "恆溫時間 3", "恒温时间 3")]
        public double WarmingTime_3
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                Set(value);
                WarmingAlarm_3 = WarmingAlarm_3;
            }
        }

        [OrderIndex(40)]
        [LanguageTranslator("Warning Time 4", "恆溫時間 4", "恒温时间 4")]
        public double WarmingTime_4
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                Set(value);
                WarmingAlarm_4 = WarmingAlarm_4;
            }
        }

        [OrderIndex(41)]
        [LanguageTranslator("Warning Time 5", "恆溫時間 5", "恒温时间 5")]
        public double WarmingTime_5
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                Set(value);
                WarmingAlarm_5 = WarmingAlarm_5;
            }
        }

        [OrderIndex(42)]
        [LanguageTranslator("Warning Time 6", "恆溫時間 6", "恒温时间 6")]
        public double WarmingTime_6
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                Set(value);
                WarmingAlarm_6 = WarmingAlarm_6;
            }
        }

        [GPIgnore]
        [OrderIndex(43)]
        [LanguageTranslator("Warning Time 7", "恆溫時間 7", "恒温时间 7")]
        public double WarmingTime_7
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                Set(value);
                WarmingAlarm_7 = WarmingAlarm_7;
            }
        }

        [GPIgnore]
        [OrderIndex(44)]
        [LanguageTranslator("Warning Time 8", "恆溫時間 8", "恒温时间 8")]
        public double WarmingTime_8
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                Set(value);
                WarmingAlarm_8 = WarmingAlarm_8;
            }
        }

        [OrderIndex(45)]
        [LanguageTranslator("Warming Alarm 1", "恆溫警報 1", "恒温警报 1")]
        public double WarmingAlarm_1
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < WarmingTime_1 + 0.1)
                {
                    value = WarmingTime_1 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(46)]
        [LanguageTranslator("Warming Alarm 2", "恆溫警報 2", "恒温警报 2")]
        public double WarmingAlarm_2
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < WarmingTime_2 + 0.1)
                {
                    value = WarmingTime_2 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(47)]
        [LanguageTranslator("Warming Alarm 3", "恆溫警報 3", "恒温警报 3")]
        public double WarmingAlarm_3
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < WarmingTime_3 + 0.1)
                {
                    value = WarmingTime_3 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(48)]
        [LanguageTranslator("Warming Alarm 4", "恆溫警報 4", "恒温警报 4")]
        public double WarmingAlarm_4
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < WarmingTime_4 + 0.1)
                {
                    value = WarmingTime_4 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(49)]
        [LanguageTranslator("Warming Alarm 5", "恆溫警報 5", "恒温警报 5")]
        public double WarmingAlarm_5
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < WarmingTime_5 + 0.1)
                {
                    value = WarmingTime_5 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(50)]
        [LanguageTranslator("Warming Alarm 6", "恆溫警報 6", "恒温警报 6")]
        public double WarmingAlarm_6
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < WarmingTime_6 + 0.1)
                {
                    value = WarmingTime_6 + 0.1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(51)]
        [LanguageTranslator("Warming Alarm 7", "恆溫警報 7", "恒温警报 7")]
        public double WarmingAlarm_7
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < WarmingTime_7 + 0.1)
                {
                    value = WarmingTime_7 + 0.1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(52)]
        [LanguageTranslator("Warming Alarm 8", "恆溫警報 8", "恒温警报 8")]
        public double WarmingAlarm_8
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < WarmingTime_8 + 0.1)
                {
                    value = WarmingTime_8 + 0.1;
                }

                Set(value);
            }
        }

        public override PLC_Recipe Copy(string user, UserLevel level) =>
            new PLC_Recipe
            {
                Updated                   = DateTime.Now,
                RecipeName                = RecipeName,
                ThermostaticTemperature_1 = ThermostaticTemperature_1,
                ThermostaticTemperature_2 = ThermostaticTemperature_2,
                ThermostaticTemperature_3 = ThermostaticTemperature_3,
                ThermostaticTemperature_4 = ThermostaticTemperature_4,
                ThermostaticTemperature_5 = ThermostaticTemperature_5,
                ThermostaticTemperature_6 = ThermostaticTemperature_6,
                ThermostaticTemperature_7 = ThermostaticTemperature_7,
                ThermostaticTemperature_8 = ThermostaticTemperature_8,
                WarmingTime_1             = WarmingTime_1,
                WarmingTime_2             = WarmingTime_2,
                WarmingTime_3             = WarmingTime_3,
                WarmingTime_4             = WarmingTime_4,
                WarmingTime_5             = WarmingTime_5,
                WarmingTime_6             = WarmingTime_6,
                WarmingTime_7             = WarmingTime_7,
                WarmingTime_8             = WarmingTime_8,
                WarmingAlarm_1            = WarmingAlarm_1,
                WarmingAlarm_2            = WarmingAlarm_2,
                WarmingAlarm_3            = WarmingAlarm_3,
                WarmingAlarm_4            = WarmingAlarm_4,
                WarmingAlarm_5            = WarmingAlarm_5,
                WarmingAlarm_6            = WarmingAlarm_6,
                WarmingAlarm_7            = WarmingAlarm_7,
                WarmingAlarm_8            = WarmingAlarm_8,
                CoolingTime               = CoolingTime,
                CoolingTemperature        = CoolingTemperature,
                HeatingTime_1             = HeatingTime_1,
                HeatingTime_2             = HeatingTime_2,
                HeatingTime_3             = HeatingTime_3,
                HeatingTime_4             = HeatingTime_4,
                HeatingTime_5             = HeatingTime_5,
                HeatingTime_6             = HeatingTime_6,
                HeatingTime_7             = HeatingTime_7,
                HeatingTime_8             = HeatingTime_8,
                HeatingAlarm_1            = HeatingAlarm_1,
                HeatingAlarm_2            = HeatingAlarm_2,
                HeatingAlarm_3            = HeatingAlarm_3,
                HeatingAlarm_4            = HeatingAlarm_4,
                HeatingAlarm_5            = HeatingAlarm_5,
                HeatingAlarm_6            = HeatingAlarm_6,
                HeatingAlarm_7            = HeatingAlarm_7,
                HeatingAlarm_8            = HeatingAlarm_8,
                InflatingTime             = InflatingTime,
                ProgramStopAlarmTime      = ProgramStopAlarmTime,
                TargetTemperature_1       = TargetTemperature_1,
                TargetTemperature_2       = TargetTemperature_2,
                TargetTemperature_3       = TargetTemperature_3,
                TargetTemperature_4       = TargetTemperature_4,
                TargetTemperature_5       = TargetTemperature_5,
                TargetTemperature_6       = TargetTemperature_6,
                TargetTemperature_7       = TargetTemperature_7,
                TargetTemperature_8       = TargetTemperature_8,
                UsedSegmentCounts         = UsedSegmentCounts,
                Used_Stations             = Used_Stations,
                Editor                    = user,
                EditorLevel               = level
            };

        public override void CopyValue(string user, UserLevel level, PLC_Recipe recipe)
        {
            Updated                   = DateTime.Now;
            ThermostaticTemperature_1 = recipe.ThermostaticTemperature_1;
            ThermostaticTemperature_2 = recipe.ThermostaticTemperature_2;
            ThermostaticTemperature_3 = recipe.ThermostaticTemperature_3;
            ThermostaticTemperature_4 = recipe.ThermostaticTemperature_4;
            ThermostaticTemperature_5 = recipe.ThermostaticTemperature_5;
            ThermostaticTemperature_6 = recipe.ThermostaticTemperature_6;
            ThermostaticTemperature_7 = recipe.ThermostaticTemperature_7;
            ThermostaticTemperature_8 = recipe.ThermostaticTemperature_8;
            WarmingTime_1             = recipe.WarmingTime_1;
            WarmingTime_2             = recipe.WarmingTime_2;
            WarmingTime_3             = recipe.WarmingTime_3;
            WarmingTime_4             = recipe.WarmingTime_4;
            WarmingTime_5             = recipe.WarmingTime_5;
            WarmingTime_6             = recipe.WarmingTime_6;
            WarmingTime_7             = recipe.WarmingTime_7;
            WarmingTime_8             = recipe.WarmingTime_8;
            CoolingTime               = recipe.CoolingTime;
            CoolingTemperature        = recipe.CoolingTemperature;
            HeatingTime_1             = recipe.HeatingTime_1;
            HeatingTime_2             = recipe.HeatingTime_2;
            HeatingTime_3             = recipe.HeatingTime_3;
            HeatingTime_4             = recipe.HeatingTime_4;
            HeatingTime_5             = recipe.HeatingTime_5;
            HeatingTime_6             = recipe.HeatingTime_6;
            HeatingTime_7             = recipe.HeatingTime_7;
            HeatingTime_8             = recipe.HeatingTime_8;
            InflatingTime             = recipe.InflatingTime;
            ProgramStopAlarmTime      = recipe.ProgramStopAlarmTime;
            TargetTemperature_1       = recipe.TargetTemperature_1;
            TargetTemperature_2       = recipe.TargetTemperature_2;
            TargetTemperature_3       = recipe.TargetTemperature_3;
            TargetTemperature_4       = recipe.TargetTemperature_4;
            TargetTemperature_5       = recipe.TargetTemperature_5;
            TargetTemperature_6       = recipe.TargetTemperature_6;
            TargetTemperature_7       = recipe.TargetTemperature_7;
            TargetTemperature_8       = recipe.TargetTemperature_8;
            UsedSegmentCounts         = recipe.UsedSegmentCounts;
            Editor                    = user;
            EditorLevel               = level;
        }

        public PLC_Recipe(string name, string user, UserLevel level) : base(name, user, level)
        {
            ThermostaticTemperature_1 = 200;
            ThermostaticTemperature_2 = 200;
            ThermostaticTemperature_3 = 200;
            ThermostaticTemperature_4 = 200;
            ThermostaticTemperature_5 = 200;
            ThermostaticTemperature_6 = 200;
            ThermostaticTemperature_7 = 200;
            ThermostaticTemperature_8 = 200;
            WarmingTime_1             = 10;
            WarmingTime_2             = 10;
            WarmingTime_3             = 10;
            WarmingTime_4             = 10;
            WarmingTime_5             = 10;
            WarmingTime_6             = 10;
            WarmingTime_7             = 10;
            WarmingTime_8             = 10;
            CoolingTime               = 30;
            CoolingTemperature        = 40;
            HeatingTime_1             = 10;
            HeatingTime_2             = 10;
            HeatingTime_3             = 10;
            HeatingTime_4             = 10;
            HeatingTime_5             = 10;
            HeatingTime_6             = 10;
            HeatingTime_7             = 10;
            HeatingTime_8             = 10;
            InflatingTime             = 10;
            ProgramStopAlarmTime      = 10;
            TargetTemperature_1       = 200;
            TargetTemperature_2       = 200;
            TargetTemperature_3       = 200;
            TargetTemperature_4       = 200;
            TargetTemperature_5       = 200;
            TargetTemperature_6       = 200;
            TargetTemperature_7       = 200;
            TargetTemperature_8       = 200;
            UsedSegmentCounts         = SegmentCounts_Max;
            Used_Stations             = new bool[20];
        }

        public PLC_Recipe() => Used_Stations = new bool[20];
    }
}