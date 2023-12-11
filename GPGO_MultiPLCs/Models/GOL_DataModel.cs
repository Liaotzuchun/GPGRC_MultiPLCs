using System.Collections.Generic;
using GPMVVM.Models;
using PLCService;

namespace GPGO_MultiPLCs.Models;

public class GOL_DataModel : PLCDataProvider
{
    public GOL_DataModel(IGate plcGate, int plcIndex, string plcTag, (Dictionary<BitType, int> bits_shift, Dictionary<DataType, int> datas_shift) shift = new()) : base(plcGate, plcIndex, plcTag, shift) { }

    #region PC=>PLC
    [PLCData(DataType.D, 120, LogType.None)]
    public short Check
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>門鎖開關</summary>
    [PLCBit(BitType.M, 350, LogType.None)]
    public bool DoorLock
    {
        get => Get<bool>();
        set => Set(value);
    }

    ///M60 ON 才可以下
    [PLCBit(BitType.M, 60, LogType.StatusVariables)]
    public bool RemoteStatus
    {
        get => Get<bool>();
        set => Set(value);
    }

    //M50 自動
    [PLCBit(BitType.M, 50, LogType.StatusVariables)]
    public bool AutoStatus
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

    [PLCData(DataType.D, 208, 10, LogType.None)]
    public string? TopLotID
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
    #endregion

    #region 配方實際值   
    /// <summary>第1段烘烤溫度</summary>
    [PLCData(DataType.D, 700, 20, LogType.None)]
    public double TopTemperatureSetpoint_1SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第2段烘烤溫度</summary>
    [PLCData(DataType.D, 701, 20, LogType.None)]
    public double TopTemperatureSetpoint_2SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第3段烘烤溫度</summary>
    [PLCData(DataType.D, 702, 20, LogType.None)]
    public double TopTemperatureSetpoint_3SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第4段烘烤溫度</summary>
    [PLCData(DataType.D, 703, 20, LogType.None)]
    public double TopTemperatureSetpoint_4SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第5段烘烤溫度</summary>
    [PLCData(DataType.D, 704, 20, LogType.None)]
    public double TopTemperatureSetpoint_5SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第6段烘烤溫度</summary>
    [PLCData(DataType.D, 705, 20, LogType.None)]
    public double TopTemperatureSetpoint_6SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間1</summary>
    [PLCData(DataType.D, 760, 0.1, LogType.None)]
    public double TopRampTime_1SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間2</summary>
    [PLCData(DataType.D, 761, 0.1, LogType.None)]
    public double TopRampTime_2SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間3</summary>
    [PLCData(DataType.D, 762, 0.1, LogType.None)]
    public double TopRampTime_3SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間4</summary>
    [PLCData(DataType.D, 763, 0.1, LogType.None)]
    public double TopRampTime_4SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間5</summary>
    [PLCData(DataType.D, 764, 0.1, LogType.None)]
    public double TopRampTime_5SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間6</summary>
    [PLCData(DataType.D, 765, 0.1, LogType.None)]
    public double TopRampTime_6SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間1</summary>
    [PLCData(DataType.D, 715, 0.1, LogType.None)]
    public double TopDwellTime_1SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間2</summary>
    [PLCData(DataType.D, 716, 0.1, LogType.None)]
    public double TopDwellTime_2SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間3</summary>
    [PLCData(DataType.D, 717, 0.1, LogType.None)]
    public double TopDwellTime_3SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間4</summary>
    [PLCData(DataType.D, 718, 0.1, LogType.None)]
    public double TopDwellTime_4SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間5</summary>
    [PLCData(DataType.D, 719, 0.1, LogType.None)]
    public double TopDwellTime_5SV
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間6</summary>
    [PLCData(DataType.D, 720, 0.1, LogType.None)]
    public double TopDwellTime_6SV
    {
        get => Get<double>();
        set => Set(value);
    }
    #endregion

    #region 配方設定值 上爐
    /// <summary>配方名</summary>
    [PLCData(DataType.D, 780, 16, LogType.RecipeSet)]
    public string RecipeName
    {
        get => Get<string>();
        set => Set(value);
    }

    /// <summary>降溫溫度</summary>
    [PLCData(DataType.D, 777, 1, LogType.RecipeSet)]
    public double CoolingTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>降溫時間</summary>
    [PLCData(DataType.D, 776, 0.1, LogType.RecipeSet)]
    public double CoolingTime
    {
        get => Get<double>();
        set => Set(value);
    }

