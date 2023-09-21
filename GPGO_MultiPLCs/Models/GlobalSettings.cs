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
    public string HeartContent
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string HeartService
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public int HeartPort
    {
        get => Get<int>();
        set => Set(value);
    }
    public string EquipmentID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string iMESURL
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string CallCarrierID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string OutCarrierID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }
    public string NGCarrierID
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public bool UseHeart
    {
        get => Get<bool>();
        set
        {
            Set(value);
            NotifyPropertyChanged();
        }
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

    public int ClearInputDelay
    {
        get => Get<int>();
        set
        {
            value = value switch
            {
                < 30 => 30,
                > 600 => 600,
                _ => value
            };

            Set(value);
        }
    }

    public int TimeOut
    {
        get => Get<int>();
        set
        {
            value = value switch
            {
                < 30 => 30,
                > 600 => 600,
                _ => value
            };

            Set(value);
        }
    }
    public int AVGTime
    {
        get => Get<int>();
        set
        {
            value = value switch
            {
                < 1 => 1,
                > 600 => 600,
                _ => value
            };

            Set(value);
        }
    }
    public int HeartTime
    {
        get => Get<int>();
        set
        {
            value = value switch
            {
                < 1 => 1,
                > 600 => 600,
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
        DataOutputPath = "C:\\GPOutput";
        RecipeImportPath = "C:\\GPOutput\\Recipe.csv";
        Lng = Language.TW;
        OvenCount = 1;
        RecordDelay = 1;
        ClearInputDelay = 60;
        TimeOut = 60;
        AVGTime = 1;
        HeartTime = 1;
        HeartContent = "SSEMP";
        HeartPort = 5001;
        HeartService = "127.0.0.1";
        EquipmentID = "GPGO";
        iMESURL = "iMESurl";
        CallCarrierID = "CallCarrierID";
        OutCarrierID = "OutCarrierID";
        NGCarrierID = "NGCarrierID";
    }
}