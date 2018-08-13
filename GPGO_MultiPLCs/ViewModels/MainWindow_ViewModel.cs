using System;
using System.Windows.Threading;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class MainWindow_ViewModel : ObservableObject
    {
        public RelayCommand LoadedCommand { get; }

        public int ViewIndex
        {
            get => Get<int>();
            set
            {
                Set(value);
                IndexChangedEvent?.Invoke(value);
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