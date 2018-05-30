using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GPGO_MultiPLCs.Views
{
    public class UniformGrid_H : UniformGrid
    {
        private int _columns;
        private int _rows;

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateComputedValues();

            var step_w_0 = Math.Floor(constraint.Width / _columns);
            var step_w_1 = Math.Ceiling(constraint.Width / _columns);
            var step_h_0 = Math.Floor(constraint.Height / _rows);
            var step_h_1 = Math.Ceiling(constraint.Height / _rows);
            var total_w = 0.0;
            var total_h = 0.0;

            var i = 0;
            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                double x = i % _columns;
                double y = _columns == 0 ? 0 : i / _columns;

                child.Measure(new Size(x % 2.0 == 0 ? step_w_0 : step_w_1, y % 2.0 == 0 ? step_h_0 : step_h_1));
                if (i < _columns)
                {
                    var childDesiredSize = child.DesiredSize;
                    total_w += childDesiredSize.Width;
                }

                if (x == 0)
                {
                    var childDesiredSize = child.DesiredSize;
                    total_h += childDesiredSize.Height;
                }

                i++;
            }

            return new Size(total_w, total_h);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var step_w_0 = Math.Floor(arrangeSize.Width / _columns);
            var step_w_1 = Math.Ceiling(arrangeSize.Width / _columns);
            var step_h_0 = Math.Floor(arrangeSize.Height / _rows);
            var step_h_1 = Math.Ceiling(arrangeSize.Height / _rows);
            var x = 0.0;
            var y = 0.0;

            var i = 0;
            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                double xc = i % _columns;
                double yc = _columns == 0 ? 0 : i / _columns;

                var w = xc == _columns - 1 ? arrangeSize.Width - x : xc % 2.0 == 0.0 ? step_w_0 : step_w_1;
                var h = yc == _rows - 1 ? arrangeSize.Height - y : yc % 2.0 == 0.0 ? step_h_0 : step_h_1;

                child.Arrange(new Rect(x, y, w, h));

                if (xc == _columns - 1)
                {
                    x = 0;
                    y += h;
                }
                else
                {
                    x += w;
                }

                i++;
            }

            return arrangeSize;
        }

        private void UpdateComputedValues()
        {
            _columns = Columns;
            _rows = Rows;

            if (FirstColumn >= _columns)
            {
                FirstColumn = 0;
            }

            if (_rows == 0 || _columns == 0)
            {
                var nonCollapsedCount = 0;
                var count = InternalChildren.Count;
                var i = 0;
                while (i < count)
                {
                    var child = InternalChildren[i];
                    if (child.Visibility != Visibility.Collapsed)
                    {
                        nonCollapsedCount++;
                    }

                    i++;
                }

                if (nonCollapsedCount == 0)
                {
                    nonCollapsedCount = 1;
                }

                if (_rows == 0)
                {
                    if (_columns > 0)
                    {
                        _rows = (nonCollapsedCount + FirstColumn + (_columns - 1)) / _columns;
                    }
                    else
                    {
                        _rows = (int)Math.Sqrt(nonCollapsedCount);
                        if (_rows * _rows < nonCollapsedCount)
                        {
                            _rows++;
                        }

                        _columns = _rows;
                    }
                }
                else if (_columns == 0)
                {
                    _columns = (nonCollapsedCount + (_rows - 1)) / _rows;
                }
            }
        }
    }

    public class UniformGrid_V : UniformGrid
    {
        private int _columns;
        private int _rows;

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateComputedValues();

            var step_w_0 = Math.Floor(constraint.Width / _columns);
            var step_w_1 = Math.Ceiling(constraint.Width / _columns);
            var step_h_0 = Math.Floor(constraint.Height / _rows);
            var step_h_1 = Math.Ceiling(constraint.Height / _rows);
            var total_w = 0.0;
            var total_h = 0.0;

            var i = 0;
            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                double x = _rows == 0 ? 0 : i / _rows;
                double y = i % _rows;

                child.Measure(new Size(x % 2.0 == 0 ? step_w_0 : step_w_1, y % 2.0 == 0 ? step_h_0 : step_h_1));
                if (i < _rows)
                {
                    var childDesiredSize = child.DesiredSize;
                    total_h += childDesiredSize.Height;
                }

                if (x == 0)
                {
                    var childDesiredSize = child.DesiredSize;
                    total_w += childDesiredSize.Width;
                }

                i++;
            }

            return new Size(total_w, total_h);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var step_w_0 = Math.Floor(arrangeSize.Width / _columns);
            var step_w_1 = Math.Ceiling(arrangeSize.Width / _columns);
            var step_h_0 = Math.Floor(arrangeSize.Height / _rows);
            var step_h_1 = Math.Ceiling(arrangeSize.Height / _rows);
            var x = 0.0;
            var y = 0.0;

            var i = 0;
            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                double xc = _rows == 0 ? 0 : i / _rows;
                double yc = i % _rows;

                var w = xc == _columns - 1 ? arrangeSize.Width - x : xc % 2.0 == 0.0 ? step_w_0 : step_w_1;
                var h = yc == _rows - 1 ? arrangeSize.Height - y : yc % 2.0 == 0.0 ? step_h_0 : step_h_1;

                child.Arrange(new Rect(x, y, w, h));

                if (yc == _rows - 1)
                {
                    y = 0;
                    x += w;
                }
                else
                {
                    y += h;
                }

                i++;
            }

            return arrangeSize;
        }

        private void UpdateComputedValues()
        {
            _columns = Columns;
            _rows = Rows;
            if (FirstColumn >= _columns)
            {
                FirstColumn = 0;
            }

            if (FirstColumn > 0)
            {
                throw new NotImplementedException("There is no support for seting the FirstColumn (nor the FirstRow).");
            }

            if (_rows == 0 || _columns == 0)
            {
                var num = 0; // Visible children  
                var num2 = 0;
                var count = InternalChildren.Count;
                while (num2 < count)
                {
                    var element = InternalChildren[num2];
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        num++;
                    }

                    num2++;
                }

                if (num == 0)
                {
                    num = 1;
                }

                if (_rows == 0)
                {
                    if (_columns > 0)
                    {
                        _rows = (num + FirstColumn + (_columns - 1)) / _columns;
                    }
                    else
                    {
                        _rows = (int)Math.Sqrt(num);
                        if (_rows * _rows < num)
                        {
                            _rows++;
                        }

                        _columns = _rows;
                    }
                }
                else if (_columns == 0)
                {
                    _columns = (num + (_rows - 1)) / _rows;
                }
            }
        }
    }

    public class UniformGridWithOrientation : UniformGrid
    {
        private int _columns;
        private int _rows;

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateComputedValues();
            var availableSize = new Size(constraint.Width / _columns, constraint.Height / _rows);
            var width = 0.0;
            var height = 0.0;
            var num3 = 0;
            var count = InternalChildren.Count;
            while (num3 < count)
            {
                var element = InternalChildren[num3];
                element.Measure(availableSize);
                var desiredSize = element.DesiredSize;
                if (width < desiredSize.Width)
                {
                    width = desiredSize.Width;
                }

                if (height < desiredSize.Height)
                {
                    height = desiredSize.Height;
                }

                num3++;
            }

            return new Size(width * _columns, height * _rows);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var finalRect = new Rect(0.0, 0.0, arrangeSize.Width / _columns, arrangeSize.Height / _rows);
            var height = finalRect.Height;
            var numX = arrangeSize.Height - 1.0;
            finalRect.X += finalRect.Width * FirstColumn;
            foreach (UIElement element in InternalChildren)
            {
                element.Arrange(finalRect);
                if (element.Visibility != Visibility.Collapsed)
                {
                    finalRect.Y += height;
                    if (finalRect.Y >= numX)
                    {
                        finalRect.X += finalRect.Width;
                        finalRect.Y = 0.0;
                    }
                }
            }

            return arrangeSize;
        }

        private void UpdateComputedValues()
        {
            _columns = Columns;
            _rows = Rows;
            if (FirstColumn >= _columns)
            {
                FirstColumn = 0;
            }

            if (FirstColumn > 0)
            {
                throw new NotImplementedException("There is no support for seting the FirstColumn (nor the FirstRow).");
            }

            if (_rows == 0 || _columns == 0)
            {
                var num = 0; // Visible children  
                var num2 = 0;
                var count = InternalChildren.Count;
                while (num2 < count)
                {
                    var element = InternalChildren[num2];
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        num++;
                    }

                    num2++;
                }

                if (num == 0)
                {
                    num = 1;
                }

                if (_rows == 0)
                {
                    if (_columns > 0)
                    {
                        _rows = (num + FirstColumn + (_columns - 1)) / _columns;
                    }
                    else
                    {
                        _rows = (int)Math.Sqrt(num);
                        if (_rows * _rows < num)
                        {
                            _rows++;
                        }

                        _columns = _rows;
                    }
                }
                else if (_columns == 0)
                {
                    _columns = (num + (_rows - 1)) / _rows;
                }
            }
        }

        #region Orientation (Dependency Property)  

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation",
                                                                                                    typeof(Orientation),
                                                                                                    typeof(UniformGridWithOrientation),
                                                                                                    new FrameworkPropertyMetadata(Orientation.Vertical,
                                                                                                                                  FrameworkPropertyMetadataOptions.AffectsMeasure),
                                                                                                    IsValidOrientation);

        internal static bool IsValidOrientation(object o)
        {
            var orientation = (Orientation)o;
            if (orientation != Orientation.Horizontal)
            {
                return orientation == Orientation.Vertical;
            }

            return true;
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        #endregion
    }
}