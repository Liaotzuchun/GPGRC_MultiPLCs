using System;
using System.Windows.Threading;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class MainWindow_ViewModel : ViewModelBase
    {
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

        public event Action<int> IndexChangedEvent;

        public event Action<Dispatcher> LoadedEvent;

        public MainWindow_ViewModel()
        {
            LoadedCommand = new RelayCommand(e =>
                                             {
                                                 LoadedEvent?.Invoke(e as Dispatcher);
                                             });
        }
    }
}