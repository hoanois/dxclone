using Microsoft.AspNetCore.Components;
using DExpressClone.Components.Core;

namespace DExpressClone.Components.Layout;

public partial class DxAccordion : DxComponentBase
{
    private readonly List<DxAccordionItem> _items = new();

    /// <summary>
    /// Gets or sets the child content containing DxAccordionItem components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the expand mode.
    /// </summary>
    [Parameter]
    public AccordionExpandMode ExpandMode { get; set; } = AccordionExpandMode.Multiple;

    private string CssClassString => CssClassBuilder.New("dx-accordion")
        .Add(CssClass)
        .Build();

    internal void RegisterItem(DxAccordionItem item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
        }
    }

    internal void UnregisterItem(DxAccordionItem item)
    {
        _items.Remove(item);
    }

    internal async Task NotifyItemExpanded(DxAccordionItem expandedItem)
    {
        if (ExpandMode == AccordionExpandMode.Single)
        {
            foreach (var item in _items)
            {
                if (item != expandedItem && item.Expanded)
                {
                    await item.CollapseAsync();
                }
            }
        }
    }
}
