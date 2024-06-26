using System.Collections.Generic;
using GPMVVM.Models;
using PLCService;

namespace GPGRC_MultiPLCs.Models;

public class GRC_DataModel : PLCDataProvider
{
    public GRC_DataModel(IGate plcGate, int plcIndex, string plcTag, (Dictionary<BitType, int> bits_shift, Dictionary<DataType, int> datas_shift) shift = new()) : base(plcGate, plcIndex, plcTag, shift) { }

    #region PC=>PLC
    [PLCData(DataType.D, 4140, LogType.None)]
    public short Quantity
    {
        get => Get<short>();
        set => Set(value);
    }

    [PLCData(DataType.D, 120, LogType.None)]
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

    #endregion

    #region 配方實際值   
    /// <summary>第1段烘烤溫度</summary>
    [PLCData(DataType.D, 2400, 0.1, LogType.RecipeSet)]
    public double TemperaturePV1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第2段烘烤溫度</summary>
    [PLCData(DataType.D, 2401, 0.1, LogType.RecipeSet)]
    public double TemperaturePV2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第1段烘烤溫度</summary>
    [PLCData(DataType.D, 14120, 0.1, LogType.RecipeSet)]
    public double RC3_TemperaturePV1
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第2段烘烤溫度</summary>
    [PLCData(DataType.D, 14121, 0.1, LogType.RecipeSet)]
    public double RC3_TemperaturePV2
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第3段烘烤溫度</summary>
    [PLCData(DataType.D, 14122, 0.1, LogType.RecipeSet)]
    public double RC3_TemperaturePV3
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第4段烘烤溫度</summary>
    [PLCData(DataType.D, 14123, 0.1, LogType.RecipeSet)]
    public double RC3_TemperaturePV4
    {
        get => Get<double>();
        set => Set(value);
    }

    /// <summary>第5段烘烤溫度</summary>
    [PLCData(DataType.D, 14124, 0.1, LogType.RecipeSet)]
    public double RC3_TemperaturePV5
    {
        get => Get<double>();
        set => Set(value);
    }
    /// <summary>第6段烘烤溫度</summary>
    [PLCData(DataType.D, 14125, 0.1, LogType.RecipeSet)]
    public double RC3_TemperaturePV6
    {
        get => Get<double>();
        set => Set(value);
    }
    /// <summary>第7段烘烤溫度</summary>
    [PLCData(DataType.D, 14126, 0.1, LogType.RecipeSet)]
    public double RC3_TemperaturePV7
    {
        get => Get<double>();
        set => Set(value);
    }
    /// <summary>第8段烘烤溫度</summary>
    [PLCData(DataType.D, 14127, 0.1, LogType.RecipeSet)]
    public double RC3_TemperaturePV8
    {
        get => Get<double>();
        set => Set(value);
    }
    #endregion

