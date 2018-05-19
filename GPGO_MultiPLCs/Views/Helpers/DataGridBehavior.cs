﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GPGO_MultiPLCs.Views.Helpers
{
    public class DataGridBehavior
    {
        public static DependencyProperty DisplayRowNumberProperty =
            DependencyProperty.RegisterAttached("DisplayRowNumber", typeof(bool), typeof(DataGridBehavior), new FrameworkPropertyMetadata(false, OnDisplayRowNumberChanged));

        public static bool GetDisplayRowNumber(DependencyObject target)
        {
            return (bool)target.GetValue(DisplayRowNumberProperty);
        }

        public static void SetDisplayRowNumber(DependencyObject target, bool value)
        {
            target.SetValue(DisplayRowNumberProperty, value);
        }

        private static void OnDisplayRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = target as DataGrid;
            if ((bool)e.NewValue)
            {
                void loadedRowHandler(object sender, DataGridRowEventArgs ea)
                {
                    if (GetDisplayRowNumber(dataGrid) == false)
                    {
                        dataGrid.LoadingRow -= loadedRowHandler;
                        return;
                    }

                    ea.Row.Header = ea.Row.GetIndex() + 1;
                }

                if (dataGrid != null)
                {
                    dataGrid.LoadingRow += loadedRowHandler;

                    void itemsChangedHandler(object sender, ItemsChangedEventArgs ea)
                    {
                        if (GetDisplayRowNumber(dataGrid) == false)
                        {
                            dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
                            return;
                        }

                        GetVisualChildCollection<DataGridRow>(dataGrid).ForEach(d => d.Header = d.GetIndex() + 1);
                    }

                    dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
                }
            }
        }

        private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            var visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }

                GetVisualChildCollection(child, visualCollection);
            }
        }
    }
}