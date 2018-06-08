using System;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    [BsonIgnoreExtraElements]
    public class Record_Temperatures
    {
        public TimeSpan Time;
        public double[] OvenTemperatures = new double[8];
        public double ThermostatTemperature;

        public double Avg => OvenTemperatures.Average();

        public double Max => OvenTemperatures.Max();

        public double Min => OvenTemperatures.Min();
    }
}