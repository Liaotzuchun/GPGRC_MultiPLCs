using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class GlobalDialog_ViewModel : ViewModelBase, IDialogService<string>
    {
        public async Task<(bool result, string intput)> ShowWithIntput(string msg)
        {
            Result = false;
            SupportCancel = true;
            Message = msg;
            IsShown = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            Lock.WaitOne(30000);
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown = Visibility.Collapsed;

            return (Result, _Intput);
        }

        public async Task<(bool result, string intput)> ShowWithIntput(string msg, string condition)
        {
            Result = false;
            SupportCancel = true;
            Message = msg;
            IsShown = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            do
                                            {
                                                Lock.WaitOne(30000);
                                            } while (Result && _Intput != condition);
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown = Visibility.Collapsed;

            return (Result && _Intput == condition, _Intput);
        }

        public async Task<(bool result, string intput)> ShowWithIntput(string msg, IEnumerable<string> conditions)
        {
            Result = false;
            SupportCancel = true;
            Message = msg;
            IsShown = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            do
                                            {
                                                Lock.WaitOne(30000);
                                            } while (Result && conditions.All(x => x != _Intput));
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown = Visibility.Collapsed;

            return (Result && conditions.Any(x => x == _Intput), _Intput);
        }

        public async Task<bool> Show(string msg, bool support_cancel)
        {
            Result = false;
            SupportCancel = support_cancel;
            Message = msg;
            IsShown = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            Lock.WaitOne(9000);
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown = Visibility.Collapsed;

            return Result;
        }

        public async Task Show(string msg, TimeSpan delay)
        {
            Message = msg;
            IsShown = Visibility.Visible;

            await Task.Delay(delay);

            IsShown = Visibility.Collapsed;
        }

        private readonly AutoResetEvent Lock;
        private bool _EnterEnable;
        private string _Intput;
        private Visibility _IsShown = Visibility.Collapsed;
        private string _Message;
        private bool Result;

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

        public string Intput
        {
            get => _Intput;
            set
            {
                _Intput = value;
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

        public bool EnterEnable
        {
            get => _EnterEnable;
            set
            {
                _EnterEnable = value;
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