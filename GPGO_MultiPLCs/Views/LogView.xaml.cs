using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>LogView.xaml 的互動邏輯</summary>
    public partial class LogView : UserControl
    {
        private void MenuItem_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        public LogView()
        {
            InitializeComponent();
        }
    }
}