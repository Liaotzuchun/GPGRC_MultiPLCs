using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GPGO_MultiPLCs.Models
{
    //!此區是烘烤時的溫度和事件紀錄資料模型

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

            return (EventType)value == EventType.Operator ? Colors.DarkOrange : Colors.Red;
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

        [LanguageTranslator("Duration", "歷時", "历时")]
        public TimeSpan Time => AddedTime - StartTime;

        [LanguageTranslator("Recorded", "紀錄時間", "纪录时间")]
        public DateTime AddedTime { get; set; }

        public double OvenTemperatures_1 { get; set; }
        public double OvenTemperatures_2 { get; set; }
        public double OvenTemperatures_3 { get; set; }
        public double OvenTemperatures_4 { get; set; }
        public double OvenTemperatures_5 { get; set; }
        public double OvenTemperatures_6 { get; set; }
        public double OvenTemperatures_7 { get; set; }
        public double OvenTemperatures_8 { get; set; }

        [LanguageTranslator("Started", "開始時間", "开始时间")]
        public DateTime StartTime { get; set; }

        public double ThermostatTemperature { get; set; }
    }

    public enum PCEventCode
    {
        PC_Offline = 1
    }

    /// <summary>事件類型</summary>
    public enum EventType
    {
        Normal, //一般事件
        Trigger, //觸發事件
        Operator, //OP操作事件
        Alarm //警報事件
    }

    [BsonIgnoreExtraElements]
    public class LogEvent : ILogData
    {
        [LanguageTranslator("Recorded", "紀錄時間", "纪录时间")]
        public DateTime AddedTime { get; set; }

        [LanguageTranslator("Duration", "歷時", "历时")]
        public TimeSpan Time => AddedTime - StartTime;

        [LanguageTranslator("Event", "事件", "事件")]
        public string Description2
        {
            get
            {
                var obj = Application.Current.TryFindResource(Description);

                return obj != null ? obj.ToString() : Description;
            }
        }

        [LanguageTranslator("Event", "事件", "事件")]
        public string Description { get; set; }

        //!當處在生產中時，即烤箱開始生產的時間，若未在生產，則沒有值
        [LanguageTranslator("Started", "開始時間", "开始时间")]
        public DateTime StartTime { get; set; }

        //!站號由1開始
        [LanguageTranslator("Oven No.", "烤箱序號", "烤箱序号")]
        public int StationNumber { get; set; }

        [LanguageTranslator("Tag", "標籤", "标签")]
        public int TagCode { get; set; }

        [LanguageTranslator("Type", "類型", "类型")]
        public EventType Type { get; set; }

        [LanguageTranslator("Value", "值", "值")]
        public bool Value { get; set; }
    }
}