using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class GlobalDialog_ViewModel : ViewModelBase, IDialogService<string>
    {
        public async Task<bool> Show(string msg, bool support_cancel, DialogMsgType type = DialogMsgType.Normal)
        {
            MsgType = type;
            EnterResult = false;
            SupportCancel = support_cancel;
            NoButton = false;
            WithIntput = false;
            Message = msg;
            IsShown = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            Lock.WaitOne(9000);
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown = Visibility.Collapsed;

            return EnterResult;
        }

        public async Task Show(string msg, TimeSpan delay, DialogMsgType type = DialogMsgType.Normal)
        {
            MsgType = type;
            Message = msg;
            NoButton = true;
            WithIntput = false;
            IsShown = Visibility.Visible;

            await Task.Delay(delay);

            IsShown = Visibility.Collapsed;
        }

        public async Task<(bool result, string intput)> ShowWithIntput(string msg, string header)
        {
            Intput = "";
            ConditionResult = null;
            EnterResult = false;
            SupportCancel = true;
            NoButton = false;
            WithIntput = true;
            Message = msg;
            TitleHeader = header;
            IsShown = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            Lock.WaitOne(30000);
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown = Visibility.Collapsed;

            return (EnterResult, Intput);
        }

        public async Task<(bool result, string intput)> ShowWithIntput(string msg, string header, Func<string, (bool result, string title_msg)> condition)
        {
            Title = "";
            Intput = "";
            ConditionResult = null;
            EnterResult = false;
            SupportCancel = true;
            WithIntput = true;
            Message = msg;
            TitleHeader = header;
            IsShown = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            while (true)
                                            {
                                                if (Lock.WaitOne(30000))
                                                {
                                                    var (result, title_msg) = condition(_Intput);

                                                    if (EnterResult)
                                                    {
                                                        ConditionResult = result;
                                                    }

                                                    if (_ConditionResult != null && EnterResult && !_ConditionResult.Value)
                                                    {
                                                        Title = title_msg;
                                                        Intput = "";
                                                    }
                                                    else
                                                    {
                                                        Title = "";

                                                        if (EnterResult)
                                                        {
                                                            Thread.Sleep(450);
                                                        }

                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown = Visibility.Collapsed;

            return (_ConditionResult != null && EnterResult && _ConditionResult.Value, Intput);
        }

        private readonly AutoResetEvent Lock;
        private bool? _ConditionResult;
        private bool _EnterEnable;
        private string _Intput = "";
        private Visibility _IsShown = Visibility.Collapsed;
        private string _Message;
        private DialogMsgType _MsgType;
        private bool _NoButton;
        private bool _SupportCancel;
        private string _Title = "";
        private string _TitleHeader = "";
        private bool _WithIntput;

        private bool EnterResult;

        public RelayCommand CancelCommand { get; }

        public RelayCommand EnterCommand { get; }

        public RelayCommand OkayCommand { get; }

        public bool? ConditionResult
        {
            get => _ConditionResult;
            set
            {
                _ConditionResult = value;
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

        public string Intput
        {
            get => _Intput;
            set
            {
                value = value.Trim().Replace(" ", "_");
                _Intput = value;
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

        public string Message
        {
            get => _Message;
            set
            {
                _Message = value;
                NotifyPropertyChanged();
            }
        }

        public DialogMsgType MsgType
        {
            get => _MsgType;
            set
            {
                _MsgType = value;
                NotifyPropertyChanged();
            }
        }

        public bool NoButton
        {
            get => _NoButton;
            set
            {
                _NoButton = value;
                NotifyPropertyChanged();
            }
        }

        public bool SupportCancel
        {
            get => _SupportCancel;
            set
            {
                _SupportCancel = value;
                NotifyPropertyChanged();
            }
        }

        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                NotifyPropertyChanged();
            }
        }

        public string TitleHeader
        {
            get => _TitleHeader;
            set
            {
                _TitleHeader = value;
                NotifyPropertyChanged();
            }
        }

        public bool WithIntput
        {
            get => _WithIntput;
            set
            {
                _WithIntput = value;
                NotifyPropertyChanged();
            }
        }

        public GlobalDialog_ViewModel()
        {
            Lock = new AutoResetEvent(false);

            OkayCommand = new RelayCommand(e =>
                                           {
                                               EnterResult = true;
                                               Lock.Set();
                                           });

            CancelCommand = new RelayCommand(e =>
                                             {
                                                 EnterResult = false;
                                                 Lock.Set();
                                             });

            EnterCommand = new RelayCommand(e =>
                                            {
                                                var args = (KeyEventArgs)e;
                                                if (args.Key == Key.Enter || args.Key == Key.Return)
                                                {
                                                    EnterResult = true;
                                                    Lock.Set();
                                                }
                                            });
        }
    }
}