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