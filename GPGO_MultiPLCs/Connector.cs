using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPGO_MultiPLCs.ViewModels;

namespace GPGO_MultiPLCs
{
    public class Connector
    {
        public MainWindow_ViewModel MainVM { get; }
        public TotalView_ViewModel TotalVM { get; }

        public Connector()
        {
            MainVM = new MainWindow_ViewModel();
            TotalVM = new TotalView_ViewModel();
        }
    }
}
