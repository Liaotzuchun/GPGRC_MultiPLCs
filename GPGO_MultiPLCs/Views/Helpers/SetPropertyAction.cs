using Microsoft.Xaml.Behaviors;
using System.Reflection;
using System.Windows;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    /// 提供給予FrameworkElement事件觸發時設定屬性的功能
    /// </summary>
    public class SetPropertyAction : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(SetPropertyAction));

        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof(object), typeof(SetPropertyAction));

        public object PropertyValue
        {
            get => GetValue(PropertyValueProperty);
            set => SetValue(PropertyValueProperty, value);
        }

        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(object), typeof(SetPropertyAction));

        public object TargetObject
        {
            get => GetValue(TargetObjectProperty);
            set => SetValue(TargetObjectProperty, value);
        }

        protected override void Invoke(object parameter)
        {
            var target = TargetObject ?? AssociatedObject;
            var propertyInfo = target.GetType().GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(target, PropertyValue);
            }
        }
    }
}