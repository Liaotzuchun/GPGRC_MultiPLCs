using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>GlobalDialog.xaml 的互動邏輯</summary>
    public partial class GlobalDialog
    {
        private async void Intput_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Intput.IsVisible)
            {
                await Task.Delay(60);
                Keyboard.Focus(Intput);
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) { Keyboard.ClearFocus(); }

        public GlobalDialog() { InitializeComponent(); }
    }
}