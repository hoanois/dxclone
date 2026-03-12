using System.Globalization;
using DExpressClone.Components.Core;
using DExpressClone.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DExpressClone.Components.Editors.DxDateEdit;

/// <summary>
/// A date editor with an integrated calendar dropdown picker.
/// </summary>
public partial class DxDateEdit : DxFormEditorBase<DateTime?>
{
    [Inject] private JsInteropService JsInterop { get; set; } = default!;

    /// <summary>
    /// Gets or sets the selected date.
    /// </summary>
    [Parameter]
    public DateTime? Date { get; set; }

    /// <summary>
    /// Callback invoked when the date changes.
    /// </summary>
    [Parameter]
    public EventCallback<DateTime?> DateChanged { get; set; }

    /// <summary>
    /// The minimum selectable date.
    /// </summary>
    [Parameter]
    public DateTime? MinDate { get; set; }

    /// <summary>
    /// The maximum selectable date.
    /// </summary>
    [Parameter]
    public DateTime? MaxDate { get; set; }

    /// <summary>
    /// The display format for the date, e.g. "d", "D", "MM/dd/yyyy".
    /// </summary>
    [Parameter]
    public string? DisplayFormat { get; set; }

    /// <summary>
    /// Whether to show a time section in the editor.
    /// </summary>
    [Parameter]
    public bool TimeSectionVisible { get; set; } = false;

    private bool IsCalendarOpen { get; set; } = false;
    private bool _dropUp;
    private int _inputKey;
    private ElementReference _containerRef;

    private string RootCssClass => CssClassBuilder.New("dx-dateedit")
        .Add(InteractiveStateCssClass)
        .Add(ValidationCssClass)
        .Add(CssClass)
        .Build();

    private string DisplayValue
    {
        get
        {
            if (Date is null) return string.Empty;
            var format = DisplayFormat ?? (TimeSectionVisible ? "g" : "d");
            return Date.Value.ToString(format, CultureInfo.CurrentCulture);
        }
    }

    private async Task ToggleCalendar()
    {
        if (!Enabled || ReadOnly) return;
        if (!IsCalendarOpen)
        {
            _dropUp = await JsInterop.ShouldDropUpAsync(_containerRef, 320);
        }
        IsCalendarOpen = !IsCalendarOpen;
        RequestRender();
    }

    private async Task HandleDateSelectedAsync(DateTime date)
    {
        Date = date;
        await DateChanged.InvokeAsync(date);
        await SetValueAsync(date);
        IsCalendarOpen = false;
        RequestRender();
    }

    private async Task HandleInputChangeAsync(ChangeEventArgs e)
    {
        var text = e.Value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            Date = null;
            await DateChanged.InvokeAsync(null);
            await SetValueAsync(null);
            return;
        }

        if (DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out var parsed))
        {
            if ((MinDate is null || parsed >= MinDate) && (MaxDate is null || parsed <= MaxDate))
            {
                Date = parsed;
                await DateChanged.InvokeAsync(parsed);
                await SetValueAsync(parsed);
                return;
            }
        }

        // Invalid text: force input to revert to DisplayValue by changing @key
        _inputKey++;
        RequestRender();
    }

    private void HandleClickOutside()
    {
        if (IsCalendarOpen)
        {
            IsCalendarOpen = false;
            RequestRender();
        }
    }
}
