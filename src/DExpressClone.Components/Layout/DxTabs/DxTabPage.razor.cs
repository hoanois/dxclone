using Microsoft.AspNetCore.Components;
using DExpressClone.Components.Core;

namespace DExpressClone.Components.Layout;

public partial class DxTabPage : DxComponentBase, IDisposable
{
    /// <summary>
    /// Gets or sets the tab header text.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the tab icon.
    /// </summary>
    [Parameter]
    public string? IconCssClass { get; set; }

    /// <summary>
    /// Gets or sets the tab content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the tab page is visible.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the tab page is enabled.
    /// </summary>
    [Parameter]
    public bool Enabled { get; set; } = true;

    [CascadingParameter]
    internal DxTabs? ParentTabs { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ParentTabs?.RegisterTab(this);
    }

    public void Dispose()
    {
        ParentTabs?.UnregisterTab(this);
    }
}
