using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Editors.DxRadioGroup;

/// <summary>
/// A radio button group editor that renders radio buttons for a collection of items.
/// </summary>
/// <typeparam name="TValue">The type of value each radio button represents.</typeparam>
public partial class DxRadioGroup<TValue> : DxFormEditorBase<TValue>
{
    /// <summary>
    /// The collection of items to render as radio buttons.
    /// </summary>
    [Parameter]
    public IEnumerable<TValue>? Items { get; set; }

    /// <summary>
    /// Data source when using ValueField/TextField projection.
    /// </summary>
    [Parameter]
    public IEnumerable<object>? Data { get; set; }

    /// <summary>
    /// Function to extract a value from a data item.
    /// </summary>
    [Parameter]
    public Func<object, TValue>? ValueField { get; set; }

    /// <summary>
    /// Function to extract display text from a data item.
    /// </summary>
    [Parameter]
    public Func<object, string>? TextField { get; set; }

    /// <summary>
    /// Layout direction for the radio buttons.
    /// </summary>
    [Parameter]
    public RadioGroupLayout Layout { get; set; } = RadioGroupLayout.Vertical;

    /// <summary>
    /// Custom template for rendering each item.
    /// </summary>
    [Parameter]
    public RenderFragment<TValue>? ItemTemplate { get; set; }

    private string RootCssClass => CssClassBuilder.New("dx-radio-group")
        .AddIf("dx-radio-group--horizontal", Layout == RadioGroupLayout.Horizontal)
        .Add(InteractiveStateCssClass)
        .Add(ValidationCssClass)
        .Add(CssClass)
        .Build();

    private IEnumerable<RadioItem> GetRadioItems()
    {
        if (Items is not null)
        {
            foreach (var item in Items)
            {
                yield return new RadioItem(item, item?.ToString() ?? string.Empty);
            }
        }
        else if (Data is not null)
        {
            foreach (var dataItem in Data)
            {
                var value = ValueField is not null ? ValueField(dataItem) : default;
                var text = TextField is not null ? TextField(dataItem) : dataItem.ToString() ?? string.Empty;
                yield return new RadioItem(value, text);
            }
        }
    }

    private bool IsSelected(TValue? itemValue)
    {
        return EqualityComparer<TValue>.Default.Equals(Value!, itemValue!);
    }

    private string GetRadioItemCssClass(TValue? itemValue)
    {
        return CssClassBuilder.New("dx-radio")
            .AddIf("dx-radio--checked", IsSelected(itemValue))
            .Build();
    }

    private async Task HandleItemClickAsync(TValue? itemValue)
    {
        if (!Enabled || ReadOnly) return;
        await SetValueAsync(itemValue);
    }

    private record struct RadioItem(TValue? Value, string Text);
}

/// <summary>
/// Layout direction for a radio group.
/// </summary>
public enum RadioGroupLayout
{
    /// <summary>Vertical layout (stacked).</summary>
    Vertical,
    /// <summary>Horizontal layout (inline).</summary>
    Horizontal
}
