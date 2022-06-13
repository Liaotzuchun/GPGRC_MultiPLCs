﻿using GPGO_MultiPLCs.Models;
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

namespace GPGO_MultiPLCs.ViewModels;

/// <summary>生產紀錄追蹤</summary>
public class TraceabilityView_ViewModel : DataCollectionByDate<ProcessInfo>
{
    public enum ChartMode
    {
        ByDateTime = 0,
        ByPLC      = 1,
        ByLot      = 2,
        ByPart     = 3,
        Pie        = 4
    }

    private Color[] ColorArray =
    {
        Color.Red,
        Color.DarkOrange,
        Color.Gold,
        Color.Lime,
        Color.DodgerBlue,
        Color.DarkOrchid,
        Color.Magenta
    };

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

    /// <summary>基於工單的Filter</summary>
    public FilterGroup OrderFilter { get; }

    /// <summary>基於料號的Filter</summary>
    public FilterGroup PartIDFilter { get; }

    /// <summary>基於批號的Filter</summary>
    public FilterGroup LotIDFilter { get; }

    /// <summary>基於PLC站號的Filter</summary>
    public FilterGroup OvenFilter { get; }

    /// <summary>基於配方的Filter</summary>
    public FilterGroup RecipeFilter { get; }

    /// <summary>基於正反面的Filter</summary>
    public FilterGroup SideFilter { get; }

    /// <summary>基於板架的Filter</summary>
    public FilterGroup RackFilter { get; }

    public FilterGroup LayerFilter { get; }

    /// <summary>總量統計</summary>
    public int ProduceTotalCount => ViewResults?.Count > 0 ? ViewResults.Where(x => x.IsFinished).Sum(x => x.Quantity) : 0;

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
                ChartModel.SetData(value.RecordTemperatures.ToList());
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
    public int Mode
    {
        get => Get<int>();
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

    /// <summary>將目前顯示資料輸出至Excel OpenXML格式檔案</summary>
    /// <param name="path">資料夾路徑</param>
    private async Task<bool> SaveToExcel(string path)
    {
        Standby = false;

        var result = true;

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
                                                    result = false;

                                                    return;
                                                }
                                            }

                                            var created = DateTime.Now;
                                            var x       = ViewResults.Count / 500;     //!檔案數
                                            var y       = ViewResults.Count - 500 * x; //!剩餘數

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
                                                             var fi    = new FileInfo($"{path}\\{created:yyyy-MM-dd-HH-mm-ss-fff(}{index + 1}).xlsm");
                                                             var datas = ViewResults.GetRange(500 * index, index == x                    - 1 ? y : 500);
                                                             var n     = datas.Count;
                                                             var xlwb  = new ExcelPackage();
                                                             xlwb.Workbook.CreateVBAProject();
                                                             var wsht = xlwb.Workbook.Worksheets.Add($"{n}{(n <= 1 ? " result" : " results")}");
                                                             wsht.View.ShowGridLines = false;
                                                             wsht.View.FreezePanes(4, 1);
                                                             wsht.Row(1).Height = 225;

                                                             var        keys      = datas[0].ToDic(Language).Keys.ToArray();
                                                             var        temp_keys = Array.Empty<string>();
                                                             var        max_count = 0;
                                                             ExcelRange cells;

