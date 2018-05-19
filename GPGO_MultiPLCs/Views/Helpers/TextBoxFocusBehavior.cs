﻿using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views.Helpers
{
    public static class TextBoxFocusBehavior
    {
        public static readonly DependencyProperty IsWatermarkEnabled =
            DependencyProperty.RegisterAttached("IsWatermarkEnabled", typeof(bool), typeof(TextBoxFocusBehavior), new UIPropertyMetadata(false, OnIsWatermarkEnabled));

        public static readonly DependencyProperty WatermarkText =
            DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(TextBoxFocusBehavior), new UIPropertyMetadata(string.Empty, OnWatermarkTextChanged));

        public static string GetWatermarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkText);
        }

        public static void SetWatermarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkText, value);
        }

        public static bool GetIsWatermarkEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWatermarkEnabled);
        }

        public static void SetIsWatermarkEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWatermarkEnabled, value);
        }

        private static void OnWatermarkTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.Text = (string)e.NewValue;
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
    }
}