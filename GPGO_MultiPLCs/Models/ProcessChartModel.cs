using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace GPGO_MultiPLCs.Models;

public class ProcessChartModel
{
    private          OxyColor Bgcolor;
    private          OxyColor Bordercolor;
    private          OxyColor Fontcolor;
    private readonly OxyColor S0color = OxyColors.Red;
    private readonly OxyColor S1color = OxyColors.DarkOrange;
    private readonly OxyColor S2color = OxyColors.Gold;
    private readonly OxyColor S3color = OxyColors.Lime;
    private readonly OxyColor S4color = OxyColors.DodgerBlue;
    private readonly OxyColor S5color = OxyColors.DarkOrchid;
    private readonly OxyColor S6color = OxyColors.Magenta;
    private readonly OxyColor S7color = OxyColors.Brown;
    private readonly OxyColor S8color = OxyColors.BurlyWood;

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

    public void RefreshView(bool UpdateData)
    {
        lock (ChartView.SyncRoot)
        {
            ChartView.InvalidatePlot(UpdateData);
        }
    }

    public void SetFrameworkElement(FrameworkElement el)
    {
        UpdateTitle(el);
        UpdateColor(el);
    }

    public void UpdateTitle(FrameworkElement element)
    {
        if (element != null)
        {
            if (element.TryFindResource("時間") is string s1)
            {
                TimeSpanAxis.Title = s1;
            }

            if (element.TryFindResource("爐內溫度1") is string s2)
            {
                LineSeries1.Title = s2;
            }

            if (element.TryFindResource("爐內溫度2") is string s3)
            {
                LineSeries2.Title = s3;
            }

            if (element.TryFindResource("爐內溫度3") is string s4)
            {
                LineSeries3.Title = s4;
            }

            if (element.TryFindResource("爐內溫度4") is string s5)
            {
                LineSeries4.Title = s5;
            }

            if (element.TryFindResource("爐內溫度5") is string s6)
            {
                LineSeries5.Title = s6;
            }

            if (element.TryFindResource("爐內溫度6") is string s7)
            {
                LineSeries6.Title = s7;
            }

            if (element.TryFindResource("爐內溫度7") is string s8)
            {
                LineSeries7.Title = s8;
            }

            if (element.TryFindResource("爐內溫度8") is string s9)
            {
                LineSeries8.Title = s9;
            }

            RefreshView(true);
        }
    }

    public void UpdateColor(FrameworkElement element)
    {
        if (element != null)
        {
            if (element.TryFindResource("LightColor") is Color lc)
            {
                Bgcolor = OxyColor.FromRgb(lc.R, lc.G, lc.B);
            }

            if (element.TryFindResource("WindowBackgroundColor4") is Color wb4)
            {
                Bordercolor = OxyColor.FromRgb(wb4.R, wb4.G, wb4.B);
            }

            if (element.TryFindResource("BaseForegroundColor") is Color bf)
            {
                Fontcolor = OxyColor.FromRgb(bf.R, bf.G, bf.B);
            }

            TimeSpanAxis.TitleColor               = Fontcolor;
            TimeSpanAxis.AxislineColor            = Bordercolor;
            TimeSpanAxis.MajorGridlineColor       = Bordercolor;
            TimeSpanAxis.MinorGridlineColor       = Bordercolor;
            TimeSpanAxis.TicklineColor            = Bordercolor;
            TimeSpanAxis.ExtraGridlineColor       = Bordercolor;
            TimeSpanAxis.TextColor                = Fontcolor;
            LinearAxis.TitleColor                 = Fontcolor;
            LinearAxis.MajorGridlineColor         = Bordercolor;
            LinearAxis.MinorGridlineColor         = Bordercolor;
            LinearAxis.TicklineColor              = Bordercolor;
            LinearAxis.ExtraGridlineColor         = Bordercolor;
            LinearAxis.TextColor                  = Fontcolor;
            LinearAxis.AxislineColor              = Bordercolor;
            ChartView.PlotAreaBackground          = Bgcolor;
            ChartView.PlotAreaBorderColor         = Bordercolor;
            ChartView.Legends[0].LegendTitleColor = Fontcolor;
            ChartView.Legends[0].LegendTextColor  = Fontcolor;
            ChartView.Legends[0].LegendBorder     = Bordercolor;
            ChartView.Legends[0].LegendBackground = Bgcolor;

            RefreshView(true);
        }
    }

