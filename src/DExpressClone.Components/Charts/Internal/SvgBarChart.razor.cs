using System.Globalization;
using DExpressClone.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts.Internal;

public partial class SvgBarChart : ComponentBase
{
    [Parameter] public IReadOnlyList<ChartSeriesDescriptor> Series { get; set; } = Array.Empty<ChartSeriesDescriptor>();
    [Parameter] public ChartScaleCalculator? Scale { get; set; }
    [Parameter] public double BarSpacing { get; set; } = 0.2;

    private string GetColor(ChartSeriesDescriptor series) =>
        series.Color ?? $"var(--dx-chart-series-{series.SeriesIndex}-color)";

    private static string F(double v) =>
        v.ToString("F2", CultureInfo.InvariantCulture);

    private double GetBarGroupWidth()
    {
        if (Scale is null || Series.Count == 0) return 0;
        // Find the minimum gap between consecutive X values across all series
        var allX = Series.SelectMany(s => s.DataPoints.Select(p => p.X)).Distinct().OrderBy(x => x).ToList();
        if (allX.Count < 2)
            return Scale.PlotWidth * 0.8;

        double minGap = double.MaxValue;
        for (int i = 1; i < allX.Count; i++)
        {
            var gap = Scale.MapX(allX[i]) - Scale.MapX(allX[i - 1]);
            if (gap < minGap) minGap = gap;
        }
        return minGap * (1.0 - BarSpacing);
    }

    private record BarRect(double X, double Y, double Width, double Height, string Color, ChartSeriesDescriptor Series, ChartDataPoint Point);

    private IEnumerable<BarRect> ComputeBars()
    {
        if (Scale is null || Series.Count == 0) yield break;

        double groupWidth = GetBarGroupWidth();
        double barWidth = groupWidth / Series.Count;
        int seriesCount = Series.Count;

        foreach (var series in Series)
        {
            foreach (var pt in series.DataPoints)
            {
                double cx = Scale.MapX(pt.X);
                double groupLeft = cx - groupWidth / 2.0;
                double barLeft = groupLeft + series.SeriesIndex * barWidth;

                double yValue = Scale.MapY(pt.Y);
                double yZero = Scale.MapY(0);

                double barTop = Math.Min(yValue, yZero);
                double barHeight = Math.Abs(yValue - yZero);

                yield return new BarRect(barLeft, barTop, barWidth, barHeight, GetColor(series), series, pt);
            }
        }
    }
}
