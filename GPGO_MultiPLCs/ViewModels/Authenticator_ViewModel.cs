using System.Net;
using GPGO_MultiPLCs.Models;
using GPMVVM.Models;

namespace GPGO_MultiPLCs.ViewModels;

/// <summary>提供身分驗證登入和系統設定</summary>
public class Authenticator_ViewModel : AuthenticatorModel
{
    public string IPString
    {
        get => Settings.PLCIP;
        set
        {
            if (!string.IsNullOrEmpty(value) && IPAddress.TryParse(value, out var ip))
            {
                Settings.PLCIP = ip.ToString();
                NotifyPropertyChanged();
            }
        }
    }

    /// <summary>系統參數</summary>
    public GlobalSettings Settings { get; }

    public Authenticator_ViewModel()
    {
        Settings = new GlobalSettings();
        Settings.Load(false);
        Settings.RegisterChanged();
        Settings.UseHeart = false;
    }
}