using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DExpressClone.Components.Core;

namespace DExpressClone.Components.Layout;

public partial class DxToolbarItem : DxComponentBase
{
    /// <summary>
    /// Gets or sets the item text.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the item icon.
    /// </summary>
    [Parameter]
    public string? IconCssClass { get; set; }

    /// <summary>
    /// Gets or sets the click event callback.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> Click { get; set; }

    /// <summary>
    /// Gets or sets whether to add a separator before this item.
    /// </summary>
    [Parameter]
    public bool BeginGroup { get; set; }

    /// <summary>
    /// Gets or sets the item alignment.
    /// </summary>
    [Parameter]
    public ToolbarItemAlignment Alignment { get; set; } = ToolbarItemAlignment.Left;

    /// <summary>
    /// Gets or sets the custom template for the item.
    /// </summary>
    [Parameter]
    public RenderFragment? Template { get; set; }

    /// <summary>
    /// Gets or sets whether the item is visible.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the item is enabled.
    /// </summary>
    [Parameter]
    public bool Enabled { get; set; } = true;

    [CascadingParameter]
    internal DxToolbar? ParentToolbar { get; set; }

    private string CssClassString => CssClassBuilder.New("dx-toolbar-item")
        .AddIf("dx-toolbar-group--right", Alignment == ToolbarItemAlignment.Right)
        .AddIf("dx-state-disabled", !Enabled)
        .Add(CssClass)
        .Build();

    private async Task HandleClick(MouseEventArgs args)
    {
        if (Enabled && Click.HasDelegate)
        {
            await Click.InvokeAsync(args);
        }
    }
}
