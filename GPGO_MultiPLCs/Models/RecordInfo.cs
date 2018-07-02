using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
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

            return (EventType)value == EventType.Trigger ? Colors.Blue : Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [BsonIgnoreExtraElements]
    public class RecordTemperatures
    {
        public double Avg => new[] { OvenTemperatures_0, OvenTemperatures_1, OvenTemperatures_2, OvenTemperatures_3, OvenTemperatures_4, OvenTemperatures_5, OvenTemperatures_6, OvenTemperatures_7 }
            .Average();

        public double Max => new[] { OvenTemperatures_0, OvenTemperatures_1, OvenTemperatures_2, OvenTemperatures_3, OvenTemperatures_4, OvenTemperatures_5, OvenTemperatures_6, OvenTemperatures_7 }
            .Max();

        public double Min => new[] { OvenTemperatures_0, OvenTemperatures_1, OvenTemperatures_2, OvenTemperatures_3, OvenTemperatures_4, OvenTemperatures_5, OvenTemperatures_6, OvenTemperatures_7 }
            .Min();

        public double OvenTemperatures_0 { get; set; }
        public double OvenTemperatures_1 { get; set; }
        public double OvenTemperatures_2 { get; set; }
        public double OvenTemperatures_3 { get; set; }
        public double OvenTemperatures_4 { get; set; }
        public double OvenTemperatures_5 { get; set; }
        public double OvenTemperatures_6 { get; set; }
        public double OvenTemperatures_7 { get; set; }
        public double ThermostatTemperature { get; set; }
        public TimeSpan Time { get; set; }
    }

    public enum EventType
    {
        Normal,
        Trigger,
        Alarm
    }

    [BsonIgnoreExtraElements]
    public class RecordEvent
    {
        public string Description { get; set; }
        public TimeSpan Time { get; set; }
        public EventType Type { get; set; }
    }
}