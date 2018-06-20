using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using GPGO_MultiPLCs.Models;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Wpf;
using LineAnnotation = OxyPlot.Wpf.LineAnnotation;
using TimeSpanAxis = OxyPlot.Axes.TimeSpanAxis;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    ///     TraceabilityView.xaml 的互動邏輯
    /// </summary>
    public partial class TraceabilityView : UserControl
    {
        private void CB_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((ComboBox)sender).IsEnabled)
            {
                CB.SelectedItem = CB.Items.IsEmpty ? -1 : CB.Items[0];
            }
            else
            {
                CB.SelectedItem = -1;
            }
        }

        private void Plot_Loaded(object sender, RoutedEventArgs e)
        {
            var p = (Plot)sender;
            p.Annotations.Clear();

            foreach (var alarm in ((ProcessInfo)p.DataContext).AlarmList)
            {
                p.Annotations.Add(new LineAnnotation {
                                                         Type = LineAnnotationType.Vertical,
                                                         X = TimeSpanAxis.ToDouble(alarm.Time),
                                                         Text = alarm.Description,
                                                         TextRotation = 0.5,
                                                         TextColor = Colors.Red,
                                                         StrokeThickness = 1,
                                                         Color = Colors.Red
                                                     });
            }

            p.InvalidatePlot();
        }

        private void TotalTB_Checked(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "全部站點";
        }

        private void TotalTB_Unchecked(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "指定站點";
        }

        public TraceabilityView()
        {
            InitializeComponent();
        }
    }
}