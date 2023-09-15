using System;
using MongodbConnect.DataClass;

namespace GPGO_MultiPLCs.Models;
public class WebRecipe : MongodbDataBaseClass
{
    public string RecipeName { get; set; }
    public DateTime Updated { get; set; }
    public string Editor { get; set; }
    public int EditorLevel { get; set; }
    public int SegmentCounts { get; set; }
    public bool NitrogenMode { get; set; }
    public double OxygenContentSet { get; set; }
    public double CoolingTemperature { get; set; }
    public double CoolingTime { get; set; }
    public double InflatingTime { get; set; }
    public double TemperatureSetpoint_1 { get; set; }
    public double TemperatureSetpoint_2 { get; set; }
    public double TemperatureSetpoint_3 { get; set; }
    public double TemperatureSetpoint_4 { get; set; }
    public double TemperatureSetpoint_5 { get; set; }
    public double TemperatureSetpoint_6 { get; set; }
    public double TemperatureSetpoint_7 { get; set; }
    public double TemperatureSetpoint_8 { get; set; }
    public double RampTime_1 { get; set; }
    public double RampTime_2 { get; set; }
    public double RampTime_3 { get; set; }
    public double RampTime_4 { get; set; }
    public double RampTime_5 { get; set; }
    public double RampTime_6 { get; set; }
    public double RampTime_7 { get; set; }
    public double RampTime_8 { get; set; }
    public double RampAlarm_1 { get; set; }
    public double RampAlarm_2 { get; set; }
    public double RampAlarm_3 { get; set; }
    public double RampAlarm_4 { get; set; }
    public double RampAlarm_5 { get; set; }
    public double RampAlarm_6 { get; set; }
    public double RampAlarm_7 { get; set; }
    public double RampAlarm_8 { get; set; }
    public double DwellTemperature_1 { get; set; }
    public double DwellTemperature_2 { get; set; }
    public double DwellTemperature_3 { get; set; }
    public double DwellTemperature_4 { get; set; }
    public double DwellTemperature_5 { get; set; }
    public double DwellTemperature_6 { get; set; }
    public double DwellTemperature_7 { get; set; }
    public double DwellTemperature_8 { get; set; }
    public double DwellTime_1 { get; set; }
    public double DwellTime_2 { get; set; }
    public double DwellTime_3 { get; set; }
    public double DwellTime_4 { get; set; }
    public double DwellTime_5 { get; set; }
    public double DwellTime_6 { get; set; }
    public double DwellTime_7 { get; set; }
    public double DwellTime_8 { get; set; }
    public double DwellAlarm_1 { get; set; }
    public double DwellAlarm_2 { get; set; }
    public double DwellAlarm_3 { get; set; }
    public double DwellAlarm_4 { get; set; }
    public double DwellAlarm_5 { get; set; }
    public double DwellAlarm_6 { get; set; }
    public double DwellAlarm_7 { get; set; }
    public double DwellAlarm_8 { get; set; }
    public double DwellTimeOffset_1 { get; set; }
    public double DwellTimeOffset_2 { get; set; }
    public double DwellTimeOffset_3 { get; set; }
    public double DwellTimeOffset_4 { get; set; }
    public double DwellTimeOffset_5 { get; set; }
    public double DwellTimeOffset_6 { get; set; }
    public double DwellTimeOffset_7 { get; set; }
    public double DwellTimeOffset_8 { get; set; }
    public WebRecipe(string recipeName, DateTime updated, string editor, int editorLevel, int segmentCounts, bool nitrogenMode, double oxygenContentSet, double coolingTemperature, double coolingTime, double inflatingTime, double temperatureSetpoint_1, double temperatureSetpoint_2, double temperatureSetpoint_3, double temperatureSetpoint_4, double temperatureSetpoint_5, double temperatureSetpoint_6, double temperatureSetpoint_7, double temperatureSetpoint_8, double rampTime_1, double rampTime_2, double rampTime_3, double rampTime_4, double rampTime_5, double rampTime_6, double rampTime_7, double rampTime_8, double rampAlarm_1, double rampAlarm_2, double rampAlarm_3, double rampAlarm_4, double rampAlarm_5, double rampAlarm_6, double rampAlarm_7, double rampAlarm_8, double dwellTemperature_1, double dwellTemperature_2, double dwellTemperature_3, double dwellTemperature_4, double dwellTemperature_5, double dwellTemperature_6, double dwellTemperature_7, double dwellTemperature_8, double dwellTime_1, double dwellTime_2, double dwellTime_3, double dwellTime_4, double dwellTime_5, double dwellTime_6, double dwellTime_7, double dwellTime_8, double dwellAlarm_1, double dwellAlarm_2, double dwellAlarm_3, double dwellAlarm_4, double dwellAlarm_5, double dwellAlarm_6, double dwellAlarm_7, double dwellAlarm_8, double dwellTimeOffset_1, double dwellTimeOffset_2, double dwellTimeOffset_3, double dwellTimeOffset_4, double dwellTimeOffset_5, double dwellTimeOffset_6, double dwellTimeOffset_7, double dwellTimeOffset_8)
    {
        RecipeName = recipeName;
        Updated = updated;
        Editor = editor;
        EditorLevel = editorLevel;
        SegmentCounts = segmentCounts;
        NitrogenMode = nitrogenMode;
        OxygenContentSet = oxygenContentSet;
        CoolingTemperature = coolingTemperature;
        CoolingTime = coolingTime;
        InflatingTime = inflatingTime;
        TemperatureSetpoint_1 = temperatureSetpoint_1;
        TemperatureSetpoint_2 = temperatureSetpoint_2;
        TemperatureSetpoint_3 = temperatureSetpoint_3;
        TemperatureSetpoint_4 = temperatureSetpoint_4;
        TemperatureSetpoint_5 = temperatureSetpoint_5;
        TemperatureSetpoint_6 = temperatureSetpoint_6;
        TemperatureSetpoint_7 = temperatureSetpoint_7;
        TemperatureSetpoint_8 = temperatureSetpoint_8;
        RampTime_1 = rampTime_1;
        RampTime_2 = rampTime_2;
        RampTime_3 = rampTime_3;
        RampTime_4 = rampTime_4;
        RampTime_5 = rampTime_5;
        RampTime_6 = rampTime_6;
        RampTime_7 = rampTime_7;
        RampTime_8 = rampTime_8;
        RampAlarm_1 = rampAlarm_1;
        RampAlarm_2 = rampAlarm_2;
        RampAlarm_3 = rampAlarm_3;
        RampAlarm_4 = rampAlarm_4;
        RampAlarm_5 = rampAlarm_5;
        RampAlarm_6 = rampAlarm_6;
        RampAlarm_7 = rampAlarm_7;
        RampAlarm_8 = rampAlarm_8;
        DwellTemperature_1 = dwellTemperature_1;
        DwellTemperature_2 = dwellTemperature_2;
        DwellTemperature_3 = dwellTemperature_3;
        DwellTemperature_4 = dwellTemperature_4;
        DwellTemperature_5 = dwellTemperature_5;
        DwellTemperature_6 = dwellTemperature_6;
        DwellTemperature_7 = dwellTemperature_7;
        DwellTemperature_8 = dwellTemperature_8;
        DwellTime_1 = dwellTime_1;
        DwellTime_2 = dwellTime_2;
        DwellTime_3 = dwellTime_3;
        DwellTime_4 = dwellTime_4;
        DwellTime_5 = dwellTime_5;
        DwellTime_6 = dwellTime_6;
        DwellTime_7 = dwellTime_7;
        DwellTime_8 = dwellTime_8;
        DwellAlarm_1 = dwellTime_1;
        DwellAlarm_2 = dwellTime_2;
        DwellAlarm_3 = dwellTime_3;
        DwellAlarm_4 = dwellTime_4;
        DwellAlarm_5 = dwellTime_5;
        DwellAlarm_6 = dwellTime_6;
        DwellAlarm_7 = dwellTime_7;
        DwellAlarm_8 = dwellTime_8;
        DwellTimeOffset_1 = dwellTimeOffset_1;
        DwellTimeOffset_2 = dwellTimeOffset_2;
        DwellTimeOffset_3 = dwellTimeOffset_3;
        DwellTimeOffset_4 = dwellTimeOffset_4;
        DwellTimeOffset_5 = dwellTimeOffset_5;
        DwellTimeOffset_6 = dwellTimeOffset_6;
        DwellTimeOffset_7 = dwellTimeOffset_7;
        DwellTimeOffset_8 = dwellTimeOffset_8;
    }
}
