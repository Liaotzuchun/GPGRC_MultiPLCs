namespace GPGO_MultiPLCs.Views;

/// <summary>TotalView.xaml 的互動邏輯</summary>
public partial class TotalView
{
    private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        MessageDataGrid.UnselectAll();
    }

    public TotalView()
    {
        InitializeComponent();
    }
}