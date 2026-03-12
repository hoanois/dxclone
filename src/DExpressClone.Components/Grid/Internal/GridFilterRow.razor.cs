using DExpressClone.Components.Grid.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Grid.Internal;

public partial class GridFilterRow : ComponentBase
{
    private readonly Dictionary<string, Func<ChangeEventArgs, Task>> _filterHandlers = new();

    [Parameter]
    public IReadOnlyList<DxGridColumnBase> Columns { get; set; } = Array.Empty<DxGridColumnBase>();

    [Parameter]
    public IReadOnlyList<GridFilterDescriptor> FilterDescriptors { get; set; } = Array.Empty<GridFilterDescriptor>();

    [Parameter]
    public EventCallback<GridFilterDescriptor> OnFilterChanged { get; set; }

    private string GetFilterValue(string fieldName)
    {
        var descriptor = FilterDescriptors.FirstOrDefault(f => f.FieldName == fieldName);
        return descriptor?.FilterValue?.ToString() ?? string.Empty;
    }

    private Func<ChangeEventArgs, Task> GetFilterHandler(string fieldName)
    {
        if (!_filterHandlers.TryGetValue(fieldName, out var handler))
        {
            handler = e => OnFilterInput(fieldName, e.Value);
            _filterHandlers[fieldName] = handler;
        }
        return handler;
    }

    private async Task OnFilterInput(string fieldName, object? value)
    {
        var filterValue = value?.ToString();

        await OnFilterChanged.InvokeAsync(new GridFilterDescriptor
        {
            FieldName = fieldName,
            FilterValue = string.IsNullOrWhiteSpace(filterValue) ? null : filterValue,
            FilterOperator = GridFilterOperator.Contains
        });
    }
}
