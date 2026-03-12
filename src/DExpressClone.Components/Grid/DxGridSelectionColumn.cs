using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Grid;

/// <summary>
/// A grid column that displays selection checkboxes for each row.
/// </summary>
public class DxGridSelectionColumn : DxGridColumnBase
{
    /// <summary>
    /// Gets or sets the width of the selection column. Defaults to 40px.
    /// </summary>
    public DxGridSelectionColumn()
    {
        Width = "40px";
        Caption = string.Empty;
    }

    /// <summary>
    /// Gets or sets whether to show a select-all checkbox in the header.
    /// </summary>
    [Parameter]
    public bool ShowSelectAll { get; set; } = true;
}
