using System;
using System.IO;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>PC程式參數</summary>
    public class GlobalTempSettings : ObservableObject
    {
        private readonly string FilePath = "Settings.json";

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

        public void Load()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    if (FilePath.ReadFromJsonFile<GlobalTempSettings>() is GlobalTempSettings val)
                    {
                        Lng = val.Lng;
                        DataOutputPath = val.DataOutputPath;
                    }
                    else
                    {
                        File.Move(FilePath, "Settings" + DateTime.Now.Ticks + ".back");
                        Save();
                    }
                }
                catch
                {
                    File.Move(FilePath, "Settings" + DateTime.Now.Ticks + ".back");
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
            try
            {
                this.WriteToJsonFile(FilePath);
            }
            catch (Exception ex)
            {
                ex.RecordError();
            }
        }

        public GlobalTempSettings()
        {
            Set("D:", nameof(DataOutputPath));
            Set(Language.TW, nameof(Lng));
        }
    }
}