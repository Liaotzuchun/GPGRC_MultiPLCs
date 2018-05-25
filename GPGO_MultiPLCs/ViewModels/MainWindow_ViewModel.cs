using GPGO_MultiPLCs.Helpers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GPGO_MultiPLCs.ViewModels
{
    public class MainWindow_ViewModel : ViewModelBase
    {
        private int _ViewIndex;

        public int ViewIndex
        {
            get => _ViewIndex;
            set
            {
                _ViewIndex = value;
                NotifyPropertyChanged();
            }
        }

        public PlotModel HistogramView { get; }

        public MainWindow_ViewModel()
        {
            HistogramView = new PlotModel
                            {
                                PlotAreaBackground = OxyColor.FromArgb(0, 0, 0, 0),
                                DefaultFont = "Microsoft JhengHei",
                                PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0),
                                PlotMargins = new OxyThickness(20, 10, 10, 20)
                            };

            var color = OxyColor.FromRgb(50, 70, 60);

            var categoryAxis1 = new CategoryAxis { TickStyle = TickStyle.Inside, AxislineStyle = LineStyle.Solid, AxislineColor = color, GapWidth = 0, MinorStep = 1, Position = AxisPosition.Left };
            categoryAxis1.ActualLabels.Add("1");
            categoryAxis1.ActualLabels.Add("2");
            categoryAxis1.ActualLabels.Add("3");
            categoryAxis1.ActualLabels.Add("4");
            categoryAxis1.ActualLabels.Add("5");
            categoryAxis1.ActualLabels.Add("6");
            categoryAxis1.ActualLabels.Add("7");
            categoryAxis1.ActualLabels.Add("8");
            categoryAxis1.ActualLabels.Add("9");
            categoryAxis1.ActualLabels.Add("10");
            categoryAxis1.ActualLabels.Add("11");
            categoryAxis1.ActualLabels.Add("12");
            categoryAxis1.ActualLabels.Add("13");
            categoryAxis1.ActualLabels.Add("14");
            categoryAxis1.ActualLabels.Add("15");
            categoryAxis1.ActualLabels.Add("16");
            categoryAxis1.ActualLabels.Add("17");
            categoryAxis1.ActualLabels.Add("18");
            categoryAxis1.ActualLabels.Add("19");
            categoryAxis1.ActualLabels.Add("20");

            var XAxis = new LinearAxis
                        {
                            MinimumPadding = 0,
                            MaximumPadding = 0,
                            TickStyle = TickStyle.Inside,
                            MajorGridlineStyle = LineStyle.None,
                            MajorStep = 100,
                            MinorGridlineStyle = LineStyle.None,
                            MinorTickSize = 0,
                            MinorStep = 100,
                            Position = AxisPosition.Bottom,
                            AxislineStyle = LineStyle.Solid,
                            AxislineColor = color,
                            MajorGridlineColor = color,
                            MinorGridlineColor = color,
                            TicklineColor = color,
                            ExtraGridlineColor = color,
                            TextColor = color,
                            Minimum = 0
                        };

            var barSeries1 = new BarSeries { LabelFormatString = "{0}", ValueField = "Value" };

            HistogramView.Axes.Add(categoryAxis1);
            HistogramView.Axes.Add(XAxis);
            HistogramView.Series.Add(barSeries1);
        }
    }
}