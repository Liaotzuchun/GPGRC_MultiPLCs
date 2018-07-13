using System.Windows;
using System.Windows.Controls;

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

        public TraceabilityView()
        {
            InitializeComponent();
        }
    }
}