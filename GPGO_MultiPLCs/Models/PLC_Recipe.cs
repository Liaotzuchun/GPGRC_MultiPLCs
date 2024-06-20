using System;
using System.Collections.Generic;
using GPMVVM.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace GPGRC_MultiPLCs.Models;

/// <summary>PLC配方</summary>
[OrderedObject]
[BsonIgnoreExtraElements]
public class PLC_Recipe : RecipeBase<PLC_Recipe>
{
    private const int Digits0 = 0;
    private const int Digits1 = 1;
    public double Coatingoftimes_Max => 100;
    public double Coatingoftimes_Min => 1;
    public double CoatingSpeedSetting_Max => 999.9;
    public double CoatingSpeedSetting_Min => 0.0;
    public double BoardClampingDistance_Max => 999.9;
    public double BoardClampingDistance_Min => 0.0;
    public double Plugoftimes_Max => 999.9;
    public double Plugoftimes_Min => 0.0;
    public double CoatingPressureSetting_Max => 999.9;
    public double CoatingPressureSetting_Min => 0.0;
    public double PanelThicknessSetting_Max => 999.9;
    public double PanelThicknessSetting_Min => 0.0;
    public double PanelWidthSetting_Max => 999.9;
    public double PanelWidthSetting_Min => 0.0;
    public double LocationOfDrop_Max => 999.9;
    public double LocationOfDrop_Min => 0.0;
    public double SpeedOfDrop_Max => 999.9;
    public double SpeedOfDrop_Min => 0.0;
    public double Blade_Pressure_Max => 999.9;
    public double Blade_Pressure_Min => 0.0;
    public double BakingTimeSetting_Max => 999.9;
    public double BakingTimeSetting_Min => 0.0;
    public double TemperatureSV_Max => 999.9;
    public double TemperatureSV_Min => 0.0;
    public double CoatingPullSpeed_Max => 999.9;
    public double CoatingPullSpeed_Min => 0.0;
    public double Feedingrollerdelaytimesetting_Max => 999.9;
    public double Feedingrollerdelaytimesetting_Min => 0.0;


