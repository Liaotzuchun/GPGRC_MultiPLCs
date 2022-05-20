using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views;

/// <summary>
/// OvenEditView.xaml 的互動邏輯
/// </summary>
public partial class OvenEditView : UserControl
{
    private void CB_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) { ((ComboBox)sender).Text = ""; }

    private void CB_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) { ((ComboBox)sender).Text = ((ComboBox)sender).SelectedItem as string; }


    public OvenEditView()
    {
        InitializeComponent();
    }
}