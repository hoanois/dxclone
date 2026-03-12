using System.Globalization;

namespace DExpressClone.Components.Charts.Models;

/// <summary>
/// Computes scales for mapping data values to SVG coordinates.
/// </summary>
public sealed class ChartScaleCalculator
{
    public double PlotLeft { get; set; }
    public double PlotTop { get; set; }
    public double PlotWidth { get; set; }
    public double PlotHeight { get; set; }
    public double MinX { get; set; }
    public double MaxX { get; set; }
    public double MinY { get; set; }
    public double MaxY { get; set; }

    /// <summary>
    /// Computes a scale calculator from chart dimensions, padding, and data bounds.
    /// </summary>
    public static ChartScaleCalculator Compute(
        double chartWidth, double chartHeight,
        ChartPadding padding,
        double minX, double maxX, double minY, double maxY)
    {
        // Avoid zero-range by adding a small epsilon
        if (Math.Abs(maxX - minX) < 1e-10)
        {
            minX -= 0.5;
            maxX += 0.5;
        }
        if (Math.Abs(maxY - minY) < 1e-10)
        {
            minY -= 0.5;
            maxY += 0.5;
        }

        return new ChartScaleCalculator
        {
            PlotLeft = padding.Left,
            PlotTop = padding.Top,
            PlotWidth = chartWidth - padding.Left - padding.Right,
            PlotHeight = chartHeight - padding.Top - padding.Bottom,
            MinX = minX,
            MaxX = maxX,
            MinY = minY,
            MaxY = maxY
        };
    }

    /// <summary>
    /// Maps a data X value to an SVG X coordinate.
    /// </summary>
    public double MapX(double value)
    {
        double ratio = (value - MinX) / (MaxX - MinX);
        return PlotLeft + ratio * PlotWidth;
    }

    /// <summary>
    /// Maps a data Y value to an SVG Y coordinate (inverted: Y grows downward in SVG).
    /// </summary>
    public double MapY(double value)
    {
        double ratio = (value - MinY) / (MaxY - MinY);
        return PlotTop + PlotHeight - ratio * PlotHeight;
    }

    /// <summary>
    /// Computes a list of nice tick values between min and max.
    /// </summary>
    public IReadOnlyList<double> ComputeTicks(double min, double max, int targetTickCount = 5)
    {
        if (targetTickCount < 2) targetTickCount = 2;

        double range = max - min;
        double roughStep = range / (targetTickCount - 1);

        // Find a "nice" step size
        double magnitude = Math.Pow(10, Math.Floor(Math.Log10(roughStep)));
        double residual = roughStep / magnitude;

        double niceStep;
        if (residual <= 1.5) niceStep = magnitude;
        else if (residual <= 3.0) niceStep = 2 * magnitude;
        else if (residual <= 7.0) niceStep = 5 * magnitude;
        else niceStep = 10 * magnitude;

        double tickMin = Math.Floor(min / niceStep) * niceStep;
        double tickMax = Math.Ceiling(max / niceStep) * niceStep;

        var ticks = new List<double>();
        for (double t = tickMin; t <= tickMax + niceStep * 0.01; t += niceStep)
        {
            ticks.Add(Math.Round(t, 10));
        }

        return ticks;
    }
}
