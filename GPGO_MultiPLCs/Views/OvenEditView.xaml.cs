﻿using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GPGRC_MultiPLCs.Views;

/// <summary>OvenEditView.xaml 的互動邏輯</summary>
public partial class OvenEditView : UserControl
{
    public OvenEditView() => InitializeComponent();

    private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => ((TextBox)sender).SelectAll();

    private void DataGrid_Loaded(object sender, RoutedEventArgs e)
    {
        var products = CollectionViewSource.GetDefaultView(((DataGrid)sender).ItemsSource);
        if (products is { CanSort: true } && !products.SortDescriptions.Any())
        {
            products.SortDescriptions.Add(new SortDescription("Layer", ListSortDirection.Ascending));
        }
    }

    private void OPTextBox_KeyDown(object sender, KeyEventArgs e)
    {

    }

    private void PartTextBox_KeyDown(object sender, KeyEventArgs e)
    {

    }

    private void LotTextBox_KeyDown(object sender, KeyEventArgs e)
    {

    }

    private void RecipeTextBox_KeyDown(object sender, KeyEventArgs e)
    {

    }

    private void Grid_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
    }
}