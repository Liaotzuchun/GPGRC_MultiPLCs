using System;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    [BsonIgnoreExtraElements]
    public class PanelInfo
    {
        public Record_Info RecordTemperatures { get; set; }
        public string OrderCode { get; set; }
        public int ProcessNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MachineCode { get; set; }
        public string CodeType { get; set; }
        public string ProduceCode { get; set; }
        public string RecipeName { get; set; }
        public int OrderCount { get; set; }
        public int ProcessCount { get; set; }
        public string WorkerID { get; set; }
        public string JigCode { get; set; }
        public string Side { get; set; }
        public int PCS_Number { get; set; }
        public string FirstPanel { get; set; }
        public string TrolleyCode { get; set; }
        public double OvenTemperatureSet { get; set; }
        public short HeatingTime { get; set; }
        public short WarmingTime { get; set; }
        public short TotalHeatingTime { get; set; }
        public string AlarmList { get; set; }
    }
}