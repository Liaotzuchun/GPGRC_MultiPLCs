using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    ///     Authenticator.xaml 的互動邏輯
    /// </summary>
    public partial class Authenticator : UserControl
    {
        private async void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Keyboard.ClearFocus();

                await Task.Delay(60);

                Keyboard.Focus(NameBox);
            }
        }

        private void NameBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void Password_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        public Authenticator()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var lng = Application.Current.Resources.MergedDictionaries.Last();
            lng.Source = new Uri("pack://application:,,,/Views/Languages/TW.xaml");
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            var lng = Application.Current.Resources.MergedDictionaries.Last();
            lng.Source = new Uri("pack://application:,,,/Views/Languages/CHS.xaml");
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            var lng = Application.Current.Resources.MergedDictionaries.Last();
            lng.Source = new Uri("pack://application:,,,/Views/Languages/EN.xaml");
        }
    }
}