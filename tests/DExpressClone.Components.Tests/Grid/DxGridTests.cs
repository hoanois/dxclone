using Bunit;
using DExpressClone.Components.Grid;
using DExpressClone.Components.Grid.Models;
using DExpressClone.Components.Interop;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace DExpressClone.Components.Tests.Grid;

public class DxGridTests : TestContext
{
    private record TestItem(string Name, int Age);

    private readonly List<TestItem> _testData = new()
    {
        new("Alice", 25),
        new("Bob", 30),
        new("Charlie", 35),
    };

    public DxGridTests()
    {
        // Register a mock JSRuntime for JsInteropService
        Services.AddSingleton<JsInteropService>(sp =>
            new JsInteropService(JSInterop.JSRuntime));
        JSInterop.SetupVoid("import", _ => true);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersGridContainer()
    {
        var cut = RenderComponent<DxGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, _testData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.AddAttribute(2, "Caption", "Name");
                builder.CloseComponent();
            }));

        cut.Find(".dx-grid").Should().NotBeNull();
    }

    [Fact]
    public void RendersColumnHeaders()
    {
        var cut = RenderComponent<DxGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, _testData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.AddAttribute(2, "Caption", "Full Name");
                builder.CloseComponent();

                builder.OpenComponent<DxGridDataColumn>(3);
                builder.AddAttribute(4, "FieldName", "Age");
                builder.AddAttribute(5, "Caption", "Age");
                builder.CloseComponent();
            }));

        var headers = cut.FindAll(".dx-grid-header-caption");
        headers.Should().HaveCount(2);
        headers[0].TextContent.Should().Be("Full Name");
        headers[1].TextContent.Should().Be("Age");
    }

    [Fact]
    public void RendersDataRows()
    {
        var cut = RenderComponent<DxGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, _testData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.AddAttribute(2, "Caption", "Name");
                builder.CloseComponent();
            }));

        var rows = cut.FindAll(".dx-grid-row");
        rows.Should().HaveCount(3);
    }

    [Fact]
    public void EmptyData_ShowsEmptyDataTemplate()
    {
        var cut = RenderComponent<DxGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, new List<TestItem>())
            .Add(p => p.PageSize, 10)
            .Add(p => p.EmptyDataTemplate, builder =>
            {
                builder.AddContent(0, "No items found.");
            })
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.CloseComponent();
            }));

        cut.Markup.Should().Contain("No items found.");
    }

    [Fact]
    public void EmptyData_WithoutTemplate_ShowsDefaultMessage()
    {
        var cut = RenderComponent<DxGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, new List<TestItem>())
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.CloseComponent();
            }));

        cut.Find(".dx-grid-empty").TextContent.Should().Contain("No data to display");
    }

    [Fact]
    public void SortClick_ShowsSortIndicator()
    {
        GridSortDescriptor? lastSort = null;
        var cut = RenderComponent<DxGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, _testData)
            .Add(p => p.PageSize, 10)
            .Add(p => p.AllowSort, true)
            .Add(p => p.SortChanged, (GridSortDescriptor desc) => lastSort = desc)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DxGridDataColumn>(0);
                builder.AddAttribute(1, "FieldName", "Name");
                builder.AddAttribute(2, "Caption", "Name");
                builder.AddAttribute(3, "AllowSort", true);
                builder.CloseComponent();
            }));

        // Click header to sort ascending
        cut.Find(".dx-grid-header-cell-content").Click();

        lastSort.Should().NotBeNull();
        lastSort!.FieldName.Should().Be("Name");
        lastSort.SortOrder.Should().Be(GridSortOrder.Ascending);

        // Should show ascending sort indicator
        cut.FindAll(".dx-grid-sort-asc").Should().HaveCount(1);
    }
}
