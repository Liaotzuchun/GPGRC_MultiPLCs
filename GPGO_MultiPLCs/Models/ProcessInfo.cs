using System;
using System.Collections.Generic;
using System.Linq;
using GPMVVM.Helpers;
using GPMVVM.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models;

/// <summary>機台資訊</summary>
[BsonIgnoreExtraElements]
public class BaseInfo : ObservableObject
{
    [GPIgnore]
    [LanguageTranslator("Asset No.", "財產編號", "财产编号")]
    public string AssetNumber
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    [GPIgnore]
    [LanguageTranslator("Device", "設備編號", "设备编号")]
    public string MachineCode
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    [GPIgnore]
    [LanguageTranslator("Oven", "機台別", "机台别")]
    public string OvenCode
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    [LanguageTranslator("Operator", "操作員", "操作员")]
    public string OperatorID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    [LanguageTranslator("Lot", "Lot", "Lot")]
    public string LotID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    [LanguageTranslator("Part", "Part", "Part")]
    public string Part
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    [LanguageTranslator("Recipe", "配方", "配方")]
    public PLC_Recipe? Recipe
    {
        get => Get<PLC_Recipe>();
        set => Set(value);
    }

    [GPIgnore]
    [LanguageTranslator("RackID", "板架", "台车")]
    public string RackID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    [LanguageTranslator("Starting Time", "開始時間", "开始时间")]
    public DateTime StartTime
    {
        get => Get<DateTime>();
        set => Set(value);
    }

    [LanguageTranslator("Closing Time", "結束時間", "结束时间")]
    public DateTime EndTime
    {
        get => Get<DateTime>();
        set => Set(value);
    }

    [LanguageTranslator("Finished", "完成烘烤", "完成烘烤")]
    public bool TopIsFinished
    {
        get => Get<bool>();
        set => Set(value);
    }

    [LanguageTranslator("Temps", "溫度紀錄", "温度纪录")]
    public List<RecordTemperatures> RecordTemperatures
    {
        get => Get<List<RecordTemperatures>>()!;
        set => Set(value);
    }

    [LanguageTranslator("Events", "事件紀錄", "事件纪录")]
    public ObservableConcurrentCollection<LogEvent> EventList
    {
        get => Get<ObservableConcurrentCollection<LogEvent>>()!;
        set => Set(value);
    }

    [GPIgnore]
    public ObservableConcurrentCollection<ProductInfo> TopTempProducts
    {
        get => Get<ObservableConcurrentCollection<ProductInfo>>()!;
        set => Set(value);
    }
    [GPIgnore]
    public ObservableConcurrentCollection<ProductInfo> TempProducts
    {
        get => Get<ObservableConcurrentCollection<ProductInfo>>()!;
        set => Set(value);
    }

    [LanguageTranslator("Products", "產品", "产品")]
    public ObservableConcurrentCollection<ProductInfo> Products
    {
        get => Get<ObservableConcurrentCollection<ProductInfo>>()!;
        set => Set(value);
    }

    [LanguageTranslator("Qty", "數量", "數量")]
    public int Qty
    {
        get => Get<int>();
        set => Set(value);
    }

    public int TempQuantity => TempProducts.Sum(x => x.Quantity);
    public int TopTempQuantity => TopTempProducts.Sum(x => x.Quantity);
    public int Quantity => Products.Sum(x => x.Quantity);

    [GPIgnore]
    [LanguageTranslator("Total Time", "總烘烤時間", "总烘烤时间")]
    public double TotalRampTime
    {
        get => Get<double>();
        set => Set(value);
    }

    public BaseInfo()
    {
        Recipe = new PLC_Recipe();
        EventList = new ObservableConcurrentCollection<LogEvent>();
        RecordTemperatures = new List<RecordTemperatures>();
        Products = new ObservableConcurrentCollection<ProductInfo>();
        TempProducts = new ObservableConcurrentCollection<ProductInfo>();
        TopTempProducts = new ObservableConcurrentCollection<ProductInfo>();

        Products.CollectionChanged += (_, _) =>
                                      {
                                          NotifyPropertyChanged(nameof(Quantity));
                                      };
        TempProducts.CollectionChanged += (_, _) =>
                                          {
                                              NotifyPropertyChanged(nameof(TempQuantity));
                                          };
        TopTempProducts.CollectionChanged += (_, _) =>
                                          {
                                              NotifyPropertyChanged(nameof(TopTempQuantity));
                                          };
    }

    /// <summary>初始化清除資訊</summary>
    public virtual void Clear()
    {
        EventList.Clear();
        RecordTemperatures.Clear();
        Products.Clear();

        StartTime = new DateTime();
        EndTime = new DateTime();
        TotalRampTime = 0.0;
        TopIsFinished = false;
    }
}

public class BaseInfoWithChart : BaseInfo
{
    public ProcessChartModel ChartModel { get; }

    public BaseInfoWithChart() => ChartModel = new ProcessChartModel();

    public override void Clear()
    {
        base.Clear();
        ChartModel.Clear();
    }
}

[BsonIgnoreExtraElements]
public class ProcessInfo : BaseInfo, ILogData
{
    public ProcessInfo() { }

    public ProcessInfo(BaseInfo baseInfo) => baseInfo.CopyTo(this);

    /// <summary>匯出成Dictionary</summary>
    /// <param name="lng">語系</param>
    /// <returns></returns>
    public Dictionary<string, object?> ToDic(Language lng)
    {
        var type = GetType();

        return new Dictionary<string, object?>
               {
                   { type.GetProperty(nameof(AddedTime))?.GetName(lng)          ?? nameof(AddedTime), AddedTime },
                   { type.GetProperty(nameof(StartTime))?.GetName(lng)          ?? nameof(StartTime), StartTime },
                   { type.GetProperty(nameof(EndTime))?.GetName(lng)            ?? nameof(EndTime), EndTime },
                   { type.GetProperty(nameof(OperatorID))?.GetName(lng)         ?? nameof(OperatorID), OperatorID },
                   { type.GetProperty(nameof(LotID))?.GetName(lng)              ?? nameof(LotID), LotID },
                   { type.GetProperty(nameof(Recipe))?.GetName(lng)             ?? nameof(Recipe), Recipe },
                   { type.GetProperty(nameof(Products))?.GetName(lng)           ?? nameof(Products), Products },
                   { type.GetProperty(nameof(RecordTemperatures))?.GetName(lng) ?? nameof(RecordTemperatures), RecordTemperatures },
                   { type.GetProperty(nameof(EventList))?.GetName(lng)          ?? nameof(EventList), EventList }
               };
    }

    public IEnumerable<(DateTime AddedTime, int StationNumber, ProductInfo Product)> GetFlatInfos() => Products.Select(x => (AddedTime, StationNumber, x));

    #region 此區由TraceabilityView_ViewModel新增至資料庫時填入
    /// <summary>新增至資料庫的時間</summary>
    [OrderIndex(-10)]
    [LanguageTranslator("Recorded", "紀錄時間", "纪录时间")]
    public DateTime AddedTime { get; set; }

    /// <summary>PLC站號</summary>
    [GPIgnore]
    [OrderIndex(-9)]
    [LanguageTranslator("Oven No.", "烤箱序號", "烤箱序号")]
    public int StationNumber { get; set; }
    #endregion 此區由TraceabilityView_ViewModel新增至資料庫時填入
}