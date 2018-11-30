using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>使uniformgrid可選擇由左至右排列優先還是由上至下優先</summary>
    public class UniformGridWithOrientation : UniformGrid
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation),
                                                                                                    typeof(Orientation),
                                                                                                    typeof(UniformGridWithOrientation),
                                                                                                    new FrameworkPropertyMetadata(Orientation.Vertical,
                                                                                                                                  FrameworkPropertyMetadataOptions.AffectsMeasure),
                                                                                                    IsValidOrientation);

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        private int _columns;
        private int _rows;

        internal static bool IsValidOrientation(object o)
        {
            var orientation = (Orientation)o;
            if (orientation != Orientation.Horizontal)
            {
                return orientation == Orientation.Vertical;
            }

            return true;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var x = 0.0;
            var y = 0.0;
            var aw = Math.Floor(arrangeSize.Width / _columns);
            var ah = Math.Floor(arrangeSize.Height / _rows);
            var _x = Math.Round(arrangeSize.Width) - aw * _columns;
            var _y = Math.Round(arrangeSize.Height) - ah * _rows;
            var c = 1;
            var r = 1;

            if (Orientation == Orientation.Horizontal)
            {
                foreach (UIElement child in InternalChildren)
                {
                    if (child.Visibility != Visibility.Collapsed && r <= _rows)
                    {
                        var w = _x > 0 ? aw + 1 : aw;
                        var h = _y > 0 ? ah + 1 : ah;
                        child.Arrange(new Rect(x, y, w, h));

                        c += 1;
                        x += w;
                        _x -= 1;
                        if (c > _columns + 1)
                        {
                            c = 1;
                            x = 0;
                            _x = arrangeSize.Width - aw * _columns;
                            y += h;
                            _y -= 1;
                            r += 1;
                        }
                    }
                }
            }
            else
            {
                foreach (UIElement child in InternalChildren)
                {
                    if (child.Visibility != Visibility.Collapsed && c <= _columns)
                    {
                        var w = _x > 0 ? aw + 1 : aw;
                        var h = _y > 0 ? ah + 1 : ah;
                        child.Arrange(new Rect(x, y, w, h));

                        r += 1;
                        y += h;
                        _y -= 1;
                        if (r >= _rows + 1)
                        {
                            r = 0;
                            y = 0;
                            _y = arrangeSize.Height - ah * Rows;
                            x += w;
                            _x -= 1;
                            c += 1;
                        }
                    }
                }
            }

            return arrangeSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateComputedValues();

            var availableSize = new Size(constraint.Width / _columns, constraint.Height / _rows);
            var w = 0.0;
            var h = 0.0;

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(availableSize);

                if (child.Visibility != Visibility.Collapsed)
                {
                    if (child.DesiredSize.Width > w)
                    {
                        w = child.DesiredSize.Width;
                    }

                    if (child.DesiredSize.Height > h)
                    {
                        h = child.DesiredSize.Height;
                    }
                }
            }

            return new Size(w * _columns, h * _rows);
        }

        private void UpdateComputedValues()
        {
            _columns = InternalChildren.Count < Columns ? InternalChildren.Count : Columns;
            _rows = InternalChildren.Count < Rows ? InternalChildren.Count : Rows;
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
                    if (InternalChildren[num2].Visibility != Visibility.Collapsed)
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
}