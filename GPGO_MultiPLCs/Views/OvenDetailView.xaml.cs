using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace GPGO_MultiPLCs.Views;

/// <summary>
/// OvenDetailView.xaml 的互動邏輯
/// </summary>
public partial class OvenDetailView
{
    private void SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (dg.SelectedItem != null)
        {
            dg.ScrollIntoView(dg.SelectedItem);
        }
    }

    private void DataGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        var products = CollectionViewSource.GetDefaultView(((DataGrid)sender).ItemsSource);
        if (products is { CanSort: true } && !products.SortDescriptions.Any())
        {
            products.SortDescriptions.Add(new SortDescription("Layer", ListSortDirection.Ascending));
        }
    }

    public OvenDetailView()
    {
        InitializeComponent();
    }
}