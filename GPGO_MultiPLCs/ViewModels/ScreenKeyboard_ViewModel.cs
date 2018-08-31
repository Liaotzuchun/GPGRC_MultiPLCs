using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.ViewModels
{
    public class ScreenKeyboard_ViewModel : ObservableObject
    {
        public RelayCommand ActionCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand CopyCommand { get; }
        public RelayCommand KeyInputCommand { get; }
        public RelayCommand PasteCommand { get; }
        public RelayCommand FocusCommand { get; }

        public Visibility Shown => Target == null ? Visibility.Collapsed : Visibility.Visible;
        public ScreenKeyboardLangKey StrVals { get; }

        public bool IsNumOnly
        {
            get => Get<bool>();
            set => Set(value);
        }

        public UIElement Target
        {
            get => Get<UIElement>();
            set
            {
                Set(value);
                NotifyPropertyChanged(nameof(Shown));
            }
        }

        public VerticalAlignment VA
        {
            get => Get<VerticalAlignment>();
            set => Set(value);
        }

        public void SetTargetElement(UIElement target, UIElement container)
        {
            Target = target;
            if (Target != null && container!= null)
            {
                var h = container.RenderSize.Height;
                var y = Target.TranslatePoint(new Point(0.0, 0.0), container).Y;
                VA = y > h / 2.0 ? VerticalAlignment.Top : VerticalAlignment.Bottom;
            }
        }

        public sealed class ScreenKeyboardLangKey : ObservableObject
        {
            private bool _IsUpper;

            public string A => _IsUpper ? "A" : "a";
            public string B => _IsUpper ? "B" : "b";
            public string C => _IsUpper ? "C" : "c";
            public string D => _IsUpper ? "D" : "d";
            public string E => _IsUpper ? "E" : "e";
            public string F => _IsUpper ? "F" : "f";
            public string G => _IsUpper ? "G" : "g";
            public string H => _IsUpper ? "H" : "h";
            public string I => _IsUpper ? "I" : "i";
            public string J => _IsUpper ? "J" : "j";
            public string K => _IsUpper ? "K" : "k";
            public string L => _IsUpper ? "L" : "l";
            public string M => _IsUpper ? "M" : "m";
            public string N => _IsUpper ? "N" : "n";
            public string O => _IsUpper ? "O" : "o";
            public string P => _IsUpper ? "P" : "p";
            public string Q => _IsUpper ? "Q" : "q";
            public string R => _IsUpper ? "R" : "r";
            public string S => _IsUpper ? "S" : "s";
            public string T => _IsUpper ? "T" : "t";
            public string U => _IsUpper ? "U" : "u";
            public string V => _IsUpper ? "V" : "v";
            public string W => _IsUpper ? "W" : "w";
            public string X => _IsUpper ? "X" : "x";
            public string Y => _IsUpper ? "Y" : "y";
            public string Z => _IsUpper ? "Z" : "z";

            public bool IsUpper
            {
                get => _IsUpper;
                set
                {
                    _IsUpper = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(A));
                    NotifyPropertyChanged(nameof(B));
                    NotifyPropertyChanged(nameof(C));
                    NotifyPropertyChanged(nameof(D));
                    NotifyPropertyChanged(nameof(E));
                    NotifyPropertyChanged(nameof(F));
                    NotifyPropertyChanged(nameof(G));
                    NotifyPropertyChanged(nameof(H));
                    NotifyPropertyChanged(nameof(I));
                    NotifyPropertyChanged(nameof(J));
                    NotifyPropertyChanged(nameof(K));
                    NotifyPropertyChanged(nameof(L));
                    NotifyPropertyChanged(nameof(M));
                    NotifyPropertyChanged(nameof(N));
                    NotifyPropertyChanged(nameof(O));
                    NotifyPropertyChanged(nameof(P));
                    NotifyPropertyChanged(nameof(Q));
                    NotifyPropertyChanged(nameof(R));
                    NotifyPropertyChanged(nameof(S));
                    NotifyPropertyChanged(nameof(T));
                    NotifyPropertyChanged(nameof(U));
                    NotifyPropertyChanged(nameof(V));
                    NotifyPropertyChanged(nameof(W));
                    NotifyPropertyChanged(nameof(X));
                    NotifyPropertyChanged(nameof(Y));
                    NotifyPropertyChanged(nameof(Z));
                }
            }
        }

        public ScreenKeyboard_ViewModel()
        {
            StrVals = new ScreenKeyboardLangKey();

            KeyInputCommand = new RelayCommand(e =>
                                               {
                                                   var teve = new TextCompositionEventArgs(Keyboard.PrimaryDevice,
                                                                                           new TextComposition(InputManager.Current,
                                                                                                               Target,
                                                                                                               e is Button but ? (string)but.GetValue(ContentControl.ContentProperty) : ""))
                                                              {
                                                                  RoutedEvent = UIElement.TextInputEvent
                                                              };
                                                   Target.RaiseEvent(teve);
                                               });

            CopyCommand = new RelayCommand(e =>
                                           {
                                               Clipboard.SetText((string)Target.GetValue(TextBox.TextProperty));
                                           });

            PasteCommand = new RelayCommand(e =>
                                            {
                                                var teve = new TextCompositionEventArgs(Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, Target, Clipboard.GetText()))
                                                           {
                                                               RoutedEvent = UIElement.TextInputEvent
                                                           };
                                                Target.RaiseEvent(teve);
                                            });

            ActionCommand = new RelayCommand(e =>
                                             {
                                                 var keve = new KeyEventArgs(Keyboard.PrimaryDevice,
                                                                             PresentationSource.FromDependencyObject(Target) ?? throw new InvalidOperationException(),
                                                                             0,
                                                                             (Key)e) { RoutedEvent = UIElement.KeyDownEvent };

                                                 Target.RaiseEvent(keve);
                                             });

            ClearCommand = new RelayCommand(e =>
                                            {
                                                Target.SetValue(TextBox.TextProperty, "");
                                            });

            CloseCommand = new RelayCommand(e =>
                                            {
                                                Target = null;
                                                Keyboard.ClearFocus();
                                            });

            FocusCommand = new RelayCommand(e=>
                                            {
                                                if(e is KeyboardFocusChangedEventArgs args && args.NewFocus is UIElement ele && ( ele is TextBox || ele is PasswordBox || (ele is ComboBox cb && cb.IsEditable) ))
                                                {
                                                    SetTargetElement(ele, Window.GetWindow(ele)?.Content as UIElement);
                                                }
                                                else
                                                {
                                                    Target = null;
                                                }
                                            });
        }
    }
}