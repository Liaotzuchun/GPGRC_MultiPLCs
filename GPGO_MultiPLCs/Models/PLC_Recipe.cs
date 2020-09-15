using GPMVVM.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
            DwellTemperature_1.ToString("0.0") == other.DwellTemperature_1.ToString("0.0") &&
            DwellTemperature_2.ToString("0.0") == other.DwellTemperature_2.ToString("0.0") &&
            DwellTemperature_3.ToString("0.0") == other.DwellTemperature_3.ToString("0.0") &&
            DwellTemperature_4.ToString("0.0") == other.DwellTemperature_4.ToString("0.0") &&
            DwellTemperature_5.ToString("0.0") == other.DwellTemperature_5.ToString("0.0") &&
            DwellTemperature_6.ToString("0.0") == other.DwellTemperature_6.ToString("0.0") &&
            DwellTemperature_7.ToString("0.0") == other.DwellTemperature_7.ToString("0.0") &&
            DwellTemperature_8.ToString("0.0") == other.DwellTemperature_8.ToString("0.0") &&
            DwellTime_1.ToString("0.0") == other.DwellTime_1.ToString("0.0") &&
            DwellTime_2.ToString("0.0") == other.DwellTime_2.ToString("0.0") &&
            DwellTime_3.ToString("0.0") == other.DwellTime_3.ToString("0.0") &&
            DwellTime_4.ToString("0.0") == other.DwellTime_4.ToString("0.0") &&
            DwellTime_5.ToString("0.0") == other.DwellTime_5.ToString("0.0") &&
            DwellTime_6.ToString("0.0") == other.DwellTime_6.ToString("0.0") &&
            DwellTime_7.ToString("0.0") == other.DwellTime_7.ToString("0.0") &&
            DwellTime_8.ToString("0.0") == other.DwellTime_8.ToString("0.0") &&
            DwellAlarm_1.ToString("0.0") == other.DwellAlarm_1.ToString("0.0") &&
            DwellAlarm_2.ToString("0.0") == other.DwellAlarm_2.ToString("0.0") &&
            DwellAlarm_3.ToString("0.0") == other.DwellAlarm_3.ToString("0.0") &&
            DwellAlarm_4.ToString("0.0") == other.DwellAlarm_4.ToString("0.0") &&
            DwellAlarm_5.ToString("0.0") == other.DwellAlarm_5.ToString("0.0") &&
            DwellAlarm_6.ToString("0.0") == other.DwellAlarm_6.ToString("0.0") &&
            DwellAlarm_7.ToString("0.0") == other.DwellAlarm_7.ToString("0.0") &&
            DwellAlarm_8.ToString("0.0") == other.DwellAlarm_8.ToString("0.0") &&
            CoolingTime.ToString("0.0") == other.CoolingTime.ToString("0.0") &&
            CoolingTemperature.ToString("0.0") == other.CoolingTemperature.ToString("0.0") &&
            RampTime_1.ToString("0.0") == other.RampTime_1.ToString("0.0") &&
            RampTime_2.ToString("0.0") == other.RampTime_2.ToString("0.0") &&
            RampTime_3.ToString("0.0") == other.RampTime_3.ToString("0.0") &&
            RampTime_4.ToString("0.0") == other.RampTime_4.ToString("0.0") &&
            RampTime_5.ToString("0.0") == other.RampTime_5.ToString("0.0") &&
            RampTime_6.ToString("0.0") == other.RampTime_6.ToString("0.0") &&
            RampTime_7.ToString("0.0") == other.RampTime_7.ToString("0.0") &&
            RampTime_8.ToString("0.0") == other.RampTime_8.ToString("0.0") &&
            RampAlarm_1.ToString("0.0") == other.RampAlarm_1.ToString("0.0") &&
            RampAlarm_2.ToString("0.0") == other.RampAlarm_2.ToString("0.0") &&
            RampAlarm_3.ToString("0.0") == other.RampAlarm_3.ToString("0.0") &&
            RampAlarm_4.ToString("0.0") == other.RampAlarm_4.ToString("0.0") &&
            RampAlarm_5.ToString("0.0") == other.RampAlarm_5.ToString("0.0") &&
            RampAlarm_6.ToString("0.0") == other.RampAlarm_6.ToString("0.0") &&
            RampAlarm_7.ToString("0.0") == other.RampAlarm_7.ToString("0.0") &&
            RampAlarm_8.ToString("0.0") == other.RampAlarm_8.ToString("0.0") &&
            InflatingTime.ToString("0.0") == other.InflatingTime.ToString("0.0") &&
            ProgramEndWarningTime.ToString("0.0") == other.ProgramEndWarningTime.ToString("0.0") &&
            TemperatureSetpoint_1.ToString("0.0") == other.TemperatureSetpoint_1.ToString("0.0") &&
            TemperatureSetpoint_2.ToString("0.0") == other.TemperatureSetpoint_2.ToString("0.0") &&
            TemperatureSetpoint_3.ToString("0.0") == other.TemperatureSetpoint_3.ToString("0.0") &&
            TemperatureSetpoint_4.ToString("0.0") == other.TemperatureSetpoint_4.ToString("0.0") &&
            TemperatureSetpoint_5.ToString("0.0") == other.TemperatureSetpoint_5.ToString("0.0") &&
            TemperatureSetpoint_6.ToString("0.0") == other.TemperatureSetpoint_6.ToString("0.0") &&
            TemperatureSetpoint_7.ToString("0.0") == other.TemperatureSetpoint_7.ToString("0.0") &&
            TemperatureSetpoint_8.ToString("0.0") == other.TemperatureSetpoint_8.ToString("0.0") &&
            StepCounts == other.StepCounts;

        [JsonIgnore] public short StepCounts_Max => 6;

        [JsonIgnore] public short StepCounts_Min => 1;

        [JsonIgnore] public double Temperature_Max => 250.0;

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
        [LanguageTranslator("Used Step Counts", "使用段數", "使用段数")]
        public short StepCounts
        {
            get => Get<short>();
            set
            {
                if (value > StepCounts_Max)
                {
                    value = StepCounts_Max;
                }
                else if (value < StepCounts_Min)
                {
                    value = StepCounts_Min;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [LanguageTranslator("Cooling Temp.", "降溫溫度", "降温温度")]
        public double CoolingTemperature
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

        [LanguageTranslator("ProgramEndWarningTime", "程式結束警報時間", "程序结束警报时间")]
        public double ProgramEndWarningTime
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
        [LanguageTranslator("Temp. SP 1", "目標溫度 1", "目标温度 1")]
        public double TemperatureSetpoint_1
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
                TemperatureSetpoint_2 = TemperatureSetpoint_2;
            }
        }

        [OrderIndex(6)]
        [LanguageTranslator("Temp. SP 2", "目標溫度 2", "目标温度 2")]
        public double TemperatureSetpoint_2
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TemperatureSetpoint_1)
                {
                    value = TemperatureSetpoint_1;
                }

                Set(value);
                TemperatureSetpoint_3 = TemperatureSetpoint_3;
            }
        }

        [OrderIndex(7)]
        [LanguageTranslator("Temp. SP 3", "目標溫度 3", "目标温度 3")]
        public double TemperatureSetpoint_3
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TemperatureSetpoint_2)
                {
                    value = TemperatureSetpoint_2;
                }

                Set(value);
                TemperatureSetpoint_4 = TemperatureSetpoint_4;
            }
        }

        [OrderIndex(8)]
        [LanguageTranslator("Temp. SP 4", "目標溫度 4", "目标温度 4")]
        public double TemperatureSetpoint_4
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TemperatureSetpoint_3)
                {
                    value = TemperatureSetpoint_3;
                }

                Set(value);
                TemperatureSetpoint_5 = TemperatureSetpoint_5;
            }
        }

        [OrderIndex(9)]
        [LanguageTranslator("Temp. SP 5", "目標溫度 5", "目标温度 5")]
        public double TemperatureSetpoint_5
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TemperatureSetpoint_4)
                {
                    value = TemperatureSetpoint_4;
                }

                Set(value);
                TemperatureSetpoint_6 = TemperatureSetpoint_6;
            }
        }

        [OrderIndex(10)]
        [LanguageTranslator("Temp. SP 6", "目標溫度 6", "目标温度 6")]
        public double TemperatureSetpoint_6
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TemperatureSetpoint_5)
                {
                    value = TemperatureSetpoint_5;
                }

                Set(value);
                TemperatureSetpoint_7 = TemperatureSetpoint_7;
            }
        }

        [OrderIndex(11)]
        [LanguageTranslator("Temp. SP 7", "目標溫度 7", "目标温度 7")]
        public double TemperatureSetpoint_7
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TemperatureSetpoint_6)
                {
                    value = TemperatureSetpoint_6;
                }

                Set(value);
                TemperatureSetpoint_8 = TemperatureSetpoint_8;
            }
        }

        [OrderIndex(12)]
        [LanguageTranslator("Temp. SP 8", "目標溫度 8", "目标温度 8")]
        public double TemperatureSetpoint_8
        {
            get => Get<double>();
            set
            {
                if (value > Temperature_Max)
                {
                    value = Temperature_Max;
                }
                else if (value < TemperatureSetpoint_7)
                {
                    value = TemperatureSetpoint_7;
                }

                Set(value);
            }
        }

        [OrderIndex(13)]
        [LanguageTranslator("Ramp Time 1", "升溫時間 1", "升温时间 1")]
        public double RampTime_1
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
                RampAlarm_1 = RampAlarm_1;
            }
        }

        [OrderIndex(14)]
        [LanguageTranslator("Ramp Time 2", "升溫時間 2", "升温时间 2")]
        public double RampTime_2
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
                RampAlarm_2 = RampAlarm_2;
            }
        }

        [OrderIndex(15)]
        [LanguageTranslator("Ramp Time 3", "升溫時間 3", "升温时间 3")]
        public double RampTime_3
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
                RampAlarm_3 = RampAlarm_3;
            }
        }

        [OrderIndex(16)]
        [LanguageTranslator("Ramp Time 4", "升溫時間 4", "升温时间 4")]
        public double RampTime_4
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
                RampAlarm_4 = RampAlarm_4;
            }
        }

        [OrderIndex(17)]
        [LanguageTranslator("Ramp Time 5", "升溫時間 5", "升温时间 5")]
        public double RampTime_5
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
                RampAlarm_5 = RampAlarm_5;
            }
        }

        [OrderIndex(18)]
        [LanguageTranslator("Ramp Time 6", "升溫時間 6", "升温时间 6")]
        public double RampTime_6
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
                RampAlarm_6 = RampAlarm_6;
            }
        }

        [GPIgnore]
        [OrderIndex(19)]
        [LanguageTranslator("Ramp Time 7", "升溫時間 7", "升温时间 7")]
        public double RampTime_7
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
                RampAlarm_7 = RampAlarm_7;
            }
        }

        [GPIgnore]
        [OrderIndex(20)]
        [LanguageTranslator("Ramp Time 8", "升溫時間 8", "升温时间 8")]
        public double RampTime_8
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
                RampAlarm_8 = RampAlarm_8;
            }
        }

        [OrderIndex(21)]
        [LanguageTranslator("Ramp Alarm 1", "升溫警報 1", "升温警报 1")]
        public double RampAlarm_1
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < RampTime_1 + 0.1)
                {
                    value = RampTime_1 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(22)]
        [LanguageTranslator("Ramp Alarm 2", "升溫警報 2", "升温警报 2")]
        public double RampAlarm_2
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < RampTime_2 + 0.1)
                {
                    value = RampTime_2 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(23)]
        [LanguageTranslator("Ramp Alarm 3", "升溫警報 3", "升温警报 3")]
        public double RampAlarm_3
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < RampTime_3 + 0.1)
                {
                    value = RampTime_3 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(24)]
        [LanguageTranslator("Ramp Alarm 4", "升溫警報 4", "升温警报 4")]
        public double RampAlarm_4
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < RampTime_4 + 0.1)
                {
                    value = RampTime_4 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(25)]
        [LanguageTranslator("Ramp Alarm 5", "升溫警報 5", "升温警报 5")]
        public double RampAlarm_5
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < RampTime_5 + 0.1)
                {
                    value = RampTime_5 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(26)]
        [LanguageTranslator("Ramp Alarm 6", "升溫警報 6", "升温警报 6")]
        public double RampAlarm_6
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < RampTime_6 + 0.1)
                {
                    value = RampTime_6 + 0.1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(27)]
        [LanguageTranslator("Ramp Alarm 7", "升溫警報 7", "升温警报 7")]
        public double RampAlarm_7
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < RampTime_7 + 0.1)
                {
                    value = RampTime_7 + 0.1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(28)]
        [LanguageTranslator("Ramp Alarm 8", "升溫警報 8", "升温警报 8")]
        public double RampAlarm_8
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < RampTime_8 + 0.1)
                {
                    value = RampTime_8 + 0.1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(29)]
        [LanguageTranslator("Dwell Temp. 1", "恆溫溫度 1", "恒温温度 1")]
        public double DwellTemperature_1
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
        [LanguageTranslator("Dwell Temp. 2", "恆溫溫度 2", "恒温温度 2")]
        public double DwellTemperature_2
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
        [LanguageTranslator("Dwell Temp. 3", "恆溫溫度 3", "恒温温度 3")]
        public double DwellTemperature_3
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
        [LanguageTranslator("Dwell Temp. 4", "恆溫溫度 4", "恒温温度 4")]
        public double DwellTemperature_4
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
        [LanguageTranslator("Dwell Temp. 5", "恆溫溫度 5", "恒温温度 5")]
        public double DwellTemperature_5
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
        [LanguageTranslator("Dwell Temp. 6", "恆溫溫度 6", "恒温温度 6")]
        public double DwellTemperature_6
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
        [LanguageTranslator("Dwell Temp. 7", "恆溫溫度 7", "恒温温度 7")]
        public double DwellTemperature_7
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
        [LanguageTranslator("Dwell Temp. 8", "恆溫溫度 8", "恒温温度 8")]
        public double DwellTemperature_8
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
        public double DwellTime_1
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
                DwellAlarm_1 = DwellAlarm_1;
            }
        }

        [OrderIndex(38)]
        [LanguageTranslator("Warning Time 2", "恆溫時間 2", "恒温时间 2")]
        public double DwellTime_2
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
                DwellAlarm_2 = DwellAlarm_2;
            }
        }

        [OrderIndex(39)]
        [LanguageTranslator("Warning Time 3", "恆溫時間 3", "恒温时间 3")]
        public double DwellTime_3
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
                DwellAlarm_3 = DwellAlarm_3;
            }
        }

        [OrderIndex(40)]
        [LanguageTranslator("Warning Time 4", "恆溫時間 4", "恒温时间 4")]
        public double DwellTime_4
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
                DwellAlarm_4 = DwellAlarm_4;
            }
        }

        [OrderIndex(41)]
        [LanguageTranslator("Warning Time 5", "恆溫時間 5", "恒温时间 5")]
        public double DwellTime_5
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
                DwellAlarm_5 = DwellAlarm_5;
            }
        }

        [OrderIndex(42)]
        [LanguageTranslator("Warning Time 6", "恆溫時間 6", "恒温时间 6")]
        public double DwellTime_6
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
                DwellAlarm_6 = DwellAlarm_6;
            }
        }

        [GPIgnore]
        [OrderIndex(43)]
        [LanguageTranslator("Warning Time 7", "恆溫時間 7", "恒温时间 7")]
        public double DwellTime_7
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
                DwellAlarm_7 = DwellAlarm_7;
            }
        }

        [GPIgnore]
        [OrderIndex(44)]
        [LanguageTranslator("Warning Time 8", "恆溫時間 8", "恒温时间 8")]
        public double DwellTime_8
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
                DwellAlarm_8 = DwellAlarm_8;
            }
        }

        [OrderIndex(45)]
        [LanguageTranslator("Warming Alarm 1", "恆溫警報 1", "恒温警报 1")]
        public double DwellAlarm_1
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < DwellTime_1 + 0.1)
                {
                    value = DwellTime_1 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(46)]
        [LanguageTranslator("Warming Alarm 2", "恆溫警報 2", "恒温警报 2")]
        public double DwellAlarm_2
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < DwellTime_2 + 0.1)
                {
                    value = DwellTime_2 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(47)]
        [LanguageTranslator("Warming Alarm 3", "恆溫警報 3", "恒温警报 3")]
        public double DwellAlarm_3
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < DwellTime_3 + 0.1)
                {
                    value = DwellTime_3 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(48)]
        [LanguageTranslator("Warming Alarm 4", "恆溫警報 4", "恒温警报 4")]
        public double DwellAlarm_4
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < DwellTime_4 + 0.1)
                {
                    value = DwellTime_4 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(49)]
        [LanguageTranslator("Warming Alarm 5", "恆溫警報 5", "恒温警报 5")]
        public double DwellAlarm_5
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < DwellTime_5 + 0.1)
                {
                    value = DwellTime_5 + 0.1;
                }

                Set(value);
            }
        }

        [OrderIndex(50)]
        [LanguageTranslator("Warming Alarm 6", "恆溫警報 6", "恒温警报 6")]
        public double DwellAlarm_6
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < DwellTime_6 + 0.1)
                {
                    value = DwellTime_6 + 0.1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(51)]
        [LanguageTranslator("Warming Alarm 7", "恆溫警報 7", "恒温警报 7")]
        public double DwellAlarm_7
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < DwellTime_7 + 0.1)
                {
                    value = DwellTime_7 + 0.1;
                }

                Set(value);
            }
        }

        [GPIgnore]
        [OrderIndex(52)]
        [LanguageTranslator("Warming Alarm 8", "恆溫警報 8", "恒温警报 8")]
        public double DwellAlarm_8
        {
            get => Get<double>();
            set
            {
                if (value > Time_Max)
                {
                    value = Time_Max;
                }
                else if (value < DwellTime_8 + 0.1)
                {
                    value = DwellTime_8 + 0.1;
                }

                Set(value);
            }
        }

        public override PLC_Recipe Copy(string user, UserLevel level) =>
            new PLC_Recipe
            {
                Updated               = DateTime.Now,
                RecipeName            = RecipeName,
                DwellTemperature_1    = DwellTemperature_1,
                DwellTemperature_2    = DwellTemperature_2,
                DwellTemperature_3    = DwellTemperature_3,
                DwellTemperature_4    = DwellTemperature_4,
                DwellTemperature_5    = DwellTemperature_5,
                DwellTemperature_6    = DwellTemperature_6,
                DwellTemperature_7    = DwellTemperature_7,
                DwellTemperature_8    = DwellTemperature_8,
                DwellTime_1           = DwellTime_1,
                DwellTime_2           = DwellTime_2,
                DwellTime_3           = DwellTime_3,
                DwellTime_4           = DwellTime_4,
                DwellTime_5           = DwellTime_5,
                DwellTime_6           = DwellTime_6,
                DwellTime_7           = DwellTime_7,
                DwellTime_8           = DwellTime_8,
                DwellAlarm_1          = DwellAlarm_1,
                DwellAlarm_2          = DwellAlarm_2,
                DwellAlarm_3          = DwellAlarm_3,
                DwellAlarm_4          = DwellAlarm_4,
                DwellAlarm_5          = DwellAlarm_5,
                DwellAlarm_6          = DwellAlarm_6,
                DwellAlarm_7          = DwellAlarm_7,
                DwellAlarm_8          = DwellAlarm_8,
                CoolingTime           = CoolingTime,
                CoolingTemperature    = CoolingTemperature,
                RampTime_1            = RampTime_1,
                RampTime_2            = RampTime_2,
                RampTime_3            = RampTime_3,
                RampTime_4            = RampTime_4,
                RampTime_5            = RampTime_5,
                RampTime_6            = RampTime_6,
                RampTime_7            = RampTime_7,
                RampTime_8            = RampTime_8,
                RampAlarm_1           = RampAlarm_1,
                RampAlarm_2           = RampAlarm_2,
                RampAlarm_3           = RampAlarm_3,
                RampAlarm_4           = RampAlarm_4,
                RampAlarm_5           = RampAlarm_5,
                RampAlarm_6           = RampAlarm_6,
                RampAlarm_7           = RampAlarm_7,
                RampAlarm_8           = RampAlarm_8,
                InflatingTime         = InflatingTime,
                ProgramEndWarningTime = ProgramEndWarningTime,
                TemperatureSetpoint_1 = TemperatureSetpoint_1,
                TemperatureSetpoint_2 = TemperatureSetpoint_2,
                TemperatureSetpoint_3 = TemperatureSetpoint_3,
                TemperatureSetpoint_4 = TemperatureSetpoint_4,
                TemperatureSetpoint_5 = TemperatureSetpoint_5,
                TemperatureSetpoint_6 = TemperatureSetpoint_6,
                TemperatureSetpoint_7 = TemperatureSetpoint_7,
                TemperatureSetpoint_8 = TemperatureSetpoint_8,
                StepCounts            = StepCounts,
                Used_Stations         = Used_Stations,
                Editor                = user,
                EditorLevel           = level
            };

        public override void CopyValue(string user, UserLevel level, PLC_Recipe recipe)
        {
            Updated               = DateTime.Now;
            DwellTemperature_1    = recipe.DwellTemperature_1;
            DwellTemperature_2    = recipe.DwellTemperature_2;
            DwellTemperature_3    = recipe.DwellTemperature_3;
            DwellTemperature_4    = recipe.DwellTemperature_4;
            DwellTemperature_5    = recipe.DwellTemperature_5;
            DwellTemperature_6    = recipe.DwellTemperature_6;
            DwellTemperature_7    = recipe.DwellTemperature_7;
            DwellTemperature_8    = recipe.DwellTemperature_8;
            DwellTime_1           = recipe.DwellTime_1;
            DwellTime_2           = recipe.DwellTime_2;
            DwellTime_3           = recipe.DwellTime_3;
            DwellTime_4           = recipe.DwellTime_4;
            DwellTime_5           = recipe.DwellTime_5;
            DwellTime_6           = recipe.DwellTime_6;
            DwellTime_7           = recipe.DwellTime_7;
            DwellTime_8           = recipe.DwellTime_8;
            CoolingTime           = recipe.CoolingTime;
            CoolingTemperature    = recipe.CoolingTemperature;
            RampTime_1            = recipe.RampTime_1;
            RampTime_2            = recipe.RampTime_2;
            RampTime_3            = recipe.RampTime_3;
            RampTime_4            = recipe.RampTime_4;
            RampTime_5            = recipe.RampTime_5;
            RampTime_6            = recipe.RampTime_6;
            RampTime_7            = recipe.RampTime_7;
            RampTime_8            = recipe.RampTime_8;
            InflatingTime         = recipe.InflatingTime;
            ProgramEndWarningTime = recipe.ProgramEndWarningTime;
            TemperatureSetpoint_1 = recipe.TemperatureSetpoint_1;
            TemperatureSetpoint_2 = recipe.TemperatureSetpoint_2;
            TemperatureSetpoint_3 = recipe.TemperatureSetpoint_3;
            TemperatureSetpoint_4 = recipe.TemperatureSetpoint_4;
            TemperatureSetpoint_5 = recipe.TemperatureSetpoint_5;
            TemperatureSetpoint_6 = recipe.TemperatureSetpoint_6;
            TemperatureSetpoint_7 = recipe.TemperatureSetpoint_7;
            TemperatureSetpoint_8 = recipe.TemperatureSetpoint_8;
            StepCounts            = recipe.StepCounts;
            Editor                = user;
            EditorLevel           = level;
        }

        public PLC_Recipe(string name, string user, UserLevel level) : base(name, user, level)
        {
            DwellTemperature_1    = 200;
            DwellTemperature_2    = 200;
            DwellTemperature_3    = 200;
            DwellTemperature_4    = 200;
            DwellTemperature_5    = 200;
            DwellTemperature_6    = 200;
            DwellTemperature_7    = 200;
            DwellTemperature_8    = 200;
            DwellTime_1           = 10;
            DwellTime_2           = 10;
            DwellTime_3           = 10;
            DwellTime_4           = 10;
            DwellTime_5           = 10;
            DwellTime_6           = 10;
            DwellTime_7           = 10;
            DwellTime_8           = 10;
            CoolingTime           = 30;
            CoolingTemperature    = 40;
            RampTime_1            = 10;
            RampTime_2            = 10;
            RampTime_3            = 10;
            RampTime_4            = 10;
            RampTime_5            = 10;
            RampTime_6            = 10;
            RampTime_7            = 10;
            RampTime_8            = 10;
            InflatingTime         = 10;
            ProgramEndWarningTime = 10;
            TemperatureSetpoint_1 = 200;
            TemperatureSetpoint_2 = 200;
            TemperatureSetpoint_3 = 200;
            TemperatureSetpoint_4 = 200;
            TemperatureSetpoint_5 = 200;
            TemperatureSetpoint_6 = 200;
            TemperatureSetpoint_7 = 200;
            TemperatureSetpoint_8 = 200;
            StepCounts            = StepCounts_Max;
            Used_Stations         = new bool[20];
        }

        public PLC_Recipe() => Used_Stations = new bool[20];
    }
}