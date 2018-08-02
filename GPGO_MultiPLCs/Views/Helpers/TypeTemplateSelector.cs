using System;
using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views
{
    public class CheckTypeTemplateSelector : DataTemplateSelector
    {
        public Type DataType { get; set; }
        public DataTemplate Template { get; set; }
        public DataTemplate OtherTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && (container is FrameworkElement && item.GetType() == DataType))
            {
                return Template;
            }

            return OtherTemplate;
        }
    }
}