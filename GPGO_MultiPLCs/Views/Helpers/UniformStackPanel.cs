using System;
using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views.Helpers
{
    public sealed class UniformStackPanel : StackPanel
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
                    var th = Convert.ToDouble(child.GetValue(HeightProperty));
                    if (th > 0)
                    {
                        fh -= th;
                        cc += 1;
                    }
                    else if (child.Visibility == Visibility.Collapsed)
                    {
                        cc += 1;
                    }
                }

                var ah_0 = Math.Floor(fh / (InternalChildren.Count - cc));
                var ah_1 = Math.Ceiling(fh / (InternalChildren.Count - cc));
                var ah_tag = false;
                foreach (UIElement child in InternalChildren)
                {
                    var th = Convert.ToDouble(child.GetValue(HeightProperty));
                    if (th > 0)
                    {
                        child.Arrange(new Rect(x, y, finalSize.Width, th));
                        y += th;
                    }
                    else if (child.Visibility != Visibility.Collapsed)
                    {
                        ah_tag = !ah_tag;
                        var h = ah_tag ? ah_0 : ah_1;
                        child.Arrange(new Rect(x, y, finalSize.Width, h));
                        y += h;
                    }
                }
            }
            else
            {
                foreach (UIElement child in InternalChildren)
                {
                    var tw = Convert.ToDouble(child.GetValue(WidthProperty));
                    if (tw > 0)
                    {
                        fw -= tw;
                        cc += 1;
                    }
                    else if (child.Visibility == Visibility.Collapsed)
                    {
                        cc += 1;
                    }
                }

                var aw_0 = Math.Floor(fw / (InternalChildren.Count - cc));
                var aw_1 = Math.Ceiling(fw / (InternalChildren.Count - cc));
                var aw_tag = false;
                foreach (UIElement child in InternalChildren)
                {
                    var tw = Convert.ToDouble(child.GetValue(WidthProperty));
                    if (tw > 0)
                    {
                        child.Arrange(new Rect(x, y, tw, finalSize.Height));
                        x += tw;
                    }
                    else if (child.Visibility != Visibility.Collapsed)
                    {
                        aw_tag = !aw_tag;
                        var w = aw_tag ? aw_0 : aw_1;
                        child.Arrange(new Rect(x, y, w, finalSize.Height));
                        x += w;
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