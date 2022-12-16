using System.Net;
using GPMVVM.Models;

namespace GPGO_MultiPLCs.Models;

/// <summary>PC程式參數</summary>
public class GlobalSettings : RecipeFileBase<GlobalSettings>
{
    //public string CodeReaderName
    //{
    //    get => Get<string>();
    //    set => Set(value);
    //}

    public string PLCIP
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public string DataOutputPath
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public string RecipeImportPath
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    /// <summary>介面語言</summary>
    public Language Lng
    {
        get => Get<Language>();
        set => Set(value);
    }

    public int OvenCount
    {
        get => Get<int>();
        set => Set(value);
    }

    public int RecordDelay
    {
        get => Get<int>();
        set
        {
            value = value switch
            {
                < 1 => 1,
                > 60 => 60,
                _ => value
            };

            Set(value);
        }
    }

    public GlobalSettings() : base("Settings")
    {
        //CodeReaderName   = "Symbol Bar Code Scanner";
        PLCIP = new IPAddress(new byte[]
                              {
                                  192,
                                  168,
                                  3,
                                  39
                              }).ToString();
        DataOutputPath   = "C:\\GPOutput";
        RecipeImportPath = "C:\\GPOutput\\Recipe.csv";
        Lng              = Language.TW;
        OvenCount        = 1;
        RecordDelay      = 1;
    }
}