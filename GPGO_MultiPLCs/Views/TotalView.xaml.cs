using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    ///     TotalView.xaml 的互動邏輯
    /// </summary>
    public partial class TotalView : UserControl
    {
        private void CB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cb = (ComboBox)sender;
            var str = (string)cb.SelectedItem;
            if (cb.Text != str && (e.Key == Key.Enter || e.Key == Key.Return))
            {
                cb.Text = str;
            }
        }

        private void CB_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var cb = (ComboBox)sender;
            var str = (string)cb.SelectedItem;
            if (cb.Text != str)
            {
                cb.Text = str;
            }
        }

        public TotalView()
        {
            InitializeComponent();
            CA.LabelFormatter = val => "第" + (val + 1) + "站";
            BS.LabelFormatString = "{0}";
        }
    }
}