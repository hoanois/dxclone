using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts;

/// <summary>
/// Defines a line series within a DxChart.
/// </summary>
public class DxChartLineSeries<TItem> : DxChartSeriesBase<TItem>
{
    [Parameter] public int LineWidth { get; set; } = 2;
    [Parameter] public bool ShowPoints { get; set; } = true;

    public override ChartSeriesType SeriesType => ChartSeriesType.Line;
}
