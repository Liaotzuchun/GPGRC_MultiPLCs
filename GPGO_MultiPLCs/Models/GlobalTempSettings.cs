using System;
using System.IO;
using System.Text;
using GPGO_MultiPLCs.Helpers;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>PC程式參數</summary>
    public class GlobalTempSettings : ObservableObject
    {
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

        [JsonIgnore]
        public string FilePath => "Settings.json";
        [JsonIgnore]
        public string FilePathBack => "Settings" + DateTime.Now.Ticks + ".back";

        public void Load()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    if (JsonConvert.DeserializeObject<GlobalTempSettings>(File.ReadAllText(FilePath, Encoding.Unicode)) is GlobalTempSettings val)
                    {
                        Lng = val.Lng;
                        DataOutputPath = val.DataOutputPath;
                    }
                    else
                    {
                        File.Move(FilePath, FilePathBack);
                        Save();
                    }
                }
                catch
                {
                    File.Move(FilePath, FilePathBack);
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
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(this, Formatting.Indented), Encoding.Unicode);
            }
            catch(Exception ex)
            {
                ErrorRecoder.RecordError(ex);
            }
        }

        public GlobalTempSettings()
        {
            Set("D:", nameof(DataOutputPath));
            Set(Language.TW, nameof(Lng));
        }
    }
}