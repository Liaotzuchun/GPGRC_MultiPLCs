using System;
using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views
{
    public partial class RangeSlider : UserControl
    {
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0.0, PropertyChanged));

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register(nameof(LowerValue), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0.0, PropertyChanged));

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register(nameof(UpperValue), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(10.0, PropertyChanged));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(10.0, PropertyChanged));

        public static readonly DependencyProperty DisableLowerValueProperty =
            DependencyProperty.Register(nameof(DisableLowerValue), typeof(bool), typeof(RangeSlider), new UIPropertyMetadata(false, DisabledLowerValueChanged));

        private static void DisabledLowerValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (RangeSlider)d;
            slider.SetLowerValueVisibility();
        }

        public bool DisableLowerValue
        {
            get => Convert.ToBoolean(GetValue(DisableLowerValueProperty));
            set => SetValue(DisableLowerValueProperty, value);
        }

        public double LowerValue
        {
            get => Convert.ToDouble(GetValue(LowerValueProperty));
            set => SetValue(LowerValueProperty, value);
        }

        public double Maximum
        {
            get => Convert.ToDouble(GetValue(MaximumProperty));
            set => SetValue(MaximumProperty, value);
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (RangeSlider)d;

            if (e.Property.Name == nameof(Maximum))
            {
                if (slider.Maximum < 0)
                {
                    slider.Maximum = 0;
                }

                if (slider.Minimum > slider.Maximum)
                {
                    slider.Minimum = slider.Maximum;
                }

                slider.UpperValue = slider.Maximum;
                slider.LowerValue = slider.LowerValue;
            }
            else if (e.Property.Name == nameof(Minimum))
            {
                if (slider.Minimum < 0)
                {
                    slider.Minimum = 0;
                }

                if (slider.Maximum < slider.Minimum)
                {
                    slider.Maximum = slider.Minimum;
                }

                slider.UpperValue = slider.UpperValue;
                slider.LowerValue = slider.Minimum;
            }
            else if (e.Property.Name == nameof(LowerValue) && slider.UpperValue < slider.LowerValue)
            {
                slider.UpperValue = slider.LowerValue;
            }
            else if (e.Property.Name == nameof(UpperValue) && slider.UpperValue < slider.LowerValue)
            {
                slider.LowerValue = slider.UpperValue;
            }

            slider.SetProgressBorder();

            PropertyChangedEvent?.Invoke();
        }

        public double Minimum
        {
            get => Convert.ToDouble(GetValue(MinimumProperty));
            set => SetValue(MinimumProperty, value);
        }

        public double UpperValue
        {
            get => Convert.ToDouble(GetValue(UpperValueProperty));
            set => SetValue(UpperValueProperty, value);
        }

        public delegate void PropertyChangedEventEventHandler();

        public static event PropertyChangedEventEventHandler PropertyChangedEvent;

        private void RangeSlider_LayoutUpdated(object sender, EventArgs e)
        {
            SetProgressBorder();
            SetLowerValueVisibility();
        }

        private void SetLowerValueVisibility()
        {
            LowerSlider.Visibility = DisableLowerValue ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SetProgressBorder()
        {
            var val = Maximum - Minimum;
            if (val <= 0)
            {
                TrackBackground.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                var lowerPoint = LowerSlider.ActualWidth * (LowerValue - Minimum) / val;
                var upperPoint = UpperSlider.ActualWidth * (UpperValue - Minimum) / val;
                upperPoint = UpperSlider.ActualWidth - upperPoint;
                TrackBackground.Margin = new Thickness(lowerPoint, 0, upperPoint, 0);
            }
        }

        public RangeSlider()
        {
            InitializeComponent();
            LayoutUpdated += RangeSlider_LayoutUpdated;
        }
    }
}