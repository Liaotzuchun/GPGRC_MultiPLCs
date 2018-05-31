using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class MainWindow_ViewModel : ViewModelBase
    {
        private int _ViewIndex;

        public int ViewIndex
        {
            get => _ViewIndex;
            set
            {
                _ViewIndex = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindow_ViewModel()
        {

        }
    }
}