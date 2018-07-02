using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    ///     TotalView.xaml 的互動邏輯
    /// </summary>
    public partial class TotalView : UserControl
    {
        public TotalView()
        {
            InitializeComponent();
            CA.LabelFormatter = val => "第" + (val + 1) + "站";
            BS.LabelFormatString = "{0}";
        }

        private void CB_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((ComboBox)sender).Text = "";
        }
    }
}