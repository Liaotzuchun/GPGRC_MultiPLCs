using System.ComponentModel;
using System.Windows;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Views
{
    public abstract class BindingDataProxy<T> : Freezable
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(T), typeof(BindingDataProxy<T>), new PropertyMetadata(null));

        [Bindable(true)]
        public T Data
        {
            get => (T)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new DependencyCommand();
        }
    }

    public class FilterGroupProxy : BindingDataProxy<FilterGroup> { }
}