    public void SetAnnotation(LogEvent ev)
    {
        if (ev == null)
        {
            ChartView.Annotations.Clear();
        }
        else
        {
            if (ChartView.Annotations.Count <= 0)
            {
                ChartView.Annotations.Add(LineAnnotation);
            }

            LineAnnotation.X = TimeSpanAxis.ToDouble(ev.Time);
            LineAnnotation.Color = ev.Type switch
                                   {
                                       EventType.Normal        => OxyColors.Green,
                                       EventType.StatusChanged => OxyColors.DodgerBlue,
                                       EventType.Trigger       => OxyColors.Blue,
                                       EventType.Operator      => OxyColors.DarkOrange,
                                       EventType.Alert         => OxyColors.Magenta,
                                       EventType.Alarm         => OxyColors.Red,
                                       _                       => OxyColors.Gray
                                   };

            LineAnnotation.TextColor = LineAnnotation.Color;
            var value = ev.Value is bool b ? b ? "ON" : "OFF" : ev.Value.ToString();
            LineAnnotation.Text      = $"{ev.Description}: {value}";
        }

        RefreshView(true);
    }

    public void Clear()
    {
        LineSeries0.Points.Clear();
        LineSeries1.Points.Clear();
        LineSeries2.Points.Clear();
        LineSeries3.Points.Clear();
        LineSeries4.Points.Clear();
        LineSeries5.Points.Clear();
        LineSeries6.Points.Clear();
        LineSeries7.Points.Clear();
        LineSeries8.Points.Clear();

        RefreshView(true);
    }

    public void AddDate(RecordTemperatures tp)
    {
        var t = TimeSpanAxis.ToDouble(tp.Time);
        LineSeries0.Points.Add(new DataPoint(t, tp.PV_ThermostatTemperature));
        LineSeries1.Points.Add(new DataPoint(t, tp.OvenTemperatures_1));
        LineSeries2.Points.Add(new DataPoint(t, tp.OvenTemperatures_2));
        LineSeries3.Points.Add(new DataPoint(t, tp.OvenTemperatures_3));
        LineSeries4.Points.Add(new DataPoint(t, tp.OvenTemperatures_4));
        LineSeries5.Points.Add(new DataPoint(t, tp.OvenTemperatures_5));
        LineSeries6.Points.Add(new DataPoint(t, tp.OvenTemperatures_6));
        LineSeries7.Points.Add(new DataPoint(t, tp.OvenTemperatures_7));
        LineSeries8.Points.Add(new DataPoint(t, tp.OvenTemperatures_8));

        RefreshView(true);
    }

    public void SetData(ICollection<RecordTemperatures> tps)
    {
        if (tps != null)
        {
            LineSeries0.Points.Clear();
            LineSeries1.Points.Clear();
            LineSeries2.Points.Clear();
            LineSeries3.Points.Clear();
            LineSeries4.Points.Clear();
            LineSeries5.Points.Clear();
            LineSeries6.Points.Clear();
            LineSeries7.Points.Clear();
            LineSeries8.Points.Clear();

            foreach (var tp in tps)
            {
                var t = TimeSpanAxis.ToDouble(tp.Time);
                LineSeries0.Points.Add(new DataPoint(t, tp.PV_ThermostatTemperature));
                LineSeries1.Points.Add(new DataPoint(t, tp.OvenTemperatures_1));
                LineSeries2.Points.Add(new DataPoint(t, tp.OvenTemperatures_2));
                LineSeries3.Points.Add(new DataPoint(t, tp.OvenTemperatures_3));
                LineSeries4.Points.Add(new DataPoint(t, tp.OvenTemperatures_4));
                LineSeries5.Points.Add(new DataPoint(t, tp.OvenTemperatures_5));
                LineSeries6.Points.Add(new DataPoint(t, tp.OvenTemperatures_6));
                LineSeries7.Points.Add(new DataPoint(t, tp.OvenTemperatures_7));
                LineSeries8.Points.Add(new DataPoint(t, tp.OvenTemperatures_8));
            }

            ChartView.InvalidatePlot(true);
        }
    }

