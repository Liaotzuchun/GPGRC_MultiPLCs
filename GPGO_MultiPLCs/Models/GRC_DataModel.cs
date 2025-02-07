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
    //塗佈速度設定
    //[PointInfo(Device.D, "RecipeCompare_CoatingSpeedSetting", InOut = "Coater1", Operator = Operator.DivisionC, OperandA = "10")]
    //[PPAttribute("RecipeCompare_CoatingSpeedSetting", "1", Min = "0.1", Max = "10", MachineType = "Oven1", IsInt = false, IsSetting = true, ParameterType = "Coater", Unit = "(M/Min)")]
    //D4931,
    //    //塗佈壓力
    //    [PointInfo(Device.D, "RecipeCompare_COATING_PRESSURE_SETTING", InOut = "Coater1", Operator = Operator.DivisionC, OperandA = "1")]
    //[PPAttribute("RecipeCompare_COATING_PRESSURE_SETTING", "1", Min = "-300", Max = "0", MachineType = "Oven1", IsInt = false, IsSetting = true, ParameterType = "Coater", Unit = "(mm)")]
    //D4932,
    //    //基板厚度設定
    //    [PointInfo(Device.D, "RecipeCompare_PanelThicknessSetting", InOut = "Coater1", Operator = Operator.DivisionC, OperandA = "100")]
    //[PPAttribute("RecipeCompare_PanelThicknessSetting", "1", Min = "0.05", Max = "2", MachineType = "Oven1", IsInt = false, IsSetting = true, ParameterType = "Coater", Unit = "(mm)")]
    //D4933,
    //    //烘烤時間設定
    //    [PointInfo(Device.D, "RecipeCompare_BakingTimeSetting", InOut = "Coater1", Operator = Operator.DivisionC, OperandA = "10")]
    //[PPAttribute("RecipeCompare_BakingTimeSetting", "1", Min = "5", Max = "15", MachineType = "Oven1", IsInt = false, IsSetting = true, ParameterType = "Coater", Unit = "(Min)")]
    //D4934,
    //    //第1段溫度設定值
    //    [PointInfo(Device.D, "RecipeCompare_TemperatureSV1", InOut = "Coater1OvenSV1", Operator = Operator.DivisionC, OperandA = "10")]
    //[PPAttribute("RecipeCompare_TemperatureSV1", "1", Min = "0", Max = "150", MachineType = "Oven1", IsInt = false, IsSetting = true, ParameterType = "Oven", Unit = "(℃)")]
    //D4935,
    //    //第2段溫度設定值
    //    [PointInfo(Device.D, "RecipeCompare_TemperatureSV2", InOut = "Coater1OvenSV2", Operator = Operator.DivisionC, OperandA = "10")]
    //[PPAttribute("RecipeCompare_TemperatureSV2", "1", Min = "0", Max = "150", MachineType = "Oven1", IsInt = false, IsSetting = true, ParameterType = "Oven", Unit = "(℃)")]
    //D4936,
    //    //第3段溫度設定值
    //    [PointInfo(Device.D, "RecipeCompare_TemperatureSV3", InOut = "Coater1OvenSV3", Operator = Operator.DivisionC, OperandA = "10")]
    //[PPAttribute("RecipeCompare_TemperatureSV3", "1", Min = "0", Max = "150", MachineType = "Oven1", IsInt = false, IsSetting = true, ParameterType = "Oven", Unit = "(℃)")]
    //D4937,
    //    //第4段溫度設定值
    //    [PointInfo(Device.D, "RecipeCompare_TemperatureSV4", InOut = "Coater1OvenSV4", Operator = Operator.DivisionC, OperandA = "10")]
    //[PPAttribute("RecipeCompare_TemperatureSV4", "1", Min = "0", Max = "150", MachineType = "Oven1", IsInt = false, IsSetting = true, ParameterType = "Oven", Unit = "(℃)")]
    //D4938,
    //    //第5段溫度設定值
    //    [PointInfo(Device.D, "RecipeCompare_TemperatureSV5", InOut = "Coater1OvenSV5", Operator = Operator.DivisionC, OperandA = "10")]
    //[PPAttribute("RecipeCompare_TemperatureSV5", "1", Min = "0", Max = "150", MachineType = "Oven1", IsInt = false, IsSetting = true, ParameterType = "Oven", Unit = "(℃)")]
    //D4939,
    #region Coater1
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
    #endregion
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
    ///// <summary>塗佈次數</summary>
    //[PLCData(DataType.D, 220, LogType.RecipeSet)] public double Coatingoftimes { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈速度設定</summary>
    //[PLCData(DataType.D, 221, LogType.RecipeSet)] public double CoatingSpeedSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>板面夾持距離設定</summary>
    //[PLCData(DataType.D, 223, LogType.RecipeSet)] public double BoardClampingDistance { get => Get<double>(); set => Set(value); }
    ///// <summary>塞孔次數設定</summary>
    //[PLCData(DataType.D, 224, LogType.RecipeSet)] public double Plugoftimes { get => Get<double>(); set => Set(value); }
    ///// <summary>塗佈壓力設定</summary>
    //[PLCData(DataType.D, 225, LogType.RecipeSet)] public double CoatingPressureSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>拉料速度比</summary>
    //[PLCData(DataType.D, 226, LogType.RecipeSet)] public double CoatingPullSpeed { get => Get<double>(); set => Set(value); }
    ///// <summary>基板厚度設定</summary>
    //[PLCData(DataType.D, 227, LogType.RecipeSet)] public double PanelThicknessSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>入料下降位置設定</summary>
    //[PLCData(DataType.D, 228, LogType.RecipeSet)] public double LocationOfDrop { get => Get<double>(); set => Set(value); }
    ///// <summary>入料滾輪延遲時間設定</summary>
    //[PLCData(DataType.D, 229, LogType.RecipeSet)] public double Feedingrollerdelaytimesetting { get => Get<double>(); set => Set(value); }
    ///// <summary>入料下降速度設定</summary>
    //[PLCData(DataType.D, 230, LogType.RecipeSet)] public double SpeedOfDrop { get => Get<double>(); set => Set(value); }
    ///// <summary>基板寬度設定</summary>
    //[PLCData(DataType.D, 233, LogType.RecipeSet)] public double PanelWidthSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>刮刀壓力設定1</summary>
    //[PLCData(DataType.D, 100, 0.01, LogType.RecipeSet)] public double Blade_Pressure1 { get => Get<double>(); set => Set(value); }
    ///// <summary>刮刀壓力設定2</summary>
    //[PLCData(DataType.D, 101, 0.01, LogType.RecipeSet)] public double Blade_Pressure2 { get => Get<double>(); set => Set(value); }
    ///// <summary>刮刀壓力設定3</summary>
    //[PLCData(DataType.D, 102, 0.01, LogType.RecipeSet)] public double Blade_Pressure3 { get => Get<double>(); set => Set(value); }
    ///// <summary>刮刀壓力設定4</summary>
    //[PLCData(DataType.D, 103, 0.01, LogType.RecipeSet)] public double Blade_Pressure4 { get => Get<double>(); set => Set(value); }
    ///// <summary>烘烤時間設定</summary>
    //[PLCData(DataType.D, 2026, LogType.RecipeSet)] public double BakingTimeSetting { get => Get<double>(); set => Set(value); }
    ///// <summary>第1段溫度設定值</summary>
    //[PLCData(DataType.D, 2450, 0.1, LogType.RecipeSet)] public double TemperatureSV1 { get => Get<double>(); set => Set(value); }
    ///// <summary>第2段溫度設定值</summary>
    //[PLCData(DataType.D, 2451, 0.1, LogType.RecipeSet)] public double TemperatureSV2 { get => Get<double>(); set => Set(value); }
    ///// <summary>第1段溫度設定值</summary>
    //[PLCData(DataType.D, 14010, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV1 { get => Get<double>(); set => Set(value); }
    ///// <summary>第2段溫度設定值</summary>
    //[PLCData(DataType.D, 14011, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV2 { get => Get<double>(); set => Set(value); }
    ///// <summary>第3段溫度設定值</summary>
    //[PLCData(DataType.D, 14012, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV3 { get => Get<double>(); set => Set(value); }
    ///// <summary>第4段溫度設定值</summary>
    //[PLCData(DataType.D, 14013, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV4 { get => Get<double>(); set => Set(value); }
    ///// <summary>第5段溫度設定值</summary>
    //[PLCData(DataType.D, 14014, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV5 { get => Get<double>(); set => Set(value); }
    ///// <summary>第6段溫度設定值</summary>
    //[PLCData(DataType.D, 14015, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV6 { get => Get<double>(); set => Set(value); }
    ///// <summary>第7段溫度設定值</summary>
    //[PLCData(DataType.D, 14016, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV7 { get => Get<double>(); set => Set(value); }
    /// <summary>第8段溫度設定值</summary>
    //[PLCData(DataType.D, 14017, 0.1, LogType.RecipeSet)] public double RC3_TemperatureSV8 { get => Get<double>(); set => Set(value); }
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

    #region Coater
    [PLCBit(BitType.F, 0000, LogType.Alarm)] public bool F0000 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0001, LogType.Alarm)] public bool F0001 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0002, LogType.Alarm)] public bool F0002 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0003, LogType.Alarm)] public bool F0003 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0004, LogType.Alarm)] public bool F0004 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0006, LogType.Alarm)] public bool F0006 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0008, LogType.Alarm)] public bool F0008 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0009, LogType.Alarm)] public bool F0009 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0010, LogType.Alarm)] public bool F0010 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0011, LogType.Alarm)] public bool F0011 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0012, LogType.Alarm)] public bool F0012 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0014, LogType.Alarm)] public bool F0014 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0016, LogType.Alarm)] public bool F0016 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0017, LogType.Alarm)] public bool F0017 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0018, LogType.Alarm)] public bool F0018 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0019, LogType.Alarm)] public bool F0019 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0020, LogType.Alarm)] public bool F0020 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0022, LogType.Alarm)] public bool F0022 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0024, LogType.Alarm)] public bool F0024 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0025, LogType.Alarm)] public bool F0025 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0026, LogType.Alarm)] public bool F0026 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0027, LogType.Alarm)] public bool F0027 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0028, LogType.Alarm)] public bool F0028 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0030, LogType.Alarm)] public bool F0030 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0044, LogType.Alarm)] public bool F0044 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0045, LogType.Alarm)] public bool F0045 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0046, LogType.Alarm)] public bool F0046 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0047, LogType.Alarm)] public bool F0047 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0058, LogType.Alarm)] public bool F0058 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0059, LogType.Alarm)] public bool F0059 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0060, LogType.Alarm)] public bool F0060 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0061, LogType.Alarm)] public bool F0061 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0062, LogType.Alarm)] public bool F0062 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0075, LogType.Alarm)] public bool F0075 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0076, LogType.Alarm)] public bool F0076 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0077, LogType.Alarm)] public bool F0077 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0078, LogType.Alarm)] public bool F0078 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0079, LogType.Alarm)] public bool F0079 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0080, LogType.Alarm)] public bool F0080 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0086, LogType.Alarm)] public bool F0086 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0087, LogType.Alarm)] public bool F0087 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0088, LogType.Alarm)] public bool F0088 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0089, LogType.Alarm)] public bool F0089 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0095, LogType.Alarm)] public bool F0095 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0096, LogType.Alarm)] public bool F0096 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0097, LogType.Alarm)] public bool F0097 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0099, LogType.Alarm)] public bool F0099 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0100, LogType.Alarm)] public bool F0100 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0101, LogType.Alarm)] public bool F0101 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0102, LogType.Alarm)] public bool F0102 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0103, LogType.Alarm)] public bool F0103 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0104, LogType.Alarm)] public bool F0104 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0105, LogType.Alarm)] public bool F0105 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0106, LogType.Alarm)] public bool F0106 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0107, LogType.Alarm)] public bool F0107 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0108, LogType.Alarm)] public bool F0108 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0109, LogType.Alarm)] public bool F0109 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0110, LogType.Alarm)] public bool F0110 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0115, LogType.Alarm)] public bool F0115 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0116, LogType.Alarm)] public bool F0116 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0118, LogType.Alarm)] public bool F0118 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0119, LogType.Alarm)] public bool F0119 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0120, LogType.Alarm)] public bool F0120 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0122, LogType.Alarm)] public bool F0122 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0127, LogType.Alarm)] public bool F0127 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0140, LogType.Alarm)] public bool F0140 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0141, LogType.Alarm)] public bool F0141 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0151, LogType.Alarm)] public bool F0151 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0152, LogType.Alarm)] public bool F0152 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0153, LogType.Alarm)] public bool F0153 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0166, LogType.Alarm)] public bool F0166 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0170, LogType.Alarm)] public bool F0170 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0171, LogType.Alarm)] public bool F0171 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6000, LogType.Alarm)] public bool M6000 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6001, LogType.Alarm)] public bool M6001 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6002, LogType.Alarm)] public bool M6002 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6003, LogType.Alarm)] public bool M6003 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6005, LogType.Alarm)] public bool M6005 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6006, LogType.Alarm)] public bool M6006 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6007, LogType.Alarm)] public bool M6007 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6008, LogType.Alarm)] public bool M6008 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6009, LogType.Alarm)] public bool M6009 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6010, LogType.Alarm)] public bool M6010 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6011, LogType.Alarm)] public bool M6011 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6012, LogType.Alarm)] public bool M6012 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6013, LogType.Alarm)] public bool M6013 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6014, LogType.Alarm)] public bool M6014 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6015, LogType.Alarm)] public bool M6015 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6016, LogType.Alarm)] public bool M6016 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6017, LogType.Alarm)] public bool M6017 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6018, LogType.Alarm)] public bool M6018 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6019, LogType.Alarm)] public bool M6019 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6020, LogType.Alarm)] public bool M6020 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6021, LogType.Alarm)] public bool M6021 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6022, LogType.Alarm)] public bool M6022 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6023, LogType.Alarm)] public bool M6023 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6024, LogType.Alarm)] public bool M6024 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6026, LogType.Alarm)] public bool M6026 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6027, LogType.Alarm)] public bool M6027 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6028, LogType.Alarm)] public bool M6028 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6029, LogType.Alarm)] public bool M6029 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6030, LogType.Alarm)] public bool M6030 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6033, LogType.Alarm)] public bool M6033 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6038, LogType.Alarm)] public bool M6038 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6039, LogType.Alarm)] public bool M6039 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6040, LogType.Alarm)] public bool M6040 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6041, LogType.Alarm)] public bool M6041 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6044, LogType.Alarm)] public bool M6044 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6045, LogType.Alarm)] public bool M6045 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6046, LogType.Alarm)] public bool M6046 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6047, LogType.Alarm)] public bool M6047 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6048, LogType.Alarm)] public bool M6048 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6050, LogType.Alarm)] public bool M6050 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6051, LogType.Alarm)] public bool M6051 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6052, LogType.Alarm)] public bool M6052 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6054, LogType.Alarm)] public bool M6054 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6055, LogType.Alarm)] public bool M6055 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6056, LogType.Alarm)] public bool M6056 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6058, LogType.Alarm)] public bool M6058 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6059, LogType.Alarm)] public bool M6059 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6060, LogType.Alarm)] public bool M6060 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6062, LogType.Alarm)] public bool M6062 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6063, LogType.Alarm)] public bool M6063 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6064, LogType.Alarm)] public bool M6064 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6066, LogType.Alarm)] public bool M6066 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6067, LogType.Alarm)] public bool M6067 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6068, LogType.Alarm)] public bool M6068 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6074, LogType.Alarm)] public bool M6074 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6075, LogType.Alarm)] public bool M6075 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6076, LogType.Alarm)] public bool M6076 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6077, LogType.Alarm)] public bool M6077 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6078, LogType.Alarm)] public bool M6078 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6079, LogType.Alarm)] public bool M6079 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6080, LogType.Alarm)] public bool M6080 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6081, LogType.Alarm)] public bool M6081 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6083, LogType.Alarm)] public bool M6083 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6084, LogType.Alarm)] public bool M6084 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6085, LogType.Alarm)] public bool M6085 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6086, LogType.Alarm)] public bool M6086 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6087, LogType.Alarm)] public bool M6087 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6088, LogType.Alarm)] public bool M6088 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6089, LogType.Alarm)] public bool M6089 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6090, LogType.Alarm)] public bool M6090 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6091, LogType.Alarm)] public bool M6091 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6092, LogType.Alarm)] public bool M6092 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6093, LogType.Alarm)] public bool M6093 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6094, LogType.Alarm)] public bool M6094 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0048, LogType.Alarm)] public bool F0048 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0049, LogType.Alarm)] public bool F0049 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0056, LogType.Alarm)] public bool F0056 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0057, LogType.Alarm)] public bool F0057 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0063, LogType.Alarm)] public bool F0063 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0064, LogType.Alarm)] public bool F0064 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0065, LogType.Alarm)] public bool F0065 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0066, LogType.Alarm)] public bool F0066 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0067, LogType.Alarm)] public bool F0067 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0068, LogType.Alarm)] public bool F0068 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0069, LogType.Alarm)] public bool F0069 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0070, LogType.Alarm)] public bool F0070 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0071, LogType.Alarm)] public bool F0071 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0072, LogType.Alarm)] public bool F0072 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0073, LogType.Alarm)] public bool F0073 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0074, LogType.Alarm)] public bool F0074 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0081, LogType.Alarm)] public bool F0081 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0082, LogType.Alarm)] public bool F0082 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0117, LogType.Alarm)] public bool F0117 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0160, LogType.Alarm)] public bool F0160 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0161, LogType.Alarm)] public bool F0161 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0162, LogType.Alarm)] public bool F0162 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0163, LogType.Alarm)] public bool F0163 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0164, LogType.Alarm)] public bool F0164 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0165, LogType.Alarm)] public bool F0165 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0167, LogType.Alarm)] public bool F0167 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.F, 0168, LogType.Alarm)] public bool F0168 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6004, LogType.Alarm)] public bool M6004 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6031, LogType.Alarm)] public bool M6031 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6032, LogType.Alarm)] public bool M6032 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6036, LogType.Alarm)] public bool M6036 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6037, LogType.Alarm)] public bool M6037 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6053, LogType.Alarm)] public bool M6053 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6057, LogType.Alarm)] public bool M6057 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6061, LogType.Alarm)] public bool M6061 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6065, LogType.Alarm)] public bool M6065 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6069, LogType.Alarm)] public bool M6069 { get => Get<bool>(); set => Set(value); }
    [PLCBit(BitType.M, 6106, LogType.Alarm)] public bool M6106 { get => Get<bool>(); set => Set(value); }
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
    //[PLCData(DataType.D, 20000, 16, LogType.CustomData)] public string LotID { get => Get<string>(); set => Set(value); }

    /// <summary> 入料 </summary>
    //[PLCBit(BitType.M, 100, LogType.CustomData)] public bool PanelIn { get => Get<bool>(); set => Set(value); }
    /// <summary> 出料 </summary>
    //[PLCBit(BitType.M, 103, LogType.CustomData)] public bool PanelOut { get => Get<bool>(); set => Set(value); }

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