using DExpressClone.Components.Grid.DataProcessing;

namespace DExpressClone.Components.Grid.Models;

/// <summary>
/// Tracks the current virtualization state for the grid viewport.
/// </summary>
public class GridVirtualizationState
{
    /// <summary>
    /// Gets or sets the current vertical scroll position in pixels.
    /// </summary>
    public double ScrollTop { get; set; }

    /// <summary>
    /// Gets or sets the visible viewport height in pixels.
    /// </summary>
    public double ViewportHeight { get; set; }

    /// <summary>
    /// Gets or sets the currently calculated virtualization window.
    /// </summary>
    public VirtualizationWindow? CurrentWindow { get; set; }
}
