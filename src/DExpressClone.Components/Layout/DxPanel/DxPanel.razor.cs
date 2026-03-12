using Microsoft.AspNetCore.Components;
using DExpressClone.Components.Core;

namespace DExpressClone.Components.Layout;

public partial class DxPanel : DxComponentBase
{
    /// <summary>
    /// Gets or sets the header text.
    /// </summary>
    [Parameter]
    public string? HeaderText { get; set; }

    /// <summary>
    /// Gets or sets the header template.
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the body content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the footer template.
    /// </summary>
    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    /// <summary>
    /// Gets or sets whether the collapse button is shown.
    /// </summary>
    [Parameter]
    public bool ShowCollapseButton { get; set; }

    /// <summary>
    /// Gets or sets whether the panel is collapsed.
    /// </summary>
    [Parameter]
    public bool Collapsed { get; set; }

    /// <summary>
    /// Gets or sets the callback for when the collapsed state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> CollapsedChanged { get; set; }

    /// <summary>
    /// Gets or sets whether the panel is visible.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    private string CssClassString => CssClassBuilder.New("dx-panel")
        .AddIf("dx-panel--collapsed", Collapsed)
        .Add(CssClass)
        .Build();

    private async Task ToggleCollapse()
    {
        if (ShowCollapseButton)
        {
            Collapsed = !Collapsed;
            await CollapsedChanged.InvokeAsync(Collapsed);
        }
    }
}
