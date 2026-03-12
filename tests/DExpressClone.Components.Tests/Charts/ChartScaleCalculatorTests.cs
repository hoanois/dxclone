using DExpressClone.Components.Charts.Models;
using FluentAssertions;
using Xunit;

namespace DExpressClone.Components.Tests.Charts;

public class ChartScaleCalculatorTests
{
    [Fact]
    public void Compute_SetsCorrectPlotDimensions()
    {
        var padding = new ChartPadding(40, 20, 50, 60);
        var calc = ChartScaleCalculator.Compute(800, 600, padding, 0, 100, 0, 200);

        calc.PlotLeft.Should().Be(60);
        calc.PlotTop.Should().Be(40);
        calc.PlotWidth.Should().Be(800 - 60 - 20); // 720
        calc.PlotHeight.Should().Be(600 - 40 - 50); // 510
        calc.MinX.Should().Be(0);
        calc.MaxX.Should().Be(100);
        calc.MinY.Should().Be(0);
        calc.MaxY.Should().Be(200);
    }

    [Fact]
    public void MapX_MinMapsToPlotLeft()
    {
        var padding = new ChartPadding(40, 20, 50, 60);
        var calc = ChartScaleCalculator.Compute(800, 600, padding, 0, 100, 0, 200);

        calc.MapX(0).Should().Be(calc.PlotLeft);
    }

    [Fact]
    public void MapX_MaxMapsToPlotLeftPlusPlotWidth()
    {
        var padding = new ChartPadding(40, 20, 50, 60);
        var calc = ChartScaleCalculator.Compute(800, 600, padding, 0, 100, 0, 200);

        calc.MapX(100).Should().Be(calc.PlotLeft + calc.PlotWidth);
    }

    [Fact]
    public void MapY_MinMapsToPlotTopPlusPlotHeight_BecauseSvgYIsInverted()
    {
        var padding = new ChartPadding(40, 20, 50, 60);
        var calc = ChartScaleCalculator.Compute(800, 600, padding, 0, 100, 0, 200);

        calc.MapY(0).Should().Be(calc.PlotTop + calc.PlotHeight);
    }

    [Fact]
    public void MapY_MaxMapsToPlotTop()
    {
        var padding = new ChartPadding(40, 20, 50, 60);
        var calc = ChartScaleCalculator.Compute(800, 600, padding, 0, 100, 0, 200);

        calc.MapY(200).Should().Be(calc.PlotTop);
    }

    [Fact]
    public void ComputeTicks_ReturnsReasonableValues()
    {
        var padding = ChartPadding.Default;
        var calc = ChartScaleCalculator.Compute(800, 600, padding, 0, 100, 0, 100);

        var ticks = calc.ComputeTicks(0, 100, 5);

        ticks.Should().NotBeEmpty();
        ticks.First().Should().BeLessOrEqualTo(0);
        ticks.Last().Should().BeGreaterOrEqualTo(100);
        // Should have roughly the target number of ticks
        ticks.Count.Should().BeGreaterOrEqualTo(3);
        ticks.Count.Should().BeLessOrEqualTo(15);
    }

    [Fact]
    public void Compute_WithNegativeValues()
    {
        var padding = new ChartPadding(10, 10, 10, 10);
        var calc = ChartScaleCalculator.Compute(400, 300, padding, -50, 50, -100, 100);

        calc.MinX.Should().Be(-50);
        calc.MaxX.Should().Be(50);
        calc.MinY.Should().Be(-100);
        calc.MaxY.Should().Be(100);

        // Midpoint X should map to center
        var midX = calc.MapX(0);
        midX.Should().BeApproximately(calc.PlotLeft + calc.PlotWidth / 2, 0.001);

        // Midpoint Y should map to center
        var midY = calc.MapY(0);
        midY.Should().BeApproximately(calc.PlotTop + calc.PlotHeight / 2, 0.001);
    }

    [Fact]
    public void Compute_WithZeroRange_AdjustsMinMax()
    {
        var padding = new ChartPadding(10, 10, 10, 10);
        var calc = ChartScaleCalculator.Compute(400, 300, padding, 5, 5, 10, 10);

        // Zero range should be adjusted by epsilon
        (calc.MaxX - calc.MinX).Should().BeGreaterThan(0);
        (calc.MaxY - calc.MinY).Should().BeGreaterThan(0);

        // MapX and MapY should not throw or return NaN
        double.IsNaN(calc.MapX(5)).Should().BeFalse();
        double.IsNaN(calc.MapY(10)).Should().BeFalse();
    }

    [Fact]
    public void ComputeTicks_WithNegativeRange()
    {
        var padding = ChartPadding.Default;
        var calc = ChartScaleCalculator.Compute(800, 600, padding, 0, 100, -50, 50);

        var ticks = calc.ComputeTicks(-50, 50, 5);

        ticks.Should().NotBeEmpty();
        ticks.First().Should().BeLessOrEqualTo(-50);
        ticks.Last().Should().BeGreaterOrEqualTo(50);
    }
}
