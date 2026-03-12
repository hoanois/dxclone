using Microsoft.AspNetCore.Components;
using DExpressClone.Components.Core;

namespace DExpressClone.Components.Layout;

public partial class DxToolbar : DxComponentBase
{
    /// <summary>
    /// Gets or sets the child content containing DxToolbarItem components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the toolbar title.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    private string CssClassString => CssClassBuilder.New("dx-toolbar")
        .Add(CssClass)
        .Build();
}
