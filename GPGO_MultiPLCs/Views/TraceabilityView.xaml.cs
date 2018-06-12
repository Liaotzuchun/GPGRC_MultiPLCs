using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    ///     TraceabilityView.xaml 的互動邏輯
    /// </summary>
    public partial class TraceabilityView : UserControl
    {
        public TraceabilityView() => InitializeComponent();

        private void CB_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
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

        private void TotalTB_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "全部站點";
        }

        private void TotalTB_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "指定站點";
        }
    }
}