    public ProcessChartModel()
    {
        ChartView = new PlotModel
                    {
                        DefaultFont             = "Microsoft JhengHei",
                        PlotAreaBorderThickness = new OxyThickness(0,  1,  1, 0),
                        PlotMargins             = new OxyThickness(40, 0, 0, 35)
                    };

        ChartView.Legends.Add(new Legend
                              {
                                  LegendFont            = "Microsoft JhengHei",
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
                           AxislineStyle          = LineStyle.Solid,
                           ExtraGridlineStyle     = LineStyle.Solid,
                           ExtraGridlineThickness = 1,
                           IsPanEnabled           = false,
                           IsZoomEnabled          = false,
                           MajorGridlineStyle     = LineStyle.Dot,
                           MajorTickSize          = 0,
                           MaximumPadding         = 0,
                           Minimum                = 0,
                           MinimumPadding         = 0,
                           MinorGridlineStyle     = LineStyle.None,
                           MinorTickSize          = 0,
                           Position               = AxisPosition.Bottom,
                           StringFormat           = "hh:mm:ss",
                           TickStyle              = TickStyle.Inside
                       };

        LinearAxis = new LinearAxis
                     {
                         Title                  = "°C",
                         AxislineStyle          = LineStyle.Solid,
                         ExtraGridlineStyle     = LineStyle.Solid,
                         ExtraGridlineThickness = 1,
                         IsPanEnabled           = false,
                         IsZoomEnabled          = false,
                         MajorGridlineStyle     = LineStyle.Dot,
                         MajorTickSize          = 0,
                         MaximumPadding         = 0.15,
                         MinimumPadding         = 0.15,
                         MinorGridlineStyle     = LineStyle.None,
                         MinorTickSize          = 0,
                         Position               = AxisPosition.Left,
                         TickStyle              = TickStyle.Inside
                     };

        LineSeries0 = new LineSeries
                      {
                          LineStyle           = LineStyle.Solid,
                          MarkerSize          = 3,
                          MarkerType          = MarkerType.None,
                          StrokeThickness     = 2,
                          Decimator           = Decimator.Decimate,
                          TrackerFormatString = "{0}\n{1}: {2:hh\\:mm\\:ss\\.fff}\n{4:F3}{3}"
        };

        LineSeries1 = new LineSeries
                      {
                          LineStyle           = LineStyle.Solid,
                          MarkerSize          = 3,
                          MarkerType          = MarkerType.None,
                          StrokeThickness     = 2,
                          Decimator           = Decimator.Decimate,
                          TrackerFormatString = "{0}\n{1}: {2:hh\\:mm\\:ss\\.fff}\n{4:F3}{3}"
        };

        LineSeries2 = new LineSeries
                      {
                          LineStyle           = LineStyle.Solid,
                          MarkerSize          = 3,
                          MarkerType          = MarkerType.None,
                          StrokeThickness     = 2,
                          Decimator           = Decimator.Decimate,
                          TrackerFormatString = "{0}\n{1}: {2:hh\\:mm\\:ss\\.fff}\n{4:F3}{3}"
        };

        LineSeries3 = new LineSeries
                      {
                          LineStyle           = LineStyle.Solid,
                          MarkerSize          = 3,
                          MarkerType          = MarkerType.None,
                          StrokeThickness     = 2,
                          Decimator           = Decimator.Decimate,
                          TrackerFormatString = "{0}\n{1}: {2:hh\\:mm\\:ss\\.fff}\n{4:F3}{3}"
        };

        LineSeries4 = new LineSeries
                      {
                          LineStyle           = LineStyle.Solid,
                          MarkerSize          = 3,
                          MarkerType          = MarkerType.None,
                          StrokeThickness     = 2,
                          Decimator           = Decimator.Decimate,
                          TrackerFormatString = "{0}\n{1}: {2:hh\\:mm\\:ss\\.fff}\n{4:F3}{3}"
        };

        LineSeries5 = new LineSeries
                      {
                          LineStyle           = LineStyle.Solid,
                          MarkerSize          = 3,
                          MarkerType          = MarkerType.None,
                          StrokeThickness     = 2,
                          Decimator           = Decimator.Decimate,
                          TrackerFormatString = "{0}\n{1}: {2:hh\\:mm\\:ss\\.fff}\n{4:F3}{3}"
        };

        LineSeries6 = new LineSeries
                      {
                          LineStyle           = LineStyle.Solid,
                          MarkerSize          = 3,
                          MarkerType          = MarkerType.None,
                          StrokeThickness     = 2,
                          Decimator           = Decimator.Decimate,
                          TrackerFormatString = "{0}\n{1}: {2:hh\\:mm\\:ss\\.fff}\n{4:F3}{3}"
        };

        LineSeries7 = new LineSeries
                      {
                          LineStyle           = LineStyle.Solid,
                          MarkerSize          = 3,
                          MarkerType          = MarkerType.None,
                          StrokeThickness     = 2,
                          Decimator           = Decimator.Decimate,
                          TrackerFormatString = "{0}\n{1}: {2:hh\\:mm\\:ss\\.fff}\n{4:F3}{3}"
        };

        LineSeries8 = new LineSeries
                      {
                          LineStyle           = LineStyle.Solid,
                          MarkerSize          = 3,
                          MarkerType          = MarkerType.None,
                          StrokeThickness     = 2,
                          Decimator           = Decimator.Decimate,
                          TrackerFormatString = "{0}\n{1}: {2:hh\\:mm\\:ss\\.fff}\n{4:F3}{3}"
        };

        LineAnnotation = new LineAnnotation
                         {
                             FontSize                = 12,
                             StrokeThickness         = 1,
                             TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                             TextOrientation         = AnnotationTextOrientation.Horizontal,
                             TextVerticalAlignment   = OxyPlot.VerticalAlignment.Top,
                             TextMargin              = 0,
                             TextPadding             = 2,
                             Type                    = LineAnnotationType.Vertical,
                             X                       = 0
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

        TimeSpanAxis.TitleColor               = Fontcolor;
        TimeSpanAxis.AxislineColor            = Bordercolor;
        TimeSpanAxis.MajorGridlineColor       = Bordercolor;
        TimeSpanAxis.MinorGridlineColor       = Bordercolor;
        TimeSpanAxis.TicklineColor            = Bordercolor;
        TimeSpanAxis.ExtraGridlineColor       = Bordercolor;
        TimeSpanAxis.TextColor                = Fontcolor;
        LinearAxis.TitleColor                 = Fontcolor;
        LinearAxis.MajorGridlineColor         = Bordercolor;
        LinearAxis.MinorGridlineColor         = Bordercolor;
        LinearAxis.TicklineColor              = Bordercolor;
        LinearAxis.ExtraGridlineColor         = Bordercolor;
        LinearAxis.TextColor                  = Fontcolor;
        LinearAxis.AxislineColor              = Bordercolor;
        ChartView.PlotAreaBackground          = Bgcolor;
        ChartView.PlotAreaBorderColor         = Bordercolor;
        ChartView.Legends[0].LegendTitleColor = Fontcolor;
        ChartView.Legends[0].LegendTextColor  = Fontcolor;
        ChartView.Legends[0].LegendBorder     = Bordercolor;
        ChartView.Legends[0].LegendBackground = Bgcolor;

        LineSeries0.MarkerFill = S0color;
        LineSeries0.Color      = S0color;
        LineSeries1.MarkerFill = S1color;
        LineSeries1.Color      = S1color;
        LineSeries2.MarkerFill = S2color;
        LineSeries2.Color      = S2color;
        LineSeries3.MarkerFill = S3color;
        LineSeries3.Color      = S3color;
        LineSeries4.MarkerFill = S4color;
        LineSeries4.Color      = S4color;
        LineSeries5.MarkerFill = S5color;
        LineSeries5.Color      = S5color;
        LineSeries6.MarkerFill = S6color;
        LineSeries6.Color      = S6color;
        LineSeries7.MarkerFill = S7color;
        LineSeries7.Color      = S7color;
        LineSeries8.MarkerFill = S8color;
        LineSeries8.Color      = S8color;
    }
}