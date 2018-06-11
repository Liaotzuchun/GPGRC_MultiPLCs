using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    ///     TraceabilityView.xaml 的互動邏輯
    /// </summary>
    public partial class TraceabilityView : UserControl
    {
        public TraceabilityView()
        {
            InitializeComponent();
        }

        private void CB_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            CB.SelectedIndex = -1;
        }
    }
}