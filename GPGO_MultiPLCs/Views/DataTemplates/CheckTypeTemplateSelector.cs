using System;
using System.Windows;
using System.Windows.Controls;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.Views;

public class LogTypeTemplateSelector : DataTemplateSelector
{
    public Type         DataType      { get; set; }
    public DataTemplate OtherTemplate { get; set; }
    public DataTemplate Template      { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container) => item is LogEvent ev && container is FrameworkElement && ev.Value.GetType() == DataType ? Template : OtherTemplate;
}