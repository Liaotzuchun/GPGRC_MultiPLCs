using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>主視窗</summary>
    public class MainWindow_ViewModel : ObservableObject
    {
        public RelayCommand LoadedCommand { get; }
        public RelayCommand ClosingCommand { get; }

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

        public event Func<bool> CheckClosing;

        public MainWindow_ViewModel()
        {
            LoadedCommand = new RelayCommand(e =>
                                             {
                                                 LoadedEvent?.Invoke(e as Dispatcher);
                                             });

            ClosingCommand = new RelayCommand(e =>
                                              {
                                                  if (!(e is CancelEventArgs ce) || CheckClosing == null)
                                                  {
                                                      return;
                                                  }

                                                  if(!CheckClosing.Invoke())
                                                  {
                                                      ce.Cancel = true;
                                                  }
                                              });
        }
    }
}