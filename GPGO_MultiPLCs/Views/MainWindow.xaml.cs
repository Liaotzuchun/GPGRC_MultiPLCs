using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs
{
    /// <summary>MainWindow.xaml 的互動邏輯</summary>
    public partial class MainWindow : Window
    {
        private void GlobalDialog_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Authenticator.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(PopGrid, 100);
        }

        public MainWindow()
        {
            InitializeComponent();

            Title = $"群翊工業 {Assembly.GetExecutingAssembly().GetName().Version}";
        }
    }
}