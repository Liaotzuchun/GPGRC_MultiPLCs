using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GPMVVM.Helpers;
using GPMVVM.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models;
//!此區是烘烤時的溫度和事件紀錄資料模型

/// <summary>溫度紀錄</summary>
[BsonIgnoreExtraElements]
public class RecordTemperatures
{
    public double Avg =>
        new[]
            {
                OvenTemperatures_1, OvenTemperatures_2, OvenTemperatures_3, OvenTemperatures_4,
                OvenTemperatures_5, OvenTemperatures_6, OvenTemperatures_7, OvenTemperatures_8
            }
           .Average();

    public double Max =>
        new[]
            {
                OvenTemperatures_1, OvenTemperatures_2, OvenTemperatures_3, OvenTemperatures_4,
                OvenTemperatures_5, OvenTemperatures_6, OvenTemperatures_7, OvenTemperatures_8
            }
           .Max();

    public double Min =>
        new[]
            {
                OvenTemperatures_1, OvenTemperatures_2, OvenTemperatures_3, OvenTemperatures_4,
                OvenTemperatures_5, OvenTemperatures_6, OvenTemperatures_7, OvenTemperatures_8
            }
           .Min();

    [GPIgnore]
    [LanguageTranslator("Started", "開始時間", "开始时间")]
    public DateTime StartTime { get; set; }

    [LanguageTranslator("Duration", "歷時", "历时")]
    public TimeSpan Time => AddedTime - StartTime;

    [LanguageTranslator("Recorded", "紀錄時間", "纪录时间")]
    public DateTime AddedTime { get; set; }

    [LanguageTranslator("ThermostatTemperature", "溫控器溫度", "温控器温度")]
    public double PV_ThermostatTemperature { get; set; }

    [LanguageTranslator("OvenTemperatures_1", "感溫器溫度1", "感温器温度1")]
    public double OvenTemperatures_1 { get; set; }

    [LanguageTranslator("OvenTemperatures_2", "感溫器溫度2", "感温器温度2")]
    public double OvenTemperatures_2 { get; set; }

    [LanguageTranslator("OvenTemperatures_3", "感溫器溫度3", "感温器温度3")]
    public double OvenTemperatures_3 { get; set; }

    [LanguageTranslator("OvenTemperatures_4", "感溫器溫度4", "感温器温度4")]
    public double OvenTemperatures_4 { get; set; }

    [LanguageTranslator("OvenTemperatures_5", "感溫器溫度5", "感温器温度5")]
    public double OvenTemperatures_5 { get; set; }

    [LanguageTranslator("OvenTemperatures_6", "感溫器溫度6", "感温器温度6")]
    public double OvenTemperatures_6 { get; set; }

    [GPIgnore]
    [LanguageTranslator("OvenTemperatures_7", "感溫器溫度7", "感温器温度7")]
    public double OvenTemperatures_7 { get; set; }

    [GPIgnore]
    [LanguageTranslator("OvenTemperatures_8", "感溫器溫度8", "感温器温度8")]
    public double OvenTemperatures_8 { get; set; }

    public Dictionary<string, object> ToDic(Language lng)
    {
        var type = GetType();

        return new Dictionary<string, object>
               {
                   { type.GetProperty(nameof(Time))?.GetName(lng)                     ?? nameof(Time), Time },
                   { type.GetProperty(nameof(PV_ThermostatTemperature))?.GetName(lng) ?? nameof(PV_ThermostatTemperature), PV_ThermostatTemperature },
                   { type.GetProperty(nameof(OvenTemperatures_1))?.GetName(lng)       ?? nameof(OvenTemperatures_1), OvenTemperatures_1 },
                   { type.GetProperty(nameof(OvenTemperatures_2))?.GetName(lng)       ?? nameof(OvenTemperatures_2), OvenTemperatures_2 },
                   { type.GetProperty(nameof(OvenTemperatures_3))?.GetName(lng)       ?? nameof(OvenTemperatures_3), OvenTemperatures_3 },
                   { type.GetProperty(nameof(OvenTemperatures_4))?.GetName(lng)       ?? nameof(OvenTemperatures_4), OvenTemperatures_4 },
                   { type.GetProperty(nameof(OvenTemperatures_5))?.GetName(lng)       ?? nameof(OvenTemperatures_5), OvenTemperatures_5 },
                   { type.GetProperty(nameof(OvenTemperatures_6))?.GetName(lng)       ?? nameof(OvenTemperatures_6), OvenTemperatures_6 },
               };
    }
}

/// <summary>事件類型</summary>
public enum EventType
{
    Normal,        //一般事件
    StatusChanged, //狀態變化
    Trigger,       //觸發事件
    Operator,      //OP操作事件
    Alert,         //警告
    Alarm,         //警報事件
    SECSCommnd
}

[BsonIgnoreExtraElements]
public class LogEvent : ILogData
{
    [LanguageTranslator("Recorded", "紀錄時間", "纪录时间")]
    public DateTime AddedTime { get; set; }

    [GPIgnore]
    [LanguageTranslator("Duration", "歷時", "历时")]
    public TimeSpan Time => AddedTime - StartTime;

    [GPIgnore]
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
    [GPIgnore]
    [LanguageTranslator("Started", "開始時間", "开始时间")]
    public DateTime StartTime { get; set; }

    //!站號由1開始
    [GPIgnore]
    [LanguageTranslator("Oven No.", "烤箱序號", "烤箱序号")]
    public int StationNumber { get; set; }

    [GPIgnore]
    [LanguageTranslator("Tag", "標籤", "标签")]
    public string TagCode { get; set; }

    [LanguageTranslator("Type", "類型", "类型")]
    public EventType Type { get; set; }

    [LanguageTranslator("Value", "值", "值")]
    public object Value { get; set; }
}