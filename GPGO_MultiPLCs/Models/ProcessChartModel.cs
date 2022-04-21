using GPMVVM.Helpers;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using OxyPlot.Legends;

namespace GPGO_MultiPLCs.Models
{
    public class ProcessChartModel
    {
        private FrameworkElement element;
        private string           TimeSpanAxisTitle = "";
        private string           LineSeries0Title  = "";
        private string           LineSeries1Title  = "";
        private string           LineSeries2Title  = "";
        private string           LineSeries3Title  = "";
        private string           LineSeries4Title  = "";
        private string           LineSeries5Title  = "";
        private string           LineSeries6Title  = "";
        private string           LineSeries7Title  = "";
        private string           LineSeries8Title  = "";
        private OxyColor         chartbg           = OxyColor.FromRgb(231, 246, 226);
        private OxyColor         bgcolor           = OxyColor.FromRgb(215, 230, 207);
        private OxyColor         bordercolor       = OxyColor.FromRgb(174, 187, 168);
        private OxyColor         fontcolor         = OxyColor.FromRgb(50,  70,  60);

        private readonly TimeSpanAxis   TimeSpanAxis;
        private readonly LinearAxis     LinearAxis;
        private readonly LineSeries     LineSeries0;
        private readonly LineSeries     LineSeries1;
        private readonly LineSeries     LineSeries2;
        private readonly LineSeries     LineSeries3;
        private readonly LineSeries     LineSeries4;
        private readonly LineSeries     LineSeries5;
        private readonly LineSeries     LineSeries6;
        private readonly LineSeries     LineSeries7;
        private readonly LineSeries     LineSeries8;
        private readonly LineAnnotation LineAnnotation;
        public           PlotModel      ChartView { get; }

        public void SetFrameworkElement(FrameworkElement el)
        {
            element = el;

            UpdateTitle();
            UpdateColor();
        }

        public void UpdateTitle()
        {
            if (element != null)
            {
                if (element.TryFindResource("時間") is string s1)
                {
                    TimeSpanAxisTitle = s1;
                }

                if (element.TryFindResource("爐內溫度1") is string s2)
                {
                    LineSeries1Title = s2;
                }

                if (element.TryFindResource("爐內溫度2") is string s3)
                {
                    LineSeries2Title = s3;
                }

                if (element.TryFindResource("爐內溫度3") is string s4)
                {
                    LineSeries3Title = s4;
                }

                if (element.TryFindResource("爐內溫度4") is string s5)
                {
                    LineSeries4Title = s5;
                }

                if (element.TryFindResource("爐內溫度5") is string s6)
                {
                    LineSeries5Title = s6;
                }

                if (element.TryFindResource("爐內溫度6") is string s7)
                {
                    LineSeries6Title = s7;
                }

                if (element.TryFindResource("爐內溫度7") is string s8)
                {
                    LineSeries7Title = s8;
                }

                if (element.TryFindResource("爐內溫度8") is string s9)
                {
                    LineSeries8Title = s9;
                }

                ChartView.InvalidatePlot(true);
            }
        }

