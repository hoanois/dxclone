using System.Globalization;
using DExpressClone.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts.Internal;

public partial class SvgPieChart : ComponentBase
{
    [Parameter] public ChartSeriesDescriptor? Series { get; set; }
    [Parameter] public bool ShowLabels { get; set; } = true;
    [Parameter] public double InnerRadius { get; set; } = 0;
    [Parameter] public double CenterX { get; set; }
    [Parameter] public double CenterY { get; set; }
    [Parameter] public double Radius { get; set; }

    private record SliceData(string Path, string Color, string? Label, double Percentage, double MidAngle);

    private static readonly string[] DefaultColors = new[]
    {
        "#5470c6", "#91cc75", "#fac858", "#ee6666", "#73c0de",
        "#3ba272", "#fc8452", "#9a60b4", "#ea7ccc", "#48b8d0"
    };

    private IReadOnlyList<SliceData> ComputeSlices()
    {
        if (Series is null || Series.DataPoints.Count == 0)
            return Array.Empty<SliceData>();

        double total = Series.DataPoints.Sum(p => Math.Abs(p.Y));
        if (total < 1e-10) return Array.Empty<SliceData>();

        var slices = new List<SliceData>();
        double currentAngle = -Math.PI / 2; // start at top

        for (int i = 0; i < Series.DataPoints.Count; i++)
        {
            var pt = Series.DataPoints[i];
            double sliceAngle = (Math.Abs(pt.Y) / total) * 2 * Math.PI;
            double endAngle = currentAngle + sliceAngle;
            double midAngle = currentAngle + sliceAngle / 2;
            double pct = (Math.Abs(pt.Y) / total) * 100;

            string color = (i < DefaultColors.Length) ? DefaultColors[i] : $"var(--dx-chart-series-{i}-color)";
            string path = DescribeSlice(CenterX, CenterY, Radius, InnerRadius, currentAngle, endAngle);
            string? label = pt.Label ?? pt.Y.ToString("G", CultureInfo.InvariantCulture);

            slices.Add(new SliceData(path, color, label, pct, midAngle));
            currentAngle = endAngle;
        }

        return slices;
    }

    private static string DescribeSlice(double cx, double cy, double outerR, double innerR,
        double startAngle, double endAngle)
    {
        var outerStart = PolarToCartesian(cx, cy, outerR, startAngle);
        var outerEnd = PolarToCartesian(cx, cy, outerR, endAngle);
        int largeArc = (endAngle - startAngle > Math.PI) ? 1 : 0;

        if (innerR > 0)
        {
            var innerStart = PolarToCartesian(cx, cy, innerR, startAngle);
            var innerEnd = PolarToCartesian(cx, cy, innerR, endAngle);
            return $"M {F(outerStart.x)} {F(outerStart.y)} " +
                   $"A {F(outerR)} {F(outerR)} 0 {largeArc} 1 {F(outerEnd.x)} {F(outerEnd.y)} " +
                   $"L {F(innerEnd.x)} {F(innerEnd.y)} " +
                   $"A {F(innerR)} {F(innerR)} 0 {largeArc} 0 {F(innerStart.x)} {F(innerStart.y)} Z";
        }
        else
        {
            return $"M {F(cx)} {F(cy)} " +
                   $"L {F(outerStart.x)} {F(outerStart.y)} " +
                   $"A {F(outerR)} {F(outerR)} 0 {largeArc} 1 {F(outerEnd.x)} {F(outerEnd.y)} Z";
        }
    }

    private static (double x, double y) PolarToCartesian(double cx, double cy, double r, double angle)
    {
        return (cx + r * Math.Cos(angle), cy + r * Math.Sin(angle));
    }

    private static string F(double v) =>
        v.ToString("F2", CultureInfo.InvariantCulture);
}
