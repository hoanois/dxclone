using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Editors.DxDateEdit.Internal;

/// <summary>
/// Internal calendar component used by DxDateEdit for date picking.
/// </summary>
public partial class DatePickerCalendar : DxComponentBase
{
    [Parameter]
    public DateTime? SelectedDate { get; set; }

    [Parameter]
    public DateTime? MinDate { get; set; }

    [Parameter]
    public DateTime? MaxDate { get; set; }

    [Parameter]
    public EventCallback<DateTime> OnDateSelected { get; set; }

    private DateTime DisplayMonth { get; set; }

    private static readonly string[] DayNames = { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (DisplayMonth == default)
        {
            DisplayMonth = SelectedDate?.Date ?? DateTime.Today;
            DisplayMonth = new DateTime(DisplayMonth.Year, DisplayMonth.Month, 1);
        }
    }

    private void PreviousMonth()
    {
        DisplayMonth = DisplayMonth.AddMonths(-1);
        RequestRender();
    }

    private void NextMonth()
    {
        DisplayMonth = DisplayMonth.AddMonths(1);
        RequestRender();
    }

    private async Task SelectDateAsync(DateTime date)
    {
        if (IsDateDisabled(date)) return;
        await OnDateSelected.InvokeAsync(date);
    }

    private async Task SelectTodayAsync()
    {
        var today = DateTime.Today;
        if (!IsDateDisabled(today))
        {
            await OnDateSelected.InvokeAsync(today);
        }
    }

    private bool IsDateDisabled(DateTime date)
    {
        if (MinDate.HasValue && date.Date < MinDate.Value.Date) return true;
        if (MaxDate.HasValue && date.Date > MaxDate.Value.Date) return true;
        return false;
    }

    private bool IsSelected(DateTime date)
    {
        return SelectedDate.HasValue && SelectedDate.Value.Date == date.Date;
    }

    private bool IsToday(DateTime date)
    {
        return date.Date == DateTime.Today;
    }

    private string GetDayCssClass(DateTime date)
    {
        return CssClassBuilder.New("dx-calendar-day")
            .AddIf("dx-calendar-day--selected", IsSelected(date))
            .AddIf("dx-calendar-day--today", IsToday(date))
            .AddIf("dx-calendar-day--disabled", IsDateDisabled(date))
            .AddIf("dx-calendar-day--other-month", date.Month != DisplayMonth.Month)
            .Build();
    }

    private List<DateTime> GetCalendarDays()
    {
        var days = new List<DateTime>();
        var firstOfMonth = new DateTime(DisplayMonth.Year, DisplayMonth.Month, 1);
        var startDay = firstOfMonth.AddDays(-(int)firstOfMonth.DayOfWeek);

        for (int i = 0; i < 42; i++) // 6 weeks
        {
            days.Add(startDay.AddDays(i));
        }

        return days;
    }

    private string MonthYearDisplay => DisplayMonth.ToString("MMMM yyyy");
}
