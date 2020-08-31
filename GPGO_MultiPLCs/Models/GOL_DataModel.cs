using GPMVVM.Models;
using GPMVVM.PLCService;

namespace GPGO_MultiPLCs.Models
{
    [PLCObject]
    public class GOL_DataModel : DataProvider
    {
        #region PC=>PLC

        [PLCBit(BitType.M, 200, LogType.Trigger)]
        public bool Start
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBit(BitType.M, 201, LogType.Trigger)]
        public bool Stop
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBit(BitType.M, 21, LogType.Trigger)]
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
        [PLCData(DataType.D, 1080, 16, LogType.Recipe)]
        public string RecipeName
        {
            get => GetPLC<string>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 降溫溫度
        /// </summary>
        public double CoolingTemperature
        {
            get => GetPLC<short>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 降溫時間
        /// </summary>
        [PLCData(DataType.D, 1076, 0.1, LogType.Recipe)]
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
        [PLCData(DataType.D, 157, 0.1, LogType.Recipe)]
        public double ProgramStopAlarmTime
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 使用段數
        /// </summary>
        [PLCData(DataType.D, 1075, LogType.Recipe)]
        public short UsedSegmentCounts
        {
            get => GetPLC<short>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度1
        /// </summary>
        [PLCData(DataType.D, 700, LogType.Recipe)]
        public double TargetTemperature_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度2
        /// </summary>
        [PLCData(DataType.D, 701, LogType.Recipe)]
        public double TargetTemperature_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度3
        /// </summary>
        [PLCData(DataType.D, 702, LogType.Recipe)]
        public double TargetTemperature_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度4
        /// </summary>
        [PLCData(DataType.D, 703, LogType.Recipe)]
        public double TargetTemperature_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度5
        /// </summary>
        [PLCData(DataType.D, 704, LogType.Recipe)]
        public double TargetTemperature_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目標溫度6
        /// </summary>
        [PLCData(DataType.D, 705, LogType.Recipe)]
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
        [PLCData(DataType.D, 760, 0.1, LogType.Recipe)]
        public double HeatingTime_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間2
        /// </summary>
        [PLCData(DataType.D, 761, 0.1, LogType.Recipe)]
        public double HeatingTime_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間3
        /// </summary>
        [PLCData(DataType.D, 762, 0.1, LogType.Recipe)]
        public double HeatingTime_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間4
        /// </summary>
        [PLCData(DataType.D, 763, 0.1, LogType.Recipe)]
        public double HeatingTime_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間5
        /// </summary>
        [PLCData(DataType.D, 764, 0.1, LogType.Recipe)]
        public double HeatingTime_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫時間6
        /// </summary>
        [PLCData(DataType.D, 765, 0.1, LogType.Recipe)]
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
        [PLCData(DataType.D, 730, 0.1, LogType.Recipe)]
        public double HeatingAlarm_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間2
        /// </summary>
        [PLCData(DataType.D, 731, 0.1, LogType.Recipe)]
        public double HeatingAlarm_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間3
        /// </summary>
        [PLCData(DataType.D, 732, 0.1, LogType.Recipe)]
        public double HeatingAlarm_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間4
        /// </summary>
        [PLCData(DataType.D, 733, 0.1, LogType.Recipe)]
        public double HeatingAlarm_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間5
        /// </summary>
        [PLCData(DataType.D, 734, 0.1, LogType.Recipe)]
        public double HeatingAlarm_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升溫警報時間6
        /// </summary>
        [PLCData(DataType.D, 735, 0.1, LogType.Recipe)]
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
        [PLCData(DataType.D, 715, 0.1, LogType.Recipe)]
        public double WarmingTime_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間2
        /// </summary>
        [PLCData(DataType.D, 716, 0.1, LogType.Recipe)]
        public double WarmingTime_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間3
        /// </summary>
        [PLCData(DataType.D, 717, 0.1, LogType.Recipe)]
        public double WarmingTime_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間4
        /// </summary>
        [PLCData(DataType.D, 718, 0.1, LogType.Recipe)]
        public double WarmingTime_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間5
        /// </summary>
        [PLCData(DataType.D, 719, 0.1, LogType.Recipe)]
        public double WarmingTime_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫時間6
        /// </summary>
        [PLCData(DataType.D, 720, 0.1, LogType.Recipe)]
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
        [PLCData(DataType.D, 745, 0.1, LogType.Recipe)]
        public double WarmingAlarm_1
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間2
        /// </summary>
        [PLCData(DataType.D, 746, 0.1, LogType.Recipe)]
        public double WarmingAlarm_2
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間3
        /// </summary>
        [PLCData(DataType.D, 747, 0.1, LogType.Recipe)]
        public double WarmingAlarm_3
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間4
        /// </summary>
        [PLCData(DataType.D, 748, 0.1, LogType.Recipe)]
        public double WarmingAlarm_4
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間5
        /// </summary>
        [PLCData(DataType.D, 749, 0.1, LogType.Recipe)]
        public double WarmingAlarm_5
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 恆溫警報時間6
        /// </summary>
        [PLCData(DataType.D, 750, 0.1, LogType.Recipe)]
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
        [PLCBit(BitType.M, 209, LogType.Alarm)]
        public bool ProgramStop
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 加熱門未關
        /// </summary>
        [PLCBit(BitType.M, 250, LogType.Alarm)]
        public bool DoorNotClosed
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 緊急停止
        /// </summary>
        [PLCBit(BitType.M, 700, LogType.Alarm)]
        public bool EmergencyStop
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 溫控器低溫異常
        /// </summary>
        public bool LowTemperature
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 電源反相
        /// </summary>
        [PLCBit(BitType.M, 702, LogType.Alarm)]
        public bool PowerInversion
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// OTP超溫異常
        /// </summary>
        [PLCBit(BitType.M, 703, LogType.Alarm)]
        public bool OTP_TemperatureError
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 循環風車過載
        /// </summary>
        [PLCBit(BitType.M, 704, LogType.Alarm)]
        public bool CirculatingFanOverload
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 冷卻進氣風車異常
        /// </summary>
        [PLCBit(BitType.M, 701, LogType.Alarm)]
        public bool CoolingFanException
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 冷卻進氣風車電流異常
        /// </summary>
        [PLCBit(BitType.M, 705, LogType.Alarm)]
        public bool CoolingFanCurrentException
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 超溫警報
        /// </summary>
        [PLCBit(BitType.M, 302, LogType.Alarm)]
        public bool OverTemperatureAlarm
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 停止後未開門
        /// </summary>
        [PLCBit(BitType.M, 714, LogType.Alarm)]
        public bool DoorNotOpen
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 循環風車INV異常
        /// </summary>
        public bool CirculatingFanInversion
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 循環風車電流異常
        /// </summary>
        [PLCBit(BitType.M, 707, LogType.Alarm)]
        public bool CirculatingFanCurrentException
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 充氮氣逾時
        /// </summary>
        public bool InflatingTimeExceeded
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 門未關定位異常
        /// </summary>
        public bool DoorNotClosed_PositionException
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 升恆溫逾時
        /// </summary>
        public bool HeatingTimeExceeded
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 加熱分路跳脫
        /// </summary>
        [PLCBit(BitType.M, 706, LogType.Alarm)]
        public bool HeatingBranchException
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 溫控器異常
        /// </summary>
        [PLCBit(BitType.M, 708, LogType.Alarm)]
        public bool ThermostatException
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 通訊異常
        /// </summary>
        [PLCBit(BitType.M, 709, LogType.Alarm)]
        public bool CommunicationException
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 寫入溫度比對異常
        /// </summary>
        [PLCBit(BitType.M, 710, LogType.Alarm)]
        public bool TemperatureWriteError
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        #endregion

        #region 機台狀態

        /// <summary>
        /// 允許啟動
        /// </summary>
        [PLCBit(BitType.M, 100, LogType.Status)]
        public bool AllowStart
        {
            get => GetPLC<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 允許停止
        /// </summary>
        [PLCBit(BitType.M, 101, LogType.Status)]
        public bool AllowStop
        {
            get => GetPLC<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 蜂鳴器
        /// </summary>
        [PLCBit(BitType.Y, 0, LogType.Status)]
        public bool Buzzer
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 綠燈
        /// </summary>
        [PLCBit(BitType.Y, 1, LogType.Status)]
        public bool GreenLight
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 黃燈
        /// </summary>
        [PLCBit(BitType.Y, 2, LogType.Status)]
        public bool YellowLight
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 紅燈
        /// </summary>
        [PLCBit(BitType.Y, 3, LogType.Status)]
        public bool RedLight
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 降溫中
        /// </summary>
        [PLCBit(BitType.M, 208, LogType.Status)]
        public bool IsCooling
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 手動模式
        /// </summary>
        [PLCBit(BitType.M, 60, LogType.Status)]
        public bool ManualMode
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 自動模式
        /// </summary>
        [PLCBit(BitType.M, 50, LogType.Status)]
        public bool AutoMode
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 自動停止
        /// </summary>
        [PLCBit(BitType.M, 52, LogType.Status)]
        public bool AutoMode_Stop
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 自動啟動
        /// </summary>
        [PLCBit(BitType.M, 51, LogType.Status)]
        public bool AutoMode_Start
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        [PLCBit(BitType.S, 501, LogType.Status)]
        public bool PC_InUsed
        {
            get => GetPLC<bool>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 溫控器實際溫度
        /// </summary>
        [PLCData(DataType.D, 64, LogType.Status)]
        public double ThermostatTemperature
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 溫控器設定溫度
        /// </summary>
        [PLCData(DataType.D, 65, LogType.Status)]
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
        /// 片段剩餘時間
        /// </summary>
        public double Segment_RemainingTime
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 總剩餘時間
        /// </summary>
        public double Total_RemainingTime
        {
            get => GetPLC<double>();
            set => SetPLC(value);
        }

        /// <summary>
        /// 目前段數
        /// </summary>
        [PLCData(DataType.D, 22, LogType.Status)]
        public int CurrentSegment
        {
            get => GetPLC<int>();
            set => SetPLC(value);
        }

        [PLCData(DataType.D, 880, 40, LogType.Status)]
        public string RackID
        {
            get => GetPLC<string>();
            set => Set(value);
        }

        #endregion
    }
}
