using DExpressClone.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts.Internal;

public partial class ChartLegend : ComponentBase
{
    [Parameter] public IReadOnlyList<ChartSeriesDescriptor> Series { get; set; } = Array.Empty<ChartSeriesDescriptor>();

    private static readonly string[] DefaultColors = new[]
    {
        "#5470c6", "#91cc75", "#fac858", "#ee6666", "#73c0de",
        "#3ba272", "#fc8452", "#9a60b4", "#ea7ccc", "#48b8d0"
    };

    private string GetColor(ChartSeriesDescriptor s) =>
        s.Color ?? (s.SeriesIndex < DefaultColors.Length ? DefaultColors[s.SeriesIndex] : $"var(--dx-chart-series-{s.SeriesIndex}-color)");
}
