namespace DExpressClone.Components.Grid.Models;

/// <summary>
/// Options passed to the custom data source callback describing what data to load.
/// </summary>
public class GridDataLoadOptions
{
    /// <summary>
    /// Gets the number of items to skip.
    /// </summary>
    public int Skip { get; init; }

    /// <summary>
    /// Gets the number of items to take.
    /// </summary>
    public int Take { get; init; }

    /// <summary>
    /// Gets the current sort descriptors.
    /// </summary>
    public IReadOnlyList<GridSortDescriptor> SortDescriptors { get; init; } = Array.Empty<GridSortDescriptor>();

    /// <summary>
    /// Gets the current filter descriptors.
    /// </summary>
    public IReadOnlyList<GridFilterDescriptor> FilterDescriptors { get; init; } = Array.Empty<GridFilterDescriptor>();
}

/// <summary>
/// Result returned from a custom data source callback.
/// </summary>
/// <typeparam name="TItem">The type of data item.</typeparam>
public class GridDataLoadResult<TItem>
{
    /// <summary>
    /// Gets the items for the current page/window.
    /// </summary>
    public IEnumerable<TItem> Items { get; init; } = Enumerable.Empty<TItem>();

    /// <summary>
    /// Gets the total number of items matching the current filters.
    /// </summary>
    public int TotalCount { get; init; }
}