        public void UpdateColor()
        {
            if (element != null)
            {
                if (element.TryFindResource("LightColor") is Color lc)
                {
                    chartbg = OxyColor.FromRgb(lc.R, lc.G, lc.B);
                }

                if (element.TryFindResource("WindowBackgroundColor4") is Color wb4)
                {
                    bordercolor = OxyColor.FromRgb(wb4.R, wb4.G, wb4.B);
                }

                if (element.TryFindResource("WindowBackgroundColor6") is Color wb6)
                {
                    bgcolor = OxyColor.FromRgb(wb6.R, wb6.G, wb6.B);
                }

                if (element.TryFindResource("BaseForegroundColor") is Color bf)
                {
                    fontcolor = OxyColor.FromRgb(bf.R, bf.G, bf.B);
                }

                TimeSpanAxis.TitleColor         = fontcolor;
                TimeSpanAxis.AxislineColor      = bordercolor;
                TimeSpanAxis.MajorGridlineColor = bordercolor;
                TimeSpanAxis.MinorGridlineColor = bordercolor;
                TimeSpanAxis.TicklineColor      = bordercolor;
                TimeSpanAxis.ExtraGridlineColor = bordercolor;
                TimeSpanAxis.TextColor          = fontcolor;
                LinearAxis.TitleColor           = fontcolor;
                LinearAxis.MajorGridlineColor   = bordercolor;
                LinearAxis.MinorGridlineColor   = bordercolor;
                LinearAxis.TicklineColor        = bordercolor;
                LinearAxis.ExtraGridlineColor   = bordercolor;
                LinearAxis.TextColor            = fontcolor;
                LinearAxis.AxislineColor        = bordercolor;
                ChartView.PlotAreaBackground    = chartbg;
                ChartView.PlotAreaBorderColor   = bordercolor;
                ChartView.Legends[0].LegendTitleColor      = fontcolor;
                ChartView.Legends[0].LegendTextColor       = fontcolor;
                ChartView.Legends[0].LegendBorder          = bordercolor;
                ChartView.Legends[0].LegendBackground      = bgcolor;

                ChartView.InvalidatePlot(true);
            }
        }

        public void SetAnnotation(LogEvent ev)
        {
            if (ev != null)
            {
                LineAnnotation.TextColor = ev.Type switch
                                           {
                                               EventType.Normal   => OxyColors.Green,
                                               EventType.Trigger  => OxyColors.Blue,
                                               EventType.Operator => OxyColors.DarkOrange,
                                               _                  => OxyColors.Red
                                           };

                LineAnnotation.Color = LineAnnotation.TextColor;
                ChartView.InvalidatePlot(true);
            }
        }

        public void SetData(ICollection<RecordTemperatures> tps)
        {
            if (tps != null)
            {
                LineSeries8.ItemsSource = tps;
                LineSeries7.ItemsSource = tps;
                LineSeries6.ItemsSource = tps;
                LineSeries5.ItemsSource = tps;
                LineSeries4.ItemsSource = tps;
                LineSeries3.ItemsSource = tps;
                LineSeries2.ItemsSource = tps;
                LineSeries1.ItemsSource = tps;
                LineSeries0.ItemsSource = tps;
                ChartView.InvalidatePlot(true);
            }
        }

