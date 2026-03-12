using Microsoft.AspNetCore.Components;
using DExpressClone.Components.Core;

namespace DExpressClone.Components.Layout;

public partial class DxTabs : DxComponentBase
{
    private readonly List<DxTabPage> _tabs = new();

    /// <summary>
    /// Gets or sets the child content containing DxTabPage components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the index of the active tab.
    /// </summary>
    [Parameter]
    public int ActiveTabIndex { get; set; }

    /// <summary>
    /// Gets or sets the callback for when the active tab index changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> ActiveTabIndexChanged { get; set; }

    /// <summary>
    /// Gets or sets the render mode for tab content.
    /// </summary>
    [Parameter]
    public TabRenderMode RenderMode { get; set; } = TabRenderMode.AllTabs;

    private string CssClassString => CssClassBuilder.New("dx-tabs")
        .Add(CssClass)
        .Build();

    internal void RegisterTab(DxTabPage tab)
    {
        if (!_tabs.Contains(tab))
        {
            _tabs.Add(tab);
            RequestRender();
        }
    }

    internal void UnregisterTab(DxTabPage tab)
    {
        if (_tabs.Remove(tab))
        {
            RequestRender();
        }
    }

    internal int GetTabIndex(DxTabPage tab) => _tabs.IndexOf(tab);

    internal bool IsActiveTab(DxTabPage tab) => _tabs.IndexOf(tab) == ActiveTabIndex;

    internal bool ShouldRenderTab(DxTabPage tab)
    {
        if (RenderMode == TabRenderMode.AllTabs) return true;
        return IsActiveTab(tab);
    }

    private async Task SetActiveTab(int index)
    {
        if (index >= 0 && index < _tabs.Count && _tabs[index].Enabled)
        {
            ActiveTabIndex = index;
            await ActiveTabIndexChanged.InvokeAsync(index);
            RequestRender();
        }
    }

    private string GetTabCssClass(int index, DxTabPage tab)
    {
        return CssClassBuilder.New("dx-tab")
            .AddIf("dx-tab--active", index == ActiveTabIndex)
            .AddIf("dx-state-disabled", !tab.Enabled)
            .Build();
    }
}
