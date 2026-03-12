namespace DExpressClone.Components.Grid.Models;

/// <summary>
/// Describes a grouping operation on a grid column.
/// </summary>
public class GridGroupDescriptor
{
    /// <summary>
    /// Gets or sets the name of the field to group by.
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group interval (e.g., for date grouping).
    /// </summary>
    public GridGroupInterval GroupInterval { get; set; } = GridGroupInterval.Value;
}

/// <summary>
/// Specifies how values are grouped.
/// </summary>
public enum GridGroupInterval
{
    Value,
    Date,
    Month,
    Year
}
