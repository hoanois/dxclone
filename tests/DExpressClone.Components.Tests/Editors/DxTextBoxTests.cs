using Bunit;
using FluentAssertions;
using Xunit;

namespace DExpressClone.Components.Tests.Editors;

public class DxTextBoxTests : TestContext
{
    [Fact]
    public void RendersInputElement()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxTextBox.DxTextBox>();

        cut.Find("input.dx-textbox-input").Should().NotBeNull();
    }

    [Fact]
    public void ValueBinding()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxTextBox.DxTextBox>(parameters => parameters
            .Add(p => p.Text, "Hello"));

        var input = cut.Find("input.dx-textbox-input");
        input.GetAttribute("value").Should().Be("Hello");
    }

    [Fact]
    public void NullText_RendersAsPlaceholder()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxTextBox.DxTextBox>(parameters => parameters
            .Add(p => p.NullText, "Enter text..."));

        var input = cut.Find("input.dx-textbox-input");
        input.GetAttribute("placeholder").Should().Be("Enter text...");
    }

    [Fact]
    public void PasswordMode_SetsTypePassword()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxTextBox.DxTextBox>(parameters => parameters
            .Add(p => p.Password, true));

        var input = cut.Find("input.dx-textbox-input");
        input.GetAttribute("type").Should().Be("password");
    }

    [Fact]
    public void EnabledFalse_DisablesInput()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxTextBox.DxTextBox>(parameters => parameters
            .Add(p => p.Enabled, false));

        var input = cut.Find("input.dx-textbox-input");
        input.HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void DefaultType_IsText()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxTextBox.DxTextBox>();

        var input = cut.Find("input.dx-textbox-input");
        input.GetAttribute("type").Should().Be("text");
    }
}
