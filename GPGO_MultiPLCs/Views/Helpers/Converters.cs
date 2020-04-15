using OxyPlot.Axes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GPGO_MultiPLCs.Views
{
    public class TimespanToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null ? TimeSpanAxis.ToDouble((TimeSpan)value) : 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value != null ? TimeSpanAxis.ToTimeSpan((double)value) : TimeSpan.Zero;
    }
}