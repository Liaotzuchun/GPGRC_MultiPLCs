using GPMVVM.Models;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>PC程式參數</summary>
    public class GlobalTempSettings : RecipeFileBase<GlobalTempSettings>
    {
        /// <summary>前端資料輸入位置</summary>
        public string DataInputPath
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>上傳資料輸出位置</summary>
        public string DataOutputPath
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

        public GlobalTempSettings() : base("Settings")
        {
            DataInputPath = "D:\\Intput";
            DataOutputPath = "D:\\";
            Lng = Language.TW;
            OvenCount = 20;
        }
    }
}