                                                             for (var i = 0; i < keys.Length; i++)
                                                             {
                                                                 cells                        = wsht.Cells[3, i + 1];
                                                                 cells.Value                  = keys[i];
                                                                 cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                 cells.Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);
                                                             }

                                                             wsht.Cells[3, 1, 3, keys.Length - 1].AutoFilter = true;

                                                             for (var i = 0; i < n; i++)
                                                             {
                                                                 var values   = datas[i].ToDic(Language).Values.ToArray();
                                                                 var temps    = datas[i].RecordTemperatures.Select(o => o.ToDictionary(Language)).ToArray();
                                                                 var products = datas[i].Products.Select(o => o.ToDictionary(Language)).ToArray();

                                                                 if (i == 0)
                                                                 {
                                                                     temp_keys = temps[0].Keys.ToArray();
                                                                 }

                                                                 if (temps.Length > max_count)
                                                                 {
                                                                     max_count = temps.Length;
                                                                 }

                                                                 for (var j = 0; j < values.Length; j++)
                                                                 {
                                                                     cells = wsht.Cells[i + 4, j + 1];
                                                                     if (values[j] is DateTime date)
                                                                     {
                                                                         cells.Value                     = date.ToOADate();
                                                                         cells.Style.Numberformat.Format = System.Globalization.DateTimeFormatInfo.CurrentInfo.LongTimePattern;
                                                                     }
                                                                     else if (values[j] is string str)
                                                                     {
                                                                         cells.Value                     = str;
                                                                         cells.Style.Numberformat.Format = "@";
                                                                     }
                                                                     else
                                                                     {
                                                                         cells.Value = values[j];
                                                                     }
                                                                 }

                                                                 var sheet_name = $"Records {i + 1}";
                                                                 cells         = wsht.Cells[i + 4, values.Length];
                                                                 cells.Formula = $"HYPERLINK(\"#'{sheet_name}'!$A$4\",\"@\")";
                                                                 cells.Style.Font.Color.SetColor(Color.Blue);
                                                                 cells.Style.Font.UnderLine      = false;
                                                                 cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                                                 var record_sht = xlwb.Workbook.Worksheets.Add(sheet_name);
                                                                 record_sht.View.ShowGridLines = false;
                                                                 record_sht.View.FreezePanes(4, 1);
                                                                 record_sht.Row(1).Height = 225;

                                                                 cells         = record_sht.Cells[2, 1];
                                                                 cells.Formula = $"HYPERLINK(\"#'{wsht.Name}'!$A${i + 4}\",\"<<<<<<Back\")";
                                                                 cells.Style.Font.Color.SetColor(Color.Blue);
                                                                 cells.Style.Font.UnderLine = false;

                                                                 for (var j = 0; j < temps[0].Count; j++)
                                                                 {
                                                                     record_sht.Cells[3, j + 1].Value = temps[0].Keys.ElementAt(j);
                                                                 }

                                                                 cells                        = record_sht.Cells[3, 1, 3, temps[0].Count];
                                                                 cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                 cells.Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);

                                                                 for (var j = 0; j < temps.Length; j++)
                                                                 {
                                                                     for (var k = 0; k < temps[j].Count; k++)
                                                                     {
                                                                         var val = temps[j].Values.ElementAt(k);
                                                                         cells = record_sht.Cells[4 + j, k + 1];

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
                                                                     }
                                                                 }

                                                                 var jj = temps.Length + 3;
                                                                 cells                           = record_sht.Cells[3, 1, jj, temps[0].Count];
                                                                 cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                                 cells.Style.Border.Left.Style   = ExcelBorderStyle.Thin;
                                                                 cells.Style.Border.Right.Style  = ExcelBorderStyle.Thin;
                                                                 cells.Style.Border.Top.Style    = ExcelBorderStyle.Thin;
                                                                 cells.AutoFitColumns();

                                                                 var record_code = new StringBuilder();
                                                                 record_code.AppendLine("Private Sub Worksheet_SelectionChange(ByVal Target As Range)");
                                                                 record_code.AppendLine("Dim num As Integer");
                                                                 record_code.AppendLine("num = ActiveCell.Row - 3");
                                                                 record_code.AppendLine("If num < 1 Then");
                                                                 record_code.AppendLine("num = 1");
                                                                 record_code.AppendLine("End If");
                                                                 record_code.AppendLine("Range(\"A1\").Value = num");
                                                                 record_code.AppendLine("End Sub");
                                                                 record_sht.CodeModule.Code = record_code.ToString();

                                                                 var record_condition_h =
                                                                     record_sht.ConditionalFormatting.AddExpression(new ExcelAddress(record_sht.Cells[4, 1, temps.Length + 3, 10].Address));
                                                                 record_condition_h.Formula                          = "ROW()=CELL(\"row\")";
                                                                 record_condition_h.Style.Fill.PatternType           = ExcelFillStyle.Solid;
                                                                 record_condition_h.Style.Fill.BackgroundColor.Color = Color.LemonChiffon;
                                                                 var record_condition_v =
                                                                     record_sht.ConditionalFormatting.AddExpression(new ExcelAddress(record_sht.Cells[4, 1, temps.Length + 3, 10].Address));
                                                                 record_condition_v.Formula                          = "COLUMN()=CELL(\"col\")";
                                                                 record_condition_v.Style.Fill.PatternType           = ExcelFillStyle.Solid;
                                                                 record_condition_v.Style.Fill.BackgroundColor.Color = Color.Honeydew;

                                                                 var record_chart = (ExcelLineChart)record_sht.Drawings.AddChart("", eChartType.Line);
                                                                 record_chart.SetSize(970, 300);

                                                                 for (var j = 2; j <= temps[0].Count; j++)
                                                                 {
                                                                     var record_s = record_chart.Series.Add(record_sht.Cells[4, j, jj, j], record_sht.Cells[4, 1, jj, 1]);
                                                                     record_s.Header            = temps[0].Keys.ElementAt(j - 1);
                                                                     record_s.Border.Fill.Color = ColorArray[j              - 2];
                                                                 }

                                                                 record_chart.XAxis.Title.Text = "Time";
                                                                 record_chart.XAxis.Title.Font.SetFromFont("Segoe UI", 11, true);
                                                                 record_chart.XAxis.MajorTickMark = eAxisTickMark.In;
                                                                 record_chart.XAxis.MinorTickMark = eAxisTickMark.None;
                                                                 record_chart.XAxis.Font.SetFromFont("Calibri", 10);
                                                                 record_chart.XAxis.TickLabelPosition = eTickLabelPosition.NextTo;
                                                                 record_chart.XAxis.Format            = "MM/dd HH:mm:ss";
                                                                 record_chart.YAxis.Title.Text        = "Temperature (°C)";
                                                                 record_chart.YAxis.Title.Font.SetFromFont("Segoe UI", 11, true);
                                                                 record_chart.YAxis.Title.Rotation = 270;
                                                                 record_chart.YAxis.MajorTickMark  = eAxisTickMark.In;
                                                                 record_chart.YAxis.MinorTickMark  = eAxisTickMark.None;
                                                                 record_chart.YAxis.Font.SetFromFont("Calibri", 10);
                                                                 record_chart.RoundedCorners    = false;
                                                                 record_chart.Border.Fill.Color = Color.Black;
                                                                 record_chart.SetPosition(0, 0, 0, 0);

                                                                 record_sht.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                                 record_sht.Cells.Style.Font.SetFromFont("Segoe UI", 11);
                                                                 record_sht.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                                                 for (var j = 1; j <= 8; j++)
                                                                 {
                                                                     record_sht.Column(j).Width += 2;
                                                                 }
                                                             }

                                                             cells                           = wsht.Cells[3, 1, n + 3, keys.Length];
                                                             cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                             cells.Style.Border.Left.Style   = ExcelBorderStyle.Thin;
                                                             cells.Style.Border.Right.Style  = ExcelBorderStyle.Thin;
                                                             cells.Style.Border.Top.Style    = ExcelBorderStyle.Thin;
                                                             cells.AutoFitColumns();

                                                             wsht.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                             wsht.Cells.Style.Font.SetFromFont("Segoe UI", 11);
                                                             for (var i = 1; i <= keys.Length; i++)
                                                             {
                                                                 wsht.Column(i).Width += 2;
                                                             }

                                                             wsht.Cells[1, 1].Formula = "IF((CELL(\"row\")-3)<1,1,CELL(\"row\")-3)";

                                                             var code = new StringBuilder();
                                                             code.AppendLine("Private Sub Worksheet_SelectionChange(ByVal Target As Range)");
                                                             code.AppendLine("Dim num As Integer");
                                                             code.AppendLine("num = ActiveCell.Row - 3");
                                                             code.AppendLine("If num < 1 Then");
                                                             code.AppendLine("num = 1");
                                                             code.AppendLine("End If");
                                                             code.AppendLine("Range(\"A1\").Value = num");
                                                             code.AppendLine("End Sub");
                                                             wsht.CodeModule.Code = code.ToString();

                                                             var condition_h = wsht.ConditionalFormatting.AddExpression(new ExcelAddress(wsht.Cells[4, 1, n + 3, keys.Length].Address));
                                                             condition_h.Formula                          = "ROW()=CELL(\"row\")";
                                                             condition_h.Style.Fill.PatternType           = ExcelFillStyle.Solid;
                                                             condition_h.Style.Fill.BackgroundColor.Color = Color.LemonChiffon;
                                                             var condition_v = wsht.ConditionalFormatting.AddExpression(new ExcelAddress(wsht.Cells[4, 1, n + 3, keys.Length].Address));
                                                             condition_v.Formula                          = "COLUMN()=CELL(\"col\")";
                                                             condition_v.Style.Fill.PatternType           = ExcelFillStyle.Solid;
                                                             condition_v.Style.Fill.BackgroundColor.Color = Color.Honeydew;

                                                             xlwb.Workbook.Names.Add("ooxx", wsht.Cells[1, 1]);

                                                             //!首頁溫度曲線所需的分頁
                                                             var data_sht = xlwb.Workbook.Worksheets.Add("Data");
                                                             data_sht.Hidden = eWorkSheetHidden.VeryHidden;

                                                             for (var i = 1; i <= max_count; i++)
                                                             {
                                                                 data_sht.Cells[i, 1].Formula                   = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$A${i + 3}\")";
                                                                 data_sht.Cells[i, 1].Style.Numberformat.Format = System.Globalization.DateTimeFormatInfo.CurrentInfo.LongTimePattern;
                                                                 data_sht.Cells[i, 2].Formula                   = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$B${i + 3}\")";
                                                                 data_sht.Cells[i, 3].Formula                   = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$C${i + 3}\")";
                                                                 data_sht.Cells[i, 4].Formula                   = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$D${i + 3}\")";
                                                                 data_sht.Cells[i, 5].Formula                   = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$E${i + 3}\")";
                                                                 data_sht.Cells[i, 6].Formula                   = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$F${i + 3}\")";
                                                                 data_sht.Cells[i, 7].Formula                   = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$G${i + 3}\")";
                                                                 data_sht.Cells[i, 8].Formula                   = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$H${i + 3}\")";
                                                                 data_sht.Cells[i, 9].Formula                   = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$I${i + 3}\")";
                                                                 data_sht.Cells[i, 10].Formula                  = $"INDIRECT(\"'\" & \"Records \" & ooxx & \"'\" & \"!$J${i + 3}\")";
                                                             }

                                                             var chart = (ExcelLineChart)wsht.Drawings.AddChart("", eChartType.Line);
                                                             chart.SetSize(970, 300);

                                                             for (var j = 2; j <= temp_keys.Length; j++)
                                                             {
                                                                 var record_s = chart.Series.Add(data_sht.Cells[1, j, max_count, j], data_sht.Cells[1, 1, max_count, 1]);
                                                                 record_s.Header            = temp_keys[j  - 1];
                                                                 record_s.Border.Fill.Color = ColorArray[j - 2];
                                                             }

                                                             chart.XAxis.Title.Text = "Time";
                                                             chart.XAxis.Title.Font.SetFromFont("Segoe UI", 11, true);
                                                             chart.XAxis.MajorTickMark = eAxisTickMark.In;
                                                             chart.XAxis.MinorTickMark = eAxisTickMark.None;
                                                             chart.XAxis.Font.SetFromFont("Calibri", 10);
                                                             chart.XAxis.TickLabelPosition = eTickLabelPosition.NextTo;
                                                             chart.XAxis.Format            = "MM/dd HH:mm:ss";
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
                                                             //!活頁簿保護不受web瀏覽支援(會無法開啟)
                                                             // xlwb.Workbook.Protection.LockRevision = true;
                                                             // xlwb.Workbook.Protection.LockStructure = true;
                                                             // xlwb.Workbook.Protection.SetPassword("23555277");

                                                             try
                                                             {
                                                                 xlwb.SaveAs(fi);
                                                             }
                                                             catch (Exception ex)
                                                             {
                                                                 Log.Error(ex, "EXCEL儲存失敗");
                                                                 result = false;

                                                                 return;
                                                             }

                                                             xlwb.Dispose();
                                                         });
                                        });
        }
        else
        {
            result = false;
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
            var finishedResults = ViewResults.Where(x => x.IsFinished).SelectMany(x => x.GetFlatInfos()).ToList();

            var ByDate = date2 - date1 > TimeSpan.FromDays(1);
            var result2 = finishedResults
                         .GroupBy(x => Mode >= (int)ChartMode.ByLot ?
                                           x.StationNumber.ToString("00") :
                                           x.Product.PartID)
                         .OrderBy(x => x.Key)
                         .Select(x => (x.Key, x))
                         .ToArray();

            var NoLayer2   = result2.Length > 20 && Mode >= (int)ChartMode.ByLot;
            var categories = new List<string>();

            var result1 = finishedResults.GroupBy(x =>
                                                  {
                                                      return (ChartMode)Mode switch
                                                             {
                                                                 ChartMode.ByPLC  => x.StationNumber.ToString("00"),
                                                                 ChartMode.ByLot  => x.Product.LotID,
                                                                 ChartMode.ByPart => x.Product.PartID,
                                                                 _                => ByDate ? x.AddedTime.Date.ToString("MM/dd") : $"{x.AddedTime.Hour:00}:00"
                                                             };
                                                  })
                                         .OrderBy(x => x.Key)
                                         .Select(x => (x.Key, x.Sum(y => y.Product.Quantity)))
                                         .ToArray();

            categoryAxis1.FontSize = result1.Length > 20 ? 9 : 12;

            var color_step_1 = 0.9 / result1.Length;

            for (var i = 0; i < result1.Length; i++)
            {
                var (result, count) = result1[i];
                categories.Add(result);
                categoryAxis1.Labels.Add(result);
                categoryAxis2.Labels.Add(result);

                var cs = new BarSeries
                         {
                             FontSize          = result1.Length > 20 ? 8 : 10,
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
                ResultView.Legends[0].LegendTitle = Mode >= (int)ChartMode.ByLot ?
                                                        ProcessInfoProperties[nameof(ProcessInfo.StationNumber)].GetName(Language) :
                                                        ProductInfoProperties[nameof(ProductInfo.PartID)].GetName(Language);

                var color_step_2 = 0.9 / result2.Length;

                for (var i = 0; i < result2.Length; i++)
                {
                    var (cat, info) = result2[i];

                    var ccs = new BarSeries
                              {
                                  FontSize          = 10,
                                  LabelFormatString = result2.Length > 10 || categories.Count > 20 ? "" : "{0}",
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
                                                 switch ((ChartMode)Mode)
                                                 {
                                                     case ChartMode.ByPLC:
                                                         return x.StationNumber.ToString("00") == categories[j];
                                                     case ChartMode.ByLot:
                                                         return x.Product.LotID == categories[j];
                                                     case ChartMode.ByPart:
                                                         return x.Product.PartID == categories[j];
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
                                 .Where(x => OvenFilter.Check(x.StationNumber)       &&
                                             RecipeFilter.Check(x.Recipe.RecipeName) &&
                                             //OrderFilter.Check(x.OrderCode)    &&
                                             OpFilter.Check(x.OperatorID)                      &&
                                             x.Products.Any(y => PartIDFilter.Check(y.PartID)) &&
                                             x.Products.Any(y => LotIDFilter.Check(y.LotID))   &&
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
                                              if (await SaveToExcel(path))
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

        SelectedIndex = -1;
        OvenFilter    = new FilterGroup(UpdateAct);
        RecipeFilter  = new FilterGroup(UpdateAct);
        OrderFilter   = new FilterGroup(UpdateAct);
        PartIDFilter  = new FilterGroup(UpdateAct);
        LotIDFilter   = new FilterGroup(UpdateAct);
        OpFilter      = new FilterGroup(UpdateAct);
        RackFilter    = new FilterGroup(UpdateAct);
        SideFilter    = new FilterGroup(UpdateAct);
        LayerFilter   = new FilterGroup(UpdateAct);

        ResultsChanged += async e =>
                          {
                              OvenFilter.Filter   = e?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()                      ?? new List<EqualFilter>();
                              RecipeFilter.Filter = e?.Select(x => x.Recipe.RecipeName).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()                  ?? new List<EqualFilter>();
                              OpFilter.Filter     = e?.Select(x => x.OperatorID).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()                         ?? new List<EqualFilter>();
                              PartIDFilter.Filter = e?.SelectMany(x => x.Products.Select(y => y.PartID)).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList() ?? new List<EqualFilter>();
                              LotIDFilter.Filter  = e?.SelectMany(x => x.Products.Select(y => y.LotID)).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()  ?? new List<EqualFilter>();
                              LayerFilter.Filter  = e?.SelectMany(x => x.Products.Select(y => y.Layer)).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList()  ?? new List<EqualFilter>();

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
                                  var matchevents = ViewResults[SelectedIndex]
                                                   .EventList
                                                   .Select((x, i) => (index: i, value: x))
                                                   .Where(x => x.value.Value.Equals(SearchEvent.Value) && x.value.Description == SearchEvent.Description)
                                                   .ToList();

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