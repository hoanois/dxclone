using DExpressClone.Components.Core;
using DExpressClone.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DExpressClone.Components.Editors.DxComboBox;

/// <summary>
/// A combo box editor with dropdown selection, filtering, and keyboard navigation.
/// </summary>
/// <typeparam name="TData">The data item type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public partial class DxComboBox<TData, TValue> : DxFormEditorBase<TValue>
{
    [Inject] private JsInteropService JsInterop { get; set; } = default!;

    /// <summary>
    /// The data source for the dropdown items.
    /// </summary>
    [Parameter]
    public IEnumerable<TData>? Data { get; set; }

    /// <summary>
    /// Function to extract a value from a data item.
    /// </summary>
    [Parameter]
    public Func<TData, TValue>? ValueFieldName { get; set; }

    /// <summary>
    /// Function to extract display text from a data item.
    /// </summary>
    [Parameter]
    public Func<TData, string>? TextFieldName { get; set; }

    /// <summary>
    /// Whether to allow the user to type custom input.
    /// </summary>
    [Parameter]
    public bool AllowUserInput { get; set; } = false;

    /// <summary>
    /// The filtering mode for the dropdown.
    /// </summary>
    [Parameter]
    public ComboBoxFilteringMode FilteringMode { get; set; } = ComboBoxFilteringMode.Contains;

    /// <summary>
    /// Controls when the clear button is displayed.
    /// </summary>
    [Parameter]
    public DxTextBox.ClearButtonDisplayMode ClearButtonDisplayMode { get; set; } = DxTextBox.ClearButtonDisplayMode.Never;

    /// <summary>
    /// Controls the dropdown width behavior.
    /// </summary>
    [Parameter]
    public DropDownWidthMode DropDownWidthMode { get; set; } = DropDownWidthMode.ContentOrEditorWidth;

    /// <summary>
    /// Custom template for rendering each dropdown item.
    /// </summary>
    [Parameter]
    public RenderFragment<TData>? ItemTemplate { get; set; }

    /// <summary>
    /// Custom template for rendering the selected item in the input.
    /// </summary>
    [Parameter]
    public RenderFragment<TData>? SelectedItemTemplate { get; set; }

    private bool IsDropdownOpen { get; set; } = false;
    private string FilterText { get; set; } = string.Empty;
    private int HighlightedIndex { get; set; } = -1;
    private bool _dropUp;
    private ElementReference _containerRef;

    private bool IsInputReadOnly => ReadOnly || (!AllowUserInput && !IsDropdownOpen);

    private string RootCssClass => CssClassBuilder.New("dx-combobox")
        .Add(InteractiveStateCssClass)
        .Add(ValidationCssClass)
        .Add(CssClass)
        .Build();

    private string DisplayText
    {
        get
        {
            if (Data is null || Value is null) return FilterText;
            var item = Data.FirstOrDefault(d => EqualityComparer<TValue>.Default.Equals(GetItemValue(d)!, Value!));
            if (item is not null)
                return GetItemText(item);
            return FilterText;
        }
    }

    private TValue? GetItemValue(TData item)
    {
        if (ValueFieldName is not null)
            return ValueFieldName(item);
        if (item is TValue val)
            return val;
        return default;
    }

    private string GetItemText(TData item)
    {
        if (TextFieldName is not null)
            return TextFieldName(item);
        return item?.ToString() ?? string.Empty;
    }

    private IEnumerable<TData> GetFilteredItems()
    {
        if (Data is null) return Enumerable.Empty<TData>();
        if (string.IsNullOrEmpty(FilterText)) return Data;

        return FilteringMode switch
        {
            ComboBoxFilteringMode.StartsWith => Data.Where(d =>
                GetItemText(d).StartsWith(FilterText, StringComparison.OrdinalIgnoreCase)),
            _ => Data.Where(d =>
                GetItemText(d).Contains(FilterText, StringComparison.OrdinalIgnoreCase))
        };
    }

    private bool IsItemSelected(TData item)
    {
        return EqualityComparer<TValue>.Default.Equals(GetItemValue(item)!, Value!);
    }

    private string GetItemCssClass(TData item, int index)
    {
        return CssClassBuilder.New("dx-combobox-item")
            .AddIf("dx-combobox-item--highlighted", index == HighlightedIndex)
            .AddIf("dx-combobox-item--selected", IsItemSelected(item))
            .Build();
    }

    private void CloseDropdown()
    {
        IsDropdownOpen = false;
        FilterText = string.Empty;
        RequestRender();
    }

    private async Task ToggleDropdown()
    {
        if (!Enabled || ReadOnly) return;
        if (!IsDropdownOpen)
        {
            _dropUp = await JsInterop.ShouldDropUpAsync(_containerRef, 200);
            HighlightedIndex = -1;
        }
        else
        {
            FilterText = string.Empty;
        }
        IsDropdownOpen = !IsDropdownOpen;
        RequestRender();
    }

    private async Task OpenDropdown()
    {
        if (!Enabled || ReadOnly || IsDropdownOpen) return;
        _dropUp = await JsInterop.ShouldDropUpAsync(_containerRef, 200);
        IsDropdownOpen = true;
        HighlightedIndex = -1;
        RequestRender();
    }

    private async Task HandleInputChange(ChangeEventArgs e)
    {
        FilterText = e.Value?.ToString() ?? string.Empty;
        await OpenDropdown();
        RequestRender();
    }

    private async Task SelectItemAsync(TData item)
    {
        var value = GetItemValue(item);
        FilterText = GetItemText(item);
        IsDropdownOpen = false;
        await SetValueAsync(value);
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        if (!Enabled || ReadOnly) return;

        var items = GetFilteredItems().ToList();

        switch (e.Key)
        {
            case "ArrowDown":
                await OpenDropdown();
                HighlightedIndex = Math.Min(HighlightedIndex + 1, items.Count - 1);
                RequestRender();
                break;
            case "ArrowUp":
                HighlightedIndex = Math.Max(HighlightedIndex - 1, 0);
                RequestRender();
                break;
            case "Enter":
                if (IsDropdownOpen && HighlightedIndex >= 0 && HighlightedIndex < items.Count)
                {
                    await SelectItemAsync(items[HighlightedIndex]);
                }
                break;
            case "Escape":
                IsDropdownOpen = false;
                RequestRender();
                break;
        }
    }

    private bool ShowClearButton => ClearButtonDisplayMode == DxTextBox.ClearButtonDisplayMode.Auto
        && Value is not null && Enabled && !ReadOnly;

    private async Task HandleClearAsync()
    {
        FilterText = string.Empty;
        IsDropdownOpen = false;
        await SetValueAsync(default);
    }
}

/// <summary>
/// Filtering mode for combo box dropdown items.
/// </summary>
public enum ComboBoxFilteringMode
{
    /// <summary>Filter items that contain the search text.</summary>
    Contains,
    /// <summary>Filter items that start with the search text.</summary>
    StartsWith
}

/// <summary>
/// Controls the dropdown width behavior.
/// </summary>
public enum DropDownWidthMode
{
    /// <summary>Dropdown width matches content or editor width, whichever is larger.</summary>
    ContentOrEditorWidth,
    /// <summary>Dropdown width matches the editor width.</summary>
    EditorWidth
}
