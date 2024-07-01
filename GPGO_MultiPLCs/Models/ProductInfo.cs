using GPMVVM.Helpers;
using GPMVVM.Models;

namespace GPGRC_MultiPLCs.Models;

/// <summary>材料生產資訊</summary>
public class ProductInfo : ObservableObject //! 這是一個批號的資料
{
    [LanguageTranslator("TotalQuantity", "總數量", "總数量")]
    public int TotalQuantity
    {
        get => Get<int>();
        set => Set(value);
    }
    [LanguageTranslator("ProductionQuantity", "生產數量", "生產數量")]
    public int ProductionQuantity
    {
        get => Get<int>();
        set => Set(value);
    }
    [LanguageTranslator("PanelInQty", "進板數量", "進板数量")]
    public int PanelInQty
    {
        get => Get<int>();
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
    [LanguageTranslator("Recipe", "配方", "配方")]
    public string Recipe
    {
        get => Get<string>()!;
        set => Set(value);
    }

    public ProductInfo()
    {
        PartID = string.Empty;
        LotID = string.Empty;
        Recipe = string.Empty;
    }

}