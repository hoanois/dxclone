using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Grid;

/// <summary>
/// Abstract base class for all grid column types.
/// Registers itself with the parent DxGrid via CascadingParameter.
/// </summary>
public abstract class DxGridColumnBase : ComponentBase, IDisposable
{
    private bool _disposed;

    /// <summary>
    /// The parent grid that owns this column.
    /// </summary>
    [CascadingParameter]
    public IDxGridColumnOwner? Grid { get; set; }

    /// <summary>
    /// Gets or sets the column caption displayed in the header.
    /// </summary>
    [Parameter]
    public string? Caption { get; set; }

    /// <summary>
    /// Gets or sets the column width (CSS value, e.g., "150px", "20%").
    /// </summary>
    [Parameter]
    public string? Width { get; set; }

    /// <summary>
    /// Gets or sets whether this column is visible.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets the internal identifier for this column.
    /// </summary>
    internal int ColumnIndex { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Grid?.AddColumn(this);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            Grid?.RemoveColumn(this);
        }
    }

    protected override bool ShouldRender() => false;
}

/// <summary>
/// Interface for the parent grid to support column registration.
/// </summary>
public interface IDxGridColumnOwner
{
    void AddColumn(DxGridColumnBase column);
    void RemoveColumn(DxGridColumnBase column);
}
