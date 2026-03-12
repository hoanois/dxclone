using DExpressClone.Components.Grid.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Grid.Internal;

public partial class GridHeader<TItem> : ComponentBase
{
    private readonly Dictionary<DxGridDataColumn, Func<Task>> _headerClickHandlers = new();

    [Parameter]
    public IReadOnlyList<DxGridColumnBase> Columns { get; set; } = Array.Empty<DxGridColumnBase>();

    [Parameter]
    public IReadOnlyList<GridSortDescriptor> SortDescriptors { get; set; } = Array.Empty<GridSortDescriptor>();

    [Parameter]
    public EventCallback<GridSortDescriptor> OnSortChanged { get; set; }

    [Parameter]
    public bool AllowSort { get; set; } = true;

    [Parameter]
    public bool AllowColumnResize { get; set; }

    private GridSortDescriptor? GetSortDescriptor(string fieldName)
    {
        return SortDescriptors.FirstOrDefault(s => s.FieldName == fieldName);
    }

    private Func<Task> GetHeaderClickHandler(DxGridDataColumn column)
    {
        if (!_headerClickHandlers.TryGetValue(column, out var handler))
        {
            handler = () => OnHeaderClick(column);
            _headerClickHandlers[column] = handler;
        }
        return handler;
    }

    private async Task OnHeaderClick(DxGridDataColumn column)
    {
        if (!column.AllowSort || !AllowSort)
            return;

        var existing = GetSortDescriptor(column.FieldName);
        var newOrder = existing?.SortOrder switch
        {
            GridSortOrder.None => GridSortOrder.Ascending,
            GridSortOrder.Ascending => GridSortOrder.Descending,
            GridSortOrder.Descending => GridSortOrder.None,
            _ => GridSortOrder.Ascending
        };

        await OnSortChanged.InvokeAsync(new GridSortDescriptor
        {
            FieldName = column.FieldName,
            SortOrder = newOrder
        });
    }
}
