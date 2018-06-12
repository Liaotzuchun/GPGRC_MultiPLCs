using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    ///     GlobalDialog.xaml 的互動邏輯
    /// </summary>
    public partial class GlobalDialog : UserControl
    {
        public GlobalDialog() => InitializeComponent();

        private void Intput_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Intput.IsVisible)
            {
                Keyboard.Focus(Intput);
            }
        }
    }
}