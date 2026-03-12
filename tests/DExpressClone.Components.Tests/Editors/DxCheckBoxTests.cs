using Bunit;
using FluentAssertions;
using Xunit;

namespace DExpressClone.Components.Tests.Editors;

public class DxCheckBoxTests : TestContext
{
    [Fact]
    public void RendersCheckbox()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxCheckBox.DxCheckBox>(parameters => parameters
            .Add(p => p.Checked, false));

        var root = cut.Find(".dx-checkbox");
        root.Should().NotBeNull();
        root.GetAttribute("role").Should().Be("checkbox");
    }

    [Fact]
    public void ClickTogglesChecked()
    {
        bool? checkedState = false;
        var cut = RenderComponent<DExpressClone.Components.Editors.DxCheckBox.DxCheckBox>(parameters => parameters
            .Add(p => p.Checked, false)
            .Add(p => p.CheckedChanged, (bool? val) => checkedState = val));

        cut.Find(".dx-checkbox").Click();

        checkedState.Should().BeTrue();
    }

    [Fact]
    public void TriStateCycling_WithAllowIndeterminate()
    {
        bool? checkedState = true;
        var cut = RenderComponent<DExpressClone.Components.Editors.DxCheckBox.DxCheckBox>(parameters => parameters
            .Add(p => p.Checked, true)
            .Add(p => p.AllowIndeterminate, true)
            .Add(p => p.CheckedChanged, (bool? val) => checkedState = val));

        // true -> false
        cut.Find(".dx-checkbox").Click();
        checkedState.Should().BeFalse();

        // Update the parameter to reflect the new state
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Checked, false)
            .Add(p => p.AllowIndeterminate, true)
            .Add(p => p.CheckedChanged, (bool? val) => checkedState = val));

        // false -> null (indeterminate)
        cut.Find(".dx-checkbox").Click();
        checkedState.Should().BeNull();

        // Update to indeterminate
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Checked, (bool?)null)
            .Add(p => p.AllowIndeterminate, true)
            .Add(p => p.CheckedChanged, (bool? val) => checkedState = val));

        // null -> true
        cut.Find(".dx-checkbox").Click();
        checkedState.Should().BeTrue();
    }

    [Fact]
    public void LabelText_Renders()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxCheckBox.DxCheckBox>(parameters => parameters
            .Add(p => p.Checked, false)
            .Add(p => p.LabelText, "Accept Terms"));

        cut.Find(".dx-checkbox-label").TextContent.Should().Be("Accept Terms");
    }

    [Fact]
    public void CheckedTrue_HasCheckedClass()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxCheckBox.DxCheckBox>(parameters => parameters
            .Add(p => p.Checked, true));

        cut.Find(".dx-checkbox").ClassList.Should().Contain("dx-checkbox--checked");
    }

    [Fact]
    public void Indeterminate_HasIndeterminateClass()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxCheckBox.DxCheckBox>(parameters => parameters
            .Add(p => p.Checked, (bool?)null)
            .Add(p => p.AllowIndeterminate, true));

        cut.Find(".dx-checkbox").ClassList.Should().Contain("dx-checkbox--indeterminate");
    }
}
