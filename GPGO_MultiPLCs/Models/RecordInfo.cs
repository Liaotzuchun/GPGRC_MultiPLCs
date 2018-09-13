using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>定義事件type的代表顏色</summary>
    public class EventTypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Colors.Red;
            }

            if ((EventType)value == EventType.Normal)
            {
                return Colors.Green;
            }

            if ((EventType)value == EventType.Trigger)
            {
                return Colors.Blue;
            }

            return (EventType)value == EventType.Action ? Colors.DarkOrange : Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>溫度紀錄</summary>
    [BsonIgnoreExtraElements]
    public class RecordTemperatures
    {
        public double Avg => new[] { OvenTemperatures_1, OvenTemperatures_2, OvenTemperatures_3, OvenTemperatures_4, OvenTemperatures_5, OvenTemperatures_6, OvenTemperatures_7, OvenTemperatures_8 }
            .Average();

        public double Max => new[] { OvenTemperatures_1, OvenTemperatures_2, OvenTemperatures_3, OvenTemperatures_4, OvenTemperatures_5, OvenTemperatures_6, OvenTemperatures_7, OvenTemperatures_8 }
            .Max();

        public double Min => new[] { OvenTemperatures_1, OvenTemperatures_2, OvenTemperatures_3, OvenTemperatures_4, OvenTemperatures_5, OvenTemperatures_6, OvenTemperatures_7, OvenTemperatures_8 }
            .Min();

        public double OvenTemperatures_1 { get; set; }
        public double OvenTemperatures_2 { get; set; }
        public double OvenTemperatures_3 { get; set; }
        public double OvenTemperatures_4 { get; set; }
        public double OvenTemperatures_5 { get; set; }
        public double OvenTemperatures_6 { get; set; }
        public double OvenTemperatures_7 { get; set; }
        public double OvenTemperatures_8 { get; set; }
        public double ThermostatTemperature { get; set; }
        public TimeSpan Time { get; set; }
    }

    /// <summary>事件類型</summary>
    public enum EventType
    {
        Normal,
        Trigger,
        Action,
        Alarm
    }

    /// <summary>事件紀錄</summary>
    [BsonIgnoreExtraElements]
    public class RecordEvent
    {
        public string Description { get; set; }
        public TimeSpan Time { get; set; }
        public EventType Type { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class LogEvent : ILogData
    {
        [LanguageTranslator("Recorded", "紀錄時間", "纪录时间")]
        public DateTime AddedTime { get; set; }

        [LanguageTranslator("Event", "事件", "事件")]
        public string Description { get; set; }

        [LanguageTranslator("Oven No.", "烤箱序號", "烤箱序号")]
        public int StationNumber { get; set; }

        [LanguageTranslator("Type", "類型", "类型")]
        public EventType Type { get; set; }
    }
}