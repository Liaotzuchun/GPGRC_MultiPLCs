using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace GPGO_MultiPLCs.Views;

public class DoublesTakeCount : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length > 1 && int.TryParse(values.First().ToString(), out var count) && count > 0)
        {
            return values.Skip(1).Take(count).Cast<double>().ToList();
        }

        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class IsEqual : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.Length > 1     &&
               values[0]     != null &&
               values[1]     != null &&
               values[0].ToString().Trim().Equals(values[1].ToString().Trim());
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class EqualBrush : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length > 1 && 
            values[0] != null && 
            values[1] != null && 
            values[0].ToString().Trim().Equals(values[1].ToString().Trim()))
        {
            return Brushes.Green;
        }

        return Brushes.Red;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}