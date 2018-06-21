using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class MainWindow_ViewModel : ViewModelBase
    {
        public delegate void LoadedEventHandeler();

        private int _ViewIndex;

        public RelayCommand LoadedCommand { get; }

        public int ViewIndex
        {
            get => _ViewIndex;
            set
            {
                _ViewIndex = value;
                NotifyPropertyChanged();
            }
        }

        public event LoadedEventHandeler LoadedEvent;

        public MainWindow_ViewModel()
        {
            LoadedCommand = new RelayCommand(_ =>
                                             {
                                                 LoadedEvent?.Invoke();
                                             });
        }
    }
}