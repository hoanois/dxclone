namespace DExpressClone.Components.Grid.Models;

/// <summary>
/// Describes a sort operation on a grid column.
/// </summary>
public class GridSortDescriptor
{
    /// <summary>
    /// Gets or sets the name of the field to sort by.
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    public GridSortOrder SortOrder { get; set; } = GridSortOrder.None;
}

/// <summary>
/// Specifies the sort direction.
/// </summary>
public enum GridSortOrder
{
    None,
    Ascending,
    Descending
}
