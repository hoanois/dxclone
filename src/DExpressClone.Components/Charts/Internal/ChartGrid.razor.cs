using DExpressClone.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts.Internal;

public partial class ChartGrid : ComponentBase
{
    [Parameter] public ChartScaleCalculator? Scale { get; set; }
    [Parameter] public IReadOnlyList<double> XTicks { get; set; } = Array.Empty<double>();
    [Parameter] public IReadOnlyList<double> YTicks { get; set; } = Array.Empty<double>();

    private static string F(double v) =>
        v.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
}
