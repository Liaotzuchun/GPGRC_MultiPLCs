using System.Collections.Generic;
using GPMVVM.Models;
using PLCService;

namespace GPGRC_MultiPLCs.Models;

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
    ///// <summary>第1段烘烤溫度</summary>
    //[PLCData(DataType.D, 700, 20, LogType.None)]
    //public double TopTemperatureSetpoint_1SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>第2段烘烤溫度</summary>
    //[PLCData(DataType.D, 701, 20, LogType.None)]
    //public double TopTemperatureSetpoint_2SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>第3段烘烤溫度</summary>
    //[PLCData(DataType.D, 702, 20, LogType.None)]
    //public double TopTemperatureSetpoint_3SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>第4段烘烤溫度</summary>
    //[PLCData(DataType.D, 703, 20, LogType.None)]
    //public double TopTemperatureSetpoint_4SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>第5段烘烤溫度</summary>
    //[PLCData(DataType.D, 704, 20, LogType.None)]
    //public double TopTemperatureSetpoint_5SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>第6段烘烤溫度</summary>
    //[PLCData(DataType.D, 705, 20, LogType.None)]
    //public double TopTemperatureSetpoint_6SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>升溫時間1</summary>
    //[PLCData(DataType.D, 760, 0.1, LogType.None)]
    //public double TopRampTime_1SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>升溫時間2</summary>
    //[PLCData(DataType.D, 761, 0.1, LogType.None)]
    //public double TopRampTime_2SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>升溫時間3</summary>
    //[PLCData(DataType.D, 762, 0.1, LogType.None)]
    //public double TopRampTime_3SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>升溫時間4</summary>
    //[PLCData(DataType.D, 763, 0.1, LogType.None)]
    //public double TopRampTime_4SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>升溫時間5</summary>
    //[PLCData(DataType.D, 764, 0.1, LogType.None)]
    //public double TopRampTime_5SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>升溫時間6</summary>
    //[PLCData(DataType.D, 765, 0.1, LogType.None)]
    //public double TopRampTime_6SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>恆溫時間1</summary>
    //[PLCData(DataType.D, 715, 0.1, LogType.None)]
    //public double TopDwellTime_1SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>恆溫時間2</summary>
    //[PLCData(DataType.D, 716, 0.1, LogType.None)]
    //public double TopDwellTime_2SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>恆溫時間3</summary>
    //[PLCData(DataType.D, 717, 0.1, LogType.None)]
    //public double TopDwellTime_3SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>恆溫時間4</summary>
    //[PLCData(DataType.D, 718, 0.1, LogType.None)]
    //public double TopDwellTime_4SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>恆溫時間5</summary>
    //[PLCData(DataType.D, 719, 0.1, LogType.None)]
    //public double TopDwellTime_5SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}

    ///// <summary>恆溫時間6</summary>
    //[PLCData(DataType.D, 720, 0.1, LogType.None)]
    //public double TopDwellTime_6SV
    //{
    //    get => Get<double>();
    //    set => Set(value);
    //}
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
    #region Coater
    /// <summary>塗佈次數</summary>
    [PLCData(DataType.D, 700, LogType.RecipeSet)] public double Coatingoftimes { get => Get<double>(); set => Set(value); }
    /// <summary>塗佈速度設定</summary>
    [PLCData(DataType.D, 701, LogType.RecipeSet)] public double CoatingSpeedSetting { get => Get<double>(); set => Set(value); }
    /// <summary>板面夾持距離設定</summary>
    [PLCData(DataType.D, 702, LogType.RecipeSet)] public double BoardClampingDistance { get => Get<double>(); set => Set(value); }
    /// <summary>塞孔次數設定</summary>
    [PLCData(DataType.D, 703, LogType.RecipeSet)] public double Plugoftimes { get => Get<double>(); set => Set(value); }
    /// <summary>塗佈壓力設定</summary>
    [PLCData(DataType.D, 704, LogType.RecipeSet)] public double CoatingPressureSetting { get => Get<double>(); set => Set(value); }
    /// <summary>基板厚度設定</summary>
    [PLCData(DataType.D, 705, LogType.RecipeSet)] public double PanelThicknessSetting { get => Get<double>(); set => Set(value); }
    /// <summary>入料下降位置設定</summary>
    [PLCData(DataType.D, 706, LogType.RecipeSet)] public double LocationOfDrop { get => Get<double>(); set => Set(value); }
    /// <summary>左前D.BAR壓力設定</summary>
    [PLCData(DataType.D, 707, LogType.RecipeSet)] public double D_BarPressureSetting1 { get => Get<double>(); set => Set(value); }
    /// <summary>右前D.BAR壓力設定</summary>
    [PLCData(DataType.D, 708, LogType.RecipeSet)] public double D_BarPressureSetting2 { get => Get<double>(); set => Set(value); }
    /// <summary>左後D.BAR壓力設定</summary>
    [PLCData(DataType.D, 709, LogType.RecipeSet)] public double D_BarPressureSetting3 { get => Get<double>(); set => Set(value); }
    /// <summary>右後D.BAR壓力設定</summary>
    [PLCData(DataType.D, 710, LogType.RecipeSet)] public double D_BarPressureSetting4 { get => Get<double>(); set => Set(value); }
    /// <summary>塞孔刮刀壓力設定</summary>
    [PLCData(DataType.D, 711, LogType.RecipeSet)] public double Blade_Pressure { get => Get<double>(); set => Set(value); }
    /// <summary>烘烤時間設定</summary>
    [PLCData(DataType.D, 712, LogType.RecipeSet)] public double BakingTimeSetting { get => Get<double>(); set => Set(value); }
    /// <summary>第1段溫度設定值</summary>
    [PLCData(DataType.D, 713, LogType.RecipeSet)] public double TemperatureSV1 { get => Get<double>(); set => Set(value); }
    /// <summary>第2段溫度設定值</summary>
    [PLCData(DataType.D, 714, LogType.RecipeSet)] public double TemperatureSV2 { get => Get<double>(); set => Set(value); }
    /// <summary>塗佈使用</summary>
    [PLCData(DataType.D, 715, LogType.RecipeSet)] public double UseCoating { get => Get<double>(); set => Set(value); }
    /// <summary>塞孔使用</summary>
    [PLCData(DataType.D, 716, LogType.RecipeSet)] public double UsePlug { get => Get<double>(); set => Set(value); }
    /// <summary>標準墨重</summary>
    [PLCData(DataType.D, 717, LogType.RecipeSet)] public double StandardInk { get => Get<double>(); set => Set(value); }
    /// <summary>墨重誤差值</summary>
    [PLCData(DataType.D, 718, LogType.RecipeSet)] public double DifferenceOfInk { get => Get<double>(); set => Set(value); }
    #endregion
    #endregion
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

    #region Coater追板
    [PLCData(DataType.D, 10000, 16, LogType.CustomData)]
    public string PanelID
    {
        get => Get<string>();
        set => Set(value);
    }
    /// <summary> 入料 </summary>
    [PLCBit(BitType.M, 100, LogType.CustomData)] public bool FeedInlet { get => Get<bool>(); set => Set(value); }

    /// <summary> 入料到等待塗佈 </summary>
    [PLCBit(BitType.M, 112, LogType.CustomData)] public bool FeedToWait { get => Get<bool>(); set => Set(value); }

    /// <summary> 等待塗佈到前秤 </summary>
    [PLCBit(BitType.M, 113, LogType.CustomData)] public bool WaitToFrontWeight { get => Get<bool>(); set => Set(value); }

    /// <summary> 前秤到塗佈中 </summary>
    [PLCBit(BitType.M, 114, LogType.CustomData)] public bool FrontWeightToCoater { get => Get<bool>(); set => Set(value); }

    /// <summary> 塗佈中到後秤 </summary>
    [PLCBit(BitType.M, 115, LogType.CustomData)] public bool CoaterToBackWeight { get => Get<bool>(); set => Set(value); }

    /// <summary> 後秤到夾式爐 </summary>
    [PLCBit(BitType.M, 116, LogType.CustomData)] public bool BackWeightToOven { get => Get<bool>(); set => Set(value); }
    #endregion
}