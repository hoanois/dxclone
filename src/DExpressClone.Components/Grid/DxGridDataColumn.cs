using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Grid;

/// <summary>
/// A data-bound grid column that displays values from a specified field.
/// </summary>
public class DxGridDataColumn : DxGridColumnBase
{
    /// <summary>
    /// Gets or sets the property name of the data item to display.
    /// </summary>
    [Parameter]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this column can be sorted.
    /// </summary>
    [Parameter]
    public bool AllowSort { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this column can be filtered.
    /// </summary>
    [Parameter]
    public bool AllowFilter { get; set; } = true;

    /// <summary>
    /// Gets or sets the text alignment for cells in this column.
    /// </summary>
    [Parameter]
    public GridTextAlignment TextAlignment { get; set; } = GridTextAlignment.Left;

    /// <summary>
    /// Gets or sets a custom cell template for display mode.
    /// The context is the data item.
    /// </summary>
    [Parameter]
    public RenderFragment<object>? CellTemplate { get; set; }

    /// <summary>
    /// Gets or sets a custom cell template for edit mode.
    /// </summary>
    [Parameter]
    public RenderFragment<object>? EditCellTemplate { get; set; }

    /// <summary>
    /// Gets or sets the display format string (e.g., "{0:C2}", "{0:yyyy-MM-dd}").
    /// </summary>
    [Parameter]
    public string? DisplayFormat { get; set; }

    /// <summary>
    /// Gets the display caption, falling back to FieldName if Caption is not set.
    /// </summary>
    public string DisplayCaption => Caption ?? FieldName;
}

/// <summary>
/// Specifies text alignment within a grid cell.
/// </summary>
public enum GridTextAlignment
{
    Left,
    Center,
    Right
}
