using System;
using System.Net;
using System.Threading.Tasks;
using GPGRC_MultiPLCs.Models;
using GPMVVM.Models;
using MongoDB.Driver;


//using MongodbConnect;
using Serilog;

namespace GPGRC_MultiPLCs.ViewModels;

/// <summary>提供身分驗證登入和系統設定</summary>
public class Authenticator_ViewModel : AuthenticatorModel
{

    /// <summary>系統參數</summary>
    public GlobalSettings Settings { get; }
    public RelayCommand BtnSaveCommand { get; }
    public RelayCommand BtnHeartBeatCommand { get; }
    public GlobalDialog_ViewModel DialogVM { get; }

    public event Func<Task>? BtnSaveEvent;
    public event Action<bool> BtnHeartBeatEvent;
    public Authenticator_ViewModel()
    {
        Settings = new GlobalSettings();
        Settings.Load(false);
        Settings.RegisterChanged();
        //Settings.UseHeart = false;

        BtnSaveCommand = new RelayCommand(e =>
        {
            try
            {
                SaveData();
                _ = BtnSaveEvent.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DB寫入失敗");
            }
        });

        BtnHeartBeatCommand = new RelayCommand(e =>
        {
            try
            {
                BtnHeartBeatEvent?.Invoke(Settings.UseHeart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MES心跳");
            }
        });
    }

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

    private void SaveData()
    {
        var db = new MongoClient("mongodb://localhost:27017").GetDatabase("GP_GRC");
        var tmp = new WebSetting(Settings.EquipmentID,Settings.iMESURL,Settings.CallCarrierID,Settings.OutCarrierID,Settings.NGCarrierID,Settings.AVGTime,Settings.TimeOut,Settings.UseHeart,Settings.HeartTime,Settings.HeartContent,Settings.HeartPort,Settings.HeartService);
        db.GetCollection<WebSetting>("WebSetting").DeleteMany(n => n.EquipmentID != "");
        db.GetCollection<WebSetting>("WebSetting").InsertOne(tmp);
    }
}