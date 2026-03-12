using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Editors.DxComboBox.Internal;

/// <summary>
/// Internal dropdown component used by DxComboBox.
/// </summary>
/// <typeparam name="TData">The data item type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public partial class ComboBoxDropdown<TData, TValue> : DxComponentBase
{
    [Parameter]
    public IEnumerable<TData> Items { get; set; } = Enumerable.Empty<TData>();

    [Parameter]
    public RenderFragment<TData>? ItemTemplate { get; set; }

    [Parameter]
    public Func<TData, string>? GetItemText { get; set; }

    [Parameter]
    public Func<TData, int, string>? GetItemCssClass { get; set; }

    [Parameter]
    public EventCallback<TData> OnItemSelected { get; set; }

    [Parameter]
    public DropDownWidthMode DropDownWidthMode { get; set; }

    private string DropdownCssClass => CssClassBuilder.New("dx-combobox-dropdown")
        .AddIf("dx-combobox-dropdown--editor-width", DropDownWidthMode == DropDownWidthMode.EditorWidth)
        .Add(CssClass)
        .Build();

    private string GetCssClass(TData item, int index)
    {
        return GetItemCssClass?.Invoke(item, index) ?? "dx-combobox-item";
    }

    private string GetText(TData item)
    {
        return GetItemText?.Invoke(item) ?? item?.ToString() ?? string.Empty;
    }

    private async Task HandleItemClickAsync(TData item)
    {
        await OnItemSelected.InvokeAsync(item);
    }
}
