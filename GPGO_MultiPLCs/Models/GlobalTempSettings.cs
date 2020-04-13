using Serilog;
using System;
using System.IO;
using GPMVVM.Helpers;
using GPMVVM.Models;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>PC程式參數</summary>
    public class GlobalTempSettings : ObservableObject
    {
        private const string FilePath = "Settings.json";

        /// <summary>前端資料輸入位置</summary>
        public string DataInputPath
        {
            get => Get<string>();
            set
            {
                Set(value);

                Save();
            }
        }

        /// <summary>上傳資料輸出位置</summary>
        public string DataOutputPath
        {
            get => Get<string>();
            set
            {
                Set(value);

                Save();
            }
        }

        /// <summary>介面語言</summary>
        public Language Lng
        {
            get => Get<Language>();
            set
            {
                Set(value);

                Save();
            }
        }

        public int OvenCount
        {
            get => Get<int>();
            set
            {
                Set(value);

                Save();
            }
        }

        public void Load()
        {
            if (File.Exists(FilePath))
            {
                if (FilePath.ReadFromJsonFile<GlobalTempSettings>() is GlobalTempSettings val)
                {
                    Set(val.DataInputPath, nameof(DataInputPath));
                    Set(val.DataOutputPath, nameof(DataOutputPath));
                    Set(val.Lng, nameof(Lng));
                    Set(val.OvenCount, nameof(OvenCount));
                }
                else
                {
                    try
                    {
                        File.Move(FilePath, $"Settings{DateTime.Now.Ticks}.back");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "");
                    }

                    Save();
                }
            }
            else
            {
                Save();
            }
        }

        public void Save()
        {
            this.WriteToJsonFile(FilePath);
        }

        public GlobalTempSettings()
        {
            Set("D:\\Intput", nameof(DataInputPath));
            Set("D:", nameof(DataOutputPath));
            Set(Language.TW, nameof(Lng));
            Set(20, nameof(OvenCount));
        }
    }
}