        public ProcessChartModel(ObservableConcurrentCollection<RecordTemperatures> tps = null)
        {
            ChartView = new PlotModel
                        {
                            DefaultFont             = "Microsoft JhengHei",
                            PlotAreaBackground      = chartbg,
                            PlotAreaBorderColor     = bordercolor,
                            PlotAreaBorderThickness = new OxyThickness(0,  1,  1, 0),
                            PlotMargins             = new OxyThickness(40, 11, 0, 35),
                            TextColor               = fontcolor,
                        };

            ChartView.Legends.Add(new Legend
                                  {
                                      LegendTitleColor      = fontcolor,
                                      LegendTextColor       = fontcolor,
                                      LegendBorder          = bordercolor,
                                      LegendBackground      = bgcolor,
                                      LegendPlacement       = LegendPlacement.Outside,
                                      LegendPosition        = LegendPosition.RightTop,
                                      LegendOrientation     = LegendOrientation.Vertical,
                                      LegendFontSize        = 12,
                                      LegendTitleFontSize   = 12,
                                      LegendItemOrder       = LegendItemOrder.Reverse,
                                      LegendMargin          = 4,
                                      LegendPadding         = 5,
                                      LegendBorderThickness = 1.0,
                                      LegendItemSpacing     = 8,
                                      LegendLineSpacing     = 2
                                  });

            TimeSpanAxis = new TimeSpanAxis
                           {
                               Title                  = TimeSpanAxisTitle,
                               AxislineColor          = bordercolor,
                               AxislineStyle          = LineStyle.Solid,
                               ExtraGridlineColor     = bordercolor,
                               ExtraGridlineStyle     = LineStyle.Solid,
                               ExtraGridlineThickness = 1,
                               IsPanEnabled           = false,
                               IsZoomEnabled          = false,
                               MajorGridlineColor     = bordercolor,
                               MajorGridlineStyle     = LineStyle.Dot,
                               MajorTickSize          = 0,
                               MaximumPadding         = 0.2,
                               Minimum                = 0,
                               MinimumPadding         = 0,
                               MinorGridlineColor     = bordercolor,
                               MinorGridlineStyle     = LineStyle.None,
                               MinorTickSize          = 0,
                               Position               = AxisPosition.Bottom,
                               StringFormat           = "hh:mm:ss",
                               TickStyle              = TickStyle.Inside,
                               TicklineColor          = bordercolor,
                               TitleColor             = fontcolor
                           };

            LinearAxis = new LinearAxis
                         {
                             Title                  = "°C",
                             AxislineColor          = bordercolor,
                             AxislineStyle          = LineStyle.Solid,
                             ExtraGridlineColor     = bordercolor,
                             ExtraGridlineStyle     = LineStyle.Solid,
                             ExtraGridlineThickness = 1,
                             IsPanEnabled           = false,
                             IsZoomEnabled          = false,
                             MajorGridlineColor     = bordercolor,
                             MajorGridlineStyle     = LineStyle.Dot,
                             MajorTickSize          = 0,
                             MaximumPadding         = 0.2,
                             MinimumPadding         = 0.2,
                             MinorGridlineColor     = bordercolor,
                             MinorGridlineStyle     = LineStyle.None,
                             MinorTickSize          = 0,
                             Position               = AxisPosition.Left,
                             TickStyle              = TickStyle.Inside,
                             TicklineColor          = bordercolor,
                             TitleColor             = fontcolor
                         };

            LineSeries0 = new LineSeries
                          {
                              Title           = LineSeries0Title,
                              DataFieldX      = "Time",
                              DataFieldY      = nameof(RecordTemperatures.PV_ThermostatTemperature),
                              LineStyle       = LineStyle.Solid,
                              MarkerFill      = OxyColors.Red,
                              MarkerSize      = 3,
                              MarkerType      = MarkerType.None,
                              StrokeThickness = 2,
                              Color           = OxyColors.Red,
                          };

            LineSeries1 = new LineSeries
                          {
                              Title           = LineSeries1Title,
                              DataFieldX      = "Time",
                              DataFieldY      = nameof(RecordTemperatures.OvenTemperatures_1),
                              LineStyle       = LineStyle.Solid,
                              MarkerFill      = OxyColors.DarkOrange,
                              MarkerSize      = 3,
                              MarkerType      = MarkerType.None,
                              StrokeThickness = 2,
                              Color           = OxyColors.DarkOrange,
                          };

            LineSeries2 = new LineSeries
                          {
                              Title           = LineSeries2Title,
                              DataFieldX      = "Time",
                              DataFieldY      = nameof(RecordTemperatures.OvenTemperatures_2),
                              LineStyle       = LineStyle.Solid,
                              MarkerFill      = OxyColors.Gold,
                              MarkerSize      = 3,
                              MarkerType      = MarkerType.None,
                              StrokeThickness = 2,
                              Color           = OxyColors.Gold,
                              ItemsSource     = tps
                          };

            LineSeries3 = new LineSeries
                          {
                              Title           = LineSeries3Title,
                              DataFieldX      = "Time",
                              DataFieldY      = nameof(RecordTemperatures.OvenTemperatures_3),
                              LineStyle       = LineStyle.Solid,
                              MarkerFill      = OxyColors.Lime,
                              MarkerSize      = 3,
                              MarkerType      = MarkerType.None,
                              StrokeThickness = 2,
                              Color           = OxyColors.Lime,
                          };

            LineSeries4 = new LineSeries
                          {
                              Title           = LineSeries4Title,
                              DataFieldX      = "Time",
                              DataFieldY      = nameof(RecordTemperatures.OvenTemperatures_4),
                              LineStyle       = LineStyle.Solid,
                              MarkerFill      = OxyColors.DodgerBlue,
                              MarkerSize      = 3,
                              MarkerType      = MarkerType.None,
                              StrokeThickness = 2,
                              Color           = OxyColors.DodgerBlue,
                          };

            LineSeries5 = new LineSeries
                          {
                              Title           = LineSeries5Title,
                              DataFieldX      = "Time",
                              DataFieldY      = nameof(RecordTemperatures.OvenTemperatures_5),
                              LineStyle       = LineStyle.Solid,
                              MarkerFill      = OxyColors.DarkOrchid,
                              MarkerSize      = 3,
                              MarkerType      = MarkerType.None,
                              StrokeThickness = 2,
                              Color           = OxyColors.DarkOrchid,
                          };

            LineSeries6 = new LineSeries
                          {
                              Title           = LineSeries6Title,
                              DataFieldX      = "Time",
                              DataFieldY      = nameof(RecordTemperatures.OvenTemperatures_6),
                              LineStyle       = LineStyle.Solid,
                              MarkerFill      = OxyColors.Magenta,
                              MarkerSize      = 3,
                              MarkerType      = MarkerType.None,
                              StrokeThickness = 2,
                              Color           = OxyColors.Magenta,
                          };

            LineSeries7 = new LineSeries
                          {
                              Title           = LineSeries7Title,
                              DataFieldX      = "Time",
                              DataFieldY      = nameof(RecordTemperatures.OvenTemperatures_7),
                              LineStyle       = LineStyle.Solid,
                              MarkerFill      = OxyColors.Brown,
                              MarkerSize      = 3,
                              MarkerType      = MarkerType.None,
                              StrokeThickness = 2,
                              Color           = OxyColors.Brown,
                          };

            LineSeries8 = new LineSeries
                          {
                              Title           = LineSeries8Title,
                              DataFieldX      = "Time",
                              DataFieldY      = nameof(RecordTemperatures.OvenTemperatures_8),
                              LineStyle       = LineStyle.Solid,
                              MarkerFill      = OxyColors.BurlyWood,
                              MarkerSize      = 3,
                              MarkerType      = MarkerType.None,
                              StrokeThickness = 2,
                              Color           = OxyColors.BurlyWood,
                          };

            LineAnnotation = new LineAnnotation
                             {
                                 FontSize                = 12,
                                 StrokeThickness         = 1,
                                 TextColor               = fontcolor,
                                 TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center,
                                 TextOrientation         = AnnotationTextOrientation.Horizontal,
                                 TextVerticalAlignment   = OxyPlot.VerticalAlignment.Bottom,
                                 Type                    = LineAnnotationType.Vertical,
                                 X                       = 0,
                                 Color                   = fontcolor
                             };

            ChartView.Axes.Add(LinearAxis);
            ChartView.Axes.Add(TimeSpanAxis);
            ChartView.Series.Add(LineSeries8);
            ChartView.Series.Add(LineSeries7);
            ChartView.Series.Add(LineSeries6);
            ChartView.Series.Add(LineSeries5);
            ChartView.Series.Add(LineSeries4);
            ChartView.Series.Add(LineSeries3);
            ChartView.Series.Add(LineSeries2);
            ChartView.Series.Add(LineSeries1);
            ChartView.Series.Add(LineSeries0);

            if (tps != null)
            {
                LineSeries8.ItemsSource = tps;
                LineSeries7.ItemsSource = tps;
                LineSeries6.ItemsSource = tps;
                LineSeries5.ItemsSource = tps;
                LineSeries4.ItemsSource = tps;
                LineSeries3.ItemsSource = tps;
                LineSeries2.ItemsSource = tps;
                LineSeries1.ItemsSource = tps;
                LineSeries0.ItemsSource = tps;

                tps.CollectionChanged += (_, _) =>
                                         {
                                             ChartView.InvalidatePlot(true);
                                         };
            }
        }
    }
}