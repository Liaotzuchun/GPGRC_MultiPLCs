using System.Windows;
using System.Windows.Threading;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class MainWindow_ViewModel : ViewModelBase
    {
        public delegate void LoadedEventHandeler(Dispatcher dp);

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
            LoadedCommand = new RelayCommand(e =>
                                             {
                                                 LoadedEvent?.Invoke(e as Dispatcher);
                                             });
        }
    }
}