    [JsonIgnore] //匯出時檔名已是名稱，此處用JsonIgnore是用來做配方比較需忽略此項
    [OrderIndex(-1)]
    [LanguageTranslator("Recipe Name", "配方名稱", "配方名称")]
    public override string RecipeName { get; set; } = string.Empty;
    #region RC1
    [OrderIndex(3)]
    [LanguageTranslator("Coatingoftimes", "塗佈次數", "塗佈次數")]
    public double RC1_Coatingoftimes
    {
        get => Get<double>();
        set
        {
            if (value > Coatingoftimes_Max)
            {
                value = Coatingoftimes_Max;
            }
            else if (value < Coatingoftimes_Min)
            {
                value = Coatingoftimes_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("CoatingSpeedSetting", "塗佈速度設定", "塗佈速度設定")]
    public double RC1_CoatingSpeedSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > CoatingSpeedSetting_Max)
            {
                value = CoatingSpeedSetting_Max;
            }
            else if (value < CoatingSpeedSetting_Min)
            {
                value = CoatingSpeedSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("BoardClampingDistance", "板面夾持距離", "塗佈速度設定")]
    public double RC1_BoardClampingDistance
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > BoardClampingDistance_Max)
            {
                value = BoardClampingDistance_Max;
            }
            else if (value < BoardClampingDistance_Min)
            {
                value = BoardClampingDistance_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("Plugoftimes", "塞孔次數", "塞孔次數")]
    public double RC1_Plugoftimes
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Plugoftimes_Max)
            {
                value = Plugoftimes_Max;
            }
            else if (value < Plugoftimes_Min)
            {
                value = Plugoftimes_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("CoatingPressureSetting", "塗佈壓力設定", "塗佈壓力設定")]
    public double RC1_CoatingPressureSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > CoatingPressureSetting_Max)
            {
                value = CoatingPressureSetting_Max;
            }
            else if (value < CoatingPressureSetting_Min)
            {
                value = CoatingPressureSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("PanelThicknessSetting", "基板厚度設定", "基板厚度設定")]
    public double RC1_PanelThicknessSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > PanelThicknessSetting_Max)
            {
                value = PanelThicknessSetting_Max;
            }
            else if (value < PanelThicknessSetting_Min)
            {
                value = PanelThicknessSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("PanelWidthSetting", "基板寬度設定", "基板寬度設定")]
    public double RC1_PanelWidthSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > PanelWidthSetting_Max)
            {
                value = PanelWidthSetting_Max;
            }
            else if (value < PanelWidthSetting_Min)
            {
                value = PanelWidthSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("LocationOfDrop", "入料下降位置設定", "入料下降位置設定")]
    public double RC1_LocationOfDrop
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > LocationOfDrop_Max)
            {
                value = LocationOfDrop_Max;
            }
            else if (value < LocationOfDrop_Min)
            {
                value = LocationOfDrop_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("SpeedOfDrop", "入料下降位置設定", "入料下降位置設定")]
    public double RC1_SpeedOfDrop
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > SpeedOfDrop_Max)
            {
                value = SpeedOfDrop_Max;
            }
            else if (value < SpeedOfDrop_Min)
            {
                value = SpeedOfDrop_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure1", "刮刀壓力設定1", "刮刀壓力設定1")]
    public double RC1_Blade_Pressure1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure2", "刮刀壓力設定2", "刮刀壓力設定2")]
    public double RC1_Blade_Pressure2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure3", "刮刀壓力設定3", "刮刀壓力設定3")]
    public double RC1_Blade_Pressure3
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure4", "刮刀壓力設定4", "刮刀壓力設定4")]
    public double RC1_Blade_Pressure4
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("CoatingPullSpeed", "拉料速度", "拉料速度")]
    public double RC1_CoatingPullSpeed
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > CoatingPullSpeed_Max)
            {
                value = CoatingPullSpeed_Max;
            }
            else if (value < CoatingPullSpeed_Min)
            {
                value = CoatingPullSpeed_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("BakingTimeSetting", "烘烤時間設定", "烘烤時間設定")]
    public double RC1_BakingTimeSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > BakingTimeSetting_Max)
            {
                value = BakingTimeSetting_Max;
            }
            else if (value < BakingTimeSetting_Min)
            {
                value = BakingTimeSetting_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV1", "第1段溫度設定值", "第1段溫度設定值")]
    public double RC1_TemperatureSV1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV2", "第2段溫度設定值", "第2段溫度設定值")]
    public double RC1_TemperatureSV2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    #endregion
    #region RC2
    [OrderIndex(3)]
    [LanguageTranslator("Coatingoftimes", "塗佈次數", "塗佈次數")]
    public double RC2_Coatingoftimes
    {
        get => Get<double>();
        set
        {
            if (value > Coatingoftimes_Max)
            {
                value = Coatingoftimes_Max;
            }
            else if (value < Coatingoftimes_Min)
            {
                value = Coatingoftimes_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("CoatingSpeedSetting", "塗佈速度設定", "塗佈速度設定")]
    public double RC2_CoatingSpeedSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > CoatingSpeedSetting_Max)
            {
                value = CoatingSpeedSetting_Max;
            }
            else if (value < CoatingSpeedSetting_Min)
            {
                value = CoatingSpeedSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("BoardClampingDistance", "板面夾持距離", "塗佈速度設定")]
    public double RC2_BoardClampingDistance
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > BoardClampingDistance_Max)
            {
                value = BoardClampingDistance_Max;
            }
            else if (value < BoardClampingDistance_Min)
            {
                value = BoardClampingDistance_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("Plugoftimes", "塞孔次數", "塞孔次數")]
    public double RC2_Plugoftimes
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Plugoftimes_Max)
            {
                value = Plugoftimes_Max;
            }
            else if (value < Plugoftimes_Min)
            {
                value = Plugoftimes_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("CoatingPressureSetting", "塗佈壓力設定", "塗佈壓力設定")]
    public double RC2_CoatingPressureSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > CoatingPressureSetting_Max)
            {
                value = CoatingPressureSetting_Max;
            }
            else if (value < CoatingPressureSetting_Min)
            {
                value = CoatingPressureSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("PanelThicknessSetting", "基板厚度設定", "基板厚度設定")]
    public double RC2_PanelThicknessSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > PanelThicknessSetting_Max)
            {
                value = PanelThicknessSetting_Max;
            }
            else if (value < PanelThicknessSetting_Min)
            {
                value = PanelThicknessSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("PanelWidthSetting", "基板寬度設定", "基板寬度設定")]
    public double RC2_PanelWidthSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > PanelWidthSetting_Max)
            {
                value = PanelWidthSetting_Max;
            }
            else if (value < PanelWidthSetting_Min)
            {
                value = PanelWidthSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("LocationOfDrop", "入料下降位置設定", "入料下降位置設定")]
    public double RC2_LocationOfDrop
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > LocationOfDrop_Max)
            {
                value = LocationOfDrop_Max;
            }
            else if (value < LocationOfDrop_Min)
            {
                value = LocationOfDrop_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("SpeedOfDrop", "入料下降位置設定", "入料下降位置設定")]
    public double RC2_SpeedOfDrop
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > SpeedOfDrop_Max)
            {
                value = SpeedOfDrop_Max;
            }
            else if (value < SpeedOfDrop_Min)
            {
                value = SpeedOfDrop_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure1", "刮刀壓力設定1", "刮刀壓力設定1")]
    public double RC2_Blade_Pressure1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure2", "刮刀壓力設定2", "刮刀壓力設定2")]
    public double RC2_Blade_Pressure2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure3", "刮刀壓力設定3", "刮刀壓力設定3")]
    public double RC2_Blade_Pressure3
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure4", "刮刀壓力設定4", "刮刀壓力設定4")]
    public double RC2_Blade_Pressure4
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("CoatingPullSpeed", "拉料速度", "拉料速度")]
    public double RC2_CoatingPullSpeed
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > CoatingPullSpeed_Max)
            {
                value = CoatingPullSpeed_Max;
            }
            else if (value < CoatingPullSpeed_Min)
            {
                value = CoatingPullSpeed_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("BakingTimeSetting", "烘烤時間設定", "烘烤時間設定")]
    public double RC2_BakingTimeSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > BakingTimeSetting_Max)
            {
                value = BakingTimeSetting_Max;
            }
            else if (value < BakingTimeSetting_Min)
            {
                value = BakingTimeSetting_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV1", "第1段溫度設定值", "第1段溫度設定值")]
    public double RC2_TemperatureSV1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV2", "第2段溫度設定值", "第2段溫度設定值")]
    public double RC2_TemperatureSV2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    #endregion
    #region RC3
    [OrderIndex(3)]
    [LanguageTranslator("Coatingoftimes", "塗佈次數", "塗佈次數")]
    public double RC3_Coatingoftimes
    {
        get => Get<double>();
        set
        {
            if (value > Coatingoftimes_Max)
            {
                value = Coatingoftimes_Max;
            }
            else if (value < Coatingoftimes_Min)
            {
                value = Coatingoftimes_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("CoatingSpeedSetting", "塗佈速度設定", "塗佈速度設定")]
    public double RC3_CoatingSpeedSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > CoatingSpeedSetting_Max)
            {
                value = CoatingSpeedSetting_Max;
            }
            else if (value < CoatingSpeedSetting_Min)
            {
                value = CoatingSpeedSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("BoardClampingDistance", "板面夾持距離", "塗佈速度設定")]
    public double RC3_BoardClampingDistance
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > BoardClampingDistance_Max)
            {
                value = BoardClampingDistance_Max;
            }
            else if (value < BoardClampingDistance_Min)
            {
                value = BoardClampingDistance_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("Plugoftimes", "塞孔次數", "塞孔次數")]
    public double RC3_Plugoftimes
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Plugoftimes_Max)
            {
                value = Plugoftimes_Max;
            }
            else if (value < Plugoftimes_Min)
            {
                value = Plugoftimes_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("CoatingPressureSetting", "塗佈壓力設定", "塗佈壓力設定")]
    public double RC3_CoatingPressureSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > CoatingPressureSetting_Max)
            {
                value = CoatingPressureSetting_Max;
            }
            else if (value < CoatingPressureSetting_Min)
            {
                value = CoatingPressureSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("PanelThicknessSetting", "基板厚度設定", "基板厚度設定")]
    public double RC3_PanelThicknessSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > PanelThicknessSetting_Max)
            {
                value = PanelThicknessSetting_Max;
            }
            else if (value < PanelThicknessSetting_Min)
            {
                value = PanelThicknessSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("PanelWidthSetting", "基板寬度設定", "基板寬度設定")]
    public double RC3_PanelWidthSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > PanelWidthSetting_Max)
            {
                value = PanelWidthSetting_Max;
            }
            else if (value < PanelWidthSetting_Min)
            {
                value = PanelWidthSetting_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("LocationOfDrop", "入料下降位置設定", "入料下降位置設定")]
    public double RC3_LocationOfDrop
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > LocationOfDrop_Max)
            {
                value = LocationOfDrop_Max;
            }
            else if (value < LocationOfDrop_Min)
            {
                value = LocationOfDrop_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("SpeedOfDrop", "入料下降位置設定", "入料下降位置設定")]
    public double RC3_SpeedOfDrop
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > SpeedOfDrop_Max)
            {
                value = SpeedOfDrop_Max;
            }
            else if (value < SpeedOfDrop_Min)
            {
                value = SpeedOfDrop_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure1", "刮刀壓力設定1", "刮刀壓力設定1")]
    public double RC3_Blade_Pressure1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure2", "刮刀壓力設定2", "刮刀壓力設定2")]
    public double RC3_Blade_Pressure2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure3", "刮刀壓力設定3", "刮刀壓力設定3")]
    public double RC3_Blade_Pressure3
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure4", "刮刀壓力設定4", "刮刀壓力設定4")]
    public double RC3_Blade_Pressure4
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > Blade_Pressure_Max)
            {
                value = Blade_Pressure_Max;
            }
            else if (value < Blade_Pressure_Min)
            {
                value = Blade_Pressure_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("CoatingPullSpeed", "拉料速度", "拉料速度")]
    public double RC3_CoatingPullSpeed
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > CoatingPullSpeed_Max)
            {
                value = CoatingPullSpeed_Max;
            }
            else if (value < CoatingPullSpeed_Min)
            {
                value = CoatingPullSpeed_Min;
            }
            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("BakingTimeSetting", "烘烤時間設定", "烘烤時間設定")]
    public double RC3_BakingTimeSetting
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > BakingTimeSetting_Max)
            {
                value = BakingTimeSetting_Max;
            }
            else if (value < BakingTimeSetting_Min)
            {
                value = BakingTimeSetting_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV1", "第1段溫度設定值", "第1段溫度設定值")]
    public double RC3_TemperatureSV1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV2", "第2段溫度設定值", "第2段溫度設定值")]
    public double RC3_TemperatureSV2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV3", "第3段溫度設定值", "第3段溫度設定值")]
    public double RC3_TemperatureSV3
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV4", "第4段溫度設定值", "第4段溫度設定值")]
    public double RC3_TemperatureSV4
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV5", "第5段溫度設定值", "第5段溫度設定值")]
    public double RC3_TemperatureSV5
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV6", "第6段溫度設定值", "第6段溫度設定值")]
    public double RC3_TemperatureSV6
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV7", "第7段溫度設定值", "第7段溫度設定值")]
    public double RC3_TemperatureSV7
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV8", "第8段溫度設定值", "第8段溫度設定值")]
    public double RC3_TemperatureSV8
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV_Max)
            {
                value = TemperatureSV_Max;
            }
            else if (value < TemperatureSV_Min)
            {
                value = TemperatureSV_Min;
            }

            Set(value);
        }
    }
    #endregion

    public PLC_Recipe(string name, string user, UserLevel level) : base(name, user, level)
    {
        RC1_Coatingoftimes = 1;
        RC1_CoatingSpeedSetting = 10;
        RC1_BoardClampingDistance = 10;
        RC1_Plugoftimes = 1;
        RC1_PanelThicknessSetting = 200;
        RC1_PanelWidthSetting = 200;
        RC1_CoatingPressureSetting = 100;
        RC1_LocationOfDrop = 10;
        RC1_SpeedOfDrop = 10;
        RC1_Blade_Pressure1 = 200;
        RC1_Blade_Pressure2 = 200;
        RC1_Blade_Pressure3 = 200;
        RC1_Blade_Pressure4 = 200;
        RC1_BakingTimeSetting = 20;
        RC1_TemperatureSV1 = 30;
        RC1_TemperatureSV2 = 30;
        RC1_CoatingPullSpeed = 10;
        RC2_Coatingoftimes = 1;
        RC2_CoatingSpeedSetting = 10;
        RC2_BoardClampingDistance = 10;
        RC2_Plugoftimes = 1;
        RC2_PanelThicknessSetting = 200;
        RC2_PanelWidthSetting = 200;
        RC2_CoatingPressureSetting = 100;
        RC2_LocationOfDrop = 10;
        RC2_SpeedOfDrop = 10;
        RC2_Blade_Pressure1 = 200;
        RC2_Blade_Pressure2 = 200;
        RC2_Blade_Pressure3 = 200;
        RC2_Blade_Pressure4 = 200;
        RC2_BakingTimeSetting = 20;
        RC2_TemperatureSV1 = 30;
        RC2_TemperatureSV2 = 30;
        RC2_CoatingPullSpeed = 10;
        RC3_Coatingoftimes = 1;
        RC3_CoatingSpeedSetting = 10;
        RC3_BoardClampingDistance = 10;
        RC3_Plugoftimes = 1;
        RC3_PanelThicknessSetting = 200;
        RC3_PanelWidthSetting = 200;
        RC3_CoatingPressureSetting = 100;
        RC3_LocationOfDrop = 10;
        RC3_SpeedOfDrop = 10;
        RC3_Blade_Pressure1 = 200;
        RC3_Blade_Pressure2 = 200;
        RC3_Blade_Pressure3 = 200;
        RC3_Blade_Pressure4 = 200;
        RC3_BakingTimeSetting = 20;
        RC3_TemperatureSV1 = 30;
        RC3_TemperatureSV2 = 30;
        RC3_TemperatureSV3 = 30;
        RC3_TemperatureSV4 = 30;
        RC3_TemperatureSV5 = 30;
        RC3_TemperatureSV6 = 30;
        RC3_TemperatureSV7 = 30;
        RC3_TemperatureSV8 = 30;
        RC3_CoatingPullSpeed = 10;
    }

    public PLC_Recipe() { }

    public override bool Equals(PLC_Recipe? other) => other != null &&
                                                      RecipeName == other.RecipeName &&
                                                      RC1_Coatingoftimes == other.RC1_Coatingoftimes &&
                                                      RC1_CoatingSpeedSetting.ToString("0.0") == other.RC1_CoatingSpeedSetting.ToString("0.0") &&
                                                      RC1_BoardClampingDistance.ToString("0.0") == other.RC1_BoardClampingDistance.ToString("0.0") &&
                                                      RC1_Plugoftimes.ToString("0.0") == other.RC1_Plugoftimes.ToString("0.0") &&
                                                      RC1_CoatingPressureSetting.ToString("0.0") == other.RC1_CoatingPressureSetting.ToString("0.0") &&
                                                      RC1_PanelThicknessSetting.ToString("0.0") == other.RC1_PanelThicknessSetting.ToString("0.0") &&
                                                      RC1_PanelWidthSetting.ToString("0.0") == other.RC1_PanelWidthSetting.ToString("0.0") &&
                                                      RC1_LocationOfDrop.ToString("0.0") == other.RC1_LocationOfDrop.ToString("0.0") &&
                                                      RC1_SpeedOfDrop.ToString("0.0") == other.RC1_SpeedOfDrop.ToString("0.0") &&
                                                      RC1_Blade_Pressure1.ToString("0.0") == other.RC1_Blade_Pressure1.ToString("0.0") &&
                                                      RC1_Blade_Pressure2.ToString("0.0") == other.RC1_Blade_Pressure2.ToString("0.0") &&
                                                      RC1_Blade_Pressure3.ToString("0.0") == other.RC1_Blade_Pressure3.ToString("0.0") &&
                                                      RC1_Blade_Pressure4.ToString("0.0") == other.RC1_Blade_Pressure4.ToString("0.0") &&
                                                      RC1_BakingTimeSetting.ToString("0.0") == other.RC1_BakingTimeSetting.ToString("0.0") &&
                                                      RC1_TemperatureSV1.ToString("0.0") == other.RC1_TemperatureSV1.ToString("0.0") &&
                                                      RC1_TemperatureSV2.ToString("0.0") == other.RC1_TemperatureSV2.ToString("0.0") &&
                                                      RC1_CoatingPullSpeed.ToString("0.0") == other.RC1_CoatingPullSpeed.ToString("0.0") &&
                                                      RC2_Coatingoftimes == other.RC2_Coatingoftimes &&
                                                      RC2_CoatingSpeedSetting.ToString("0.0") == other.RC2_CoatingSpeedSetting.ToString("0.0") &&
                                                      RC2_BoardClampingDistance.ToString("0.0") == other.RC2_BoardClampingDistance.ToString("0.0") &&
                                                      RC2_Plugoftimes.ToString("0.0") == other.RC2_Plugoftimes.ToString("0.0") &&
                                                      RC2_CoatingPressureSetting.ToString("0.0") == other.RC2_CoatingPressureSetting.ToString("0.0") &&
                                                      RC2_PanelThicknessSetting.ToString("0.0") == other.RC2_PanelThicknessSetting.ToString("0.0") &&
                                                      RC2_PanelWidthSetting.ToString("0.0") == other.RC2_PanelWidthSetting.ToString("0.0") &&
                                                      RC2_LocationOfDrop.ToString("0.0") == other.RC2_LocationOfDrop.ToString("0.0") &&
                                                      RC2_SpeedOfDrop.ToString("0.0") == other.RC2_SpeedOfDrop.ToString("0.0") &&
                                                      RC2_Blade_Pressure1.ToString("0.0") == other.RC2_Blade_Pressure1.ToString("0.0") &&
                                                      RC2_Blade_Pressure2.ToString("0.0") == other.RC2_Blade_Pressure2.ToString("0.0") &&
                                                      RC2_Blade_Pressure3.ToString("0.0") == other.RC2_Blade_Pressure3.ToString("0.0") &&
                                                      RC2_Blade_Pressure4.ToString("0.0") == other.RC2_Blade_Pressure4.ToString("0.0") &&
                                                      RC2_BakingTimeSetting.ToString("0.0") == other.RC2_BakingTimeSetting.ToString("0.0") &&
                                                      RC2_TemperatureSV1.ToString("0.0") == other.RC2_TemperatureSV1.ToString("0.0") &&
                                                      RC2_TemperatureSV2.ToString("0.0") == other.RC2_TemperatureSV2.ToString("0.0") &&
                                                      RC2_CoatingPullSpeed.ToString("0.0") == other.RC2_CoatingPullSpeed.ToString("0.0") &&
                                                      RC3_Coatingoftimes == other.RC3_Coatingoftimes &&
                                                      RC3_CoatingSpeedSetting.ToString("0.0") == other.RC3_CoatingSpeedSetting.ToString("0.0") &&
                                                      RC3_BoardClampingDistance.ToString("0.0") == other.RC3_BoardClampingDistance.ToString("0.0") &&
                                                      RC3_Plugoftimes.ToString("0.0") == other.RC3_Plugoftimes.ToString("0.0") &&
                                                      RC3_CoatingPressureSetting.ToString("0.0") == other.RC3_CoatingPressureSetting.ToString("0.0") &&
                                                      RC3_PanelThicknessSetting.ToString("0.0") == other.RC3_PanelThicknessSetting.ToString("0.0") &&
                                                      RC3_PanelWidthSetting.ToString("0.0") == other.RC3_PanelWidthSetting.ToString("0.0") &&
                                                      RC3_LocationOfDrop.ToString("0.0") == other.RC3_LocationOfDrop.ToString("0.0") &&
                                                      RC3_SpeedOfDrop.ToString("0.0") == other.RC3_SpeedOfDrop.ToString("0.0") &&
                                                      RC3_Blade_Pressure1.ToString("0.0") == other.RC3_Blade_Pressure1.ToString("0.0") &&
                                                      RC3_Blade_Pressure2.ToString("0.0") == other.RC3_Blade_Pressure2.ToString("0.0") &&
                                                      RC3_Blade_Pressure3.ToString("0.0") == other.RC3_Blade_Pressure3.ToString("0.0") &&
                                                      RC3_Blade_Pressure4.ToString("0.0") == other.RC3_Blade_Pressure4.ToString("0.0") &&
                                                      RC3_BakingTimeSetting.ToString("0.0") == other.RC3_BakingTimeSetting.ToString("0.0") &&
                                                      RC3_TemperatureSV1.ToString("0.0") == other.RC3_TemperatureSV1.ToString("0.0") &&
                                                      RC3_TemperatureSV2.ToString("0.0") == other.RC3_TemperatureSV2.ToString("0.0") &&
                                                      RC3_TemperatureSV3.ToString("0.0") == other.RC3_TemperatureSV3.ToString("0.0") &&
                                                      RC3_TemperatureSV4.ToString("0.0") == other.RC3_TemperatureSV4.ToString("0.0") &&
                                                      RC3_TemperatureSV5.ToString("0.0") == other.RC3_TemperatureSV5.ToString("0.0") &&
                                                      RC3_TemperatureSV6.ToString("0.0") == other.RC3_TemperatureSV6.ToString("0.0") &&
                                                      RC3_TemperatureSV7.ToString("0.0") == other.RC3_TemperatureSV7.ToString("0.0") &&
                                                      RC3_TemperatureSV8.ToString("0.0") == other.RC3_TemperatureSV8.ToString("0.0") &&
                                                      RC3_CoatingPullSpeed.ToString("0.0") == other.RC3_CoatingPullSpeed.ToString("0.0");

    public override PLC_Recipe Copy(string user, UserLevel level) => new()
    {
        Updated = DateTime.Now,
        RecipeName = RecipeName,
        RC1_Coatingoftimes = RC1_Coatingoftimes,
        RC1_CoatingSpeedSetting = RC1_CoatingSpeedSetting,
        RC1_BoardClampingDistance = RC1_BoardClampingDistance,
        RC1_Plugoftimes = RC1_Plugoftimes,
        RC1_PanelThicknessSetting = RC1_PanelThicknessSetting,
        RC1_CoatingPressureSetting = RC1_CoatingPressureSetting,
        RC1_LocationOfDrop = RC1_LocationOfDrop,
        RC1_SpeedOfDrop = RC1_SpeedOfDrop,
        RC1_PanelWidthSetting = RC1_PanelWidthSetting,
        RC1_CoatingPullSpeed = RC1_CoatingPullSpeed,
        RC1_Blade_Pressure1 = RC1_Blade_Pressure1,
        RC1_Blade_Pressure2 = RC1_Blade_Pressure2,
        RC1_Blade_Pressure3 = RC1_Blade_Pressure3,
        RC1_Blade_Pressure4 = RC1_Blade_Pressure4,
        RC1_BakingTimeSetting = RC1_BakingTimeSetting,
        RC1_TemperatureSV1 = RC1_TemperatureSV1,
        RC1_TemperatureSV2 = RC1_TemperatureSV2,
        RC2_Coatingoftimes = RC2_Coatingoftimes,
        RC2_CoatingPullSpeed = RC2_CoatingPullSpeed,
        RC2_CoatingSpeedSetting = RC2_CoatingSpeedSetting,
        RC2_BoardClampingDistance = RC2_BoardClampingDistance,
        RC2_Plugoftimes = RC2_Plugoftimes,
        RC2_PanelThicknessSetting = RC2_PanelThicknessSetting,
        RC2_PanelWidthSetting = RC2_PanelWidthSetting,
        RC2_CoatingPressureSetting = RC2_CoatingPressureSetting,
        RC2_SpeedOfDrop = RC2_SpeedOfDrop,
        RC2_LocationOfDrop = RC2_LocationOfDrop,
        RC2_Blade_Pressure1 = RC2_Blade_Pressure1,
        RC2_Blade_Pressure2 = RC2_Blade_Pressure2,
        RC2_Blade_Pressure3 = RC2_Blade_Pressure3,
        RC2_Blade_Pressure4 = RC2_Blade_Pressure4,
        RC2_BakingTimeSetting = RC2_BakingTimeSetting,
        RC2_TemperatureSV1 = RC2_TemperatureSV1,
        RC2_TemperatureSV2 = RC2_TemperatureSV2,
        RC3_Coatingoftimes = RC3_Coatingoftimes,
        RC3_CoatingPullSpeed = RC3_CoatingPullSpeed,
        RC3_CoatingSpeedSetting = RC3_CoatingSpeedSetting,
        RC3_BoardClampingDistance = RC3_BoardClampingDistance,
        RC3_Plugoftimes = RC3_Plugoftimes,
        RC3_PanelThicknessSetting = RC3_PanelThicknessSetting,
        RC3_PanelWidthSetting = RC3_PanelWidthSetting,
        RC3_CoatingPressureSetting = RC3_CoatingPressureSetting,
        RC3_SpeedOfDrop = RC3_SpeedOfDrop,
        RC3_LocationOfDrop = RC3_LocationOfDrop,
        RC3_Blade_Pressure1 = RC3_Blade_Pressure1,
        RC3_Blade_Pressure2 = RC3_Blade_Pressure2,
        RC3_Blade_Pressure3 = RC3_Blade_Pressure3,
        RC3_Blade_Pressure4 = RC3_Blade_Pressure4,
        RC3_BakingTimeSetting = RC3_BakingTimeSetting,
        RC3_TemperatureSV1 = RC3_TemperatureSV1,
        RC3_TemperatureSV2 = RC3_TemperatureSV2,
        RC3_TemperatureSV3 = RC3_TemperatureSV3,
        RC3_TemperatureSV4 = RC3_TemperatureSV4,
        RC3_TemperatureSV5 = RC3_TemperatureSV5,
        RC3_TemperatureSV6 = RC3_TemperatureSV6,
        RC3_TemperatureSV7 = RC3_TemperatureSV7,
        RC3_TemperatureSV8 = RC3_TemperatureSV8,
        Editor = user,
        EditorLevel = level
    };

    public override void CopyValue(string user, UserLevel level, PLC_Recipe recipe)
    {
        Updated = DateTime.Now;
        RC1_Coatingoftimes = recipe.RC1_Coatingoftimes;
        RC1_CoatingSpeedSetting = recipe.RC1_CoatingSpeedSetting;
        RC1_Plugoftimes = recipe.RC1_Plugoftimes;
        RC1_CoatingPressureSetting = recipe.RC1_CoatingPressureSetting;
        RC1_PanelThicknessSetting = recipe.RC1_PanelThicknessSetting;
        RC1_PanelWidthSetting = recipe.RC1_PanelWidthSetting;
        RC1_LocationOfDrop = recipe.RC1_LocationOfDrop;
        RC1_SpeedOfDrop = recipe.RC1_SpeedOfDrop;
        RC1_Blade_Pressure1 = recipe.RC1_Blade_Pressure1;
        RC1_Blade_Pressure2 = recipe.RC1_Blade_Pressure2;
        RC1_Blade_Pressure3 = recipe.RC1_Blade_Pressure3;
        RC1_Blade_Pressure4 = recipe.RC1_Blade_Pressure4;
        RC1_CoatingPullSpeed = recipe.RC1_CoatingPullSpeed;
        RC1_BakingTimeSetting = recipe.RC1_BakingTimeSetting;
        RC1_TemperatureSV1 = recipe.RC1_TemperatureSV1;
        RC1_TemperatureSV2 = recipe.RC1_TemperatureSV2;
        RC2_Coatingoftimes = recipe.RC2_Coatingoftimes;
        RC2_CoatingSpeedSetting = recipe.RC2_CoatingSpeedSetting;
        RC2_Plugoftimes = recipe.RC2_Plugoftimes;
        RC2_CoatingPressureSetting = recipe.RC2_CoatingPressureSetting;
        RC2_PanelThicknessSetting = recipe.RC2_PanelThicknessSetting;
        RC2_PanelWidthSetting = recipe.RC2_PanelWidthSetting;
        RC2_LocationOfDrop = recipe.RC2_LocationOfDrop;
        RC2_SpeedOfDrop = recipe.RC2_SpeedOfDrop;
        RC2_Blade_Pressure1 = recipe.RC2_Blade_Pressure1;
        RC2_Blade_Pressure2 = recipe.RC2_Blade_Pressure2;
        RC2_Blade_Pressure3 = recipe.RC2_Blade_Pressure3;
        RC2_Blade_Pressure4 = recipe.RC2_Blade_Pressure4;
        RC2_CoatingPullSpeed = recipe.RC2_CoatingPullSpeed;
        RC2_BakingTimeSetting = recipe.RC2_BakingTimeSetting;
        RC2_TemperatureSV1 = recipe.RC2_TemperatureSV1;
        RC2_TemperatureSV2 = recipe.RC2_TemperatureSV2;
        RC3_Coatingoftimes = recipe.RC3_Coatingoftimes;
        RC3_CoatingSpeedSetting = recipe.RC3_CoatingSpeedSetting;
        RC3_Plugoftimes = recipe.RC3_Plugoftimes;
        RC3_CoatingPressureSetting = recipe.RC3_CoatingPressureSetting;
        RC3_PanelThicknessSetting = recipe.RC3_PanelThicknessSetting;
        RC3_PanelWidthSetting = recipe.RC3_PanelWidthSetting;
        RC3_LocationOfDrop = recipe.RC3_LocationOfDrop;
        RC3_SpeedOfDrop = recipe.RC3_SpeedOfDrop;
        RC3_Blade_Pressure1 = recipe.RC3_Blade_Pressure1;
        RC3_Blade_Pressure2 = recipe.RC3_Blade_Pressure2;
        RC3_Blade_Pressure3 = recipe.RC3_Blade_Pressure3;
        RC3_Blade_Pressure4 = recipe.RC3_Blade_Pressure4;
        RC3_CoatingPullSpeed = recipe.RC3_CoatingPullSpeed;
        RC3_BakingTimeSetting = recipe.RC3_BakingTimeSetting;
        RC3_TemperatureSV1 = recipe.RC3_TemperatureSV1;
        RC3_TemperatureSV2 = recipe.RC3_TemperatureSV2;
        RC3_TemperatureSV3 = recipe.RC3_TemperatureSV3;
        RC3_TemperatureSV4 = recipe.RC3_TemperatureSV4;
        RC3_TemperatureSV5 = recipe.RC3_TemperatureSV5;
        RC3_TemperatureSV6 = recipe.RC3_TemperatureSV6;
        RC3_TemperatureSV7 = recipe.RC3_TemperatureSV7;
        RC3_TemperatureSV8 = recipe.RC3_TemperatureSV8;
        Editor = user;
        EditorLevel = level;
    }

    public Dictionary<string, object> ToDictionary()
    {

        return new()
        {
            { nameof(RecipeName), RecipeName },
            { nameof(RC1_Coatingoftimes), RC1_Coatingoftimes },
            { nameof(RC1_CoatingSpeedSetting), RC1_CoatingSpeedSetting },
            { nameof(RC1_BoardClampingDistance), RC1_BoardClampingDistance },
            { nameof(RC1_Plugoftimes), RC1_Plugoftimes },
            { nameof(RC1_CoatingPressureSetting), RC1_CoatingPressureSetting },
            { nameof(RC1_PanelThicknessSetting), RC1_PanelThicknessSetting },
            { nameof(RC1_PanelWidthSetting), RC1_PanelWidthSetting },
            { nameof(RC1_LocationOfDrop), RC1_LocationOfDrop },
            { nameof(RC1_SpeedOfDrop), RC1_SpeedOfDrop },
            { nameof(RC1_CoatingPullSpeed), RC1_CoatingPullSpeed },
            { nameof(RC1_Blade_Pressure1), RC1_Blade_Pressure1 },
            { nameof(RC1_Blade_Pressure2), RC1_Blade_Pressure2 },
            { nameof(RC1_Blade_Pressure3), RC1_Blade_Pressure3 },
            { nameof(RC1_Blade_Pressure4), RC1_Blade_Pressure4 },
            { nameof(RC1_BakingTimeSetting), RC1_BakingTimeSetting },
            { nameof(RC1_TemperatureSV1), RC1_TemperatureSV1 },
            { nameof(RC1_TemperatureSV2), RC1_TemperatureSV2 },
            { nameof(RC2_Coatingoftimes), RC2_Coatingoftimes },
            { nameof(RC2_CoatingSpeedSetting), RC2_CoatingSpeedSetting },
            { nameof(RC2_BoardClampingDistance), RC2_BoardClampingDistance },
            { nameof(RC2_Plugoftimes), RC2_Plugoftimes },
            { nameof(RC2_CoatingPressureSetting), RC2_CoatingPressureSetting },
            { nameof(RC2_PanelThicknessSetting), RC2_PanelThicknessSetting },
            { nameof(RC2_PanelWidthSetting), RC2_PanelWidthSetting },
            { nameof(RC2_LocationOfDrop), RC2_LocationOfDrop },
            { nameof(RC2_SpeedOfDrop), RC2_SpeedOfDrop },
            { nameof(RC2_CoatingPullSpeed), RC2_CoatingPullSpeed },
            { nameof(RC2_Blade_Pressure1), RC2_Blade_Pressure1 },
            { nameof(RC2_Blade_Pressure2), RC2_Blade_Pressure2 },
            { nameof(RC2_Blade_Pressure3), RC2_Blade_Pressure3 },
            { nameof(RC2_Blade_Pressure4), RC2_Blade_Pressure4 },
            { nameof(RC2_BakingTimeSetting), RC2_BakingTimeSetting },
            { nameof(RC2_TemperatureSV1), RC2_TemperatureSV1 },
            { nameof(RC2_TemperatureSV2), RC2_TemperatureSV2 },
            { nameof(RC3_Coatingoftimes), RC3_Coatingoftimes },
            { nameof(RC3_CoatingSpeedSetting), RC3_CoatingSpeedSetting },
            { nameof(RC3_BoardClampingDistance), RC3_BoardClampingDistance },
            { nameof(RC3_Plugoftimes), RC3_Plugoftimes },
            { nameof(RC3_CoatingPressureSetting), RC3_CoatingPressureSetting },
            { nameof(RC3_PanelThicknessSetting), RC3_PanelThicknessSetting },
            { nameof(RC3_PanelWidthSetting), RC3_PanelWidthSetting },
            { nameof(RC3_LocationOfDrop), RC3_LocationOfDrop },
            { nameof(RC3_SpeedOfDrop), RC3_SpeedOfDrop },
            { nameof(RC3_CoatingPullSpeed), RC3_CoatingPullSpeed },
            { nameof(RC3_Blade_Pressure1), RC3_Blade_Pressure1 },
            { nameof(RC3_Blade_Pressure2), RC3_Blade_Pressure2 },
            { nameof(RC3_Blade_Pressure3), RC3_Blade_Pressure3 },
            { nameof(RC3_Blade_Pressure4), RC3_Blade_Pressure4 },
            { nameof(RC3_BakingTimeSetting), RC3_BakingTimeSetting },
            { nameof(RC3_TemperatureSV1), RC3_TemperatureSV1 },
            { nameof(RC3_TemperatureSV2), RC3_TemperatureSV2 },
            { nameof(RC3_TemperatureSV3), RC3_TemperatureSV3 },
            { nameof(RC3_TemperatureSV4), RC3_TemperatureSV4 },
            { nameof(RC3_TemperatureSV5), RC3_TemperatureSV5 },
            { nameof(RC3_TemperatureSV6), RC3_TemperatureSV6 },
            { nameof(RC3_TemperatureSV7), RC3_TemperatureSV7 },
            { nameof(RC3_TemperatureSV8), RC3_TemperatureSV8 },
        };
    }
    public Dictionary<string, object> ToDictionary(int i) => i switch
    {
        0 => new Dictionary<string, object>
        {
            { "RecipeName", RecipeName },
            { "Coatingoftimes", RC1_Coatingoftimes },
            { "CoatingSpeedSetting", RC1_CoatingSpeedSetting },
            { "BoardClampingDistance", RC1_BoardClampingDistance },
            { "Plugoftimes", RC1_Plugoftimes },
            { "CoatingPressureSetting", RC1_CoatingPressureSetting },
            { "PanelThicknessSetting", RC1_PanelThicknessSetting },
            { "PanelWidthSetting", RC1_PanelWidthSetting },
            { "LocationOfDrop", RC1_LocationOfDrop },
            { "SpeedOfDrop", RC1_SpeedOfDrop },
            { "CoatingPullSpeed", RC1_CoatingPullSpeed },
            { "Blade_Pressure1", RC1_Blade_Pressure1 },
            { "Blade_Pressure2", RC1_Blade_Pressure2 },
            { "Blade_Pressure3", RC1_Blade_Pressure3 },
            { "Blade_Pressure4", RC1_Blade_Pressure4 },
            { "BakingTimeSetting", RC1_BakingTimeSetting },
            { "TemperatureSV1", RC1_TemperatureSV1 },
            { "TemperatureSV2", RC1_TemperatureSV2 },
        },
        1 => new Dictionary<string, object>
        {
              { "RecipeName", RecipeName },
              { "Coatingoftimes", RC2_Coatingoftimes },
              { "CoatingSpeedSetting", RC2_CoatingSpeedSetting },
              { "BoardClampingDistance", RC2_BoardClampingDistance },
              { "Plugoftimes", RC2_Plugoftimes },
              { "CoatingPressureSetting", RC2_CoatingPressureSetting },
              { "PanelThicknessSetting", RC2_PanelThicknessSetting },
              { "PanelWidthSetting", RC2_PanelWidthSetting },
              { "LocationOfDrop", RC2_LocationOfDrop },
              { "SpeedOfDrop", RC2_SpeedOfDrop },
              { "CoatingPullSpeed", RC2_CoatingPullSpeed },
              { "Blade_Pressure1", RC2_Blade_Pressure1 },
              { "Blade_Pressure2", RC2_Blade_Pressure2 },
              { "Blade_Pressure3", RC2_Blade_Pressure3 },
              { "Blade_Pressure4", RC2_Blade_Pressure4 },
              { "BakingTimeSetting", RC2_BakingTimeSetting },
              { "TemperatureSV1", RC2_TemperatureSV1 },
              { "TemperatureSV2", RC2_TemperatureSV2 },
        },
        2 => new Dictionary<string, object>
        {
              { "RecipeName", RecipeName },
              { "Coatingoftimes", RC3_Coatingoftimes },
              { "CoatingSpeedSetting", RC3_CoatingSpeedSetting },
              { "BoardClampingDistance", RC3_BoardClampingDistance },
              { "Plugoftimes", RC3_Plugoftimes },
              { "CoatingPressureSetting", RC3_CoatingPressureSetting },
              { "PanelThicknessSetting", RC3_PanelThicknessSetting },
              { "PanelWidthSetting", RC3_PanelWidthSetting },
              { "LocationOfDrop", RC3_LocationOfDrop },
              { "SpeedOfDrop", RC3_SpeedOfDrop },
              { "CoatingPullSpeed", RC3_CoatingPullSpeed },
              { "Blade_Pressure1", RC3_Blade_Pressure1 },
              { "Blade_Pressure2", RC3_Blade_Pressure2 },
              { "Blade_Pressure3", RC3_Blade_Pressure3 },
              { "Blade_Pressure4", RC3_Blade_Pressure4 },
              { "BakingTimeSetting", RC3_BakingTimeSetting },
              { "RC3_TemperatureSV1", RC3_TemperatureSV1 },
              { "RC3_TemperatureSV2", RC3_TemperatureSV2 },
              { "RC3_TemperatureSV3", RC3_TemperatureSV3 },
              { "RC3_TemperatureSV4", RC3_TemperatureSV4 },
              { "RC3_TemperatureSV5", RC3_TemperatureSV5 },
              { "RC3_TemperatureSV6", RC3_TemperatureSV6 },
              { "RC3_TemperatureSV7", RC3_TemperatureSV7 },
              { "RC3_TemperatureSV8", RC3_TemperatureSV8 },

        },
        _ => throw new ArgumentException("不支援的 plcindex 值"),
    };

    public Dictionary<string, object> ToShowDictionary() => new()
                                                        {
                                                            { "配方", RecipeName },
                                                            { "RC1_塗佈次數", RC1_Coatingoftimes },
                                                            { "RC1_塗佈速度設定", RC1_CoatingSpeedSetting },
                                                            { "RC1_板面夾持距離", RC1_BoardClampingDistance },
                                                            { "RC1_塞孔次數", RC1_Plugoftimes },
                                                            { "RC1_塗佈壓力設定", RC1_CoatingPressureSetting },
                                                            { "RC1_基板厚度設定", RC1_PanelThicknessSetting },
                                                            { "RC1_基板寬度設定", RC1_PanelWidthSetting },
                                                            { "RC1_入料下降位置設定", RC1_LocationOfDrop },
                                                            { "RC1_入料下降速度設定", RC1_SpeedOfDrop },
                                                            { "RC1_拉料速度比", RC1_CoatingPullSpeed },
                                                            { "RC1_刮刀壓力設定1", RC1_Blade_Pressure1 },
                                                            { "RC1_刮刀壓力設定2", RC1_Blade_Pressure2 },
                                                            { "RC1_刮刀壓力設定3", RC1_Blade_Pressure3 },
                                                            { "RC1_刮刀壓力設定4", RC1_Blade_Pressure4 },
                                                            { "RC1_烘烤時間設定", RC1_BakingTimeSetting },
                                                            { "RC1_第1段溫度設定值", RC1_TemperatureSV1 },
                                                            { "RC1_第2段溫度設定值", RC1_TemperatureSV2 },

                                                            { "RC2_塗佈次數", RC2_Coatingoftimes },
                                                            { "RC2_塗佈速度設定", RC2_CoatingSpeedSetting },
                                                            { "RC2_板面夾持距離", RC2_BoardClampingDistance },
                                                            { "RC2_塞孔次數", RC2_Plugoftimes },
                                                            { "RC2_塗佈壓力設定", RC2_CoatingPressureSetting },
                                                            { "RC2_基板厚度設定", RC2_PanelThicknessSetting },
                                                            { "RC2_基板寬度設定", RC2_PanelWidthSetting },
                                                            { "RC2_入料下降位置設定", RC2_LocationOfDrop },
                                                            { "RC2_入料下降速度設定", RC2_SpeedOfDrop },
                                                            { "RC2_拉料速度比", RC2_CoatingPullSpeed },
                                                            { "RC2_刮刀壓力設定1", RC2_Blade_Pressure1 },
                                                            { "RC2_刮刀壓力設定2", RC2_Blade_Pressure2 },
                                                            { "RC2_刮刀壓力設定3", RC2_Blade_Pressure3 },
                                                            { "RC2_刮刀壓力設定4", RC2_Blade_Pressure4 },
                                                            { "RC2_烘烤時間設定", RC2_BakingTimeSetting },
                                                            { "RC2_第1段溫度設定值", RC2_TemperatureSV1 },
                                                            { "RC2_第2段溫度設定值", RC2_TemperatureSV2 },

                                                            { "RC3_塗佈次數", RC3_Coatingoftimes },
                                                            { "RC3_塗佈速度設定", RC3_CoatingSpeedSetting },
                                                            { "RC3_板面夾持距離", RC3_BoardClampingDistance },
                                                            { "RC3_塞孔次數", RC3_Plugoftimes },
                                                            { "RC3_塗佈壓力設定", RC3_CoatingPressureSetting },
                                                            { "RC3_基板厚度設定", RC3_PanelThicknessSetting },
                                                            { "RC3_基板寬度設定", RC3_PanelWidthSetting },
                                                            { "RC3_入料下降位置設定", RC3_LocationOfDrop },
                                                            { "RC3_入料下降速度設定", RC3_SpeedOfDrop },
                                                            { "RC3_拉料速度比", RC3_CoatingPullSpeed },
                                                            { "RC3_刮刀壓力設定1", RC3_Blade_Pressure1 },
                                                            { "RC3_刮刀壓力設定2", RC3_Blade_Pressure2 },
                                                            { "RC3_刮刀壓力設定3", RC3_Blade_Pressure3 },
                                                            { "RC3_刮刀壓力設定4", RC3_Blade_Pressure4 },
                                                            { "RC3_烘烤時間設定", RC3_BakingTimeSetting },
                                                            { "RC3_第1段溫度設定值", RC3_TemperatureSV1 },
                                                            { "RC3_第2段溫度設定值", RC3_TemperatureSV2 },
                                                            { "RC3_第3段溫度設定值", RC3_TemperatureSV3 },
                                                            { "RC3_第4段溫度設定值", RC3_TemperatureSV4 },
                                                            { "RC3_第5段溫度設定值", RC3_TemperatureSV5 },
                                                            { "RC3_第6段溫度設定值", RC3_TemperatureSV6 },
                                                            { "RC3_第7段溫度設定值", RC3_TemperatureSV7 },
                                                            { "RC3_第8段溫度設定值", RC3_TemperatureSV8 },
                                                            };
    public bool SetByDictionary(Dictionary<string, string> dic)
    {
        var type = GetType();

        foreach (var kv in dic)
        {
            var p = type.GetProperty(kv.Key);
            if (p != null && p.CanWrite)
            {
                if (p.PropertyType == typeof(bool) && bool.TryParse(kv.Value, out var bo))
                {
                    p.SetValue(this, bo);

                    if ((bool)p.GetValue(this) != bo)
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(byte) && byte.TryParse(kv.Value, out var b))
                {
                    p.SetValue(this, b);

                    if ((byte)p.GetValue(this) != b)
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(short) && short.TryParse(kv.Value, out var s))
                {
                    p.SetValue(this, s);

                    if ((short)p.GetValue(this) != s)
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(int) && int.TryParse(kv.Value, out var i))
                {
                    p.SetValue(this, i);

                    if ((int)p.GetValue(this) != i)
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(float) && float.TryParse(kv.Value, out var f))
                {
                    p.SetValue(this, f);

                    if (((float)p.GetValue(this)).ToString("0.0") != f.ToString("0.0"))
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(double) && double.TryParse(kv.Value, out var d))
                {
                    p.SetValue(this, d);

                    if (((double)p.GetValue(this)).ToString("0.0") != d.ToString("0.0"))
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(string))
                {
                    p.SetValue(this, kv.Value);

                    if ((string)p.GetValue(this) != kv.Value)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public bool SetByDictionary(Dictionary<string, object> dic)
    {
        var type = GetType();

        foreach (var kv in dic)
        {
            var p = type.GetProperty(kv.Key);
            if (p != null && p.CanWrite)
            {
                if (p.PropertyType == typeof(bool) && kv.Value is bool bo)
                {
                    p.SetValue(this, bo);

                    if ((bool)p.GetValue(this) != bo)
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(byte) && kv.Value is byte b)
                {
                    p.SetValue(this, b);

                    if ((byte)p.GetValue(this) != b)
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(short) && kv.Value is short s)
                {
                    p.SetValue(this, s);

                    if ((short)p.GetValue(this) != s)
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(int) && kv.Value is int i)
                {
                    p.SetValue(this, i);

                    if ((int)p.GetValue(this) != i)
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(float) && kv.Value is float f)
                {
                    p.SetValue(this, f);

                    if (((float)p.GetValue(this)).ToString("0.0") != f.ToString("0.0"))
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(double) && kv.Value is double d)
                {
                    p.SetValue(this, d);

                    if (((double)p.GetValue(this)).ToString("0.0") != d.ToString("0.0"))
                    {
                        return false;
                    }
                }
                else if (p.PropertyType == typeof(string) && kv.Value is string str)
                {
                    p.SetValue(this, str);

                    if ((string)p.GetValue(this) != str)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}