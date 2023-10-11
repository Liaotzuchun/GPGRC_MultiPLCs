using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GPGO_MultiPLCs.Models;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PooledCollections;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Serilog;

namespace GPGO_MultiPLCs.ViewModels;

public enum ChartMode
{
    ByDateTime = 0,
    ByPLC      = 1,
    ByPart     = 2,
    ByLot      = 3,
    ByLayer    = 4
}

/// <summary>生產紀錄追蹤</summary>
public class TraceabilityView_ViewModel : DataCollectionByDate<ProcessInfo>
{
    private const int back_row      = 14;
    private const int chart_row     = product_name_row;
    private const int log_name_row  = 15;
    private const int log_value_row = 16;

    private const    int          main_chart_row    = 1;
    private const    int          main_name_row     = 3;
    private const    int          main_value_row    = 4;
    private const    int          product_name_row  = 4;
    private const    int          product_value_row = 5;
    private const    int          recipe_name_row   = 1;
    private const    int          recipe_value_row  = 2;
    private readonly CategoryAxis categoryAxis1;
    private readonly CategoryAxis categoryAxis2;

    private readonly Dictionary<string, PropertyInfo> ProcessInfoProperties = typeof(ProcessInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(x => x.Name, x => x);
    private readonly Dictionary<string, PropertyInfo> ProductInfoProperties = typeof(ProductInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(x => x.Name, x => x);
    private readonly LinearAxis                       linearAxis;
    private Color[] ColorArray =
    {
        Color.Red,
        Color.DarkOrange,
        Color.Gold,
        Color.Lime,
        Color.DodgerBlue,
        Color.DarkOrchid,
        Color.Magenta,
        Color.Brown
    };
    private FrameworkElement? elementView;
    private OxyColor          bgcolor     = OxyColor.FromRgb(215, 230, 207);
    private OxyColor          bordercolor = OxyColor.FromRgb(174, 187, 168);
    private OxyColor          chartbg     = OxyColor.FromRgb(231, 246, 226);
    private OxyColor          fontcolor   = OxyColor.FromRgb(50,  70,  60);

    public Language     Language = Language.TW;
    public LogEvent?    SearchEvent;
    public ProcessInfo? SearchResult;
    public ProductInfo? SearchProduct;

    /// <summary>依據工單或料號搜尋</summary>
    public RelayCommand FindCommand { get; }

    /// <summary>輸出Excel報表</summary>
    public RelayCommand ToExcelCommand { get; }

    public RelayCommand LoadedCommand { get; }

    /// <summary>基於操作員的Filter</summary>
    public FilterGroup OpFilter { get; }

    /// <summary>基於料號的Filter</summary>
    public FilterGroup PartIDFilter { get; }

    /// <summary>基於PLC站號的Filter</summary>
    public FilterGroup OvenFilter { get; }

    /// <summary>基於配方的Filter</summary>
    public FilterGroup RecipeFilter { get; }

    public FilterGroup LayerFilter { get; }

    public FilterGroup FinishedFilter { get; }

    /// <summary>總量統計</summary>
    public int ProduceTotalCount => ViewResults?.Count > 0 ? ViewResults.Sum(x => x.Quantity) : 0;

    public ProcessChartModel ChartModel { get; }

    public PlotModel ResultView { get; }

    public ProcessInfo? SelectedProcessInfo
    {
        get => Get<ProcessInfo>();
        set
        {
            Set(value);
            if (value != null)
            {
                ChartModel.SetData(value.RecordTemperatures);
            }
            else
            {
                ChartModel.Clear();
            }
        }
    }

    public LogEvent SelectedLogEvent
    {
        set => ChartModel.SetAnnotation(value);
    }

    /// <summary>切換分類統計</summary>
    public ChartMode Mode
    {
        get => Get<ChartMode>();
        set
        {
            Set(value);

            UpdateChart(Date1, Date2);
        }
    }

    public int SelectedIndex
    {
        get => Get<int>();
        set
        {
            Set(value);
            EventIndex = -1;
        }
    }

    public int EventIndex
    {
        get => Get<int>();
        set => Set(value);
    }

    public bool ShowProducts
    {
        get => Get<bool>();
        set
        {
            Set(value);

            ProductIndex = -1;
        }
    }

    public int ProductIndex
    {
        get => Get<int>();
        set => Set(value);
    }

    /// <summary>顯示的資料列表</summary>
    public List<ProcessInfo>? ViewResults
    {
        get => Get<List<ProcessInfo>>();
        set => Set(value);
    }

    public TraceabilityView_ViewModel(IDataBase<ProcessInfo> db, IDialogService dialog) : base(db)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        ChartModel = new ProcessChartModel();

        LoadedCommand = new RelayCommand(e =>
                                         {
                                             if (e is FrameworkElement el)
                                             {
                                                 elementView = el;
                                                 ChartModel.SetFrameworkElement(el);
                                             }
                                         });

        FindCommand = new RelayCommand(async _ =>
                                       {
                                           var (result1, input1) = await dialog.ShowWithInput(new Dictionary<Language, string>
                                                                                              {
                                                                                                  { Language.TW, "請輸入欲搜尋之料號：" },
                                                                                                  { Language.CHS, "请输入欲搜寻之料号：" },
                                                                                                  { Language.EN, "Please enter the PartID you want to find：" }
                                                                                              },
                                                                                              new Dictionary<Language, string>
                                                                                              {
                                                                                                  { Language.TW, "搜尋" },
                                                                                                  { Language.CHS, "搜寻" },
                                                                                                  { Language.EN, "Find" }
                                                                                              });

                                           var (result2, input2) = await dialog.ShowWithInput(new Dictionary<Language, string>
                                                                                              {
                                                                                                  { Language.TW, "請輸入欲搜尋之批號：" },
                                                                                                  { Language.CHS, "请输入欲搜寻之批号：" },
                                                                                                  { Language.EN, "Please enter the LotID you want to find：" }
                                                                                              },
                                                                                              new Dictionary<Language, string>
                                                                                              {
                                                                                                  { Language.TW, "搜尋" },
                                                                                                  { Language.CHS, "搜寻" },
                                                                                                  { Language.EN, "Find" }
                                                                                              });

                                           if ((!result1 && !result2) || (string.IsNullOrEmpty(input1.ToString()) && string.IsNullOrEmpty(input2.ToString())))
                                           {
                                               return;
                                           }

                                           Standby = false;

                                           SearchResult = (await DataCollection.FindAsync(x => x.Products.Any(y => (!result1 ||
                                                                                                                    y.PartID.Contains(input1.ToString())) &&
                                                                                                                   (!result2 ||
                                                                                                                    y.LotID.Contains(input2.ToString())))))
                                              .LastOrDefault();

                                           Standby = true;

                                           if (SearchResult == null)
                                           {
                                               dialog.Show(new Dictionary<Language, string>
                                                           {
                                                               { Language.TW, "查無此產品！" },
                                                               { Language.CHS, "查无此产品！" },
                                                               { Language.EN, "No product be found!" }
                                                           },
                                                           DialogMsgType.Alarm);
                                           }
                                           else
                                           {
                                               SearchProduct = SearchResult.Products.Where(x => x.PartID.Contains(input1.ToString()) && x.LotID.Contains(input2.ToString())).OrderBy(x => x.Layer).FirstOrDefault();

                                               Date1 = SearchResult.AddedTime.Date;
                                           }
                                       });

        ToExcelCommand = new RelayCommand(async _ =>
                                          {
                                              var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Reports";
                                              if (await SaveToCSV(path))
                                              {
                                                  dialog?.Show(new Dictionary<Language, string>
                                                              {
                                                                  { Language.TW, $"檔案已輸出至\n{path}" },
                                                                  { Language.CHS, $"档案已输出至\n{path}" },
                                                                  { Language.EN, $"The file has been output to\n{path}" }
                                                              },
                                                               TimeSpan.FromSeconds(6));
                                              }
                                          });

        linearAxis = new LinearAxis
        {
            IsPanEnabled = false,
            IsZoomEnabled = false,
            FontSize = 12,
            TitleColor = fontcolor,
            TickStyle = TickStyle.Inside,
            MajorGridlineStyle = LineStyle.Dot,
            //MajorStep = 100,
            MinorGridlineStyle = LineStyle.None,
            MajorTickSize = 0,
            MinorTickSize = 0,
            //MinorStep = 10,
            AxislineStyle = LineStyle.Solid,
            AxislineColor = bordercolor,
            MajorGridlineColor = bordercolor,
            MinorGridlineColor = bordercolor,
            TicklineColor = bordercolor,
            ExtraGridlineColor = bordercolor,
            ExtraGridlineStyle = LineStyle.None,
            TextColor = fontcolor,
            Minimum = 0,
            MaximumPadding = 0.15,
            Position = AxisPosition.Left,
            Key = "0"
            //Maximum = 1000
        };

        categoryAxis1 = new CategoryAxis
        {
            IsPanEnabled = false,
            IsZoomEnabled = false,
            FontSize = 12,
            TitleColor = fontcolor,
            MajorGridlineColor = bordercolor,
            MinorGridlineColor = bordercolor,
            TicklineColor = bordercolor,
            ExtraGridlineColor = bordercolor,
            TextColor = fontcolor,
            TickStyle = TickStyle.Inside,
            MajorTickSize = 0,
            MinorTickSize = 0,
            AxislineStyle = LineStyle.Solid,
            ExtraGridlineStyle = LineStyle.None,
            AxislineColor = bordercolor,
            GapWidth = 1,
            MinorStep = 1,
            MajorStep = 1,
            Position = AxisPosition.Bottom,
            Key = "1"
        };

        categoryAxis2 = new CategoryAxis
        {
            IsPanEnabled = false,
            IsZoomEnabled = false,
            IsAxisVisible = false,
            Position = AxisPosition.Bottom,
            Key = "2"
        };

        ResultView = new PlotModel
        {
            DefaultFont = "Microsoft JhengHei",
            PlotAreaBackground = chartbg,
            PlotAreaBorderColor = bordercolor,
            PlotAreaBorderThickness = new OxyThickness(0, 1, 1, 1),
            PlotMargins = new OxyThickness(35, 0, 0, 20),
            TextColor = fontcolor
        };

        ResultView.Legends.Add(new Legend
        {
            LegendTitle = ProductInfoProperties[nameof(ProductInfo.PartID)].GetName(Language),
            LegendTitleColor = fontcolor,
            LegendTextColor = fontcolor,
            LegendBorder = bordercolor,
            LegendBackground = bgcolor,
            LegendPlacement = LegendPlacement.Outside,
            LegendPosition = LegendPosition.RightTop,
            LegendOrientation = LegendOrientation.Vertical,
            LegendFontSize = 12,
            LegendTitleFontSize = 12,
            //LegendItemOrder = LegendItemOrder.Reverse,
            LegendMargin = 4,
            LegendPadding = 5
        });

        ResultView.Axes.Add(linearAxis);
        ResultView.Axes.Add(categoryAxis2);
        ResultView.Axes.Add(categoryAxis1);

        void UpdateAct()
        {
            UpdateViewResult();
            UpdateChart(Date1, Date2);
        }

        SelectedIndex = -1;
        OvenFilter = new FilterGroup(UpdateAct);
        RecipeFilter = new FilterGroup(UpdateAct);
        PartIDFilter = new FilterGroup(UpdateAct);
        OpFilter = new FilterGroup(UpdateAct);
        LayerFilter = new FilterGroup(UpdateAct);
        FinishedFilter = new FilterGroup(UpdateAct);

        ResultsChanged += async e =>
                          {
                              OvenFilter.Filter = e?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList() ?? new List<EqualFilter>();
                              RecipeFilter.Filter = e?.Select(x => x.Recipe.RecipeName).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList() ?? new List<EqualFilter>();
                              OpFilter.Filter = e?.Select(x => x.OperatorID).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList() ?? new List<EqualFilter>();
                              PartIDFilter.Filter = e?.SelectMany(x => x.Products.Select(y => y.PartID)).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList() ?? new List<EqualFilter>();
                              LayerFilter.Filter = e?.SelectMany(x => x.Products.Select(y => y.Layer)).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList() ?? new List<EqualFilter>();
                              FinishedFilter.Filter = e?.Select(x => x.TopIsFinished).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList() ?? new List<EqualFilter>();

                              await Task.Delay(150);

                              if (SearchResult != null)
                              {
                                  SelectedIndex = ViewResults?.Count > 0 ? ViewResults.FindIndex(x => x.StationNumber == SearchResult.StationNumber && x.AddedTime == SearchResult.AddedTime) : -1;
                                  SearchResult = null;

                                  if (SelectedIndex == -1)
                                  {
                                      dialog.Show(new Dictionary<Language, string>
                                                  {
                                                      { Language.TW, "查無資料！" },
                                                      { Language.CHS, "查无资料！" },
                                                      { Language.EN, "No data be found!" }
                                                  },
                                                  DialogMsgType.Alarm);
                                  }
                              }
                              else
                              {
                                  SelectedIndex = -1;
                              }

                              if (SearchEvent != null && ViewResults?.Count > 0 && SelectedIndex > -1)
                              {
                                  using var matchevents = ViewResults[SelectedIndex]
                                                         .EventList
                                                         .Select((x, i) => (index: i, value: x))
                                                         .Where(x => x.value.Value.Equals(SearchEvent.Value) && x.value.Description == SearchEvent.Description)
                                                         .ToPooledList();

                                  if (matchevents.Count > 0)
                                  {
                                      EventIndex = matchevents.OrderBy(x => Math.Abs((x.value.AddedTime - SearchEvent.AddedTime).TotalSeconds))
                                                              .First()
                                                              .index;
                                  }
                                  else
                                  {
                                      dialog.Show(new Dictionary<Language, string>
                                                  {
                                                      { Language.TW, "查無事件！" },
                                                      { Language.CHS, "查无事件！" },
                                                      { Language.EN, "No event found!" }
                                                  },
                                                  DialogMsgType.Alarm);
                                  }

                                  SearchEvent = null;
                              }
                              else if (SearchProduct != null && ViewResults?.Count > 0 && SelectedIndex > -1)
                              {
                                  ShowProducts = true;
                                  ProductIndex = ViewResults[SelectedIndex]
                                                .Products
                                                .Select((x, i) => (index: i, value: x))
                                                .OrderBy(x => x.value.Layer)
                                                .FirstOrDefault(x => x.value.PartID == SearchProduct.PartID &&
                                                                     x.value.LotID == SearchProduct.LotID &&
                                                                     x.value.Layer == SearchProduct.Layer)
                                                .index;

                                  SearchProduct = null;
                              }
                              else
                              {
                                  EventIndex = -1;
                              }

                              UpdateAct();
                          };

        BeginIndexChanged += _ => UpdateAct();
        EndIndexChanged += _ => UpdateAct();
    }

    private void CellLink(ExcelRangeBase cells)
    {
        cells.Style.Font.Color.SetColor(Color.Blue);
        cells.Style.Font.UnderLine = false;
    }

    private void CellHeader(ExcelRangeBase cells)
    {
        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        cells.Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);
        cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
    }

