using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>TraceabilityView.xaml 的互動邏輯</summary>
    public partial class TraceabilityView : UserControl
    {
        public TraceabilityView()
        {
            InitializeComponent();
        }

        private void MenuItem_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}