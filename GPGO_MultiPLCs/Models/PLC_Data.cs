using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    public class PLC_Data : ViewModelBase
    {
        private bool _IsProcessing;
        private bool _OnlineStatus;
        private double _NowTemperature;
        public bool IsRecording;
        public CancellationTokenSource CTS;
        public TwoKeyDictionary<DataNames, int, short> D_Values;
        public TwoKeyDictionary<SignalNames, int, bool> M_Values;

        private readonly Stopwatch sw = new Stopwatch();

        public bool OnlineStatus
        {
            get => _OnlineStatus;
            set
            {
                _OnlineStatus = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsProcessing
        {
            get => _IsProcessing;
            set
            {
                _IsProcessing = value;
                NotifyPropertyChanged();
            }
        }

        public double NowTemperature
        {
            get => _NowTemperature;
            set
            {
                _NowTemperature = value;
                NotifyPropertyChanged();
            }
        }

        public PLC_Data(Dictionary<SignalNames, string> M_MapList, Dictionary<DataNames, string> D_MapList)
        {
            M_Values = new TwoKeyDictionary<SignalNames, int, bool>();
            D_Values = new TwoKeyDictionary<DataNames, int, short>();

            foreach (var loc in M_MapList)
            {
                M_Values.Add(loc.Key, int.Parse(loc.Value.Substring(1)), false);
            }

            foreach (var loc in D_MapList)
            {
                D_Values.Add(loc.Key, int.Parse(loc.Value.Substring(1)), 0);
            }

            M_Values.UpdatedEvent += e =>
                                     {
                                     };

            D_Values.UpdatedEvent += e =>
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

        public async Task<List<double>> StartRecoder()
        {
            if (IsRecording)
            {
                return null;
            }

            IsRecording = true;
            var val = await Task.Factory.StartNew(() =>
                                                  {
                                                      var list = new List<double>();
                                                      var n = 0;
                                                      sw.Restart();

                                                      while (!CTS.IsCancellationRequested)
                                                      {
                                                          if (sw.ElapsedMilliseconds >= n * 60000)
                                                          {
                                                              list.Add(_NowTemperature);

                                                              n++;
                                                          }
                                                      }

                                                      sw.Stop();

                                                      return list;
                                                  },
                                                  TaskCreationOptions.LongRunning);

            return val;
        }
    }
}