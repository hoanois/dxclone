using DExpressClone.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts.Internal;

public partial class ChartAxis : ComponentBase
{
    [Parameter] public ChartScaleCalculator? Scale { get; set; }
    [Parameter] public ChartAxisDescriptor? XAxisDescriptor { get; set; }
    [Parameter] public ChartAxisDescriptor? YAxisDescriptor { get; set; }

    private IReadOnlyList<double> XTicks => Scale?.ComputeTicks(
        Scale.MinX, Scale.MaxX, XAxisDescriptor?.TickCount ?? 5) ?? Array.Empty<double>();

    private IReadOnlyList<double> YTicks => Scale?.ComputeTicks(
        Scale.MinY, Scale.MaxY, YAxisDescriptor?.TickCount ?? 5) ?? Array.Empty<double>();

    private string FormatX(double value)
    {
        if (XAxisDescriptor?.DisplayFormat is { } fmt)
            return value.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);
        return value.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
    }

    private string FormatY(double value)
    {
        if (YAxisDescriptor?.DisplayFormat is { } fmt)
            return value.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);
        return value.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string F(double v) =>
        v.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
}
