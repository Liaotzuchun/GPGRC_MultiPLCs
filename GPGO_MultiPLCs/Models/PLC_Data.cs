using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    public class PLC_Data : ViewModelBase
    {
        private readonly Stopwatch sw = new Stopwatch();
        private bool _OnlineStatus;
        public int StationNumber { get; }
        public CancellationTokenSource CTS;
        public TwoKeyDictionary<DataNames, int, short> D_Values;
        public bool IsRecording;
        public TwoKeyDictionary<SignalNames, int, bool> M_Values;
        public TwoKeyDictionary<DataNames, int, short> Recipe_Values;

        public ProcessInfo Process_Info { get; }

        public bool OnlineStatus
        {
            get => _OnlineStatus;
            set
            {
                _OnlineStatus = value;
                NotifyPropertyChanged();
            }
        }

        public PLC_Data(int index, Dictionary<SignalNames, int> M_MapList, Dictionary<DataNames, int> D_MapList, Dictionary<DataNames, int> Recipe_MapList)
        {
            StationNumber = index;
            Process_Info = new ProcessInfo();
            M_Values = new TwoKeyDictionary<SignalNames, int, bool>();
            D_Values = new TwoKeyDictionary<DataNames, int, short>();
            Recipe_Values = new TwoKeyDictionary<DataNames, int, short>();

            foreach (var loc in M_MapList)
            {
                M_Values.Add(loc.Key, loc.Value, false);
            }

            foreach (var loc in D_MapList)
            {
                D_Values.Add(loc.Key, loc.Value, 0);
            }

            foreach (var loc in Recipe_MapList)
            {
                Recipe_Values.Add(loc.Key, loc.Value, 0);
            }

            M_Values.Key1UpdatedEvent += (key, val) =>
                                         {
                                         };

            D_Values.Key1UpdatedEvent += (key, val) =>
                                         {
                                         };
        }

        public void ResetStopTokenSource()
        {
            CTS?.Dispose();

            CTS = new CancellationTokenSource();
            //CTS.Token.Register(() =>
            //{
            //
            //});
        }

        public async Task<List<Record_Info>> StartRecoder(long cycle_ms, CancellationToken ct)
        {
            if (IsRecording)
            {
                return null;
            }

            IsRecording = true;
            var val = await Task.Factory.StartNew(() =>
                                                  {
                                                      var list = new List<Record_Info>();
                                                      var n = 0;
                                                      sw.Restart();

                                                      while (!ct.IsCancellationRequested)
                                                      {
                                                          if (sw.ElapsedMilliseconds >= n * cycle_ms)
                                                          {
                                                              list.Add(new Record_Info
                                                                       {
                                                                           ThermostatTemperature = ThermostatTemperature,
                                                                           OvenTemperature_1 = OvenTemperature_1,
                                                                           OvenTemperature_2 = OvenTemperature_2,
                                                                           OvenTemperature_3 = OvenTemperature_3,
                                                                           OvenTemperature_4 = OvenTemperature_4,
                                                                           OvenTemperature_5 = OvenTemperature_5,
                                                                           OvenTemperature_6 = OvenTemperature_6,
                                                                           OvenTemperature_7 = OvenTemperature_7,
                                                                           OvenTemperature_8 = OvenTemperature_8
                                                                       });

                                                              n++;
                                                          }

                                                          Thread.Sleep(1);
                                                      }

                                                      sw.Stop();

                                                      return list;
                                                  },
                                                  TaskCreationOptions.LongRunning);

            return val;
        }

        #region 生產配方

        public string RecipeName
        {
            get => new[]
                   {
                       Recipe_Values[DataNames.配方名稱_01],
                       Recipe_Values[DataNames.配方名稱_02],
                       Recipe_Values[DataNames.配方名稱_03],
                       Recipe_Values[DataNames.配方名稱_04],
                       Recipe_Values[DataNames.配方名稱_05],
                       Recipe_Values[DataNames.配方名稱_06],
                       Recipe_Values[DataNames.配方名稱_07],
                       Recipe_Values[DataNames.配方名稱_08],
                       Recipe_Values[DataNames.配方名稱_09],
                       Recipe_Values[DataNames.配方名稱_10],
                       Recipe_Values[DataNames.配方名稱_11],
                       Recipe_Values[DataNames.配方名稱_12],
                       Recipe_Values[DataNames.配方名稱_13]
                   }.ASCIIfromShorts();
            set
            {
                if (value.Length > 26)
                {
                    value = value.Substring(0, 26);
                }

                var vals = value.PadLeft(26).ASCIItoShorts();

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

                NotifyPropertyChanged(nameof(RecipeName));
            }
        }

        public double TargetTemperature_1
        {
            get => Recipe_Values[DataNames.目標溫度_1] * 0.1;
            set
            {
                Recipe_Values[DataNames.目標溫度_1] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_2
        {
            get => Recipe_Values[DataNames.目標溫度_2] * 0.1;
            set
            {
                Recipe_Values[DataNames.目標溫度_2] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_3
        {
            get => Recipe_Values[DataNames.目標溫度_3] * 0.1;
            set
            {
                Recipe_Values[DataNames.目標溫度_3] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_4
        {
            get => Recipe_Values[DataNames.目標溫度_4] * 0.1;
            set
            {
                Recipe_Values[DataNames.目標溫度_4] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_5
        {
            get => Recipe_Values[DataNames.目標溫度_5] * 0.1;
            set
            {
                Recipe_Values[DataNames.目標溫度_5] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_6
        {
            get => Recipe_Values[DataNames.目標溫度_6] * 0.1;
            set
            {
                Recipe_Values[DataNames.目標溫度_6] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_7
        {
            get => Recipe_Values[DataNames.目標溫度_7] * 0.1;
            set
            {
                Recipe_Values[DataNames.目標溫度_7] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double TargetTemperature_8
        {
            get => Recipe_Values[DataNames.目標溫度_8] * 0.1;
            set
            {
                Recipe_Values[DataNames.目標溫度_8] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double ConstantTemperature_1
        {
            get => Recipe_Values[DataNames.恆溫溫度_1] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫溫度_1] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double ConstantTemperature_2
        {
            get => Recipe_Values[DataNames.恆溫溫度_2] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫溫度_2] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double ConstantTemperature_3
        {
            get => Recipe_Values[DataNames.恆溫溫度_3] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫溫度_3] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double ConstantTemperature_4
        {
            get => Recipe_Values[DataNames.恆溫溫度_4] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫溫度_4] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double ConstantTemperature_5
        {
            get => Recipe_Values[DataNames.恆溫溫度_5] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫溫度_5] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double ConstantTemperature_6
        {
            get => Recipe_Values[DataNames.恆溫溫度_6] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫溫度_6] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double ConstantTemperature_7
        {
            get => Recipe_Values[DataNames.恆溫溫度_7] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫溫度_7] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public double ConstantTemperature_8
        {
            get => Recipe_Values[DataNames.恆溫溫度_8] * 0.1;
            set
            {
                Recipe_Values[DataNames.恆溫溫度_8] = (short)(value * 10);
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_1
        {
            get => Recipe_Values[DataNames.升溫時間_1];
            set
            {
                Recipe_Values[DataNames.升溫時間_1] = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_2
        {
            get => Recipe_Values[DataNames.升溫時間_2];
            set
            {
                Recipe_Values[DataNames.升溫時間_2] = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_3
        {
            get => Recipe_Values[DataNames.升溫時間_3];
            set
            {
                Recipe_Values[DataNames.升溫時間_3] = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_4
        {
            get => Recipe_Values[DataNames.升溫時間_4];
            set
            {
                Recipe_Values[DataNames.升溫時間_4] = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_5
        {
            get => Recipe_Values[DataNames.升溫時間_5];
            set
            {
                Recipe_Values[DataNames.升溫時間_5] = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_6
        {
            get => Recipe_Values[DataNames.升溫時間_6];
            set
            {
                Recipe_Values[DataNames.升溫時間_6] = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_7
        {
            get => Recipe_Values[DataNames.升溫時間_7];
            set
            {
                Recipe_Values[DataNames.升溫時間_7] = value;
                NotifyPropertyChanged();
            }
        }

        public short HeatingTime_8
        {
            get => Recipe_Values[DataNames.升溫時間_8];
            set
            {
                Recipe_Values[DataNames.升溫時間_8] = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_1
        {
            get => Recipe_Values[DataNames.恆溫時間_1];
            set
            {
                Recipe_Values[DataNames.恆溫時間_1] = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_2
        {
            get => Recipe_Values[DataNames.恆溫時間_2];
            set
            {
                Recipe_Values[DataNames.恆溫時間_2] = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_3
        {
            get => Recipe_Values[DataNames.恆溫時間_3];
            set
            {
                Recipe_Values[DataNames.恆溫時間_3] = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_4
        {
            get => Recipe_Values[DataNames.恆溫時間_4];
            set
            {
                Recipe_Values[DataNames.恆溫時間_4] = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_5
        {
            get => Recipe_Values[DataNames.恆溫時間_5];
            set
            {
                Recipe_Values[DataNames.恆溫時間_5] = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_6
        {
            get => Recipe_Values[DataNames.恆溫時間_6];
            set
            {
                Recipe_Values[DataNames.恆溫時間_6] = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_7
        {
            get => Recipe_Values[DataNames.恆溫時間_7];
            set
            {
                Recipe_Values[DataNames.恆溫時間_7] = value;
                NotifyPropertyChanged();
            }
        }

        public short ConstantTime_8
        {
            get => Recipe_Values[DataNames.恆溫時間_8];
            set
            {
                Recipe_Values[DataNames.恆溫時間_8] = value;
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

        public short InflatingTime
        {
            get => Recipe_Values[DataNames.充氣時間];
            set
            {
                Recipe_Values[DataNames.充氣時間] = value;
                NotifyPropertyChanged();
            }
        }

        public short UsedNumbers
        {
            get => Recipe_Values[DataNames.使用段數];
            set
            {
                Recipe_Values[DataNames.使用段數] = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region 警告

        public bool ProgramStop => M_Values[SignalNames.程式結束];
        public bool DoorNotClosed => M_Values[SignalNames.加熱門未關];
        public bool EmergencyStopd => M_Values[SignalNames.緊急停止];
        public bool LowTemperature => M_Values[SignalNames.溫控器低溫異常];
        public bool PowerInversion => M_Values[SignalNames.電源反相];
        public bool OTP_TemperatureError => M_Values[SignalNames.OTP超溫異常];
        public bool CirculatingFanOverload => M_Values[SignalNames.循環風車過載];
        public bool CoolingFanAbnormal => M_Values[SignalNames.冷卻進氣風車異常];
        public bool OverTemperatureAlarm => M_Values[SignalNames.超溫警報];
        public bool DoorNotOpen => M_Values[SignalNames.停止後未開門];
        public bool CirculatingFanInversion => M_Values[SignalNames.循環風車INV異常];
        public bool FillingTimeExceeded => M_Values[SignalNames.充氮氣逾時];
        public bool DoorNotClosed_AbnormalPositioning => M_Values[SignalNames.門未關定位異常];
        public bool HeatingTimeExceeded => M_Values[SignalNames.升恆溫逾時];

        #endregion

        #region 機台狀態

        public bool ManualMode => M_Values[SignalNames.手動模式];
        public bool AutoMode => M_Values[SignalNames.自動模式];
        public bool AutoMode_Stop => M_Values[SignalNames.自動停止];
        public bool AutoMode_Start => M_Values[SignalNames.自動啟動];
        public bool PC_ByPass => M_Values[SignalNames.PC_ByPass];
        public double ThermostatTemperature => D_Values[DataNames.溫控器溫度] * 0.1;
        public short OvenTemperature_1 => D_Values[DataNames.爐內溫度_1];
        public short OvenTemperature_2 => D_Values[DataNames.爐內溫度_2];
        public short OvenTemperature_3 => D_Values[DataNames.爐內溫度_3];
        public short OvenTemperature_4 => D_Values[DataNames.爐內溫度_4];
        public short OvenTemperature_5 => D_Values[DataNames.爐內溫度_5];
        public short OvenTemperature_6 => D_Values[DataNames.爐內溫度_6];
        public short OvenTemperature_7 => D_Values[DataNames.爐內溫度_7];
        public short OvenTemperature_8 => D_Values[DataNames.爐內溫度_8];
        public short Fragment_RemainingTime => D_Values[DataNames.片段剩餘時間];
        public short Total_RemainingTime => D_Values[DataNames.總剩餘時間];

        #endregion
    }
}