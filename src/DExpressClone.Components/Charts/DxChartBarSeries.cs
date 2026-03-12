using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts;

/// <summary>
/// Defines a bar series within a DxChart.
/// </summary>
public class DxChartBarSeries<TItem> : DxChartSeriesBase<TItem>
{
    [Parameter] public double? BarWidth { get; set; }

    public override ChartSeriesType SeriesType => ChartSeriesType.Bar;
}
