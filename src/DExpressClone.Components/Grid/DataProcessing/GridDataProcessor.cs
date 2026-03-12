using System.Reflection;
using DExpressClone.Components.Grid.Models;

namespace DExpressClone.Components.Grid.DataProcessing;

/// <summary>
/// Processes grid data through a pipeline of filter, sort, group, and page operations.
/// </summary>
public sealed class GridDataProcessor<TItem>
{
    /// <summary>
    /// Gets or sets the sort descriptors to apply.
    /// </summary>
    public IReadOnlyList<GridSortDescriptor> SortDescriptors { get; set; } = Array.Empty<GridSortDescriptor>();

    /// <summary>
    /// Gets or sets the filter descriptors to apply.
    /// </summary>
    public IReadOnlyList<GridFilterDescriptor> FilterDescriptors { get; set; } = Array.Empty<GridFilterDescriptor>();

    /// <summary>
    /// Gets or sets the group descriptors to apply.
    /// </summary>
    public IReadOnlyList<GridGroupDescriptor> GroupDescriptors { get; set; } = Array.Empty<GridGroupDescriptor>();

    /// <summary>
    /// Gets or sets the current page index (zero-based).
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// Gets or sets the page size. 0 means no paging.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Sorts data by the specified sort descriptors using reflection.
    /// </summary>
    public IReadOnlyList<TItem> Sort(IReadOnlyList<TItem> data, IReadOnlyList<GridSortDescriptor> sorts)
    {
        if (data.Count == 0 || sorts.Count == 0)
            return data;

        var activeSorts = sorts.Where(s => s.SortOrder != GridSortOrder.None).ToList();
        if (activeSorts.Count == 0)
            return data;

        IOrderedEnumerable<TItem>? ordered = null;

        for (int i = 0; i < activeSorts.Count; i++)
        {
            var sort = activeSorts[i];
            var prop = GetProperty(sort.FieldName);
            if (prop is null) continue;

            if (i == 0)
            {
                ordered = sort.SortOrder == GridSortOrder.Ascending
                    ? data.OrderBy(x => prop.GetValue(x))
                    : data.OrderByDescending(x => prop.GetValue(x));
            }
            else
            {
                ordered = sort.SortOrder == GridSortOrder.Ascending
                    ? ordered!.ThenBy(x => prop.GetValue(x))
                    : ordered!.ThenByDescending(x => prop.GetValue(x));
            }
        }

        return ordered?.ToList().AsReadOnly() ?? data;
    }

    /// <summary>
    /// Filters data by the specified filter descriptors using reflection.
    /// </summary>
    public IReadOnlyList<TItem> Filter(IReadOnlyList<TItem> data, IReadOnlyList<GridFilterDescriptor> filters)
    {
        if (data.Count == 0 || filters.Count == 0)
            return data;

        var result = data.AsEnumerable();

        foreach (var filter in filters)
        {
            if (filter.FilterValue is null)
                continue;

            var prop = GetProperty(filter.FieldName);
            if (prop is null) continue;

            result = result.Where(item => MatchesFilter(prop.GetValue(item), filter));
        }

        return result.ToList().AsReadOnly();
    }

    /// <summary>
    /// Groups data by the specified group descriptors.
    /// </summary>
    public IReadOnlyList<GridGroupResult<TItem>> Group(IReadOnlyList<TItem> data, IReadOnlyList<GridGroupDescriptor> groups)
    {
        if (data.Count == 0 || groups.Count == 0)
            return new List<GridGroupResult<TItem>>
            {
                new() { GroupValue = null, Items = data }
            };

        var firstGroup = groups[0];
        var prop = GetProperty(firstGroup.FieldName);
        if (prop is null)
            return new List<GridGroupResult<TItem>>
            {
                new() { GroupValue = null, Items = data }
            };

        return data
            .GroupBy(item => prop.GetValue(item))
            .Select(g => new GridGroupResult<TItem>
            {
                GroupValue = g.Key,
                FieldName = firstGroup.FieldName,
                Items = g.ToList().AsReadOnly()
            })
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Pages data by the specified page index and size.
    /// </summary>
    public IReadOnlyList<TItem> Page(IReadOnlyList<TItem> data, int pageIndex, int pageSize)
    {
        if (pageSize <= 0 || data.Count == 0)
            return data;

        return data
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Runs the full processing pipeline: filter, sort, then group or page.
    /// </summary>
    public GridDataProcessorResult<TItem> Process(IReadOnlyList<TItem> data)
    {
        var filtered = Filter(data, FilterDescriptors);
        var sorted = Sort(filtered, SortDescriptors);

        if (GroupDescriptors.Count > 0)
        {
            var grouped = Group(sorted, GroupDescriptors);
            return new GridDataProcessorResult<TItem>
            {
                FlatData = sorted,
                Groups = grouped,
                TotalCount = filtered.Count,
                IsGrouped = true
            };
        }

        var paged = Page(sorted, PageIndex, PageSize);
        return new GridDataProcessorResult<TItem>
        {
            FlatData = paged,
            Groups = null,
            TotalCount = filtered.Count,
            IsGrouped = false
        };
    }

    private static PropertyInfo? GetProperty(string fieldName)
    {
        return typeof(TItem).GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
    }

    private static bool MatchesFilter(object? value, GridFilterDescriptor filter)
    {
        var filterValue = filter.FilterValue;
        if (filterValue is null) return true;

        var stringValue = value?.ToString() ?? string.Empty;
        var filterString = filterValue.ToString() ?? string.Empty;

        return filter.FilterOperator switch
        {
            GridFilterOperator.Contains => stringValue.Contains(filterString, StringComparison.OrdinalIgnoreCase),
            GridFilterOperator.Equals => stringValue.Equals(filterString, StringComparison.OrdinalIgnoreCase),
            GridFilterOperator.StartsWith => stringValue.StartsWith(filterString, StringComparison.OrdinalIgnoreCase),
            GridFilterOperator.GreaterThan => CompareValues(value, filterValue) > 0,
            GridFilterOperator.LessThan => CompareValues(value, filterValue) < 0,
            GridFilterOperator.Between => true, // Requires special handling with two values
            _ => true
        };
    }

    private static int CompareValues(object? a, object? b)
    {
        if (a is IComparable comparableA && b is not null)
        {
            try
            {
                return comparableA.CompareTo(Convert.ChangeType(b, a.GetType()));
            }
            catch
            {
                return 0;
            }
        }
        return 0;
    }
}

/// <summary>
/// Result of a grouping operation.
/// </summary>
public class GridGroupResult<TItem>
{
    public object? GroupValue { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public IReadOnlyList<TItem> Items { get; init; } = Array.Empty<TItem>();
}

/// <summary>
/// Result of the full data processing pipeline.
/// </summary>
public class GridDataProcessorResult<TItem>
{
    public IReadOnlyList<TItem> FlatData { get; init; } = Array.Empty<TItem>();
    public IReadOnlyList<GridGroupResult<TItem>>? Groups { get; init; }
    public int TotalCount { get; init; }
    public bool IsGrouped { get; init; }
}
