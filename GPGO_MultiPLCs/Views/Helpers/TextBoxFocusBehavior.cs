using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>提供TextBox在無焦點且無文字時顯示水印文字的能力</summary>
    public static class TextBoxWatermarkBehavior
    {
        public static readonly DependencyProperty IsWatermarkEnabled =
            DependencyProperty.RegisterAttached("IsWatermarkEnabled", typeof(bool), typeof(TextBoxWatermarkBehavior), new UIPropertyMetadata(false, OnIsWatermarkEnabled));

        public static bool GetIsWatermarkEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWatermarkEnabled);
        }

        public static void SetIsWatermarkEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWatermarkEnabled, value);
        }

        public static readonly DependencyProperty WatermarkText =
            DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(TextBoxWatermarkBehavior), new UIPropertyMetadata(string.Empty, OnWatermarkTextChanged));

        public static string GetWatermarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkText);
        }

        public static void SetWatermarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkText, value);
        }

        private static void OnInputTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox tb)
            {
                if (tb.Text == GetWatermarkText(tb))
                {
                    tb.Text = string.Empty;
                }
            }
        }

        private static void OnInputTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox tb)
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    tb.Text = GetWatermarkText(tb);
                }
            }
        }

        private static void OnIsWatermarkEnabled(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                var isEnabled = (bool)e.NewValue;
                if (isEnabled)
                {
                    tb.GotFocus += OnInputTextBoxGotFocus;
                    tb.LostFocus += OnInputTextBoxLostFocus;
                }
                else
                {
                    tb.GotFocus -= OnInputTextBoxGotFocus;
                    tb.LostFocus -= OnInputTextBoxLostFocus;
                }
            }
        }

        private static void OnWatermarkTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.Text = (string)e.NewValue;
            }
        }
    }
}