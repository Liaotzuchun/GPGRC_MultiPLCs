using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PooledCollections;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Models;

public class DataoutputCSV
{
    private string   Titles;
    private Language Language = Language.TW;

    private async Task DataMethod(ProcessInfo info, string folder, string filename)
    {
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

        var sb       = new StringBuilder();
        var datapath = $"{folder}{filename}.csv";

        if (!File.Exists(datapath))
        {
            sb.AppendLine(Titles);
        }

        var recipe = info.Recipe.ToDictionary(Language);
        foreach (var product in info.Products)
        {
            var vals = new[]
                       {
                           info.AddedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                           info.StartTime.ToString("HH:mm:ss"),
                           info.EndTime.ToString("HH:mm:ss"),
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

        using var outputFile = new StreamWriter(datapath, true, Encoding.UTF8);
        await outputFile.WriteAsync(sb.ToString());
    }

    private async Task RecordMethod(ProcessInfo info, string folder, string filename)
    {
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

        var sb       = new StringBuilder();
        var datapath = $"{folder}{filename}.csv";

        if (!File.Exists(datapath))
        {
            sb.AppendLine(Titles);
        }

        //todo
    }

    private async Task EventMethod(ProcessInfo info, string folder, string filename)
    {
        if (!Directory.Exists(folder))
        {
            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Event資料夾不存在且無法創建");
                return;
            }
        }

        var sb       = new StringBuilder();
        var datapath = $"{folder}{filename}.csv";

        if (!File.Exists(datapath))
        {
            sb.AppendLine(Titles);
        }

        //todo
    }

    private async Task AlarmMethod(ProcessInfo info, string folder, string filename)
    {
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

        var sb       = new StringBuilder();
        var datapath = $"{folder}{filename}.csv";

        if (!File.Exists(datapath))
        {
            sb.AppendLine(Titles);
        }

        //todo
    }

    public async Task AddInfo(ProcessInfo info, string folderpath)
    {
        var outpath      = folderpath.Trim().TrimEnd('\\');
        var datafolder   = $"{outpath}\\Data\\";
        var recordfolder = $"{outpath}\\Record\\";
        var eventfolder  = $"{outpath}\\Event\\";
        var alarmfolder  = $"{outpath}\\Alarm\\";

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

        var d = $"{DateTime.Now:yyyy-MM-dd}";
        await DataMethod(info, datafolder, d);
        await RecordMethod(info, recordfolder, d);
        await EventMethod(info, eventfolder, d);
        await AlarmMethod(info, alarmfolder, d);
    }

    public void UpdateLanguage(Language lng)
    {
        Language = lng;

        var type   = typeof(ProcessInfo);
        var recipe = new PLC_Recipe().ToDictionary(Language);

        Titles = string.Join(",",
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
    }

    public DataoutputCSV()
    {
        UpdateLanguage(Language.TW);
    }
}