using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace GPGRC_MultiPLCs.Views;

public class DoublesTakeCount : IMultiValueConverter
{
    #region Interface Implementations
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values.Length > 1 && int.TryParse(values.First().ToString(), out var count) && count > 0 ?
                                                                                                           values.Skip(1).Take(count).Cast<double>().ToList() :
                                                                                                           (object?)null;

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    #endregion
}

public class IsEqual : IMultiValueConverter
{
    #region Interface Implementations
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture) => values.Length > 1 &&
                                                                                                       values[0] != null &&
                                                                                                       values[1] != null &&
                                                                                                       values[0]!.ToString().Trim().Equals(values[1]!.ToString().Trim());

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    #endregion
}

public class EqualBrush : IMultiValueConverter
{
    #region Interface Implementations
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture) => values.Length > 1 &&
                                                                                                       values[0] != null &&
                                                                                                       values[1] != null &&
                                                                                                       values[0]!.ToString().Trim().Equals(values[1]!.ToString().Trim()) ?
                                                                                                           Brushes.Green :
                                                                                                           (object)Brushes.Red;

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    #endregion
}