using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class ProcessInfo : ViewModelBase
    {
        private CodeType _CodeType = CodeType.Panel;
        private DateTime _EndTime;
        private ObservableConcurrentCollection<RecordEvent> _EventList = new ObservableConcurrentCollection<RecordEvent>();
        private bool _FirstPanel;
        private int _HeatingTime;
        private string _MachineCode = "";
        private string _OperatorID = "";
        private string _OrderCode = "";
        private int _OrderCount;
        private int _ProcessCount;
        private int _ProcessNumber;
        private string _ProduceCode = "";
        private string _RecipeName = "";
        private ObservableConcurrentCollection<RecordTemperatures> _RecordTemperatures = new ObservableConcurrentCollection<RecordTemperatures>();
        private string _Side = "";
        private DateTime _StartTime;
        private double _TargetOvenTemperature;
        private int _TotalHeatingTime;
        private string _TrolleyCode = "";
        private int _WarmingTime;

        /// <summary>條碼類型</summary>
        [EN_Name("Code Type")]
        [CHT_Name("條碼類型")]
        [CHS_Name("条码类型")]
        public CodeType CodeType
        {
            get => _CodeType;
            set
            {
                _CodeType = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>結束時間</summary>
        [EN_Name("Closing Time")]
        [CHT_Name("結束時間")]
        [CHS_Name("结束时间")]
        public DateTime EndTime
        {
            get => _EndTime;
            set
            {
                _EndTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>事件紀錄</summary>
        [EN_Name("Events")]
        [CHT_Name("事件紀錄")]
        [CHS_Name("事件纪录")]
        public ObservableConcurrentCollection<RecordEvent> EventList
        {
            get => _EventList;
            set
            {
                _EventList = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>是否為首件</summary>
        [EN_Name("First Article")]
        [CHT_Name("首件")]
        [CHS_Name("首件")]
        public bool FirstPanel
        {
            get => _FirstPanel;
            set
            {
                _FirstPanel = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>加熱時間(升溫至目標溫度)</summary>
        [EN_Name("Heating Time")]
        [CHT_Name("加熱時間")]
        [CHS_Name("加热时间")]
        public int HeatingTime
        {
            get => _HeatingTime;
            set
            {
                _HeatingTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>機台編號</summary>
        [EN_Name("Device")]
        [CHT_Name("設備編號")]
        [CHS_Name("设备编号")]
        public string MachineCode
        {
            get => _MachineCode;
            set
            {
                _MachineCode = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>操作人員ID</summary>
        [EN_Name("Operator")]
        [CHT_Name("操作員")]
        [CHS_Name("操作员")]
        public string OperatorID
        {
            get => _OperatorID;
            set
            {
                _OperatorID = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>工單號</summary>
        [EN_Name("Order")]
        [CHT_Name("工單")]
        [CHS_Name("工单")]
        public string OrderCode
        {
            get => _OrderCode;
            set
            {
                _OrderCode = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>工單材料總量</summary>
        [EN_Name("Order Quantity")]
        [CHT_Name("工單總量")]
        [CHS_Name("工单总量")]
        public int OrderCount
        {
            get => _OrderCount;
            set
            {
                _OrderCount = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>單一製造序材料數量</summary>
        [EN_Name("Quantity")]
        [CHT_Name("數量")]
        [CHS_Name("数量")]
        public int ProcessCount
        {
            get => _ProcessCount;
            set
            {
                _ProcessCount = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>製造序</summary>
        [EN_Name("SN")]
        [CHT_Name("序號")]
        [CHS_Name("序号")]
        public int ProcessNumber
        {
            get => _ProcessNumber;
            set
            {
                _ProcessNumber = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>條碼 = OrderCode + ProcessNumber</summary>
        [EN_Name("Product ID")]
        [CHT_Name("產品編號")]
        [CHS_Name("产品编号")]
        public string ProduceCode
        {
            get => _ProduceCode;
            set
            {
                _ProduceCode = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>配方名</summary>
        [EN_Name("Recipe")]
        [CHT_Name("配方")]
        [CHS_Name("配方")]
        public string RecipeName
        {
            get => _RecipeName;
            set
            {
                _RecipeName = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>紀錄溫度</summary>
        [EN_Name("Temperatures")]
        [CHT_Name("溫度紀錄")]
        [CHS_Name("温度纪录")]
        public ObservableConcurrentCollection<RecordTemperatures> RecordTemperatures
        {
            get => _RecordTemperatures;
            set
            {
                _RecordTemperatures = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>正反面</summary>
        [EN_Name("Side")]
        [CHT_Name("正反面")]
        [CHS_Name("正反面")]
        public string Side
        {
            get => _Side;
            set
            {
                _Side = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>開始時間</summary>
        [EN_Name("Starting Time")]
        [CHT_Name("開始時間")]
        [CHS_Name("开始时间")]
        public DateTime StartTime
        {
            get => _StartTime;
            set
            {
                _StartTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>目標溫度</summary>
        [EN_Name("Target Temperature")]
        [CHT_Name("目標溫度")]
        [CHS_Name("目标温度")]
        public double TargetOvenTemperature
        {
            get => _TargetOvenTemperature;
            set
            {
                _TargetOvenTemperature = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>總烘烤時間</summary>
        [EN_Name("Total Heating Time")]
        [CHT_Name("總烘烤時間")]
        [CHS_Name("总烘烤时间")]
        public int TotalHeatingTime
        {
            get => _TotalHeatingTime;
            set
            {
                _TotalHeatingTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>台車編號</summary>
        [EN_Name("Trolley")]
        [CHT_Name("台車")]
        [CHS_Name("台车")]
        public string TrolleyCode
        {
            get => _TrolleyCode;
            set
            {
                _TrolleyCode = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>恆溫時間</summary>
        [EN_Name("Warming Time")]
        [CHT_Name("恆溫時間")]
        [CHS_Name("恒温时间")]
        public int WarmingTime
        {
            get => _WarmingTime;
            set
            {
                _WarmingTime = value;
                NotifyPropertyChanged();
            }
        }

        private static string GetName(PropertyInfo info, Language lng)
        {
            switch (lng)
            {
                case Language.EN:

                    return info.IsDefined(typeof(EN_Name), false) ? info.GetCustomAttributes(typeof(EN_Name), false).First().ToString() : info.Name;
                case Language.TW:

                    return info.IsDefined(typeof(CHT_Name), false) ? info.GetCustomAttributes(typeof(CHT_Name), false).First().ToString() : info.Name;
                case Language.CHS:

                    return info.IsDefined(typeof(CHS_Name), false) ? info.GetCustomAttributes(typeof(CHS_Name), false).First().ToString() : info.Name;
                default:

                    return info.Name;
            }
        }

        /// <summary>初始化清除資訊</summary>
        public void Clear()
        {
            StartTime = new DateTime();
            EndTime = new DateTime();
            OperatorID = "";
            OrderCode = "";
            OrderCount = 0;
            ProcessCount = 0;
            ProcessNumber = 0;
            ProduceCode = "";
            TrolleyCode = "";
        }

        /// <summary>匯出成Dictionary</summary>
        /// <param name="lng">語系</param>
        /// <returns></returns>
        public Dictionary<string, object> ToDic(Language lng)
        {
            return new Dictionary<string, object>
                   {
                       { GetName(GetType().GetProperty(nameof(AddedTime)), lng), AddedTime },
                       { GetName(GetType().GetProperty(nameof(StationNumber)), lng), StationNumber },
                       { GetName(GetType().GetProperty(nameof(RecipeName)), lng), RecipeName },
                       { GetName(GetType().GetProperty(nameof(MachineCode)), lng), MachineCode },
                       { GetName(GetType().GetProperty(nameof(OrderCode)), lng), OrderCode },
                       { GetName(GetType().GetProperty(nameof(OperatorID)), lng), OperatorID },
                       { GetName(GetType().GetProperty(nameof(TrolleyCode)), lng), TrolleyCode },
                       { GetName(GetType().GetProperty(nameof(ProcessCount)), lng), ProcessCount },
                       { GetName(GetType().GetProperty(nameof(Side)), lng), Side },
                       { GetName(GetType().GetProperty(nameof(StartTime)), lng), StartTime },
                       { GetName(GetType().GetProperty(nameof(EndTime)), lng), EndTime },
                       { GetName(GetType().GetProperty(nameof(RecordTemperatures)), lng), "@" }
                   };
        }

        /// <summary>輸出客戶指定之文字字串</summary>
        /// <returns></returns>
        public new string ToString()
        {
            var stb = new StringBuilder();
            stb.Append("General1=");
            stb.AppendLine(_OrderCode);
            stb.Append("General2=");
            stb.AppendLine(_ProcessNumber.ToString("000"));
            stb.Append("General3=");
            stb.AppendLine(_StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            stb.Append("General4=");
            stb.AppendLine(_EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            stb.Append("General5=");
            stb.AppendLine(_MachineCode);
            stb.Append("General6=");
            stb.AppendLine(_CodeType.ToString());
            stb.Append("General7=");
            stb.AppendLine(_ProduceCode);
            stb.Append("General8=");
            stb.AppendLine(_RecipeName);
            stb.Append("General9=");
            stb.AppendLine(_ProcessCount.ToString());
            stb.Append("General10=");
            stb.AppendLine(_OrderCount.ToString());
            stb.Append("General11=");
            stb.AppendLine(_OperatorID);
            stb.Append("General12=");
            stb.AppendLine("");
            stb.Append("General13=");
            stb.AppendLine(_Side);
            stb.Append("General14=");
            stb.AppendLine("");
            stb.Append("General15=");
            stb.AppendLine(_FirstPanel ? "Y" : "N");
            stb.Append("Machine1=");
            stb.AppendLine(_TrolleyCode);
            stb.Append("Machine2=");
            stb.AppendLine(_TargetOvenTemperature.ToString("0"));
            stb.Append("Machine3=");
            stb.AppendLine(_WarmingTime.ToString());
            stb.Append("Machine4=");
            stb.AppendLine(_HeatingTime.ToString());
            stb.Append("Machine5=");
            stb.AppendLine(_TotalHeatingTime.ToString());
            stb.Append("Machine6=");
            stb.AppendLine("");

            return stb.ToString();
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