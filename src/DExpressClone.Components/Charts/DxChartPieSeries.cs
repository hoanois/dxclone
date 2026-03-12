using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts;

/// <summary>
/// Defines a pie/donut series within a DxChart.
/// </summary>
public class DxChartPieSeries<TItem> : DxChartSeriesBase<TItem>
{
    [Parameter] public double InnerRadius { get; set; } = 0;
    [Parameter] public bool ShowLabels { get; set; } = true;

    public override ChartSeriesType SeriesType => ChartSeriesType.Pie;
}
