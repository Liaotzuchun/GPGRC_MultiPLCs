using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views;

/// <summary>TraceabilityView.xaml 的互動邏輯</summary>
public partial class TraceabilityView
{
    public TraceabilityView() => InitializeComponent();
    private void MenuItem_SubmenuClosed(object sender, RoutedEventArgs e) => Keyboard.ClearFocus();

    private void SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (dg.SelectedItem != null)
        {
            dg.ScrollIntoView(dg.SelectedItem);
        }
    }

    private void SelectedCellsChanged2(object sender, SelectedCellsChangedEventArgs e)
    {
        //if (dg2.SelectedItem != null)
        //{
        //    dg2.ScrollIntoView(dg2.SelectedItem);
        //}
    }

    private void DataGrid_Loaded(object sender, RoutedEventArgs e)
    {
        var products = CollectionViewSource.GetDefaultView(((DataGrid)sender).ItemsSource);
        if (products is { CanSort: true } && !products.SortDescriptions.Any())
        {
            products.SortDescriptions.Add(new SortDescription("Layer", ListSortDirection.Ascending));
        }
    }
}