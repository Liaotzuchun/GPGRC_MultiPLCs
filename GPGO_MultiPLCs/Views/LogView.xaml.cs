using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views;

/// <summary>LogView.xaml 的互動邏輯</summary>
public partial class LogView
{
    private void dg1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (dg1.SelectedItem != null)
        {
            dg1.ScrollIntoView(dg1.SelectedItem);
        }
    }

    private void dg2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (dg2.SelectedItem != null)
        {
            dg2.ScrollIntoView(dg2.SelectedItem);
        }
    }

    private void MenuItem_SubmenuClosed(object sender, RoutedEventArgs e) { Keyboard.ClearFocus(); }

    public LogView() { InitializeComponent(); }
}