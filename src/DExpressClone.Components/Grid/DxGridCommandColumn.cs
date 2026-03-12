using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Grid;

/// <summary>
/// A grid column that displays command buttons (edit, delete, etc.).
/// </summary>
public class DxGridCommandColumn : DxGridColumnBase
{
    /// <summary>
    /// Gets or sets the header text for this command column.
    /// </summary>
    [Parameter]
    public string? HeaderText { get; set; }

    /// <summary>
    /// Gets or sets a custom cell template for the command buttons.
    /// The context is the data item.
    /// </summary>
    [Parameter]
    public RenderFragment<object>? CellTemplate { get; set; }
}