    ///// <summary>氮氣模式</summary>
    //[PLCBitData(DataType.D, 789, 0, LogType.RecipeSet)]
    //public bool NitrogenMode
    //{
    //    get => Get<bool>();
    //    set => Set(value);
    //}

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
    [PLCData(DataType.D, 775, 1, LogType.RecipeSet)]
    public short SegmentCounts
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>目標溫度1</summary>
    [PLCData(DataType.D, 700, 1, LogType.RecipeSet)]
    public double TemperatureSetpoint_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度2</summary>
    [PLCData(DataType.D, 701, 1, LogType.RecipeSet)]
    public double TemperatureSetpoint_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度3</summary>
    [PLCData(DataType.D, 702, 1, LogType.RecipeSet)]
    public double TemperatureSetpoint_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度4</summary>
    [PLCData(DataType.D, 703, 1, LogType.RecipeSet)]
    public double TemperatureSetpoint_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度5</summary>
    [PLCData(DataType.D, 704, 1, LogType.RecipeSet)]
    public double TemperatureSetpoint_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>目標溫度6</summary>
    [PLCData(DataType.D, 705, 1, LogType.RecipeSet)]
    public double TemperatureSetpoint_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間1</summary>
    [PLCData(DataType.D, 760, 0.1, LogType.RecipeSet)]
    public double RampTime_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間2</summary>
    [PLCData(DataType.D, 761, 0.1, LogType.RecipeSet)]
    public double RampTime_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間3</summary>
    [PLCData(DataType.D, 762, 0.1, LogType.RecipeSet)]
    public double RampTime_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間4</summary>
    [PLCData(DataType.D, 763, 0.1, LogType.RecipeSet)]
    public double RampTime_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間5</summary>
    [PLCData(DataType.D, 764, 0.1, LogType.RecipeSet)]
    public double RampTime_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫時間6</summary>
    [PLCData(DataType.D, 765, 0.1, LogType.RecipeSet)]
    public double RampTime_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間1</summary>
    [PLCData(DataType.D, 730, 0.1, LogType.RecipeSet)]
    public double RampAlarm_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間2</summary>
    [PLCData(DataType.D, 731, 0.1, LogType.RecipeSet)]
    public double RampAlarm_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間3</summary>
    [PLCData(DataType.D, 732, 0.1, LogType.RecipeSet)]
    public double RampAlarm_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間4</summary>
    [PLCData(DataType.D, 733, 0.1, LogType.RecipeSet)]
    public double RampAlarm_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間5</summary>
    [PLCData(DataType.D, 734, 0.1, LogType.RecipeSet)]
    public double RampAlarm_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>升溫警報時間6</summary>
    [PLCData(DataType.D, 735, 0.1, LogType.RecipeSet)]
    public double RampAlarm_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間1</summary>
    [PLCData(DataType.D, 715, 0.1, LogType.RecipeSet)]
    public double DwellTime_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間2</summary>
    [PLCData(DataType.D, 716, 0.1, LogType.RecipeSet)]
    public double DwellTime_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間3</summary>
    [PLCData(DataType.D, 717, 0.1, LogType.RecipeSet)]
    public double DwellTime_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間4</summary>
    [PLCData(DataType.D, 718, 0.1, LogType.RecipeSet)]
    public double DwellTime_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間5</summary>
    [PLCData(DataType.D, 719, 0.1, LogType.RecipeSet)]
    public double DwellTime_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間6</summary>
    [PLCData(DataType.D, 720, 0.1, LogType.RecipeSet)]
    public double DwellTime_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間1</summary>
    [PLCData(DataType.D, 745, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間2</summary>
    [PLCData(DataType.D, 746, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間3</summary>
    [PLCData(DataType.D, 747, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間4</summary>
    [PLCData(DataType.D, 748, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間5</summary>
    [PLCData(DataType.D, 749, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫警報時間6</summary>
    [PLCData(DataType.D, 750, 0.1, LogType.RecipeSet)]
    public double DwellAlarm_6
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset1</summary>
    [PLCData(DataType.D, 793, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset2</summary>
    [PLCData(DataType.D, 794, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset3</summary>
    [PLCData(DataType.D, 795, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset4</summary>
    [PLCData(DataType.D, 796, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset5</summary>
    [PLCData(DataType.D, 797, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_5
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>恆溫時間Offset6</summary>
    [PLCData(DataType.D, 798, 0.1, LogType.RecipeSet)]
    public double DwellTimeOffset_6
    {
        get => Get<double>();
        set => Set(value);
    }
    #endregion

    #region 警報
    [PLCBit(BitType.M, 240, LogType.Alarm)] public bool M240 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 250, LogType.Alarm)] public bool M250 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 302, LogType.Alarm)] public bool M302 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 700, LogType.Alarm)] public bool M700 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 701, LogType.Alarm)] public bool M701 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 702, LogType.Alarm)] public bool M702 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 703, LogType.Alarm)] public bool M703 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 704, LogType.Alarm)] public bool M704 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 705, LogType.Alarm)] public bool M705 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 706, LogType.Alarm)] public bool M706 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 707, LogType.Alarm)] public bool M707 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 708, LogType.Alarm)] public bool M708 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 709, LogType.Alarm)] public bool M709 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 710, LogType.Alarm)] public bool M710 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 711, LogType.Alarm)] public bool M711 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 712, LogType.Alarm)] public bool M712 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 713, LogType.Alarm)] public bool M713 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 714, LogType.Alarm)] public bool M714 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 715, LogType.Alarm)] public bool M715 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 716, LogType.Alarm)] public bool M716 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 717, LogType.Alarm)] public bool M717 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 718, LogType.Alarm)] public bool M718 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 719, LogType.Alarm)] public bool M719 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 720, LogType.Alarm)] public bool M720 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 721, LogType.Alarm)] public bool M721 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 722, LogType.Alarm)] public bool M722 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 723, LogType.Alarm)] public bool M723 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 724, LogType.Alarm)] public bool M724 { get => Get<bool>(); set => Set(value); }
    #endregion

    #region 機台狀態
    /// <summary>程式結束警報時間</summary>
    [PLCData(DataType.D, 157, 0.1, LogType.EquipmentConstants)]
    public double ProgramEndWarningTime
    {
        get => Get<double>();
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
    [PLCBit(BitType.Y, 39, LogType.None)]
    public bool BlueLight
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
        set => Set(value);
    }

    /// <summary>程式結束(程式結束會早於AutoMode_Stop)</summary>
    [PLCBit(BitType.M, 209, LogType.StatusVariables)]
    public bool TopProcessComplete
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

    [PLCBit(BitType.M, 341, LogType.StatusVariables)]
    public bool RemoteMode
    {
        get => Get<bool>();
        set => Set(value);
    }

    /// <summary>溫控器實際溫度</summary>
    [PLCData(DataType.D, 64, 1, LogType.StatusVariables)]
    public double PV_TopThermostatTemperature
    {
        get => Get<double>();
        set => Set(value);
    }

    ///// <summary>氮氣流量(L/m)</summary>
    //[PLCData(DataType.D, 47, LogType.StatusVariables)]
    //public double TopNitrogenFlow
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>含氧量(%)</summary>
    //[PLCData(DataType.D, 213, 0.1, LogType.StatusVariables)]
    //public double OxygenContent
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    /// <summary>RackID</summary>
    public string RackID
    {
        get => Get<string>();
        set => Set(value);
    }

    /// <summary>設備狀態，0:停機(STOP)、1:自動(IDLE)、2:自動啟動(RUN)、3:異常(DOWN)、4:保養(PM))</summary>
    [PLCData(DataType.D, 28, LogType.StatusVariables)]
    public short TopEquipmentState
    {
        get => Get<short>();
        set => Set(value);
    }

    /// <summary>生產狀態0:手動、1:昇溫中、2:恆溫中、7:冷卻降溫中、8:程式結束、9:自動、10:氮氣充氣中 </summary>
    [PLCData(DataType.D, 29, LogType.StatusVariables)]
    public short ProcessState
    {
        get => Get<short>();
        set => Set(value);
    }
    /// <summary>狀態  0:未知、1:無框、2:空框、3:有料未烘烤、4:有料烘烤OK、5:有料烘烤NG、6:烘烤中  (有料:框+基板) </summary>
    [PLCData(DataType.D, 58, LogType.StatusVariables)]
    public short TopStatus
    {
        get => Get<short>();
        set => Set(value);
    }
    /// <summary>連線模式 </summary>
    [PLCData(DataType.D, 121, LogType.StatusVariables)]
    public short TopPCtoPLC
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



    #endregion
}