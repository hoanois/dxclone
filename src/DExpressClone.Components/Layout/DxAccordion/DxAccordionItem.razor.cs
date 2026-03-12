using Microsoft.AspNetCore.Components;
using DExpressClone.Components.Core;

namespace DExpressClone.Components.Layout;

public partial class DxAccordionItem : DxComponentBase, IDisposable
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
    /// Gets or sets the item content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the item is expanded.
    /// </summary>
    [Parameter]
    public bool Expanded { get; set; }

    /// <summary>
    /// Gets or sets the callback for when the expanded state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    [CascadingParameter]
    internal DxAccordion? ParentAccordion { get; set; }

    private string CssClassString => CssClassBuilder.New("dx-accordion-item")
        .AddIf("dx-accordion-item--expanded", Expanded)
        .Add(CssClass)
        .Build();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ParentAccordion?.RegisterItem(this);
    }

    private async Task ToggleExpanded()
    {
        Expanded = !Expanded;
        await ExpandedChanged.InvokeAsync(Expanded);

        if (Expanded && ParentAccordion != null)
        {
            await ParentAccordion.NotifyItemExpanded(this);
        }
    }

    internal async Task CollapseAsync()
    {
        if (Expanded)
        {
            Expanded = false;
            await ExpandedChanged.InvokeAsync(false);
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        ParentAccordion?.UnregisterItem(this);
    }
}
