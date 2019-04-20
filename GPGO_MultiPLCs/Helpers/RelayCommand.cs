using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>提供可繫結的command</summary>
    public sealed class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            execute?.Invoke(parameter);
        }

        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        internal void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
    }

    /// <summary>提供實作DependencyObject可作為資源供繫結的command，若單純為DependencyObject會無法存取VisualTree，無法繫結DataContext來源，所以使用Freezable(最輕量)</summary>
    public sealed class DependencyCommand : Freezable, ICommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(DependencyCommand), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return Command?.CanExecute(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            Command?.Execute(parameter);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new DependencyCommand();
        }

        internal void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>替代InvokeCommandAction，可傳遞EventArgs</summary>
    public sealed class InteractiveCommand : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(InteractiveCommand), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        private string commandName;

        public string CommandName
        {
            get
            {
                ReadPreamble();

                return commandName;
            }
            set
            {
                if (CommandName != value)
                {
                    WritePreamble();
                    commandName = value;
                    WritePostscript();
                }
            }
        }

        protected override void Invoke(object parameter)
        {
            if (AssociatedObject != null)
            {
                var command = ResolveCommand();
                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }

        private ICommand ResolveCommand()
        {
            ICommand command = null;
            if (Command != null)
            {
                return Command;
            }

            if (AssociatedObject == null)
            {
                return null;
            }

            foreach (var info in AssociatedObject.GetType()
                                                 .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                 .Where(info => typeof(ICommand).IsAssignableFrom(info.PropertyType) && string.Equals(info.Name, CommandName, StringComparison.Ordinal)))
            {
                command = (ICommand)info.GetValue(AssociatedObject, null);
            }

            return command;
        }
    }

    /// <summary>提供能代入Function並提供Result存取的Command</summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CommandWithResult<T> : ObservableObject, ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Func<object, T> execute;
        private readonly Func<object, ValueTask<T>> execute_Task;

        public T Result
        {
            get => Get<T>();
            set
            {
                Set(value);
                ResultChanged?.Invoke(value);
            }
        }

        public event Action<T> ResultChanged;

        internal void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public CommandWithResult(Func<object, T> execute) : this(execute, null)
        {
        }

        public CommandWithResult(Func<object, T> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public CommandWithResult(Func<object, ValueTask<T>> execute_Task) : this(execute_Task, null)
        {
        }

        public CommandWithResult(Func<object, ValueTask<T>> execute_Task, Predicate<object> canExecute)
        {
            this.execute_Task = execute_Task;
            this.canExecute = canExecute;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        public async void Execute(object parameter)
        {
            if (execute != null)
            {
                Result = execute.Invoke(parameter);
            }
            else if (execute_Task != null)
            {
                Result = await execute_Task.Invoke(parameter);
            }
        }

        #endregion
    }

    /// <summary>Attached Behaviour，提供更簡易的Command繫結方式(缺點是只能單一事件繫結)</summary>
    public sealed class EventToCommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(EventToCommand), new PropertyMetadata(null));

        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty EventProperty = DependencyProperty.RegisterAttached("Event", typeof(RoutedEvent), typeof(EventToCommand), new PropertyMetadata(null, EventChanged));

        private static void EventChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UIElement ele)
            {
                ele.AddHandler((RoutedEvent)e.NewValue, new RoutedEventHandler(DoCommand));
            }
        }

        public static RoutedEvent GetEvent(DependencyObject obj)
        {
            return (RoutedEvent)obj.GetValue(EventProperty);
        }

        public static void SetEvent(DependencyObject obj, RoutedEvent value)
        {
            obj.SetValue(EventProperty, value);
        }

        private static void DoCommand(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement ele)
            {
                var command = (ICommand)ele.GetValue(CommandProperty);
                if (command != null)
                {
                    var parameter = ele.GetValue(CommandParameterProperty);
                    parameter = parameter ?? e;
                    command.Execute(parameter);
                }
            }
        }
    }
}