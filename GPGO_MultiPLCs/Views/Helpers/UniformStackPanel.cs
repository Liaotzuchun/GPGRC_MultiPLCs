using System;
using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views.Helpers
{
    public class UniformStackPanel : StackPanel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            var x = 0.0;
            var y = 0.0;
            var fh = finalSize.Height;
            var fw = finalSize.Width;
            var cc = 0.0;

            if (Orientation == Orientation.Vertical)
            {
                foreach (UIElement child in InternalChildren)
                {
                    if (child.GetValue(HeightProperty) is double th && th > 0)
                    {
                        if (child.GetValue(MarginProperty) is Thickness tk)
                        {
                            th += (tk.Top + tk.Bottom);
                        }

                        fh -= th;
                        cc += 1;
                    }
                    else if (child.Visibility == Visibility.Collapsed)
                    {
                        cc += 1;
                    }
                }

                var ah = Math.Floor(fh / (InternalChildren.Count - cc));
                var _y = fh - ah * (InternalChildren.Count - cc);
                foreach (UIElement child in InternalChildren)
                {
                    if (child.GetValue(HeightProperty) is double th && th > 0)
                    {
                        if (child.GetValue(MarginProperty) is Thickness tk)
                        {
                            th += (tk.Top + tk.Bottom);
                        }

                        child.Arrange(new Rect(x, y, finalSize.Width, th));
                        y += th;
                    }
                    else if (child.Visibility != Visibility.Collapsed)
                    {
                        var h = _y > 0 ? ah + 1 : ah;
                        child.Arrange(new Rect(x, y, finalSize.Width, h));
                        y += h;
                        _y -= 1;
                    }
                }
            }
            else
            {
                foreach (UIElement child in InternalChildren)
                {
                    if (child.GetValue(WidthProperty) is double tw && tw > 0)
                    {
                        if (child.GetValue(MarginProperty) is Thickness tk)
                        {
                            tw += (tk.Left + tk.Right);
                        }

                        fw -= tw;
                        cc += 1;
                    }
                    else if (child.Visibility == Visibility.Collapsed)
                    {
                        cc += 1;
                    }
                }

                var aw = Math.Floor(fw / (InternalChildren.Count - cc));
                var _x = fw - aw * ((InternalChildren.Count - cc));
                foreach (UIElement child in InternalChildren)
                {
                    if (child.GetValue(WidthProperty) is double tw && tw > 0)
                    {
                        if (child.GetValue(MarginProperty) is Thickness tk)
                        {
                            tw += (tk.Left + tk.Right);
                        }

                        child.Arrange(new Rect(x, y, tw, finalSize.Height));
                        x += tw;
                    }
                    else if (child.Visibility != Visibility.Collapsed)
                    {
                        var w = _x > 0 ? aw + 1 : aw;
                        child.Arrange(new Rect(x, y, w, finalSize.Height));
                        x += w;
                        _x -= 1;
                    }
                }
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var fw = 0.0;
            var fh = 0.0;

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(constraint);
                var childDesiredSize = child.DesiredSize;

                if (Orientation == Orientation.Horizontal)
                {
                    fw += childDesiredSize.Width;
                    fh = Math.Max(fh, childDesiredSize.Height);
                }
                else if (Orientation == Orientation.Vertical)
                {
                    fh += childDesiredSize.Height;
                    fw = Math.Max(fw, childDesiredSize.Width);
                }
            }

            return new Size(fw, fh);
        }
    }
}