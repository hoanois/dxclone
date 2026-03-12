using DExpressClone.Components.Grid.DataProcessing;
using FluentAssertions;
using Xunit;

namespace DExpressClone.Components.Tests.Grid;

public class VirtualScrollCalculatorTests
{
    private readonly VirtualScrollCalculator _calculator = new() { RowHeight = 36, OverscanCount = 5 };

    [Fact]
    public void BasicCalculation_ScrollTopZero()
    {
        var result = _calculator.Calculate(totalRows: 1000, scrollTop: 0, viewportHeight: 500);

        result.StartIndex.Should().Be(0);
        // visibleCount = ceil(500/36) = 14, firstVisible = 0
        // endIndex = min(999, 0 + 14 + 5) = 19
        result.EndIndex.Should().Be(19);
        result.TopSpacerHeight.Should().Be(0);
        result.BottomSpacerHeight.Should().BeGreaterThan(0);
    }

    [Fact]
    public void ScrollToMiddle()
    {
        // scrollTop = 18000 means firstVisible = floor(18000/36) = 500
        var result = _calculator.Calculate(totalRows: 1000, scrollTop: 18000, viewportHeight: 500);

        result.StartIndex.Should().Be(495); // 500 - 5 overscan
        // visibleCount = ceil(500/36) = 14
        // endIndex = min(999, 500 + 14 + 5) = 519
        result.EndIndex.Should().Be(519);
        result.TopSpacerHeight.Should().Be(495 * 36.0);
    }

    [Fact]
    public void ScrollToEnd()
    {
        // scrollTop so that firstVisible is near the end
        double scrollTop = (1000 - 14) * 36; // near bottom
        var result = _calculator.Calculate(totalRows: 1000, scrollTop: scrollTop, viewportHeight: 500);

        result.EndIndex.Should().Be(999);
        result.BottomSpacerHeight.Should().Be(0);
    }

    [Fact]
    public void OverscanDoesNotGoBelowZero()
    {
        var result = _calculator.Calculate(totalRows: 1000, scrollTop: 72, viewportHeight: 500);
        // firstVisible = floor(72/36) = 2, startIndex = max(0, 2 - 5) = 0
        result.StartIndex.Should().Be(0);
    }

    [Fact]
    public void OverscanDoesNotGoAboveTotalRows()
    {
        double scrollTop = 999 * 36;
        var result = _calculator.Calculate(totalRows: 1000, scrollTop: scrollTop, viewportHeight: 500);
        result.EndIndex.Should().BeLessOrEqualTo(999);
    }

    [Fact]
    public void SmallDataset_FewerRowsThanViewport()
    {
        var result = _calculator.Calculate(totalRows: 5, scrollTop: 0, viewportHeight: 500);

        result.StartIndex.Should().Be(0);
        result.EndIndex.Should().Be(4);
        result.TopSpacerHeight.Should().Be(0);
        result.BottomSpacerHeight.Should().Be(0);
    }

    [Fact]
    public void SpacerHeightsAreCorrect()
    {
        var result = _calculator.Calculate(totalRows: 100, scrollTop: 1800, viewportHeight: 500);
        // firstVisible = floor(1800/36) = 50
        // startIndex = max(0, 50 - 5) = 45
        // visibleCount = ceil(500/36) = 14
        // endIndex = min(99, 50 + 14 + 5) = 69

        result.TopSpacerHeight.Should().Be(result.StartIndex * 36.0);
        result.BottomSpacerHeight.Should().Be((99 - result.EndIndex) * 36.0);
    }

    [Fact]
    public void ZeroTotalRows_ReturnsZeroWindow()
    {
        var result = _calculator.Calculate(totalRows: 0, scrollTop: 0, viewportHeight: 500);
        result.StartIndex.Should().Be(0);
        result.EndIndex.Should().Be(0);
    }

    [Fact]
    public void ZeroViewportHeight_ReturnsZeroWindow()
    {
        var result = _calculator.Calculate(totalRows: 1000, scrollTop: 0, viewportHeight: 0);
        result.StartIndex.Should().Be(0);
        result.EndIndex.Should().Be(0);
    }
}
