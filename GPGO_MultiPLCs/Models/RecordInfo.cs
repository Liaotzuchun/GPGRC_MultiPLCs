using System;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
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

    [BsonIgnoreExtraElements]
    public class RecordAlarm
    {
        public string Description { get; set; }
        public TimeSpan Time{ get; set; }
    }
}