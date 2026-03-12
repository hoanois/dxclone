namespace DExpressClone.Components.Charts.Models;

/// <summary>
/// Represents a single data point in a chart series.
/// </summary>
public record ChartDataPoint(double X, double Y, string? Label = null);
