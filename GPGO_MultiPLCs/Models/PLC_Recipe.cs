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

    [JsonIgnore]
    public double RC1_Coatingoftimes_Max => 100;

    [JsonIgnore]
    public double RC1_Coatingoftimes_Min => 1;

    [JsonIgnore]
    public double RC1_CoatingSpeedSetting_Max => 999.9;

    [JsonIgnore]
    public double RC1_CoatingSpeedSetting_Min => 0.0;
    [JsonIgnore]
    public double RC1_BoardClampingDistance_Max => 999.9;

    [JsonIgnore]
    public double RC1_BoardClampingDistance_Min => 0.0;
    [JsonIgnore]
    public double RC1_Plugoftimes_Max => 999.9;

    [JsonIgnore]
    public double RC1_Plugoftimes_Min => 0.0;

    [JsonIgnore]
    public double RC1_CoatingPressureSetting_Max => 999.9;

    [JsonIgnore]
    public double RC1_CoatingPressureSetting_Min => 0.0;
    [JsonIgnore]
    public double RC1_PanelThicknessSetting_Max => 999.9;

    [JsonIgnore]
    public double RC1_PanelThicknessSetting_Min => 0.0;
    [JsonIgnore]
    public double RC1_LocationOfDrop_Max => 999.9;

    [JsonIgnore]
    public double RC1_LocationOfDrop_Min => 0.0;
    [JsonIgnore]
    public double RC1_Blade_Pressure_Max => 999.9;

    [JsonIgnore]
    public double RC1_Blade_Pressure_Min => 0.0;
    [JsonIgnore]
    public double RC1_D_BarPressureSetting1_Max => 999.9;

    [JsonIgnore]
    public double RC1_D_BarPressureSetting1_Min => 0.0;
    [JsonIgnore]
    public double RC1_D_BarPressureSetting2_Max => 999.9;

    [JsonIgnore]
    public double RC1_D_BarPressureSetting2_Min => 0.0;
    [JsonIgnore]
    public double RC1_D_BarPressureSetting3_Max => 999.9;

    [JsonIgnore]
    public double RC1_D_BarPressureSetting3_Min => 0.0;
    [JsonIgnore]
    public double RC1_D_BarPressureSetting4_Max => 999.9;

    [JsonIgnore]
    public double RC1_D_BarPressureSetting4_Min => 0.0;
    [JsonIgnore]
    public double RC1_BakingTimeSetting_Max => 999.9;

    [JsonIgnore]
    public double RC1_BakingTimeSetting_Min => 0.0;
    [JsonIgnore]
    public double RC1_TemperatureSV1_Max => 999.9;

    [JsonIgnore]
    public double RC1_TemperatureSV1_Min => 0.0;
    [JsonIgnore]
    public double RC1_TemperatureSV2_Max => 999.9;

    [JsonIgnore]
    public double RC1_TemperatureSV2_Min => 0.0;
    [JsonIgnore]
    public double RC1_UseCoating_Max => 999.9;

    [JsonIgnore]
    public double RC1_UseCoating_Min => 0.0;

    [JsonIgnore]
    public double RC1_UsePlug_Max => 999.9;

    [JsonIgnore]
    public double RC1_UsePlug_Min => 0.0;

    [JsonIgnore]
    public double RC1_StandardInk_Max => 999.9;

    [JsonIgnore]
    public double RC1_StandardInk_Min => 0.0;

    [JsonIgnore]
    public double RC1_DifferenceOfInk_Max => 999.9;

    [JsonIgnore]
    public double RC1_DifferenceOfInk_Min => 0.0;
    [JsonIgnore]
    public double Coatingoftimes_Max => 100;

    [JsonIgnore]
    public double Coatingoftimes_Min => 1;

    [JsonIgnore]
    public double CoatingSpeedSetting_Max => 999.9;

    [JsonIgnore]
    public double CoatingSpeedSetting_Min => 0.0;
    [JsonIgnore]
    public double BoardClampingDistance_Max => 999.9;

    [JsonIgnore]
    public double BoardClampingDistance_Min => 0.0;
    [JsonIgnore]
    public double Plugoftimes_Max => 999.9;

    [JsonIgnore]
    public double Plugoftimes_Min => 0.0;

    [JsonIgnore]
    public double CoatingPressureSetting_Max => 999.9;

    [JsonIgnore]
    public double CoatingPressureSetting_Min => 0.0;
    [JsonIgnore]
    public double PanelThicknessSetting_Max => 999.9;

    [JsonIgnore]
    public double PanelThicknessSetting_Min => 0.0;
    [JsonIgnore]
    public double LocationOfDrop_Max => 999.9;

    [JsonIgnore]
    public double LocationOfDrop_Min => 0.0;
    [JsonIgnore]
    public double Blade_Pressure_Max => 999.9;

    [JsonIgnore]
    public double Blade_Pressure_Min => 0.0;
    [JsonIgnore]
    public double D_BarPressureSetting1_Max => 999.9;

    [JsonIgnore]
    public double D_BarPressureSetting1_Min => 0.0;
    [JsonIgnore]
    public double D_BarPressureSetting2_Max => 999.9;

    [JsonIgnore]
    public double D_BarPressureSetting2_Min => 0.0;
    [JsonIgnore]
    public double D_BarPressureSetting3_Max => 999.9;

    [JsonIgnore]
    public double D_BarPressureSetting3_Min => 0.0;
    [JsonIgnore]
    public double D_BarPressureSetting4_Max => 999.9;

    [JsonIgnore]
    public double D_BarPressureSetting4_Min => 0.0;
    [JsonIgnore]
    public double BakingTimeSetting_Max => 999.9;

    [JsonIgnore]
    public double BakingTimeSetting_Min => 0.0;
    [JsonIgnore]
    public double TemperatureSV1_Max => 999.9;

    [JsonIgnore]
    public double TemperatureSV1_Min => 0.0;
    [JsonIgnore]
    public double TemperatureSV2_Max => 999.9;

    [JsonIgnore]
    public double TemperatureSV2_Min => 0.0;
    [JsonIgnore]
    public double UseCoating_Max => 999.9;

    [JsonIgnore]
    public double UseCoating_Min => 0.0;

    [JsonIgnore]
    public double UsePlug_Max => 999.9;

    [JsonIgnore]
    public double UsePlug_Min => 0.0;

    [JsonIgnore]
    public double StandardInk_Max => 999.9;

    [JsonIgnore]
    public double StandardInk_Min => 0.0;

    [JsonIgnore]
    public double DifferenceOfInk_Max => 999.9;

    [JsonIgnore]
    public double DifferenceOfInk_Min => 0.0;
    [JsonIgnore]
    public double RC2_Coatingoftimes_Max => 100;

    [JsonIgnore]
    public double RC2_Coatingoftimes_Min => 1;

    [JsonIgnore]
    public double RC2_CoatingSpeedSetting_Max => 999.9;

    [JsonIgnore]
    public double RC2_CoatingSpeedSetting_Min => 0.0;
    [JsonIgnore]
    public double RC2_BoardClampingDistance_Max => 999.9;

    [JsonIgnore]
    public double RC2_BoardClampingDistance_Min => 0.0;
    [JsonIgnore]
    public double RC2_Plugoftimes_Max => 999.9;

    [JsonIgnore]
    public double RC2_Plugoftimes_Min => 0.0;

    [JsonIgnore]
    public double RC2_CoatingPressureSetting_Max => 999.9;

    [JsonIgnore]
    public double RC2_CoatingPressureSetting_Min => 0.0;
    [JsonIgnore]
    public double RC2_PanelThicknessSetting_Max => 999.9;

    [JsonIgnore]
    public double RC2_PanelThicknessSetting_Min => 0.0;
    [JsonIgnore]
    public double RC2_LocationOfDrop_Max => 999.9;

    [JsonIgnore]
    public double RC2_LocationOfDrop_Min => 0.0;
    [JsonIgnore]
    public double RC2_Blade_Pressure_Max => 999.9;

    [JsonIgnore]
    public double RC2_Blade_Pressure_Min => 0.0;
    [JsonIgnore]
    public double RC2_D_BarPressureSetting1_Max => 999.9;

    [JsonIgnore]
    public double RC2_D_BarPressureSetting1_Min => 0.0;
    [JsonIgnore]
    public double RC2_D_BarPressureSetting2_Max => 999.9;

    [JsonIgnore]
    public double RC2_D_BarPressureSetting2_Min => 0.0;
    [JsonIgnore]
    public double RC2_D_BarPressureSetting3_Max => 999.9;

    [JsonIgnore]
    public double RC2_D_BarPressureSetting3_Min => 0.0;
    [JsonIgnore]
    public double RC2_D_BarPressureSetting4_Max => 999.9;

    [JsonIgnore]
    public double RC2_D_BarPressureSetting4_Min => 0.0;
    [JsonIgnore]
    public double RC2_BakingTimeSetting_Max => 999.9;

    [JsonIgnore]
    public double RC2_BakingTimeSetting_Min => 0.0;
    [JsonIgnore]
    public double RC2_TemperatureSV1_Max => 999.9;

    [JsonIgnore]
    public double RC2_TemperatureSV1_Min => 0.0;
    [JsonIgnore]
    public double RC2_TemperatureSV2_Max => 999.9;

    [JsonIgnore]
    public double RC2_TemperatureSV2_Min => 0.0;
    [JsonIgnore]
    public double RC2_UseCoating_Max => 999.9;

    [JsonIgnore]
    public double RC2_UseCoating_Min => 0.0;

    [JsonIgnore]
    public double RC2_UsePlug_Max => 999.9;

    [JsonIgnore]
    public double RC2_UsePlug_Min => 0.0;

    [JsonIgnore]
    public double RC2_StandardInk_Max => 999.9;

    [JsonIgnore]
    public double RC2_StandardInk_Min => 0.0;

    [JsonIgnore]
    public double RC2_DifferenceOfInk_Max => 999.9;

    [JsonIgnore]
    public double RC2_DifferenceOfInk_Min => 0.0;
    [JsonIgnore]
    public double RC3_Coatingoftimes_Max => 100;

    [JsonIgnore]
    public double RC3_Coatingoftimes_Min => 1;

    [JsonIgnore]
    public double RC3_CoatingSpeedSetting_Max => 999.9;

    [JsonIgnore]
    public double RC3_CoatingSpeedSetting_Min => 0.0;
    [JsonIgnore]
    public double RC3_BoardClampingDistance_Max => 999.9;

    [JsonIgnore]
    public double RC3_BoardClampingDistance_Min => 0.0;
    [JsonIgnore]
    public double RC3_Plugoftimes_Max => 999.9;

    [JsonIgnore]
    public double RC3_Plugoftimes_Min => 0.0;

    [JsonIgnore]
    public double RC3_CoatingPressureSetting_Max => 999.9;

    [JsonIgnore]
    public double RC3_CoatingPressureSetting_Min => 0.0;
    [JsonIgnore]
    public double RC3_PanelThicknessSetting_Max => 999.9;

    [JsonIgnore]
    public double RC3_PanelThicknessSetting_Min => 0.0;
    [JsonIgnore]
    public double RC3_LocationOfDrop_Max => 999.9;

    [JsonIgnore]
    public double RC3_LocationOfDrop_Min => 0.0;
    [JsonIgnore]
    public double RC3_Blade_Pressure_Max => 999.9;

    [JsonIgnore]
    public double RC3_Blade_Pressure_Min => 0.0;
    [JsonIgnore]
    public double RC3_D_BarPressureSetting1_Max => 999.9;

    [JsonIgnore]
    public double RC3_D_BarPressureSetting1_Min => 0.0;
    [JsonIgnore]
    public double RC3_D_BarPressureSetting2_Max => 999.9;

    [JsonIgnore]
    public double RC3_D_BarPressureSetting2_Min => 0.0;
    [JsonIgnore]
    public double RC3_D_BarPressureSetting3_Max => 999.9;

    [JsonIgnore]
    public double RC3_D_BarPressureSetting3_Min => 0.0;
    [JsonIgnore]
    public double RC3_D_BarPressureSetting4_Max => 999.9;

    [JsonIgnore]
    public double RC3_D_BarPressureSetting4_Min => 0.0;
    [JsonIgnore]
    public double RC3_BakingTimeSetting_Max => 999.9;

    [JsonIgnore]
    public double RC3_BakingTimeSetting_Min => 0.0;
    [JsonIgnore]
    public double RC3_TemperatureSV1_Max => 999.9;

    [JsonIgnore]
    public double RC3_TemperatureSV1_Min => 0.0;
    [JsonIgnore]
    public double RC3_TemperatureSV2_Max => 999.9;

    [JsonIgnore]
    public double RC3_TemperatureSV2_Min => 0.0;
    [JsonIgnore]
    public double RC3_UseCoating_Max => 999.9;

    [JsonIgnore]
    public double RC3_UseCoating_Min => 0.0;

    [JsonIgnore]
    public double RC3_UsePlug_Max => 999.9;

    [JsonIgnore]
    public double RC3_UsePlug_Min => 0.0;

    [JsonIgnore]
    public double RC3_StandardInk_Max => 999.9;

    [JsonIgnore]
    public double RC3_StandardInk_Min => 0.0;

    [JsonIgnore]
    public double RC3_DifferenceOfInk_Max => 999.9;

    [JsonIgnore]
    public double RC3_DifferenceOfInk_Min => 0.0;

    [JsonIgnore] //匯出時檔名已是名稱，此處用JsonIgnore是用來做配方比較需忽略此項
    [OrderIndex(-1)]
    [LanguageTranslator("Recipe Name", "配方名稱", "配方名称")]
    public override string RecipeName { get; set; } = string.Empty;

    [JsonIgnore]
    [OrderIndex(0)]
    [LanguageTranslator("Updated Time", "更新時間", "更新时间")]
    public override DateTime Updated { get; set; }

    [JsonIgnore]
    [OrderIndex(1)]
    [LanguageTranslator("Editor", "修改者", "修改者")]
    public override string Editor { get; set; } = string.Empty;

    [JsonIgnore]
    [OrderIndex(2)]
    [LanguageTranslator("Editor Level", "權限", "权限")]
    public override UserLevel EditorLevel { get; set; }

    [OrderIndex(3)]
    [LanguageTranslator("Coatingoftimes", "塗佈次數", "塗佈次數")]
    public double RC1_Coatingoftimes
    {
        get => Get<double>();
        set
        {
            if (value > RC1_Coatingoftimes_Max)
            {
                value = RC1_Coatingoftimes_Max;
            }
            else if (value < RC1_Coatingoftimes_Min)
            {
                value = RC1_Coatingoftimes_Min;
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

            if (value > RC1_CoatingSpeedSetting_Max)
            {
                value = RC1_CoatingSpeedSetting_Max;
            }
            else if (value < RC1_CoatingSpeedSetting_Min)
            {
                value = RC1_CoatingSpeedSetting_Min;
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

            if (value > RC1_BoardClampingDistance_Max)
            {
                value = RC1_BoardClampingDistance_Max;
            }
            else if (value < RC1_BoardClampingDistance_Min)
            {
                value = RC1_BoardClampingDistance_Min;
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

            if (value > RC1_Plugoftimes_Max)
            {
                value = RC1_Plugoftimes_Max;
            }
            else if (value < RC1_Plugoftimes_Min)
            {
                value = RC1_Plugoftimes_Min;
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

            if (value > RC1_CoatingPressureSetting_Max)
            {
                value = RC1_CoatingPressureSetting_Max;
            }
            else if (value < RC1_CoatingPressureSetting_Min)
            {
                value = RC1_CoatingPressureSetting_Min;
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

            if (value > RC1_PanelThicknessSetting_Max)
            {
                value = RC1_PanelThicknessSetting_Max;
            }
            else if (value < RC1_PanelThicknessSetting_Min)
            {
                value = RC1_PanelThicknessSetting_Min;
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

            if (value > RC1_LocationOfDrop_Max)
            {
                value = RC1_LocationOfDrop_Max;
            }
            else if (value < RC1_LocationOfDrop_Min)
            {
                value = RC1_LocationOfDrop_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure", "入料下降位置設定", "入料下降位置設定")]
    public double RC1_Blade_Pressure
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC1_Blade_Pressure_Max)
            {
                value = RC1_Blade_Pressure_Max;
            }
            else if (value < RC1_Blade_Pressure_Min)
            {
                value = RC1_Blade_Pressure_Min;
            }

            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting1", "左前D.BAR壓力設定", "左前D.BAR壓力設定")]
    public double RC1_D_BarPressureSetting1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC1_D_BarPressureSetting1_Max)
            {
                value = RC1_D_BarPressureSetting1_Max;
            }
            else if (value < RC1_D_BarPressureSetting1_Min)
            {
                value = RC1_D_BarPressureSetting1_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting2", "右前D.BAR壓力設定", "右前D.BAR壓力設定")]
    public double RC1_D_BarPressureSetting2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC1_D_BarPressureSetting2_Max)
            {
                value = RC1_D_BarPressureSetting2_Max;
            }
            else if (value < RC1_D_BarPressureSetting2_Min)
            {
                value = RC1_D_BarPressureSetting2_Min;
            }

            Set(value);
        }
    }


    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting3", "左後D.BAR壓力設定", "左後D.BAR壓力設定")]
    public double RC1_D_BarPressureSetting3
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC1_D_BarPressureSetting3_Max)
            {
                value = RC1_D_BarPressureSetting3_Max;
            }
            else if (value < RC1_D_BarPressureSetting3_Min)
            {
                value = RC1_D_BarPressureSetting3_Min;
            }

            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting4", "右後D.BAR壓力設定", "右後D.BAR壓力設定")]
    public double RC1_D_BarPressureSetting4
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC1_D_BarPressureSetting4_Max)
            {
                value = RC1_D_BarPressureSetting4_Max;
            }
            else if (value < RC1_D_BarPressureSetting4_Min)
            {
                value = RC1_D_BarPressureSetting4_Min;
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

            if (value > RC1_BakingTimeSetting_Max)
            {
                value = RC1_BakingTimeSetting_Max;
            }
            else if (value < RC1_BakingTimeSetting_Min)
            {
                value = RC1_BakingTimeSetting_Min;
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

            if (value > RC1_TemperatureSV1_Max)
            {
                value = RC1_TemperatureSV1_Max;
            }
            else if (value < RC1_TemperatureSV1_Min)
            {
                value = RC1_TemperatureSV1_Min;
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

            if (value > RC1_TemperatureSV2_Max)
            {
                value = RC1_TemperatureSV2_Max;
            }
            else if (value < RC1_TemperatureSV2_Min)
            {
                value = RC1_TemperatureSV2_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("UseCoating", "塗佈使用", "塗佈使用")]
    public double RC1_UseCoating
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC1_UseCoating_Max)
            {
                value = RC1_UseCoating_Max;
            }
            else if (value < RC1_UseCoating_Min)
            {
                value = RC1_UseCoating_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("UsePlug", "塞孔使用", "塞孔使用")]
    public double RC1_UsePlug
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC1_UsePlug_Max)
            {
                value = RC1_UsePlug_Max;
            }
            else if (value < RC1_UsePlug_Min)
            {
                value = RC1_UsePlug_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("StandardInk", "標準墨重", "標準墨重")]
    public double RC1_StandardInk
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC1_StandardInk_Max)
            {
                value = RC1_StandardInk_Max;
            }
            else if (value < RC1_StandardInk_Min)
            {
                value = RC1_StandardInk_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("DifferenceOfInk", "墨重誤差值", "墨重誤差值")]
    public double RC1_DifferenceOfInk
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC1_DifferenceOfInk_Max)
            {
                value = RC1_DifferenceOfInk_Max;
            }
            else if (value < RC1_DifferenceOfInk_Min)
            {
                value = RC1_DifferenceOfInk_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(3)]
    [LanguageTranslator("Coatingoftimes", "塗佈次數", "塗佈次數")]
    public double Coatingoftimes
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
    public double CoatingSpeedSetting
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
    public double BoardClampingDistance
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
    public double Plugoftimes
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
    public double CoatingPressureSetting
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
    public double PanelThicknessSetting
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
    [LanguageTranslator("LocationOfDrop", "入料下降位置設定", "入料下降位置設定")]
    public double LocationOfDrop
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
    [LanguageTranslator("Blade_Pressure", "入料下降位置設定", "入料下降位置設定")]
    public double Blade_Pressure
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
    [LanguageTranslator("D_BarPressureSetting1", "左前D.BAR壓力設定", "左前D.BAR壓力設定")]
    public double D_BarPressureSetting1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > D_BarPressureSetting1_Max)
            {
                value = D_BarPressureSetting1_Max;
            }
            else if (value < D_BarPressureSetting1_Min)
            {
                value = D_BarPressureSetting1_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting2", "右前D.BAR壓力設定", "右前D.BAR壓力設定")]
    public double D_BarPressureSetting2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > D_BarPressureSetting2_Max)
            {
                value = D_BarPressureSetting2_Max;
            }
            else if (value < D_BarPressureSetting2_Min)
            {
                value = D_BarPressureSetting2_Min;
            }

            Set(value);
        }
    }


    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting3", "左後D.BAR壓力設定", "左後D.BAR壓力設定")]
    public double D_BarPressureSetting3
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > D_BarPressureSetting3_Max)
            {
                value = D_BarPressureSetting3_Max;
            }
            else if (value < D_BarPressureSetting3_Min)
            {
                value = D_BarPressureSetting3_Min;
            }

            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting4", "右後D.BAR壓力設定", "右後D.BAR壓力設定")]
    public double D_BarPressureSetting4
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > D_BarPressureSetting4_Max)
            {
                value = D_BarPressureSetting4_Max;
            }
            else if (value < D_BarPressureSetting4_Min)
            {
                value = D_BarPressureSetting4_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("BakingTimeSetting", "烘烤時間設定", "烘烤時間設定")]
    public double BakingTimeSetting
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
    public double TemperatureSV1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV1_Max)
            {
                value = TemperatureSV1_Max;
            }
            else if (value < TemperatureSV1_Min)
            {
                value = TemperatureSV1_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("TemperatureSV2", "第2段溫度設定值", "第2段溫度設定值")]
    public double TemperatureSV2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > TemperatureSV2_Max)
            {
                value = TemperatureSV2_Max;
            }
            else if (value < TemperatureSV2_Min)
            {
                value = TemperatureSV2_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("UseCoating", "塗佈使用", "塗佈使用")]
    public double UseCoating
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > UseCoating_Max)
            {
                value = UseCoating_Max;
            }
            else if (value < UseCoating_Min)
            {
                value = UseCoating_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("UsePlug", "塞孔使用", "塞孔使用")]
    public double UsePlug
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > UsePlug_Max)
            {
                value = UsePlug_Max;
            }
            else if (value < UsePlug_Min)
            {
                value = UsePlug_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("StandardInk", "標準墨重", "標準墨重")]
    public double StandardInk
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > StandardInk_Max)
            {
                value = StandardInk_Max;
            }
            else if (value < StandardInk_Min)
            {
                value = StandardInk_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("DifferenceOfInk", "墨重誤差值", "墨重誤差值")]
    public double DifferenceOfInk
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > DifferenceOfInk_Max)
            {
                value = DifferenceOfInk_Max;
            }
            else if (value < DifferenceOfInk_Min)
            {
                value = DifferenceOfInk_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(3)]
    [LanguageTranslator("Coatingoftimes", "塗佈次數", "塗佈次數")]
    public double RC2_Coatingoftimes
    {
        get => Get<double>();
        set
        {
            if (value > RC2_Coatingoftimes_Max)
            {
                value = RC2_Coatingoftimes_Max;
            }
            else if (value < RC2_Coatingoftimes_Min)
            {
                value = RC2_Coatingoftimes_Min;
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

            if (value > RC2_CoatingSpeedSetting_Max)
            {
                value = RC2_CoatingSpeedSetting_Max;
            }
            else if (value < RC2_CoatingSpeedSetting_Min)
            {
                value = RC2_CoatingSpeedSetting_Min;
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

            if (value > RC2_BoardClampingDistance_Max)
            {
                value = RC2_BoardClampingDistance_Max;
            }
            else if (value < RC2_BoardClampingDistance_Min)
            {
                value = RC2_BoardClampingDistance_Min;
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

            if (value > RC2_Plugoftimes_Max)
            {
                value = RC2_Plugoftimes_Max;
            }
            else if (value < RC2_Plugoftimes_Min)
            {
                value = RC2_Plugoftimes_Min;
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

            if (value > RC2_CoatingPressureSetting_Max)
            {
                value = RC2_CoatingPressureSetting_Max;
            }
            else if (value < RC2_CoatingPressureSetting_Min)
            {
                value = RC2_CoatingPressureSetting_Min;
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

            if (value > RC2_PanelThicknessSetting_Max)
            {
                value = RC2_PanelThicknessSetting_Max;
            }
            else if (value < RC2_PanelThicknessSetting_Min)
            {
                value = RC2_PanelThicknessSetting_Min;
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

            if (value > RC2_LocationOfDrop_Max)
            {
                value = RC2_LocationOfDrop_Max;
            }
            else if (value < RC2_LocationOfDrop_Min)
            {
                value = RC2_LocationOfDrop_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure", "入料下降位置設定", "入料下降位置設定")]
    public double RC2_Blade_Pressure
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC2_Blade_Pressure_Max)
            {
                value = RC2_Blade_Pressure_Max;
            }
            else if (value < RC2_Blade_Pressure_Min)
            {
                value = RC2_Blade_Pressure_Min;
            }

            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting1", "左前D.BAR壓力設定", "左前D.BAR壓力設定")]
    public double RC2_D_BarPressureSetting1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC2_D_BarPressureSetting1_Max)
            {
                value = RC2_D_BarPressureSetting1_Max;
            }
            else if (value < RC2_D_BarPressureSetting1_Min)
            {
                value = RC2_D_BarPressureSetting1_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting2", "右前D.BAR壓力設定", "右前D.BAR壓力設定")]
    public double RC2_D_BarPressureSetting2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC2_D_BarPressureSetting2_Max)
            {
                value = RC2_D_BarPressureSetting2_Max;
            }
            else if (value < RC2_D_BarPressureSetting2_Min)
            {
                value = RC2_D_BarPressureSetting2_Min;
            }

            Set(value);
        }
    }


    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting3", "左後D.BAR壓力設定", "左後D.BAR壓力設定")]
    public double RC2_D_BarPressureSetting3
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC2_D_BarPressureSetting3_Max)
            {
                value = RC2_D_BarPressureSetting3_Max;
            }
            else if (value < RC2_D_BarPressureSetting3_Min)
            {
                value = RC2_D_BarPressureSetting3_Min;
            }

            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting4", "右後D.BAR壓力設定", "右後D.BAR壓力設定")]
    public double RC2_D_BarPressureSetting4
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC2_D_BarPressureSetting4_Max)
            {
                value = RC2_D_BarPressureSetting4_Max;
            }
            else if (value < RC2_D_BarPressureSetting4_Min)
            {
                value = RC2_D_BarPressureSetting4_Min;
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

            if (value > RC2_BakingTimeSetting_Max)
            {
                value = RC2_BakingTimeSetting_Max;
            }
            else if (value < RC2_BakingTimeSetting_Min)
            {
                value = RC2_BakingTimeSetting_Min;
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

            if (value > RC2_TemperatureSV1_Max)
            {
                value = RC2_TemperatureSV1_Max;
            }
            else if (value < RC2_TemperatureSV1_Min)
            {
                value = RC2_TemperatureSV1_Min;
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

            if (value > RC2_TemperatureSV2_Max)
            {
                value = RC2_TemperatureSV2_Max;
            }
            else if (value < RC2_TemperatureSV2_Min)
            {
                value = RC2_TemperatureSV2_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("UseCoating", "塗佈使用", "塗佈使用")]
    public double RC2_UseCoating
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC2_UseCoating_Max)
            {
                value = RC2_UseCoating_Max;
            }
            else if (value < RC2_UseCoating_Min)
            {
                value = RC2_UseCoating_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("UsePlug", "塞孔使用", "塞孔使用")]
    public double RC2_UsePlug
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC2_UsePlug_Max)
            {
                value = RC2_UsePlug_Max;
            }
            else if (value < RC2_UsePlug_Min)
            {
                value = RC2_UsePlug_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("StandardInk", "標準墨重", "標準墨重")]
    public double RC2_StandardInk
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC2_StandardInk_Max)
            {
                value = RC2_StandardInk_Max;
            }
            else if (value < RC2_StandardInk_Min)
            {
                value = RC2_StandardInk_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("DifferenceOfInk", "墨重誤差值", "墨重誤差值")]
    public double RC2_DifferenceOfInk
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC2_DifferenceOfInk_Max)
            {
                value = RC2_DifferenceOfInk_Max;
            }
            else if (value < RC2_DifferenceOfInk_Min)
            {
                value = RC2_DifferenceOfInk_Min;
            }

            Set(value);
        }
    }

    [OrderIndex(3)]
    [LanguageTranslator("Coatingoftimes", "塗佈次數", "塗佈次數")]
    public double RC3_Coatingoftimes
    {
        get => Get<double>();
        set
        {
            if (value > RC3_Coatingoftimes_Max)
            {
                value = RC3_Coatingoftimes_Max;
            }
            else if (value < RC3_Coatingoftimes_Min)
            {
                value = RC3_Coatingoftimes_Min;
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

            if (value > RC3_CoatingSpeedSetting_Max)
            {
                value = RC3_CoatingSpeedSetting_Max;
            }
            else if (value < RC3_CoatingSpeedSetting_Min)
            {
                value = RC3_CoatingSpeedSetting_Min;
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

            if (value > RC3_BoardClampingDistance_Max)
            {
                value = RC3_BoardClampingDistance_Max;
            }
            else if (value < RC3_BoardClampingDistance_Min)
            {
                value = RC3_BoardClampingDistance_Min;
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

            if (value > RC3_Plugoftimes_Max)
            {
                value = RC3_Plugoftimes_Max;
            }
            else if (value < RC3_Plugoftimes_Min)
            {
                value = RC3_Plugoftimes_Min;
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

            if (value > RC3_CoatingPressureSetting_Max)
            {
                value = RC3_CoatingPressureSetting_Max;
            }
            else if (value < RC3_CoatingPressureSetting_Min)
            {
                value = RC3_CoatingPressureSetting_Min;
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

            if (value > RC3_PanelThicknessSetting_Max)
            {
                value = RC3_PanelThicknessSetting_Max;
            }
            else if (value < RC3_PanelThicknessSetting_Min)
            {
                value = RC3_PanelThicknessSetting_Min;
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

            if (value > RC3_LocationOfDrop_Max)
            {
                value = RC3_LocationOfDrop_Max;
            }
            else if (value < RC3_LocationOfDrop_Min)
            {
                value = RC3_LocationOfDrop_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("Blade_Pressure", "入料下降位置設定", "入料下降位置設定")]
    public double RC3_Blade_Pressure
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC3_Blade_Pressure_Max)
            {
                value = RC3_Blade_Pressure_Max;
            }
            else if (value < RC3_Blade_Pressure_Min)
            {
                value = RC3_Blade_Pressure_Min;
            }

            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting1", "左前D.BAR壓力設定", "左前D.BAR壓力設定")]
    public double RC3_D_BarPressureSetting1
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC3_D_BarPressureSetting1_Max)
            {
                value = RC3_D_BarPressureSetting1_Max;
            }
            else if (value < RC3_D_BarPressureSetting1_Min)
            {
                value = RC3_D_BarPressureSetting1_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting2", "右前D.BAR壓力設定", "右前D.BAR壓力設定")]
    public double RC3_D_BarPressureSetting2
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC3_D_BarPressureSetting2_Max)
            {
                value = RC3_D_BarPressureSetting2_Max;
            }
            else if (value < RC3_D_BarPressureSetting2_Min)
            {
                value = RC3_D_BarPressureSetting2_Min;
            }

            Set(value);
        }
    }


    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting3", "左後D.BAR壓力設定", "左後D.BAR壓力設定")]
    public double RC3_D_BarPressureSetting3
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC3_D_BarPressureSetting3_Max)
            {
                value = RC3_D_BarPressureSetting3_Max;
            }
            else if (value < RC3_D_BarPressureSetting3_Min)
            {
                value = RC3_D_BarPressureSetting3_Min;
            }

            Set(value);
        }
    }

    [OrderIndex(5)]
    [LanguageTranslator("D_BarPressureSetting4", "右後D.BAR壓力設定", "右後D.BAR壓力設定")]
    public double RC3_D_BarPressureSetting4
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC3_D_BarPressureSetting4_Max)
            {
                value = RC3_D_BarPressureSetting4_Max;
            }
            else if (value < RC3_D_BarPressureSetting4_Min)
            {
                value = RC3_D_BarPressureSetting4_Min;
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

            if (value > RC3_BakingTimeSetting_Max)
            {
                value = RC3_BakingTimeSetting_Max;
            }
            else if (value < RC3_BakingTimeSetting_Min)
            {
                value = RC3_BakingTimeSetting_Min;
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

            if (value > RC3_TemperatureSV1_Max)
            {
                value = RC3_TemperatureSV1_Max;
            }
            else if (value < RC3_TemperatureSV1_Min)
            {
                value = RC3_TemperatureSV1_Min;
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

            if (value > RC3_TemperatureSV2_Max)
            {
                value = RC3_TemperatureSV2_Max;
            }
            else if (value < RC3_TemperatureSV2_Min)
            {
                value = RC3_TemperatureSV2_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("UseCoating", "塗佈使用", "塗佈使用")]
    public double RC3_UseCoating
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC3_UseCoating_Max)
            {
                value = RC3_UseCoating_Max;
            }
            else if (value < RC3_UseCoating_Min)
            {
                value = RC3_UseCoating_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("UsePlug", "塞孔使用", "塞孔使用")]
    public double RC3_UsePlug
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC3_UsePlug_Max)
            {
                value = RC3_UsePlug_Max;
            }
            else if (value < RC3_UsePlug_Min)
            {
                value = RC3_UsePlug_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("StandardInk", "標準墨重", "標準墨重")]
    public double RC3_StandardInk
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC3_StandardInk_Max)
            {
                value = RC3_StandardInk_Max;
            }
            else if (value < RC3_StandardInk_Min)
            {
                value = RC3_StandardInk_Min;
            }

            Set(value);
        }
    }
    [OrderIndex(5)]
    [LanguageTranslator("DifferenceOfInk", "墨重誤差值", "墨重誤差值")]
    public double RC3_DifferenceOfInk
    {
        get => Get<double>();
        set
        {
            value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (value > RC3_DifferenceOfInk_Max)
            {
                value = RC3_DifferenceOfInk_Max;
            }
            else if (value < RC3_DifferenceOfInk_Min)
            {
                value = RC3_DifferenceOfInk_Min;
            }

            Set(value);
        }
    }

    public PLC_Recipe(string name, string user, UserLevel level) : base(name, user, level)
    {
        RecipeName = RecipeName;
        RC1_Coatingoftimes = RC1_Coatingoftimes;
        RC1_CoatingSpeedSetting = RC1_CoatingSpeedSetting;
        RC1_BoardClampingDistance = RC1_BoardClampingDistance;
        RC1_Plugoftimes = RC1_Plugoftimes;
        RC1_PanelThicknessSetting = RC1_PanelThicknessSetting;
        RC1_CoatingPressureSetting = RC1_CoatingPressureSetting;
        RC1_LocationOfDrop = RC1_LocationOfDrop;
        RC1_Blade_Pressure = 0;
        RC1_D_BarPressureSetting1 = 0;
        RC1_D_BarPressureSetting2 = 0;
        RC1_D_BarPressureSetting3 = 0;
        RC1_D_BarPressureSetting4 = 0;
        RC1_BakingTimeSetting = 0;
        RC1_TemperatureSV1 = 0;
        RC1_TemperatureSV2 = 0;
        RC1_UseCoating = 0;
        RC1_UsePlug = 0;
        RC1_StandardInk = 0;
        RC1_DifferenceOfInk = 0;
        RC2_Coatingoftimes = 0;
        RC2_CoatingSpeedSetting = 0;
        RC2_BoardClampingDistance = 0;
        RC2_Plugoftimes = 0;
        RC2_PanelThicknessSetting = 0;
        RC2_CoatingPressureSetting = 0;
        RC2_LocationOfDrop = 0;
        RC2_Blade_Pressure = 0;
        RC2_D_BarPressureSetting1 = 0;
        RC2_D_BarPressureSetting2 = 0;
        RC2_D_BarPressureSetting3 = 0;
        RC2_D_BarPressureSetting4 = 0;
        RC2_BakingTimeSetting = 0;
        RC2_TemperatureSV1 = 0;
        RC2_TemperatureSV2 = 0;
        RC2_UseCoating = 0;
        RC2_UsePlug = 0;
        RC2_StandardInk = 0;
        RC2_DifferenceOfInk = 0;
        RC3_Coatingoftimes = 0;
        RC3_CoatingSpeedSetting = 0;
        RC3_BoardClampingDistance = 0;
        RC3_Plugoftimes = 0;
        RC3_PanelThicknessSetting = 0;
        RC3_CoatingPressureSetting = 0;
        RC3_LocationOfDrop = 0;
        RC3_Blade_Pressure = 0;
        RC3_D_BarPressureSetting1 = 0;
        RC3_D_BarPressureSetting2 = 0;
        RC3_D_BarPressureSetting3 = 0;
        RC3_D_BarPressureSetting4 = 0;
        RC3_BakingTimeSetting = 0;
        RC3_TemperatureSV1 = 0;
        RC3_TemperatureSV2 = 0;
        RC3_UseCoating = 0;
        RC3_UsePlug = 0;
        RC3_StandardInk = 0;
        RC3_DifferenceOfInk = 0;
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
                                                      RC1_LocationOfDrop.ToString("0.0") == other.RC1_LocationOfDrop.ToString("0.0") &&
                                                      RC1_Blade_Pressure.ToString("0.0") == other.RC1_Blade_Pressure.ToString("0.0") &&
                                                      RC1_D_BarPressureSetting1.ToString("0.0") == other.RC1_D_BarPressureSetting1.ToString("0.0") &&
                                                      RC1_D_BarPressureSetting2.ToString("0.0") == other.RC1_D_BarPressureSetting2.ToString("0.0") &&
                                                      RC1_D_BarPressureSetting3.ToString("0.0") == other.RC1_D_BarPressureSetting3.ToString("0.0") &&
                                                      RC1_D_BarPressureSetting4.ToString("0.0") == other.RC1_D_BarPressureSetting4.ToString("0.0") &&
                                                      RC1_BakingTimeSetting.ToString("0.0") == other.RC1_BakingTimeSetting.ToString("0.0") &&
                                                      RC1_TemperatureSV1.ToString("0.0") == other.RC1_TemperatureSV1.ToString("0.0") &&
                                                      RC1_TemperatureSV2.ToString("0.0") == other.RC1_TemperatureSV2.ToString("0.0") &&
                                                      RC1_UseCoating.ToString("0.0") == other.RC1_UseCoating.ToString("0.0") &&
                                                      RC1_UsePlug.ToString("0.0") == other.RC1_UsePlug.ToString("0.0") &&
                                                      RC1_StandardInk.ToString("0.0") == other.RC1_StandardInk.ToString("0.0") &&
                                                      RC1_DifferenceOfInk.ToString("0.0") == other.RC1_DifferenceOfInk.ToString("0.0") &&
                                                      RC2_Coatingoftimes == other.RC2_Coatingoftimes &&
                                                      RC2_CoatingSpeedSetting.ToString("0.0") == other.RC2_CoatingSpeedSetting.ToString("0.0") &&
                                                      RC2_BoardClampingDistance.ToString("0.0") == other.RC2_BoardClampingDistance.ToString("0.0") &&
                                                      RC2_Plugoftimes.ToString("0.0") == other.RC2_Plugoftimes.ToString("0.0") &&
                                                      RC2_CoatingPressureSetting.ToString("0.0") == other.RC2_CoatingPressureSetting.ToString("0.0") &&
                                                      RC2_PanelThicknessSetting.ToString("0.0") == other.RC2_PanelThicknessSetting.ToString("0.0") &&
                                                      RC2_LocationOfDrop.ToString("0.0") == other.RC2_LocationOfDrop.ToString("0.0") &&
                                                      RC2_Blade_Pressure.ToString("0.0") == other.RC2_Blade_Pressure.ToString("0.0") &&
                                                      RC2_D_BarPressureSetting1.ToString("0.0") == other.RC2_D_BarPressureSetting1.ToString("0.0") &&
                                                      RC2_D_BarPressureSetting2.ToString("0.0") == other.RC2_D_BarPressureSetting2.ToString("0.0") &&
                                                      RC2_D_BarPressureSetting3.ToString("0.0") == other.RC2_D_BarPressureSetting3.ToString("0.0") &&
                                                      RC2_D_BarPressureSetting4.ToString("0.0") == other.RC2_D_BarPressureSetting4.ToString("0.0") &&
                                                      RC2_BakingTimeSetting.ToString("0.0") == other.RC2_BakingTimeSetting.ToString("0.0") &&
                                                      RC2_TemperatureSV1.ToString("0.0") == other.RC2_TemperatureSV1.ToString("0.0") &&
                                                      RC2_TemperatureSV2.ToString("0.0") == other.RC2_TemperatureSV2.ToString("0.0") &&
                                                      RC2_UseCoating.ToString("0.0") == other.RC2_UseCoating.ToString("0.0") &&
                                                      RC2_UsePlug.ToString("0.0") == other.RC2_UsePlug.ToString("0.0") &&
                                                      RC2_StandardInk.ToString("0.0") == other.RC2_StandardInk.ToString("0.0") &&
                                                      RC2_DifferenceOfInk.ToString("0.0") == other.RC2_DifferenceOfInk.ToString("0.0") &&
                                                      RC3_Coatingoftimes == other.RC3_Coatingoftimes &&
                                                      RC3_CoatingSpeedSetting.ToString("0.0") == other.RC3_CoatingSpeedSetting.ToString("0.0") &&
                                                      RC3_BoardClampingDistance.ToString("0.0") == other.RC3_BoardClampingDistance.ToString("0.0") &&
                                                      RC3_Plugoftimes.ToString("0.0") == other.RC3_Plugoftimes.ToString("0.0") &&
                                                      RC3_CoatingPressureSetting.ToString("0.0") == other.RC3_CoatingPressureSetting.ToString("0.0") &&
                                                      RC3_PanelThicknessSetting.ToString("0.0") == other.RC3_PanelThicknessSetting.ToString("0.0") &&
                                                      RC3_LocationOfDrop.ToString("0.0") == other.RC3_LocationOfDrop.ToString("0.0") &&
                                                      RC3_Blade_Pressure.ToString("0.0") == other.RC3_Blade_Pressure.ToString("0.0") &&
                                                      RC3_D_BarPressureSetting1.ToString("0.0") == other.RC3_D_BarPressureSetting1.ToString("0.0") &&
                                                      RC3_D_BarPressureSetting2.ToString("0.0") == other.RC3_D_BarPressureSetting2.ToString("0.0") &&
                                                      RC3_D_BarPressureSetting3.ToString("0.0") == other.RC3_D_BarPressureSetting3.ToString("0.0") &&
                                                      RC3_D_BarPressureSetting4.ToString("0.0") == other.RC3_D_BarPressureSetting4.ToString("0.0") &&
                                                      RC3_BakingTimeSetting.ToString("0.0") == other.RC3_BakingTimeSetting.ToString("0.0") &&
                                                      RC3_TemperatureSV1.ToString("0.0") == other.RC3_TemperatureSV1.ToString("0.0") &&
                                                      RC3_TemperatureSV2.ToString("0.0") == other.RC3_TemperatureSV2.ToString("0.0") &&
                                                      RC3_UseCoating.ToString("0.0") == other.RC3_UseCoating.ToString("0.0") &&
                                                      RC3_UsePlug.ToString("0.0") == other.RC3_UsePlug.ToString("0.0") &&
                                                      RC3_StandardInk.ToString("0.0") == other.RC3_StandardInk.ToString("0.0") &&
                                                      RC3_DifferenceOfInk.ToString("0.0") == other.RC3_DifferenceOfInk.ToString("0.0");

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
        RC1_Blade_Pressure = RC1_Blade_Pressure,
        RC1_D_BarPressureSetting1 = RC1_D_BarPressureSetting1,
        RC1_D_BarPressureSetting2 = RC1_D_BarPressureSetting2,
        RC1_D_BarPressureSetting3 = RC1_D_BarPressureSetting3,
        RC1_D_BarPressureSetting4 = RC1_D_BarPressureSetting4,
        RC1_BakingTimeSetting = RC1_BakingTimeSetting,
        RC1_TemperatureSV1 = RC1_TemperatureSV1,
        RC1_TemperatureSV2 = RC1_TemperatureSV2,
        RC1_UseCoating = RC1_UseCoating,
        RC1_UsePlug = RC1_UsePlug,
        RC1_StandardInk = RC1_StandardInk,
        RC1_DifferenceOfInk = RC1_DifferenceOfInk,
        RC2_Coatingoftimes = RC2_Coatingoftimes,
        RC2_CoatingSpeedSetting = RC2_CoatingSpeedSetting,
        RC2_BoardClampingDistance = RC2_BoardClampingDistance,
        RC2_Plugoftimes = RC2_Plugoftimes,
        RC2_PanelThicknessSetting = RC2_PanelThicknessSetting,
        RC2_CoatingPressureSetting = RC2_CoatingPressureSetting,
        RC2_LocationOfDrop = RC2_LocationOfDrop,
        RC2_Blade_Pressure = RC2_Blade_Pressure,
        RC2_D_BarPressureSetting1 = RC2_D_BarPressureSetting1,
        RC2_D_BarPressureSetting2 = RC2_D_BarPressureSetting2,
        RC2_D_BarPressureSetting3 = RC2_D_BarPressureSetting3,
        RC2_D_BarPressureSetting4 = RC2_D_BarPressureSetting4,
        RC2_BakingTimeSetting = RC2_BakingTimeSetting,
        RC2_TemperatureSV1 = RC2_TemperatureSV1,
        RC2_TemperatureSV2 = RC2_TemperatureSV2,
        RC2_UseCoating = RC2_UseCoating,
        RC2_UsePlug = RC2_UsePlug,
        RC2_StandardInk = RC2_StandardInk,
        RC2_DifferenceOfInk = RC2_DifferenceOfInk,
        RC3_Coatingoftimes = RC3_Coatingoftimes,
        RC3_CoatingSpeedSetting = RC3_CoatingSpeedSetting,
        RC3_BoardClampingDistance = RC3_BoardClampingDistance,
        RC3_Plugoftimes = RC3_Plugoftimes,
        RC3_PanelThicknessSetting = RC3_PanelThicknessSetting,
        RC3_CoatingPressureSetting = RC3_CoatingPressureSetting,
        RC3_LocationOfDrop = RC3_LocationOfDrop,
        RC3_Blade_Pressure = RC3_Blade_Pressure,
        RC3_D_BarPressureSetting1 = RC3_D_BarPressureSetting1,
        RC3_D_BarPressureSetting2 = RC3_D_BarPressureSetting2,
        RC3_D_BarPressureSetting3 = RC3_D_BarPressureSetting3,
        RC3_D_BarPressureSetting4 = RC3_D_BarPressureSetting4,
        RC3_BakingTimeSetting = RC3_BakingTimeSetting,
        RC3_TemperatureSV1 = RC3_TemperatureSV1,
        RC3_TemperatureSV2 = RC3_TemperatureSV2,
        RC3_UseCoating = RC3_UseCoating,
        RC3_UsePlug = RC3_UsePlug,
        RC3_StandardInk = RC3_StandardInk,
        RC3_DifferenceOfInk = RC3_DifferenceOfInk,
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
        RC1_LocationOfDrop = recipe.RC1_LocationOfDrop;
        RC1_Blade_Pressure = recipe.RC1_Blade_Pressure;
        RC1_D_BarPressureSetting1 = recipe.RC1_D_BarPressureSetting1;
        RC1_D_BarPressureSetting2 = recipe.RC1_D_BarPressureSetting2;
        RC1_D_BarPressureSetting3 = recipe.RC1_D_BarPressureSetting3;
        RC1_D_BarPressureSetting4 = recipe.RC1_D_BarPressureSetting4;
        RC1_BakingTimeSetting = recipe.RC1_BakingTimeSetting;
        RC1_TemperatureSV1 = recipe.RC1_TemperatureSV1;
        RC1_TemperatureSV2 = recipe.RC1_TemperatureSV2;
        RC1_UseCoating = recipe.RC1_UseCoating;
        RC1_UsePlug = recipe.RC1_UsePlug;
        RC1_StandardInk = recipe.RC1_StandardInk;
        RC1_DifferenceOfInk = recipe.RC1_DifferenceOfInk;
        RC2_Coatingoftimes = recipe.RC2_Coatingoftimes;
        RC2_CoatingSpeedSetting = recipe.RC2_CoatingSpeedSetting;
        RC2_Plugoftimes = recipe.RC2_Plugoftimes;
        RC2_CoatingPressureSetting = recipe.RC2_CoatingPressureSetting;
        RC2_PanelThicknessSetting = recipe.RC2_PanelThicknessSetting;
        RC2_LocationOfDrop = recipe.RC2_LocationOfDrop;
        RC2_Blade_Pressure = recipe.RC2_Blade_Pressure;
        RC2_D_BarPressureSetting1 = recipe.RC2_D_BarPressureSetting1;
        RC2_D_BarPressureSetting2 = recipe.RC2_D_BarPressureSetting2;
        RC2_D_BarPressureSetting3 = recipe.RC2_D_BarPressureSetting3;
        RC2_D_BarPressureSetting4 = recipe.RC2_D_BarPressureSetting4;
        RC2_BakingTimeSetting = recipe.RC2_BakingTimeSetting;
        RC2_TemperatureSV1 = recipe.RC2_TemperatureSV1;
        RC2_TemperatureSV2 = recipe.RC2_TemperatureSV2;
        RC2_UseCoating = recipe.RC2_UseCoating;
        RC2_UsePlug = recipe.RC2_UsePlug;
        RC2_StandardInk = recipe.RC2_StandardInk;
        RC2_DifferenceOfInk = recipe.RC2_DifferenceOfInk;
        RC3_Coatingoftimes = recipe.RC3_Coatingoftimes;
        RC3_CoatingSpeedSetting = recipe.RC3_CoatingSpeedSetting;
        RC3_Plugoftimes = recipe.RC3_Plugoftimes;
        RC3_CoatingPressureSetting = recipe.RC3_CoatingPressureSetting;
        RC3_PanelThicknessSetting = recipe.RC3_PanelThicknessSetting;
        RC3_LocationOfDrop = recipe.RC3_LocationOfDrop;
        RC3_Blade_Pressure = recipe.RC3_Blade_Pressure;
        RC3_D_BarPressureSetting1 = recipe.RC3_D_BarPressureSetting1;
        RC3_D_BarPressureSetting2 = recipe.RC3_D_BarPressureSetting2;
        RC3_D_BarPressureSetting3 = recipe.RC3_D_BarPressureSetting3;
        RC3_D_BarPressureSetting4 = recipe.RC3_D_BarPressureSetting4;
        RC3_BakingTimeSetting = recipe.RC3_BakingTimeSetting;
        RC3_TemperatureSV1 = recipe.RC3_TemperatureSV1;
        RC3_TemperatureSV2 = recipe.RC3_TemperatureSV2;
        RC3_UseCoating = recipe.RC3_UseCoating;
        RC3_UsePlug = recipe.RC3_UsePlug;
        RC3_StandardInk = recipe.RC3_StandardInk;
        RC3_DifferenceOfInk = recipe.RC3_DifferenceOfInk;
        Editor = user;
        EditorLevel = level;
    }

    public Dictionary<string, object> ToDictionary() => new()
                                                        {
                                                            { nameof(RecipeName), RecipeName },
                                                            { nameof(Coatingoftimes), RC1_Coatingoftimes },
                                                            { nameof(CoatingSpeedSetting), RC1_CoatingSpeedSetting },
                                                            { nameof(BoardClampingDistance), RC1_BoardClampingDistance },
                                                            { nameof(Plugoftimes), RC1_Plugoftimes },
                                                            { nameof(CoatingPressureSetting), RC1_CoatingPressureSetting },
                                                            { nameof(PanelThicknessSetting), RC1_PanelThicknessSetting },
                                                            { nameof(LocationOfDrop), RC1_LocationOfDrop },
                                                            { nameof(Blade_Pressure), RC1_Blade_Pressure },
                                                            { nameof(D_BarPressureSetting1), RC1_D_BarPressureSetting1 },
                                                            { nameof(D_BarPressureSetting2), RC1_D_BarPressureSetting2 },
                                                            { nameof(D_BarPressureSetting3), RC1_D_BarPressureSetting3 },
                                                            { nameof(D_BarPressureSetting4), RC1_D_BarPressureSetting4 },
                                                            { nameof(BakingTimeSetting), RC1_BakingTimeSetting },
                                                            { nameof(TemperatureSV1), RC1_TemperatureSV1 },
                                                            { nameof(TemperatureSV2), RC1_TemperatureSV2 },
                                                            { nameof(UseCoating), RC1_UseCoating },
                                                            { nameof(UsePlug), RC1_UsePlug },
                                                            { nameof(StandardInk), RC1_StandardInk },
                                                            { nameof(DifferenceOfInk), RC1_DifferenceOfInk },
                                                            //{ nameof(RC2_Coatingoftimes), RC2_Coatingoftimes },
                                                            //{ nameof(RC2_CoatingSpeedSetting), RC2_CoatingSpeedSetting },
                                                            //{ nameof(RC2_BoardClampingDistance), RC2_BoardClampingDistance },
                                                            //{ nameof(RC2_Plugoftimes), RC2_Plugoftimes },
                                                            //{ nameof(RC2_CoatingPressureSetting), RC2_CoatingPressureSetting },
                                                            //{ nameof(RC2_PanelThicknessSetting), RC2_PanelThicknessSetting },
                                                            //{ nameof(RC2_LocationOfDrop), RC2_LocationOfDrop },
                                                            //{ nameof(RC2_Blade_Pressure), RC2_Blade_Pressure },
                                                            //{ nameof(RC2_D_BarPressureSetting1), RC2_D_BarPressureSetting1 },
                                                            //{ nameof(RC2_D_BarPressureSetting2), RC2_D_BarPressureSetting2 },
                                                            //{ nameof(RC2_D_BarPressureSetting3), RC2_D_BarPressureSetting3 },
                                                            //{ nameof(RC2_D_BarPressureSetting4), RC2_D_BarPressureSetting4 },
                                                            //{ nameof(RC2_BakingTimeSetting), RC2_BakingTimeSetting },
                                                            //{ nameof(RC2_TemperatureSV1), RC2_TemperatureSV1 },
                                                            //{ nameof(RC2_TemperatureSV2), RC2_TemperatureSV2 },
                                                            //{ nameof(RC2_UseCoating), RC2_UseCoating },
                                                            //{ nameof(RC2_UsePlug), RC2_UsePlug },
                                                            //{ nameof(RC2_StandardInk), RC2_StandardInk },
                                                            //{ nameof(RC2_DifferenceOfInk), RC2_DifferenceOfInk },
                                                            //{ nameof(RC3_Coatingoftimes), RC3_Coatingoftimes },
                                                            //{ nameof(RC3_CoatingSpeedSetting), RC3_CoatingSpeedSetting },
                                                            //{ nameof(RC3_BoardClampingDistance), RC3_BoardClampingDistance },
                                                            //{ nameof(RC3_Plugoftimes), RC3_Plugoftimes },
                                                            //{ nameof(RC3_CoatingPressureSetting), RC3_CoatingPressureSetting },
                                                            //{ nameof(RC3_PanelThicknessSetting), RC3_PanelThicknessSetting },
                                                            //{ nameof(RC3_LocationOfDrop), RC3_LocationOfDrop },
                                                            //{ nameof(RC3_Blade_Pressure), RC3_Blade_Pressure },
                                                            //{ nameof(RC3_D_BarPressureSetting1), RC3_D_BarPressureSetting1 },
                                                            //{ nameof(RC3_D_BarPressureSetting2), RC3_D_BarPressureSetting2 },
                                                            //{ nameof(RC3_D_BarPressureSetting3), RC3_D_BarPressureSetting3 },
                                                            //{ nameof(RC3_D_BarPressureSetting4), RC3_D_BarPressureSetting4 },
                                                            //{ nameof(RC3_BakingTimeSetting), RC3_BakingTimeSetting },
                                                            //{ nameof(RC3_TemperatureSV1), RC3_TemperatureSV1 },
                                                            //{ nameof(RC3_TemperatureSV2), RC3_TemperatureSV2 },
                                                            //{ nameof(RC3_UseCoating), RC3_UseCoating },
                                                            //{ nameof(RC3_UsePlug), RC3_UsePlug },
                                                            //{ nameof(RC3_StandardInk), RC3_StandardInk },
                                                            //{ nameof(RC3_DifferenceOfInk), RC3_DifferenceOfInk },
                                                            };
    public Dictionary<string, object> ToDictionary(int i) => i switch
    {
        0 => new Dictionary<string, object>
        {
            { nameof(RecipeName), RecipeName },
            { nameof(Coatingoftimes), RC1_Coatingoftimes },
            { nameof(CoatingSpeedSetting), RC1_CoatingSpeedSetting },
            { nameof(BoardClampingDistance), RC1_BoardClampingDistance },
            { nameof(Plugoftimes), RC1_Plugoftimes },
            { nameof(CoatingPressureSetting), RC1_CoatingPressureSetting },
            { nameof(PanelThicknessSetting), RC1_PanelThicknessSetting },
            { nameof(LocationOfDrop), RC1_LocationOfDrop },
            { nameof(Blade_Pressure), RC1_Blade_Pressure },
            { nameof(D_BarPressureSetting1), RC1_D_BarPressureSetting1 },
            { nameof(D_BarPressureSetting2), RC1_D_BarPressureSetting2 },
            { nameof(D_BarPressureSetting3), RC1_D_BarPressureSetting3 },
            { nameof(D_BarPressureSetting4), RC1_D_BarPressureSetting4 },
            { nameof(BakingTimeSetting), RC1_BakingTimeSetting },
            { nameof(TemperatureSV1), RC1_TemperatureSV1 },
            { nameof(TemperatureSV2), RC1_TemperatureSV2 },
            { nameof(UseCoating), RC1_UseCoating },
            { nameof(UsePlug), RC1_UsePlug },
            { nameof(StandardInk), RC1_StandardInk },
            { nameof(DifferenceOfInk), RC1_DifferenceOfInk },
        },
        1 => new Dictionary<string, object>
        {
            { nameof(RecipeName), RecipeName },
            { nameof(Coatingoftimes), RC2_Coatingoftimes },
            { nameof(CoatingSpeedSetting), RC2_CoatingSpeedSetting },
            { nameof(BoardClampingDistance), RC2_BoardClampingDistance },
            { nameof(Plugoftimes), RC2_Plugoftimes },
            { nameof(CoatingPressureSetting), RC2_CoatingPressureSetting },
            { nameof(PanelThicknessSetting), RC2_PanelThicknessSetting },
            { nameof(LocationOfDrop), RC2_LocationOfDrop },
            { nameof(Blade_Pressure), RC2_Blade_Pressure },
            { nameof(D_BarPressureSetting1), RC2_D_BarPressureSetting1 },
            { nameof(D_BarPressureSetting2), RC2_D_BarPressureSetting2 },
            { nameof(D_BarPressureSetting3), RC2_D_BarPressureSetting3 },
            { nameof(D_BarPressureSetting4), RC2_D_BarPressureSetting4 },
            { nameof(BakingTimeSetting), RC2_BakingTimeSetting },
            { nameof(TemperatureSV1), RC2_TemperatureSV1 },
            { nameof(TemperatureSV2), RC2_TemperatureSV2 },
            { nameof(UseCoating), RC2_UseCoating },
            { nameof(UsePlug), RC2_UsePlug },
            { nameof(StandardInk), RC2_StandardInk },
            { nameof(DifferenceOfInk), RC2_DifferenceOfInk },
        },
        2 => new Dictionary<string, object>
        {
             { nameof(RecipeName), RecipeName },
             { nameof(Coatingoftimes), RC3_Coatingoftimes },
             { nameof(CoatingSpeedSetting), RC3_CoatingSpeedSetting },
             { nameof(BoardClampingDistance), RC3_BoardClampingDistance },
             { nameof(Plugoftimes), RC3_Plugoftimes },
             { nameof(CoatingPressureSetting), RC3_CoatingPressureSetting },
             { nameof(PanelThicknessSetting), RC3_PanelThicknessSetting },
             { nameof(LocationOfDrop), RC3_LocationOfDrop },
             { nameof(Blade_Pressure), RC3_Blade_Pressure },
             { nameof(D_BarPressureSetting1), RC3_D_BarPressureSetting1 },
             { nameof(D_BarPressureSetting2), RC3_D_BarPressureSetting2 },
             { nameof(D_BarPressureSetting3), RC3_D_BarPressureSetting3 },
             { nameof(D_BarPressureSetting4), RC3_D_BarPressureSetting4 },
             { nameof(BakingTimeSetting), RC3_BakingTimeSetting },
             { nameof(TemperatureSV1), RC3_TemperatureSV1 },
             { nameof(TemperatureSV2), RC3_TemperatureSV2 },
             { nameof(UseCoating), RC3_UseCoating },
             { nameof(UsePlug), RC3_UsePlug },
             { nameof(StandardInk), RC3_StandardInk },
             { nameof(DifferenceOfInk), RC3_DifferenceOfInk },
        },
        // 可以根據需要添加其他 case
        _ => throw new ArgumentException("不支援的 plcindex 值"),
    };
    //{ nameof(RecipeName), RecipeName },
    //{ nameof(Coatingoftimes), RC1_Coatingoftimes },
    //{ nameof(CoatingSpeedSetting), RC1_CoatingSpeedSetting },
    //{ nameof(BoardClampingDistance), RC1_BoardClampingDistance },
    //{ nameof(Plugoftimes), RC1_Plugoftimes },
    //{ nameof(CoatingPressureSetting), RC1_CoatingPressureSetting },
    //{ nameof(PanelThicknessSetting), RC1_PanelThicknessSetting },
    //{ nameof(LocationOfDrop), RC1_LocationOfDrop },
    //{ nameof(Blade_Pressure), RC1_Blade_Pressure },
    //{ nameof(D_BarPressureSetting1), RC1_D_BarPressureSetting1 },
    //{ nameof(D_BarPressureSetting2), RC1_D_BarPressureSetting2 },
    //{ nameof(D_BarPressureSetting3), RC1_D_BarPressureSetting3 },
    //{ nameof(D_BarPressureSetting4), RC1_D_BarPressureSetting4 },
    //{ nameof(BakingTimeSetting), RC1_BakingTimeSetting },
    //{ nameof(TemperatureSV1), RC1_TemperatureSV1 },
    //{ nameof(TemperatureSV2), RC1_TemperatureSV2 },
    //{ nameof(UseCoating), RC1_UseCoating },
    //{ nameof(UsePlug), RC1_UsePlug },
    //{ nameof(StandardInk), RC1_StandardInk },
    //{ nameof(DifferenceOfInk), RC1_DifferenceOfInk },


    public Dictionary<string, object> ToShowDictionary() => new()
                                                        {
                                                            { "配方", RecipeName },
                                                            { "RC1_塗佈次數", RC1_Coatingoftimes },
                                                            { "RC1_塗佈速度設定", RC1_CoatingSpeedSetting },
                                                            { "RC1_板面夾持距離", RC1_BoardClampingDistance },
                                                            { "RC1_塞孔次數", RC1_Plugoftimes },
                                                            { "RC1_塗佈壓力設定", RC1_CoatingPressureSetting },
                                                            { "RC1_基板厚度設定", RC1_PanelThicknessSetting },
                                                            { "RC1_塞孔刮刀壓力設定", RC1_Blade_Pressure },
                                                            { "RC1_入料下降位置設定", RC1_LocationOfDrop },
                                                            { "RC1_左前D.BAR壓力設定", RC1_D_BarPressureSetting1 },
                                                            { "RC1_右前D.BAR壓力設定", RC1_D_BarPressureSetting2 },
                                                            { "RC1_左後D.BAR壓力設定", RC1_D_BarPressureSetting3 },
                                                            { "RC1_右後D.BAR壓力設定", RC1_D_BarPressureSetting4 },
                                                            { "RC1_烘烤時間設定", RC1_BakingTimeSetting },
                                                            { "RC1_第1段溫度設定值", RC1_TemperatureSV1 },
                                                            { "RC1_第2段溫度設定值", RC1_TemperatureSV2 },
                                                            { "RC1_塗佈使用", RC1_UseCoating },
                                                            { "RC1_塞孔使用", RC1_UsePlug },
                                                            { "RC1_標準墨重", RC1_StandardInk },
                                                            { "RC1_墨重誤差值", RC1_DifferenceOfInk },
                                                            { "RC2_塗佈次數", RC2_Coatingoftimes },
                                                            { "RC2_塗佈速度設定", RC2_CoatingSpeedSetting },
                                                            { "RC2_板面夾持距離", RC2_BoardClampingDistance },
                                                            { "RC2_塞孔次數", RC2_Plugoftimes },
                                                            { "RC2_塗佈壓力設定", RC2_CoatingPressureSetting },
                                                            { "RC2_基板厚度設定", RC2_PanelThicknessSetting },
                                                            { "RC2_塞孔刮刀壓力設定", RC2_Blade_Pressure },
                                                            { "RC2_入料下降位置設定", RC2_LocationOfDrop },
                                                            { "RC2_左前D.BAR壓力設定", RC2_D_BarPressureSetting1 },
                                                            { "RC2_右前D.BAR壓力設定", RC2_D_BarPressureSetting2 },
                                                            { "RC2_左後D.BAR壓力設定", RC2_D_BarPressureSetting3 },
                                                            { "RC2_右後D.BAR壓力設定", RC2_D_BarPressureSetting4 },
                                                            { "RC2_烘烤時間設定", RC2_BakingTimeSetting },
                                                            { "RC2_第1段溫度設定值", RC2_TemperatureSV1 },
                                                            { "RC2_第2段溫度設定值", RC2_TemperatureSV2 },
                                                            { "RC2_塗佈使用", RC2_UseCoating },
                                                            { "RC2_塞孔使用", RC2_UsePlug },
                                                            { "RC2_標準墨重", RC2_StandardInk },
                                                            { "RC2_墨重誤差值", RC2_DifferenceOfInk },
                                                            { "RC3_塗佈次數", RC3_Coatingoftimes },
                                                            { "RC3_塗佈速度設定", RC3_CoatingSpeedSetting },
                                                            { "RC3_板面夾持距離", RC3_BoardClampingDistance },
                                                            { "RC3_塞孔次數", RC3_Plugoftimes },
                                                            { "RC3_塗佈壓力設定", RC3_CoatingPressureSetting },
                                                            { "RC3_基板厚度設定", RC3_PanelThicknessSetting },
                                                            { "RC3_塞孔刮刀壓力設定", RC3_Blade_Pressure },
                                                            { "RC3_入料下降位置設定", RC3_LocationOfDrop },
                                                            { "RC3_左前D.BAR壓力設定", RC3_D_BarPressureSetting1 },
                                                            { "RC3_右前D.BAR壓力設定", RC3_D_BarPressureSetting2 },
                                                            { "RC3_左後D.BAR壓力設定", RC3_D_BarPressureSetting3 },
                                                            { "RC3_右後D.BAR壓力設定", RC3_D_BarPressureSetting4 },
                                                            { "RC3_烘烤時間設定", RC3_BakingTimeSetting },
                                                            { "RC3_第1段溫度設定值", RC3_TemperatureSV1 },
                                                            { "RC3_第2段溫度設定值", RC3_TemperatureSV2 },
                                                            { "RC3_塗佈使用", RC3_UseCoating },
                                                            { "RC3_塞孔使用", RC3_UsePlug },
                                                            { "RC3_標準墨重", RC3_StandardInk },
                                                            { "RC3_墨重誤差值", RC3_DifferenceOfInk },
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