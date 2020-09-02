using GPMVVM.Models;
using GPMVVM.PLCService;

namespace GPGO_MultiPLCs.Models
{
    [PLCObject]
    public class GOL_DataModel : DataProvider
    {
        #region PC=>PLC

        [PLCBitData(DataType.D, 12020, 0, LogType.Trigger)]
        public bool RemoteCommandStart
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12020, 1, LogType.Trigger)]
        public bool RemoteCommandStop
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 11067, 1, LogType.Trigger)]
        public bool RemoteCommandSelectPP
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBit(BitType.M, 21, LogType.None)]
        public bool Check
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        #endregion

        #region 配方

        /// <summary>
        /// 配方名
        /// </summary>
        [PLCData(DataType.D, 11200, 40, LogType.Recipe)]
        public string RecipeName
        {
            get => GetPLC<string>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 降溫溫度
        /// </summary>
        [PLCData(DataType.D, 11272, 0.1, LogType.Recipe)]
        public double CoolingTemperature
        {
            get => GetPLC<short>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 降溫時間
        /// </summary>
        [PLCData(DataType.D, 11271, LogType.Recipe)]
        public double CoolingTime
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 充氣時間
        /// </summary>
        public double InflatingTime
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 程式結束警報時間
        /// </summary>
        [PLCData(DataType.D, 11273, LogType.Recipe)]
        public double ProgramStopAlarmTime
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 使用段數
        /// </summary>
        [PLCData(DataType.D, 11270, LogType.Recipe)]
        public short UsedSegmentCounts
        {
            get => GetPLC<short>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度1
        /// </summary>
        [PLCData(DataType.D, 11220, 0.1, LogType.Recipe)]
        public double TargetTemperature_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度2
        /// </summary>
        [PLCData(DataType.D, 11221, 0.1, LogType.Recipe)]
        public double TargetTemperature_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度3
        /// </summary>
        [PLCData(DataType.D, 11222, 0.1, LogType.Recipe)]
        public double TargetTemperature_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度4
        /// </summary>
        [PLCData(DataType.D, 11223, 0.1, LogType.Recipe)]
        public double TargetTemperature_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度5
        /// </summary>
        [PLCData(DataType.D, 11224, 0.1, LogType.Recipe)]
        public double TargetTemperature_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度6
        /// </summary>
        [PLCData(DataType.D, 11225, 0.1, LogType.Recipe)]
        public double TargetTemperature_6
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度7
        /// </summary>
        public double TargetTemperature_7
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度8
        /// </summary>
        public double TargetTemperature_8
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫溫度1
        /// </summary>
        public double ThermostaticTemperature_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫溫度2
        /// </summary>
        public double ThermostaticTemperature_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫溫度3
        /// </summary>
        public double ThermostaticTemperature_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫溫度4
        /// </summary>
        public double ThermostaticTemperature_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫溫度5
        /// </summary>
        public double ThermostaticTemperature_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫溫度6
        /// </summary>
        public double ThermostaticTemperature_6
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫溫度7
        /// </summary>
        public double ThermostaticTemperature_7
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫溫度8
        /// </summary>
        public double ThermostaticTemperature_8
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間1
        /// </summary>
        [PLCData(DataType.D, 11260, LogType.Recipe)]
        public double HeatingTime_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間2
        /// </summary>
        [PLCData(DataType.D, 11261, LogType.Recipe)]
        public double HeatingTime_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間3
        /// </summary>
        [PLCData(DataType.D, 11262, LogType.Recipe)]
        public double HeatingTime_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間4
        /// </summary>
        [PLCData(DataType.D, 11263, LogType.Recipe)]
        public double HeatingTime_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間5
        /// </summary>
        [PLCData(DataType.D, 11264, LogType.Recipe)]
        public double HeatingTime_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間6
        /// </summary>
        [PLCData(DataType.D, 11265, LogType.Recipe)]
        public double HeatingTime_6
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間7
        /// </summary>
        public double HeatingTime_7
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間8
        /// </summary>
        public double HeatingTime_8
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間1
        /// </summary>
        [PLCData(DataType.D, 11240, LogType.Recipe)]
        public double HeatingAlarm_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間2
        /// </summary>
        [PLCData(DataType.D, 11241, LogType.Recipe)]
        public double HeatingAlarm_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間3
        /// </summary>
        [PLCData(DataType.D, 11242, LogType.Recipe)]
        public double HeatingAlarm_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間4
        /// </summary>
        [PLCData(DataType.D, 11243, LogType.Recipe)]
        public double HeatingAlarm_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間5
        /// </summary>
        [PLCData(DataType.D, 11244, LogType.Recipe)]
        public double HeatingAlarm_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間6
        /// </summary>
        [PLCData(DataType.D, 11245, LogType.Recipe)]
        public double HeatingAlarm_6
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間7
        /// </summary>
        public double HeatingAlarm_7
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間8
        /// </summary>
        public double HeatingAlarm_8
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間1
        /// </summary>
        [PLCData(DataType.D, 11230, LogType.Recipe)]
        public double WarmingTime_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間2
        /// </summary>
        [PLCData(DataType.D, 11231, LogType.Recipe)]
        public double WarmingTime_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間3
        /// </summary>
        [PLCData(DataType.D, 11232, LogType.Recipe)]
        public double WarmingTime_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間4
        /// </summary>
        [PLCData(DataType.D, 11233, LogType.Recipe)]
        public double WarmingTime_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間5
        /// </summary>
        [PLCData(DataType.D, 11234, LogType.Recipe)]
        public double WarmingTime_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間6
        /// </summary>
        [PLCData(DataType.D, 11235, LogType.Recipe)]
        public double WarmingTime_6
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間7
        /// </summary>
        public double WarmingTime_7
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間8
        /// </summary>
        public double WarmingTime_8
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間1
        /// </summary>
        [PLCData(DataType.D, 11250, LogType.Recipe)]
        public double WarmingAlarm_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間2
        /// </summary>
        [PLCData(DataType.D, 11251, LogType.Recipe)]
        public double WarmingAlarm_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間3
        /// </summary>
        [PLCData(DataType.D, 11252, LogType.Recipe)]
        public double WarmingAlarm_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間4
        /// </summary>
        [PLCData(DataType.D, 11253, LogType.Recipe)]
        public double WarmingAlarm_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間5
        /// </summary>
        [PLCData(DataType.D, 11254, LogType.Recipe)]
        public double WarmingAlarm_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間6
        /// </summary>
        [PLCData(DataType.D, 11255, LogType.Recipe)]
        public double WarmingAlarm_6
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間7
        /// </summary>
        public double WarmingAlarm_7
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間8
        /// </summary>
        public double WarmingAlarm_8
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        #endregion

        #region 警報

        /// <summary>
        /// 程式結束
        /// </summary>
        [PLCBitData(DataType.D, 12002, 1, LogType.Alarm)]
        public bool ProgramStop
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 加熱門未關
        /// </summary>
        [PLCBitData(DataType.D, 12002, 2, LogType.Alert)]
        public bool DoorNotClosed
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 緊急停止
        /// </summary>
        [PLCBitData(DataType.D, 12000, 0, LogType.Alarm)]
        public bool EmergencyStop
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 溫控器低溫異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 1, LogType.Alert)]
        public bool LowTemperature
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 電源相位異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 2, LogType.Alarm)]
        public bool PowerPhaseError
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// OTP超溫異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 3, LogType.Alarm)]
        public bool OTP_TemperatureError
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 循環風車電流異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 4, LogType.Alarm)]
        public bool CirculatingFanCurrentError
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 冷卻進氣風車異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 5, LogType.Alert)]
        public bool CoolingFanError
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12000, 6, LogType.Alarm)]
        public bool ThermostatSignalError
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12000, 7, LogType.Alarm)]
        public bool DoorOpeningLimit
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12000, 8, LogType.Alarm)]
        public bool DoorClosingLimit
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 超溫警報
        /// </summary>
        [PLCBitData(DataType.D, 12000, 10, LogType.Alarm)]
        public bool OverTemperatureAlarm
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 停止後未開門
        /// </summary>
        [PLCBitData(DataType.D, 12000, 15, LogType.Alert)]
        public bool DoorNotOpen
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 循環風車變頻器異常
        /// </summary>
        [PLCBitData(DataType.D, 12001, 2, LogType.Alarm)]
        public bool CirculatingFanInverterError
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 充氮氣逾時
        /// </summary>
        [PLCBitData(DataType.D, 12001, 5, LogType.Alert)]
        public bool InflatingTimeExceeded
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 門未關定位異常
        /// </summary>
        [PLCBitData(DataType.D, 12001, 6, LogType.Alert)]
        public bool DoorNotClosedPositionException
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升恆溫逾時
        /// </summary>
        [PLCBitData(DataType.D, 12001, 7, LogType.Alarm)]
        public bool HeatingTimeExceeded
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 加熱分路跳脫
        /// </summary>
        [PLCBitData(DataType.D, 12001, 8, LogType.Alarm)]
        public bool HeatingBranchException
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        #endregion

        #region 機台狀態

        /// <summary>
        /// 允許啟動
        /// </summary>
        [PLCBitData(DataType.D, 12010, 1, LogType.Status)]
        public bool AllowStart
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 允許停止
        /// </summary>
        public bool AllowStop
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 綠燈
        /// </summary>
        public bool GreenLight
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 黃燈
        /// </summary>
        public bool YellowLight
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 紅燈
        /// </summary>
        public bool RedLight
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫中
        /// </summary>
        [PLCBitData(DataType.D, 12011, 3, LogType.Status)]
        public bool IsHeating
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫中
        /// </summary>
        [PLCBitData(DataType.D, 12011, 4, LogType.Status)]
        public bool IsWarming
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 降溫中
        /// </summary>
        [PLCBitData(DataType.D, 12011, 15, LogType.Status)]
        public bool IsCooling
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12010, 3, LogType.Status)]
        public bool AutoMode
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12010, 4, LogType.Status)]
        public bool ManualMode
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12010, 5, LogType.Status)]
        public bool AutoMode_Start
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12010, 6, LogType.Status)]
        public bool AutoMode_Stop
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12010, 7, LogType.Status)]
        public bool PC_InUsed
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12010, 8, LogType.Status)]
        public bool RemoteMode
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBitData(DataType.D, 12010, 9, LogType.Status)]
        public bool LocalMode
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 溫控器實際溫度
        /// </summary>
        [PLCData(DataType.D, 11124, LogType.Status)]
        public double ThermostatTemperature
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 溫控器設定溫度
        /// </summary>
        [PLCData(DataType.D, 11125, LogType.Status)]
        public double ThermostatTemperatureSet
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 爐內溫度1
        /// </summary>
        public double OvenTemperature_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 爐內溫度2
        /// </summary>
        public double OvenTemperature_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 爐內溫度3
        /// </summary>
        public double OvenTemperature_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 爐內溫度4
        /// </summary>
        public double OvenTemperature_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 爐內溫度5
        /// </summary>
        public double OvenTemperature_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 爐內溫度6
        /// </summary>
        public double OvenTemperature_6
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 爐內溫度7
        /// </summary>
        public double OvenTemperature_7
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 爐內溫度8
        /// </summary>
        public double OvenTemperature_8
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目前段數
        /// </summary>
        [PLCData(DataType.D, 11479, LogType.Status)]
        public int CurrentSegment
        {
            get => GetPLC<int>();
            set => SetPLC(value);
        }

        /// <summary>
        /// RackID
        /// </summary>
        [PLCData(DataType.D, 11100, 40, LogType.Status)]
        public string RackID
        {
            get => GetPLC<string>();
            set => SetPLC(value);
        }

        [PLCData(DataType.D, 11043, LogType.Status)]
        public int EquipmentStatus
        {
            get => GetPLC<int>();
            set => SetPLC(value);
        }

        [PLCData(DataType.D, 11045, 40, LogType.Status)]
        public string EquipmentName
        {
            get => GetPLC<string>();
            set => SetPLC(value);
        }

        #endregion
    }
}
