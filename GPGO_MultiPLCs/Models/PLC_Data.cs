using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    public class PLC_Data : ViewModelBase
    {
        private bool _Status;

        public bool Status
        {
            get => _Status;
            set
            {
                _Status = value;
                NotifyPropertyChanged();
            }
        }

        public TwoKeyDictionary<SignalNames, int, bool> M_Values;
        public TwoKeyDictionary<DataNames, int, short> D_Values;

        public PLC_Data(Dictionary<SignalNames, string> M_MapList, Dictionary<DataNames, string>  D_MapList)
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
    }
}
