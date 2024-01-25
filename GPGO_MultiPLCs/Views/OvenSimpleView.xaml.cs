using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPGRC_MultiPLCs.Views;

/// <summary>OvenSimpleView.xaml 的互動邏輯</summary>
public partial class OvenSimpleView
{
    public OvenSimpleView() => InitializeComponent();

    private void CB_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => ((ComboBox)sender).Text = "";

    private void CB_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => ((ComboBox)sender).Text = ((ComboBox)sender).SelectedItem as string;

    private async void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs _)
    {
        if (sender is not TextBox tb)
        {
            return;
        }

        await Task.Delay(15);

        tb.SelectAll();
    }
}