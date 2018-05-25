using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class GlobalDialog_ViewModel : ViewModelBase, IDialogService
    {
        public async Task<bool> Show(string msg, bool support_cancel)
        {
            SupportCancel = support_cancel;
            Message = msg;
            IsShown = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            Result = false;
                                            Lock.WaitOne(9000);
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown = Visibility.Collapsed;

            return _Result;
        }

        public async Task Show(string msg, TimeSpan delay)
        {
            Message = msg;
            IsShown = Visibility.Visible;

            await Task.Delay(delay);

            IsShown = Visibility.Collapsed;
        }

        private readonly AutoResetEvent Lock;
        private Visibility _IsShown = Visibility.Collapsed;
        private string _Message;
        private bool _Result;

        private bool _SupportCancel;

        public bool SupportCancel
        {
            get => _SupportCancel;
            set
            {
                _SupportCancel = value;
                NotifyPropertyChanged();
            }
        }

        public bool Result
        {
            get => _Result;
            set
            {
                _Result = value;
                NotifyPropertyChanged();
            }
        }

        public string Message
        {
            get => _Message;
            set
            {
                _Message = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility IsShown
        {
            get => _IsShown;
            set
            {
                _IsShown = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand OkayCommand { get; }
        public RelayCommand CancelCommand { get; }

        public GlobalDialog_ViewModel()
        {
            Lock = new AutoResetEvent(false);

            OkayCommand = new RelayCommand(e =>
                                           {
                                               Result = true;
                                               Lock.Set();
                                           });

            CancelCommand = new RelayCommand(e =>
                                             {
                                                 Result = false;
                                                 Lock.Set();
                                             });
        }
    }
}