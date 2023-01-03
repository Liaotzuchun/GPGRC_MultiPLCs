using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PooledCollections;
using Serilog;

namespace GPGO_MultiPLCs.Models;

public class DataoutputCSV
{
    private Language Language     = Language.TW;
    private string   AlarmTitles  = string.Empty;
    private string   DataTitles   = string.Empty;
    private string   RecipeTitles = string.Empty;
    private string   RecordTitles = string.Empty;

    public DataoutputCSV() => UpdateLanguage(Language.TW);

    private async Task DataMethod(ProcessInfo info, string folder, string filename)
    {
        folder = folder.Trim().TrimEnd('\\');
        if (!Directory.Exists(folder))
        {
            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Data資料夾不存在且無法創建");
                return;
            }
        }
        else
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                var fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddMonths(-3))
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        var sb       = new StringBuilder();
        var datapath = $"{folder}{filename}.csv";

        if (!File.Exists(datapath))
        {
            sb.AppendLine(DataTitles);
        }

        var recipe = info.Recipe.ToDictionary(Language);
        foreach (var product in info.Products)
        {
            using var vals = new[]
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

        try
        {
            using var outputFile = new StreamWriter(datapath, true, Encoding.UTF8);
            await outputFile.WriteAsync(sb.ToString());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Data CSV寫入失敗");
        }
    }

    private async Task RecordMethod(BaseInfo info, string folder, string filename)
    {
        folder = folder.Trim().TrimEnd('\\');
        if (!Directory.Exists(folder))
        {
            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Record資料夾不存在且無法創建");
                return;
            }
        }
        else
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                var fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddMonths(-3))
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        var sb       = new StringBuilder();
        var datapath = $"{folder}{filename}.csv";

        if (!File.Exists(datapath))
        {
            sb.AppendLine(RecordTitles);
        }

        foreach (var temp in info.RecordTemperatures)
        {
            var _temp = temp.ToDic(Language);
            sb.AppendLine(string.Join(",", _temp.Values));
        }

        try
        {
            using var outputFile = new StreamWriter(datapath, true, Encoding.UTF8);
            await outputFile.WriteAsync(sb.ToString());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Record CSV寫入失敗");
        }
    }

    private async Task AlarmMethod(LogEvent logEvent, string folder, string filename)
    {
        folder = folder.Trim().TrimEnd('\\');
        if (!Directory.Exists(folder))
        {
            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Alarm資料夾不存在且無法創建");
                return;
            }
        }
        else
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                var fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddMonths(-3))
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        var sb       = new StringBuilder();
        var datapath = $"{folder}{filename}.csv";

        if (!File.Exists(datapath))
        {
            sb.AppendLine(AlarmTitles);
        }

        var _temp = logEvent.ToDictionary();
        sb.AppendLine(string.Join(",", _temp.Values));

        try
        {
            using var outputFile = new StreamWriter(datapath, true, Encoding.UTF8);
            await outputFile.WriteAsync(sb.ToString());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Alarm CSV寫入失敗");
        }
    }

    public async Task AddEvent(LogEvent logEvent, string folderpath)
    {
        var d       = $"{DateTime.Now:yyyy-MM-dd}";
        var outpath = folderpath.Trim().TrimEnd('\\');
        if (!Directory.Exists(outpath))
        {
            try
            {
                Directory.CreateDirectory(outpath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CSV資料夾不存在且無法創建");
                return;
            }
        }
        else
        {
            foreach (var file in Directory.GetFiles(outpath))
            {
                var fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddMonths(-3))
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        if (logEvent.Type is EventType.Alarm or EventType.Alert)
        {
            var alarmfolder = $"{outpath}\\Alarm\\";
            await AlarmMethod(logEvent, alarmfolder, d);
        }
    }

    public async Task AddInfo(ProcessInfo info, string folderpath)
    {
        var outpath = folderpath.Trim().TrimEnd('\\');

        if (!Directory.Exists(outpath))
        {
            try
            {
                Directory.CreateDirectory(outpath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CSV資料夾不存在且無法創建");
                return;
            }
        }
        else
        {
            foreach (var file in Directory.GetFiles(outpath))
            {
                var fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddMonths(-3))
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        var datafolder   = $"{outpath}\\Data\\";
        var recordfolder = $"{outpath}\\Record\\";
        var d            = $"{DateTime.Now:yyyy-MM-dd}";
        await DataMethod(info, datafolder, d);
        await RecordMethod(info, recordfolder, d);
    }

    public async Task ExportRecipe(IEnumerable<PLC_Recipe> recipies, string folderpath)
    {
        var outpath = folderpath.Trim().TrimEnd('\\');

        if (!Directory.Exists(outpath))
        {
            try
            {
                Directory.CreateDirectory(outpath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CSV資料夾不存在且無法創建");
                return;
            }
        }

        var datapath = $"{outpath}\\Recipe.csv";
        var sb       = new StringBuilder();
        sb.AppendLine(RecipeTitles);

        foreach (var recipe in recipies)
        {
            var _temp = recipe.ToDictionary();
            sb.AppendLine(string.Join(",", _temp.Values));
        }

        try
        {
            using var outputFile = new StreamWriter(datapath, false, Encoding.UTF8);
            await outputFile.WriteAsync(sb.ToString());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Recipe CSV寫入失敗");
        }
    }

    public void UpdateLanguage(Language lng)
    {
        Language = lng;

        var type     = typeof(ProcessInfo);
        var recipe   = new PLC_Recipe().ToDictionary(Language);
        var record   = new RecordTemperatures().ToDic(Language);
        var logevent = new LogEvent().ToDictionary(Language);

        DataTitles = string.Join(",",
                                 new[]
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
                                 }.Concat(recipe.Keys));

        RecordTitles = $"{string.Join(",", record.Keys)}";
        RecipeTitles = $"{string.Join(",", recipe.Keys)}";
        AlarmTitles  = $"{string.Join(",", logevent.Keys)}";
    }
}