using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    [BsonIgnoreExtraElements]
    public class Record_Temperatures
    {
        public short OvenTemperature_1 { get; set; }
        public short OvenTemperature_2 { get; set; }
        public short OvenTemperature_3 { get; set; }
        public short OvenTemperature_4 { get; set; }
        public short OvenTemperature_5 { get; set; }
        public short OvenTemperature_6 { get; set; }
        public short OvenTemperature_7 { get; set; }
        public short OvenTemperature_8 { get; set; }
        public double ThermostatTemperature { get; set; }
    }
}