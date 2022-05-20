using System.Windows.Controls;

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

    public OvenDetailView()
    {
        InitializeComponent();
    }
}