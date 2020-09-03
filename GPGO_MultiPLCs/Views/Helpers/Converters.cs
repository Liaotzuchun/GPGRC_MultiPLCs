using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace GPGO_MultiPLCs.Views
{
    public class ListTakeCount : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.First() is int count)
            {
                return values.Skip(1).Take(count).ToList();
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}