using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>讓Resourcekey可提供binding</summary>
    public sealed class ResourceBinding : MarkupExtension
    {
        public static readonly DependencyProperty ResourceBindingKeyHelperProperty =
            DependencyProperty.RegisterAttached("ResourceBindingKeyHelper", typeof(object), typeof(ResourceBinding), new PropertyMetadata(null, ResourceKeyChanged));

        private static void ResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newVal = e.NewValue as Tuple<object, DependencyProperty>;

            if (!(d is FrameworkElement target) || newVal == null)
            {
                return;
            }

            var dp = newVal.Item2;

            if (newVal.Item1 == null)
            {
                target.SetValue(dp, dp.GetMetadata(target).DefaultValue);

                return;
            }

            if (target.TryFindResource(newVal.Item1.ToString()) == null)
            {
                target.SetValue(dp, dp.PropertyType == typeof(string) || dp.Name == "Header" ? newVal.Item1.ToString() : newVal.Item1);
            }
            else
            {
                target.SetResourceReference(dp, dp.PropertyType == typeof(string) || dp.Name == "Header" ? newVal.Item1.ToString() : newVal.Item1);
            }
        }

        public static object GetResourceBindingKeyHelper(DependencyObject obj)
        {
            return obj.GetValue(ResourceBindingKeyHelperProperty);
        }

        public static void SetResourceBindingKeyHelper(DependencyObject obj, object value)
        {
            obj.SetValue(ResourceBindingKeyHelperProperty, value);
        }

        [DefaultValue(null)]
        public IValueConverter Converter { get; set; }

        [DefaultValue(null)]
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture { get; set; }

        [DefaultValue(null)]
        public object ConverterParameter { get; set; }

        [DefaultValue(null)]
        public string ElementName { get; set; }

        public object FallbackValue { get; set; }

        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode { get; set; }

        public PropertyPath Path { get; set; }

        [DefaultValue(null)]
        public RelativeSource RelativeSource { get; set; }

        public object Source { get; set; }

        [DefaultValue(UpdateSourceTrigger.Default)]
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

        [DefaultValue(null)]
        public string XPath { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTargetService = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (provideValueTargetService == null)
            {
                return null;
            }

            if (provideValueTargetService.TargetObject != null && provideValueTargetService.TargetObject.GetType().FullName == "System.Windows.SharedDp")
            {
                return this;
            }

            var targetProperty = provideValueTargetService.TargetProperty as DependencyProperty;
            if (!(provideValueTargetService.TargetObject is FrameworkElement targetObject) || targetProperty == null)
            {
                return null;
            }

            var binding = new Binding
                          {
                              Path = Path,
                              XPath = XPath,
                              Mode = Mode,
                              UpdateSourceTrigger = UpdateSourceTrigger,
                              Converter = Converter,
                              ConverterParameter = ConverterParameter,
                              ConverterCulture = ConverterCulture
                          };

            if (RelativeSource != null)
            {
                binding.RelativeSource = RelativeSource;
            }

            if (ElementName != null)
            {
                binding.ElementName = ElementName;
            }

            if (Source != null)
            {
                binding.Source = Source;
            }

            binding.FallbackValue = FallbackValue;

            var multiBinding = new MultiBinding { Converter = HelperConverter.Current, ConverterParameter = targetProperty };

            multiBinding.Bindings.Add(binding);

            multiBinding.NotifyOnSourceUpdated = true;

            targetObject.SetBinding(ResourceBindingKeyHelperProperty, multiBinding);

            return null;
        }

        private class HelperConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                return Tuple.Create(values[0], (DependencyProperty)parameter);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            public static readonly HelperConverter Current = new HelperConverter();
        }

        public ResourceBinding()
        {
        }

        public ResourceBinding(string path)
        {
            Path = new PropertyPath(path);
        }
    }
}