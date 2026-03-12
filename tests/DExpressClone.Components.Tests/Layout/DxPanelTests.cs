using Bunit;
using DExpressClone.Components.Layout;
using FluentAssertions;
using Xunit;

namespace DExpressClone.Components.Tests.Layout;

public class DxPanelTests : TestContext
{
    [Fact]
    public void RendersHeaderText()
    {
        var cut = RenderComponent<DxPanel>(parameters => parameters
            .Add(p => p.HeaderText, "My Panel"));

        cut.Find(".dx-panel-header-text").TextContent.Should().Be("My Panel");
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = RenderComponent<DxPanel>(parameters => parameters
            .Add(p => p.HeaderText, "Panel")
            .AddChildContent("<p>Hello World</p>"));

        cut.Find(".dx-panel-body").InnerHtml.Should().Contain("Hello World");
    }

    [Fact]
    public void CollapseToggle_HidesBody()
    {
        var cut = RenderComponent<DxPanel>(parameters => parameters
            .Add(p => p.HeaderText, "Panel")
            .Add(p => p.ShowCollapseButton, true)
            .AddChildContent("<p>Content</p>"));

        // Initially visible
        var body = cut.Find(".dx-panel-body");
        body.GetAttribute("style").Should().NotContain("display:none");

        // Click to collapse
        cut.Find(".dx-panel-header").Click();

        body = cut.Find(".dx-panel-body");
        body.GetAttribute("style").Should().Contain("display:none");
    }

    [Fact]
    public void VisibleFalse_HidesPanel()
    {
        var cut = RenderComponent<DxPanel>(parameters => parameters
            .Add(p => p.HeaderText, "Hidden Panel")
            .Add(p => p.Visible, false));

        cut.Markup.Should().BeEmpty();
    }

    [Fact]
    public void Collapsed_ShowsCollapseIcon()
    {
        var cut = RenderComponent<DxPanel>(parameters => parameters
            .Add(p => p.HeaderText, "Panel")
            .Add(p => p.ShowCollapseButton, true)
            .Add(p => p.Collapsed, true));

        cut.Find(".dx-panel").ClassList.Should().Contain("dx-panel--collapsed");
    }
}
