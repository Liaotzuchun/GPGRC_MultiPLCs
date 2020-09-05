﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace GPGO_MultiPLCs.Views
{
    public class ListTakeCount : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 1 && int.TryParse(values.First().ToString(), out var count) && count > 0)
            {
                return values.Skip(1).Take(count).Cast<double>().ToArray();
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}