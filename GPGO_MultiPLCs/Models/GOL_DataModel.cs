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

    [PLCData(DataType.D, 120, 16, LogType.None)]
    public string? TopLotID
    {
        get => Get<string>();
        set => Set(value);
    }

    [PLCData(DataType.D, 4120, 16, LogType.None)]
    public string? BottomLotID
    {
        get => Get<string>();
        set => Set(value);
    }

    [PLCData(DataType.D, 140, LogType.None)]
    public int TopQuantity
    {
        get => Get<int>();
        set => Set(value);
    }

    [PLCData(DataType.D, 4140, LogType.None)]
    public int BottomQuantity
    {
        get => Get<int>();
        set => Set(value);
    }
    #endregion

    #region 配方設定值 上爐
    /// <summary>配方名</summary>
    [PLCData(DataType.D, 700, 20, LogType.RecipeSet)]
    public string RecipeName
    {
        get => Get<string>();
        set => Set(value);
    }

    /// <summary>降溫溫度</summary>
    [PLCData(DataType.D, 713, 0.1, LogType.RecipeSet)]
    public double CoolingTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>降溫時間</summary>
    [PLCData(DataType.D, 712, 0.1, LogType.RecipeSet)]
    public double CoolingTime
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>氮氣模式</summary>
    [PLCBitData(DataType.D, 752, 0, LogType.RecipeSet)]
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
    [PLCData(DataType.D, 753, 0.1, LogType.RecipeSet)]
    public double OxygenContentSet
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>使用段數</summary>
    [PLCData(DataType.D, 711, LogType.RecipeSet)]
    public short SegmentCounts
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>目標溫度1</summary>
    [PLCData(DataType.D, 715, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度2</summary>
    [PLCData(DataType.D, 716, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度3</summary>
    [PLCData(DataType.D, 717, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度4</summary>
    [PLCData(DataType.D, 718, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度5</summary>
    [PLCData(DataType.D, 719, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度6</summary>
    [PLCData(DataType.D, 720, 0.1, LogType.RecipeSet)]
    public double TemperatureSetpoint_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間1</summary>
    [PLCData(DataType.D, 721, 0.1, LogType.RecipeSet)]
    public double RampTime_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間2</summary>
    [PLCData(DataType.D, 722, 0.1, LogType.RecipeSet)]
    public double RampTime_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間3</summary>
    [PLCData(DataType.D, 723, 0.1, LogType.RecipeSet)]
    public double RampTime_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間4</summary>
    [PLCData(DataType.D, 724, 0.1, LogType.RecipeSet)]
    public double RampTime_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間5</summary>
    [PLCData(DataType.D, 725, 0.1, LogType.RecipeSet)]
    public double RampTime_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間6</summary>
    [PLCData(DataType.D, 726, 0.1, LogType.RecipeSet)]
    public double RampTime_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間1</summary>
    [PLCData(DataType.D, 727, 0.1, LogType.RecipeSet)]
    public double RampAlarm_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間2</summary>
    [PLCData(DataType.D, 728, 0.1, LogType.RecipeSet)]
    public double RampAlarm_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間3</summary>
    [PLCData(DataType.D, 729, 0.1, LogType.RecipeSet)]
    public double RampAlarm_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間4</summary>
    [PLCData(DataType.D, 730, 0.1, LogType.RecipeSet)]
    public double RampAlarm_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間5</summary>
    [PLCData(DataType.D, 731, 0.1, LogType.RecipeSet)]
    public double RampAlarm_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間6</summary>
    [PLCData(DataType.D, 732, 0.1, LogType.RecipeSet)]
    public double RampAlarm_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間1</summary>
    [PLCData(DataType.D, 733, 0.1, LogType.RecipeSet)]
    public double DwellTime_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間2</summary>
    [PLCData(DataType.D, 734, 0.1, LogType.RecipeSet)]
    public double DwellTime_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間3</summary>
    [PLCData(DataType.D, 735, 0.1, LogType.RecipeSet)]
    public double DwellTime_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間4</summary>
    [PLCData(DataType.D, 736, 0.1, LogType.RecipeSet)]
    public double DwellTime_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間5</summary>
    [PLCData(DataType.D, 737, 0.1, LogType.RecipeSet)]
    public double DwellTime_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間6</summary>
    [PLCData(DataType.D, 738, 0.1, LogType.RecipeSet)]
    public double DwellTime_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間1</summary>
    [PLCData(DataType.D, 739, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間2</summary>
    [PLCData(DataType.D, 740, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間3</summary>
    [PLCData(DataType.D, 741, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間4</summary>
    [PLCData(DataType.D, 742, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間5</summary>
    [PLCData(DataType.D, 743, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間6</summary>
    [PLCData(DataType.D, 744, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset1</summary>
    [PLCData(DataType.D, 745, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset2</summary>
    [PLCData(DataType.D, 746, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset3</summary>
    [PLCData(DataType.D, 747, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset4</summary>
    [PLCData(DataType.D, 748, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset5</summary>
    [PLCData(DataType.D, 749, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset6</summary>
    [PLCData(DataType.D, 750, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_6
    {
        get => Get<double>();
        set => Set(value);
    }
    #endregion
    #region 配方設定值 下爐
    /// <summary>配方名</summary>
    [PLCData(DataType.D, 800, 16, LogType.RecipeSet)]
    public string BottomRecipeName
    {
        get => Get<string>();
        set => Set(value);
    }

    /// <summary>降溫溫度</summary>
    [PLCData(DataType.D, 813, 0.1, LogType.RecipeSet)]
    public double BottomCoolingTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>降溫時間</summary>
    [PLCData(DataType.D, 812, 0.1, LogType.RecipeSet)]
    public double BottomCoolingTime
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>氮氣模式</summary>
    [PLCBitData(DataType.D, 852, 0, LogType.RecipeSet)]
    public bool BottomNitrogenMode
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>充氣逾時時間</summary>
    [PLCData(DataType.D, 3990, 1.0, LogType.RecipeSet)]
    public double BottomInflatingTime
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>含氧量設定</summary>
    [PLCData(DataType.D, 853, 0.1, LogType.RecipeSet)]
    public double BottomOxygenContentSet
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>使用段數</summary>
    [PLCData(DataType.D, 811, LogType.RecipeSet)]
    public short BottomSegmentCounts
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>目標溫度1</summary>
    [PLCData(DataType.D, 815, 0.1, LogType.RecipeSet)]
    public double BottomTemperatureSetpoint_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度2</summary>
    [PLCData(DataType.D, 816, 0.1, LogType.RecipeSet)]
    public double BottomTemperatureSetpoint_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度3</summary>
    [PLCData(DataType.D, 817, 0.1, LogType.RecipeSet)]
    public double BottomTemperatureSetpoint_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度4</summary>
    [PLCData(DataType.D, 818, 0.1, LogType.RecipeSet)]
    public double BottomTemperatureSetpoint_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度5</summary>
    [PLCData(DataType.D, 819, 0.1, LogType.RecipeSet)]
    public double BottomTemperatureSetpoint_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度6</summary>
    [PLCData(DataType.D, 820, 0.1, LogType.RecipeSet)]
    public double BottomTemperatureSetpoint_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間1</summary>
    [PLCData(DataType.D, 821, 0.1, LogType.RecipeSet)]
    public double BottomRampTime_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間2</summary>
    [PLCData(DataType.D, 822, 0.1, LogType.RecipeSet)]
    public double BottomRampTime_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間3</summary>
    [PLCData(DataType.D, 823, 0.1, LogType.RecipeSet)]
    public double BottomRampTime_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間4</summary>
    [PLCData(DataType.D, 824, 0.1, LogType.RecipeSet)]
    public double BottomRampTime_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間5</summary>
    [PLCData(DataType.D, 825, 0.1, LogType.RecipeSet)]
    public double BottomRampTime_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間6</summary>
    [PLCData(DataType.D, 826, 0.1, LogType.RecipeSet)]
    public double BottomRampTime_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間1</summary>
    [PLCData(DataType.D, 827, 0.1, LogType.RecipeSet)]
    public double BottomRampAlarm_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間2</summary>
    [PLCData(DataType.D, 828, 0.1, LogType.RecipeSet)]
    public double BottomRampAlarm_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間3</summary>
    [PLCData(DataType.D, 829, 0.1, LogType.RecipeSet)]
    public double BottomRampAlarm_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間4</summary>
    [PLCData(DataType.D, 830, 0.1, LogType.RecipeSet)]
    public double BottomRampAlarm_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間5</summary>
    [PLCData(DataType.D, 831, 0.1, LogType.RecipeSet)]
    public double BottomRampAlarm_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間6</summary>
    [PLCData(DataType.D, 832, 0.1, LogType.RecipeSet)]
    public double BottomRampAlarm_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間1</summary>
    [PLCData(DataType.D, 833, 0.1, LogType.RecipeSet)]
    public double BottomDwellTime_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間2</summary>
    [PLCData(DataType.D, 834, 0.1, LogType.RecipeSet)]
    public double BottomDwellTime_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間3</summary>
    [PLCData(DataType.D, 835, 0.1, LogType.RecipeSet)]
    public double BottomDwellTime_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間4</summary>
    [PLCData(DataType.D, 836, 0.1, LogType.RecipeSet)]
    public double BottomDwellTime_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間5</summary>
    [PLCData(DataType.D, 837, 0.1, LogType.RecipeSet)]
    public double BottomDwellTime_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間6</summary>
    [PLCData(DataType.D, 838, 0.1, LogType.RecipeSet)]
    public double BottomDwellTime_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間1</summary>
    [PLCData(DataType.D, 839, 0.1, LogType.RecipeSet)]
    public double BottomDwellAlarm_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間2</summary>
    [PLCData(DataType.D, 840, 0.1, LogType.RecipeSet)]
    public double BottomDwellAlarm_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間3</summary>
    [PLCData(DataType.D, 841, 0.1, LogType.RecipeSet)]
    public double BottomDwellAlarm_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間4</summary>
    [PLCData(DataType.D, 842, 0.1, LogType.RecipeSet)]
    public double BottomDwellAlarm_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間5</summary>
    [PLCData(DataType.D, 843, 0.1, LogType.RecipeSet)]
    public double BottomDwellAlarm_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間6</summary>
    [PLCData(DataType.D, 844, 0.1, LogType.RecipeSet)]
    public double BottomDwellAlarm_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset1</summary>
    [PLCData(DataType.D, 845, 0.1, LogType.RecipeSet)]
    public double BottomDwellTimeOffset_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset2</summary>
    [PLCData(DataType.D, 846, 0.1, LogType.RecipeSet)]
    public double BottomDwellTimeOffset_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset3</summary>
    [PLCData(DataType.D, 847, 0.1, LogType.RecipeSet)]
    public double BottomDwellTimeOffset_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset4</summary>
    [PLCData(DataType.D, 848, 0.1, LogType.RecipeSet)]
    public double BottomDwellTimeOffset_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset5</summary>
    [PLCData(DataType.D, 849, 0.1, LogType.RecipeSet)]
    public double BottomDwellTimeOffset_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset6</summary>
    [PLCData(DataType.D, 850, 0.1, LogType.RecipeSet)]
    public double BottomDwellTimeOffset_6
    {
        get => Get<double>();
        set => Set(value);
    }
    #endregion

    #region 警報
    /// <summary>緊急停止</summary>
    [PLCBit(BitType.M, 230, LogType.Alarm)]
    public bool M230
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>緊急停止</summary>
    [PLCBit(BitType.M, 1230, LogType.Alarm)]
    public bool M1230
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>電源相序檢測異常</summary>
    [PLCBit(BitType.M, 231, LogType.Alarm)]
    public bool M231
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>電源相序檢測異常</summary>
    [PLCBit(BitType.M, 1231, LogType.Alarm)]
    public bool M1231
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣壓源壓力檢測異常</summary>
    [PLCBit(BitType.M, 233, LogType.Alarm)]
    public bool M233
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣壓源壓力檢測異常</summary>
    [PLCBit(BitType.M, 1233, LogType.Alarm)]
    public bool M1233
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>維修門開啟微動開關</summary>
    [PLCBit(BitType.M, 234, LogType.Alarm)]
    public bool M234
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>維修門開啟微動開關</summary>
    [PLCBit(BitType.M, 1234, LogType.Alarm)]
    public bool M1234
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣動門橫移開啟定點(縮)異常</summary>
    [PLCBit(BitType.M, 235, LogType.Alarm)]
    public bool M235
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣動門橫移開啟定點(縮)異常</summary>
    [PLCBit(BitType.M, 1235, LogType.Alarm)]
    public bool M1235
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣動門橫移關閉定點(伸)異常</summary>
    [PLCBit(BitType.M, 236, LogType.Alarm)]
    public bool M236
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣動門橫移關閉定點(伸)異常</summary>
    [PLCBit(BitType.M, 1236, LogType.Alarm)]
    public bool M1236
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣動門壓合伸出定點異常</summary>
    [PLCBit(BitType.M, 237, LogType.Alarm)]
    public bool M237
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣動門壓合伸出定點異常</summary>
    [PLCBit(BitType.M, 1237, LogType.Alarm)]
    public bool M1237
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣動門壓合縮回定點異常</summary>
    [PLCBit(BitType.M, 238, LogType.Alarm)]
    public bool M238
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>氣動門壓合縮回定點異常</summary>
    [PLCBit(BitType.M, 1238, LogType.Alarm)]
    public bool M1238
    {
        get => Get<bool>();
        set => Set(value);
    }
    /// <summary>溫控器TC1溫度超溫</summary>
    [PLCBit(BitType.M, 239, LogType.Alarm)]
    public bool M239
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>溫控器TC1溫度超溫</summary>
    [PLCBit(BitType.M, 1239, LogType.Alarm)]
    public bool M1239
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>超溫防止器1溫度異常</summary>
    [PLCBit(BitType.M, 240, LogType.Alarm)]
    public bool M240
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>超溫防止器1溫度異常</summary>
    [PLCBit(BitType.M, 1240, LogType.Alarm)]
    public bool M1240
    {
        get => Get<bool>();
        set => Set(value);
    }
    /// <summary>爐門光閘</summary>
    [PLCBit(BitType.M, 241, LogType.Alarm)]
    public bool M241
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>爐門光閘</summary>
    [PLCBit(BitType.M, 1241, LogType.Alarm)]
    public bool M1241
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第1段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 243, LogType.Alarm)]
    public bool M243
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第1段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1243, LogType.Alarm)]
    public bool M1243
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第1段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 244, LogType.Alarm)]
    public bool M244
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第1段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1244, LogType.Alarm)]
    public bool M1244
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第2段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 245, LogType.Alarm)]
    public bool M245
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第2段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1245, LogType.Alarm)]
    public bool M1245
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第2段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 246, LogType.Alarm)]
    public bool M246
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第2段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1246, LogType.Alarm)]
    public bool M1246
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第3段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 247, LogType.Alarm)]
    public bool M247
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第3段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1247, LogType.Alarm)]
    public bool M1247
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第3段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 248, LogType.Alarm)]
    public bool M248
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第3段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1248, LogType.Alarm)]
    public bool M1248
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第4段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 249, LogType.Alarm)]
    public bool M249
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第4段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1249, LogType.Alarm)]
    public bool M1249
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第4段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 250, LogType.Alarm)]
    public bool M250
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第4段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1250, LogType.Alarm)]
    public bool M1250
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第5段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 251, LogType.Alarm)]
    public bool M251
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第5段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1251, LogType.Alarm)]
    public bool M1251
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第5段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 252, LogType.Alarm)]
    public bool M252
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第5段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1252, LogType.Alarm)]
    public bool M1252
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第6段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 253, LogType.Alarm)]
    public bool M253
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第6段升溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1253, LogType.Alarm)]
    public bool M1253
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第6段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 254, LogType.Alarm)]
    public bool M254
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>第6段恆溫警報時間延遲</summary>
    [PLCBit(BitType.M, 1254, LogType.Alarm)]
    public bool M1254
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>降溫時間異常警報</summary>
    [PLCBit(BitType.M, 255, LogType.Alarm)]
    public bool M255
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>降溫時間異常警報</summary>
    [PLCBit(BitType.M, 1255, LogType.Alarm)]
    public bool M1255
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>循環風車電流檢測異常</summary>
    [PLCBit(BitType.M, 256, LogType.Alarm)]
    public bool M256
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>循環風車電流檢測異常</summary>
    [PLCBit(BitType.M, 1256, LogType.Alarm)]
    public bool M1256
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>大量充氮氣逾時異常</summary>
    [PLCBit(BitType.M, 257, LogType.Alarm)]
    public bool M257
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>大量充氮氣逾時異常</summary>
    [PLCBit(BitType.M, 1257, LogType.Alarm)]
    public bool M1257
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>斷路器跳脫異常</summary>
    [PLCBit(BitType.M, 258, LogType.Alarm)]
    public bool M258
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>斷路器跳脫異常</summary>
    [PLCBit(BitType.M, 1258, LogType.Alarm)]
    public bool M1258
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>循環風車馬達異常</summary>
    [PLCBit(BitType.M, 259, LogType.Alarm)]
    public bool M259
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>循環風車馬達異常</summary>
    [PLCBit(BitType.M, 1259, LogType.Alarm)]
    public bool M1259
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>進氣風扇異常</summary>
    [PLCBit(BitType.M, 260, LogType.Alarm)]
    public bool M260
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>進氣風扇異常</summary>
    [PLCBit(BitType.M, 1260, LogType.Alarm)]
    public bool M1260
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>加熱門未關</summary>
    [PLCBit(BitType.M, 261, LogType.Alarm)]
    public bool M261
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>加熱門未關</summary>
    [PLCBit(BitType.M, 1261, LogType.Alarm)]
    public bool M1261
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>超溫防止器13溫度異常</summary>
    [PLCBit(BitType.M, 262, LogType.Alarm)]
    public bool M262
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>超溫防止器13溫度異常</summary>
    [PLCBit(BitType.M, 1262, LogType.Alarm)]
    public bool M1262
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>含氧感知器(OC1)通訊異常</summary>
    [PLCBit(BitType.M, 263, LogType.Alarm)]
    public bool M263
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>含氧感知器(OC1)通訊異常</summary>
    [PLCBit(BitType.M, 1263, LogType.Alarm)]
    public bool M1263
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>含氧感知器(OC1)通訊異常</summary>
    [PLCBit(BitType.M, 265, LogType.Alarm)]
    public bool M265
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>含氧感知器(OC1)通訊異常</summary>
    [PLCBit(BitType.M, 1265, LogType.Alarm)]
    public bool M1265
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>動力電源異常</summary>
    [PLCBit(BitType.M, 266, LogType.Alarm)]
    public bool M266
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>動力電源異常</summary>
    [PLCBit(BitType.M, 1266, LogType.Alarm)]
    public bool M1266
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>對BC通訊異常</summary>
    [PLCBit(BitType.M, 267, LogType.Alarm)]
    public bool M267
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>對BC通訊異常</summary>
    [PLCBit(BitType.M, 1267, LogType.Alarm)]
    public bool M1267
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>PLC電池不足異常</summary>
    [PLCBit(BitType.M, 268, LogType.Alarm)]
    public bool M268
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>PLC電池不足異常</summary>
    [PLCBit(BitType.M, 1268, LogType.Alarm)]
    public bool M1268
    {
        get => Get<bool>();
        set => Set(value);
    }


    /// <summary>溫控器(TC1)通訊異常</summary>
    [PLCBit(BitType.M, 269, LogType.Alarm)]
    public bool M269
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>溫控器(TC1)通訊異常</summary>
    [PLCBit(BitType.M, 1269, LogType.Alarm)]
    public bool M1269
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

    /// <summary>配方切換異常</summary>
    public bool RecipeChangeError
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


    /// <summary>綠燈</summary>
    [PLCBit(BitType.Y, 40, LogType.None)]
    public bool GreenLight
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>黃燈</summary>
    [PLCBit(BitType.Y, 41, LogType.None)]
    public bool YellowLight
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>紅燈</summary>
    [PLCBit(BitType.Y, 42, LogType.None)]
    public bool RedLight
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>藍燈</summary>
    [PLCBit(BitType.Y, 39, LogType.None)]
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

    /// <summary>充氣中</summary>
    public bool Inflating
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

    [PLCBit(BitType.M, 51, LogType.StatusVariables)]
    public bool TopAutoMode_Start
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

    [PLCBit(BitType.M, 1051, LogType.StatusVariables)]
    public bool BottomAutoMode_Start
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
    public bool TopProcessComplete
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>程式結束(程式結束會早於AutoMode_Stop)</summary>
    [PLCBit(BitType.M, 1209, LogType.StatusVariables)]
    public bool BottomProcessComplete
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>自動模式停止(需要手動按)</summary>
    [PLCBit(BitType.M, 52, LogType.StatusVariables)]
    public bool TopAutoMode_Stop
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>自動模式停止(需要手動按)</summary>
    [PLCBit(BitType.M, 1052, LogType.StatusVariables)]
    public bool BottomAutoMode_Stop
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
    [PLCData(DataType.D, 150, 0.1, LogType.StatusVariables)]
    public double PV_TopThermostatTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>溫控器實際溫度</summary>
    [PLCData(DataType.D, 1150, 0.1, LogType.StatusVariables)]
    public double PV_BottomThermostatTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    ///// <summary>溫控器設定溫度</summary>
    //[PLCData(DataType.D, 151, 0.1, LogType.StatusVariables)]
    //public double ThermostatTemperature
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    /// <summary>氮氣流量(L/m)</summary>
    [PLCData(DataType.D, 47, LogType.StatusVariables)]
    public double TopNitrogenFlow
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>氮氣流量(L/m)</summary>
    [PLCData(DataType.D, 1047, LogType.StatusVariables)]
    public double BottomNitrogenFlow
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>含氧量(%)</summary>
    [PLCData(DataType.D, 212, 0.1, LogType.StatusVariables)]
    public double OxygenContent
    {
        get => Get<double>();
        set => Set(value);
    }
    /// <summary>含氧量(%)</summary>
    [PLCData(DataType.D, 1212, 0.1, LogType.StatusVariables)]
    public double BottomOxygenContent
    {
        get => Get<double>();
        set => Set(value);
    }

    ///// <summary>現在風速(m/s)</summary>
    //[PLCData(DataType.D, 212, 0.1, LogType.StatusVariables)]
    //public double PV_WindSpeed
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>設定風速(m/s)</summary>
    //public double WindSpeed
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>耗電量(kWh)</summary>
    //[PLCData(DataType.D, 212, 0.1, LogType.StatusVariables)]
    //public double PowerConsumption
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>爐內溫度1</summary>
    //[PLCData(DataType.D, 700, 0.1, LogType.StatusVariables)]
    //public double OvenTemperature_1
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>爐內溫度2</summary>
    //[PLCData(DataType.D, 701, 0.1, LogType.StatusVariables)]
    //public double OvenTemperature_2
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>爐內溫度3</summary>
    //[PLCData(DataType.D, 702, 0.1, LogType.StatusVariables)]
    //public double OvenTemperature_3
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>爐內溫度4</summary>
    //[PLCData(DataType.D, 703, 0.1, LogType.StatusVariables)]
    //public double OvenTemperature_4
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>爐內溫度5</summary>
    //[PLCData(DataType.D, 704, 0.1, LogType.StatusVariables)]
    //public double OvenTemperature_5
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>爐內溫度6</summary>
    //[PLCData(DataType.D, 705, 0.1, LogType.StatusVariables)]
    //public double OvenTemperature_6
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>目前段數</summary>
    //[PLCData(DataType.D, 22, LogType.StatusVariables)]
    //public short CurrentSegment
    //{
    //    get => Get<short>();
    //    set => Set(value);
    //}

    /// <summary>RackID</summary>
    public string RackID
    {
        get => Get<string>();
        set => Set(value);
    }

    ///// <summary>設備狀態，0:停機(STOP)、1:自動(IDLE)、2:自動啟動(RUN)、3:異常(DOWN)、4:保養(PM))</summary>
    //[PLCData(DataType.D, 28, LogType.StatusVariables)]
    //public short EquipmentState
    //{
    //    get => Get<short>();
    //    set => Set(value);
    //}

    /// <summary>設備狀態，0:停機(STOP)、1:自動(IDLE)、2:自動啟動(RUN)、3:異常(DOWN)、4:保養(PM))</summary>
    [PLCData(DataType.D, 28, LogType.StatusVariables)]
    public short TopEquipmentState
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>設備狀態，0:停機(STOP)、1:自動(IDLE)、2:自動啟動(RUN)、3:異常(DOWN)、4:保養(PM))</summary>
    [PLCData(DataType.D, 28, LogType.StatusVariables)]
    public short BottomEquipmentState
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>生產狀態0:手動、1:昇溫中、2:恆溫中、7:冷卻降溫中、8:程式結束、9:自動、10:氮氣充氣中 </summary>
    [PLCData(DataType.D, 29, LogType.StatusVariables)]
    public short TopProcessState
    {
        get => Get<short>();
        set => Set(value);
    }
    /// <summary>生產狀態，0:手動、1:昇溫中、2:恆溫中、7:冷卻降溫中、8:程式結束、9:自動、10:氮氣充氣中 </summary>
    [PLCData(DataType.D, 29, LogType.StatusVariables)]
    public short BottomProcessState
    {
        get => Get<short>();
        set => Set(value);
    }

    public string EquipmentName
    {
        get => Get<string>();
        set => Set(value);
    }

    /// <summary>烘烤剩餘時間</summary>
    [PLCData(DataType.D, 153, 0.1, LogType.StatusVariables)]
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

    [PLCBit(BitType.M, 10, LogType.StatusVariables)]
    public bool TopDoorOpen
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 1010, LogType.StatusVariables)]
    public bool BottomDoorOpen
    {
        get => Get<bool>();
        set => Set(value);
    }


    #endregion
}