    private void CellBorder(ExcelRangeBase cells)
    {
        cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
    }

    private async Task<bool> SaveToCSV(string path)
    {
        Standby = false;

        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CSV輸出資料夾無法創建");

                return false;
            }
        }

        if (ViewResults != null &&
            typeof(LogEvent).GetProperty(nameof(LogEvent.AddedTime)) is { } addedTime &&
            typeof(LogEvent).GetProperty(nameof(LogEvent.StationNumber)) is { } stationNumber &&
            typeof(LogEvent).GetProperty(nameof(LogEvent.Type)) is { } type &&
            typeof(LogEvent).GetProperty(nameof(LogEvent.Description2)) is { } description2 &&
            typeof(LogEvent).GetProperty(nameof(LogEvent.Value)) is { } value)
        {
            var csv = ViewResults.ToCSV(Language,
                                        addedTime,
                                        stationNumber,
                                        type,
                                        description2,
                                        value);

            try
            {
                using var outputFile = new StreamWriter($"{path}\\{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.csv", false, Encoding.UTF8);
                await outputFile.WriteAsync(csv);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "輸出CSV失敗");

                return false;
            }
        }

        Standby = true;

        return true;
    }

    /// <summary>更新統計圖</summary>
    /// <param name="date1">開始日期</param>
    /// <param name="date2">結束日期</param>
    private void UpdateChart(DateTime date1, DateTime date2)
    {
        if (elementView != null)
        {
            if (elementView.TryFindResource("LightColor") is System.Windows.Media.Color lc)
            {
                chartbg = OxyColor.FromRgb(lc.R, lc.G, lc.B);
            }

            if (elementView.TryFindResource("WindowBackgroundColor4") is System.Windows.Media.Color wb4)
            {
                bordercolor = OxyColor.FromRgb(wb4.R, wb4.G, wb4.B);
            }

            if (elementView.TryFindResource("WindowBackgroundColor6") is System.Windows.Media.Color wb6)
            {
                bgcolor = OxyColor.FromRgb(wb6.R, wb6.G, wb6.B);
            }

            if (elementView.TryFindResource("BaseForegroundColor") is System.Windows.Media.Color bf)
            {
                fontcolor = OxyColor.FromRgb(bf.R, bf.G, bf.B);
            }

            linearAxis.TitleColor = fontcolor;
            linearAxis.AxislineColor = bordercolor;
            linearAxis.MajorGridlineColor = bordercolor;
            linearAxis.MinorGridlineColor = bordercolor;
            linearAxis.TicklineColor = bordercolor;
            linearAxis.ExtraGridlineColor = bordercolor;
            linearAxis.TextColor = fontcolor;
            categoryAxis1.TitleColor = fontcolor;
            categoryAxis1.MajorGridlineColor = bordercolor;
            categoryAxis1.MinorGridlineColor = bordercolor;
            categoryAxis1.TicklineColor = bordercolor;
            categoryAxis1.ExtraGridlineColor = bordercolor;
            categoryAxis1.TextColor = fontcolor;
            categoryAxis1.AxislineColor = bordercolor;
            ResultView.PlotAreaBackground = chartbg;
            ResultView.PlotAreaBorderColor = bordercolor;
            ResultView.Legends[0].LegendTitleColor = fontcolor;
            ResultView.Legends[0].LegendTextColor = fontcolor;
            ResultView.Legends[0].LegendBorder = bordercolor;
            ResultView.Legends[0].LegendBackground = bgcolor;
        }

        try
        {
            ResultView.Series.Clear();
        }
        catch (Exception)
        {
            // ignored
        }

        ResultView.IsLegendVisible = false;
        categoryAxis1.Labels.Clear();
        categoryAxis2.Labels.Clear();

        if (ViewResults?.Count > 0)
        {
            using var finishedResults = ViewResults.SelectMany(x => x.GetFlatInfos()).ToPooledList();

            var ByDate = date2 - date1 > TimeSpan.FromDays(1);
            using var result2 = finishedResults
                               .GroupBy(x => Mode == ChartMode.ByPart ?
                                                 x.Product.Layer.ToString() :
                                                 x.Product.PartID)
                               .OrderBy(x => x.Key)
                               .Select(x => (x.Key, x))
                               .ToPooledList();

            var NoLayer2   = result2.Count > 20 && Mode >= ChartMode.ByPart;
            var categories = new List<string>();

            using var result1 = finishedResults.GroupBy(x =>
                                                        {
                                                            return Mode switch
                                                            {
                                                                ChartMode.ByPLC   => x.StationNumber.ToString(),
                                                                ChartMode.ByPart  => x.Product.PartID,
                                                                ChartMode.ByLot   => x.Product.LotID,
                                                                ChartMode.ByLayer => x.Product.Layer.ToString(),
                                                                _                 => ByDate ? x.AddedTime.Date.ToString("MM/dd") : $"{x.AddedTime.Hour:00}:00"
                                                            };
                                                        })
                                               .OrderBy(x => x.Key)
                                               .Select(x => (x.Key, x.Sum(y => y.Product.Quantity)))
                                               .ToPooledList();

            categoryAxis1.FontSize = result1.Count > 20 ? 9 : 12;

            var color_step_1 = 0.9 / result1.Count;

            for (var i = 0; i < result1.Count; i++)
            {
                var (result, count) = result1[i];
                categories.Add(result);
                categoryAxis1.Labels.Add(result);
                categoryAxis2.Labels.Add(result);

                var cs = new BarSeries
                {
                    FontSize          = result1.Count > 20 ? 8 : 10,
                    LabelFormatString = "{0}",
                    TextColor         = fontcolor,
                    IsStacked         = true,
                    StrokeThickness   = 0,
                    StrokeColor       = NoLayer2 ? bordercolor : OxyColors.Transparent,
                    FillColor         = NoLayer2 ? OxyColor.FromHsv(i * color_step_1, 0.9, 0.9) : OxyColors.Transparent,
                    YAxisKey          = "2",
                    XAxisKey          = "0"
                };

                cs.Items.Add(new BarItem(count, i));
                ResultView.Series.Add(cs);
            }

            if (!NoLayer2)
            {
                ResultView.IsLegendVisible = true;
                ResultView.Legends[0].LegendTitle = Mode == ChartMode.ByPart ?
                                                        ProductInfoProperties[nameof(ProductInfo.Layer)].GetName(Language) :
                                                        ProductInfoProperties[nameof(ProductInfo.PartID)].GetName(Language);

                var color_step_2 = 0.9 / result2.Count;

                for (var i = 0; i < result2.Count; i++)
                {
                    var (cat, info) = result2[i];

                    var ccs = new BarSeries
                    {
                        FontSize          = 10,
                        LabelFormatString = result2.Count > 10 || categories.Count > 20 ? "" : "{0}",
                        LabelPlacement    = LabelPlacement.Middle,
                        TextColor         = OxyColors.White,
                        Title             = cat,
                        IsStacked         = true,
                        StrokeThickness   = 0,
                        StrokeColor       = bordercolor,
                        FillColor         = OxyColor.FromHsv(i * color_step_2, 0.9, 0.9),
                        YAxisKey          = "1",
                        XAxisKey          = "0"
                    };

                    for (var j = 0; j < categories.Count; j++)
                    {
                        var val = info.Where(x => Mode switch
                                                  {
                                                      ChartMode.ByPLC   => x.StationNumber.ToString() == categories[j],
                                                      ChartMode.ByPart  => x.Product.PartID           == categories[j],
                                                      ChartMode.ByLot   => x.Product.LotID            == categories[j],
                                                      ChartMode.ByLayer => x.Product.Layer.ToString() == categories[j],
                                                      _                 => ByDate ? x.AddedTime.Date.ToString("MM/dd") == categories[j] : $"{x.AddedTime.Hour:00}:00" == categories[j]
                                                  })
                                      .Sum(x => x.Product.Quantity);
                        if (val > 0)
                        {
                            ccs.Items.Add(new BarItem(val, j));
                        }
                    }

                    ResultView.Series.Add(ccs);
                }
            }
        }

        lock (ResultView.SyncRoot)
        {
            ResultView.InvalidatePlot(true);
        }
    }

    private void UpdateViewResult()
    {
        if (Results == null || Results.Count == 0 || EndIndex < BeginIndex)
        {
            ViewResults = null;
        }
        else
        {
            var min = Math.Max(0, BeginIndex);
            var max = Math.Min(Results.Count - 1, EndIndex);

            ViewResults = Results.GetRange(min, max - min + 1)
                                 .Where(x => OvenFilter.Check(x.StationNumber) &&
                                             RecipeFilter.Check(x.Recipe?.RecipeName ?? string.Empty) &&
                                             OpFilter.Check(x.OperatorID) &&
                                             FinishedFilter.Check(x.TopIsFinished) || FinishedFilter.Check(x.BottomIsFinished) &&
                                             x.Products.Any(y => PartIDFilter.Check(y.PartID)))
                                 .OrderByDescending(x => x.AddedTime)
                                 .ToList();
        }

        NotifyPropertyChanged(nameof(ProduceTotalCount));
    }

    /// <summary>新增至資料庫</summary>
    /// <param name="index">PLC序號，由0開始(寫入時會自動+1)</param>
    /// <param name="info">紀錄資訊</param>
    /// <param name="dateTime">紀錄時間，預設為當下時間，帶入default(DateTime)同樣為當下時間</param>
    /// <param name="UpdateResult">決定是否更新Ram Data</param>
    public async Task<bool> AddToDBAsync(int index, ProcessInfo info, DateTime dateTime = default, bool UpdateResult = false)
    {
        info.StationNumber = index + 1;
        info.AddedTime = dateTime == default ? DateTime.Now : dateTime;

        try
        {
            if (!await DataCollection.AddAsync(info))
            {
                return false;
            }

            if (UpdateResult)
            {
                Results = await DataCollection.FindAsync(x => x.AddedTime >= Date1 && x.AddedTime < Date2.AddDays(1));
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "生產紀錄寫入資料庫失敗");
            return false;
        }
    }

    public bool AddToDB(int index, ProcessInfo info, DateTime dateTime = default, bool UpdateResult = false)
    {
        info.StationNumber = index + 1;
        info.AddedTime = dateTime == default ? DateTime.Now : dateTime;

        try
        {
            if (!DataCollection.Add(info))
            {
                return false;
            }

            if (UpdateResult)
            {
                Results = DataCollection.Find(x => x.AddedTime >= Date1 && x.AddedTime < Date2.AddDays(1));
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "生產紀錄寫入資料庫失敗");
            return false;
        }
    }

    /// <summary>新增至資料庫</summary>
    /// <param name="index">PLC序號，由0開始(寫入時會自動+1)</param>
    /// <param name="infos">紀錄資訊</param>
    /// <param name="dateTime">紀錄時間，預設為當下時間，帶入default(DateTime)同樣為當下時間</param>
    /// <param name="UpdateResult">決定是否更新Ram Data</param>
    public async Task<bool> AddToDBAsync(int index, ICollection<ProcessInfo> infos, DateTime dateTime = default, bool UpdateResult = false)
    {
        var dt = dateTime == default ? DateTime.Now : dateTime;

        foreach (var info in infos)
        {
            info.StationNumber = index + 1;
            info.AddedTime = dt;
        }

        try
        {
            if (!await DataCollection.AddManyAsync(infos))
            {
                return false;
            }

            if (UpdateResult)
            {
                Results = await DataCollection.FindAsync(x => x.AddedTime >= Date1 && x.AddedTime < Date2.AddDays(1));
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "生產紀錄寫入資料庫失敗");
            return false;
        }
    }

    public bool AddToDB(int index, ICollection<ProcessInfo> infos, DateTime dateTime = default, bool UpdateResult = false)
    {
        var dt = dateTime == default ? DateTime.Now : dateTime;

        foreach (var info in infos)
        {
            info.StationNumber = index + 1;
            info.AddedTime = dt;
        }

        try
        {
            if (!DataCollection.AddMany(infos))
            {
                return false;
            }

            if (UpdateResult)
            {
                Results = DataCollection.Find(x => x.AddedTime >= Date1 && x.AddedTime < Date2.AddDays(1));
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "生產紀錄寫入資料庫失敗");
            return false;
        }
    }

    public async Task<int> CheckProductions(int index)
    {
        try
        {
            var date1  = DateTime.Today.Date;
            var date2  = date1.AddDays(1);
            var result = await DataCollection.FindAsync(x => x.StationNumber == index + 1 && x.AddedTime >= date1 && x.AddedTime < date2);

            return result.Where(x => x.TopIsFinished).Sum(x => x.Quantity);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "");

            return 0;
        }
    }

    public Task<ProcessInfo> FindInfo(int station, DateTime time) => DataCollection.FindOneAsync(x => x.StationNumber == station && x.StartTime < time && x.EndTime > time);

    public Task<List<ProcessInfo>> FindInfo(string lotid) => DataCollection.FindAsync(x => x.Products.Any(y => y.LotID == lotid));
}