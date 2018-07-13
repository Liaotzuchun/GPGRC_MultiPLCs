using System;
using System.IO;
using System.Text;
using GPGO_MultiPLCs.Helpers;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.Models
{
    public class GlobalTempSettings : ViewModelBase
    {
        public enum Language
        {
            TW,
            CHS,
            EN
        }

        private int _Lng = (int)Language.TW;

        public int Lng
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