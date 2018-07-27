using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using MongoDB.Driver;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>
    ///     生產紀錄追蹤
    /// </summary>
    public class TraceabilityView_ViewModel : ViewModelBase
    {
        public enum ChartMode
        {
            ByDateTime,
            ByPLC,
            ByOrder,
            Pie
        }

        public delegate void TodayProduction(List<(int station, int production)> list);

        public Language Language = Language.TW;

        private readonly OxyColor bgcolor = OxyColor.FromRgb(240, 255, 235);
        private readonly OxyColor bordercolor = OxyColor.FromRgb(174, 187, 168);
        private readonly CategoryAxis categoryAxis1;
        private readonly CategoryAxis categoryAxis2;
        private readonly OxyColor fontcolor = OxyColor.FromRgb(50, 70, 60);

        private readonly MongoClient Mongo_Client;

        private DateTime _Date1;
        private DateTime _Date2;
        private int _FilterIndex;
        private int _Index1;
        private int _Index2;
        private int _Mode;
        private List<ProcessInfo> _Results;
        private bool _Standby = true;
        private List<ProcessInfo> _ViewResults;

        /// <summary>
        ///     位移+1天
        /// </summary>
        public RelayCommand AddDayCommand { get; }

        /// <summary>
        ///     位移+1月
        /// </summary>
        public RelayCommand AddMonthCommand { get; }

        /// <summary>
        ///     位移+1週
        /// </summary>
        public RelayCommand AddWeekCommand { get; }

        /// <summary>
        ///     位移-1天
        /// </summary>
        public RelayCommand SubDayCommand { get; }

        /// <summary>
        ///     位移-1月
        /// </summary>
        public RelayCommand SubMonthCommand { get; }

        /// <summary>
        ///     位移-1週
        /// </summary>
        public RelayCommand SubWeekCommand { get; }

        /// <summary>
        ///     指定至本月
        /// </summary>
        public RelayCommand ThisMonthCommand { get; }

        /// <summary>
        ///     指定至本週
        /// </summary>
        public RelayCommand ThisWeekCommand { get; }

        /// <summary>
        ///     指定至本日
        /// </summary>
        public RelayCommand TodayCommand { get; }

        /// <summary>
        ///     輸出Excel報表
        /// </summary>
        public RelayCommand ToExcelCommand { get; }

        /// <summary>
        ///     基於PLC站號的Filter，站號由1開始
        /// </summary>
        public List<int> EnumFilter => _Results?.Select(x => x.StationNumber).Distinct().OrderBy(x => x).ToList();

        /// <summary>
        ///     日期範圍的開始
        /// </summary>
        public DateTime? LowerDate => _Results?.Count > 0 ? _Results[_Index1]?.AddedTime : null;

        /// <summary>
        ///     日期範圍的結束
        /// </summary>
        public DateTime? UpperDate => _Results?.Count > 0 ? _Results[_Index2]?.AddedTime : null;

        /// <summary>
        ///     總量統計
        /// </summary>
        public int ProduceTotalCount => _ViewResults?.Count > 0 ? _ViewResults.Sum(x => x.ProcessCount) : 0;

        public PlotModel ResultView { get; }

        public int TotalCount => _Results?.Count > 0 ? _Results.Count - 1 : 0;

        /// <summary>
        ///     選取的開始日期(資料庫)
        /// </summary>
        public DateTime Date1
        {
            get => _Date1;
            set
            {
                _Date1 = value;
                NotifyPropertyChanged();

                if (_Date2 < _Date1)
                {
                    _Date2 = _Date1;
                    NotifyPropertyChanged(nameof(Date2));
                }
                else if (_Date2 - Date1 > TimeSpan.FromDays(30))
                {
                    _Date2 = _Date1 + TimeSpan.FromDays(30);
                    NotifyPropertyChanged(nameof(Date2));
                }

                UpdateResults(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     選取的結束日期(資料庫)
        /// </summary>
        public DateTime Date2
        {
            get => _Date2;
            set
            {
                _Date2 = value;
                NotifyPropertyChanged();

                if (_Date1 > _Date2)
                {
                    _Date1 = _Date2;
                    NotifyPropertyChanged(nameof(Date1));
                }
                else if (_Date2 - Date1 > TimeSpan.FromDays(30))
                {
                    _Date1 = _Date2 - TimeSpan.FromDays(30);
                    NotifyPropertyChanged(nameof(Date1));
                }

                UpdateResults(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     切換烤箱站別顯示
        /// </summary>
        public int FilterIndex
        {
            get => _FilterIndex;
            set
            {
                _FilterIndex = value;
                NotifyPropertyChanged();

                UpdateViewResult();

                if (_FilterIndex != -1 && _Mode == (int)ChartMode.ByPLC)
                {
                    _Mode = (int)ChartMode.ByDateTime;
                    NotifyPropertyChanged(nameof(Mode));
                }

                UpdateChart(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     篩選的開始時間點(RAM)
        /// </summary>
        public int Index1
        {
            get => _Index1;
            set
            {
                _Index1 = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(LowerDate));
                NotifyPropertyChanged(nameof(EnumFilter));

                UpdateViewResult();

                UpdateChart(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     篩選的結束時間點(RAM)
        /// </summary>
        public int Index2
        {
            get => _Index2;
            set
            {
                _Index2 = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(UpperDate));
                NotifyPropertyChanged(nameof(EnumFilter));

                UpdateViewResult();

                UpdateChart(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     切換分類統計
        /// </summary>
        public int Mode
        {
            get => _Mode;
            set
            {
                if (_FilterIndex != -1 && value == (int)ChartMode.ByPLC)
                {
                    value = _Mode == (int)ChartMode.ByDateTime ? (int)ChartMode.ByOrder : (int)ChartMode.ByDateTime;
                }

                _Mode = value;
                NotifyPropertyChanged();

                UpdateChart(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     資料庫查詢結果
        /// </summary>
        public List<ProcessInfo> Results
        {
            get => _Results;
            set
            {
                _Results = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TotalCount));
                NotifyPropertyChanged(nameof(EnumFilter));

                TodayProductionUpdated?.Invoke(_Results?.Where(x => x.AddedTime.Day == DateTime.Today.Day).GroupBy(x => x.StationNumber).Select(x => (x.Key, x.Sum(y => y.ProcessCount))).ToList());

                UpdateViewResult();

                UpdateChart(_Date1, _Date2);
            }
        }

        /// <summary>
        ///     辨別是否處在讀取資料中
        /// </summary>
        public bool Standby
        {
            get => _Standby;
            set
            {
                _Standby = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     顯示的資料列表
        /// </summary>
        public List<ProcessInfo> ViewResults
        {
            get => _ViewResults;
            set
            {
                _ViewResults = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     本日產量更新事件
        /// </summary>
        public event TodayProduction TodayProductionUpdated;

        /// <summary>
        ///     新增至資料庫
        /// </summary>
        /// <param name="index">PLC序號，由0開始</param>
        /// <param name="info">紀錄資訊</param>
        /// <param name="dateTime">紀錄時間，預設為當下時間，帶入default(DateTime)同樣為當下時間</param>
        /// <param name="UpdateResult">決定是否更新Ram Data</param>
        public async void AddToDB(int index, ProcessInfo info, DateTime dateTime = default(DateTime), bool UpdateResult = false)
        {
            info.StationNumber = index;
            info.AddedTime = dateTime == default(DateTime) ? DateTime.Now : dateTime;

            try
            {
                var db = Mongo_Client.GetDatabase("GP");
                var Sets = db.GetCollection<ProcessInfo>("Product_Infos");

                await Sets.InsertOneAsync(info);

                if (UpdateResult)
                {
                    Results = await (await Sets.FindAsync(x => x.AddedTime >= _Date1 && x.AddedTime < _Date2.AddDays(1))).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorRecoder.RecordError(ex, "生產紀錄寫入資料庫失敗");
            }
        }

        /// <summary>
        ///     更新統計圖
        /// </summary>
        /// <param name="date1">開始日期</param>
        /// <param name="date2">結束日期</param>
        public void UpdateChart(DateTime date1, DateTime date2)
        {
            ResultView.Series.Clear();
            ResultView.IsLegendVisible = false;
            categoryAxis1.ActualLabels.Clear();
            categoryAxis2.ActualLabels.Clear();

            if (_ViewResults?.Count > 0)
            {
                var ByDate = date2 - date1 > TimeSpan.FromDays(1);
                var result2 = _ViewResults.GroupBy(x => (ChartMode)_Mode == ChartMode.ByOrder ? (x.StationNumber + 1).ToString("00") : x.OrderCode)
                                          .OrderBy(x => x.Key)
                                          .Select(x => (x.Key, x))
                                          .ToArray();

                var NoLayer2 = result2.Length > 20 || _FilterIndex != -1 && (ChartMode)_Mode == ChartMode.ByOrder;
                var categories = new List<string>();

                var result1 = _ViewResults.GroupBy(x =>
                                                   {
                                                       if ((ChartMode)_Mode == ChartMode.ByPLC && _FilterIndex == -1)
                                                       {
                                                           return (x.StationNumber + 1).ToString("00");
                                                       }

                                                       if ((ChartMode)_Mode == ChartMode.ByOrder)
                                                       {
                                                           return x.OrderCode;
                                                       }

                                                       return ByDate ? x.AddedTime.Date.ToString("MM/dd") : x.AddedTime.Hour.ToString("00") + ":00";
                                                   })
                                          .OrderBy(x => x.Key)
                                          .Select(x => (x.Key, x.Sum(y => y.ProcessCount)))
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
                                 FontSize = result1.Length > 20 ? 8 : 10,
                                 LabelFormatString = "{0}",
                                 TextColor = fontcolor,
                                 IsStacked = true,
                                 StrokeThickness = 0,
                                 StrokeColor = NoLayer2 ? bordercolor : OxyColors.Transparent,
                                 FillColor = NoLayer2 ? OxyColor.FromHsv(i * color_step_1, 1, 1) : OxyColors.Transparent,
                                 XAxisKey = "2"
                             };

                    cs.Items.Add(new ColumnItem(count, i));
                    ResultView.Series.Add(cs);
                }

                if (!NoLayer2)
                {
                    ResultView.IsLegendVisible = true;
                    ResultView.LegendTitle = (ChartMode)_Mode == ChartMode.ByOrder ? nameof(ProcessInfo.StationNumber) : nameof(ProcessInfo.OrderCode);

                    var color_step_2 = 0.9 / result2.Length;

                    for (var i = 0; i < result2.Length; i++)
                    {
                        var (cat, info) = result2[i];
                        var ccs = new ColumnSeries
                                  {
                                      FontSize = 10,
                                      LabelFormatString = result2.Length > 10 || categories.Count > 20 ? "" : "{0}",
                                      LabelPlacement = LabelPlacement.Middle,
                                      TextColor = OxyColors.White,
                                      Title = cat,
                                      IsStacked = true,
                                      StrokeThickness = 0,
                                      StrokeColor = bordercolor,
                                      FillColor = OxyColor.FromHsv(i * color_step_2, 1, 1),
                                      XAxisKey = "1"
                                  };

                        for (var j = 0; j < categories.Count; j++)
                        {
                            var val = info.Where(x =>
                                                 {
                                                     if ((ChartMode)_Mode == ChartMode.ByPLC && _FilterIndex == -1)
                                                     {
                                                         return (x.StationNumber + 1).ToString("00") == categories[j];
                                                     }

                                                     if ((ChartMode)_Mode == ChartMode.ByOrder)
                                                     {
                                                         return x.OrderCode == categories[j];
                                                     }

                                                     if (ByDate)
                                                     {
                                                         return x.AddedTime.Date.ToString("MM/dd") == categories[j];
                                                     }

                                                     return x.AddedTime.Hour.ToString("00") + ":00" == categories[j];
                                                 })
                                          .Sum(x => x.ProcessCount);
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

        /// <summary>
        ///     依據條件，更新查詢資料庫結果列表
        /// </summary>
        /// <param name="date1">起始時間</param>
        /// <param name="date2">結束時間</param>
        public async void UpdateResults(DateTime date1, DateTime date2)
        {
            Standby = false;

            try
            {
                var db = Mongo_Client.GetDatabase("GP");
                var Sets = db.GetCollection<ProcessInfo>("Product_Infos");

                Results = await (await Sets.FindAsync(x => x.AddedTime >= date1 && x.AddedTime < date2.AddDays(1))).ToListAsync();
            }
            catch (Exception)
            {
            }

            Standby = true;
        }

        public async void SaveToExcel(string save_path)
        {
            Standby = false;

            var dic = save_path + "\\Reports";
            if (!Directory.Exists(dic))
            {
                Directory.CreateDirectory(dic);
            }

            if (_ViewResults.Any())
            {
                await Task.Factory.StartNew(() =>
                                            {
                                                var fi = new FileInfo(dic + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".xlsx");

                                                var n = _ViewResults.Count;
                                                var xlwb = new ExcelPackage();
                                                var wsht = xlwb.Workbook.Worksheets.Add(n + (n <= 1 ? " result" : " results"));
                                                wsht.View.ShowGridLines = false;
                                                wsht.View.FreezePanes(4, 2);
                                                wsht.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                wsht.Cells.Style.Font.SetFromFont(new Font("Segoe UI", 11, FontStyle.Regular));

                                                var keys = _ViewResults[0].ToDic(Language).Keys.ToArray();

                                                for (var i = 0; i < keys.Length; i++)
                                                {
                                                    wsht.Cells[3, i + 1].Value = keys[i];
                                                    wsht.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                    wsht.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);
                                                }

                                                for (var i = 0; i < n; i++)
                                                {
                                                    var values = _ViewResults[i].ToDic(Language).Values.ToArray();
                                                    var temps = _ViewResults[i].RecordTemperatures.ToArray();

                                                    for (var j = 0; j < values.Length; j++)
                                                    {
                                                        if (values[j] is DateTime date)
                                                        {
                                                            wsht.Cells[i + 4, j + 1].Value = date.ToOADate();
                                                            wsht.Cells[i + 4, j + 1].Style.Numberformat.Format = "yyyy/MM/dd HH:mm:ss";
                                                        }
                                                        else if (values[j] is string str)
                                                        {
                                                            wsht.Cells[i + 4, j + 1].Value = str;
                                                            wsht.Cells[i + 4, j + 1].Style.Numberformat.Format = "@";
                                                        }
                                                        else
                                                        {
                                                            wsht.Cells[i + 4, j + 1].Value = values[j];
                                                        }
                                                    }

                                                    var sheet_name = "Records " + (i + 1);
                                                    wsht.Cells[i + 4, values.Length + 1].Value = "'" + sheet_name + "'!" + "$A$4:$A$" + (temps.Length + 3);
                                                    wsht.Cells[i + 4, values.Length + 2].Value = "'" + sheet_name + "'!" + "$B$4:$B$" + (temps.Length + 3);
                                                    wsht.Cells[i + 4, values.Length + 3].Value = "'" + sheet_name + "'!" + "$C$4:$C$" + (temps.Length + 3);
                                                    wsht.Cells[i + 4, values.Length + 4].Value = "'" + sheet_name + "'!" + "$D$4:$D$" + (temps.Length + 3);
                                                    wsht.Cells[i + 4, values.Length + 5].Value = "'" + sheet_name + "'!" + "$E$4:$E$" + (temps.Length + 3);
                                                    wsht.Cells[i + 4, values.Length + 6].Value = "'" + sheet_name + "'!" + "$F$4:$F$" + (temps.Length + 3);
                                                    wsht.Cells[i + 4, values.Length + 7].Value = "'" + sheet_name + "'!" + "$G$4:$G$" + (temps.Length + 3);
                                                    wsht.Cells[i + 4, values.Length + 8].Value = "'" + sheet_name + "'!" + "$H$4:$H$" + (temps.Length + 3);
                                                    wsht.Cells[i + 4, values.Length + 9].Value = "'" + sheet_name + "'!" + "$I$4:$I$" + (temps.Length + 3);
                                                    wsht.Cells[i + 4, values.Length + 10].Value = "'" + sheet_name + "'!" + "$J$4:$J$" + (temps.Length + 3);

                                                    var record_sht = xlwb.Workbook.Worksheets.Add(sheet_name);
                                                    record_sht.View.ShowGridLines = false;
                                                    record_sht.View.FreezePanes(4, 2);
                                                    record_sht.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                    record_sht.Cells.Style.Font.SetFromFont(new Font("Segoe UI", 11, FontStyle.Regular));
                                                    record_sht.Cells[3, 1].Value = nameof(RecordTemperatures.Time);
                                                    record_sht.Cells[3, 2].Value = nameof(RecordTemperatures.ThermostatTemperature);
                                                    record_sht.Cells[3, 3].Value = nameof(RecordTemperatures.OvenTemperatures_0);
                                                    record_sht.Cells[3, 4].Value = nameof(RecordTemperatures.OvenTemperatures_1);
                                                    record_sht.Cells[3, 5].Value = nameof(RecordTemperatures.OvenTemperatures_2);
                                                    record_sht.Cells[3, 6].Value = nameof(RecordTemperatures.OvenTemperatures_3);
                                                    record_sht.Cells[3, 7].Value = nameof(RecordTemperatures.OvenTemperatures_4);
                                                    record_sht.Cells[3, 8].Value = nameof(RecordTemperatures.OvenTemperatures_5);
                                                    record_sht.Cells[3, 9].Value = nameof(RecordTemperatures.OvenTemperatures_6);
                                                    record_sht.Cells[3, 10].Value = nameof(RecordTemperatures.OvenTemperatures_7);
                                                    record_sht.Cells[3, 1, 3, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                    record_sht.Cells[3, 1, 3, 10].Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);

                                                    for (var j = 0; j < temps.Length; j++)
                                                    {
                                                        record_sht.Cells[4 + j, 1].Value = temps[j].Time;
                                                        record_sht.Cells[4 + j, 1].Style.Numberformat.Format = "[h]:mm:ss";
                                                        record_sht.Cells[4 + j, 2].Value = temps[j].ThermostatTemperature;
                                                        record_sht.Cells[4 + j, 3].Value = temps[j].OvenTemperatures_0;
                                                        record_sht.Cells[4 + j, 4].Value = temps[j].OvenTemperatures_1;
                                                        record_sht.Cells[4 + j, 5].Value = temps[j].OvenTemperatures_2;
                                                        record_sht.Cells[4 + j, 6].Value = temps[j].OvenTemperatures_3;
                                                        record_sht.Cells[4 + j, 7].Value = temps[j].OvenTemperatures_4;
                                                        record_sht.Cells[4 + j, 8].Value = temps[j].OvenTemperatures_5;
                                                        record_sht.Cells[4 + j, 9].Value = temps[j].OvenTemperatures_6;
                                                        record_sht.Cells[4 + j, 10].Value = temps[j].OvenTemperatures_7;
                                                    }

                                                    //record_sht.Cells[2, 1].Value = "$A$4:$A$" + (temps.Length + 3);
                                                    //record_sht.Cells[2, 2].Value = "$B$4:$B$" + (temps.Length + 3);
                                                    //record_sht.Cells[2, 3].Value = "$C$4:$C$" + (temps.Length + 3);
                                                    record_sht.Cells[3, 1, temps.Length + 3, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                    record_sht.Cells[3, 1, temps.Length + 3, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                    record_sht.Cells[3, 1, temps.Length + 3, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                    record_sht.Cells[3, 1, temps.Length + 3, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                    record_sht.Cells[3, 1, temps.Length + 3, 10].AutoFitColumns();

                                                    var chart = (ExcelLineChart)record_sht.Drawings.AddChart("", eChartType.Line);
                                                    chart.SetSize(900, 300);
                                                    var s1 = chart.Series.Add(record_sht.Cells[4, 2, temps.Length + 3, 2], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                    var s2 = chart.Series.Add(record_sht.Cells[4, 3, temps.Length + 3, 3], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                    var s3 = chart.Series.Add(record_sht.Cells[4, 4, temps.Length + 3, 4], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                    var s4 = chart.Series.Add(record_sht.Cells[4, 5, temps.Length + 3, 5], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                    var s5 = chart.Series.Add(record_sht.Cells[4, 6, temps.Length + 3, 6], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                    var s6 = chart.Series.Add(record_sht.Cells[4, 7, temps.Length + 3, 7], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                    var s7 = chart.Series.Add(record_sht.Cells[4, 8, temps.Length + 3, 8], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                    var s8 = chart.Series.Add(record_sht.Cells[4, 9, temps.Length + 3, 9], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                    var s9 = chart.Series.Add(record_sht.Cells[4, 10, temps.Length + 3, 10], record_sht.Cells[4, 1, temps.Length + 3, 1]);
                                                    s1.Header = nameof(RecordTemperatures.ThermostatTemperature);
                                                    s2.Header = nameof(RecordTemperatures.OvenTemperatures_0);
                                                    s3.Header = nameof(RecordTemperatures.OvenTemperatures_1);
                                                    s4.Header = nameof(RecordTemperatures.OvenTemperatures_2);
                                                    s5.Header = nameof(RecordTemperatures.OvenTemperatures_3);
                                                    s6.Header = nameof(RecordTemperatures.OvenTemperatures_4);
                                                    s7.Header = nameof(RecordTemperatures.OvenTemperatures_5);
                                                    s8.Header = nameof(RecordTemperatures.OvenTemperatures_6);
                                                    s9.Header = nameof(RecordTemperatures.OvenTemperatures_7);
                                                    s1.Border.Fill.Color = Color.Red;
                                                    s2.Border.Fill.Color = Color.DarkOrange;
                                                    s1.Border.Fill.Color = Color.Gold;
                                                    s4.Border.Fill.Color = Color.Lime;
                                                    s5.Border.Fill.Color = Color.DodgerBlue;
                                                    s6.Border.Fill.Color = Color.DarkOrchid;
                                                    s7.Border.Fill.Color = Color.Magenta;
                                                    s8.Border.Fill.Color = Color.Brown;
                                                    s9.Border.Fill.Color = Color.BurlyWood;

                                                    record_sht.Row(1).Height = 230;

                                                    chart.XAxis.Title.Text = "Time";
                                                    chart.XAxis.Title.Font.Size = 12;
                                                    chart.YAxis.Title.Text = "°C";
                                                    chart.YAxis.Title.Font.Size = 12;
                                                    chart.Border.Width = 0;
                                                    chart.Border.Fill.Color = Color.Transparent;
                                                    chart.SetPosition(0, 0, 1, 0);
                                                }

                                                wsht.Cells[3, 1, _ViewResults.Count + 3, keys.Length].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                wsht.Cells[3, 1, _ViewResults.Count + 3, keys.Length].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                wsht.Cells[3, 1, _ViewResults.Count + 3, keys.Length].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                wsht.Cells[3, 1, _ViewResults.Count + 3, keys.Length].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                wsht.Cells[3, 1, _ViewResults.Count + 3, keys.Length].AutoFitColumns();

                                                wsht.Cells[1, 1].Formula = "INDIRECT(\"$" + (keys.Length + 1).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                wsht.Cells[1, 2].Formula = "INDIRECT(\"$" + (keys.Length + 2).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                wsht.Cells[1, 3].Formula = "INDIRECT(\"$" + (keys.Length + 3).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                wsht.Cells[1, 4].Formula = "INDIRECT(\"$" + (keys.Length + 4).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                wsht.Cells[1, 5].Formula = "INDIRECT(\"$" + (keys.Length + 5).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                wsht.Cells[1, 6].Formula = "INDIRECT(\"$" + (keys.Length + 6).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                wsht.Cells[1, 7].Formula = "INDIRECT(\"$" + (keys.Length + 7).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                wsht.Cells[1, 8].Formula = "INDIRECT(\"$" + (keys.Length + 8).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                wsht.Cells[1, 9].Formula = "INDIRECT(\"$" + (keys.Length + 9).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                wsht.Cells[1, 10].Formula = "INDIRECT(\"$" + (keys.Length + 10).GetExcelColumnName() + "$\"&CELL(\"row\"))";
                                                //wsht.Cells[2, 1].Formula = "INDIRECT(\"$" + (keys.Length + 1).GetExcelColumnName() + "$4\")";

                                                var _chart = (ExcelLineChart)wsht.Drawings.AddChart("", eChartType.Line);
                                                _chart.SetSize(900, 300);
                                                var _s1 = _chart.Series.Add(wsht.Cells[4, 2, 4, 2], wsht.Cells[1, 1]);
                                                var _s2 = _chart.Series.Add(wsht.Cells[4, 3, 4, 3], wsht.Cells[1, 1]);
                                                var _s3 = _chart.Series.Add(wsht.Cells[4, 4, 4, 4], wsht.Cells[1, 1]);
                                                var _s4 = _chart.Series.Add(wsht.Cells[4, 5, 4, 5], wsht.Cells[1, 1]);
                                                var _s5 = _chart.Series.Add(wsht.Cells[4, 6, 4, 6], wsht.Cells[1, 1]);
                                                var _s6 = _chart.Series.Add(wsht.Cells[4, 7, 4, 7], wsht.Cells[1, 1]);
                                                var _s7 = _chart.Series.Add(wsht.Cells[4, 8, 4, 8], wsht.Cells[1, 1]);
                                                var _s8 = _chart.Series.Add(wsht.Cells[4, 9, 4, 9], wsht.Cells[1, 1]);
                                                var _s9 = _chart.Series.Add(wsht.Cells[4, 10, 4, 10], wsht.Cells[1, 1]);

                                                var time = new ExcelNamedRange("time", null, wsht, "A2", 1) { Formula = "INDIRECT(\"$A$1\")" };
                                                var t0 = new ExcelNamedRange("t0", null, wsht, "B2", 2) { Formula = "INDIRECT(\"$B$1\")" };
                                                var t1 = new ExcelNamedRange("t1", null, wsht, "C2", 3) { Formula = "INDIRECT(\"$C$1\")" };
                                                var t2 = new ExcelNamedRange("t2", null, wsht, "D2", 4) { Formula = "INDIRECT(\"$D$1\")" };
                                                var t3 = new ExcelNamedRange("t3", null, wsht, "E2", 5) { Formula = "INDIRECT(\"$E$1\")" };
                                                var t4 = new ExcelNamedRange("t4", null, wsht, "F2", 6) { Formula = "INDIRECT(\"$F$1\")" };
                                                var t5 = new ExcelNamedRange("t5", null, wsht, "G2", 7) { Formula = "INDIRECT(\"$G$1\")" };
                                                var t6 = new ExcelNamedRange("t6", null, wsht, "H2", 8) { Formula = "INDIRECT(\"$H$1\")" };
                                                var t7 = new ExcelNamedRange("t7", null, wsht, "I2", 9) { Formula = "INDIRECT(\"$I$1\")" };
                                                var t8 = new ExcelNamedRange("t8", null, wsht, "J2", 10) { Formula = "INDIRECT(\"$J$1\")" };

                                                wsht.Names.Add("time", time);
                                                wsht.Names.Add("t0", t0);
                                                wsht.Names.Add("t1", t1);
                                                wsht.Names.Add("t2", t2);
                                                wsht.Names.Add("t3", t3);
                                                wsht.Names.Add("t4", t4);
                                                wsht.Names.Add("t5", t5);
                                                wsht.Names.Add("t6", t6);
                                                wsht.Names.Add("t7", t7);
                                                wsht.Names.Add("t8", t8);

                                                var x = "time";
                                                _s1.XSeries = x;
                                                _s1.Series = "t0";
                                                _s2.XSeries = x;
                                                _s2.Series = "t1";
                                                _s3.XSeries = x;
                                                _s3.Series = "t2";
                                                _s4.XSeries = x;
                                                _s4.Series = "t3";
                                                _s5.XSeries = x;
                                                _s5.Series = "t4";
                                                _s6.XSeries = x;
                                                _s6.Series = "t5";
                                                _s7.XSeries = x;
                                                _s7.Series = "t6";
                                                _s8.XSeries = x;
                                                _s8.Series = "t7";
                                                _s9.XSeries = x;
                                                _s9.Series = "t8";
                                                _s1.Header = nameof(RecordTemperatures.ThermostatTemperature);
                                                _s2.Header = nameof(RecordTemperatures.OvenTemperatures_0);
                                                _s3.Header = nameof(RecordTemperatures.OvenTemperatures_1);
                                                _s4.Header = nameof(RecordTemperatures.OvenTemperatures_2);
                                                _s5.Header = nameof(RecordTemperatures.OvenTemperatures_3);
                                                _s6.Header = nameof(RecordTemperatures.OvenTemperatures_4);
                                                _s7.Header = nameof(RecordTemperatures.OvenTemperatures_5);
                                                _s8.Header = nameof(RecordTemperatures.OvenTemperatures_6);
                                                _s9.Header = nameof(RecordTemperatures.OvenTemperatures_7);
                                                _s1.Border.Fill.Color = Color.Red;
                                                _s2.Border.Fill.Color = Color.DarkOrange;
                                                _s1.Border.Fill.Color = Color.Gold;
                                                _s4.Border.Fill.Color = Color.Lime;
                                                _s5.Border.Fill.Color = Color.DodgerBlue;
                                                _s6.Border.Fill.Color = Color.DarkOrchid;
                                                _s7.Border.Fill.Color = Color.Magenta;
                                                _s8.Border.Fill.Color = Color.Brown;
                                                _s9.Border.Fill.Color = Color.BurlyWood;

                                                wsht.Row(1).Height = 230;

                                                _chart.XAxis.Title.Text = "Time";
                                                _chart.XAxis.Title.Font.Size = 12;
                                                _chart.YAxis.Title.Text = "°C";
                                                _chart.YAxis.Title.Font.Size = 12;
                                                _chart.Border.Width = 0;
                                                _chart.Border.Fill.Color = Color.Transparent;
                                                _chart.SetPosition(0, 0, 1, 0);

                                                xlwb.SaveAs(fi);
                                                xlwb.Dispose();
                                            },
                                            TaskCreationOptions.LongRunning);
            }

            Standby = true;
        }

        private void UpdateViewResult()
        {
            ViewResults = _Index2 >= _Index1 && _Results?.Count > 0 ? _Results?.GetRange(_Index1, _Index2 - _Index1 + 1).Where(x => _FilterIndex == -1 || x.StationNumber == _FilterIndex).ToList() :
                              null;

            NotifyPropertyChanged(nameof(ProduceTotalCount));
        }

        public TraceabilityView_ViewModel(MongoClient mongo)
        {
            Mongo_Client = mongo;

            //!定義更新圖表的委派
            void Act()
            {
                NotifyPropertyChanged(nameof(Date1));
                NotifyPropertyChanged(nameof(Date2));

                UpdateResults(_Date1, _Date2);
            }

            SubDayCommand = new RelayCommand(o =>
                                             {
                                                 _Date1 = _Date1.AddDays(-1);
                                                 _Date2 = _Date1;

                                                 Act();
                                             });

            TodayCommand = new RelayCommand(o =>
                                            {
                                                _Date1 = DateTime.Today.Date;
                                                _Date2 = _Date1;

                                                Act();
                                            });

            AddDayCommand = new RelayCommand(o =>
                                             {
                                                 _Date1 = _Date1.AddDays(1);
                                                 _Date2 = _Date1;

                                                 Act();
                                             });

            SubWeekCommand = new RelayCommand(o =>
                                              {
                                                  _Date1 = _Date1.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
                                                  _Date2 = _Date1.AddDays(6);

                                                  Act();
                                              });

            ThisWeekCommand = new RelayCommand(o =>
                                               {
                                                   _Date1 = DateTime.Today.Date.StartOfWeek(DayOfWeek.Monday);
                                                   _Date2 = _Date1.AddDays(6);

                                                   Act();
                                               });

            AddWeekCommand = new RelayCommand(o =>
                                              {
                                                  _Date1 = _Date1.StartOfWeek(DayOfWeek.Monday).AddDays(7);
                                                  _Date2 = _Date1.AddDays(6);

                                                  Act();
                                              });

            SubMonthCommand = new RelayCommand(o =>
                                               {
                                                   _Date1 = new DateTime(_Date1.Year, _Date1.Month - 1, 1);
                                                   _Date2 = _Date1.AddMonths(1).AddDays(-1);

                                                   Act();
                                               });
            ThisMonthCommand = new RelayCommand(o =>
                                                {
                                                    _Date1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                                                    _Date2 = _Date1.AddMonths(1).AddDays(-1);

                                                    Act();
                                                });

            AddMonthCommand = new RelayCommand(o =>
                                               {
                                                   _Date1 = new DateTime(_Date1.Year, _Date1.Month + 1, 1);
                                                   _Date2 = _Date1.AddMonths(1).AddDays(-1);

                                                   Act();
                                               });

            ToExcelCommand = new RelayCommand(o =>
                                              {
                                                  SaveToExcel(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                                              });

            var linearAxis = new LinearAxis
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
                                 MaximumPadding = 0.15
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

            categoryAxis2 = new CategoryAxis { IsPanEnabled = false, IsZoomEnabled = false, IsAxisVisible = false, Key = "2" };

            ResultView = new PlotModel
                         {
                             DefaultFont = "Microsoft JhengHei",
                             PlotAreaBackground = bgcolor,
                             PlotAreaBorderColor = bordercolor,
                             PlotAreaBorderThickness = new OxyThickness(0, 1, 1, 1),
                             PlotMargins = new OxyThickness(35, 0, 0, 20),
                             LegendTitle = nameof(ProcessInfo.OrderCode),
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
                         };

            ResultView.Axes.Add(linearAxis);
            ResultView.Axes.Add(categoryAxis2);
            ResultView.Axes.Add(categoryAxis1);
        }
    }
}