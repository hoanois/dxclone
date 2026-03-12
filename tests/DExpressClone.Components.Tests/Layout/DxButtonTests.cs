using Bunit;
using DExpressClone.Components.Layout;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace DExpressClone.Components.Tests.Layout;

public class DxButtonTests : TestContext
{
    [Fact]
    public void RendersButtonElementWithText()
    {
        var cut = RenderComponent<DxButton>(parameters => parameters
            .Add(p => p.Text, "Click Me"));

        var button = cut.Find("button");
        button.Should().NotBeNull();
        cut.Find(".dx-btn-text").TextContent.Should().Be("Click Me");
    }

    [Fact]
    public void ClickEventFires()
    {
        bool clicked = false;
        var cut = RenderComponent<DxButton>(parameters => parameters
            .Add(p => p.Text, "Click Me")
            .Add(p => p.Click, (MouseEventArgs _) => { clicked = true; }));

        cut.Find("button").Click();

        clicked.Should().BeTrue();
    }

    [Fact]
    public void DisabledButtonHasDisabledClass()
    {
        var cut = RenderComponent<DxButton>(parameters => parameters
            .Add(p => p.Text, "Disabled")
            .Add(p => p.Enabled, false));

        var button = cut.Find("button");
        button.ClassList.Should().Contain("dx-state-disabled");
        button.HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void RenderStyle_Secondary_HasCorrectClass()
    {
        var cut = RenderComponent<DxButton>(parameters => parameters
            .Add(p => p.Text, "Secondary")
            .Add(p => p.RenderStyle, ButtonRenderStyle.Secondary));

        cut.Find("button").ClassList.Should().Contain("dx-btn--secondary");
    }

    [Fact]
    public void RenderStyleMode_Outlined_HasCorrectClass()
    {
        var cut = RenderComponent<DxButton>(parameters => parameters
            .Add(p => p.Text, "Outlined")
            .Add(p => p.RenderStyleMode, ButtonRenderStyleMode.Outlined));

        cut.Find("button").ClassList.Should().Contain("dx-btn--outlined");
    }

    [Fact]
    public void RenderStyleMode_Text_HasCorrectClass()
    {
        var cut = RenderComponent<DxButton>(parameters => parameters
            .Add(p => p.Text, "Text")
            .Add(p => p.RenderStyleMode, ButtonRenderStyleMode.Text));

        cut.Find("button").ClassList.Should().Contain("dx-btn--text");
    }

    [Fact]
    public void RendersAsAnchorWhenNavigateUrlIsSet()
    {
        var cut = RenderComponent<DxButton>(parameters => parameters
            .Add(p => p.Text, "Link")
            .Add(p => p.NavigateUrl, "https://example.com"));

        var anchor = cut.Find("a");
        anchor.Should().NotBeNull();
        anchor.GetAttribute("href").Should().Be("https://example.com");
        cut.Find(".dx-btn-text").TextContent.Should().Be("Link");
    }

    [Fact]
    public void DisabledButton_ClickDoesNotFire()
    {
        bool clicked = false;
        var cut = RenderComponent<DxButton>(parameters => parameters
            .Add(p => p.Text, "Disabled")
            .Add(p => p.Enabled, false)
            .Add(p => p.Click, (MouseEventArgs _) => { clicked = true; }));

        cut.Find("button").Click();

        clicked.Should().BeFalse();
    }
}
