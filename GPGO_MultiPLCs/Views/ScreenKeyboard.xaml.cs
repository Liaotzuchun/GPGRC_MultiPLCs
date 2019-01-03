using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>ScreenKeyboard.xaml 的互動邏輯</summary>
    public partial class ScreenKeyboard : UserControl
    {
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(this, 0);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Panel.SetZIndex(this, int.MaxValue);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Panel.SetZIndex(this, 0);
        }

        public ScreenKeyboard()
        {
            InitializeComponent();
        }
    }
}