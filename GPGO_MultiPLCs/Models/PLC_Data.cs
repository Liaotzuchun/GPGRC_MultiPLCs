using GPGO_MultiPLCs.GP_PLCs;
using GPMVVM.Helpers;
using GPMVVM.Models;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>PLC基礎資料</summary>
    public abstract class PLC_Data : ObservableObject
    {
        public TwoKeyDictionary<DataNames, (DataType, int), short> Data_Values;
        public TwoKeyDictionary<SignalNames, (BitType, int), bool> Bit_Values;
        public TwoKeyDictionary<DataNames, (DataType, int), short> Recipe_Values;

        #region 生產配方

        public string RecipeName
        {
            get =>
                new[]
                    {
                        Recipe_Values[DataNames.配方名稱_01], Recipe_Values[DataNames.配方名稱_02], Recipe_Values[DataNames.配方名稱_03], Recipe_Values[DataNames.配方名稱_04],
                        Recipe_Values[DataNames.配方名稱_05], Recipe_Values[DataNames.配方名稱_06], Recipe_Values[DataNames.配方名稱_07], Recipe_Values[DataNames.配方名稱_08],
                        Recipe_Values[DataNames.配方名稱_09], Recipe_Values[DataNames.配方名稱_10], Recipe_Values[DataNames.配方名稱_11], Recipe_Values[DataNames.配方名稱_12],
                        Recipe_Values[DataNames.配方名稱_13], Recipe_Values[DataNames.配方名稱_14], Recipe_Values[DataNames.配方名稱_15], Recipe_Values[DataNames.配方名稱_16],
                        Recipe_Values[DataNames.配方名稱_17], Recipe_Values[DataNames.配方名稱_18], Recipe_Values[DataNames.配方名稱_19], Recipe_Values[DataNames.配方名稱_20],
                    }.ASCIIfromShorts()
                     .Trim();
            set
            {
                if (value.Length > 40)
                {
                    value = value.Substring(0, 40);
                }

                var vals = value.PadRight(40).ASCIItoShorts();

                Recipe_Values[DataNames.配方名稱_01] = vals[0];
                Recipe_Values[DataNames.配方名稱_02] = vals[1];
                Recipe_Values[DataNames.配方名稱_03] = vals[2];
                Recipe_Values[DataNames.配方名稱_04] = vals[3];
                Recipe_Values[DataNames.配方名稱_05] = vals[4];
                Recipe_Values[DataNames.配方名稱_06] = vals[5];
                Recipe_Values[DataNames.配方名稱_07] = vals[6];
                Recipe_Values[DataNames.配方名稱_08] = vals[7];
                Recipe_Values[DataNames.配方名稱_09] = vals[8];
                Recipe_Values[DataNames.配方名稱_10] = vals[9];
                Recipe_Values[DataNames.配方名稱_11] = vals[10];
                Recipe_Values[DataNames.配方名稱_12] = vals[11];
                Recipe_Values[DataNames.配方名稱_13] = vals[12];
                Recipe_Values[DataNames.配方名稱_14] = vals[13];
                Recipe_Values[DataNames.配方名稱_15] = vals[14];
                Recipe_Values[DataNames.配方名稱_16] = vals[15];
                Recipe_Values[DataNames.配方名稱_17] = vals[16];
                Recipe_Values[DataNames.配方名稱_18] = vals[17];
                Recipe_Values[DataNames.配方名稱_19] = vals[18];
                Recipe_Values[DataNames.配方名稱_20] = vals[19];

                NotifyPropertyChanged(nameof(RecipeName));
            }
        }

        public double TargetTemperature_1
        {
            get => Recipe_Values[DataNames.目標溫度_1];
            set
            {
                Recipe_Values[DataNames.目標溫度_1] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_2
        {
            get => Recipe_Values[DataNames.目標溫度_2];
            set
            {
                Recipe_Values[DataNames.目標溫度_2] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_3
        {
            get => Recipe_Values[DataNames.目標溫度_3];
            set
            {
                Recipe_Values[DataNames.目標溫度_3] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_4
        {
            get => Recipe_Values[DataNames.目標溫度_4];
            set
            {
                Recipe_Values[DataNames.目標溫度_4] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_5
        {
            get => Recipe_Values[DataNames.目標溫度_5];
            set
            {
                Recipe_Values[DataNames.目標溫度_5] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_6
        {
            get => Recipe_Values[DataNames.目標溫度_6];
            set
            {
                Recipe_Values[DataNames.目標溫度_6] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_7
        {
            get => Recipe_Values[DataNames.目標溫度_7];
            set
            {
                Recipe_Values[DataNames.目標溫度_7] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_8
        {
            get => Recipe_Values[DataNames.目標溫度_8];
            set
            {
                Recipe_Values[DataNames.目標溫度_8] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_1
        {
            get => Recipe_Values[DataNames.恆溫溫度_1];
            set
            {
                Recipe_Values[DataNames.恆溫溫度_1] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_2
        {
            get => Recipe_Values[DataNames.恆溫溫度_2];
            set
            {
                Recipe_Values[DataNames.恆溫溫度_2] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_3
        {
            get => Recipe_Values[DataNames.恆溫溫度_3];
            set
            {
                Recipe_Values[DataNames.恆溫溫度_3] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_4
        {
            get => Recipe_Values[DataNames.恆溫溫度_4];
            set
            {
                Recipe_Values[DataNames.恆溫溫度_4] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_5
        {
            get => Recipe_Values[DataNames.恆溫溫度_5];
            set
            {
                Recipe_Values[DataNames.恆溫溫度_5] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_6
        {
            get => Recipe_Values[DataNames.恆溫溫度_6];
            set
            {
                Recipe_Values[DataNames.恆溫溫度_6] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_7
        {
            get => Recipe_Values[DataNames.恆溫溫度_7];
            set
            {
                Recipe_Values[DataNames.恆溫溫度_7] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double ThermostaticTemperature_8
        {
            get => Recipe_Values[DataNames.恆溫溫度_8];
            set
            {
                Recipe_Values[DataNames.恆溫溫度_8] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double HeatingTime_1
        {
            get => Recipe_Values[DataNames.升溫時間_1] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫時間_1] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingTime_2
        {
            get => Recipe_Values[DataNames.升溫時間_2] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫時間_2] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingTime_3
        {
            get => Recipe_Values[DataNames.升溫時間_3] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫時間_3] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingTime_4
        {
            get => Recipe_Values[DataNames.升溫時間_4] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫時間_4] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingTime_5
        {
            get => Recipe_Values[DataNames.升溫時間_5] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫時間_5] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingTime_6
        {
            get => Recipe_Values[DataNames.升溫時間_6] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫時間_6] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingTime_7
        {
            get => Recipe_Values[DataNames.升溫時間_7] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫時間_7] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingTime_8
        {
            get => Recipe_Values[DataNames.升溫時間_8] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫時間_8] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingAlarm_1
        {
            get => Recipe_Values[DataNames.升溫警報_1] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫警報_1] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingAlarm_2
        {
            get => Recipe_Values[DataNames.升溫警報_2] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫警報_2] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingAlarm_3
        {
            get => Recipe_Values[DataNames.升溫警報_3] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫警報_3] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingAlarm_4
        {
            get => Recipe_Values[DataNames.升溫警報_4] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫警報_4] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingAlarm_5
        {
            get => Recipe_Values[DataNames.升溫警報_5] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫警報_5] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingAlarm_6
        {
            get => Recipe_Values[DataNames.升溫警報_6] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫警報_6] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingAlarm_7
        {
            get => Recipe_Values[DataNames.升溫警報_7] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫警報_7] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double HeatingAlarm_8
        {
            get => Recipe_Values[DataNames.升溫警報_8] * 0.1;
            set
            {
                Recipe_Values[DataNames.升溫警報_8] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingTime_1
        {
            get => Recipe_Values[DataNames.恆溫時間_1] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫時間_1] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingTime_2
        {
            get => Recipe_Values[DataNames.恆溫時間_2] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫時間_2] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingTime_3
        {
            get => Recipe_Values[DataNames.恆溫時間_3] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫時間_3] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingTime_4
        {
            get => Recipe_Values[DataNames.恆溫時間_4] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫時間_4] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingTime_5
        {
            get => Recipe_Values[DataNames.恆溫時間_5] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫時間_5] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingTime_6
        {
            get => Recipe_Values[DataNames.恆溫時間_6] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫時間_6] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingTime_7
        {
            get => Recipe_Values[DataNames.恆溫時間_7] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫時間_7] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingTime_8
        {
            get => Recipe_Values[DataNames.恆溫時間_8] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫時間_8] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingAlarm_1
        {
            get => Recipe_Values[DataNames.恆溫警報_1] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫警報_1] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingAlarm_2
        {
            get => Recipe_Values[DataNames.恆溫警報_2] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫警報_2] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingAlarm_3
        {
            get => Recipe_Values[DataNames.恆溫警報_3] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫警報_3] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingAlarm_4
        {
            get => Recipe_Values[DataNames.恆溫警報_4] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫警報_4] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingAlarm_5
        {
            get => Recipe_Values[DataNames.恆溫警報_5] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫警報_5] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingAlarm_6
        {
            get => Recipe_Values[DataNames.恆溫警報_6] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫警報_6] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingAlarm_7
        {
            get => Recipe_Values[DataNames.恆溫警報_7] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫警報_7] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double WarmingAlarm_8
        {
            get => Recipe_Values[DataNames.恆溫警報_8] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫警報_8] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public short CoolingTemperature
        {
            get => Recipe_Values[DataNames.降溫溫度];
            set
            {
                Recipe_Values[DataNames.降溫溫度] = value;
                NotifyPropertyChanged();
            }
        }

        public double CoolingTime
        {
            get => Recipe_Values[DataNames.降溫時間] * 0.1;
            set
            {
                Recipe_Values[DataNames.降溫時間] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double InflatingTime
        {
            get => Recipe_Values[DataNames.充氣時間];
            set
            {
                Recipe_Values[DataNames.充氣時間] = (short)value;
                NotifyPropertyChanged();
            }
        }

        public double ProgramStopAlarmTime
        {
            get => Recipe_Values[DataNames.程式結束警報時間] * 0.1;
            set
            {
                Recipe_Values[DataNames.程式結束警報時間] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public short UsedSegmentCounts
        {
            get => Recipe_Values[DataNames.使用段數];
            set
            {
                Recipe_Values[DataNames.使用段數] = value;
                NotifyPropertyChanged();
            }
        }

        #endregion 生產配方

        #region 警告

        public bool ProgramStop                     => Bit_Values[SignalNames.程式結束];
        public bool DoorNotClosed                   => Bit_Values[SignalNames.加熱門未關];
        public bool EmergencyStop                   => Bit_Values[SignalNames.緊急停止];
        public bool LowTemperature                  => Bit_Values[SignalNames.溫控器低溫異常];
        public bool PowerInversion                  => Bit_Values[SignalNames.電源反相];
        public bool OTP_TemperatureError            => Bit_Values[SignalNames.OTP超溫異常];
        public bool CirculatingFanOverload          => Bit_Values[SignalNames.循環風車過載];
        public bool CoolingFanException             => Bit_Values[SignalNames.冷卻進氣風車異常];
        public bool CoolingFanCurrentException      => Bit_Values[SignalNames.冷卻進氣風車電流異常];
        public bool OverTemperatureAlarm            => Bit_Values[SignalNames.超溫警報];
        public bool DoorNotOpen                     => Bit_Values[SignalNames.停止後未開門];
        public bool CirculatingFanInversion         => Bit_Values[SignalNames.循環風車INV異常];
        public bool CirculatingFanCurrentException  => Bit_Values[SignalNames.循環風車電流異常];
        public bool InflatingTimeExceeded           => Bit_Values[SignalNames.充氮氣逾時];
        public bool DoorNotClosed_PositionException => Bit_Values[SignalNames.門未關定位異常];
        public bool HeatingTimeExceeded             => Bit_Values[SignalNames.升恆溫逾時];
        public bool HeatingBranchException          => Bit_Values[SignalNames.加熱分路跳脫];
        public bool ThermostatException             => Bit_Values[SignalNames.溫控器異常];
        public bool CommunicationException          => Bit_Values[SignalNames.通訊異常];
        public bool TemperatureWriteError           => Bit_Values[SignalNames.寫入溫度比對異常];

        #endregion 警告

        #region 機台狀態

        public bool   Buzzer                   => Bit_Values[SignalNames.蜂鳴器];
        public bool   GreenLight               => Bit_Values[SignalNames.綠燈];
        public bool   YellowLight              => Bit_Values[SignalNames.黃燈];
        public bool   RedLight                 => Bit_Values[SignalNames.紅燈];
        public bool   IsCooling                => Bit_Values[SignalNames.降溫中];
        public bool   ManualMode               => Bit_Values[SignalNames.手動模式];
        public bool   AutoMode                 => Bit_Values[SignalNames.自動模式];
        public bool   AutoMode_Stop            => Bit_Values[SignalNames.自動停止];
        public bool   AutoMode_Start           => Bit_Values[SignalNames.自動啟動];
        public bool   PC_InUsed                => Bit_Values[SignalNames.PC_InUsed];
        public double ThermostatTemperature    => Data_Values[DataNames.溫控器實際溫度];
        public double ThermostatTemperatureSet => Data_Values[DataNames.溫控器設定溫度];
        public short  OvenTemperature_1        => Data_Values[DataNames.爐內溫度_1];
        public short  OvenTemperature_2        => Data_Values[DataNames.爐內溫度_2];
        public short  OvenTemperature_3        => Data_Values[DataNames.爐內溫度_3];
        public short  OvenTemperature_4        => Data_Values[DataNames.爐內溫度_4];
        public short  OvenTemperature_5        => Data_Values[DataNames.爐內溫度_5];
        public short  OvenTemperature_6        => Data_Values[DataNames.爐內溫度_6];
        public short  OvenTemperature_7        => Data_Values[DataNames.爐內溫度_7];
        public short  OvenTemperature_8        => Data_Values[DataNames.爐內溫度_8];
        public short  Segment_RemainingTime    => Data_Values[DataNames.片段剩餘時間];
        public short  Total_RemainingTime      => Data_Values[DataNames.總剩餘時間];
        public short  CurrentSegment           => Data_Values[DataNames.目前段數];

        #endregion 機台狀態
    }
}