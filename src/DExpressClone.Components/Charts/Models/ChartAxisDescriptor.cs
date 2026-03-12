namespace DExpressClone.Components.Charts.Models;

/// <summary>
/// Describes configuration for a chart axis.
/// </summary>
public class ChartAxisDescriptor
{
    public string? Title { get; set; }
    public string? DisplayFormat { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public int TickCount { get; set; } = 5;
}
