using System.Globalization;
using System.Text;
using DExpressClone.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts.Internal;

public partial class SvgLineChart : ComponentBase
{
    [Parameter] public IReadOnlyList<ChartSeriesDescriptor> Series { get; set; } = Array.Empty<ChartSeriesDescriptor>();
    [Parameter] public ChartScaleCalculator? Scale { get; set; }
    [Parameter] public bool ShowPoints { get; set; } = true;

    private string BuildPath(ChartSeriesDescriptor series)
    {
        if (Scale is null || series.DataPoints.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();
        for (int i = 0; i < series.DataPoints.Count; i++)
        {
            var pt = series.DataPoints[i];
            var x = Scale.MapX(pt.X);
            var y = Scale.MapY(pt.Y);
            sb.Append(i == 0 ? "M " : " L ");
            sb.Append(F(x));
            sb.Append(' ');
            sb.Append(F(y));
        }
        return sb.ToString();
    }

    private string GetColor(ChartSeriesDescriptor series) =>
        series.Color ?? $"var(--dx-chart-series-{series.SeriesIndex}-color)";

    private static string F(double v) =>
        v.ToString("F2", CultureInfo.InvariantCulture);
}
