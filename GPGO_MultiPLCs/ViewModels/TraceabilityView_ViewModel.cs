using GPGO_MultiPLCs.Models;
using GPMVVM.Helpers;
using GPMVVM.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GPMVVM.PooledCollections;

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

    private const int main_chart_row    = 1;
    private const int main_name_row     = 3;
    private const int main_value_row    = 4;
    private const int recipe_name_row   = 1;
    private const int recipe_value_row  = 2;
    private const int product_name_row  = 4;
    private const int product_value_row = 5;
    private const int chart_row         = product_name_row;
    private const int back_row          = 14;
    private const int log_name_row      = 15;
    private const int log_value_row     = 16;

    private readonly Dictionary<string, PropertyInfo> ProcessInfoProperties = typeof(ProcessInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(x => x.Name, x => x);
    private readonly Dictionary<string, PropertyInfo> ProductInfoProperties = typeof(ProductInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(x => x.Name, x => x);
    private          FrameworkElement                 elementView;
    private          OxyColor                         chartbg     = OxyColor.FromRgb(231, 246, 226);
    private          OxyColor                         bgcolor     = OxyColor.FromRgb(215, 230, 207);
    private          OxyColor                         bordercolor = OxyColor.FromRgb(174, 187, 168);
    private          OxyColor                         fontcolor   = OxyColor.FromRgb(50,  70,  60);
    private readonly LinearAxis                       linearAxis;
    private readonly CategoryAxis                     categoryAxis1;
    private readonly CategoryAxis                     categoryAxis2;

    public Language    Language = Language.TW;
    public LogEvent    SearchEvent;
    public ProcessInfo SearchResult;
    public ProductInfo SearchProduct;

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

    public ProcessInfo SelectedProcessInfo
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
    public List<ProcessInfo> ViewResults
    {
        get => Get<List<ProcessInfo>>();
        set => Set(value);
    }

    /// <summary>新增至資料庫</summary>
    /// <param name="index">PLC序號，由0開始(寫入時會自動+1)</param>
    /// <param name="info">紀錄資訊</param>
    /// <param name="dateTime">紀錄時間，預設為當下時間，帶入default(DateTime)同樣為當下時間</param>
    /// <param name="UpdateResult">決定是否更新Ram Data</param>
    public async ValueTask<bool> AddToDBAsync(int index, ProcessInfo info, DateTime dateTime = default, bool UpdateResult = false)
    {
        info.StationNumber = index + 1;
        info.AddedTime     = dateTime == default ? DateTime.Now : dateTime;

        try
        {
            if (!await DataCollection.AddAsync(info)) return false;

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
        info.AddedTime     = dateTime == default ? DateTime.Now : dateTime;

        try
        {
            if (!DataCollection.Add(info)) return false;

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
    public async ValueTask<bool> AddToDBAsync(int index, ICollection<ProcessInfo> infos, DateTime dateTime = default, bool UpdateResult = false)
    {
        var dt = dateTime == default ? DateTime.Now : dateTime;

        foreach (var info in infos)
        {
            info.StationNumber = index + 1;
            info.AddedTime     = dt;
        }

        try
        {
            if (!await DataCollection.AddManyAsync(infos)) return false;

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
            info.AddedTime     = dt;
        }

        try
        {
            if (!DataCollection.AddMany(infos)) return false;

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

            return result.Where(x => x.IsFinished).Sum(x => x.Quantity);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "");

            return 0;
        }
    }

    public ValueTask<ProcessInfo> FindInfo(int station, DateTime time) => DataCollection.FindOneAsync(x => x.StationNumber == station && x.StartTime < time && x.EndTime > time);

    public ValueTask<List<ProcessInfo>> FindInfo(string lotid) => DataCollection.FindAsync(x => x.Products.Any(y => y.LotID == lotid));

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
        cells.Style.Border.Left.Style   = ExcelBorderStyle.Thin;
        cells.Style.Border.Right.Style  = ExcelBorderStyle.Thin;
        cells.Style.Border.Top.Style    = ExcelBorderStyle.Thin;
    }

    private void CellBorder(ExcelRangeBase cells)
    {
        cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Left.Style   = ExcelBorderStyle.Thin;
        cells.Style.Border.Right.Style  = ExcelBorderStyle.Thin;
        cells.Style.Border.Top.Style    = ExcelBorderStyle.Thin;
    }

    /// <summary>將目前顯示資料輸出至Excel OpenXML格式檔案</summary>
    /// <param name="path">資料夾路徑</param>
    private async Task<bool> SaveToExcel(string path)
    {
        Standby = false;

        var result = false;

        if (ViewResults?.Count > 0)
        {
            await Task.Factory.StartNew(() =>
                                        {
                                            if (!Directory.Exists(path))
                                            {
                                                try
                                                {
                                                    Directory.CreateDirectory(path);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Log.Error(ex, "EXCEL輸出資料夾無法創建");
                                                    return;
                                                }
                                            }

                                            var created = DateTime.Now;
                                            var x       = ViewResults.Count / 500;     //! 檔案數
                                            var y       = ViewResults.Count - 500 * x; //! 剩餘數

                                            if (x > 0 && y <= 100)
                                            {
                                                y += 500;
                                            }
                                            else
                                            {
                                                x += 1;
                                            }

                                            Parallel.For(0,
                                                         x,
                                                         index =>
                                                         {
                                                             var fi    = new FileInfo($"{path}\\{created:yyyy-MM-dd-HH-mm-ss-fff}({index + 1}).xlsm");
                                                             var datas = ViewResults.GetRange(500 * index, index == x                    - 1 ? y : 500);
                                                             var n     = datas.Count;
                                                             var xlwb  = new ExcelPackage();
                                                             xlwb.Workbook.CreateVBAProject();
                                                             var wsht = xlwb.Workbook.Worksheets.Add($"{n}{(n <= 1 ? " result" : " results")}");
                                                             wsht.View.ShowGridLines = false;
                                                             wsht.View.FreezePanes(main_value_row, 1);
                                                             wsht.Row(main_chart_row).Height = 225;

                                                             using var  keys      = datas[0].ToDic(Language).Keys.ToPooledList();
                                                             using var  temp_keys = new PooledList<string>();
                                                             var        max_count = 0;
                                                             ExcelRange cells;

                                                             //! 生產紀錄各屬性名稱
                                                             for (var i = 0; i < keys.Count; i++)
                                                             {
                                                                 cells       = wsht.Cells[main_name_row, i + 1];
                                                                 cells.Value = keys[i];
                                                                 CellHeader(cells);
                                                             }

                                                             //! 加入每筆生產紀錄
                                                             for (var i = 0; i < n; i++)
                                                             {
                                                                 using var values     = datas[i].ToDic(Language).Values.ToPooledList();                            //! 生產資訊（時間、人員、機台資訊）
                                                                 using var temps      = datas[i].RecordTemperatures.Select(o => o.ToDic(Language)).ToPooledList(); //! 溫度紀錄
                                                                 using var logs       = datas[i].EventList.Select(o => o.ToDictionary(Language)).ToPooledList();   //! 事件紀錄
                                                                 using var recipe     = datas[i].Recipe.ToDictionary(Language).ToPooledList();                     //! 配方
                                                                 using var products   = datas[i].Products.Select(o => o.ToDic(Language)).ToPooledList();           //! 產品資訊
                                                                 var       sheet_name = $"Records {i + 1}";                                                        //! 每筆紀錄另開一張表

                                                                 if (i == 0)
                                                                 {
                                                                     temp_keys.AddRange(temps[0].Keys);
                                                                 }

                                                                 if (temps.Count > max_count)
                                                                 {
                                                                     max_count = temps.Count;
                                                                 }

                                                                 var ev_col = (temp_keys.Count + 2);

                                                                 //! 每個屬性值填入
                                                                 for (var j = 0; j < values.Count; j++)
                                                                 {
                                                                     cells = wsht.Cells[i + main_value_row, j + 1];
                                                                     if (values[j] is DateTime date)
                                                                     {
                                                                         cells.Value                     = date.ToOADate();
                                                                         cells.Style.Numberformat.Format = "yyyy/MM/dd HH:mm:ss";
                                                                     }
                                                                     else if (values[j] is TimeSpan timeSpan)
                                                                     {
                                                                         cells.Value                     = timeSpan;
                                                                         cells.Style.Numberformat.Format = "[h]:mm:ss";
                                                                     }
                                                                     else if (values[j] is PLC_Recipe _recipe)
                                                                     {
                                                                         cells.Value   = _recipe.RecipeName;
                                                                         cells.Formula = $"HYPERLINK(\"#'{sheet_name}'!$A${recipe_value_row}\",\"{cells.Value}\")";
                                                                         CellLink(cells);
                                                                     }
                                                                     else if (values[j] is IEnumerable<ProductInfo> ps)
                                                                     {
                                                                         var count = ps.Sum(o => o.Quantity);
                                                                         var unit = Language switch
                                                                                    {
                                                                                        Language.EN => count > 1 ? "pcs" : "pc",
                                                                                        _           => "片"
                                                                                    };

                                                                         cells.Value   = $"{count}{unit}";
                                                                         cells.Formula = $"HYPERLINK(\"#'{sheet_name}'!$A${product_value_row}\",\"{cells.Value}\")";
                                                                         CellLink(cells);
                                                                     }
                                                                     else if (values[j] is IEnumerable<LogEvent> evs)
                                                                     {
                                                                         var count = evs.Count();
                                                                         var unit = Language switch
                                                                                    {
                                                                                        Language.EN  => count > 1 ? "logs" : "log",
                                                                                        Language.CHS => "笔",
                                                                                        _            => "筆"
                                                                                    };

                                                                         cells.Value   = $"{count}{unit}";
                                                                         cells.Formula = $"HYPERLINK(\"#'{sheet_name}'!${ev_col.GetExcelColumnName()}${log_value_row}\",\"{cells.Value}\")";
                                                                         CellLink(cells);
                                                                     }
                                                                     else if (values[j] is IEnumerable<RecordTemperatures> ts)
                                                                     {
                                                                         var count = ts.Count();
                                                                         var unit = Language switch
                                                                                    {
                                                                                        Language.EN  => count > 1 ? "records" : "record",
                                                                                        Language.CHS => "笔",
                                                                                        _            => "筆"
                                                                                    };

                                                                         cells.Value   = $"{count}{unit}";
                                                                         cells.Formula = $"HYPERLINK(\"#'{sheet_name}'!$A${log_value_row}\",\"{cells.Value}\")";
                                                                         CellLink(cells);
                                                                     }
                                                                     else
                                                                     {
                                                                         cells.Value = values[j];
                                                                     }

                                                                     CellBorder(cells);
                                                                 }

                                                                 var record_sht = xlwb.Workbook.Worksheets.Add(sheet_name);
                                                                 record_sht.View.ShowGridLines = false;
                                                                 record_sht.View.FreezePanes(log_value_row, 1);
                                                                 //record_sht.Row(chart_row).Height = 225;

                                                                 cells         = record_sht.Cells[back_row, 1];
                                                                 cells.Formula = $"HYPERLINK(\"#'{wsht.Name}'!$A${i + main_value_row}\",\"<<<<<<Back\")";
                                                                 CellLink(cells);

                                                                 //! 加入配方
                                                                 for (var j = 0; j < recipe.Count; j++)
                                                                 {
                                                                     cells       = record_sht.Cells[recipe_name_row, j + 1];
                                                                     cells.Value = recipe[j].Key;
                                                                     CellHeader(cells);
                                                                     cells       = record_sht.Cells[recipe_value_row, j + 1];
                                                                     cells.Value = recipe[j].Value;
                                                                     CellBorder(cells);
                                                                 }

                                                                 //! 加入產品屬性名稱
                                                                 for (var j = 0; j < products[0].Count; j++)
                                                                 {
                                                                     cells       = record_sht.Cells[product_name_row, j + 1];
                                                                     cells.Value = products[0].ElementAt(j).Key;
                                                                     CellHeader(cells);
                                                                 }

                                                                 //! 加入產品資料
                                                                 for (var j = 0; j < products.Count; j++)
                                                                 {
                                                                     for (var k = 0; k < products[0].Count; k++)
                                                                     {
                                                                         cells       = record_sht.Cells[product_value_row + j, k + 1];
                                                                         cells.Value = products[j].ElementAt(k).Value;
                                                                         CellBorder(cells);
                                                                     }
                                                                 }

                                                                 //! 加入事件屬性名稱
                                                                 for (var j = 0; j < logs[0].Count; j++)
                                                                 {
                                                                     cells       = record_sht.Cells[log_name_row, j + ev_col];
                                                                     cells.Value = logs[0].ElementAt(j).Key;
                                                                     CellHeader(cells);
                                                                 }

                                                                 //! 加入事件資料
                                                                 for (var j = 0; j < logs.Count; j++)
                                                                 {
                                                                     for (var k = 0; k < logs[0].Count; k++)
                                                                     {
                                                                         var val = logs[j].ElementAt(k).Value;
                                                                         cells = record_sht.Cells[log_value_row + j, k + ev_col];

                                                                         if (val is DateTime date)
                                                                         {
                                                                             cells.Value                     = date.ToOADate();
                                                                             cells.Style.Numberformat.Format = "yyyy/MM/dd HH:mm:ss";
                                                                         }
                                                                         else if (val is TimeSpan)
                                                                         {
                                                                             cells.Value                     = val;
                                                                             cells.Style.Numberformat.Format = "[h]:mm:ss";
                                                                         }
                                                                         else
                                                                         {
                                                                             cells.Value = val;
                                                                         }

                                                                         CellBorder(cells);
                                                                     }
                                                                 }

                                                                 //! 加入溫度屬性名稱
                                                                 for (var j = 0; j < temp_keys.Count; j++)
                                                                 {
                                                                     cells       = record_sht.Cells[log_name_row, j + 1];
                                                                     cells.Value = temp_keys[j];
                                                                     CellHeader(cells);
                                                                 }

                                                                 //! 加入溫度數值
                                                                 for (var j = 0; j < temps.Count; j++)
                                                                 {
                                                                     for (var k = 0; k < temp_keys.Count; k++)
                                                                     {
                                                                         var val = temps[j].Values.ElementAt(k);
                                                                         cells = record_sht.Cells[log_value_row + j, k + 1];

                                                                         if (val is DateTime date)
                                                                         {
                                                                             cells.Value                     = date.ToOADate();
                                                                             cells.Style.Numberformat.Format = System.Globalization.DateTimeFormatInfo.CurrentInfo.LongTimePattern;
                                                                         }
                                                                         else if (val is TimeSpan)
                                                                         {
                                                                             cells.Value                     = val;
                                                                             cells.Style.Numberformat.Format = "[h]:mm:ss";
                                                                         }
                                                                         else
                                                                         {
                                                                             cells.Value = val;
                                                                         }

                                                                         CellBorder(cells);
                                                                     }
                                                                 }

                                                                 var record_code = new StringBuilder();
                                                                 record_code.AppendLine("Private Sub Worksheet_SelectionChange(ByVal Target As Range)");
                                                                 record_code.AppendLine("Dim num As Integer");
                                                                 record_code.AppendLine($"num = ActiveCell.Row - {log_name_row}");
                                                                 record_code.AppendLine("If num < 1 Then");
                                                                 record_code.AppendLine("num = 1");
                                                                 record_code.AppendLine("End If");
                                                                 record_code.AppendLine($"Range(\"{(products[0].Count + 1).GetExcelColumnName()}{chart_row}\").Value = num");
                                                                 record_code.AppendLine("End Sub");
                                                                 record_sht.CodeModule.Code = record_code.ToString();

                                                                 //! 高亮顯示所選的行列
                                                                 var exp_address        = new ExcelAddress(record_sht.Cells[log_value_row, 1, temps.Count + log_name_row, temp_keys.Count].Address);
                                                                 var record_condition_h = record_sht.ConditionalFormatting.AddExpression(exp_address);
                                                                 record_condition_h.Formula                          = "ROW()=CELL(\"row\")";
                                                                 record_condition_h.Style.Fill.PatternType           = ExcelFillStyle.Solid;
                                                                 record_condition_h.Style.Fill.BackgroundColor.Color = Color.LemonChiffon;
                                                                 var record_condition_v = record_sht.ConditionalFormatting.AddExpression(exp_address);
                                                                 record_condition_v.Formula                          = "COLUMN()=CELL(\"col\")";
                                                                 record_condition_v.Style.Fill.PatternType           = ExcelFillStyle.Solid;
                                                                 record_condition_v.Style.Fill.BackgroundColor.Color = Color.Honeydew;

                                                                 var record_chart = (ExcelLineChart)record_sht.Drawings.AddChart("", eChartType.Line);
                                                                 record_chart.SetSize(500, 200);

                                                                 var jj = temps.Count + log_name_row;
                                                                 for (var j = 2; j <= 8; j++) //! 插入繪圖溫度資料點
                                                                 {
                                                                     var record_s = record_chart.Series.Add(record_sht.Cells[log_value_row, j, jj, j],  //! 溫度資料 
                                                                                                            record_sht.Cells[log_value_row, 1, jj, 1]); //! 紀錄時間
                                                                     record_s.Header            = temp_keys[j  - 1];
                                                                     record_s.Border.Fill.Color = ColorArray[j - 2];
                                                                 }

                                                                 record_chart.XAxis.Title.Text = "Time";
                                                                 record_chart.XAxis.Title.Font.SetFromFont("Segoe UI", 11, true);
                                                                 record_chart.XAxis.MajorTickMark = eAxisTickMark.In;
                                                                 record_chart.XAxis.MinorTickMark = eAxisTickMark.None;
                                                                 record_chart.XAxis.Font.SetFromFont("Calibri", 10);
                                                                 record_chart.XAxis.TickLabelPosition = eTickLabelPosition.NextTo;
                                                                 record_chart.XAxis.Format            = "HH:mm:ss";
                                                                 record_chart.YAxis.Title.Text        = "Temperature (°C)";
                                                                 record_chart.YAxis.Title.Font.SetFromFont("Segoe UI", 11, true);
                                                                 record_chart.YAxis.Title.Rotation = 270;
                                                                 record_chart.YAxis.MajorTickMark  = eAxisTickMark.In;
                                                                 record_chart.YAxis.MinorTickMark  = eAxisTickMark.None;
                                                                 record_chart.YAxis.Font.SetFromFont("Calibri", 10);
                                                                 record_chart.RoundedCorners    = false;
                                                                 record_chart.Border.Fill.Color = Color.Black;
                                                                 record_chart.SetPosition(chart_row - 1, 0, products[0].Count, 0);

                                                                 record_sht.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                                 record_sht.Cells.Style.Font.SetFromFont("Segoe UI", 11);
                                                                 record_sht.Cells[back_row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                                                 record_sht.Cells.AutoFitColumns();
                                                                 //for (var j = 1; j <= ev_col + logs[0].Count; j++)
                                                                 //{
                                                                 //    record_sht.Column(j).Width += 2;
                                                                 //}

                                                                 //for (var j = 0; j < temp_keys; j++)
                                                                 //{
                                                                 //    record_sht.Cells[3, j + 1].Value = temps[0].Keys.ElementAt(j);
                                                                 //}
                                                             }

                                                             wsht.Cells.AutoFitColumns();
                                                             wsht.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                             wsht.Cells.Style.Font.SetFromFont("Segoe UI", 11);
                                                             for (var i = 1; i <= keys.Count; i++)
                                                             {
                                                                 wsht.Column(i).Width += 2;
                                                             }

                                                             wsht.Cells[1, 1].Formula = $"IF((CELL(\"row\")-{main_name_row})<1,1,CELL(\"row\")-{main_name_row})";

                                                             var code = new StringBuilder();
                                                             code.AppendLine("Private Sub Worksheet_SelectionChange(ByVal Target As Range)");
                                                             code.AppendLine("Dim num As Integer");
                                                             code.AppendLine($"num = ActiveCell.Row - {main_name_row}");
                                                             code.AppendLine("If num < 1 Then");
                                                             code.AppendLine("num = 1");
                                                             code.AppendLine("End If");
                                                             code.AppendLine("Range(\"A1\").Value = num");
                                                             code.AppendLine("End Sub");
                                                             wsht.CodeModule.Code = code.ToString();

                                                             //! 高亮顯示所選的行列
                                                             var exp_address2 = new ExcelAddress(wsht.Cells[main_value_row, 1, n + main_name_row, keys.Count].Address);
                                                             var condition_h  = wsht.ConditionalFormatting.AddExpression(exp_address2);
                                                             condition_h.Formula                          = "ROW()=CELL(\"row\")";
                                                             condition_h.Style.Fill.PatternType           = ExcelFillStyle.Solid;
                                                             condition_h.Style.Fill.BackgroundColor.Color = Color.LemonChiffon;
                                                             var condition_v = wsht.ConditionalFormatting.AddExpression(exp_address2);
                                                             condition_v.Formula                          = "COLUMN()=CELL(\"col\")";
                                                             condition_v.Style.Fill.PatternType           = ExcelFillStyle.Solid;
                                                             condition_v.Style.Fill.BackgroundColor.Color = Color.Honeydew;

                                                             xlwb.Workbook.Names.Add("GPGO", wsht.Cells[1, 1]);

                                                             //! 首頁溫度曲線所需的分頁
                                                             var data_sht = xlwb.Workbook.Worksheets.Add("Data");
                                                             data_sht.Hidden = eWorkSheetHidden.VeryHidden;

                                                             for (var i = 1; i <= max_count; i++)
                                                             {
                                                                 for (var j = 1; j <= temp_keys.Count; j++)
                                                                 {
                                                                     data_sht.Cells[i, j].Formula = $"INDIRECT(\"'\" & \"Records \" & GPGO & \"'\" & \"!${j.GetExcelColumnName()}${i + log_name_row}\")";
                                                                 }

                                                                 data_sht.Cells[i, 1].Style.Numberformat.Format = System.Globalization.DateTimeFormatInfo.CurrentInfo.LongTimePattern;
                                                             }

                                                             var chart = (ExcelLineChart)wsht.Drawings.AddChart("", eChartType.Line);
                                                             chart.SetSize(900, 300);

                                                             for (var j = 2; j <= 8; j++)
                                                             {
                                                                 var record_s = chart.Series.Add(data_sht.Cells[1, j, max_count, j],
                                                                                                 data_sht.Cells[1, 1, max_count, 1]);
                                                                 record_s.Header            = temp_keys[j  - 1];
                                                                 record_s.Border.Fill.Color = ColorArray[j - 2];
                                                             }

                                                             chart.XAxis.Title.Text = "Time";
                                                             chart.XAxis.Title.Font.SetFromFont("Segoe UI", 11, true);
                                                             chart.XAxis.MajorTickMark = eAxisTickMark.In;
                                                             chart.XAxis.MinorTickMark = eAxisTickMark.None;
                                                             chart.XAxis.Font.SetFromFont("Calibri", 10);
                                                             chart.XAxis.TickLabelPosition = eTickLabelPosition.NextTo;
                                                             chart.XAxis.Format            = "HH:mm:ss";
                                                             chart.YAxis.Title.Text        = "Temperature (°C)";
                                                             chart.YAxis.Title.Font.SetFromFont("Segoe UI", 11, true);
                                                             chart.YAxis.Title.Rotation = 270;
                                                             chart.YAxis.MajorTickMark  = eAxisTickMark.In;
                                                             chart.YAxis.MinorTickMark  = eAxisTickMark.None;
                                                             chart.YAxis.Font.SetFromFont("Calibri", 10);
                                                             chart.RoundedCorners    = false;
                                                             chart.Border.Fill.Color = Color.Black;
                                                             chart.SetPosition(0, 0, 0, 0);

                                                             //foreach (var sheet in xlwb.Workbook.Worksheets)
                                                             //{
                                                             //    sheet.Protection.IsProtected              = true;
                                                             //    sheet.Protection.AllowAutoFilter          = true;
                                                             //    sheet.Protection.AllowSelectLockedCells   = true;
                                                             //    sheet.Protection.AllowSelectUnlockedCells = true;
                                                             //    sheet.Protection.AllowSort                = true;
                                                             //    sheet.Protection.SetPassword("23555277");
                                                             //    sheet.Cells[1, 1].Style.Locked = false;
                                                             //}

                                                             xlwb.Workbook.Properties.Author   = "Luo Wunmao";
                                                             xlwb.Workbook.Properties.Company  = "Group Up Industrial Co., Ltd.";
                                                             xlwb.Workbook.Properties.Comments = "Made by the Program of GP";
                                                             xlwb.Workbook.Properties.Created  = created;
                                                             //! 活頁簿保護不受web瀏覽支援(會無法開啟)
                                                             // xlwb.Workbook.Protection.LockRevision = true;
                                                             // xlwb.Workbook.Protection.LockStructure = true;
                                                             // xlwb.Workbook.Protection.SetPassword("23555277");

                                                             try
                                                             {
                                                                 xlwb.SaveAs(fi);
                                                                 result = true;
                                                             }
                                                             catch (Exception ex)
                                                             {
                                                                 Log.Error(ex, "EXCEL儲存失敗");
                                                                 return;
                                                             }

                                                             xlwb.Dispose();
                                                         });
                                        });
        }

        Standby = true;

        return result;
    }

    private async Task<bool> SaveToCSV(string path)
    {
        Standby = false;

        var result = false;

        if (ViewResults?.Count > 0)
        {
            await Task.Factory.StartNew(() =>
                                        {
                                            path = path.Trim().TrimEnd('\\');

                                            if (!Directory.Exists(path))
                                            {
                                                try
                                                {
                                                    Directory.CreateDirectory(path);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Log.Error(ex, "CSV輸出資料夾無法創建");
                                                    return;
                                                }
                                            }

                                            var datas = ViewResults.Where(x => x.IsFinished).ToPooledList();
                                            if (datas.Count <= 0) return;

                                            try
                                            {
                                                var recipe = datas[0].Recipe.ToDictionary(Language);
                                                var type   = typeof(ProcessInfo);

                                                using var titles = new[]
                                                                   {
                                                                       type.GetProperty(nameof(ProcessInfo.AddedTime))?.GetName(Language)  ?? nameof(ProcessInfo.AddedTime),
                                                                       type.GetProperty(nameof(ProcessInfo.StartTime))?.GetName(Language)  ?? nameof(ProcessInfo.StartTime),
                                                                       type.GetProperty(nameof(ProcessInfo.EndTime))?.GetName(Language)    ?? nameof(ProcessInfo.EndTime),
                                                                       type.GetProperty(nameof(ProductInfo.PartID))?.GetName(Language)     ?? nameof(ProductInfo.PartID),
                                                                       type.GetProperty(nameof(ProductInfo.LotID))?.GetName(Language)      ?? nameof(ProductInfo.LotID),
                                                                       type.GetProperty(nameof(ProductInfo.Quantity))?.GetName(Language)   ?? nameof(ProductInfo.Quantity),
                                                                       type.GetProperty(nameof(ProcessInfo.OvenCode))?.GetName(Language)   ?? nameof(ProcessInfo.OvenCode),
                                                                       type.GetProperty(nameof(ProductInfo.Layer))?.GetName(Language)      ?? nameof(ProductInfo.Layer),
                                                                       type.GetProperty(nameof(ProcessInfo.OperatorID))?.GetName(Language) ?? nameof(ProcessInfo.OperatorID),
                                                                       type.GetProperty(nameof(ProcessInfo.IsFinished))?.GetName(Language) ?? nameof(ProcessInfo.IsFinished)
                                                                   }.Concat(recipe.Keys)
                                                                    .ToPooledList();

                                                var sb = new StringBuilder();
                                                sb.AppendLine(string.Join(",", titles));

                                                foreach (var info in datas)
                                                {
                                                    recipe = info.Recipe.ToDictionary(Language);

                                                    foreach (var product in info.Products)
                                                    {
                                                        var vals = new[]
                                                                   {
                                                                       info.AddedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                       info.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                       info.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                       product.PartID,
                                                                       product.LotID,
                                                                       product.Quantity.ToString(),
                                                                       info.OvenCode,
                                                                       product.Layer.ToString(),
                                                                       info.OperatorID,
                                                                       info.IsFinished.ToString()
                                                                   }.Concat(recipe.Values)
                                                                    .ToPooledList();

                                                        sb.AppendLine(string.Join(",", vals));
                                                    }
                                                }

                                                using var outputFile = new StreamWriter($"{path}\\{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.csv", false, Encoding.UTF8);
                                                outputFile.Write(sb.ToString());

                                                result = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.Error(ex, "CSV儲存失敗");
                                            }
                                        });
        }

        Standby = true;

        return result;
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

            linearAxis.TitleColor                  = fontcolor;
            linearAxis.AxislineColor               = bordercolor;
            linearAxis.MajorGridlineColor          = bordercolor;
            linearAxis.MinorGridlineColor          = bordercolor;
            linearAxis.TicklineColor               = bordercolor;
            linearAxis.ExtraGridlineColor          = bordercolor;
            linearAxis.TextColor                   = fontcolor;
            categoryAxis1.TitleColor               = fontcolor;
            categoryAxis1.MajorGridlineColor       = bordercolor;
            categoryAxis1.MinorGridlineColor       = bordercolor;
            categoryAxis1.TicklineColor            = bordercolor;
            categoryAxis1.ExtraGridlineColor       = bordercolor;
            categoryAxis1.TextColor                = fontcolor;
            categoryAxis1.AxislineColor            = bordercolor;
            ResultView.PlotAreaBackground          = chartbg;
            ResultView.PlotAreaBorderColor         = bordercolor;
            ResultView.Legends[0].LegendTitleColor = fontcolor;
            ResultView.Legends[0].LegendTextColor  = fontcolor;
            ResultView.Legends[0].LegendBorder     = bordercolor;
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

        ResultView.InvalidatePlot(true);

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
                        var val = info.Where(x =>
                                             {
                                                 switch (Mode)
                                                 {
                                                     case ChartMode.ByPLC:
                                                         return x.StationNumber.ToString() == categories[j];
                                                     case ChartMode.ByPart:
                                                         return x.Product.PartID == categories[j];
                                                     case ChartMode.ByLot:
                                                         return x.Product.LotID == categories[j];
                                                     case ChartMode.ByLayer:
                                                         return x.Product.Layer.ToString() == categories[j];
                                                 }

                                                 if (ByDate)
                                                 {
                                                     return x.AddedTime.Date.ToString("MM/dd") == categories[j];
                                                 }

                                                 return $"{x.AddedTime.Hour:00}:00" == categories[j];
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

        ResultView.InvalidatePlot(true);
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
                                 .Where(x => OvenFilter.Check(x.StationNumber)                 &&
                                             RecipeFilter.Check(x.Recipe.RecipeName)           &&
                                             OpFilter.Check(x.OperatorID)                      &&
                                             FinishedFilter.Check(x.IsFinished)                &&
                                             x.Products.Any(y => PartIDFilter.Check(y.PartID)) &&
                                             x.Products.Any(y => LayerFilter.Check(y.Layer)))
                                 .OrderByDescending(x => x.AddedTime)
                                 .ToList();
        }

        NotifyPropertyChanged(nameof(ProduceTotalCount));
    }

    public TraceabilityView_ViewModel(IDataBase<ProcessInfo> db, IDialogService dialog) : base(db)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        ChartModel                  = new ProcessChartModel();

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

                                           if (!result1 && !result2 || string.IsNullOrEmpty(input1.ToString()) && string.IsNullOrEmpty(input2.ToString()))
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
                                              var (result, choose) = await dialog.ChooseOne(new Dictionary<Language, string>
                                                                                            {
                                                                                                { Language.TW, "選擇輸出類型" },
                                                                                                { Language.CHS, "选择输出类型" },
                                                                                                { Language.EN, "Select output type" }
                                                                                            },
                                                                                            new[]
                                                                                            {
                                                                                                new Dictionary<Language, string>
                                                                                                {
                                                                                                    { Language.TW, "EXCEL" },
                                                                                                    { Language.CHS, "EXCEL" },
                                                                                                    { Language.EN, "EXCEL" }
                                                                                                },
                                                                                                new Dictionary<Language, string>
                                                                                                {
                                                                                                    { Language.TW, "CSV" },
                                                                                                    { Language.CHS, "CSV" },
                                                                                                    { Language.EN, "CSV" }
                                                                                                }
                                                                                            },
                                                                                            new Dictionary<Language, string>
                                                                                            {
                                                                                                { Language.TW, "輸出類型" },
                                                                                                { Language.CHS, "输出类型" },
                                                                                                { Language.EN, "Select output type" }
                                                                                            },
                                                                                            true);

                                              if (result && ((int)choose == 0 ? await SaveToExcel(path) : await SaveToCSV(path)))
                                              {
                                                  dialog.Show(new Dictionary<Language, string>
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
                         IsPanEnabled       = false,
                         IsZoomEnabled      = false,
                         FontSize           = 12,
                         TitleColor         = fontcolor,
                         TickStyle          = TickStyle.Inside,
                         MajorGridlineStyle = LineStyle.Dot,
                         //MajorStep = 100,
                         MinorGridlineStyle = LineStyle.None,
                         MajorTickSize      = 0,
                         MinorTickSize      = 0,
                         //MinorStep = 10,
                         AxislineStyle      = LineStyle.Solid,
                         AxislineColor      = bordercolor,
                         MajorGridlineColor = bordercolor,
                         MinorGridlineColor = bordercolor,
                         TicklineColor      = bordercolor,
                         ExtraGridlineColor = bordercolor,
                         ExtraGridlineStyle = LineStyle.None,
                         TextColor          = fontcolor,
                         Minimum            = 0,
                         MaximumPadding     = 0.15,
                         Position           = AxisPosition.Left,
                         Key                = "0"
                         //Maximum = 1000
                     };

        categoryAxis1 = new CategoryAxis
                        {
                            IsPanEnabled       = false,
                            IsZoomEnabled      = false,
                            FontSize           = 12,
                            TitleColor         = fontcolor,
                            MajorGridlineColor = bordercolor,
                            MinorGridlineColor = bordercolor,
                            TicklineColor      = bordercolor,
                            ExtraGridlineColor = bordercolor,
                            TextColor          = fontcolor,
                            TickStyle          = TickStyle.Inside,
                            MajorTickSize      = 0,
                            MinorTickSize      = 0,
                            AxislineStyle      = LineStyle.Solid,
                            ExtraGridlineStyle = LineStyle.None,
                            AxislineColor      = bordercolor,
                            GapWidth           = 1,
                            MinorStep          = 1,
                            MajorStep          = 1,
                            Position           = AxisPosition.Bottom,
                            Key                = "1"
                        };

        categoryAxis2 = new CategoryAxis
                        {
                            IsPanEnabled  = false,
                            IsZoomEnabled = false,
                            IsAxisVisible = false,
                            Position      = AxisPosition.Bottom,
                            Key           = "2"
                        };

        ResultView = new PlotModel
                     {
                         DefaultFont             = "Microsoft JhengHei",
                         PlotAreaBackground      = chartbg,
                         PlotAreaBorderColor     = bordercolor,
                         PlotAreaBorderThickness = new OxyThickness(0,  1, 1, 1),
                         PlotMargins             = new OxyThickness(35, 0, 0, 20),
                         TextColor               = fontcolor
                     };

        ResultView.Legends.Add(new Legend
                               {
                                   LegendTitle         = ProductInfoProperties[nameof(ProductInfo.PartID)].GetName(Language),
                                   LegendTitleColor    = fontcolor,
                                   LegendTextColor     = fontcolor,
                                   LegendBorder        = bordercolor,
                                   LegendBackground    = bgcolor,
                                   LegendPlacement     = LegendPlacement.Outside,
                                   LegendPosition      = LegendPosition.RightTop,
                                   LegendOrientation   = LegendOrientation.Vertical,
                                   LegendFontSize      = 12,
                                   LegendTitleFontSize = 12,
                                   //LegendItemOrder = LegendItemOrder.Reverse,
                                   LegendMargin  = 4,
                                   LegendPadding = 5,
                               });

        ResultView.Axes.Add(linearAxis);
        ResultView.Axes.Add(categoryAxis2);
        ResultView.Axes.Add(categoryAxis1);

        void UpdateAct()
        {
            UpdateViewResult();
            UpdateChart(Date1, Date2);
        }

        SelectedIndex  = -1;
        OvenFilter     = new FilterGroup(UpdateAct);
        RecipeFilter   = new FilterGroup(UpdateAct);
        PartIDFilter   = new FilterGroup(UpdateAct);
        OpFilter       = new FilterGroup(UpdateAct);
        LayerFilter    = new FilterGroup(UpdateAct);
        FinishedFilter = new FilterGroup(UpdateAct);

        ResultsChanged += async e =>
                          {
                              OvenFilter.Filter     = e?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()                      ?? new List<EqualFilter>();
                              RecipeFilter.Filter   = e?.Select(x => x.Recipe.RecipeName).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()                  ?? new List<EqualFilter>();
                              OpFilter.Filter       = e?.Select(x => x.OperatorID).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()                         ?? new List<EqualFilter>();
                              PartIDFilter.Filter   = e?.SelectMany(x => x.Products.Select(y => y.PartID)).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList() ?? new List<EqualFilter>();
                              LayerFilter.Filter    = e?.SelectMany(x => x.Products.Select(y => y.Layer)).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()  ?? new List<EqualFilter>();
                              FinishedFilter.Filter = e?.Select(x => x.IsFinished).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()                         ?? new List<EqualFilter>();

                              UpdateAct();

                              await Task.Delay(150);

                              if (SearchResult != null)
                              {
                                  SelectedIndex = ViewResults?.Count > 0 ? ViewResults.FindIndex(x => x.StationNumber == SearchResult.StationNumber && x.AddedTime == SearchResult.AddedTime) : -1;
                                  SearchResult  = null;

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
                                                                     x.value.LotID  == SearchProduct.LotID  &&
                                                                     x.value.Layer  == SearchProduct.Layer)
                                                .index;

                                  SearchProduct = null;
                              }
                              else
                              {
                                  EventIndex = -1;
                              }
                          };

        BeginIndexChanged += _ =>
                             {
                                 UpdateViewResult();
                                 UpdateChart(Date1, Date2);
                             };

        EndIndexChanged += _ =>
                           {
                               UpdateViewResult();
                               UpdateChart(Date1, Date2);
                           };
    }
}