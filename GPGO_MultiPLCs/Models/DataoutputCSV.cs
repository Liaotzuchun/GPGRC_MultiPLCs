using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PooledCollections;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Models;

public class DataoutputCSV
{
    private string   DataTitles;
    private string   RecordTitles;
    private string[] _RecipeTitles;
    private string   RecipeTitles;
    private string   AlarmTitles;
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
            sb.AppendLine(DataTitles);
        }

        var recipe = info.Recipe.ToDictionary(Language);
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

    public List<PLC_Recipe> ImportRecipe(string csvfilepath)
    {
        if (File.Exists(csvfilepath))
        {
            var results = FastCSV.ReadFile<PLC_Recipe>(csvfilepath,
                                                       ',',
                                                       (recipe, col) =>
                                                       {
                                                           try
                                                           {
                                                               recipe.RecipeName            = col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RecipeName))[0]];
                                                               recipe.NitrogenMode          = bool.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.NitrogenMode))[0]]);
                                                               recipe.OxygenContentSet      = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.OxygenContentSet))[0]]);
                                                               recipe.InflatingTime         = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.InflatingTime))[0]]);
                                                               recipe.DwellTemperature_1    = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTemperature_1))[0]]);
                                                               recipe.DwellTemperature_2    = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTemperature_2))[0]]);
                                                               recipe.DwellTemperature_3    = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTemperature_3))[0]]);
                                                               recipe.DwellTemperature_4    = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTemperature_4))[0]]);
                                                               recipe.DwellTemperature_5    = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTemperature_5))[0]]);
                                                               recipe.DwellTemperature_6    = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTemperature_6))[0]]);
                                                               recipe.DwellTemperature_7    = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTemperature_7))[0]]);
                                                               recipe.DwellTemperature_8    = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTemperature_8))[0]]);
                                                               recipe.DwellTime_1           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTime_1))[0]]);
                                                               recipe.DwellTime_2           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTime_2))[0]]);
                                                               recipe.DwellTime_3           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTime_3))[0]]);
                                                               recipe.DwellTime_4           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTime_4))[0]]);
                                                               recipe.DwellTime_5           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTime_5))[0]]);
                                                               recipe.DwellTime_6           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTime_6))[0]]);
                                                               recipe.DwellTime_7           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTime_7))[0]]);
                                                               recipe.DwellTime_8           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTime_8))[0]]);
                                                               recipe.DwellAlarm_1          = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellAlarm_1))[0]]);
                                                               recipe.DwellAlarm_2          = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellAlarm_2))[0]]);
                                                               recipe.DwellAlarm_3          = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellAlarm_3))[0]]);
                                                               recipe.DwellAlarm_4          = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellAlarm_4))[0]]);
                                                               recipe.DwellAlarm_5          = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellAlarm_5))[0]]);
                                                               recipe.DwellAlarm_6          = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellAlarm_6))[0]]);
                                                               recipe.DwellAlarm_7          = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellAlarm_7))[0]]);
                                                               recipe.DwellAlarm_8          = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellAlarm_8))[0]]);
                                                               recipe.CoolingTime           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.CoolingTime))[0]]);
                                                               recipe.CoolingTemperature    = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.CoolingTemperature))[0]]);
                                                               recipe.RampTime_1            = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampTime_1))[0]]);
                                                               recipe.RampTime_2            = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampTime_2))[0]]);
                                                               recipe.RampTime_3            = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampTime_3))[0]]);
                                                               recipe.RampTime_4            = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampTime_4))[0]]);
                                                               recipe.RampTime_5            = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampTime_5))[0]]);
                                                               recipe.RampTime_6            = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampTime_6))[0]]);
                                                               recipe.RampTime_7            = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampTime_7))[0]]);
                                                               recipe.RampTime_8            = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampTime_8))[0]]);
                                                               recipe.RampAlarm_1           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampAlarm_1))[0]]);
                                                               recipe.RampAlarm_2           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampAlarm_2))[0]]);
                                                               recipe.RampAlarm_3           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampAlarm_3))[0]]);
                                                               recipe.RampAlarm_4           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampAlarm_4))[0]]);
                                                               recipe.RampAlarm_5           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampAlarm_5))[0]]);
                                                               recipe.RampAlarm_6           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampAlarm_6))[0]]);
                                                               recipe.RampAlarm_7           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampAlarm_7))[0]]);
                                                               recipe.RampAlarm_8           = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.RampAlarm_8))[0]]);
                                                               recipe.TemperatureSetpoint_1 = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.TemperatureSetpoint_1))[0]]);
                                                               recipe.TemperatureSetpoint_2 = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.TemperatureSetpoint_2))[0]]);
                                                               recipe.TemperatureSetpoint_3 = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.TemperatureSetpoint_3))[0]]);
                                                               recipe.TemperatureSetpoint_4 = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.TemperatureSetpoint_4))[0]]);
                                                               recipe.TemperatureSetpoint_5 = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.TemperatureSetpoint_5))[0]]);
                                                               recipe.TemperatureSetpoint_6 = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.TemperatureSetpoint_6))[0]]);
                                                               recipe.TemperatureSetpoint_7 = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.TemperatureSetpoint_7))[0]]);
                                                               recipe.TemperatureSetpoint_8 = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.TemperatureSetpoint_8))[0]]);
                                                               recipe.DwellTimeOffset_1     = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTimeOffset_1))[0]]);
                                                               recipe.DwellTimeOffset_2     = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTimeOffset_2))[0]]);
                                                               recipe.DwellTimeOffset_3     = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTimeOffset_3))[0]]);
                                                               recipe.DwellTimeOffset_4     = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTimeOffset_4))[0]]);
                                                               recipe.DwellTimeOffset_5     = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTimeOffset_5))[0]]);
                                                               recipe.DwellTimeOffset_6     = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTimeOffset_6))[0]]);
                                                               recipe.DwellTimeOffset_7     = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTimeOffset_7))[0]]);
                                                               recipe.DwellTimeOffset_8     = double.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.DwellTimeOffset_8))[0]]);
                                                               recipe.SegmentCounts         = short.Parse(col[_RecipeTitles.FindIndexes(nameof(PLC_Recipe.SegmentCounts))[0]]);
                                                           }
                                                           catch (Exception ex)
                                                           {
                                                               Log.Error(ex, "ImportRecipe失敗");
                                                               return false;
                                                           }

                                                           return true;
                                                       });

            return results;
        }

        return new List<PLC_Recipe>();
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

        RecordTitles  = $"{string.Join(",", record.Keys)}";
        _RecipeTitles = recipe.Keys.ToArray();
        RecipeTitles  = $"{string.Join(",", recipe.Keys)}";
        AlarmTitles   = $"{string.Join(",", logevent.Keys)}";
    }

    public DataoutputCSV()
    {
        UpdateLanguage(Language.TW);
    }
}