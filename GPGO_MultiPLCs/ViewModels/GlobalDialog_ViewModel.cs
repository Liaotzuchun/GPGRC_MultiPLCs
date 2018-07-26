using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>
    /// 實作IDialogService的對話窗
    /// </summary>
    public class GlobalDialog_ViewModel : ViewModelBase, IDialogService<string>
    {
        public async Task<bool> Show(Dictionary<Language, string> msg, bool support_cancel, DialogMsgType type = DialogMsgType.Normal)
        {
            if (!Lock_1.WaitOne(0))
            {
                EnterResult_1 = false;
                Lock_1.Set();

                await Task.Delay(30);
            }

            EnterResult_1 = false;
            SupportCancel = support_cancel;
            Message_1 = msg.TryGetValue(Language, out var val) ? val : msg.Values.First();
            IsShown_1 = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            Lock_1.Reset();
                                            Lock_1.WaitOne(12000);
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown_1 = Visibility.Collapsed;

            return EnterResult_1;
        }

        public async Task Show(Dictionary<Language, string> msg, TimeSpan delay, DialogMsgType type = DialogMsgType.Normal)
        {
            await Task.Factory.StartNew(() =>
                                        {
                                            var m = new ShowingMessage(msg.TryGetValue(Language, out var val) ? val : msg.Values.First(), type);
                                            var tag = DateTime.Now.Ticks;
                                            Msgs.Add(tag, m);
                                            Thread.Sleep(delay);
                                            Msgs.Remove(tag);
                                        },
                                        TaskCreationOptions.LongRunning);
        }

        public async Task<(bool result, string intput)> ShowWithIntput(Dictionary<Language, string> msg, Dictionary<Language, string> header)
        {
            if (!Lock_2.WaitOne(0))
            {
                EnterResult_2 = false;
                Lock_2.Set();

                await Task.Delay(30);
            }

            Intput = "";
            ConditionResult = null;
            EnterResult_2 = false;
            Message_2 = msg.TryGetValue(Language, out var val1) ? val1 : msg.Values.First();
            TitleHeader = header.TryGetValue(Language, out var val2) ? val2 : header.Values.First();
            IsShown_2 = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            Lock_2.Reset();
                                            Lock_2.WaitOne(30000);
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown_2 = Visibility.Collapsed;

            return (EnterResult_2, Intput);
        }

        public async Task<(bool result, string intput)> ShowWithIntput(Dictionary<Language, string> msg,
                                                                       Dictionary<Language, string> header,
                                                                       Func<string, (bool result, Dictionary<Language, string> title_msg)> condition)
        {
            if (!Lock_2.WaitOne(0))
            {
                EnterResult_2 = false;
                Lock_2.Set();

                await Task.Delay(30);
            }

            Title = "";
            Intput = "";
            ConditionResult = null;
            EnterResult_2 = false;
            Message_2 = msg.TryGetValue(Language, out var val1) ? val1 : msg.Values.First();
            TitleHeader = header.TryGetValue(Language, out var val2) ? val2 : header.Values.First();
            IsShown_2 = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            while (true)
                                            {
                                                Lock_2.Reset();
                                                if (Lock_2.WaitOne(30000))
                                                {
                                                    var (result, title_msg) = condition(_Intput);

                                                    if (EnterResult_2)
                                                    {
                                                        ConditionResult = result;
                                                    }

                                                    if (_ConditionResult != null && EnterResult_2 && !_ConditionResult.Value)
                                                    {
                                                        Title = title_msg.TryGetValue(Language, out var val3) ? val3 : title_msg.Values.First();
                                                        Intput = "";
                                                    }
                                                    else
                                                    {
                                                        Title = "";

                                                        if (EnterResult_2)
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

            IsShown_2 = Visibility.Collapsed;

            return (_ConditionResult != null && EnterResult_2 && _ConditionResult.Value, Intput);
        }

        public Language Language;

        private readonly ManualResetEvent Lock_1;
        private readonly ManualResetEvent Lock_2;
        private bool? _ConditionResult;
        private bool _EnterEnable;
        private string _Intput = "";
        private Visibility _IsShown_1 = Visibility.Collapsed;
        private Visibility _IsShown_2 = Visibility.Collapsed;
        private string _Message_1;
        private string _Message_2;
        private bool _SupportCancel;
        private string _Title = "";
        private string _TitleHeader = "";

        private bool EnterResult_1;
        private bool EnterResult_2;

        public RelayCommand CancelCommand_1 { get; }

        public RelayCommand CancelCommand_2 { get; }

        public RelayCommand EnterCommand { get; }

        public ObservableConcurrentDictionary<long, ShowingMessage> Msgs { get; }

        public RelayCommand OkayCommand_1 { get; }

        public RelayCommand OkayCommand_2 { get; }

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

        public Visibility IsShown_1
        {
            get => _IsShown_1;
            set
            {
                _IsShown_1 = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility IsShown_2
        {
            get => _IsShown_2;
            set
            {
                _IsShown_2 = value;
                NotifyPropertyChanged();
            }
        }

        public string Message_1
        {
            get => _Message_1;
            set
            {
                _Message_1 = value;
                NotifyPropertyChanged();
            }
        }

        public string Message_2
        {
            get => _Message_2;
            set
            {
                _Message_2 = value;
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

        public struct ShowingMessage
        {
            public string Msg { get; }
            public DialogMsgType Type { get; }

            public ShowingMessage(string msg, DialogMsgType type = DialogMsgType.Normal)
            {
                Msg = msg;
                Type = type;
            }
        }

        public GlobalDialog_ViewModel()
        {
            Lock_1 = new ManualResetEvent(true);
            Lock_2 = new ManualResetEvent(true);

            Msgs = new ObservableConcurrentDictionary<long, ShowingMessage>();

            OkayCommand_1 = new RelayCommand(e =>
                                             {
                                                 EnterResult_1 = true;
                                                 Lock_1.Set();
                                             });

            CancelCommand_1 = new RelayCommand(e =>
                                               {
                                                   EnterResult_1 = false;
                                                   Lock_1.Set();
                                               });

            OkayCommand_2 = new RelayCommand(e =>
                                             {
                                                 EnterResult_2 = true;
                                                 Lock_2.Set();
                                             });

            CancelCommand_2 = new RelayCommand(e =>
                                               {
                                                   EnterResult_2 = false;
                                                   Lock_2.Set();
                                               });

            EnterCommand = new RelayCommand(e =>
                                            {
                                                var args = (KeyEventArgs)e;
                                                if (args.Key == Key.Enter)
                                                {
                                                    EnterResult_2 = true;
                                                    Lock_2.Set();
                                                }
                                            });
        }
    }
}