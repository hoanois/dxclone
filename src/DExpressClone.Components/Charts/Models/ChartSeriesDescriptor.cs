namespace DExpressClone.Components.Charts.Models;

/// <summary>
/// Describes a chart data series with its data points and visual properties.
/// </summary>
public class ChartSeriesDescriptor
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public int SeriesIndex { get; set; }
    public List<ChartDataPoint> DataPoints { get; set; } = new();
}
