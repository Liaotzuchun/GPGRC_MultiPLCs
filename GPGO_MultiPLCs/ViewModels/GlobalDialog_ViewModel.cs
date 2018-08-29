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
    /// <summary>實作IDialogService，負責所有對話視窗</summary>
    public sealed class GlobalDialog_ViewModel : ObservableObject, IDialogService, IDisposable
    {
        public async Task<bool> Show(Dictionary<Language, string> msg, object obj, bool support_cancel, TimeSpan delay = default(TimeSpan))
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
            ObjectPropertiesView = obj.ToDictionary(Language);

            IsShown_1 = Visibility.Visible;

            await Task.Factory.StartNew(() =>
                                        {
                                            Lock_1.Reset();
                                            if (delay == TimeSpan.Zero)
                                            {
                                                Lock_1.WaitOne();
                                            }
                                            else
                                            {
                                                Lock_1.WaitOne(delay);
                                            }
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown_1 = Visibility.Collapsed;
            Message_1 = "";
            ObjectPropertiesView = null;

            return EnterResult_1;
        }

        public async Task<bool> Show(Dictionary<Language, string> msg, bool support_cancel, TimeSpan delay = default(TimeSpan))
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
                                            Lock_1.WaitOne(delay == TimeSpan.Zero ? TimeSpan.FromSeconds(15) : delay);
                                        },
                                        TaskCreationOptions.LongRunning);

            IsShown_1 = Visibility.Collapsed;
            Message_1 = "";

            return EnterResult_1;
        }

        public async Task Show(Dictionary<Language, string> msg, TimeSpan delay = default(TimeSpan), DialogMsgType type = DialogMsgType.Normal)
        {
            await Task.Factory.StartNew(() =>
                                        {
                                            var m = new ShowingMessage(msg.TryGetValue(Language, out var val) ? val : msg.Values.First(), type);
                                            var tag = DateTime.Now.Ticks;
                                            Msgs.Add(tag, m);
                                            Thread.Sleep(delay == TimeSpan.Zero ? TimeSpan.FromSeconds(1) : delay);
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
            Message_2 = "";
            TitleHeader = "";

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
                                                    var (result, title_msg) = condition(Intput);

                                                    if (EnterResult_2)
                                                    {
                                                        ConditionResult = result;
                                                    }

                                                    if (ConditionResult != null && EnterResult_2 && !ConditionResult.Value)
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
            Message_2 = "";
            TitleHeader = "";

            return (ConditionResult != null && EnterResult_2 && ConditionResult.Value, Intput);
        }

        public void Dispose()
        {
            Lock_1.Dispose();
            Lock_2.Dispose();
        }

        public Language Language;

        private readonly ManualResetEvent Lock_1;
        private readonly ManualResetEvent Lock_2;

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
            get => Get<bool?>();
            set => Set(value);
        }

        public bool EnterEnable
        {
            get => Get<bool>();
            set => Set(value);
        }

        public string Intput
        {
            get => Get<string>();
            set
            {
                value = value.Trim().Replace(" ", "_");
                Set(value);
            }
        }

        public Visibility IsShown_1
        {
            get => Get<Visibility>();
            set => Set(value);
        }

        public Visibility IsShown_2
        {
            get => Get<Visibility>();
            set => Set(value);
        }

        public string Message_1
        {
            get => Get<string>();
            set => Set(value);
        }

        public string Message_2
        {
            get => Get<string>();
            set => Set(value);
        }

        public Dictionary<string, object> ObjectPropertiesView
        {
            get => Get<Dictionary<string, object>>();
            set => Set(value);
        }

        public bool SupportCancel
        {
            get => Get<bool>();
            set => Set(value);
        }

        public string Title
        {
            get => Get<string>();
            set => Set(value);
        }

        public string TitleHeader
        {
            get => Get<string>();
            set => Set(value);
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
            IsShown_1 = Visibility.Collapsed;
            IsShown_2 = Visibility.Collapsed;
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