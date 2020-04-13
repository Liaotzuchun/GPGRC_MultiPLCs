using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using OxyPlot.Axes;

namespace GPGO_MultiPLCs.Views
{
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
}
