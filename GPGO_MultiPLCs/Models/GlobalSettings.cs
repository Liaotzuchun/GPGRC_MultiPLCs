using System.Net;
using GPMVVM.Models;

namespace GPGO_MultiPLCs.Models;

/// <summary>PC程式參數</summary>
public class GlobalSettings : RecipeFileBase<GlobalSettings>
{
    public string PLCIP
    {
        get => Get<string>();
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

    public GlobalSettings() : base("Settings")
    {
        PLCIP = new IPAddress(new byte[] { 192, 168, 3, 39 }).ToString();
        Lng = Language.TW;
        OvenCount = 1;
    }
}