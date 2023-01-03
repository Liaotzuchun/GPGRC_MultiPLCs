using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views;

/// <summary>RecipeControl.xaml 的互動邏輯</summary>
public partial class RecipeControl
{
    public RecipeControl() => InitializeComponent();
    private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) => TC.SelectedIndex = 0;

    private async void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox tb)
        {
            return;
        }

        await Task.Delay(15);

        tb.SelectAll();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) { }
}