using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    ///     TraceabilityView.xaml 的互動邏輯
    /// </summary>
    public partial class TraceabilityView : UserControl
    {
        private void CB_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((ComboBox)sender).IsEnabled)
            {
                CB.SelectedItem = CB.Items.IsEmpty ? -1 : CB.Items[0];
            }
            else
            {
                CB.SelectedItem = -1;
            }
        }

        private void TotalTB_Checked(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "全部站點";
        }

        private void TotalTB_Unchecked(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "指定站點";
        }

        public TraceabilityView()
        {
            InitializeComponent();
        }
    }
}