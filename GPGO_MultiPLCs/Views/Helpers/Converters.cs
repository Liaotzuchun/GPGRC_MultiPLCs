using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using OxyPlot.Axes;

namespace GPGO_MultiPLCs.Views
{
    public class BoolOnOff : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool booValue ? booValue ? "ON" : "OFF" : "OFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string strValue && strValue == "ON";
        }
    }

    public class TimespanAgo : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DateTime time ? (DateTime.Now - time) : default(TimeSpan);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ConstValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DateTimeZeroToEmpty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DateTime time && time.Ticks == 0 ? "" : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string str && string.IsNullOrEmpty(str) ? new DateTime() : value;
        }
    }

    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var returnValue = 0;
            if (parameter is Type type)
            {
                if (value != null)
                {
                    returnValue = (int)Enum.Parse(type, value.ToString());
                }
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumValue = default(Enum);
            if (parameter is Type type)
            {
                if (value != null)
                {
                    enumValue = (Enum)Enum.Parse(type, value.ToString());
                }
            }

            return enumValue;
        }
    }

    public class ToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumValue = default(Enum);
            if (parameter is Type type)
            {
                if (value != null)
                {
                    enumValue = (Enum)Enum.Parse(type, value.ToString());
                }
            }

            return enumValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var returnValue = 0;
            if (parameter is Type type)
            {
                if (value != null)
                {
                    returnValue = (int)Enum.Parse(type, value.ToString());
                }
            }

            return returnValue;
        }
    }

    public class TimespanToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? TimeSpanAxis.ToDouble((TimeSpan)value) : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? TimeSpanAxis.ToTimeSpan((double)value) : TimeSpan.Zero;
        }
    }

    public class StatusColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Colors.LightGray;
            }

            var val = (int)value;

            switch (val)
            {
                case 0:

                    return Colors.Red;

                case 1:

                    return Colors.Lime;

                default:

                    return Colors.LightGray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return -1;
            }

            var val = (Color)value;

            if (val.Equals(Colors.Red))
            {
                return 0;
            }

            if (val.Equals(Colors.Lime))
            {
                return 1;
            }

            return -1;
        }
    }

    public class IndexBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (int)value > -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return -1;
        }
    }

    public class StrNotBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class StrBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() != "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class DateTimeCheckVis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && ((DateTime)value).Ticks != 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTime();
        }
    }

    public class StrCheckVis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() != "" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class VisBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility)value == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || !(bool)value ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public class BoolVis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || !(bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility)value == Visibility.Visible;
        }
    }

    public class BoolNotVis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility)value == Visibility.Collapsed;
        }
    }

    public class BoolNot : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value;
        }
    }

    public class CountVis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value is int val && val > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NullVis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || string.IsNullOrEmpty(value.ToString()) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class IsNotNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection v)
            {
                return v.Count > 0;
            }

            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class RatioValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return a * b;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return a / b;
            }

            return value;
        }
    }

    public class DivideByValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return a / b;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return a * b;
            }

            return value;
        }
    }

    public class AddValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return a + b;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return a - b;
            }

            return value;
        }
    }

    public class SubValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return a - b;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return a + b;
            }

            return value;
        }
    }

    public class SubByValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return b - a;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && double.TryParse(value.ToString(), out var a) && double.TryParse(parameter.ToString(), out var b))
            {
                return b - a;
            }

            return value;
        }
    }

    public class IsEqualValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? parameter : Activator.CreateInstance(targetType);
        }
    }

    public class IsLessThanValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) <= System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class IsMoreThanValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) >= System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class GetDayofDate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? "" : ((DateTime?)value).Value.Day.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}