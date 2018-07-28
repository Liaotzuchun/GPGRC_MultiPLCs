using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs
{
    /// <summary>MainWindow.xaml 的互動邏輯</summary>
    public partial class MainWindow : Window
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(PopGrid, 100);
            Panel.SetZIndex(AuthGrid, 101);
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}