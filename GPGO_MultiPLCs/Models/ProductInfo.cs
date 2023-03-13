using System;
using System.Collections.Generic;
using GPMVVM.Helpers;
using GPMVVM.Models;

namespace GPGO_MultiPLCs.Models;

/// <summary>材料生產資訊</summary>
public class ProductInfo : ObservableObject //! 這是一個批號的資料
{
    [LanguageTranslator("Quantity", "數量", "数量")]
    public int Quantity
    {
        get => Get<int>();
        set => Set(value);
    }

    [LanguageTranslator("Code Type", "條碼類型", "条码类型")]
    public CodeType CodeType
    {
        get => Get<CodeType>();
        set => Set(value);
    }

    [LanguageTranslator("First Article", "首件", "首件")]
    public bool FirstPanel
    {
        get => Get<bool>();
        set => Set(value);
    }

    [LanguageTranslator("Order", "工單", "工单")]
    public string OrderCode
    {
        get => Get<string>()!;
        set => Set(value);
    }

    [LanguageTranslator("PartID", "料號", "料号")]
    public string PartID
    {
        get => Get<string>()!;
        set => Set(value);
    }

    [LanguageTranslator("LotID", "批號", "批号")]
    public string LotID
    {
        get => Get<string>()!;
        set => Set(value);
    }

    [LanguageTranslator("SN", "序號", "序号")]
    public int ProcessNumber
    {
        get => Get<int>();
        set => Set(value);
    }

    [LanguageTranslator("Side", "面", "面")]
    public string Side
    {
        get => Get<string>()!;
        set => Set(value);
    }

    /// <summary>放在第幾層</summary>
    [LanguageTranslator("Layer", "階層", "阶层")]
    public int Layer
    {
        get => Get<int>();
        set => Set(value);
    }

    public ProductInfo()
    {
        CodeType  = CodeType.Panel;
        OrderCode = string.Empty;
        PartID    = string.Empty;
        LotID     = string.Empty;
        Side      = "A";
    }

    /// <summary></summary>
    /// <param name="code">工單條碼</param>
    public ProductInfo(string code)
    {
        var strs = code.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        OrderCode     = strs.Length > 0 ? strs[0] : "";
        ProcessNumber = strs.Length > 1 ? int.TryParse(strs[1], out var num) ? num : 0 : 0;
    }

    public ProductInfo(string orderCode, int processNumber)
    {
        OrderCode     = orderCode;
        ProcessNumber = processNumber;
    }

    public Dictionary<string, object> ToDic(Language lng)
    {
        var type = GetType();

        return new Dictionary<string, object>
               {
                   { type.GetProperty(nameof(Layer))?.GetName(lng)    ?? nameof(Layer), Layer },
                   { type.GetProperty(nameof(PartID))?.GetName(lng)   ?? nameof(PartID), PartID },
                   { type.GetProperty(nameof(LotID))?.GetName(lng)    ?? nameof(LotID), LotID },
                   { type.GetProperty(nameof(Quantity))?.GetName(lng) ?? nameof(Quantity), Quantity }
               };
    }
}