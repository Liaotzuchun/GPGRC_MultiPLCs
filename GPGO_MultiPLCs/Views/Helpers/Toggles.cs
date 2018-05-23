using System.Windows;
using System.Windows.Controls.Primitives;

namespace GPGO_MultiPLCs.Views
{
    public class ToggleText : ToggleButton
    {
        public ToggleText()
        {
            IsVisibleChanged += (s, e) =>
                                {
                                    if (Visibility == Visibility.Visible || Visibility == Visibility.Hidden)
                                    {
                                        IsChecked = false;
                                    }
                                };
        }
    }

    public class ToggleSwich : ToggleButton
    {
    }
}