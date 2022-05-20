using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views;

/// <summary>MainWindow.xaml 的互動邏輯</summary>
public partial class MainWindow
{
    private void GlobalDialog_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (Visibility == Visibility.Visible)
        {
            Authenticator.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) { Panel.SetZIndex(PopGrid, int.MaxValue); }

    public MainWindow()
    {
        InitializeComponent();
    }
}