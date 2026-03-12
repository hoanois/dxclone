using System.Globalization;
using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DExpressClone.Components.Editors.DxNumericEdit;

/// <summary>
/// A numeric editor with spin buttons, min/max clamping, and display formatting.
/// </summary>
/// <typeparam name="TValue">A numeric struct type (int, double, decimal, etc.).</typeparam>
public partial class DxNumericEdit<TValue> : DxFormEditorBase<TValue> where TValue : struct, IComparable
{
    /// <summary>
    /// The minimum allowed value.
    /// </summary>
    [Parameter]
    public TValue? MinValue { get; set; }

    /// <summary>
    /// The maximum allowed value.
    /// </summary>
    [Parameter]
    public TValue? MaxValue { get; set; }

    /// <summary>
    /// The amount to increment or decrement on each spin step.
    /// </summary>
    [Parameter]
    public TValue? Increment { get; set; }

    /// <summary>
    /// The display format string, e.g. "N2" or "C2".
    /// </summary>
    [Parameter]
    public string? DisplayFormat { get; set; }

    /// <summary>
    /// Whether to show spin (up/down) buttons. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowSpinButtons { get; set; } = true;

    private string RootCssClass => CssClassBuilder.New("dx-numeric")
        .Add(InteractiveStateCssClass)
        .Add(ValidationCssClass)
        .Add(CssClass)
        .Build();

    // Value from the base class is TValue (struct, never null).
    // We use GetCurrentValue() to safely retrieve it.
    private TValue GetCurrentValue()
    {
        // Value is TValue? from base (but since TValue:struct in base is unconstrained,
        // the runtime type is just TValue). We cast through object to handle this.
        if (Value is TValue tv) return tv;
        return default;
    }

    private string DisplayValue
    {
        get
        {
            var val = GetCurrentValue();
            if (!string.IsNullOrEmpty(DisplayFormat) && val is IFormattable formattable)
                return formattable.ToString(DisplayFormat, CultureInfo.CurrentCulture);
            return val.ToString() ?? string.Empty;
        }
    }

    private async Task HandleInputChangeAsync(ChangeEventArgs e)
    {
        var text = e.Value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            await SetValueAsync(default(TValue)!);
            return;
        }

        if (TryParse(text, out var parsed))
        {
            var clamped = Clamp(parsed);
            await SetValueAsync(clamped!);
        }
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        if (!Enabled || ReadOnly) return;

        if (e.Key == "ArrowUp")
        {
            await SpinUpAsync();
        }
        else if (e.Key == "ArrowDown")
        {
            await SpinDownAsync();
        }
    }

    private async Task SpinUpAsync()
    {
        if (!Enabled || ReadOnly) return;
        var currentValue = GetCurrentValue();
        var increment = Increment.HasValue ? Increment.Value : GetDefaultIncrement();
        var newValue = Add(currentValue, increment);
        await SetValueAsync(Clamp(newValue)!);
    }

    private async Task SpinDownAsync()
    {
        if (!Enabled || ReadOnly) return;
        var currentValue = GetCurrentValue();
        var increment = Increment.HasValue ? Increment.Value : GetDefaultIncrement();
        var newValue = Subtract(currentValue, increment);
        await SetValueAsync(Clamp(newValue)!);
    }

    private TValue Clamp(TValue value)
    {
        if (MinValue.HasValue && value.CompareTo(MinValue.Value) < 0)
            return MinValue.Value;
        if (MaxValue.HasValue && value.CompareTo(MaxValue.Value) > 0)
            return MaxValue.Value;
        return value;
    }

    private static bool TryParse(string? text, out TValue result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(text)) return false;

        var cleanText = text.Trim();

        try
        {
            var targetType = typeof(TValue);
            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var converted = Convert.ChangeType(cleanText, targetType, CultureInfo.CurrentCulture);
            if (converted is TValue typedValue)
            {
                result = typedValue;
                return true;
            }
        }
        catch
        {
            // Parse failed
        }

        return false;
    }

    private static TValue Add(TValue a, TValue b)
    {
        var da = Convert.ToDouble(a, CultureInfo.InvariantCulture);
        var db = Convert.ToDouble(b, CultureInfo.InvariantCulture);
        var result = da + db;
        return (TValue)Convert.ChangeType(result, typeof(TValue), CultureInfo.InvariantCulture);
    }

    private static TValue Subtract(TValue a, TValue b)
    {
        var da = Convert.ToDouble(a, CultureInfo.InvariantCulture);
        var db = Convert.ToDouble(b, CultureInfo.InvariantCulture);
        var result = da - db;
        return (TValue)Convert.ChangeType(result, typeof(TValue), CultureInfo.InvariantCulture);
    }

    private static TValue GetDefaultIncrement()
    {
        return (TValue)Convert.ChangeType(1, typeof(TValue), CultureInfo.InvariantCulture);
    }
}
