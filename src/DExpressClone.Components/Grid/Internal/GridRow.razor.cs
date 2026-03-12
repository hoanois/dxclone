using System.Reflection;
using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DExpressClone.Components.Grid.Internal;

public partial class GridRow<TItem> : ComponentBase
{
    private TItem? _previousItem;
    private bool _previousIsSelected;
    private bool _previousIsFocused;
    private bool _previousIsEditing;

    [Parameter]
    public TItem? Item { get; set; }

    [Parameter]
    public IReadOnlyList<DxGridColumnBase> Columns { get; set; } = Array.Empty<DxGridColumnBase>();

    [Parameter]
    public bool IsSelected { get; set; }

    [Parameter]
    public bool IsFocused { get; set; }

    [Parameter]
    public int RowIndex { get; set; }

    [Parameter]
    public int RowHeight { get; set; } = 36;

    [Parameter]
    public EventCallback<TItem> RowClick { get; set; }

    [Parameter]
    public EventCallback<TItem> RowDoubleClick { get; set; }

    [Parameter]
    public EventCallback<TItem> SelectionToggled { get; set; }

    [Parameter]
    public bool IsEditing { get; set; }

    [Parameter]
    public object? EditItem { get; set; }

    [Parameter]
    public EditContext? RowEditContext { get; set; }

    protected string RowCssClass => CssClassBuilder.New("dx-grid-row")
        .AddIf("dx-grid-row--selected", IsSelected)
        .AddIf("dx-grid-row--focused", IsFocused)
        .AddIf("dx-grid-row--editing", IsEditing)
        .Add(RowIndex % 2 == 0 ? "dx-grid-row--even" : "dx-grid-row--odd")
        .Build();

    protected override bool ShouldRender()
    {
        var shouldRender = !ReferenceEquals(_previousItem, Item)
            || _previousIsSelected != IsSelected
            || _previousIsFocused != IsFocused
            || _previousIsEditing != IsEditing;

        _previousItem = Item;
        _previousIsSelected = IsSelected;
        _previousIsFocused = IsFocused;
        _previousIsEditing = IsEditing;

        return shouldRender;
    }

    private string GetCellCssClass(DxGridColumnBase column)
    {
        var builder = CssClassBuilder.New("dx-grid-cell");

        if (column is DxGridDataColumn dataCol)
        {
            builder.AddIf("dx-grid-cell--center", dataCol.TextAlignment == GridTextAlignment.Center);
            builder.AddIf("dx-grid-cell--right", dataCol.TextAlignment == GridTextAlignment.Right);
        }
        else if (column is DxGridSelectionColumn)
        {
            builder.Add("dx-grid-cell--selection");
        }

        return builder.Build();
    }

    private string GetCellDisplayValue(DxGridDataColumn column)
    {
        if (Item is null) return string.Empty;

        var prop = typeof(TItem).GetProperty(column.FieldName, BindingFlags.Public | BindingFlags.Instance);
        if (prop is null) return string.Empty;

        var value = prop.GetValue(Item);
        if (value is null) return string.Empty;

        if (!string.IsNullOrEmpty(column.DisplayFormat))
        {
            return string.Format(column.DisplayFormat, value);
        }

        return value.ToString() ?? string.Empty;
    }

    private async Task HandleClick()
    {
        if (Item is not null)
            await RowClick.InvokeAsync(Item);
    }

    private async Task HandleDoubleClick()
    {
        if (Item is not null)
            await RowDoubleClick.InvokeAsync(Item);
    }

    private async Task HandleSelectionToggle()
    {
        if (Item is not null)
            await SelectionToggled.InvokeAsync(Item);
    }
}
