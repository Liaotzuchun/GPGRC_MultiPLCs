using System.Windows.Threading;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class MainWindow_ViewModel : ViewModelBase
    {
        public delegate void LoadedEventHandeler(Dispatcher dp);

        public delegate void IndexChangedHandeler(int index);

        private int _ViewIndex;

        public RelayCommand LoadedCommand { get; }

        public int ViewIndex
        {
            get => _ViewIndex;
            set
            {
                _ViewIndex = value;
                NotifyPropertyChanged();
                IndexChangedEvent?.Invoke(_ViewIndex);
            }
        }

        public event LoadedEventHandeler LoadedEvent;
        public event IndexChangedHandeler IndexChangedEvent;

        public MainWindow_ViewModel()
        {
            LoadedCommand = new RelayCommand(e =>
                                             {
                                                 LoadedEvent?.Invoke(e as Dispatcher);
                                             });
        }
    }
}