using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace GPGO_MultiPLCs.Views
{
    public class BindableBinding : MarkupExtension
    {
        private readonly Binding binding = new Binding();

        public IValueConverter Converter { get; set; }

        public Binding ConverterBinding { get; set; }

        public CultureInfo ConverterCulture
        {
            get => binding.ConverterCulture;
            set => binding.ConverterCulture = value;
        }

        public object ConverterParameter { get; set; }

        public Binding ConverterParameterBinding { get; set; }

        public string ElementName
        {
            get => binding.ElementName;
            set => binding.ElementName = value;
        }

        public BindingMode Mode
        {
            get => binding.Mode;
            set => binding.Mode = value;
        }

        public PropertyPath Path
        {
            get => binding.Path;
            set => binding.Path = value;
        }

        public RelativeSource RelativeSource
        {
            get => binding.RelativeSource;
            set => binding.RelativeSource = value;
        }

        public object Source
        {
            get => binding.Source;
            set => binding.Source = value;
        }

        public string StringFormat { get; set; }

        public Binding StringFormatBinding { get; set; }

        public string XPath
        {
            get => binding.XPath;
            set => binding.XPath = value;
        }

        private class InternalConverter : IMultiValueConverter
        {
            object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                var valueIndex = 1;

                var converterParameter = parameter;
                if (binding.ConverterParameterBinding != null)
                {
                    converterParameter = values[valueIndex++];
                }

                lastConverterParameter = converterParameter;

                var converter = binding.Converter;
                if (binding.ConverterBinding != null)
                {
                    converter = values[valueIndex++] as IValueConverter;
                }

                lastConverter = converter;

                var stringFormat = binding.StringFormat;
                if (binding.StringFormatBinding != null)
                {
                    stringFormat = values[valueIndex++] as string;
                }

                var value = values[0];
                if (converter != null)
                {
                    value = converter.Convert(value, targetType, converterParameter, culture);
                }

                if (stringFormat != null)
                {
                    value = string.Format(stringFormat, value);
                }

                return value;
            }

            object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                return lastConverter != null ? new[] { lastConverter.ConvertBack(value, targetTypes[0], lastConverterParameter, culture) } : new[] { value };
            }

            private readonly BindableBinding binding;
            private IValueConverter lastConverter;
            private object lastConverterParameter;

            public InternalConverter(BindableBinding binding) => this.binding = binding;
        }

        public BindableBinding()
        {
        }

        public BindableBinding(PropertyPath path) => binding.Path = path;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var multiBinding = new MultiBinding { Mode = binding.Mode, ConverterCulture = binding.ConverterCulture, Converter = new InternalConverter(this) };
            multiBinding.Bindings.Add(binding);

            if (ConverterParameterBinding != null)
            {
                multiBinding.Bindings.Add(ConverterParameterBinding);
            }
            else
            {
                multiBinding.ConverterParameter = ConverterParameter;
            }

            if (ConverterBinding != null)
            {
                multiBinding.Bindings.Add(ConverterBinding);
            }

            if (StringFormatBinding != null)
            {
                multiBinding.Bindings.Add(StringFormatBinding);
            }

            return multiBinding.ProvideValue(serviceProvider);
        }
    }
}