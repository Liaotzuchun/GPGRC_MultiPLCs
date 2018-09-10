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
        [EN_Name("Asset Number")]
        [CHT_Name("財產編號")]
        [CHS_Name("财产编号")]
        public string AssetNumber
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>結束時間</summary>
        [EN_Name("Closing Time")]
        [CHT_Name("結束時間")]
        [CHS_Name("结束时间")]
        public DateTime EndTime
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        /// <summary>事件紀錄</summary>
        [EN_Name("Events")]
        [CHT_Name("事件紀錄")]
        [CHS_Name("事件纪录")]
        public ObservableConcurrentCollection<RecordEvent> EventList
        {
            get => Get<ObservableConcurrentCollection<RecordEvent>>();
            set => Set(value);
        }

        /// <summary>加熱時間(升溫至目標溫度)</summary>
        [EN_Name("Heating Time")]
        [CHT_Name("加熱時間")]
        [CHS_Name("加热时间")]
        public int HeatingTime
        {
            get => Get<int>();
            set => Set(value);
        }

        /// <summary>機台編號</summary>
        [EN_Name("Device")]
        [CHT_Name("設備編號")]
        [CHS_Name("设备编号")]
        public string MachineCode
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>操作人員ID</summary>
        [EN_Name("Operator")]
        [CHT_Name("操作員")]
        [CHS_Name("操作员")]
        public string OperatorID
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>配方名</summary>
        [EN_Name("Recipe")]
        [CHT_Name("配方")]
        [CHS_Name("配方")]
        public string RecipeName
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>紀錄溫度</summary>
        [EN_Name("Temperatures")]
        [CHT_Name("溫度紀錄")]
        [CHS_Name("温度纪录")]
        public ObservableConcurrentCollection<RecordTemperatures> RecordTemperatures
        {
            get => Get<ObservableConcurrentCollection<RecordTemperatures>>();
            set => Set(value);
        }

        /// <summary>開始時間</summary>
        [EN_Name("Starting Time")]
        [CHT_Name("開始時間")]
        [CHS_Name("开始时间")]
        public DateTime StartTime
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        /// <summary>目標溫度</summary>
        [EN_Name("Target Temperature")]
        [CHT_Name("目標溫度")]
        [CHS_Name("目标温度")]
        public double TargetOvenTemperature
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>總烘烤時間</summary>
        [EN_Name("Total Heating Time")]
        [CHT_Name("總烘烤時間")]
        [CHS_Name("总烘烤时间")]
        public int TotalHeatingTime
        {
            get => Get<int>();
            set => Set(value);
        }

        /// <summary>台車編號</summary>
        [EN_Name("Trolley")]
        [CHT_Name("台車")]
        [CHS_Name("台车")]
        public string TrolleyCode
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>恆溫時間</summary>
        [EN_Name("Warming Time")]
        [CHT_Name("恆溫時間")]
        [CHS_Name("恒温时间")]
        public int WarmingTime
        {
            get => Get<int>();
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
            EventList = new ObservableConcurrentCollection<RecordEvent>();
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
            var strs = code.Split(',');
            OrderCode = strs[0];
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
        [EN_Name("Quantity")]
        [CHT_Name("數量")]
        [CHS_Name("数量")]
        public int ProcessCount => PanelCodes.Count;

        /// <summary>條碼類型</summary>
        [EN_Name("Code Type")]
        [CHT_Name("條碼類型")]
        [CHS_Name("条码类型")]
        public CodeType CodeType { get; set; }

        /// <summary>是否為首件</summary>
        [EN_Name("First Article")]
        [CHT_Name("首件")]
        [CHS_Name("首件")]
        public bool FirstPanel { get; set; }

        /// <summary>工單號</summary>
        [EN_Name("Order")]
        [CHT_Name("工單")]
        [CHS_Name("工单")]
        public string OrderCode { get; set; }

        public List<string> PanelCodes { get; set; } = new List<string>();

        /// <summary>製造序</summary>
        [EN_Name("SN")]
        [CHT_Name("序號")]
        [CHS_Name("序号")]
        public int ProcessNumber { get; set; }

        /// <summary>正反面</summary>
        [EN_Name("Side")]
        [CHT_Name("正反面")]
        [CHS_Name("正反面")]
        public string Side { get; set; }

        public string AlarmListString()
        {
            return string.Join(",", EventList.Where(x => x.Type == EventType.Alarm).Select(x => x.Description));
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
            stb.AppendLine(ProcessNumber.ToString("000"));
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
            stb.AppendLine(ProcessCount.ToString());
            stb.Append("General10=");
            stb.AppendLine(index.ToString());
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
            stb.AppendLine(TargetOvenTemperature.ToString("0"));
            stb.Append("Machine3=");
            stb.AppendLine(WarmingTime.ToString());
            stb.Append("Machine4=");
            stb.AppendLine(HeatingTime.ToString());
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
        [EN_Name("Recorded")]
        [CHT_Name("紀錄時間")]
        [CHS_Name("纪录时间")]
        public DateTime AddedTime { get; set; }

        /// <summary>PLC站號</summary>
        [EN_Name("Oven No.")]
        [CHT_Name("烤箱序號")]
        [CHS_Name("烤箱序号")]
        public int StationNumber { get; set; }

        #endregion
    }
}