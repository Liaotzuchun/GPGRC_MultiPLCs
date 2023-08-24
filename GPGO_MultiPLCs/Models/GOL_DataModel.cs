using System.Collections.Generic;
using GPMVVM.Models;
using PLCService;

namespace GPGO_MultiPLCs.Models;

public class GOL_DataModel : PLCDataProvider
{
    public GOL_DataModel(IGate plcGate, int plcIndex, string plcTag, (Dictionary<BitType, int> bits_shift, Dictionary<DataType, int> datas_shift) shift = new()) : base(plcGate, plcIndex, plcTag, shift) { }

    #region PC=>PLC
    [PLCBit(BitType.M, 21, LogType.None)]
    public bool Check
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>門鎖開關</summary>
    [PLCBit(BitType.M, 350, LogType.None)]
    public bool DoorLock
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>警報器靜音</summary>
    [PLCBit(BitType.M, 351, LogType.None)]
    public bool BeepSilince
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCData(DataType.D, 4100, 16, LogType.None)]
    public string? PartID
    {
        get => Get<string>();
        set => Set(value);
    }

    [PLCData(DataType.D, 4120, 16, LogType.None)]
    public string? LotID
    {
        get => Get<string>();
        set => Set(value);
    }

    [PLCData(DataType.D, 4140, LogType.None)]
    public short Quantity
    {
        get => Get<short>();
        set => Set(value);
    }
    #endregion

    #region 配方設定值
    /// <summary>配方名</summary>
    [PLCData(DataType.D, 2980, 16, LogType.RecipeSet)]
    public string RecipeName
    {
        get => Get<string>();
        set => Set(value);
    }

    /// <summary>降溫溫度</summary>
    [PLCData(DataType.D, 2992, 0.1, LogType.RecipeSet)]
    public double CoolingTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>降溫時間</summary>
    [PLCData(DataType.D, 2976, 0.1, LogType.RecipeSet)]
    public double CoolingTime
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>氮氣模式</summary>
    [PLCBitData(DataType.D, 2989, 0, LogType.RecipeSet)]
    public bool NitrogenMode
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>充氣逾時時間</summary>
    [PLCData(DataType.D, 2990, 1.0, LogType.RecipeSet)]
    public double InflatingTime
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>含氧量設定</summary>
    [PLCData(DataType.D, 2999, 0.1, LogType.RecipeSet)]
    public double OxygenContentSet
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>使用段數</summary>
    [PLCData(DataType.D, 2975, LogType.RecipeSet)]
    public short SegmentCounts
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>目標溫度1</summary>
    [PLCData(DataType.D, 2900, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度2</summary>
    [PLCData(DataType.D, 2901, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度3</summary>
    [PLCData(DataType.D, 2902, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度4</summary>
    [PLCData(DataType.D, 2903, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度5</summary>
    [PLCData(DataType.D, 2904, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度6</summary>
    [PLCData(DataType.D, 2905, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度7</summary>
    public double TemperatureSetpoint_7
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度8</summary>
    public double TemperatureSetpoint_8
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫溫度1</summary>
    public double DwellTemperature_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫溫度2</summary>
    public double DwellTemperature_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫溫度3</summary>
    public double DwellTemperature_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫溫度4</summary>
    public double DwellTemperature_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫溫度5</summary>
    public double DwellTemperature_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫溫度6</summary>
    public double DwellTemperature_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫溫度7</summary>
    public double DwellTemperature_7
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫溫度8</summary>
    public double DwellTemperature_8
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間1</summary>
    [PLCData(DataType.D, 2960, 0.1, LogType.RecipeSet)]
    public float RampTime_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫時間2</summary>
    [PLCData(DataType.D, 2962, 0.1, LogType.RecipeSet)]
    public float RampTime_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫時間3</summary>
    [PLCData(DataType.D, 2964, 0.1, LogType.RecipeSet)]
    public float RampTime_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫時間4</summary>
    [PLCData(DataType.D, 2966, 0.1, LogType.RecipeSet)]
    public float RampTime_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫時間5</summary>
    [PLCData(DataType.D, 2968, 0.1, LogType.RecipeSet)]
    public float RampTime_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫時間6</summary>
    [PLCData(DataType.D, 2970, 0.1, LogType.RecipeSet)]
    public float RampTime_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫時間7</summary>
    public float RampTime_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫時間8</summary>
    public float RampTime_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫警報時間1</summary>
    [PLCData(DataType.D, 2930, 0.1, LogType.RecipeSet)]
    public float RampAlarm_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫警報時間2</summary>
    [PLCData(DataType.D, 2932, 0.1, LogType.RecipeSet)]
    public float RampAlarm_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫警報時間3</summary>
    [PLCData(DataType.D, 2934, 0.1, LogType.RecipeSet)]
    public float RampAlarm_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫警報時間4</summary>
    [PLCData(DataType.D, 2936, 0.1, LogType.RecipeSet)]
    public float RampAlarm_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫警報時間5</summary>
    [PLCData(DataType.D, 2938, 0.1, LogType.RecipeSet)]
    public float RampAlarm_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫警報時間6</summary>
    [PLCData(DataType.D, 2940, 0.1, LogType.RecipeSet)]
    public float RampAlarm_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫警報時間7</summary>
    public float RampAlarm_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>升溫警報時間8</summary>
    public float RampAlarm_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫時間1</summary>
    [PLCData(DataType.D, 2915, 0.1, LogType.RecipeSet)]
    public float DwellTime_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫時間2</summary>
    [PLCData(DataType.D, 2917, 0.1, LogType.RecipeSet)]
    public float DwellTime_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫時間3</summary>
    [PLCData(DataType.D, 2919, 0.1, LogType.RecipeSet)]
    public float DwellTime_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫時間4</summary>
    [PLCData(DataType.D, 2921, 0.1, LogType.RecipeSet)]
    public float DwellTime_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫時間5</summary>
    [PLCData(DataType.D, 2923, 0.1, LogType.RecipeSet)]
    public float DwellTime_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫時間6</summary>
    [PLCData(DataType.D, 2925, 0.1, LogType.RecipeSet)]
    public float DwellTime_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫時間7</summary>
    public float DwellTime_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫時間8</summary>
    public float DwellTime_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間1</summary>
    [PLCData(DataType.D, 2945, 0.1, LogType.RecipeSet)]
    public float DwellAlarm_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間2</summary>
    [PLCData(DataType.D, 2947, 0.1, LogType.RecipeSet)]
    public float DwellAlarm_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間3</summary>
    [PLCData(DataType.D, 2949, 0.1, LogType.RecipeSet)]
    public float DwellAlarm_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間4</summary>
    [PLCData(DataType.D, 2951, 0.1, LogType.RecipeSet)]
    public float DwellAlarm_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間5</summary>
    [PLCData(DataType.D, 2953, 0.1, LogType.RecipeSet)]
    public float DwellAlarm_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間6</summary>
    [PLCData(DataType.D, 2955, 0.1, LogType.RecipeSet)]
    public float DwellAlarm_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間7</summary>
    public float DwellAlarm_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間8</summary>
    public float DwellAlarm_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset1</summary>
    [PLCData(DataType.D, 2993, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset2</summary>
    [PLCData(DataType.D, 2994, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset3</summary>
    [PLCData(DataType.D, 2995, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset4</summary>
    [PLCData(DataType.D, 2996, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset5</summary>
    [PLCData(DataType.D, 2997, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset6</summary>
    [PLCData(DataType.D, 2998, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_6
    {
        get => Get<double>();
        set => Set(value);
    }
    #endregion

    #region 配方運作值(配方SV)
    /// <summary>配方SV 配方名</summary>
    [PLCData(DataType.D, 780, 16, LogType.RecipeSet)]
    public string SV_RecipeName
    {
        get => Get<string>();
        set => Set(value);
    }

    /// <summary>配方SV 降溫溫度</summary>
    [PLCData(DataType.D, 792, 0.1, LogType.RecipeSet)]
    public double SV_CoolingTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 降溫時間</summary>
    [PLCData(DataType.D, 776, 0.1, LogType.StatusVariables)]
    public double SV_CoolingTime
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 氮氣模式</summary>
    [PLCBitData(DataType.D, 789, 0, LogType.RecipeSet)]
    public bool SV_NitrogenMode
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>配方SV 充氣逾時時間</summary>
    [PLCData(DataType.D, 790, 1.0, LogType.StatusVariables)]
    public double SV_InflatingTime
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 含氧量設定</summary>
    [PLCData(DataType.D, 799, 0.1, LogType.StatusVariables)]
    public double SV_OxygenContentSet
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 使用段數</summary>
    [PLCData(DataType.D, 775, LogType.StatusVariables)]
    public short SV_SegmentCounts
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>配方SV 目標溫度1</summary>
    [PLCData(DataType.D, 700, 0.1, LogType.StatusVariables)]
    public double SV_TemperatureSetpoint_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 目標溫度2</summary>
    [PLCData(DataType.D, 701, 0.1, LogType.StatusVariables)]
    public double SV_TemperatureSetpoint_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 目標溫度3</summary>
    [PLCData(DataType.D, 702, 0.1, LogType.StatusVariables)]
    public double SV_TemperatureSetpoint_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 目標溫度4</summary>
    [PLCData(DataType.D, 703, 0.1, LogType.StatusVariables)]
    public double SV_TemperatureSetpoint_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 目標溫度5</summary>
    [PLCData(DataType.D, 704, 0.1, LogType.StatusVariables)]
    public double SV_TemperatureSetpoint_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 目標溫度6</summary>
    [PLCData(DataType.D, 705, 0.1, LogType.StatusVariables)]
    public double SV_TemperatureSetpoint_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 目標溫度7</summary>
    public double SV_TemperatureSetpoint_7
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 目標溫度8</summary>
    public double SV_TemperatureSetpoint_8
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫溫度1</summary>
    public double SV_DwellTemperature_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫溫度2</summary>
    public double SV_DwellTemperature_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫溫度3</summary>
    public double SV_DwellTemperature_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫溫度4</summary>
    public double SV_DwellTemperature_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫溫度5</summary>
    public double SV_DwellTemperature_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫溫度6</summary>
    public double SV_DwellTemperature_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫溫度7</summary>
    public double SV_DwellTemperature_7
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫溫度8</summary>
    public double SV_DwellTemperature_8
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫時間1</summary>
    [PLCData(DataType.D, 760, 0.1, LogType.StatusVariables)]
    public float SV_RampTime_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫時間2</summary>
    [PLCData(DataType.D, 762, 0.1, LogType.StatusVariables)]
    public float SV_RampTime_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫時間3</summary>
    [PLCData(DataType.D, 764, 0.1, LogType.StatusVariables)]
    public float SV_RampTime_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫時間4</summary>
    [PLCData(DataType.D, 766, 0.1, LogType.StatusVariables)]
    public float SV_RampTime_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫時間5</summary>
    [PLCData(DataType.D, 768, 0.1, LogType.StatusVariables)]
    public float SV_RampTime_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫時間6</summary>
    [PLCData(DataType.D, 770, 0.1, LogType.StatusVariables)]
    public float SV_RampTime_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫時間7</summary>
    public float SV_RampTime_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫時間8</summary>
    public float SV_RampTime_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫警報時間1</summary>
    [PLCData(DataType.D, 730, 0.1, LogType.StatusVariables)]
    public float SV_RampAlarm_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫警報時間2</summary>
    [PLCData(DataType.D, 732, 0.1, LogType.StatusVariables)]
    public float SV_RampAlarm_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫警報時間3</summary>
    [PLCData(DataType.D, 734, 0.1, LogType.StatusVariables)]
    public float SV_RampAlarm_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫警報時間4</summary>
    [PLCData(DataType.D, 736, 0.1, LogType.StatusVariables)]
    public float SV_RampAlarm_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫警報時間5</summary>
    [PLCData(DataType.D, 738, 0.1, LogType.StatusVariables)]
    public float SV_RampAlarm_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫警報時間6</summary>
    [PLCData(DataType.D, 740, 0.1, LogType.StatusVariables)]
    public float SV_RampAlarm_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫警報時間7</summary>
    public float SV_RampAlarm_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 升溫警報時間8</summary>
    public float SV_RampAlarm_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間1</summary>
    [PLCData(DataType.D, 715, 0.1, LogType.StatusVariables)]
    public float SV_DwellTime_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間2</summary>
    [PLCData(DataType.D, 717, 0.1, LogType.StatusVariables)]
    public float SV_DwellTime_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間3</summary>
    [PLCData(DataType.D, 719, 0.1, LogType.StatusVariables)]
    public float SV_DwellTime_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間4</summary>
    [PLCData(DataType.D, 721, 0.1, LogType.StatusVariables)]
    public float SV_DwellTime_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間5</summary>
    [PLCData(DataType.D, 723, 0.1, LogType.StatusVariables)]
    public float SV_DwellTime_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間6</summary>
    [PLCData(DataType.D, 725, 0.1, LogType.StatusVariables)]
    public float SV_DwellTime_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間7</summary>
    public float SV_DwellTime_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間8</summary>
    public float SV_DwellTime_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫警報時間1</summary>
    [PLCData(DataType.D, 745, 0.1, LogType.StatusVariables)]
    public float SV_DwellAlarm_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫警報時間2</summary>
    [PLCData(DataType.D, 747, 0.1, LogType.StatusVariables)]
    public float SV_DwellAlarm_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫警報時間3</summary>
    [PLCData(DataType.D, 749, 0.1, LogType.StatusVariables)]
    public float SV_DwellAlarm_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫警報時間4</summary>
    [PLCData(DataType.D, 751, 0.1, LogType.StatusVariables)]
    public float SV_DwellAlarm_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫警報時間5</summary>
    [PLCData(DataType.D, 753, 0.1, LogType.StatusVariables)]
    public float SV_DwellAlarm_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫警報時間6</summary>
    [PLCData(DataType.D, 755, 0.1, LogType.StatusVariables)]
    public float SV_DwellAlarm_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫警報時間7</summary>
    public float SV_DwellAlarm_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫警報時間8</summary>
    public float SV_DwellAlarm_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間Offset1</summary>
    [PLCData(DataType.D, 793, 0.1, LogType.RecipeSet)]
    public double SV_DwellTimeOffset_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間Offset2</summary>
    [PLCData(DataType.D, 794, 0.1, LogType.RecipeSet)]
    public double SV_DwellTimeOffset_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間Offset3</summary>
    [PLCData(DataType.D, 795, 0.1, LogType.RecipeSet)]
    public double SV_DwellTimeOffset_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間Offset4</summary>
    [PLCData(DataType.D, 796, 0.1, LogType.RecipeSet)]
    public double SV_DwellTimeOffset_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間Offset5</summary>
    [PLCData(DataType.D, 797, 0.1, LogType.RecipeSet)]
    public double SV_DwellTimeOffset_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>配方SV 恆溫時間Offset6</summary>
    [PLCData(DataType.D, 798, 0.1, LogType.RecipeSet)]
    public double SV_DwellTimeOffset_6
    {
        get => Get<double>();
        set => Set(value);
    }
    #endregion

    #region 警報
    /// <summary>緊急停止</summary>
    [PLCBit(BitType.M, 700, LogType.Alarm)]
    public bool EmergencyStop
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>溫控器低溫異常</summary>
    public bool LowTemperature
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>電源相位異常</summary>
    [PLCBit(BitType.M, 702, LogType.Alarm)]
    public bool PowerPhaseError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>OTP超溫異常</summary>
    [PLCBit(BitType.M, 703, LogType.Alarm)]
    public bool OTPTemperatureError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>循環風車電流異常</summary>
    [PLCBit(BitType.M, 704, LogType.Alarm)]
    public bool CirculatingFanCurrentError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>冷卻進氣風車異常</summary>
    public bool CoolingFanError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>溫控器訊號異常</summary>
    public bool ThermostatSignalError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>溫控器電流異常</summary>
    [PLCBit(BitType.M, 717, LogType.Alert)]
    public bool ThermostatCurrentError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>升溫中停止</summary>
    [PLCBit(BitType.M, 708, LogType.Alert)]
    public bool Ramp_Stop
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>恆溫中停止</summary>
    [PLCBit(BitType.M, 710, LogType.Alert)]
    public bool Dwell_Stop
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>冷卻中停止</summary>
    [PLCBit(BitType.M, 711, LogType.Alert)]
    public bool Cooling_Stop
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>充氮氣中停止</summary>
    [PLCBit(BitType.M, 712, LogType.Alert)]
    public bool Inflating_Stop
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>開門極限</summary>
    public bool DoorOpeningLimit
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>關門極限</summary>
    public bool DoorClosingLimit
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>含氧儀上限警報</summary>
    [PLCBit(BitType.M, 705, LogType.Alert)]
    public bool OxygenMeterUpperLimitAlarm
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>超溫警報</summary>
    [PLCBit(BitType.M, 302, LogType.Alert)]
    public bool OverTemperatureAlarm
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>溫控器通訊異常</summary>
    [PLCBit(BitType.M, 723, LogType.Alert)]
    public bool ThermostatCommunicationError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>電動門要求復歸</summary>
    public bool ElectricDoorInitRequest
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>電動門不在位置上</summary>
    public bool ElectricDoorPositionError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>回原點逾時</summary>
    public bool BackToOriginTimeout
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>停止後未開門</summary>
    [PLCBit(BitType.M, 714, LogType.Alert)]
    public bool DoorNotOpen
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>汽缸伸出逾時</summary>
    public bool CylinderExtendTimeout
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>汽缸縮回逾時</summary>
    public bool CylinderRetractTimeout
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>循環風車變頻器異常</summary>
    public bool CirculatingFanInverterError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>空氣壓力不足</summary>
    [PLCBit(BitType.M, 701, LogType.Alert)]
    public bool InsufficientAirPressure
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>EGO超溫</summary>
    public bool EGOOverTemperature
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>充氮氣逾時</summary>
    [PLCBit(BitType.M, 707, LogType.Alert)]
    public bool InflatingTimeExceeded
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>門未關定位異常</summary>
    public bool DoorNotClosedPositionException
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>升恆溫逾時</summary>
    [PLCBit(BitType.M, 240, LogType.Alert)]
    public bool RampTimeExceeded
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>加熱分路跳脫</summary>
    public bool RampBranchException
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>BarCode讀取異常</summary>
    public bool BarCodeReadError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>PLC電池電壓不足</summary>
    public bool PLCBatteryLow
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>電熱ELB跳脫</summary>
    [PLCBit(BitType.M, 719, LogType.Alarm)]
    public bool ELBtrip
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>伺服驅動器電池電壓不足</summary>
    public bool ServoDriverBatteryVoltLow
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>開門極限異常</summary>
    public bool DoorOpenLimitError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>關門極限異常</summary>
    public bool DoorCloseLimitError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>伺服驅動器異常</summary>
    public bool ServoDriverError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>光閘偵測異常</summary>
    public bool RasterError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>程式結束</summary>
    public bool ProgramStop
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>加熱門未關</summary>
    [PLCBit(BitType.M, 250, LogType.Alert)]
    public bool DoorNotClosed
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 721, LogType.Alert)]
    public bool DoorNotClosed_Auto
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 722, LogType.Alert)]
    public bool DoorNotClosed_Process
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>電控箱溫度異常</summary>
    public bool ElectricControlBoxTemperatureError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>進氣風車異常</summary>
    public bool IntakeWindmillError
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>風速下限異常</summary>
    public bool WindSpeedLowerLimitAlarm
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>風速上限異常</summary>
    public bool WindSpeedUpperLimitAlarm
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>配方切換異常</summary>
    public bool RecipeChangeError
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 706, LogType.Alarm)]
    public bool SpareAlarm
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 709, LogType.Alarm)]
    public bool SpareAlarm3
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 713, LogType.Alarm)]
    public bool SpareAlarm4
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 725, LogType.Alarm)]
    public bool SpareAlarm5
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 726, LogType.Alarm)]
    public bool SpareAlarm6
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 727, LogType.Alarm)]
    public bool SpareAlarm7
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 728, LogType.Alarm)]
    public bool SpareAlarm8
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 729, LogType.Alarm)]
    public bool SpareAlarm9
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm10
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm11
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm12
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm13
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm14
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm15
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm16
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm17
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm18
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm19
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm20
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm21
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm22
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm23
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm24
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SpareAlarm25
    {
        get => Get<bool>();
        set => Set(value);
    }
    #endregion

    #region 機台狀態
    //public bool ReadBarcode
    //{
    //    get => Get<bool>();
    //    set => Set(value);
    //}

    //public bool RackInput
    //{
    //    get => Get<bool>();
    //    set => Set(value);
    //}

    //public bool RackOutput
    //{
    //    get => Get<bool>();
    //    set => Set(value);
    //}

    /// <summary>程式結束警報時間</summary>
    [PLCData(DataType.D, 157, 0.1, LogType.EquipmentConstants)]
    public double ProgramEndWarningTime
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>允許啟動</summary>
    [PLCBit(BitType.M, 50, LogType.StatusVariables)]
    public bool AllowStart
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>允許停止</summary>
    [PLCBit(BitType.M, 209, LogType.StatusVariables)]
    public bool AllowStop
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>綠燈</summary>
    [PLCBit(BitType.Y, 4, LogType.None)]
    public bool GreenLight
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>黃燈</summary>
    [PLCBit(BitType.Y, 5, LogType.None)]
    public bool YellowLight
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>紅燈</summary>
    [PLCBit(BitType.Y, 6, LogType.None)]
    public bool RedLight
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>藍燈</summary>
    [PLCBit(BitType.Y, 7, LogType.None)]
    public bool BlueLight
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>升溫中</summary>
    public bool IsRamp
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>恆溫中</summary>
    public bool IsDwell
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>降溫中</summary>
    public bool IsCooling
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>自動模式</summary>
    [PLCBit(BitType.M, 50, LogType.StatusVariables)]
    public bool AutoMode
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>手動模式</summary>
    [PLCBit(BitType.M, 60, LogType.StatusVariables)]
    public bool ManualMode
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>充氣中</summary>
    public bool Inflating
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 51, LogType.StatusVariables)]
    public bool AutoMode_Start
    {
        get => Get<bool>();
        set
        {
            if (AutoMode)
            {
                Set(value);
            }
        }
    }

    /// <summary>程式結束(程式結束會早於AutoMode_Stop)</summary>
    [PLCBit(BitType.M, 209, LogType.StatusVariables)]
    public bool ProcessComplete
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>自動模式停止(需要手動按)</summary>
    [PLCBit(BitType.M, 52, LogType.StatusVariables)]
    public bool AutoMode_Stop
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 341, LogType.StatusVariables)]
    public bool RemoteMode
    {
        get => Get<bool>();
        set => Set(value);
    }

    //public bool LocalMode
    //{
    //    get => Get<bool>();
    //    set => Set(value);
    //}

    //public bool RecipeChanged
    //{
    //    get => Get<bool>();
    //    set => Set(value);
    //}

    /// <summary>溫控器實際溫度</summary>
    [PLCData(DataType.D, 64, 0.1, LogType.StatusVariables)]
    public double PV_ThermostatTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>溫控器設定溫度</summary>
    [PLCData(DataType.D, 65, 0.1, LogType.StatusVariables)]
    public double ThermostatTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>氮氣流量(L/m)</summary>
    [PLCData(DataType.D, 402, LogType.StatusVariables)]
    public double NitrogenFlow
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>含氧量(%)</summary>
    [PLCData(DataType.D, 404, 0.1, LogType.StatusVariables)]
    public double OxygenContent
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>現在風速(m/s)</summary>
    [PLCData(DataType.D, 670, 0.1, LogType.StatusVariables)]
    public double PV_WindSpeed
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>設定風速(m/s)</summary>
    public double WindSpeed
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>耗電量(kWh)</summary>
    [PLCData(DataType.D, 686, 0.1, LogType.StatusVariables)]
    public double PowerConsumption
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>爐內溫度1</summary>
    [PLCData(DataType.D, 500, 0.1, LogType.StatusVariables)]
    public double OvenTemperature_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>爐內溫度2</summary>
    [PLCData(DataType.D, 501, 0.1, LogType.StatusVariables)]
    public double OvenTemperature_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>爐內溫度3</summary>
    [PLCData(DataType.D, 502, 0.1, LogType.StatusVariables)]
    public double OvenTemperature_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>爐內溫度4</summary>
    [PLCData(DataType.D, 503, 0.1, LogType.StatusVariables)]
    public double OvenTemperature_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>爐內溫度5</summary>
    [PLCData(DataType.D, 504, 0.1, LogType.StatusVariables)]
    public double OvenTemperature_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>爐內溫度6</summary>
    [PLCData(DataType.D, 505, 0.1, LogType.StatusVariables)]
    public double OvenTemperature_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>爐內溫度7</summary>
    public double OvenTemperature_7
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>爐內溫度8</summary>
    public double OvenTemperature_8
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目前段數</summary>
    [PLCData(DataType.D, 22, LogType.StatusVariables)]
    public short CurrentSegment
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>RackID</summary>
    public string RackID
    {
        get => Get<string>();
        set => Set(value);
    }

    /// <summary>設備狀態，0=待機，1=生產中，2=自動停止，3=設備異常</summary>
    [PLCData(DataType.D, 60, LogType.StatusVariables)]
    public short EquipmentState
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>生產狀態，0=手動，1=升溫中，2=恆溫中，7=冷卻中，8=程式結束，9=自動，10=氮氣充氣中</summary>
    [PLCData(DataType.D, 28, LogType.StatusVariables)]
    public short ProcessState
    {
        get => Get<short>();
        set => Set(value);
    }

    public string EquipmentName
    {
        get => Get<string>();
        set => Set(value);
    }

    /// <summary>配方PV 升溫時間1</summary>
    [PLCData(DataType.D, 370, 0.1, LogType.StatusVariables)]
    public float PV_RampTime_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 升溫時間2</summary>
    [PLCData(DataType.D, 372, 0.1, LogType.StatusVariables)]
    public float PV_RampTime_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 升溫時間3</summary>
    [PLCData(DataType.D, 374, 0.1, LogType.StatusVariables)]
    public float PV_RampTime_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 升溫時間4</summary>
    [PLCData(DataType.D, 376, 0.1, LogType.StatusVariables)]
    public float PV_RampTime_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 升溫時間5</summary>
    [PLCData(DataType.D, 378, 0.1, LogType.StatusVariables)]
    public float PV_RampTime_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 升溫時間6</summary>
    [PLCData(DataType.D, 380, 0.1, LogType.StatusVariables)]
    public float PV_RampTime_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 升溫時間7</summary>
    public float PV_RampTime_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 升溫時間8</summary>
    public float PV_RampTime_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 恆溫時間1</summary>
    [PLCData(DataType.D, 386, 0.1, LogType.StatusVariables)]
    public float PV_DwellTime_1
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 恆溫時間2</summary>
    [PLCData(DataType.D, 388, 0.1, LogType.StatusVariables)]
    public float PV_DwellTime_2
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 恆溫時間3</summary>
    [PLCData(DataType.D, 390, 0.1, LogType.StatusVariables)]
    public float PV_DwellTime_3
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 恆溫時間4</summary>
    [PLCData(DataType.D, 392, 0.1, LogType.StatusVariables)]
    public float PV_DwellTime_4
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 恆溫時間5</summary>
    [PLCData(DataType.D, 394, 0.1, LogType.StatusVariables)]
    public float PV_DwellTime_5
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 恆溫時間6</summary>
    [PLCData(DataType.D, 396, 0.1, LogType.StatusVariables)]
    public float PV_DwellTime_6
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 恆溫時間7</summary>
    public float PV_DwellTime_7
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 恆溫時間8</summary>
    public float PV_DwellTime_8
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>配方PV 降溫時間</summary>
    [PLCData(DataType.D, 137, 0.1, LogType.StatusVariables)]
    public float PV_CoolingTime
    {
        get => Get<float>();
        set => Set(value);
    }

    /// <summary>烘烤剩餘時間</summary>
    [PLCData(DataType.D, 414, 0.1, LogType.StatusVariables)]
    public float RemainTime
    {
        get => Get<float>();
        set => Set(value);
    }

    [PLCData(DataType.D, 412, 0.1, LogType.StatusVariables)]
    public float TotalTime
    {
        get => Get<float>();
        set => Set(value);
    }
    #endregion
}