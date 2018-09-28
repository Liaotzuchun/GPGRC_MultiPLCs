using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>以binding的path搜尋資源</summary>
    public class StringToResource : MarkupExtension, IValueConverter
    {
        private FrameworkElement _target;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                var obj = _target?.TryFindResource(str) ?? Application.Current.TryFindResource(str);
                return obj ?? str;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IRootObjectProvider)) is IRootObjectProvider rootObjectProvider)
            {
                _target = rootObjectProvider.RootObject as FrameworkElement;
            }

            return this;
        }
    }
}