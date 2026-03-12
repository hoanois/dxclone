using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Bunit;
using DExpressClone.Components.Grid;
using DExpressClone.Components.Grid.DataProcessing;
using DExpressClone.Components.Grid.Models;
using DExpressClone.Components.Interop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace DExpressClone.Components.Benchmarks;

public record BenchmarkItem(string Name, int Age, string Department, decimal Salary);

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class GridRenderBenchmark
{
    private IReadOnlyList<BenchmarkItem> _items = null!;
    private VirtualScrollCalculator _scrollCalculator = null!;
    private GridDataProcessor<BenchmarkItem> _dataProcessor = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var departments = new[] { "Engineering", "Marketing", "Sales", "HR", "Finance" };
        var names = new[] { "Alice", "Bob", "Charlie", "Diana", "Eve", "Frank", "Grace", "Hank" };

        _items = Enumerable.Range(0, 100_000)
            .Select(i => new BenchmarkItem(
                $"{names[random.Next(names.Length)]}_{i}",
                random.Next(20, 65),
                departments[random.Next(departments.Length)],
                random.Next(30000, 150000)))
            .ToList()
            .AsReadOnly();

        _scrollCalculator = new VirtualScrollCalculator { RowHeight = 36, OverscanCount = 5 };
        _dataProcessor = new GridDataProcessor<BenchmarkItem>();
    }

    [Benchmark]
    [Arguments(0)]
    [Arguments(50000)]
    [Arguments(99000)]
    public VirtualizationWindow VirtualScrollCalculate(int scrollRow)
    {
        return _scrollCalculator.Calculate(100_000, scrollRow * 36.0, 500);
    }

    [Benchmark]
    public IReadOnlyList<BenchmarkItem> SortByName()
    {
        var sorts = new List<GridSortDescriptor>
        {
            new() { FieldName = "Name", SortOrder = GridSortOrder.Ascending }
        };
        return _dataProcessor.Sort(_items, sorts);
    }

    [Benchmark]
    public IReadOnlyList<BenchmarkItem> SortByAge()
    {
        var sorts = new List<GridSortDescriptor>
        {
            new() { FieldName = "Age", SortOrder = GridSortOrder.Descending }
        };
        return _dataProcessor.Sort(_items, sorts);
    }

    [Benchmark]
    public IReadOnlyList<BenchmarkItem> FilterByDepartment()
    {
        var filters = new List<GridFilterDescriptor>
        {
            new() { FieldName = "Department", FilterValue = "Engineering", FilterOperator = GridFilterOperator.Equals }
        };
        return _dataProcessor.Filter(_items, filters);
    }

    [Benchmark]
    public IReadOnlyList<BenchmarkItem> SortAndFilter()
    {
        var filters = new List<GridFilterDescriptor>
        {
            new() { FieldName = "Department", FilterValue = "Engineering", FilterOperator = GridFilterOperator.Equals }
        };
        var sorts = new List<GridSortDescriptor>
        {
            new() { FieldName = "Name", SortOrder = GridSortOrder.Ascending }
        };
        var filtered = _dataProcessor.Filter(_items, filters);
        return _dataProcessor.Sort(filtered, sorts);
    }
}
