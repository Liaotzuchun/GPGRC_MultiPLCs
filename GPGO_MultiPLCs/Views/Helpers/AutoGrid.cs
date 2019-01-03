using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>提供更簡易方便使用的Grid，會依元素順序自動分配位置，如果元素有自訂的rowspan或columnspan參數也會自動計算</summary>
    public sealed class AutoGrid : Grid
    {
        public static readonly DependencyProperty ChildHorizontalAlignmentProperty = DependencyProperty.Register("ChildHorizontalAlignment",
                                                                                                                 typeof(HorizontalAlignment?),
                                                                                                                 typeof(AutoGrid),
                                                                                                                 new FrameworkPropertyMetadata(null,
                                                                                                                                               FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                                                                               OnChildHorizontalAlignmentChanged));

        private static void OnChildHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoGrid grid)
            {
                foreach (UIElement child in grid.Children)
                {
                    child.SetValue(HorizontalAlignmentProperty, grid.ChildHorizontalAlignment ?? DependencyProperty.UnsetValue);
                }
            }
        }

        public HorizontalAlignment? ChildHorizontalAlignment
        {
            get => (HorizontalAlignment?)GetValue(ChildHorizontalAlignmentProperty);
            set => SetValue(ChildHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty ChildMarginProperty = DependencyProperty.Register("ChildMargin",
                                                                                                    typeof(Thickness?),
                                                                                                    typeof(AutoGrid),
                                                                                                    new FrameworkPropertyMetadata(null,
                                                                                                                                  FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                                                                  OnChildMarginChanged));

        private static void OnChildMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoGrid grid)
            {
                foreach (UIElement child in grid.Children)
                {
                    child.SetValue(MarginProperty, grid.ChildMargin ?? DependencyProperty.UnsetValue);
                }
            }
        }

        public Thickness? ChildMargin
        {
            get => (Thickness?)GetValue(ChildMarginProperty);
            set => SetValue(ChildMarginProperty, value);
        }

        public static readonly DependencyProperty ChildVerticalAlignmentProperty = DependencyProperty.Register("ChildVerticalAlignment",
                                                                                                               typeof(VerticalAlignment?),
                                                                                                               typeof(AutoGrid),
                                                                                                               new FrameworkPropertyMetadata(null,
                                                                                                                                             FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                                                                             OnChildVerticalAlignmentChanged));

        private static void OnChildVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoGrid grid)
            {
                foreach (UIElement child in grid.Children)
                {
                    child.SetValue(VerticalAlignmentProperty, grid.ChildVerticalAlignment ?? DependencyProperty.UnsetValue);
                }
            }
        }

        public VerticalAlignment? ChildVerticalAlignment
        {
            get => (VerticalAlignment?)GetValue(ChildVerticalAlignmentProperty);
            set => SetValue(ChildVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(AutoGrid), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure, ColumnCountChanged));

        public static void ColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue < 0)
            {
                return;
            }

            if (d is AutoGrid grid)
            {
                var width = grid.ColumnWidth;
                if (grid.ColumnDefinitions.Count > 0)
                {
                    if (!string.IsNullOrEmpty(grid.Columns))
                    {
                        return;
                    }

                    width = grid.ColumnDefinitions[0].Width;
                }

                grid.ColumnDefinitions.Clear();
                for (var i = 0; i < (int)e.NewValue; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
                }
            }
        }

        public int ColumnCount
        {
            get => (int)GetValue(ColumnCountProperty);
            set => SetValue(ColumnCountProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(string), typeof(AutoGrid), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure, ColumnsChanged));

        public static void ColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty((string)e.NewValue))
            {
                return;
            }

            if (d is AutoGrid grid)
            {
                grid.ColumnDefinitions.Clear();

                var defs = Parse((string)e.NewValue);
                foreach (var def in defs)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = def });
                }

                grid.ColumnCount = defs.Length;
            }
        }

        public string Columns
        {
            get => (string)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register("ColumnWidth",
                                                                                                    typeof(GridLength),
                                                                                                    typeof(AutoGrid),
                                                                                                    new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star),
                                                                                                                                  FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                                                                  FixedColumnWidthChanged));

        public static void FixedColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoGrid grid)
            {
                if (grid.ColumnDefinitions.Count == 0)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                foreach (var col in grid.ColumnDefinitions)
                {
                    col.Width = (GridLength)e.NewValue;
                }
            }
        }

        public GridLength ColumnWidth
        {
            get => (GridLength)GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        public static readonly DependencyProperty IsAutoIndexingProperty =
            DependencyProperty.Register("IsAutoIndexing", typeof(bool), typeof(AutoGrid), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public bool IsAutoIndexing
        {
            get => (bool)GetValue(IsAutoIndexingProperty);
            set => SetValue(IsAutoIndexingProperty, value);
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation",
                                                                                                    typeof(Orientation),
                                                                                                    typeof(AutoGrid),
                                                                                                    new FrameworkPropertyMetadata(Orientation.Horizontal,
                                                                                                                                  FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(AutoGrid), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure, RowCountChanged));

        public static void RowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue < 0)
            {
                return;
            }

            if (d is AutoGrid grid)
            {
                var height = grid.RowHeight;
                if (grid.RowDefinitions.Count > 0)
                {
                    if (!string.IsNullOrEmpty(grid.Rows))
                    {
                        return;
                    }

                    height = grid.RowDefinitions[0].Height;
                }

                grid.RowDefinitions.Clear();
                for (var i = 0; i < (int)e.NewValue; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = height });
                }
            }
        }

        public int RowCount
        {
            get => (int)GetValue(RowCountProperty);
            set => SetValue(RowCountProperty, value);
        }

        public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register("RowHeight",
                                                                                                  typeof(GridLength),
                                                                                                  typeof(AutoGrid),
                                                                                                  new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star),
                                                                                                                                FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                                                                FixedRowHeightChanged));

        public static void FixedRowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoGrid grid)
            {
                if (grid.RowDefinitions.Count == 0)
                {
                    grid.RowDefinitions.Add(new RowDefinition());
                }

                foreach (var row in grid.RowDefinitions)
                {
                    row.Height = (GridLength)e.NewValue;
                }
            }
        }

        public GridLength RowHeight
        {
            get => (GridLength)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(string), typeof(AutoGrid), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure, RowsChanged));

        public static void RowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty((string)e.NewValue))
            {
                return;
            }

            if (d is AutoGrid grid)
            {
                grid.RowDefinitions.Clear();

                var defs = Parse((string)e.NewValue);
                foreach (var def in defs)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = def });
                }

                grid.RowCount = defs.Length;
            }
        }

        public string Rows
        {
            get => (string)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static GridLength[] Parse(string text)
        {
            var tokens = text.Split(',');
            var definitions = new GridLength[tokens.Length];
            for (var i = 0; i < tokens.Length; i++)
            {
                var str = tokens[i];
                double value;

                if (str.Contains('*'))
                {
                    if (!double.TryParse(str.Replace("*", ""), out value))
                    {
                        value = 1.0;
                    }

                    definitions[i] = new GridLength(value, GridUnitType.Star);

                    continue;
                }

                if (double.TryParse(str, out value))
                {
                    definitions[i] = new GridLength(value);

                    continue;
                }

                definitions[i] = GridLength.Auto;
            }

            return definitions;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            PerformLayout();

            return base.MeasureOverride(constraint);
        }

        private void ApplyChildLayout(DependencyObject child)
        {
            if (ChildMargin != null)
            {
                child.SetIfDefault(MarginProperty, ChildMargin.Value);
            }

            if (ChildHorizontalAlignment != null)
            {
                child.SetIfDefault(HorizontalAlignmentProperty, ChildHorizontalAlignment.Value);
            }

            if (ChildVerticalAlignment != null)
            {
                child.SetIfDefault(VerticalAlignmentProperty, ChildVerticalAlignment.Value);
            }
        }

        private void PerformLayout()
        {
            var Clamp = new Func<int, int, int>((value, max) => value > max ? max : value);
            var rowCount = RowDefinitions.Count;
            var colCount = ColumnDefinitions.Count;

            if (rowCount == 0 || colCount == 0)
            {
                return;
            }

            var position = 0;
            var skip = new bool[rowCount, colCount];
            foreach (UIElement child in Children)
            {
                if (IsAutoIndexing && child.Visibility != Visibility.Collapsed && GetColumn(child) == 0 && GetRow(child) == 0)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        var row = Clamp(position / colCount, rowCount - 1);
                        var col = Clamp(position % colCount, colCount - 1);
                        while (row < rowCount && col < colCount && skip[row, col])
                        {
                            position++;
                            row = position / rowCount;
                            col = position % rowCount;
                        }

                        SetRow(child, row);
                        SetColumn(child, col);
                        position += GetColumnSpan(child);

                        var offset_rs = GetRowSpan(child) - 1;
                        var offset_cs = GetColumnSpan(child) - 1;

                        for (var i = 0; i <= offset_cs; i++)
                        {
                            for (var j = 0; j <= offset_rs; j++)
                            {
                                if (row < rowCount && col < colCount)
                                {
                                    skip[row + j, col + i] = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        var row = Clamp(position % rowCount, rowCount - 1);
                        var col = Clamp(position / rowCount, colCount - 1);
                        while (row < rowCount && col < colCount && skip[row, col])
                        {
                            position++;
                            row = position % rowCount;
                            col = position / rowCount;

                            if (row >= RowCount || col >= colCount)
                            {
                                break;
                            }
                        }

                        SetRow(child, row);
                        SetColumn(child, col);
                        position += GetRowSpan(child);

                        var offset_rs = GetRowSpan(child) - 1;
                        var offset_cs = GetColumnSpan(child) - 1;

                        for (var i = 0; i <= offset_cs; i++)
                        {
                            for (var j = 0; j <= offset_rs; j++)
                            {
                                if (row < rowCount && col < colCount)
                                {
                                    skip[row + j, col + i] = true;
                                }
                            }
                        }
                    }
                }

                ApplyChildLayout(child);
            }
        }
    }

    public static class DependencyExtensions
    {
        public static bool SetIfDefault<T>(this DependencyObject o, DependencyProperty property, T value)
        {
            if (DependencyPropertyHelper.GetValueSource(o, property).BaseValueSource == BaseValueSource.Default)
            {
                o.SetValue(property, value);

                return true;
            }

            return false;
        }
    }
}