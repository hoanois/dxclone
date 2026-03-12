using DExpressClone.Components.Grid.DataProcessing;
using DExpressClone.Components.Grid.Models;
using FluentAssertions;
using Xunit;

namespace DExpressClone.Components.Tests.Grid;

public class GridDataProcessorTests
{
    private record TestPerson(string Name, int Age, string Department);

    private readonly GridDataProcessor<TestPerson> _processor = new();

    private readonly IReadOnlyList<TestPerson> _testData = new List<TestPerson>
    {
        new("Charlie", 30, "Engineering"),
        new("Alice", 25, "Marketing"),
        new("Bob", 35, "Engineering"),
        new("Diana", 28, "Sales"),
        new("Eve", 22, "Marketing"),
    };

    [Fact]
    public void Sort_Ascending_ByName()
    {
        var sorts = new List<GridSortDescriptor>
        {
            new() { FieldName = "Name", SortOrder = GridSortOrder.Ascending }
        };

        var result = _processor.Sort(_testData, sorts);

        result.Select(p => p.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public void Sort_Descending_ByAge()
    {
        var sorts = new List<GridSortDescriptor>
        {
            new() { FieldName = "Age", SortOrder = GridSortOrder.Descending }
        };

        var result = _processor.Sort(_testData, sorts);

        result.Select(p => p.Age).Should().BeInDescendingOrder();
    }

    [Fact]
    public void Filter_Contains_OnName()
    {
        var filters = new List<GridFilterDescriptor>
        {
            new() { FieldName = "Name", FilterValue = "li", FilterOperator = GridFilterOperator.Contains }
        };

        var result = _processor.Filter(_testData, filters);

        result.Should().HaveCount(2);
        result.Select(p => p.Name).Should().Contain("Alice").And.Contain("Charlie");
    }

    [Fact]
    public void Filter_Equals_OnDepartment()
    {
        var filters = new List<GridFilterDescriptor>
        {
            new() { FieldName = "Department", FilterValue = "Engineering", FilterOperator = GridFilterOperator.Equals }
        };

        var result = _processor.Filter(_testData, filters);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Department == "Engineering");
    }

    [Fact]
    public void Combined_Sort_And_Filter()
    {
        var filters = new List<GridFilterDescriptor>
        {
            new() { FieldName = "Department", FilterValue = "Marketing", FilterOperator = GridFilterOperator.Equals }
        };
        var sorts = new List<GridSortDescriptor>
        {
            new() { FieldName = "Name", SortOrder = GridSortOrder.Ascending }
        };

        var filtered = _processor.Filter(_testData, filters);
        var result = _processor.Sort(filtered, sorts);

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Eve");
    }

    [Fact]
    public void Page_ReturnsCorrectSlice()
    {
        var result = _processor.Page(_testData, pageIndex: 1, pageSize: 2);

        result.Should().HaveCount(2);
        result[0].Should().Be(_testData[2]);
        result[1].Should().Be(_testData[3]);
    }

    [Fact]
    public void Page_FirstPage()
    {
        var result = _processor.Page(_testData, pageIndex: 0, pageSize: 2);

        result.Should().HaveCount(2);
        result[0].Should().Be(_testData[0]);
        result[1].Should().Be(_testData[1]);
    }

    [Fact]
    public void Page_LastPagePartial()
    {
        var result = _processor.Page(_testData, pageIndex: 2, pageSize: 2);

        result.Should().HaveCount(1);
        result[0].Should().Be(_testData[4]);
    }

    [Fact]
    public void Sort_WithNoSorts_ReturnsOriginalData()
    {
        var result = _processor.Sort(_testData, Array.Empty<GridSortDescriptor>());
        result.Should().BeSameAs(_testData);
    }

    [Fact]
    public void Filter_WithNoFilters_ReturnsOriginalData()
    {
        var result = _processor.Filter(_testData, Array.Empty<GridFilterDescriptor>());
        result.Should().BeSameAs(_testData);
    }
}
