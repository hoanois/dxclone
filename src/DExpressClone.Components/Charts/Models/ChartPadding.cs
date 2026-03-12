namespace DExpressClone.Components.Charts.Models;

/// <summary>
/// Defines padding around the chart plot area.
/// </summary>
public record ChartPadding(double Top, double Right, double Bottom, double Left)
{
    /// <summary>
    /// Default padding suitable for most charts.
    /// </summary>
    public static ChartPadding Default => new(40, 20, 50, 60);
}
