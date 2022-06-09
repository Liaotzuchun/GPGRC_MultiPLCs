using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views;

/// <summary>
/// OvenEditView.xaml 的互動邏輯
/// </summary>
public partial class OvenEditView : UserControl
{
    private void CB_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) { ((ComboBox)sender).Text = ""; }

    private void CB_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) { ((ComboBox)sender).Text = ((ComboBox)sender).SelectedItem as string; }

    private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ((TextBox)sender).SelectAll();
    }

    private void DataGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        var cvTasks = CollectionViewSource.GetDefaultView(((DataGrid)sender).ItemsSource);
        if (cvTasks is { CanSort: true } && !cvTasks.SortDescriptions.Any())
        {
            cvTasks.SortDescriptions.Add(new SortDescription("Layer", ListSortDirection.Ascending));
        }
    }

    public OvenEditView()
    {
        InitializeComponent();
    }
}