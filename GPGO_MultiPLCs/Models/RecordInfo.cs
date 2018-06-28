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

            if((EventType)value == EventType.Normal)
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
        public double[] OvenTemperatures { get; set; } = new double[8];
        public double ThermostatTemperature { get; set; }
        public TimeSpan Time { get; set; }

        public double Avg => OvenTemperatures.Average();

        public double Max => OvenTemperatures.Max();

        public double Min => OvenTemperatures.Min();
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
        public EventType Type { get; set; }
        public string Description { get; set; }
        public TimeSpan Time{ get; set; }
    }
}