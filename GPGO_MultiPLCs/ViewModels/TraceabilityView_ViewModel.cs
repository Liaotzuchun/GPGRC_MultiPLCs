using GPGO_MultiPLCs.Models;
using GPMVVM.Helpers;
using GPMVVM.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OxyPlot;
using OxyPlot.Axes;
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
using FontStyle = System.Drawing.FontStyle;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>生產紀錄追蹤</summary>
    public class TraceabilityView_ViewModel : DataCollectionByDate<ProcessInfo>
    {
        public enum ChartMode
        {
            ByDateTime = 0,
            ByPLC      = 1,

            //ByOrder = 2,
            ByLot  = 2,
            ByPart = 3,
            Pie    = 4
        }

        private readonly Dictionary<string, PropertyInfo> ProcessInfoProperties = typeof(ProcessInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(x => x.Name, x => x);
        private          FrameworkElement                 elementView;
        private          OxyColor                         chartbg     = OxyColor.FromRgb(231, 246, 226);
        private          OxyColor                         bgcolor     = OxyColor.FromRgb(215, 230, 207);
        private          OxyColor                         bordercolor = OxyColor.FromRgb(174, 187, 168);
        private readonly CategoryAxis                     categoryAxis1;
        private readonly CategoryAxis                     categoryAxis2;
        private          OxyColor                         fontcolor = OxyColor.FromRgb(50, 70, 60);

        public Language    Language = Language.TW;
        public LogEvent    SearchEvent;
        public ProcessInfo SearchResult;

        /// <summary>依據工單或料號搜尋</summary>
        public RelayCommand FindCommand { get; }

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

        /// <summary>總量統計</summary>
        public int ProduceTotalCount => ViewResults?.Count > 0 ? ViewResults.Where(x => x.IsFinished).Sum(x => x.Quantity) : 0;

        /// <summary>基於配方的Filter</summary>
        public FilterGroup RecipeFilter { get; }

        public PlotModel ResultView { get; }

        /// <summary>基於正反面的Filter</summary>
        public FilterGroup SideFilter { get; }

        /// <summary>輸出Excel報表</summary>
        public RelayCommand ToExcelCommand { get; }

        public RelayCommand LoadedCommand { get; }

        /// <summary>基於板架的Filter</summary>
        public FilterGroup RackFilter { get; }

        public int EventIndex
        {
            get => Get<int>();
            set => Set(value);
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

        /// <summary>顯示的資料列表</summary>
        public List<ProcessInfo> ViewResults
        {
            get => Get<List<ProcessInfo>>();
            set => Set(value);
        }

        /// <summary>本日產量更新事件</summary>
        public event Action<List<(int StationIndex, int Production)>> TodayProductionUpdated;

        /// <summary>新增至資料庫</summary>
        /// <param name="index">PLC序號，由0開始(寫入時會自動+1)</param>
        /// <param name="info">紀錄資訊</param>
        /// <param name="dateTime">紀錄時間，預設為當下時間，帶入default(DateTime)同樣為當下時間</param>
        /// <param name="UpdateResult">決定是否更新Ram Data</param>
        public async void AddToDB(int index, ProcessInfo info, DateTime dateTime = default, bool UpdateResult = false)
        {
            info.StationNumber = index + 1;
            info.AddedTime     = dateTime == default ? DateTime.Now : dateTime;

            try
            {
                await DataCollection.AddAsync(info);

                if (UpdateResult)
                {
                    Results = await DataCollection.FindAsync(x => x.AddedTime >= Date1 && x.AddedTime < Date2.AddDays(1));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "生產紀錄寫入資料庫失敗");
            }
        }

        /// <summary>新增至資料庫</summary>
        /// <param name="index">PLC序號，由0開始(寫入時會自動+1)</param>
        /// <param name="infos">紀錄資訊</param>
        /// <param name="dateTime">紀錄時間，預設為當下時間，帶入default(DateTime)同樣為當下時間</param>
        /// <param name="UpdateResult">決定是否更新Ram Data</param>
        public async void AddToDB(int index, ICollection<ProcessInfo> infos, DateTime dateTime = default, bool UpdateResult = false)
        {
            var n = 0;

            foreach (var info in infos)
            {
                info.StationNumber = index + 1;
                info.AddedTime     = dateTime == default ? DateTime.Now.AddMilliseconds(n) : dateTime.AddMilliseconds(n);
                n++;

                try
                {
                    await DataCollection.AddAsync(info);

                    if (UpdateResult)
                    {
                        Results = await DataCollection.FindAsync(x => x.AddedTime >= Date1 && x.AddedTime < Date2.AddDays(1));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "生產紀錄寫入資料庫失敗");
                }
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
                                                                 var datas = ViewResults.GetRange(500 * index, index == x - 1 ? y : 500);
                                                                 var n     = datas.Count;
                                                                 var xlwb  = new ExcelPackage();
                                                                 xlwb.Workbook.CreateVBAProject();
                                                                 var wsht = xlwb.Workbook.Worksheets.Add($"{n}{(n <= 1 ? " result" : " results")}");
                                                                 wsht.View.ShowGridLines = false;
                                                                 wsht.View.FreezePanes(4, 1);
                                                                 wsht.Row(1).Height = 225;

                                                                 var keys      = datas[0].ToDic(Language).Keys.ToArray();
                                                                 var max_count = 0;

                                                                 for (var i = 0; i < keys.Length; i++)
                                                                 {
                                                                     wsht.Cells[3, i + 1].Value                  = keys[i];
                                                                     wsht.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                     wsht.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);
                                                                 }

                                                                 wsht.Cells[3, 1, 3, keys.Length - 1].AutoFilter = true;

                                                                 for (var i = 0; i < n; i++)
                                                                 {
                                                                     var values = datas[i].ToDic(Language).Values.ToArray();
                                                                     var temps  = datas[i].RecordTemperatures.ToArray();

                                                                     if (temps.Length > max_count)
                                                                     {
                                                                         max_count = temps.Length;
                                                                     }

                                                                     for (var j = 0; j < values.Length; j++)
                                                                     {
                                                                         if (values[j] is DateTime date)
                                                                         {
                                                                             wsht.Cells[i + 4, j + 1].Value                     = date.ToOADate();
                                                                             wsht.Cells[i + 4, j + 1].Style.Numberformat.Format = "yyyy/MM/dd HH:mm:ss";
                                                                         }
                                                                         else if (values[j] is string str)
                                                                         {
                                                                             wsht.Cells[i + 4, j + 1].Value                     = str;
                                                                             wsht.Cells[i + 4, j + 1].Style.Numberformat.Format = "@";
                                                                         }
                                                                         else
                                                                         {
                                                                             wsht.Cells[i + 4, j + 1].Value = values[j];
                                                                         }
                                                                     }

                                                                     var sheet_name = $"Records {i + 1}";
                                                                     wsht.Cells[i + 4, values.Length - 2].Formula = $"HYPERLINK(\"#'{sheet_name}'!$A$4\",\"@\")";
                                                                     wsht.Cells[i + 4, values.Length - 2].Style.Font.Color.SetColor(Color.Blue);
                                                                     wsht.Cells[i + 4, values.Length - 2].Style.Font.UnderLine = false;

                                                                     var record_sht = xlwb.Workbook.Worksheets.Add(sheet_name);
                                                                     record_sht.View.ShowGridLines = false;
                                                                     record_sht.View.FreezePanes(4, 1);
                                                                     record_sht.Row(1).Height = 225;

                                                                     record_sht.Cells[2, 1].Formula = $"HYPERLINK(\"#'{wsht.Name}'!$A${i + 4}\",\"<<<<<<Back\")";
                                                                     record_sht.Cells[2, 1].Style.Font.Color.SetColor(Color.Blue);
                                                                     record_sht.Cells[2, 1].Style.Font.UnderLine = false;
                                                                     record_sht.Cells[3, 1].Value                = nameof(RecordTemperatures.Time);
                                                                     record_sht.Cells[3, 2].Value                = nameof(RecordTemperatures.PV_ThermostatTemperature);
                                                                     record_sht.Cells[3, 3].Value                = nameof(RecordTemperatures.OvenTemperatures_1);
                                                                     record_sht.Cells[3, 4].Value                = nameof(RecordTemperatures.OvenTemperatures_2);
                                                                     record_sht.Cells[3, 5].Value                = nameof(RecordTemperatures.OvenTemperatures_3);
                                                                     record_sht.Cells[3, 6].Value                = nameof(RecordTemperatures.OvenTemperatures_4);
                                                                     record_sht.Cells[3, 7].Value                = nameof(RecordTemperatures.OvenTemperatures_5);
                                                                     record_sht.Cells[3, 8].Value                = nameof(RecordTemperatures.OvenTemperatures_6);
                                                                     //record_sht.Cells[3, 9].Value = nameof(RecordTemperatures.OvenTemperatures_7);
                                                                     //record_sht.Cells[3, 10].Value = nameof(RecordTemperatures.OvenTemperatures_8);
                                                                     record_sht.Cells[3, 1, 3, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                     record_sht.Cells[3, 1, 3, 10].Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);

                                                                     for (var j = 0; j < temps.Length; j++)
                                                                     {
                                                                         record_sht.Cells[4 + j, 1].Value                     = temps[j].Time;
                                                                         record_sht.Cells[4 + j, 1].Style.Numberformat.Format = "[h]:mm:ss";
                                                                         record_sht.Cells[4 + j, 2].Value                     = temps[j].PV_ThermostatTemperature;
                                                                         record_sht.Cells[4 + j, 3].Value                     = temps[j].OvenTemperatures_1;
                                                                         record_sht.Cells[4 + j, 4].Value                     = temps[j].OvenTemperatures_2;
                                                                         record_sht.Cells[4 + j, 5].Value                     = temps[j].OvenTemperatures_3;
                                                                         record_sht.Cells[4 + j, 6].Value                     = temps[j].OvenTemperatures_4;
                                                                         record_sht.Cells[4 + j, 7].Value                     = temps[j].OvenTemperatures_5;
                                                                         record_sht.Cells[4 + j, 8].Value                     = temps[j].OvenTemperatures_6;
                                                                         //record_sht.Cells[4 + j, 9].Value                     = temps[j].OvenTemperatures_7;
                                                                         //record_sht.Cells[4 + j, 10].Value                    = temps[j].OvenTemperatures_8;
                                                                     }

                                                                     record_sht.Cells[3, 1, temps.Length + 3, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                                     record_sht.Cells[3, 1, temps.Length + 3, 10].Style.Border.Left.Style   = ExcelBorderStyle.Thin;
                                                                     record_sht.Cells[3, 1, temps.Length + 3, 10].Style.Border.Right.Style  = ExcelBorderStyle.Thin;
                                                                     record_sht.Cells[3, 1, temps.Length + 3, 10].Style.Border.Top.Style    = ExcelBorderStyle.Thin;
                                                                     record_sht.Cells[3, 1, temps.Length + 3, 10].AutoFitColumns();

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
                                                                     var record_s1 = record_chart.Series.Add(record_sht.Cells[4, 2, temps.Length + 3, 2], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                                     var record_s2 = record_chart.Series.Add(record_sht.Cells[4, 3, temps.Length + 3, 3], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                                     var record_s3 = record_chart.Series.Add(record_sht.Cells[4, 4, temps.Length + 3, 4], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                                     var record_s4 = record_chart.Series.Add(record_sht.Cells[4, 5, temps.Length + 3, 5], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                                     var record_s5 = record_chart.Series.Add(record_sht.Cells[4, 6, temps.Length + 3, 6], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                                     var record_s6 = record_chart.Series.Add(record_sht.Cells[4, 7, temps.Length + 3, 7], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                                     var record_s7 = record_chart.Series.Add(record_sht.Cells[4, 8, temps.Length + 3, 8], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                                     //var record_s8 = record_chart.Series.Add(record_sht.Cells[4, 9, temps.Length + 3, 9], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                                     //var record_s9 = record_chart.Series.Add(record_sht.Cells[4, 10, temps.Length + 3, 10], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                                     record_s1.Header = nameof(RecordTemperatures.PV_ThermostatTemperature);
                                                                     record_s2.Header = nameof(RecordTemperatures.OvenTemperatures_1);
                                                                     record_s3.Header = nameof(RecordTemperatures.OvenTemperatures_2);
                                                                     record_s4.Header = nameof(RecordTemperatures.OvenTemperatures_3);
                                                                     record_s5.Header = nameof(RecordTemperatures.OvenTemperatures_4);
                                                                     record_s6.Header = nameof(RecordTemperatures.OvenTemperatures_5);
                                                                     record_s7.Header = nameof(RecordTemperatures.OvenTemperatures_6);
                                                                     //record_s8.Header            = nameof(RecordTemperatures.OvenTemperatures_7);
                                                                     //record_s9.Header            = nameof(RecordTemperatures.OvenTemperatures_8);
                                                                     record_s1.Border.Fill.Color = Color.Red;
                                                                     record_s2.Border.Fill.Color = Color.DarkOrange;
                                                                     record_s3.Border.Fill.Color = Color.Gold;
                                                                     record_s4.Border.Fill.Color = Color.Lime;
                                                                     record_s5.Border.Fill.Color = Color.DodgerBlue;
                                                                     record_s6.Border.Fill.Color = Color.DarkOrchid;
                                                                     record_s7.Border.Fill.Color = Color.Magenta;
                                                                     //record_s8.Border.Fill.Color = Color.Brown;
                                                                     //record_s9.Border.Fill.Color = Color.BurlyWood;

                                                                     record_chart.XAxis.Title.Text = "Timespan (H:M:S)";
                                                                     record_chart.XAxis.Title.Font.SetFromFont(new Font("Segoe UI", 11, FontStyle.Bold));
                                                                     record_chart.XAxis.MajorTickMark = eAxisTickMark.In;
                                                                     record_chart.XAxis.MinorTickMark = eAxisTickMark.None;
                                                                     record_chart.XAxis.Font.SetFromFont(new Font("Calibri", 10, FontStyle.Regular));
                                                                     record_chart.XAxis.TickLabelPosition = eTickLabelPosition.NextTo;
                                                                     record_chart.YAxis.Title.Text        = "Temperature (°C)";
                                                                     record_chart.YAxis.Title.Font.SetFromFont(new Font("Segoe UI", 11, FontStyle.Bold));
                                                                     record_chart.YAxis.Title.Rotation = 270;
                                                                     record_chart.YAxis.MajorTickMark  = eAxisTickMark.In;
                                                                     record_chart.YAxis.MinorTickMark  = eAxisTickMark.None;
                                                                     record_chart.YAxis.Font.SetFromFont(new Font("Calibri", 10, FontStyle.Regular));
                                                                     record_chart.RoundedCorners    = false;
                                                                     record_chart.Border.Fill.Color = Color.Black;
                                                                     record_chart.SetPosition(0, 0, 0, 0);

                                                                     record_sht.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                                     record_sht.Cells.Style.Font.SetFromFont(new Font("Segoe UI", 11, FontStyle.Regular));
                                                                     record_sht.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                                 }

                                                                 wsht.Cells[3, 1, n + 3, keys.Length].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                                 wsht.Cells[3, 1, n + 3, keys.Length].Style.Border.Left.Style   = ExcelBorderStyle.Thin;
                                                                 wsht.Cells[3, 1, n + 3, keys.Length].Style.Border.Right.Style  = ExcelBorderStyle.Thin;
                                                                 wsht.Cells[3, 1, n + 3, keys.Length].Style.Border.Top.Style    = ExcelBorderStyle.Thin;
                                                                 wsht.Cells[3, 1, n + 3, keys.Length].AutoFitColumns();

                                                                 wsht.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                                 wsht.Cells.Style.Font.SetFromFont(new Font("Segoe UI", 11, FontStyle.Regular));
                                                                 wsht.Cells[2, 1].Style.HorizontalAlignment         = ExcelHorizontalAlignment.Left;
                                                                 wsht.Column(keys.Length).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
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
                                                                     data_sht.Cells[i, 1].Style.Numberformat.Format = "[h]:mm:ss";
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
                                                                 var s1 = chart.Series.Add(data_sht.Cells[1, 2, max_count, 2], data_sht.Cells[1, 1, max_count, 1]);
                                                                 var s2 = chart.Series.Add(data_sht.Cells[1, 3, max_count, 3], data_sht.Cells[1, 1, max_count, 1]);
                                                                 var s3 = chart.Series.Add(data_sht.Cells[1, 4, max_count, 4], data_sht.Cells[1, 1, max_count, 1]);
                                                                 var s4 = chart.Series.Add(data_sht.Cells[1, 5, max_count, 5], data_sht.Cells[1, 1, max_count, 1]);
                                                                 var s5 = chart.Series.Add(data_sht.Cells[1, 6, max_count, 6], data_sht.Cells[1, 1, max_count, 1]);
                                                                 var s6 = chart.Series.Add(data_sht.Cells[1, 7, max_count, 7], data_sht.Cells[1, 1, max_count, 1]);
                                                                 var s7 = chart.Series.Add(data_sht.Cells[1, 8, max_count, 8], data_sht.Cells[1, 1, max_count, 1]);
                                                                 //var s8 = chart.Series.Add(data_sht.Cells[1, 9, max_count, 9], data_sht.Cells[1, 1, max_count, 1]);
                                                                 //var s9 = chart.Series.Add(data_sht.Cells[1, 10, max_count, 10], data_sht.Cells[1, 1, max_count, 1]);
                                                                 s1.Header = nameof(RecordTemperatures.PV_ThermostatTemperature);
                                                                 s2.Header = nameof(RecordTemperatures.OvenTemperatures_1);
                                                                 s3.Header = nameof(RecordTemperatures.OvenTemperatures_2);
                                                                 s4.Header = nameof(RecordTemperatures.OvenTemperatures_3);
                                                                 s5.Header = nameof(RecordTemperatures.OvenTemperatures_4);
                                                                 s6.Header = nameof(RecordTemperatures.OvenTemperatures_5);
                                                                 s7.Header = nameof(RecordTemperatures.OvenTemperatures_6);
                                                                 //s8.Header            = nameof(RecordTemperatures.OvenTemperatures_7);
                                                                 //s9.Header            = nameof(RecordTemperatures.OvenTemperatures_8);
                                                                 s1.Border.Fill.Color = Color.Red;
                                                                 s2.Border.Fill.Color = Color.DarkOrange;
                                                                 s3.Border.Fill.Color = Color.Gold;
                                                                 s4.Border.Fill.Color = Color.Lime;
                                                                 s5.Border.Fill.Color = Color.DodgerBlue;
                                                                 s6.Border.Fill.Color = Color.DarkOrchid;
                                                                 s7.Border.Fill.Color = Color.Magenta;
                                                                 //s8.Border.Fill.Color = Color.Brown;
                                                                 //s9.Border.Fill.Color = Color.BurlyWood;

                                                                 chart.XAxis.Title.Text = "Timespan (H:M:S)";
                                                                 chart.XAxis.Title.Font.SetFromFont(new Font("Segoe UI", 11, FontStyle.Bold));
                                                                 chart.XAxis.MajorTickMark = eAxisTickMark.In;
                                                                 chart.XAxis.MinorTickMark = eAxisTickMark.None;
                                                                 chart.XAxis.Font.SetFromFont(new Font("Calibri", 10, FontStyle.Regular));
                                                                 chart.XAxis.TickLabelPosition = eTickLabelPosition.NextTo;
                                                                 chart.YAxis.Title.Text        = "Temperature (°C)";
                                                                 chart.YAxis.Title.Font.SetFromFont(new Font("Segoe UI", 11, FontStyle.Bold));
                                                                 chart.YAxis.Title.Rotation = 270;
                                                                 chart.YAxis.MajorTickMark  = eAxisTickMark.In;
                                                                 chart.YAxis.MinorTickMark  = eAxisTickMark.None;
                                                                 chart.YAxis.Font.SetFromFont(new Font("Calibri", 10, FontStyle.Regular));
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
            categoryAxis1.ActualLabels.Clear();
            categoryAxis2.ActualLabels.Clear();

            ResultView.InvalidatePlot(true);

            if (ViewResults?.Count > 0)
            {
                var finishedResults = ViewResults.Where(x => x.IsFinished).ToList();

                var ByDate = date2 - date1 > TimeSpan.FromDays(1);
                var result2 = finishedResults
                             .GroupBy(x => Mode >= (int)ChartMode.ByLot ?
                                               x.StationNumber.ToString("00") :
                                               x.PartID)
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
                                                                     ChartMode.ByLot  => x.LotID,
                                                                     ChartMode.ByPart => x.PartID,
                                                                     _                => ByDate ? x.AddedTime.Date.ToString("MM/dd") : $"{x.AddedTime.Hour:00}:00"
                                                                 };
                                                      })
                                             .OrderBy(x => x.Key)
                                             .Select(x => (x.Key, x.Sum(y => y.Quantity)))
                                             .ToArray();

                categoryAxis1.FontSize = result1.Length > 20 ? 9 : 12;

                var color_step_1 = 0.9 / result1.Length;

                for (var i = 0; i < result1.Length; i++)
                {
                    var (result, count) = result1[i];
                    categories.Add(result);
                    categoryAxis1.ActualLabels.Add(result);
                    categoryAxis2.ActualLabels.Add(result);

                    var cs = new ColumnSeries
                             {
                                 FontSize          = result1.Length > 20 ? 8 : 10,
                                 LabelFormatString = "{0}",
                                 TextColor         = fontcolor,
                                 IsStacked         = true,
                                 StrokeThickness   = 0,
                                 StrokeColor       = NoLayer2 ? bordercolor : OxyColors.Transparent,
                                 FillColor         = NoLayer2 ? OxyColor.FromHsv(i * color_step_1, 0.9, 0.9) : OxyColors.Transparent,
                                 XAxisKey          = "2"
                             };

                    cs.Items.Add(new ColumnItem(count, i));
                    ResultView.Series.Add(cs);
                }

                if (!NoLayer2)
                {
                    ResultView.IsLegendVisible = true;
                    ResultView.LegendTitle     = Mode >= (int)ChartMode.ByLot ? 
                                                     ProcessInfoProperties[nameof(ProcessInfo.StationNumber)].GetName(Language) :
                                                     ProcessInfoProperties[nameof(ProcessInfo.PartID)].GetName(Language);

                    var color_step_2 = 0.9 / result2.Length;

                    for (var i = 0; i < result2.Length; i++)
                    {
                        var (cat, info) = result2[i];
                        var ccs = new ColumnSeries
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
                                      XAxisKey          = "1"
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
                                                             return x.LotID == categories[j];
                                                         case ChartMode.ByPart:
                                                             return x.PartID == categories[j];
                                                     }

                                                     if (ByDate)
                                                     {
                                                         return x.AddedTime.Date.ToString("MM/dd") == categories[j];
                                                     }

                                                     return $"{x.AddedTime.Hour:00}:00" == categories[j];
                                                 })
                                          .Sum(x => x.Quantity);
                            if (val > 0)
                            {
                                ccs.Items.Add(new ColumnItem(val, j));
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
                                     .Where(x => OvenFilter.Check(x.StationNumber) &&
                                                 RecipeFilter.Check(x.RecipeName) &&
                                                 OrderFilter.Check(x.OrderCode) &&
                                                 PartIDFilter.Check(x.PartID) &&
                                                 LotIDFilter.Check(x.LotID) &&
                                                 OpFilter.Check(x.OperatorID) &&
                                                 RackFilter.Check(x.RackID) &&
                                                 SideFilter.Check(x.Side))
                                     .OrderByDescending(x => x.AddedTime)
                                     .ToList();
            }

            NotifyPropertyChanged(nameof(ProduceTotalCount));
        }

        public TraceabilityView_ViewModel(IDataBase<ProcessInfo> db, IDialogService dialog) : base(db)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            LoadedCommand = new RelayCommand(e =>
                                             {
                                                 if (e is FrameworkElement el)
                                                 {
                                                     elementView = el;
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

                                               SearchResult = (await DataCollection.FindAsync(x => (!result1 || x.PartID.Contains(input1.ToString())) && (!result2 || x.LotID.Contains(input2.ToString())))).LastOrDefault();

                                               Standby = true;

                                               if (SearchResult == null)
                                               {
                                                   SearchResult = new ProcessInfo
                                                                  {
                                                                      StationNumber = -1
                                                                  };

                                                   UpdateResults(Date1, Date2);
                                               }
                                               else
                                               {
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

            var linearAxis = new LinearAxis
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
                                 MaximumPadding     = 0.15
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
                                Key           = "2"
                            };

            ResultView = new PlotModel
                         {
                             DefaultFont             = "Microsoft JhengHei",
                             PlotAreaBackground      = chartbg,
                             PlotAreaBorderColor     = bordercolor,
                             PlotAreaBorderThickness = new OxyThickness(0, 1, 1, 1),
                             PlotMargins             = new OxyThickness(35, 0, 0, 20),
                             LegendTitle             = ProcessInfoProperties[nameof(ProcessInfo.PartID)].GetName(Language),
                             LegendTitleColor        = fontcolor,
                             LegendTextColor         = fontcolor,
                             LegendBorder            = bordercolor,
                             LegendBackground        = bgcolor,
                             LegendPlacement         = LegendPlacement.Outside,
                             LegendPosition          = LegendPosition.RightTop,
                             LegendOrientation       = LegendOrientation.Vertical,
                             LegendFontSize          = 12,
                             LegendTitleFontSize     = 12,
                             //LegendItemOrder = LegendItemOrder.Reverse,
                             LegendMargin  = 4,
                             LegendPadding = 5,
                             TextColor     = fontcolor
                         };

            ResultView.Axes.Add(linearAxis);
            ResultView.Axes.Add(categoryAxis2);
            ResultView.Axes.Add(categoryAxis1);

            void UpdateAct()
            {
                UpdateViewResult();
                UpdateChart(Date1, Date2);
            }

            SelectedIndex = -1;
            EventIndex    = -1;
            OvenFilter    = new FilterGroup(UpdateAct);
            RecipeFilter  = new FilterGroup(UpdateAct);
            OrderFilter   = new FilterGroup(UpdateAct);
            PartIDFilter  = new FilterGroup(UpdateAct);
            LotIDFilter   = new FilterGroup(UpdateAct);
            OpFilter      = new FilterGroup(UpdateAct);
            RackFilter    = new FilterGroup(UpdateAct);
            SideFilter    = new FilterGroup(UpdateAct);

            ResultsChanged += async e =>
                              {
                                  OvenFilter.Filter   = e?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                                  RecipeFilter.Filter = e?.Select(x => x.RecipeName).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                                  OrderFilter.Filter  = e?.Select(x => x.OrderCode).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                                  PartIDFilter.Filter = e?.Select(x => x.PartID).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                                  LotIDFilter.Filter  = e?.Select(x => x.LotID).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                                  OpFilter.Filter     = e?.Select(x => x.OperatorID).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                                  RackFilter.Filter   = e?.Select(x => x.RackID).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();
                                  SideFilter.Filter   = e?.Select(x => x.Side).Distinct().OrderBy(x => x).Select(x => new EqualFilter(x)).ToList();

                                  TodayProductionUpdated?.Invoke(e?.Where(x => x.AddedTime.Day == DateTime.Today.Day && x.IsFinished)
                                                                   .GroupBy(x => x.StationNumber - 1)
                                                                   .Select(x => (x.Key, x.Sum(y => y.Quantity)))
                                                                   .ToList());

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
                                                          { Language.EN, "No data found!" }
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

                                      SearchEvent = null;

                                      if (EventIndex == -1)
                                      {
                                          dialog.Show(new Dictionary<Language, string>
                                                      {
                                                          { Language.TW, "查無事件！" },
                                                          { Language.CHS, "查无事件！" },
                                                          { Language.EN, "No event found!" }
                                                      },
                                                      DialogMsgType.Alarm);
                                      }
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
}