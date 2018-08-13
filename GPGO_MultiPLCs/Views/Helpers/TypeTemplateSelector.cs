using System;
using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>判別資料型別，分別在符合與不符合時套用不同的DataTemplate</summary>
    public class CheckTypeTemplateSelector : DataTemplateSelector
    {
        public Type DataType { get; set; }
        public DataTemplate OtherTemplate { get; set; }
        public DataTemplate Template { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && container is FrameworkElement && item.GetType() == DataType)
            {
                return Template;
            }

            return OtherTemplate;
        }
    }
}