using System;
using System.IO;
using System.Text;
using GPGO_MultiPLCs.Helpers;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>
    /// PC程式參數
    /// </summary>
    public class GlobalTempSettings : ViewModelBase
    {
        private string _DataOutputPath = "D:\\";
        private Language _Lng = Language.TW;

        /// <summary>
        /// 上傳資料輸出位置
        /// </summary>
        public string DataOutputPath
        {
            get => _DataOutputPath;
            set
            {
                _DataOutputPath = value;
                NotifyPropertyChanged();

                Save();
            }
        }

        /// <summary>
        /// 介面語言
        /// </summary>
        public Language Lng
        {
            get => _Lng;
            set
            {
                _Lng = value;
                NotifyPropertyChanged();

                Save();
            }
        }

        public void Load()
        {
            if (File.Exists("Settings.json"))
            {
                try
                {
                    if (JsonConvert.DeserializeObject<GlobalTempSettings>(File.ReadAllText("Settings.json", Encoding.Unicode)) is GlobalTempSettings val)
                    {
                        Lng = val.Lng;
                        DataOutputPath = val.DataOutputPath;
                    }
                    else
                    {
                        File.Move("Settings.json", "Settings" + DateTime.Now.Ticks + ".back");
                        Save();
                    }
                }
                catch
                {
                    File.Move("Settings.json", "Settings" + DateTime.Now.Ticks + ".back");
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
                File.WriteAllText("Settings.json", JsonConvert.SerializeObject(this, Formatting.Indented), Encoding.Unicode);
            }
            catch
            {
            }
        }
    }
}