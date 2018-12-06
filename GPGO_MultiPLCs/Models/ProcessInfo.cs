using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace GPGO_MultiPLCs.Models
{
    public enum CodeType
    {
        Panel,
        SubPanel,
        JobNo
    }

    [BsonIgnoreExtraElements]
    public class BaseInfo : ObservableObject
    {
        /// <summary>財產編號</summary>
        [LanguageTranslator("Asset Number", "財產編號", "财产编号")]
        public string AssetNumber
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>結束時間</summary>
        [LanguageTranslator("Closing Time", "結束時間", "结束时间")]
        public DateTime EndTime
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        /// <summary>事件紀錄</summary>
        [LanguageTranslator("Events", "事件紀錄", "事件纪录")]
        public ObservableConcurrentCollection<LogEvent> EventList
        {
            get => Get<ObservableConcurrentCollection<LogEvent>>();
            set => Set(value);
        }

        /// <summary>加熱時間(升溫至目標溫度)</summary>
        [LanguageTranslator("Heating Time", "加熱時間", "加热时间")]
        public List<int> HeatingTimes
        {
            get => Get<List<int>>();
            set => Set(value);
        }

        /// <summary>機台編號</summary>
        [LanguageTranslator("Device", "設備編號", "设备编号")]
        public string MachineCode
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>操作人員ID</summary>
        [LanguageTranslator("Operator", "操作員", "操作员")]
        public string OperatorID
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>配方名</summary>
        [LanguageTranslator("Recipe", "配方", "配方")]
        public string RecipeName
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>紀錄溫度</summary>
        [LanguageTranslator("Temperatures", "溫度紀錄", "温度纪录")]
        public ObservableConcurrentCollection<RecordTemperatures> RecordTemperatures
        {
            get => Get<ObservableConcurrentCollection<RecordTemperatures>>();
            set => Set(value);
        }

        /// <summary>開始時間</summary>
        [LanguageTranslator("Starting Time", "開始時間", "开始时间")]
        public DateTime StartTime
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        /// <summary>目標溫度</summary>
        [LanguageTranslator("Target Temperature", "目標溫度", "目标温度")]
        public List<double> TargetOvenTemperatures
        {
            get => Get<List<double>>();
            set => Set(value);
        }

        /// <summary>總烘烤時間</summary>
        [LanguageTranslator("Total Heating Time", "總烘烤時間", "总烘烤时间")]
        public int TotalHeatingTime
        {
            get => Get<int>();
            set => Set(value);
        }

        /// <summary>台車編號</summary>
        [LanguageTranslator("Trolley", "台車", "台车")]
        public string TrolleyCode
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>恆溫時間</summary>
        [LanguageTranslator("Warming Time", "恆溫時間", "恒温时间")]
        public List<int> WarmingTimes
        {
            get => Get<List<int>>();
            set => Set(value);
        }

        /// <summary>初始化清除資訊</summary>
        public void Clear()
        {
            EventList.Clear();
            RecordTemperatures.Clear();

            StartTime = new DateTime();
            EndTime = new DateTime();
            OperatorID = "";
            TrolleyCode = "";
        }

        public BaseInfo()
        {
            EventList = new ObservableConcurrentCollection<LogEvent>();
            RecordTemperatures = new ObservableConcurrentCollection<RecordTemperatures>();
        }
    }

    /// <summary>客戶製程資訊</summary>
    public class ProductInfo
    {
        public CodeType CodeType { get; set; } = CodeType.Panel;
        public bool FirstPanel { get; set; } = false;
        public string OrderCode { get; set; }
        public List<string> PanelCodes { get; set; } = new List<string>();
        public int ProcessNumber { get; set; }
        public string Side { get; set; } = "A";

        /// <summary></summary>
        /// <param name="code">工單條碼</param>
        public ProductInfo(string code)
        {
            var strs = code.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            OrderCode = strs.Length > 0 ? strs[0] : "";
            ProcessNumber = strs.Length > 1 ? int.TryParse(strs[1], out var num) ? num : 0 : 0;
        }

        public ProductInfo(string orderCode, int processNumber)
        {
            OrderCode = orderCode;
            ProcessNumber = processNumber;
        }
    }

    [BsonIgnoreExtraElements]
    public class ProcessInfo : BaseInfo, ILogData
    {
        /// <summary>單一製造序材料數量</summary>
        [LanguageTranslator("Quantity", "數量", "数量")]
        public int ProcessCount => PanelCodes.Count;

        /// <summary>條碼類型</summary>
        [LanguageTranslator("Code Type", "條碼類型", "条码类型")]
        public CodeType CodeType { get; set; }

        /// <summary>是否為首件</summary>
        [LanguageTranslator("First Article", "首件", "首件")]
        public bool FirstPanel { get; set; } = false;

        /// <summary>工單號</summary>
        [LanguageTranslator("Order", "工單", "工单")]
        public string OrderCode { get; set; }

        public List<string> PanelCodes { get; set; } = new List<string>();

        /// <summary>製造序</summary>
        [LanguageTranslator("SN", "序號", "序号")]
        public int ProcessNumber { get; set; }

        /// <summary>正反面</summary>
        [LanguageTranslator("Side", "面", "面")]
        public string Side { get; set; } = "A";

        public string AlarmListString()
        {
            return string.Join(",", EventList.Where(x => x.Type == EventType.Alarm).Select(x => x.TagCode));
        }

        /// <summary>匯出成Dictionary</summary>
        /// <param name="lng">語系</param>
        /// <returns></returns>
        public Dictionary<string, object> ToDic(Language lng)
        {
            return new Dictionary<string, object>
                   {
                       { GetType().GetProperty(nameof(AddedTime)).GetName(lng), AddedTime },
                       { GetType().GetProperty(nameof(StationNumber)).GetName(lng), StationNumber },
                       { GetType().GetProperty(nameof(RecipeName)).GetName(lng), RecipeName },
                       { GetType().GetProperty(nameof(MachineCode)).GetName(lng), MachineCode },
                       { GetType().GetProperty(nameof(OrderCode)).GetName(lng), OrderCode },
                       { GetType().GetProperty(nameof(OperatorID)).GetName(lng), OperatorID },
                       { GetType().GetProperty(nameof(TrolleyCode)).GetName(lng), TrolleyCode },
                       { GetType().GetProperty(nameof(ProcessCount)).GetName(lng), ProcessCount },
                       { GetType().GetProperty(nameof(Side)).GetName(lng), Side },
                       { GetType().GetProperty(nameof(StartTime)).GetName(lng), StartTime },
                       { GetType().GetProperty(nameof(EndTime)).GetName(lng), EndTime },
                       { GetType().GetProperty(nameof(RecordTemperatures)).GetName(lng), "@" }
                   };
        }

        /// <summary>輸出客戶指定之文字字串</summary>
        /// <returns></returns>
        public string ToString(int index)
        {
            var stb = new StringBuilder();
            stb.Append("General1=");
            stb.AppendLine(OrderCode);
            stb.Append("General2=");
            stb.AppendLine(ProcessNumber.ToString("0000"));
            stb.Append("General3=");
            stb.AppendLine(StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            stb.Append("General4=");
            stb.AppendLine(EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            stb.Append("General5=");
            stb.AppendLine(MachineCode);
            stb.Append("General6=");
            stb.AppendLine(CodeType.ToString());
            stb.Append("General7=");
            stb.AppendLine(PanelCodes.Count > index ? PanelCodes[index] : "");
            stb.Append("General8=");
            stb.AppendLine(RecipeName);
            stb.Append("General9=");
            stb.AppendLine((index + 1).ToString());
            stb.Append("General10=");
            stb.AppendLine(ProcessCount.ToString());
            stb.Append("General11=");
            stb.AppendLine(OperatorID);
            stb.Append("General12=");
            stb.AppendLine("");
            stb.Append("General13=");
            stb.AppendLine(Side);
            stb.Append("General14=");
            stb.AppendLine("");
            stb.Append("General15=");
            stb.AppendLine(FirstPanel ? "Y" : "N");
            stb.Append("Machine1=");
            stb.AppendLine(TrolleyCode);
            stb.Append("Machine2=");
            stb.AppendLine(string.Join(",", TargetOvenTemperatures.Select(x => ((int)Math.Round(x, MidpointRounding.AwayFromZero)).ToString())));
            stb.Append("Machine3=");
            stb.AppendLine(string.Join(",", WarmingTimes.Select(x => x.ToString())));
            stb.Append("Machine4=");
            stb.AppendLine(string.Join(",", HeatingTimes.Select(x => x.ToString())));
            stb.Append("Machine5=");
            stb.AppendLine(TotalHeatingTime.ToString());
            stb.Append("Machine6=");
            stb.AppendLine(AlarmListString());

            return stb.ToString();
        }

        public ProcessInfo()
        {
        }

        public ProcessInfo(BaseInfo baseInfo, ProductInfo productInfo)
        {
            baseInfo.CopyTo(this);
            productInfo.CopyTo(this);
        }

        #region 此區由TraceabilityView_ViewModel新增至資料庫時填入

        /// <summary>新增至資料庫的時間</summary>
        [BsonId]
        [LanguageTranslator("Recorded", "紀錄時間", "纪录时间")]
        public DateTime AddedTime { get; set; }

        /// <summary>PLC站號</summary>
        [LanguageTranslator("Oven No.", "烤箱序號", "烤箱序号")]
        public int StationNumber { get; set; }

        #endregion
    }
}