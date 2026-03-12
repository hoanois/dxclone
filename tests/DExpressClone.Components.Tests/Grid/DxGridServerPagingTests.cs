using Bunit;
using DExpressClone.Components.Grid;
using DExpressClone.Components.Grid.Models;
using DExpressClone.Components.Interop;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace DExpressClone.Components.Tests.Grid;

public class DxGridServerPagingTests : TestContext
{
    private class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
    }

    private readonly List<Employee> _allData;

    public DxGridServerPagingTests()
    {
        Services.AddSingleton<JsInteropService>(sp =>
            new JsInteropService(JSInterop.JSRuntime));
        JSInterop.SetupVoid("import", _ => true);
        JSInterop.Mode = JSRuntimeMode.Loose;

        _allData = Enumerable.Range(1, 50).Select(i => new Employee
        {
            Id = i,
            Name = $"Employee {i}",
            Department = i % 2 == 0 ? "Engineering" : "Sales",
            Salary = 40000 + i * 100
        }).ToList();
    }

    private Task<GridDataLoadResult<Employee>> MockServerData(GridDataLoadOptions options, CancellationToken ct)
    {
        var items = _allData.Skip(options.Skip).Take(options.Take).ToList();
        return Task.FromResult(new GridDataLoadResult<Employee>
        {
            Items = items,
            TotalCount = _allData.Count
        });
    }

    [Fact]
    public void CustomData_RendersGridWithServerData()
    {
        var cut = RenderComponent<DxGrid<Employee>>(parameters => parameters
            .Add(p => p.CustomData, MockServerData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.AddAttribute(2, "Caption", "Name");
                builder.CloseComponent();
            }));

        cut.Find(".dx-grid").Should().NotBeNull();
        var rows = cut.FindAll(".dx-grid-row");
        rows.Should().HaveCount(10);
    }

    [Fact]
    public void CustomData_RendersPager()
    {
        var cut = RenderComponent<DxGrid<Employee>>(parameters => parameters
            .Add(p => p.CustomData, MockServerData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.CloseComponent();
            }));

        // Pager should be rendered
        cut.Find(".dx-grid-pager").Should().NotBeNull();
    }

    [Fact]
    public void CustomData_PassesCorrectSkipTake()
    {
        GridDataLoadOptions? lastOptions = null;

        Task<GridDataLoadResult<Employee>> TrackingServerData(GridDataLoadOptions options, CancellationToken ct)
        {
            lastOptions = options;
            return MockServerData(options, ct);
        }

        var cut = RenderComponent<DxGrid<Employee>>(parameters => parameters
            .Add(p => p.CustomData, TrackingServerData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.CloseComponent();
            }));

        lastOptions.Should().NotBeNull();
        lastOptions!.Skip.Should().Be(0);
        lastOptions.Take.Should().Be(10);
    }

    [Fact]
    public void CustomData_PageChange_UpdatesSkip()
    {
        GridDataLoadOptions? lastOptions = null;

        Task<GridDataLoadResult<Employee>> TrackingServerData(GridDataLoadOptions options, CancellationToken ct)
        {
            lastOptions = options;
            return MockServerData(options, ct);
        }

        var cut = RenderComponent<DxGrid<Employee>>(parameters => parameters
            .Add(p => p.CustomData, TrackingServerData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.CloseComponent();
            }));

        // Click next page button
        var nextBtn = cut.FindAll(".dx-grid-pager-btn").LastOrDefault(b => b.TextContent.Contains("▶") || b.TextContent.Contains("›") || b.TextContent.Contains("Next"));
        nextBtn?.Click();

        // After page change, skip should be 10
        lastOptions!.Skip.Should().Be(10);
        lastOptions.Take.Should().Be(10);
    }

    [Fact]
    public void CustomData_SortChange_ResetsPageAndPassesSortDescriptors()
    {
        GridDataLoadOptions? lastOptions = null;

        Task<GridDataLoadResult<Employee>> TrackingServerData(GridDataLoadOptions options, CancellationToken ct)
        {
            lastOptions = options;
            return MockServerData(options, ct);
        }

        var cut = RenderComponent<DxGrid<Employee>>(parameters => parameters
            .Add(p => p.CustomData, TrackingServerData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.AllowSort, true)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.AddAttribute(2, "Caption", "Name");
                builder.AddAttribute(3, "AllowSort", true);
                builder.CloseComponent();
            }));

        // Click header to sort
        cut.Find(".dx-grid-header-cell-content").Click();

        lastOptions.Should().NotBeNull();
        lastOptions!.Skip.Should().Be(0);
        lastOptions.SortDescriptors.Should().HaveCount(1);
        lastOptions.SortDescriptors[0].FieldName.Should().Be("Name");
    }

    [Fact]
    public void CustomData_EmptyResult_ShowsEmptyState()
    {
        Task<GridDataLoadResult<Employee>> EmptyData(GridDataLoadOptions options, CancellationToken ct)
        {
            return Task.FromResult(new GridDataLoadResult<Employee>
            {
                Items = Array.Empty<Employee>(),
                TotalCount = 0
            });
        }

        var cut = RenderComponent<DxGrid<Employee>>(parameters => parameters
            .Add(p => p.CustomData, EmptyData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.CloseComponent();
            }));

        cut.Find(".dx-grid-empty").Should().NotBeNull();
    }
}
