using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    /// PairValuesControl.xaml 的互動邏輯
    /// </summary>
    public partial class PairValuesControl : UserControl
    {
        public string Value1
        {
            get => (string)GetValue(Value1Property);
            set => SetValue(Value1Property, value);
        }

        public static readonly DependencyProperty Value1Property =
            DependencyProperty.Register("Value1", typeof(string), typeof(PairValuesControl), new UIPropertyMetadata(string.Empty, null));

        public string Value2
        {
            get => (string)GetValue(Value2Property);
            set => SetValue(Value2Property, value);
        }

        public static readonly DependencyProperty Value2Property =
            DependencyProperty.Register("Value2", typeof(string), typeof(PairValuesControl), new UIPropertyMetadata(string.Empty, null));

        public PairValuesControl()
        {
            InitializeComponent();
        }
    }
}
