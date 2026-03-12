using Bunit;
using DExpressClone.Components.Interop;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace DExpressClone.Components.Tests.Editors;

public class DxComboBoxTests : TestContext
{
    private readonly string[] _items = { "Apple", "Banana", "Cherry", "Date", "Elderberry" };

    public DxComboBoxTests()
    {
        Services.AddSingleton<JsInteropService>(sp =>
            new JsInteropService(JSInterop.JSRuntime));
        JSInterop.SetupVoid("import", _ => true);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersWithPlaceholder()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxComboBox.DxComboBox<string, string>>(parameters => parameters
            .Add(p => p.Data, _items)
            .Add(p => p.NullText, "Select a fruit...")
            .Add(p => p.TextFieldName, x => x)
            .Add(p => p.ValueFieldName, x => x));

        var input = cut.Find("input.dx-combobox-input");
        input.GetAttribute("placeholder").Should().Be("Select a fruit...");
    }

    [Fact]
    public void DropdownOpensOnButtonClick()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxComboBox.DxComboBox<string, string>>(parameters => parameters
            .Add(p => p.Data, _items)
            .Add(p => p.TextFieldName, x => x)
            .Add(p => p.ValueFieldName, x => x));

        // Initially no dropdown
        cut.FindAll(".dx-combobox-dropdown").Should().BeEmpty();

        // Click the toggle button
        cut.Find(".dx-combobox-button").Click();

        // Dropdown should now be visible
        cut.FindAll(".dx-combobox-item").Should().HaveCount(5);
    }

    [Fact]
    public void ItemSelection_UpdatesValue()
    {
        string? selectedValue = null;
        var cut = RenderComponent<DExpressClone.Components.Editors.DxComboBox.DxComboBox<string, string>>(parameters => parameters
            .Add(p => p.Data, _items)
            .Add(p => p.TextFieldName, x => x)
            .Add(p => p.ValueFieldName, x => x)
            .Add(p => p.ValueChanged, (string val) => selectedValue = val));

        // Open dropdown
        cut.Find(".dx-combobox-button").Click();

        // Click second item (Banana)
        var items = cut.FindAll(".dx-combobox-item");
        items[1].Click();

        selectedValue.Should().Be("Banana");
    }

    [Fact]
    public void Filtering_FiltersItems()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxComboBox.DxComboBox<string, string>>(parameters => parameters
            .Add(p => p.Data, _items)
            .Add(p => p.TextFieldName, x => x)
            .Add(p => p.ValueFieldName, x => x)
            .Add(p => p.AllowUserInput, true));

        // Type filter text
        var input = cut.Find("input.dx-combobox-input");
        input.Input("an");

        // Should show filtered results (Banana, Elderberry would not match; Banana contains "an")
        var items = cut.FindAll(".dx-combobox-item");
        items.Should().HaveCountGreaterThan(0);

        // All displayed items should contain "an" (case-insensitive)
        foreach (var item in items)
        {
            item.TextContent.Should().ContainEquivalentOf("an");
        }
    }

    [Fact]
    public void DisabledComboBox_CannotOpenDropdown()
    {
        var cut = RenderComponent<DExpressClone.Components.Editors.DxComboBox.DxComboBox<string, string>>(parameters => parameters
            .Add(p => p.Data, _items)
            .Add(p => p.TextFieldName, x => x)
            .Add(p => p.ValueFieldName, x => x)
            .Add(p => p.Enabled, false));

        cut.Find(".dx-combobox-button").Click();

        // Dropdown should not open
        cut.FindAll(".dx-combobox-item").Should().BeEmpty();
    }
}
