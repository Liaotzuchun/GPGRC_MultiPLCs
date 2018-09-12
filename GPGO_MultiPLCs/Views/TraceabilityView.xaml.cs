using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>TraceabilityView.xaml 的互動邏輯</summary>
    public partial class TraceabilityView : UserControl
    {
        private void MenuItem_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dg.SelectedItem != null)
            {
                dg.ScrollIntoView(dg.SelectedItem);
            }
        }

        public TraceabilityView()
        {
            InitializeComponent();
        }
    }
}