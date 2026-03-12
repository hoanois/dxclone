namespace DExpressClone.Components.Grid.Models;

/// <summary>
/// Describes a filter operation on a grid column.
/// </summary>
public class GridFilterDescriptor
{
    /// <summary>
    /// Gets or sets the name of the field to filter.
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value to filter by.
    /// </summary>
    public object? FilterValue { get; set; }

    /// <summary>
    /// Gets or sets the filter operator.
    /// </summary>
    public GridFilterOperator FilterOperator { get; set; } = GridFilterOperator.Contains;
}

/// <summary>
/// Specifies the filter comparison operator.
/// </summary>
public enum GridFilterOperator
{
    Contains,
    Equals,
    GreaterThan,
    LessThan,
    StartsWith,
    Between
}
