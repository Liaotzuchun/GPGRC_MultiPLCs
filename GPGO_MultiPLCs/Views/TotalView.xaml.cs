using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views;

/// <summary>TotalView.xaml 的互動邏輯</summary>
public partial class TotalView
{
    public TotalView() => InitializeComponent();

    private void Border_MouseDown(object sender, MouseButtonEventArgs e) => MessageDataGrid.UnselectAll();

    private void MessageDataGrid_Loaded(object sender, RoutedEventArgs e)
    {
        var evs = CollectionViewSource.GetDefaultView(((DataGrid)sender).ItemsSource);
        if (evs is { CanSort: true } && !evs.SortDescriptions.Any())
        {
            evs.SortDescriptions.Add(new SortDescription("AddedTime", ListSortDirection.Descending));
        }
    }
}