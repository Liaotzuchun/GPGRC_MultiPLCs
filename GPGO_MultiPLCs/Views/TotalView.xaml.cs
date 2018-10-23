using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>TotalView.xaml 的互動邏輯</summary>
    public partial class TotalView : UserControl
    {
        private void CB_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((ComboBox)sender).Text = "";
        }

        private void CB_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((ComboBox)sender).Text = ((ComboBox)sender).SelectedItem as string;
        }

        private void SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dg.SelectedItem != null)
            {
                dg.ScrollIntoView(dg.SelectedItem);
            }
        }

        public TotalView()
        {
            InitializeComponent();
            CA.LabelFormatter = val => "No. " + (val + 1).ToString("00");
            //BS.LabelFormatString = "{0}";
        }
    }
}