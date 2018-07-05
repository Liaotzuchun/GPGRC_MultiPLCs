using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs
{
    public class Languages : ViewModelBase
    {
        public enum Language
        {
            TW,
            CHS,
            EN
        }

        private Language _LNG = Language.EN;

        public string 生產頁面 => _LNG == Language.EN ? "Main Page" : _LNG == Language.CHS ? "生产页面" : "生產頁面";
        public string 配方管理 => _LNG == Language.EN ? "Recipe Manager" : _LNG == Language.CHS ? "配方管理" : "配方管理";
        public string 生產履歷 => _LNG == Language.EN ? "Production Traceability" : _LNG == Language.CHS ? "生產履歷" : "生產履歷";
        public string 設備碼 => _LNG == Language.EN ? "Device" : _LNG == Language.CHS ? "设备码" : "設備碼";
        public string 配方 => _LNG == Language.EN ? "Recipe" : _LNG == Language.CHS ? "配方" : "配方";
        public string 投入生產 => _LNG == Language.EN ? "GO into OP." : _LNG == Language.CHS ? "投入生产" : "投入生產";
        public string 取消 => _LNG == Language.EN ? "Cancel" : _LNG == Language.CHS ? "取消" : "取消";
        public string 操作員 => _LNG == Language.EN ? "OP." : _LNG == Language.CHS ? "操作员" : "操作員";
        public string 製造序 => _LNG == Language.EN ? "Serial" : _LNG == Language.CHS ? "制造序" : "製造序";
        public string 台車 => _LNG == Language.EN ? "Trolley" : _LNG == Language.CHS ? "台车" : "台車";
        public string 張數 => _LNG == Language.EN ? "Count" : _LNG == Language.CHS ? "张数" : "張數";
        public string 工單 => _LNG == Language.EN ? "Order" : _LNG == Language.CHS ? "工单" : "工單";
        public string 狀態 => _LNG == Language.EN ? "Status" : _LNG == Language.CHS ? "状态" : "狀態";
        public string 準備中 => _LNG == Language.EN ? "Ready to GO" : _LNG == Language.CHS ? "准备中" : "準備中 ";
        public string 升溫中 => _LNG == Language.EN ? "Heating" : _LNG == Language.CHS ? "升温中" : "升溫中 ";
        public string 恆溫中 => _LNG == Language.EN ? "Warming" : _LNG == Language.CHS ? "恒温中" : "恆溫中 ";
        public string 降溫中 => _LNG == Language.EN ? "Cooling" : _LNG == Language.CHS ? "降温中" : "降溫中 ";

        public Language LNG
        {
            get => _LNG;
            set
            {
                _LNG = value;
                NotifyPropertyChanged();
            }
        }
    }
}