    #region 配方設定值
    /// <summary>配方名</summary>
    [PLCData(DataType.D, 780, 16, LogType.RecipeSet)]
    public string RecipeName
    {
        get => Get<string>();
        set => Set(value);
    }
    #region 主控PLC
    //#region Coater1
    ///// <summary>塗佈次數</summary>
    //[PLCData(DataType.D, 700, LogType.RecipeSet)] public double RC1_Coatingoftimes { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈速度設定</summary>
    //[PLCData(DataType.D, 701, LogType.RecipeSet)] public double RC1_CoatingSpeedSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>板面夾持距離設定</summary>
    //[PLCData(DataType.D, 702, LogType.RecipeSet)] public double RC1_BoardClampingDistance { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔次數設定</summary>
    //[PLCData(DataType.D, 703, LogType.RecipeSet)] public double RC1_Plugoftimes { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈壓力設定</summary>
    //[PLCData(DataType.D, 704, LogType.RecipeSet)] public double RC1_CoatingPressureSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>基板厚度設定</summary>
    //[PLCData(DataType.D, 705, LogType.RecipeSet)] public double RC1_PanelThicknessSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>入料下降位置設定</summary>
    //[PLCData(DataType.D, 706, LogType.RecipeSet)] public double RC1_LocationOfDrop { get => Get<double>(); set => Set(value); }
    ///// <summary>左前D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 707, LogType.RecipeSet)] public double RC1_D_BarPressureSetting1 { get => Get<double>(); set => Set(value); }
    ///// <summary>右前D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 708, LogType.RecipeSet)] public double RC1_D_BarPressureSetting2 { get => Get<double>(); set => Set(value); }
    ///// <summary>左後D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 709, LogType.RecipeSet)] public double RC1_D_BarPressureSetting3 { get => Get<double>(); set => Set(value); }
    ///// <summary>右後D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 710, LogType.RecipeSet)] public double RC1_D_BarPressureSetting4 { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔刮刀壓力設定</summary>
    //[PLCData(DataType.D, 711, LogType.RecipeSet)] public double RC1_Blade_Pressure { get => Get<double>(); set => Set(value); }
    ///// <summary>烘烤時間設定</summary>
    //[PLCData(DataType.D, 712, LogType.RecipeSet)] public double RC1_BakingTimeSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>第1段溫度設定值</summary>
    //[PLCData(DataType.D, 713, LogType.RecipeSet)] public double RC1_TemperatureSV1 { get => Get<double>(); set => Set(value); }
    ///// <summary>第2段溫度設定值</summary>
    //[PLCData(DataType.D, 714, LogType.RecipeSet)] public double RC1_TemperatureSV2 { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈使用</summary>
    //[PLCData(DataType.D, 715, LogType.RecipeSet)] public double RC1_UseCoating { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔使用</summary>
    //[PLCData(DataType.D, 716, LogType.RecipeSet)] public double RC1_UsePlug { get => Get<double>(); set => Set(value); }
    ///// <summary>標準墨重</summary>
    //[PLCData(DataType.D, 717, LogType.RecipeSet)] public double RC1_StandardInk { get => Get<double>(); set => Set(value); }
    ///// <summary>墨重誤差值</summary>
    //[PLCData(DataType.D, 718, LogType.RecipeSet)] public double RC1_DifferenceOfInk { get => Get<double>(); set => Set(value); }
    //#endregion
    //#region Coater2
    ///// <summary>塗佈次數</summary>
    //[PLCData(DataType.D, 719, LogType.RecipeSet)] public double RC2_Coatingoftimes { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈速度設定</summary>
    //[PLCData(DataType.D, 720, LogType.RecipeSet)] public double RC2_CoatingSpeedSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>板面夾持距離設定</summary>
    //[PLCData(DataType.D, 721, LogType.RecipeSet)] public double RC2_BoardClampingDistance { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔次數設定</summary>
    //[PLCData(DataType.D, 722, LogType.RecipeSet)] public double RC2_Plugoftimes { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈壓力設定</summary>
    //[PLCData(DataType.D, 723, LogType.RecipeSet)] public double RC2_CoatingPressureSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>基板厚度設定</summary>
    //[PLCData(DataType.D, 724, LogType.RecipeSet)] public double RC2_PanelThicknessSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>入料下降位置設定</summary>
    //[PLCData(DataType.D, 725, LogType.RecipeSet)] public double RC2_LocationOfDrop { get => Get<double>(); set => Set(value); }
    ///// <summary>左前D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 726, LogType.RecipeSet)] public double RC2_D_BarPressureSetting1 { get => Get<double>(); set => Set(value); }
    ///// <summary>右前D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 727, LogType.RecipeSet)] public double RC2_D_BarPressureSetting2 { get => Get<double>(); set => Set(value); }
    ///// <summary>左後D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 728, LogType.RecipeSet)] public double RC2_D_BarPressureSetting3 { get => Get<double>(); set => Set(value); }
    ///// <summary>右後D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 729, LogType.RecipeSet)] public double RC2_D_BarPressureSetting4 { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔刮刀壓力設定</summary>
    //[PLCData(DataType.D, 730, LogType.RecipeSet)] public double RC2_Blade_Pressure { get => Get<double>(); set => Set(value); }
    ///// <summary>烘烤時間設定</summary>
    //[PLCData(DataType.D, 731, LogType.RecipeSet)] public double RC2_BakingTimeSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>第1段溫度設定值</summary>
    //[PLCData(DataType.D, 732, LogType.RecipeSet)] public double RC2_TemperatureSV1 { get => Get<double>(); set => Set(value); }
    ///// <summary>第2段溫度設定值</summary>
    //[PLCData(DataType.D, 733, LogType.RecipeSet)] public double RC2_TemperatureSV2 { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈使用</summary>
    //[PLCData(DataType.D, 734, LogType.RecipeSet)] public double RC2_UseCoating { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔使用</summary>
    //[PLCData(DataType.D, 735, LogType.RecipeSet)] public double RC2_UsePlug { get => Get<double>(); set => Set(value); }
    ///// <summary>標準墨重</summary>
    //[PLCData(DataType.D, 736, LogType.RecipeSet)] public double RC2_StandardInk { get => Get<double>(); set => Set(value); }
    ///// <summary>墨重誤差值</summary>
    //[PLCData(DataType.D, 737, LogType.RecipeSet)] public double RC2_DifferenceOfInk { get => Get<double>(); set => Set(value); }
    //#endregion
    //#region Coater3
    ///// <summary>塗佈次數</summary>
    //[PLCData(DataType.D, 738, LogType.RecipeSet)] public double RC3_Coatingoftimes { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈速度設定</summary>
    //[PLCData(DataType.D, 739, LogType.RecipeSet)] public double RC3_CoatingSpeedSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>板面夾持距離設定</summary>
    //[PLCData(DataType.D, 740, LogType.RecipeSet)] public double RC3_BoardClampingDistance { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔次數設定</summary>
    //[PLCData(DataType.D, 741, LogType.RecipeSet)] public double RC3_Plugoftimes { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈壓力設定</summary>
    //[PLCData(DataType.D, 742, LogType.RecipeSet)] public double RC3_CoatingPressureSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>基板厚度設定</summary>
    //[PLCData(DataType.D, 743, LogType.RecipeSet)] public double RC3_PanelThicknessSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>入料下降位置設定</summary>
    //[PLCData(DataType.D, 744, LogType.RecipeSet)] public double RC3_LocationOfDrop { get => Get<double>(); set => Set(value); }
    ///// <summary>左前D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 745, LogType.RecipeSet)] public double RC3_D_BarPressureSetting1 { get => Get<double>(); set => Set(value); }
    ///// <summary>右前D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 746, LogType.RecipeSet)] public double RC3_D_BarPressureSetting2 { get => Get<double>(); set => Set(value); }
    ///// <summary>左後D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 747, LogType.RecipeSet)] public double RC3_D_BarPressureSetting3 { get => Get<double>(); set => Set(value); }
    ///// <summary>右後D.BAR壓力設定</summary>
    //[PLCData(DataType.D, 748, LogType.RecipeSet)] public double RC3_D_BarPressureSetting4 { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔刮刀壓力設定</summary>
    //[PLCData(DataType.D, 749, LogType.RecipeSet)] public double RC3_Blade_Pressure { get => Get<double>(); set => Set(value); }
    ///// <summary>烘烤時間設定</summary>
    //[PLCData(DataType.D, 750, LogType.RecipeSet)] public double RC3_BakingTimeSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>第1段溫度設定值</summary>
    //[PLCData(DataType.D, 751, LogType.RecipeSet)] public double RC3_TemperatureSV1 { get => Get<double>(); set => Set(value); }
    ///// <summary>第2段溫度設定值</summary>
    //[PLCData(DataType.D, 752, LogType.RecipeSet)] public double RC3_TemperatureSV2 { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈使用</summary>
    //[PLCData(DataType.D, 753, LogType.RecipeSet)] public double RC3_UseCoating { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔使用</summary>
    //[PLCData(DataType.D, 754, LogType.RecipeSet)] public double RC3_UsePlug { get => Get<double>(); set => Set(value); }
    ///// <summary>標準墨重</summary>
    //[PLCData(DataType.D, 755, LogType.RecipeSet)] public double RC3_StandardInk { get => Get<double>(); set => Set(value); }
    ///// <summary>墨重誤差值</summary>
    //[PLCData(DataType.D, 756, LogType.RecipeSet)] public double RC3_DifferenceOfInk { get => Get<double>(); set => Set(value); }
    //#endregion
    #endregion
    #region 無主控
    /// <summary>塗佈次數</summary>
    [PLCData(DataType.D, 220, LogType.RecipeSet)] public double Coatingoftimes { get => Get<double>(); set => Set(value); }
    /// <summary>塗佈速度設定</summary>
    [PLCData(DataType.D, 221, LogType.RecipeSet)] public double CoatingSpeedSetting { get => Get<double>(); set => Set(value); }
    /// <summary>板面夾持距離設定</summary>
    [PLCData(DataType.D, 223, LogType.RecipeSet)] public double BoardClampingDistance { get => Get<double>(); set => Set(value); }
    /// <summary>塞孔次數設定</summary>
    [PLCData(DataType.D, 224, LogType.RecipeSet)] public double Plugoftimes { get => Get<double>(); set => Set(value); }
    /// <summary>塗佈壓力設定</summary>
    [PLCData(DataType.D, 225, LogType.RecipeSet)] public double CoatingPressureSetting { get => Get<double>(); set => Set(value); }
    /// <summary>拉料速度比</summary>
    [PLCData(DataType.D, 226, LogType.RecipeSet)] public double CoatingPullSpeed { get => Get<double>(); set => Set(value); }
    /// <summary>基板厚度設定</summary>
    [PLCData(DataType.D, 227, LogType.RecipeSet)] public double PanelThicknessSetting { get => Get<double>(); set => Set(value); }
    /// <summary>入料下降位置設定</summary>
    [PLCData(DataType.D, 228, LogType.RecipeSet)] public double LocationOfDrop { get => Get<double>(); set => Set(value); }
    /// <summary>入料滾輪延遲時間設定</summary>
    [PLCData(DataType.D, 229, LogType.RecipeSet)] public double Feedingrollerdelaytimesetting { get => Get<double>(); set => Set(value); }
    /// <summary>入料下降速度設定</summary>
    [PLCData(DataType.D, 230, LogType.RecipeSet)] public double SpeedOfDrop { get => Get<double>(); set => Set(value); }
    /// <summary>基板寬度設定</summary>
    [PLCData(DataType.D, 233, LogType.RecipeSet)] public double PanelWidthSetting { get => Get<double>(); set => Set(value); }
    /// <summary>刮刀壓力設定1</summary>
    [PLCData(DataType.D, 100, 0.01, LogType.RecipeSet)] public double Blade_Pressure1 { get => Get<double>(); set => Set(value); }
    /// <summary>刮刀壓力設定2</summary>
    [PLCData(DataType.D, 101, 0.01, LogType.RecipeSet)] public double Blade_Pressure2 { get => Get<double>(); set => Set(value); }
    /// <summary>刮刀壓力設定3</summary>
    [PLCData(DataType.D, 102, 0.01, LogType.RecipeSet)] public double Blade_Pressure3 { get => Get<double>(); set => Set(value); }
    /// <summary>刮刀壓力設定4</summary>
    [PLCData(DataType.D, 103, 0.01, LogType.RecipeSet)] public double Blade_Pressure4 { get => Get<double>(); set => Set(value); }
    /// <summary>烘烤時間設定</summary>
    [PLCData(DataType.D, 2026, LogType.RecipeSet)] public double BakingTimeSetting { get => Get<double>(); set => Set(value); }
    /// <summary>第1段溫度設定值</summary>
    [PLCData(DataType.D, 2450, 0.1, LogType.RecipeSet)] public double TemperatureSV1 { get => Get<double>(); set => Set(value); }
    /// <summary>第2段溫度設定值</summary>
    [PLCData(DataType.D, 2451, 0.1, LogType.RecipeSet)] public double TemperatureSV2 { get => Get<double>(); set => Set(value); }
    /// <summary>第1段溫度設定值</summary>
    [PLCData(DataType.D, 14010, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV1 { get => Get<double>(); set => Set(value); }
    /// <summary>第2段溫度設定值</summary>
    [PLCData(DataType.D, 14011, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV2 { get => Get<double>(); set => Set(value); }
    /// <summary>第3段溫度設定值</summary>
    [PLCData(DataType.D, 14012, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV3 { get => Get<double>(); set => Set(value); }
    /// <summary>第4段溫度設定值</summary>
    [PLCData(DataType.D, 14013, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV4 { get => Get<double>(); set => Set(value); }
    /// <summary>第5段溫度設定值</summary>
    [PLCData(DataType.D, 14014, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV5 { get => Get<double>(); set => Set(value); }
    /// <summary>第6段溫度設定值</summary>
    [PLCData(DataType.D, 14015, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV6 { get => Get<double>(); set => Set(value); }
    /// <summary>第7段溫度設定值</summary>
    [PLCData(DataType.D, 14016, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV7 { get => Get<double>(); set => Set(value); }
    /// <summary>第8段溫度設定值</summary>
    [PLCData(DataType.D, 14017, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV8 { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈使用</summary>
    //[PLCBitData(DataType.D, 721, 0, LogType.RecipeSet)] public bool UseCoating { get => Get<bool>(); set => Set(value); }
    ///// <summary>塞孔使用</summary>
    //[PLCBitData(DataType.D, 722, 0, LogType.RecipeSet)] public bool UsePlug { get => Get<bool>(); set => Set(value); }
    ///// <summary>標準墨重</summary>
    //[PLCData(DataType.D, 723, LogType.RecipeSet)] public double StandardInk { get => Get<double>(); set => Set(value); }
    ///// <summary>墨重誤差值</summary>
    //[PLCData(DataType.D, 724, LogType.RecipeSet)] public double DifferenceOfInk { get => Get<double>(); set => Set(value); }
    #endregion
    #endregion

    #region 警報

    #region Coater1跟2一樣
    [PLCBit(BitType.F, 0, LogType.Alarm)] public bool F0 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 1, LogType.Alarm)] public bool F1 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 2, LogType.Alarm)] public bool F2 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 3, LogType.Alarm)] public bool F3 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 4, LogType.Alarm)] public bool F4 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 5, LogType.Alarm)] public bool F5 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 6, LogType.Alarm)] public bool F6 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 7, LogType.Alarm)] public bool F7 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 8, LogType.Alarm)] public bool F8 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 9, LogType.Alarm)] public bool F9 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 10, LogType.Alarm)] public bool F10 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 11, LogType.Alarm)] public bool F11 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 12, LogType.Alarm)] public bool F12 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 13, LogType.Alarm)] public bool F13 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 14, LogType.Alarm)] public bool F14 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 15, LogType.Alarm)] public bool F15 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 16, LogType.Alarm)] public bool F16 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 17, LogType.Alarm)] public bool F17 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 18, LogType.Alarm)] public bool F18 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 19, LogType.Alarm)] public bool F19 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 20, LogType.Alarm)] public bool F20 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 21, LogType.Alarm)] public bool F21 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 22, LogType.Alarm)] public bool F22 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 23, LogType.Alarm)] public bool F23 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 24, LogType.Alarm)] public bool F24 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 25, LogType.Alarm)] public bool F25 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 26, LogType.Alarm)] public bool F26 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 27, LogType.Alarm)] public bool F27 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 28, LogType.Alarm)] public bool F28 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 29, LogType.Alarm)] public bool F29 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 30, LogType.Alarm)] public bool F30 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 31, LogType.Alarm)] public bool F31 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 32, LogType.Alarm)] public bool F32 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 33, LogType.Alarm)] public bool F33 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 34, LogType.Alarm)] public bool F34 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 35, LogType.Alarm)] public bool F35 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 36, LogType.Alarm)] public bool F36 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 37, LogType.Alarm)] public bool F37 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 38, LogType.Alarm)] public bool F38 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 39, LogType.Alarm)] public bool F39 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 40, LogType.Alarm)] public bool F40 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 41, LogType.Alarm)] public bool F41 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 42, LogType.Alarm)] public bool F42 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 43, LogType.Alarm)] public bool F43 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 44, LogType.Alarm)] public bool F44 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 45, LogType.Alarm)] public bool F45 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 46, LogType.Alarm)] public bool F46 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 47, LogType.Alarm)] public bool F47 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 48, LogType.Alarm)] public bool F48 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 49, LogType.Alarm)] public bool F49 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 50, LogType.Alarm)] public bool F50 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 51, LogType.Alarm)] public bool F51 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 52, LogType.Alarm)] public bool F52 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 53, LogType.Alarm)] public bool F53 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 54, LogType.Alarm)] public bool F54 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 55, LogType.Alarm)] public bool F55 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 56, LogType.Alarm)] public bool F56 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 57, LogType.Alarm)] public bool F57 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 58, LogType.Alarm)] public bool F58 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 59, LogType.Alarm)] public bool F59 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 60, LogType.Alarm)] public bool F60 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 61, LogType.Alarm)] public bool F61 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 62, LogType.Alarm)] public bool F62 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 63, LogType.Alarm)] public bool F63 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 64, LogType.Alarm)] public bool F64 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 65, LogType.Alarm)] public bool F65 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 66, LogType.Alarm)] public bool F66 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 67, LogType.Alarm)] public bool F67 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 68, LogType.Alarm)] public bool F68 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 69, LogType.Alarm)] public bool F69 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 70, LogType.Alarm)] public bool F70 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 71, LogType.Alarm)] public bool F71 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 72, LogType.Alarm)] public bool F72 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 73, LogType.Alarm)] public bool F73 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 74, LogType.Alarm)] public bool F74 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 75, LogType.Alarm)] public bool F75 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 76, LogType.Alarm)] public bool F76 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 77, LogType.Alarm)] public bool F77 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 78, LogType.Alarm)] public bool F78 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 79, LogType.Alarm)] public bool F79 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 80, LogType.Alarm)] public bool F80 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 81, LogType.Alarm)] public bool F81 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 82, LogType.Alarm)] public bool F82 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 83, LogType.Alarm)] public bool F83 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 84, LogType.Alarm)] public bool F84 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 85, LogType.Alarm)] public bool F85 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 86, LogType.Alarm)] public bool F86 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 87, LogType.Alarm)] public bool F87 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 88, LogType.Alarm)] public bool F88 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 89, LogType.Alarm)] public bool F89 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 90, LogType.Alarm)] public bool F90 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 91, LogType.Alarm)] public bool F91 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 92, LogType.Alarm)] public bool F92 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 93, LogType.Alarm)] public bool F93 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 94, LogType.Alarm)] public bool F94 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 95, LogType.Alarm)] public bool F95 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 96, LogType.Alarm)] public bool F96 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 97, LogType.Alarm)] public bool F97 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 98, LogType.Alarm)] public bool F98 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 99, LogType.Alarm)] public bool F99 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 100, LogType.Alarm)] public bool F100 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 101, LogType.Alarm)] public bool F101 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 102, LogType.Alarm)] public bool F102 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 103, LogType.Alarm)] public bool F103 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 104, LogType.Alarm)] public bool F104 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 105, LogType.Alarm)] public bool F105 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 106, LogType.Alarm)] public bool F106 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 107, LogType.Alarm)] public bool F107 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 108, LogType.Alarm)] public bool F108 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 109, LogType.Alarm)] public bool F109 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 110, LogType.Alarm)] public bool F110 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 111, LogType.Alarm)] public bool F111 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 112, LogType.Alarm)] public bool F112 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 113, LogType.Alarm)] public bool F113 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 114, LogType.Alarm)] public bool F114 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 115, LogType.Alarm)] public bool F115 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 116, LogType.Alarm)] public bool F116 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 117, LogType.Alarm)] public bool F117 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 118, LogType.Alarm)] public bool F118 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 119, LogType.Alarm)] public bool F119 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 120, LogType.Alarm)] public bool F120 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 121, LogType.Alarm)] public bool F121 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 122, LogType.Alarm)] public bool F122 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 123, LogType.Alarm)] public bool F123 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 124, LogType.Alarm)] public bool F124 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 125, LogType.Alarm)] public bool F125 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 126, LogType.Alarm)] public bool F126 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 128, LogType.Alarm)] public bool F128 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 129, LogType.Alarm)] public bool F129 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 131, LogType.Alarm)] public bool F131 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 132, LogType.Alarm)] public bool F132 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 133, LogType.Alarm)] public bool F133 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 134, LogType.Alarm)] public bool F134 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 135, LogType.Alarm)] public bool F135 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 136, LogType.Alarm)] public bool F136 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 137, LogType.Alarm)] public bool F137 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 138, LogType.Alarm)] public bool F138 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 139, LogType.Alarm)] public bool F139 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 140, LogType.Alarm)] public bool F140 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 141, LogType.Alarm)] public bool F141 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 142, LogType.Alarm)] public bool F142 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 66, LogType.Alarm)] public bool M66 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 67, LogType.Alarm)] public bool M67 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 68, LogType.Alarm)] public bool M68 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 3671, LogType.Alarm)] public bool M3671 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2501, LogType.Alarm)] public bool M2501 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2502, LogType.Alarm)] public bool M2502 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2503, LogType.Alarm)] public bool M2503 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2504, LogType.Alarm)] public bool M2504 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2505, LogType.Alarm)] public bool M2505 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2506, LogType.Alarm)] public bool M2506 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2507, LogType.Alarm)] public bool M2507 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2508, LogType.Alarm)] public bool M2508 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2509, LogType.Alarm)] public bool M2509 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2510, LogType.Alarm)] public bool M2510 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2511, LogType.Alarm)] public bool M2511 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2512, LogType.Alarm)] public bool M2512 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2513, LogType.Alarm)] public bool M2513 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2514, LogType.Alarm)] public bool M2514 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2515, LogType.Alarm)] public bool M2515 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2516, LogType.Alarm)] public bool M2516 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2517, LogType.Alarm)] public bool M2517 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2518, LogType.Alarm)] public bool M2518 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2519, LogType.Alarm)] public bool M2519 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2520, LogType.Alarm)] public bool M2520 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2521, LogType.Alarm)] public bool M2521 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2522, LogType.Alarm)] public bool M2522 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2523, LogType.Alarm)] public bool M2523 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2524, LogType.Alarm)] public bool M2524 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2525, LogType.Alarm)] public bool M2525 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2526, LogType.Alarm)] public bool M2526 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2527, LogType.Alarm)] public bool M2527 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2528, LogType.Alarm)] public bool M2528 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2529, LogType.Alarm)] public bool M2529 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2530, LogType.Alarm)] public bool M2530 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2531, LogType.Alarm)] public bool M2531 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2532, LogType.Alarm)] public bool M2532 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2533, LogType.Alarm)] public bool M2533 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2534, LogType.Alarm)] public bool M2534 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2535, LogType.Alarm)] public bool M2535 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2536, LogType.Alarm)] public bool M2536 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2537, LogType.Alarm)] public bool M2537 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2538, LogType.Alarm)] public bool M2538 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2539, LogType.Alarm)] public bool M2539 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2540, LogType.Alarm)] public bool M2540 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2541, LogType.Alarm)] public bool M2541 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2542, LogType.Alarm)] public bool M2542 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2543, LogType.Alarm)] public bool M2543 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2544, LogType.Alarm)] public bool M2544 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2545, LogType.Alarm)] public bool M2545 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2546, LogType.Alarm)] public bool M2546 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2547, LogType.Alarm)] public bool M2547 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2548, LogType.Alarm)] public bool M2548 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2549, LogType.Alarm)] public bool M2549 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2550, LogType.Alarm)] public bool M2550 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2551, LogType.Alarm)] public bool M2551 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2552, LogType.Alarm)] public bool M2552 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2553, LogType.Alarm)] public bool M2553 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2554, LogType.Alarm)] public bool M2554 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2555, LogType.Alarm)] public bool M2555 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2556, LogType.Alarm)] public bool M2556 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2559, LogType.Alarm)] public bool M2559 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2560, LogType.Alarm)] public bool M2560 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2561, LogType.Alarm)] public bool M2561 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2562, LogType.Alarm)] public bool M2562 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2563, LogType.Alarm)] public bool M2563 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2564, LogType.Alarm)] public bool M2564 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2565, LogType.Alarm)] public bool M2565 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2566, LogType.Alarm)] public bool M2566 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2567, LogType.Alarm)] public bool M2567 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2568, LogType.Alarm)] public bool M2568 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2570, LogType.Alarm)] public bool M2570 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2571, LogType.Alarm)] public bool M2571 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2572, LogType.Alarm)] public bool M2572 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2573, LogType.Alarm)] public bool M2573 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2574, LogType.Alarm)] public bool M2574 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2575, LogType.Alarm)] public bool M2575 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2580, LogType.Alarm)] public bool M2580 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2581, LogType.Alarm)] public bool M2581 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2582, LogType.Alarm)] public bool M2582 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2583, LogType.Alarm)] public bool M2583 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2584, LogType.Alarm)] public bool M2584 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2585, LogType.Alarm)] public bool M2585 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2590, LogType.Alarm)] public bool M2590 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2591, LogType.Alarm)] public bool M2591 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2592, LogType.Alarm)] public bool M2592 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2593, LogType.Alarm)] public bool M2593 { get => Get<bool>(); set => Set(value); }
    #endregion

    #region Coater3
    [PLCBit(BitType.F, 0, LogType.Alarm)] public bool RC3_F0 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 1, LogType.Alarm)] public bool RC3_F1 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 2, LogType.Alarm)] public bool RC3_F2 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 3, LogType.Alarm)] public bool RC3_F3 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 4, LogType.Alarm)] public bool RC3_F4 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 5, LogType.Alarm)] public bool RC3_F5 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 6, LogType.Alarm)] public bool RC3_F6 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 7, LogType.Alarm)] public bool RC3_F7 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 8, LogType.Alarm)] public bool RC3_F8 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 9, LogType.Alarm)] public bool RC3_F9 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 10, LogType.Alarm)] public bool RC3_F10 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 11, LogType.Alarm)] public bool RC3_F11 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 12, LogType.Alarm)] public bool RC3_F12 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 13, LogType.Alarm)] public bool RC3_F13 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 14, LogType.Alarm)] public bool RC3_F14 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 15, LogType.Alarm)] public bool RC3_F15 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 16, LogType.Alarm)] public bool RC3_F16 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 17, LogType.Alarm)] public bool RC3_F17 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 18, LogType.Alarm)] public bool RC3_F18 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 19, LogType.Alarm)] public bool RC3_F19 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 20, LogType.Alarm)] public bool RC3_F20 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 21, LogType.Alarm)] public bool RC3_F21 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 22, LogType.Alarm)] public bool RC3_F22 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 23, LogType.Alarm)] public bool RC3_F23 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 24, LogType.Alarm)] public bool RC3_F24 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 25, LogType.Alarm)] public bool RC3_F25 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 26, LogType.Alarm)] public bool RC3_F26 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 27, LogType.Alarm)] public bool RC3_F27 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 28, LogType.Alarm)] public bool RC3_F28 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 29, LogType.Alarm)] public bool RC3_F29 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 30, LogType.Alarm)] public bool RC3_F30 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 31, LogType.Alarm)] public bool RC3_F31 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 32, LogType.Alarm)] public bool RC3_F32 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 33, LogType.Alarm)] public bool RC3_F33 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 34, LogType.Alarm)] public bool RC3_F34 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 35, LogType.Alarm)] public bool RC3_F35 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 36, LogType.Alarm)] public bool RC3_F36 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 37, LogType.Alarm)] public bool RC3_F37 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 38, LogType.Alarm)] public bool RC3_F38 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 39, LogType.Alarm)] public bool RC3_F39 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 40, LogType.Alarm)] public bool RC3_F40 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 41, LogType.Alarm)] public bool RC3_F41 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 42, LogType.Alarm)] public bool RC3_F42 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 43, LogType.Alarm)] public bool RC3_F43 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 44, LogType.Alarm)] public bool RC3_F44 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 45, LogType.Alarm)] public bool RC3_F45 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 46, LogType.Alarm)] public bool RC3_F46 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 47, LogType.Alarm)] public bool RC3_F47 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 48, LogType.Alarm)] public bool RC3_F48 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 49, LogType.Alarm)] public bool RC3_F49 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 50, LogType.Alarm)] public bool RC3_F50 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 51, LogType.Alarm)] public bool RC3_F51 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 52, LogType.Alarm)] public bool RC3_F52 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 53, LogType.Alarm)] public bool RC3_F53 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 54, LogType.Alarm)] public bool RC3_F54 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 55, LogType.Alarm)] public bool RC3_F55 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 56, LogType.Alarm)] public bool RC3_F56 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 57, LogType.Alarm)] public bool RC3_F57 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 58, LogType.Alarm)] public bool RC3_F58 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 59, LogType.Alarm)] public bool RC3_F59 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 60, LogType.Alarm)] public bool RC3_F60 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 61, LogType.Alarm)] public bool RC3_F61 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 62, LogType.Alarm)] public bool RC3_F62 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 63, LogType.Alarm)] public bool RC3_F63 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 64, LogType.Alarm)] public bool RC3_F64 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 65, LogType.Alarm)] public bool RC3_F65 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 66, LogType.Alarm)] public bool RC3_F66 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 67, LogType.Alarm)] public bool RC3_F67 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 68, LogType.Alarm)] public bool RC3_F68 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 69, LogType.Alarm)] public bool RC3_F69 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 70, LogType.Alarm)] public bool RC3_F70 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 71, LogType.Alarm)] public bool RC3_F71 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 72, LogType.Alarm)] public bool RC3_F72 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 73, LogType.Alarm)] public bool RC3_F73 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 74, LogType.Alarm)] public bool RC3_F74 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 75, LogType.Alarm)] public bool RC3_F75 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 76, LogType.Alarm)] public bool RC3_F76 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 77, LogType.Alarm)] public bool RC3_F77 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 78, LogType.Alarm)] public bool RC3_F78 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 79, LogType.Alarm)] public bool RC3_F79 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 80, LogType.Alarm)] public bool RC3_F80 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 81, LogType.Alarm)] public bool RC3_F81 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 82, LogType.Alarm)] public bool RC3_F82 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 83, LogType.Alarm)] public bool RC3_F83 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 84, LogType.Alarm)] public bool RC3_F84 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 85, LogType.Alarm)] public bool RC3_F85 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 86, LogType.Alarm)] public bool RC3_F86 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 87, LogType.Alarm)] public bool RC3_F87 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 88, LogType.Alarm)] public bool RC3_F88 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 89, LogType.Alarm)] public bool RC3_F89 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 90, LogType.Alarm)] public bool RC3_F90 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 91, LogType.Alarm)] public bool RC3_F91 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 92, LogType.Alarm)] public bool RC3_F92 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 93, LogType.Alarm)] public bool RC3_F93 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 94, LogType.Alarm)] public bool RC3_F94 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 95, LogType.Alarm)] public bool RC3_F95 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 96, LogType.Alarm)] public bool RC3_F96 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 97, LogType.Alarm)] public bool RC3_F97 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 98, LogType.Alarm)] public bool RC3_F98 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 99, LogType.Alarm)] public bool RC3_F99 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 100, LogType.Alarm)] public bool RC3_F100 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 101, LogType.Alarm)] public bool RC3_F101 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 102, LogType.Alarm)] public bool RC3_F102 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 103, LogType.Alarm)] public bool RC3_F103 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 104, LogType.Alarm)] public bool RC3_F104 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 105, LogType.Alarm)] public bool RC3_F105 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 106, LogType.Alarm)] public bool RC3_F106 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 107, LogType.Alarm)] public bool RC3_F107 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 108, LogType.Alarm)] public bool RC3_F108 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 109, LogType.Alarm)] public bool RC3_F109 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 110, LogType.Alarm)] public bool RC3_F110 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 111, LogType.Alarm)] public bool RC3_F111 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 112, LogType.Alarm)] public bool RC3_F112 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 113, LogType.Alarm)] public bool RC3_F113 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 114, LogType.Alarm)] public bool RC3_F114 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 115, LogType.Alarm)] public bool RC3_F115 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 116, LogType.Alarm)] public bool RC3_F116 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 117, LogType.Alarm)] public bool RC3_F117 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 118, LogType.Alarm)] public bool RC3_F118 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 119, LogType.Alarm)] public bool RC3_F119 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 120, LogType.Alarm)] public bool RC3_F120 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 121, LogType.Alarm)] public bool RC3_F121 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 122, LogType.Alarm)] public bool RC3_F122 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 123, LogType.Alarm)] public bool RC3_F123 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 124, LogType.Alarm)] public bool RC3_F124 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 125, LogType.Alarm)] public bool RC3_F125 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 126, LogType.Alarm)] public bool RC3_F126 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 128, LogType.Alarm)] public bool RC3_F128 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 129, LogType.Alarm)] public bool RC3_F129 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 130, LogType.Alarm)] public bool RC3_F130 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 131, LogType.Alarm)] public bool RC3_F131 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 132, LogType.Alarm)] public bool RC3_F132 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 133, LogType.Alarm)] public bool RC3_F133 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 134, LogType.Alarm)] public bool RC3_F134 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 135, LogType.Alarm)] public bool RC3_F135 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 136, LogType.Alarm)] public bool RC3_F136 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 137, LogType.Alarm)] public bool RC3_F137 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 138, LogType.Alarm)] public bool RC3_F138 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 139, LogType.Alarm)] public bool RC3_F139 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 140, LogType.Alarm)] public bool RC3_F140 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 141, LogType.Alarm)] public bool RC3_F141 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 142, LogType.Alarm)] public bool RC3_F142 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 149, LogType.Alarm)] public bool RC3_F149 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 150, LogType.Alarm)] public bool RC3_F150 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 151, LogType.Alarm)] public bool RC3_F151 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 152, LogType.Alarm)] public bool RC3_F152 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2500, LogType.Alarm)] public bool RC3_M2500 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2501, LogType.Alarm)] public bool RC3_M2501 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2502, LogType.Alarm)] public bool RC3_M2502 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2503, LogType.Alarm)] public bool RC3_M2503 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2504, LogType.Alarm)] public bool RC3_M2504 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2505, LogType.Alarm)] public bool RC3_M2505 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2506, LogType.Alarm)] public bool RC3_M2506 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2507, LogType.Alarm)] public bool RC3_M2507 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2508, LogType.Alarm)] public bool RC3_M2508 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2509, LogType.Alarm)] public bool RC3_M2509 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2510, LogType.Alarm)] public bool RC3_M2510 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2511, LogType.Alarm)] public bool RC3_M2511 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2512, LogType.Alarm)] public bool RC3_M2512 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2513, LogType.Alarm)] public bool RC3_M2513 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2514, LogType.Alarm)] public bool RC3_M2514 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2515, LogType.Alarm)] public bool RC3_M2515 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2516, LogType.Alarm)] public bool RC3_M2516 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2517, LogType.Alarm)] public bool RC3_M2517 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2518, LogType.Alarm)] public bool RC3_M2518 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2519, LogType.Alarm)] public bool RC3_M2519 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2520, LogType.Alarm)] public bool RC3_M2520 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2521, LogType.Alarm)] public bool RC3_M2521 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2522, LogType.Alarm)] public bool RC3_M2522 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2523, LogType.Alarm)] public bool RC3_M2523 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2524, LogType.Alarm)] public bool RC3_M2524 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2525, LogType.Alarm)] public bool RC3_M2525 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2526, LogType.Alarm)] public bool RC3_M2526 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2527, LogType.Alarm)] public bool RC3_M2527 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2528, LogType.Alarm)] public bool RC3_M2528 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2529, LogType.Alarm)] public bool RC3_M2529 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2530, LogType.Alarm)] public bool RC3_M2530 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2531, LogType.Alarm)] public bool RC3_M2531 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2532, LogType.Alarm)] public bool RC3_M2532 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2533, LogType.Alarm)] public bool RC3_M2533 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2535, LogType.Alarm)] public bool RC3_M2535 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2536, LogType.Alarm)] public bool RC3_M2536 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2537, LogType.Alarm)] public bool RC3_M2537 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2538, LogType.Alarm)] public bool RC3_M2538 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2539, LogType.Alarm)] public bool RC3_M2539 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2540, LogType.Alarm)] public bool RC3_M2540 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2541, LogType.Alarm)] public bool RC3_M2541 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2542, LogType.Alarm)] public bool RC3_M2542 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2543, LogType.Alarm)] public bool RC3_M2543 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2544, LogType.Alarm)] public bool RC3_M2544 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2545, LogType.Alarm)] public bool RC3_M2545 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2546, LogType.Alarm)] public bool RC3_M2546 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2547, LogType.Alarm)] public bool RC3_M2547 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2548, LogType.Alarm)] public bool RC3_M2548 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2549, LogType.Alarm)] public bool RC3_M2549 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2550, LogType.Alarm)] public bool RC3_M2550 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2551, LogType.Alarm)] public bool RC3_M2551 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2552, LogType.Alarm)] public bool RC3_M2552 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2553, LogType.Alarm)] public bool RC3_M2553 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2554, LogType.Alarm)] public bool RC3_M2554 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2555, LogType.Alarm)] public bool RC3_M2555 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2556, LogType.Alarm)] public bool RC3_M2556 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2557, LogType.Alarm)] public bool RC3_M2557 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2558, LogType.Alarm)] public bool RC3_M2558 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2559, LogType.Alarm)] public bool RC3_M2559 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2560, LogType.Alarm)] public bool RC3_M2560 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2561, LogType.Alarm)] public bool RC3_M2561 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2562, LogType.Alarm)] public bool RC3_M2562 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2563, LogType.Alarm)] public bool RC3_M2563 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2564, LogType.Alarm)] public bool RC3_M2564 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2565, LogType.Alarm)] public bool RC3_M2565 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2566, LogType.Alarm)] public bool RC3_M2566 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2567, LogType.Alarm)] public bool RC3_M2567 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2568, LogType.Alarm)] public bool RC3_M2568 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2569, LogType.Alarm)] public bool RC3_M2569 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2570, LogType.Alarm)] public bool RC3_M2570 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2571, LogType.Alarm)] public bool RC3_M2571 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2572, LogType.Alarm)] public bool RC3_M2572 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2573, LogType.Alarm)] public bool RC3_M2573 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2576, LogType.Alarm)] public bool RC3_M2576 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2577, LogType.Alarm)] public bool RC3_M2577 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2580, LogType.Alarm)] public bool RC3_M2580 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2581, LogType.Alarm)] public bool RC3_M2581 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2582, LogType.Alarm)] public bool RC3_M2582 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2583, LogType.Alarm)] public bool RC3_M2583 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2584, LogType.Alarm)] public bool RC3_M2584 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2585, LogType.Alarm)] public bool RC3_M2585 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2586, LogType.Alarm)] public bool RC3_M2586 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2587, LogType.Alarm)] public bool RC3_M2587 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2588, LogType.Alarm)] public bool RC3_M2588 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2589, LogType.Alarm)] public bool RC3_M2589 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2590, LogType.Alarm)] public bool RC3_M2590 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2591, LogType.Alarm)] public bool RC3_M2591 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2592, LogType.Alarm)] public bool RC3_M2592 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2593, LogType.Alarm)] public bool RC3_M2593 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2595, LogType.Alarm)] public bool RC3_M2595 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2596, LogType.Alarm)] public bool RC3_M2596 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2599, LogType.Alarm)] public bool RC3_M2599 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2710, LogType.Alarm)] public bool RC3_M2710 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2711, LogType.Alarm)] public bool RC3_M2711 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2712, LogType.Alarm)] public bool RC3_M2712 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2713, LogType.Alarm)] public bool RC3_M2713 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2714, LogType.Alarm)] public bool RC3_M2714 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2715, LogType.Alarm)] public bool RC3_M2715 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2716, LogType.Alarm)] public bool RC3_M2716 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 2717, LogType.Alarm)] public bool RC3_M2717 { get => Get<bool>(); set => Set(value); }
    #endregion

    #endregion

    #region 機台狀態
    /// <summary>自動模式</summary>
    [PLCBit(BitType.M, 201, LogType.StatusVariables)]
    public bool AutoMode
    {
        get => Get<bool>();
        set => Set(value);
    }
    
    /// <summary>自動模式</summary>
    [PLCBit(BitType.M, 203, LogType.StatusVariables)]
    public bool Manual
    {
        get => Get<bool>();
        set => Set(value);
    }

    [PLCBit(BitType.M, 221, LogType.StatusVariables)]
    public bool AutoMode_Start
    {
        get => Get<bool>();
        set => Set(value);
    }
    
    [PLCBit(BitType.M, 265, LogType.StatusVariables)]
    public bool AlarmMode
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

    ///// <summary>設備狀態，0:停機(STOP)、1:自動(IDLE)、2:自動啟動(RUN)、3:異常(DOWN)、4:保養(PM))</summary>
    //[PLCData(DataType.D, 28, LogType.StatusVariables)]
    //public short EquipmentState
    //{
    //    get => Get<short>();
    //    set => Set(value);
    //}

    ///// <summary>生產狀態0:手動、1:昇溫中、2:恆溫中、7:冷卻降溫中、8:程式結束、9:自動、10:氮氣充氣中 </summary>
    //[PLCData(DataType.D, 29, LogType.StatusVariables)]
    //public short ProcessState
    //{
    //    get => Get<short>();
    //    set => Set(value);
    //}

    #endregion

    #region Coater追板
    //[PLCData(DataType.D, 10000, 16, LogType.CustomData)] public string PanelID { get => Get<string>(); set => Set(value); }
    [PLCData(DataType.D, 20000, 16, LogType.CustomData)] public string LotID { get => Get<string>(); set => Set(value); }

    ///// <summary> 入料 </summary>
    //[PLCBit(BitType.M, 100, LogType.CustomData)] public bool FeedInlet { get => Get<bool>(); set => Set(value); }

    ///// <summary> 入料到等待塗佈 </summary>
    //[PLCBit(BitType.M, 112, LogType.CustomData)] public bool FeedToWait { get => Get<bool>(); set => Set(value); }

    ///// <summary> 等待塗佈到前秤 </summary>
    //[PLCBit(BitType.M, 113, LogType.CustomData)] public bool WaitToFrontWeight { get => Get<bool>(); set => Set(value); }

    ///// <summary> 前秤到塗佈中 </summary>
    //[PLCBit(BitType.M, 114, LogType.CustomData)] public bool FrontWeightToCoater { get => Get<bool>(); set => Set(value); }

    ///// <summary> 塗佈中到後秤 </summary>
    //[PLCBit(BitType.M, 115, LogType.CustomData)] public bool CoaterToBackWeight { get => Get<bool>(); set => Set(value); }

    ///// <summary> 後秤到夾式爐 </summary>
    //[PLCBit(BitType.M, 116, LogType.CustomData)] public bool BackWeightToOven { get => Get<bool>(); set => Set(value); }
    #endregion
}