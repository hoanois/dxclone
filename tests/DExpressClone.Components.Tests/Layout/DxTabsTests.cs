using Bunit;
using DExpressClone.Components.Layout;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace DExpressClone.Components.Tests.Layout;

public class DxTabsTests : TestContext
{
    [Fact]
    public void RendersTabHeaders()
    {
        var cut = RenderComponent<DxTabs>(parameters => parameters
            .Add(p => p.ActiveTabIndex, 0)
            .AddChildContent<DxTabPage>(p => p.Add(t => t.Text, "Tab 1"))
            .AddChildContent<DxTabPage>(p => p.Add(t => t.Text, "Tab 2")));

        var tabs = cut.FindAll(".dx-tab");
        tabs.Should().HaveCount(2);
        tabs[0].TextContent.Should().Contain("Tab 1");
        tabs[1].TextContent.Should().Contain("Tab 2");
    }

    [Fact]
    public void ClickingTab_ChangesActiveIndex()
    {
        int activeIndex = 0;
        var cut = RenderComponent<DxTabs>(parameters => parameters
            .Add(p => p.ActiveTabIndex, 0)
            .Add(p => p.ActiveTabIndexChanged, (int idx) => activeIndex = idx)
            .AddChildContent<DxTabPage>(p => p.Add(t => t.Text, "Tab 1"))
            .AddChildContent<DxTabPage>(p => p.Add(t => t.Text, "Tab 2")));

        // Click second tab
        var tabs = cut.FindAll(".dx-tab");
        tabs[1].Click();

        activeIndex.Should().Be(1);
    }

    [Fact]
    public void ActiveTabIndex_RendersCorrectContent()
    {
        var cut = RenderComponent<DxTabs>(parameters => parameters
            .Add(p => p.ActiveTabIndex, 0)
            .Add(p => p.RenderMode, TabRenderMode.AllTabs)
            .AddChildContent<DxTabPage>(p => p
                .Add(t => t.Text, "Tab 1")
                .AddChildContent("Content 1"))
            .AddChildContent<DxTabPage>(p => p
                .Add(t => t.Text, "Tab 2")
                .AddChildContent("Content 2")));

        var tabPanels = cut.FindAll(".dx-tab-page");
        tabPanels.Should().HaveCount(2);

        // First tab should be visible (no display:none)
        tabPanels[0].GetAttribute("style").Should().NotContain("display:none");
        // Second tab should be hidden
        tabPanels[1].GetAttribute("style").Should().Contain("display:none");
    }

    [Fact]
    public void ActiveTab_HasActiveClass()
    {
        var cut = RenderComponent<DxTabs>(parameters => parameters
            .Add(p => p.ActiveTabIndex, 0)
            .AddChildContent<DxTabPage>(p => p.Add(t => t.Text, "Tab 1"))
            .AddChildContent<DxTabPage>(p => p.Add(t => t.Text, "Tab 2")));

        var tabs = cut.FindAll(".dx-tab");
        tabs[0].ClassList.Should().Contain("dx-tab--active");
        tabs[1].ClassList.Should().NotContain("dx-tab--active");
    }
}
