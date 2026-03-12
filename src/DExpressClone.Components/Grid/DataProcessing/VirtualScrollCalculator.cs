namespace DExpressClone.Components.Grid.DataProcessing;

/// <summary>
/// Calculates which rows are visible in a virtualized grid viewport.
/// </summary>
public sealed class VirtualScrollCalculator
{
    /// <summary>
    /// Gets the fixed height of each row in pixels.
    /// </summary>
    public int RowHeight { get; init; } = 36;

    /// <summary>
    /// Gets the number of extra rows to render above and below the visible area.
    /// </summary>
    public int OverscanCount { get; init; } = 5;

    /// <summary>
    /// Calculates the virtualization window based on scroll position and viewport size.
    /// </summary>
    public VirtualizationWindow Calculate(int totalRows, double scrollTop, double viewportHeight)
    {
        if (totalRows <= 0 || viewportHeight <= 0)
        {
            return new VirtualizationWindow(0, 0, 0, 0);
        }

        var visibleCount = (int)Math.Ceiling(viewportHeight / RowHeight);
        var firstVisible = (int)Math.Floor(scrollTop / RowHeight);

        var startIndex = Math.Max(0, firstVisible - OverscanCount);
        var endIndex = Math.Min(totalRows - 1, firstVisible + visibleCount + OverscanCount);

        var topSpacerHeight = startIndex * (double)RowHeight;
        var bottomSpacerHeight = Math.Max(0, (totalRows - 1 - endIndex) * (double)RowHeight);

        return new VirtualizationWindow(startIndex, endIndex, topSpacerHeight, bottomSpacerHeight);
    }
}

/// <summary>
/// Represents the calculated visible window for virtual scrolling.
/// </summary>
public record VirtualizationWindow(
    int StartIndex,
    int EndIndex,
    double TopSpacerHeight,
    double BottomSpacerHeight)
{
    /// <summary>
    /// Gets the number of rows in the visible window.
    /// </summary>
    public int VisibleRowCount => EndIndex - StartIndex + 1;
}
