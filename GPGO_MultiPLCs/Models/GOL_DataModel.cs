using GPMVVM.Models;
using GPMVVM.PLCService;
using System.Collections.Generic;

namespace GPGO_MultiPLCs.Models
{
    public class GOL_DataModel : PLCDataProvider
    {
        #region PC=>PLC

        [PLCBitData(DataType.D, 12020, 0, LogType.Trigger)]
        public bool RemoteCommandStart
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12020, 1, LogType.Trigger)]
        public bool RemoteCommandStop
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 11067, 0, LogType.Trigger)]
        public bool RemoteCommandSelectPP
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 11068, 0, LogType.Trigger)]
        public bool RemoteCommandSelectPPFinish
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBit(BitType.M, 21, LogType.None)]
        public bool Check
        {
            get => Get<bool>();
            set => Set(value);
        }

        #endregion

        #region 配方設定值

        /// <summary>
        /// 配方名
        /// </summary>
        [PLCData(DataType.D, 11200, 40, LogType.RecipeSet)]
        public string RecipeName
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>
        /// 降溫溫度
        /// </summary>
        [PLCData(DataType.D, 11272, 0.1, LogType.RecipeSet)]
        public double CoolingTemperature
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 降溫時間
        /// </summary>
        [PLCData(DataType.D, 11271, LogType.RecipeSet)]
        public double CoolingTime
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 充氣時間
        /// </summary>
        public double InflatingTime
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 程式結束警報時間
        /// </summary>
        [PLCData(DataType.D, 11273, LogType.RecipeSet)]
        public double ProgramEndWarningTime
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 使用段數
        /// </summary>
        [PLCData(DataType.D, 11270, LogType.RecipeSet)]
        public short StepCounts
        {
            get => Get<short>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度1
        /// </summary>
        [PLCData(DataType.D, 11220, 0.1, LogType.RecipeSet)]
        public double TemperatureSetpoint_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度2
        /// </summary>
        [PLCData(DataType.D, 11221, 0.1, LogType.RecipeSet)]
        public double TemperatureSetpoint_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度3
        /// </summary>
        [PLCData(DataType.D, 11222, 0.1, LogType.RecipeSet)]
        public double TemperatureSetpoint_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度4
        /// </summary>
        [PLCData(DataType.D, 11223, 0.1, LogType.RecipeSet)]
        public double TemperatureSetpoint_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度5
        /// </summary>
        [PLCData(DataType.D, 11224, 0.1, LogType.RecipeSet)]
        public double TemperatureSetpoint_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度6
        /// </summary>
        [PLCData(DataType.D, 11225, 0.1, LogType.RecipeSet)]
        public double TemperatureSetpoint_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度7
        /// </summary>
        public double TemperatureSetpoint_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度8
        /// </summary>
        public double TemperatureSetpoint_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度1
        /// </summary>
        public double DwellTemperature_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度2
        /// </summary>
        public double DwellTemperature_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度3
        /// </summary>
        public double DwellTemperature_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度4
        /// </summary>
        public double DwellTemperature_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度5
        /// </summary>
        public double DwellTemperature_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度6
        /// </summary>
        public double DwellTemperature_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度7
        /// </summary>
        public double DwellTemperature_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度8
        /// </summary>
        public double DwellTemperature_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間1
        /// </summary>
        [PLCData(DataType.D, 11260, LogType.RecipeSet)]
        public double RampTime_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間2
        /// </summary>
        [PLCData(DataType.D, 11261, LogType.RecipeSet)]
        public double RampTime_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間3
        /// </summary>
        [PLCData(DataType.D, 11262, LogType.RecipeSet)]
        public double RampTime_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間4
        /// </summary>
        [PLCData(DataType.D, 11263, LogType.RecipeSet)]
        public double RampTime_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間5
        /// </summary>
        [PLCData(DataType.D, 11264, LogType.RecipeSet)]
        public double RampTime_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間6
        /// </summary>
        [PLCData(DataType.D, 11265, LogType.RecipeSet)]
        public double RampTime_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間7
        /// </summary>
        public double RampTime_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間8
        /// </summary>
        public double RampTime_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間1
        /// </summary>
        [PLCData(DataType.D, 11240, LogType.RecipeSet)]
        public double RampAlarm_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間2
        /// </summary>
        [PLCData(DataType.D, 11241, LogType.RecipeSet)]
        public double RampAlarm_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間3
        /// </summary>
        [PLCData(DataType.D, 11242, LogType.RecipeSet)]
        public double RampAlarm_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間4
        /// </summary>
        [PLCData(DataType.D, 11243, LogType.RecipeSet)]
        public double RampAlarm_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間5
        /// </summary>
        [PLCData(DataType.D, 11244, LogType.RecipeSet)]
        public double RampAlarm_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間6
        /// </summary>
        [PLCData(DataType.D, 11245, LogType.RecipeSet)]
        public double RampAlarm_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間7
        /// </summary>
        public double RampAlarm_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間8
        /// </summary>
        public double RampAlarm_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間1
        /// </summary>
        [PLCData(DataType.D, 11230, LogType.RecipeSet)]
        public double DwellTime_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間2
        /// </summary>
        [PLCData(DataType.D, 11231, LogType.RecipeSet)]
        public double DwellTime_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間3
        /// </summary>
        [PLCData(DataType.D, 11232, LogType.RecipeSet)]
        public double DwellTime_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間4
        /// </summary>
        [PLCData(DataType.D, 11233, LogType.RecipeSet)]
        public double DwellTime_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間5
        /// </summary>
        [PLCData(DataType.D, 11234, LogType.RecipeSet)]
        public double DwellTime_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間6
        /// </summary>
        [PLCData(DataType.D, 11235, LogType.RecipeSet)]
        public double DwellTime_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間7
        /// </summary>
        public double DwellTime_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間8
        /// </summary>
        public double DwellTime_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間1
        /// </summary>
        [PLCData(DataType.D, 11250, LogType.RecipeSet)]
        public double DwellAlarm_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間2
        /// </summary>
        [PLCData(DataType.D, 11251, LogType.RecipeSet)]
        public double DwellAlarm_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間3
        /// </summary>
        [PLCData(DataType.D, 11252, LogType.RecipeSet)]
        public double DwellAlarm_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間4
        /// </summary>
        [PLCData(DataType.D, 11253, LogType.RecipeSet)]
        public double DwellAlarm_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間5
        /// </summary>
        [PLCData(DataType.D, 11254, LogType.RecipeSet)]
        public double DwellAlarm_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間6
        /// </summary>
        [PLCData(DataType.D, 11255, LogType.RecipeSet)]
        public double DwellAlarm_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間7
        /// </summary>
        public double DwellAlarm_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間8
        /// </summary>
        public double DwellAlarm_8
        {
            get => Get<double>();
            set => Set(value);
        }

        #endregion

        #region 配方運作值

        /// <summary>
        /// 配方名
        /// </summary>
        [PLCData(DataType.D, 11400, 40, LogType.StatusVariables)]
        public string PV_RecipeName
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>
        /// 降溫溫度
        /// </summary>
        [PLCData(DataType.D, 11472, 0.1, LogType.StatusVariables)]
        public double PV_CoolingTemperature
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 降溫時間
        /// </summary>
        [PLCData(DataType.D, 11471, LogType.StatusVariables)]
        public double PV_CoolingTime
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 充氣時間
        /// </summary>
        public double PV_InflatingTime
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 程式結束警報時間
        /// </summary>
        [PLCData(DataType.D, 11473, LogType.StatusVariables)]
        public double PV_ProgramEndWarningTime
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 使用段數
        /// </summary>
        [PLCData(DataType.D, 11470, LogType.StatusVariables)]
        public short PV_StepCounts
        {
            get => Get<short>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度1
        /// </summary>
        [PLCData(DataType.D, 11420, 0.1, LogType.StatusVariables)]
        public double PV_TemperatureSetpoint_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度2
        /// </summary>
        [PLCData(DataType.D, 11421, 0.1, LogType.StatusVariables)]
        public double PV_TemperatureSetpoint_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度3
        /// </summary>
        [PLCData(DataType.D, 11422, 0.1, LogType.StatusVariables)]
        public double PV_TemperatureSetpoint_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度4
        /// </summary>
        [PLCData(DataType.D, 11423, 0.1, LogType.StatusVariables)]
        public double PV_TemperatureSetpoint_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度5
        /// </summary>
        [PLCData(DataType.D, 11424, 0.1, LogType.StatusVariables)]
        public double PV_TemperatureSetpoint_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度6
        /// </summary>
        [PLCData(DataType.D, 11425, 0.1, LogType.StatusVariables)]
        public double PV_TemperatureSetpoint_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度7
        /// </summary>
        public double PV_TemperatureSetpoint_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目標溫度8
        /// </summary>
        public double PV_TemperatureSetpoint_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度1
        /// </summary>
        public double PV_DwellTemperature_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度2
        /// </summary>
        public double PV_DwellTemperature_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度3
        /// </summary>
        public double PV_DwellTemperature_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度4
        /// </summary>
        public double PV_DwellTemperature_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度5
        /// </summary>
        public double PV_DwellTemperature_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度6
        /// </summary>
        public double PV_DwellTemperature_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度7
        /// </summary>
        public double PV_DwellTemperature_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫溫度8
        /// </summary>
        public double PV_DwellTemperature_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間1
        /// </summary>
        [PLCData(DataType.D, 11460, LogType.StatusVariables)]
        public double PV_RampTime_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間2
        /// </summary>
        [PLCData(DataType.D, 11461, LogType.StatusVariables)]
        public double PV_RampTime_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間3
        /// </summary>
        [PLCData(DataType.D, 11462, LogType.StatusVariables)]
        public double PV_RampTime_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間4
        /// </summary>
        [PLCData(DataType.D, 11463, LogType.StatusVariables)]
        public double PV_RampTime_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間5
        /// </summary>
        [PLCData(DataType.D, 11464, LogType.StatusVariables)]
        public double PV_RampTime_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間6
        /// </summary>
        [PLCData(DataType.D, 11465, LogType.StatusVariables)]
        public double PV_RampTime_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間7
        /// </summary>
        public double PV_RampTime_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫時間8
        /// </summary>
        public double PV_RampTime_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間1
        /// </summary>
        [PLCData(DataType.D, 11440, LogType.StatusVariables)]
        public double PV_RampAlarm_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間2
        /// </summary>
        [PLCData(DataType.D, 11441, LogType.StatusVariables)]
        public double PV_RampAlarm_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間3
        /// </summary>
        [PLCData(DataType.D, 11442, LogType.StatusVariables)]
        public double PV_RampAlarm_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間4
        /// </summary>
        [PLCData(DataType.D, 11443, LogType.StatusVariables)]
        public double PV_RampAlarm_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間5
        /// </summary>
        [PLCData(DataType.D, 11444, LogType.StatusVariables)]
        public double PV_RampAlarm_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間6
        /// </summary>
        [PLCData(DataType.D, 11445, LogType.StatusVariables)]
        public double PV_RampAlarm_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間7
        /// </summary>
        public double PV_RampAlarm_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫警報時間8
        /// </summary>
        public double PV_RampAlarm_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間1
        /// </summary>
        [PLCData(DataType.D, 11430, LogType.StatusVariables)]
        public double PV_DwellTime_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間2
        /// </summary>
        [PLCData(DataType.D, 11431, LogType.StatusVariables)]
        public double PV_DwellTime_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間3
        /// </summary>
        [PLCData(DataType.D, 11432, LogType.StatusVariables)]
        public double PV_DwellTime_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間4
        /// </summary>
        [PLCData(DataType.D, 11433, LogType.StatusVariables)]
        public double PV_DwellTime_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間5
        /// </summary>
        [PLCData(DataType.D, 11434, LogType.StatusVariables)]
        public double PV_DwellTime_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間6
        /// </summary>
        [PLCData(DataType.D, 11435, LogType.StatusVariables)]
        public double PV_DwellTime_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間7
        /// </summary>
        public double PV_DwellTime_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫時間8
        /// </summary>
        public double PV_DwellTime_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間1
        /// </summary>
        [PLCData(DataType.D, 11450, LogType.StatusVariables)]
        public double PV_DwellAlarm_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間2
        /// </summary>
        [PLCData(DataType.D, 11451, LogType.StatusVariables)]
        public double PV_DwellAlarm_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間3
        /// </summary>
        [PLCData(DataType.D, 11452, LogType.StatusVariables)]
        public double PV_DwellAlarm_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間4
        /// </summary>
        [PLCData(DataType.D, 11453, LogType.StatusVariables)]
        public double PV_DwellAlarm_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間5
        /// </summary>
        [PLCData(DataType.D, 11454, LogType.StatusVariables)]
        public double PV_DwellAlarm_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間6
        /// </summary>
        [PLCData(DataType.D, 11455, LogType.StatusVariables)]
        public double PV_DwellAlarm_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間7
        /// </summary>
        public double PV_DwellAlarm_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫警報時間8
        /// </summary>
        public double PV_DwellAlarm_8
        {
            get => Get<double>();
            set => Set(value);
        }

        #endregion

        #region 警報

        /// <summary>
        /// 緊急停止
        /// </summary>
        [PLCBitData(DataType.D, 12000, 0, LogType.Alarm)]
        public bool EmergencyStop
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 溫控器低溫異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 1, LogType.Alert)]
        public bool LowTemperature
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 電源相位異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 2, LogType.Alarm)]
        public bool PowerPhaseError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// OTP超溫異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 3, LogType.Alarm)]
        public bool OTPTemperatureError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 循環風車電流異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 4, LogType.Alarm)]
        public bool CirculatingFanCurrentError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 冷卻進氣風車異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 5, LogType.Alert)]
        public bool CoolingFanError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 溫控器訊號異常
        /// </summary>
        [PLCBitData(DataType.D, 12000, 6, LogType.Alarm)]
        public bool ThermostatSignalError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 開門極限
        /// </summary>
        [PLCBitData(DataType.D, 12000, 7, LogType.Alarm)]
        public bool DoorOpeningLimit
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 關門極限
        /// </summary>
        [PLCBitData(DataType.D, 12000, 8, LogType.Alarm)]
        public bool DoorClosingLimit
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 超溫警報
        /// </summary>
        [PLCBitData(DataType.D, 12000, 10, LogType.Alarm)]
        public bool OverTemperatureAlarm
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 電動門要求復歸
        /// </summary>
        [PLCBitData(DataType.D, 12000, 12, LogType.Alarm)]
        public bool ElectricDoorInitRequest
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 電動門不在位置上
        /// </summary>
        [PLCBitData(DataType.D, 12000, 13, LogType.Alarm)]
        public bool ElectricDoorPositionError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 停止後未開門
        /// </summary>
        [PLCBitData(DataType.D, 12000, 15, LogType.Alert)]
        public bool DoorNotOpen
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 循環風車變頻器異常
        /// </summary>
        [PLCBitData(DataType.D, 12001, 2, LogType.Alarm)]
        public bool CirculatingFanInverterError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 充氮氣逾時
        /// </summary>
        [PLCBitData(DataType.D, 12001, 5, LogType.Alert)]
        public bool InflatingTimeExceeded
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 門未關定位異常
        /// </summary>
        [PLCBitData(DataType.D, 12001, 6, LogType.Alert)]
        public bool DoorNotClosedPositionException
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 升恆溫逾時
        /// </summary>
        [PLCBitData(DataType.D, 12001, 7, LogType.Alarm)]
        public bool RampTimeExceeded
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 加熱分路跳脫
        /// </summary>
        [PLCBitData(DataType.D, 12001, 8, LogType.Alarm)]
        public bool RampBranchException
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// PLC電池電壓不足
        /// </summary>
        [PLCBitData(DataType.D, 12001, 10, LogType.Alarm)]
        public bool PLCBatteryLow
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 電熱ELB跳脫
        /// </summary>
        [PLCBitData(DataType.D, 12001, 11, LogType.Alarm)]
        public bool ELBtrip
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 伺服驅動器電池電壓不足
        /// </summary>
        [PLCBitData(DataType.D, 12001, 12, LogType.Alarm)]
        public bool ServoDriverBatteryVoltLow
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 開門極限異常
        /// </summary>
        [PLCBitData(DataType.D, 12001, 13, LogType.Alarm)]
        public bool DoorOpenLimitError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 關門極限異常
        /// </summary>
        [PLCBitData(DataType.D, 12001, 14, LogType.Alarm)]
        public bool DoorCloseLimitError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 伺服驅動器異常
        /// </summary>
        [PLCBitData(DataType.D, 12001, 15, LogType.Alarm)]
        public bool ServoDriverError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 光閘偵測異常
        /// </summary>
        [PLCBitData(DataType.D, 12002, 0, LogType.Alarm)]
        public bool RasterError
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 程式結束
        /// </summary>
        [PLCBitData(DataType.D, 12002, 1, LogType.Alarm)]
        public bool ProgramStop
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 加熱門未關
        /// </summary>
        [PLCBitData(DataType.D, 12002, 2, LogType.Alert)]
        public bool DoorNotClosed
        {
            get => Get<bool>();
            set => Set(value);
        }

        #endregion

        #region 機台狀態

        [PLCBitData(DataType.D, 12010, 0, LogType.StatusVariables)]
        public bool ReadBarcode
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12011, 1, LogType.StatusVariables)]
        public bool RackInput
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12011, 7, LogType.StatusVariables)]
        public bool RackOutput
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 允許啟動
        /// </summary>
        [PLCBitData(DataType.D, 12010, 1, LogType.StatusVariables)]
        public bool AllowStart
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 允許停止
        /// </summary>
        public bool AllowStop
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 綠燈
        /// </summary>
        [PLCBitData(DataType.D, 12010, 14, LogType.None)]
        public bool GreenLight
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 黃燈
        /// </summary>
        [PLCBitData(DataType.D, 12010, 13, LogType.None)]
        public bool YellowLight
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 紅燈
        /// </summary>
        [PLCBitData(DataType.D, 12010, 12, LogType.None)]
        public bool RedLight
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 升溫中
        /// </summary>
        [PLCBitData(DataType.D, 12011, 3, LogType.StatusVariables)]
        public bool IsRamp
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 恆溫中
        /// </summary>
        [PLCBitData(DataType.D, 12011, 4, LogType.StatusVariables)]
        public bool IsWarming
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 降溫中
        /// </summary>
        [PLCBitData(DataType.D, 12011, 5, LogType.StatusVariables)]
        public bool IsCooling
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12010, 4, LogType.StatusVariables)]
        public bool AutoMode
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12010, 3, LogType.StatusVariables)]
        public bool ManualMode
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12010, 6, LogType.StatusVariables)]
        public bool AutoMode_Start
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12010, 2, LogType.StatusVariables)]
        public bool ProcessComplete
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12010, 5, LogType.StatusVariables)]
        public bool AutoMode_Stop
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12010, 10, LogType.StatusVariables)]
        public bool PC_InUse
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12010, 8, LogType.StatusVariables)]
        public bool RemoteMode
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBitData(DataType.D, 12010, 9, LogType.StatusVariables)]
        public bool LocalMode
        {
            get => Get<bool>();
            set => Set(value);
        }

        [PLCBit(BitType.M, 46, LogType.StatusVariables)]
        public bool RecipeChanged
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        /// 溫控器實際溫度
        /// </summary>
        [PLCData(DataType.D, 11124, 0.1, LogType.StatusVariables)]
        public double PV_ThermostatTemperature
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 溫控器設定溫度
        /// </summary>
        [PLCData(DataType.D, 11125, 0.1, LogType.StatusVariables)]
        public double SV_ThermostatTemperature
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 爐內溫度1
        /// </summary>
        [PLCData(DataType.D, 11380, 0.1, LogType.StatusVariables)]
        public double OvenTemperature_1
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 爐內溫度2
        /// </summary>
        [PLCData(DataType.D, 11381, 0.1, LogType.StatusVariables)]
        public double OvenTemperature_2
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 爐內溫度3
        /// </summary>
        [PLCData(DataType.D, 11382, 0.1, LogType.StatusVariables)]
        public double OvenTemperature_3
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 爐內溫度4
        /// </summary>
        [PLCData(DataType.D, 11383, 0.1, LogType.StatusVariables)]
        public double OvenTemperature_4
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 爐內溫度5
        /// </summary>
        [PLCData(DataType.D, 11384, 0.1, LogType.StatusVariables)]
        public double OvenTemperature_5
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 爐內溫度6
        /// </summary>
        [PLCData(DataType.D, 11385, 0.1, LogType.StatusVariables)]
        public double OvenTemperature_6
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 爐內溫度7
        /// </summary>
        public double OvenTemperature_7
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 爐內溫度8
        /// </summary>
        public double OvenTemperature_8
        {
            get => Get<double>();
            set => Set(value);
        }

        /// <summary>
        /// 目前段數
        /// </summary>
        [PLCData(DataType.D, 11479, LogType.StatusVariables)]
        public short CurrentStep
        {
            get => Get<short>();
            set => Set(value);
        }

        /// <summary>
        /// RackID
        /// </summary>
        [PLCData(DataType.D, 11100, 24, LogType.StatusVariables)]
        public string RackID
        {
            get => Get<string>();
            set => Set(value);
        }

        [PLCData(DataType.D, 11043, LogType.StatusVariables)]
        public short EquipmentState
        {
            get => Get<short>();
            set => Set(value);
        }

        [PLCData(DataType.D, 11045, 40, LogType.StatusVariables)]
        public string EquipmentName
        {
            get => Get<string>();
            set => Set(value);
        }

        #endregion

        public GOL_DataModel(PLCGate plcGate, int plcIndex, string plcTag, (Dictionary<BitType, int> bits_shift, Dictionary<DataType, int> datas_shift) shift = new()) : base(plcGate, plcIndex, plcTag, shift) {}
    }
}