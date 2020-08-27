using GPGO_MultiPLCs.Models;
using GPMVVM.Models;
using System.IO;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>提供身分驗證登入和系統設定</summary>
    public class Authenticator_ViewModel : AuthenticatorModel
    {
        /// <summary>系統參數</summary>
        public GlobalSettings Settings { get; }

        /// <summary>設定資料輸入路徑</summary>
        public RelayCommand SetInputPath { get; }

        /// <summary>設定資料輸出路徑</summary>
        public RelayCommand SetPath { get; }

        public Authenticator_ViewModel()
        {
            Settings = new GlobalSettings();
            Settings.Load(false);
            Settings.RegisterChanged();

            SetInputPath = new RelayCommand(e =>
                                            {
                                                if (e is string str && Directory.Exists(str))
                                                {
                                                    Settings.DataInputPath = str;
                                                }
                                            });

            SetPath = new RelayCommand(e =>
                                       {
                                           if (e is string str && Directory.Exists(str))
                                           {
                                               Settings.DataOutputPath = str;
                                           }
                                       });
        }
    }
}