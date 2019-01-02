using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>RecipeControl.xaml 的互動邏輯</summary>
    public partial class RecipeControl : UserControl
    {
        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            TC.SelectedIndex = 0;
        }

        private async void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(sender is TextBox tb))
            {
                return;
            }

            await Task.Delay(15);

            tb.SelectAll();
        }

        private async void EditedTB_Checked(object sender, RoutedEventArgs e)
        {
            await Task.Delay(15);
            Keyboard.Focus(EditTextBox);
        }

        private void EditedTB_Unchecked(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EditedTB.IsChecked = false;
        }

        private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EditedTB.RaiseEvent(new RoutedEventArgs(ToggleButton.UncheckedEvent));
            }
        }

        public RecipeControl()
        {
            InitializeComponent();
        }
    }
}