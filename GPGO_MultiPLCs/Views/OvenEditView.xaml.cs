using System;
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
    private DateTime tempTime;
    private Key      tempKey;

    private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ((TextBox)sender).SelectAll();
    }

    private void DataGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        var products = CollectionViewSource.GetDefaultView(((DataGrid)sender).ItemsSource);
        if (products is { CanSort: true } && !products.SortDescriptions.Any())
        {
            products.SortDescriptions.Add(new SortDescription("Layer", ListSortDirection.Ascending));
        }
    }

    private void OPTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Keyboard.Focus(PartTextBox);
        }
    }

    private void PartTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        tempKey  = e.Key;
        tempTime = DateTime.Now;

        if (e.Key == Key.Enter)
        {
            Keyboard.Focus(LotTextBox);
        }
    }

    private void LotTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        tempKey  = e.Key;
        tempTime = DateTime.Now;

        if (e.Key == Key.Enter)
        {
            Keyboard.Focus(NumericTextBox);
        }
    }

    private void RecipeTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        tempKey  = e.Key;
        tempTime = DateTime.Now;
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
#if DEBUG
        return;
#endif
        if (tempKey != e.Key || (DateTime.Now - tempTime).TotalMilliseconds > 15.0)
        {
            ((TextBox)sender).Clear();
        }
    }

    public OvenEditView()
    {
        InitializeComponent();
    }
}