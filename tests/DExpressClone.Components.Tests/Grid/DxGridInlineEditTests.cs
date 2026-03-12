using Bunit;
using DExpressClone.Components.Grid;
using DExpressClone.Components.Grid.Models;
using DExpressClone.Components.Interop;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace DExpressClone.Components.Tests.Grid;

public class DxGridInlineEditTests : TestContext
{
    private class EditableItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private readonly List<EditableItem> _testData;

    public DxGridInlineEditTests()
    {
        Services.AddSingleton<JsInteropService>(sp =>
            new JsInteropService(JSInterop.JSRuntime));
        JSInterop.SetupVoid("import", _ => true);
        JSInterop.Mode = JSRuntimeMode.Loose;

        _testData = new()
        {
            new() { Id = 1, Name = "Alice", Age = 25 },
            new() { Id = 2, Name = "Bob", Age = 30 },
            new() { Id = 3, Name = "Charlie", Age = 35 },
        };
    }

    private IRenderedComponent<DxGrid<EditableItem>> RenderEditableGrid(
        EventCallback<GridEditStartingEventArgs<EditableItem>>? editStarting = null,
        EventCallback<EditableItem>? editStarted = null,
        EventCallback<GridRowUpdatingEventArgs<EditableItem>>? rowUpdating = null,
        EventCallback<EditableItem>? rowUpdated = null,
        EventCallback<EditableItem>? editCancelled = null)
    {
        return RenderComponent<DxGrid<EditableItem>>(parameters =>
        {
            parameters
                .Add(p => p.Data, _testData)
                .Add(p => p.PageSize, 10)
                .Add(p => p.Columns, builder =>
                {
                    builder.OpenComponent<DxGridDataColumn>(0);
                    builder.AddAttribute(1, "FieldName", "Name");
                    builder.AddAttribute(2, "Caption", "Name");
                    builder.CloseComponent();

                    builder.OpenComponent<DxGridDataColumn>(3);
                    builder.AddAttribute(4, "FieldName", "Age");
                    builder.AddAttribute(5, "Caption", "Age");
                    builder.CloseComponent();
                });

            if (editStarting.HasValue)
                parameters.Add(p => p.EditStarting, editStarting.Value);
            if (editStarted.HasValue)
                parameters.Add(p => p.EditStarted, editStarted.Value);
            if (rowUpdating.HasValue)
                parameters.Add(p => p.RowUpdating, rowUpdating.Value);
            if (rowUpdated.HasValue)
                parameters.Add(p => p.RowUpdated, rowUpdated.Value);
            if (editCancelled.HasValue)
                parameters.Add(p => p.EditCancelled, editCancelled.Value);
        });
    }

    [Fact]
    public async Task StartEditAsync_SetsEditState()
    {
        var cut = RenderEditableGrid();
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[0]));

        grid.IsEditing(_testData[0]).Should().BeTrue();
        grid.IsEditing(_testData[1]).Should().BeFalse();
    }

    [Fact]
    public async Task StartEditAsync_FiresEditStartedEvent()
    {
        EditableItem? startedItem = null;
        var cut = RenderEditableGrid(
            editStarted: EventCallback.Factory.Create<EditableItem>(this, item => startedItem = item));
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[0]));

        startedItem.Should().NotBeNull();
        startedItem!.Name.Should().Be("Alice");
    }

    [Fact]
    public async Task StartEditAsync_CanBeCancelled()
    {
        var cut = RenderEditableGrid(
            editStarting: EventCallback.Factory.Create<GridEditStartingEventArgs<EditableItem>>(this, args => args.Cancel = true));
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[0]));

        grid.IsEditing(_testData[0]).Should().BeFalse();
    }

    [Fact]
    public async Task CancelEditAsync_ClearsEditState()
    {
        EditableItem? cancelledItem = null;
        var cut = RenderEditableGrid(
            editCancelled: EventCallback.Factory.Create<EditableItem>(this, item => cancelledItem = item));
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[0]));
        grid.IsEditing(_testData[0]).Should().BeTrue();

        await cut.InvokeAsync(() => grid.CancelEditAsync());
        grid.IsEditing(_testData[0]).Should().BeFalse();
        cancelledItem.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveEditAsync_FiresRowUpdatedEvent()
    {
        EditableItem? updatedItem = null;
        var cut = RenderEditableGrid(
            rowUpdated: EventCallback.Factory.Create<EditableItem>(this, item => updatedItem = item));
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[0]));
        await cut.InvokeAsync(() => grid.SaveEditAsync());

        updatedItem.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveEditAsync_CanBeCancelled()
    {
        EditableItem? updatedItem = null;
        var cut = RenderEditableGrid(
            rowUpdating: EventCallback.Factory.Create<GridRowUpdatingEventArgs<EditableItem>>(this, args => args.Cancel = true),
            rowUpdated: EventCallback.Factory.Create<EditableItem>(this, item => updatedItem = item));
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[0]));
        await cut.InvokeAsync(() => grid.SaveEditAsync());

        // RowUpdated should NOT have fired because RowUpdating cancelled
        updatedItem.Should().BeNull();
        // Should still be in edit mode
        grid.IsEditing(_testData[0]).Should().BeTrue();
    }

    [Fact]
    public async Task StartEditAsync_CancelsPreviousEdit()
    {
        var cut = RenderEditableGrid();
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[0]));
        grid.IsEditing(_testData[0]).Should().BeTrue();

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[1]));
        grid.IsEditing(_testData[0]).Should().BeFalse();
        grid.IsEditing(_testData[1]).Should().BeTrue();
    }

    [Fact]
    public async Task StartEditAsync_AddsEditingCssClass()
    {
        var cut = RenderEditableGrid();
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[0]));

        cut.FindAll(".dx-grid-row--editing").Should().HaveCount(1);
    }

    [Fact]
    public async Task CancelEditAsync_RemovesEditingCssClass()
    {
        var cut = RenderEditableGrid();
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.StartEditAsync(_testData[0]));
        cut.FindAll(".dx-grid-row--editing").Should().HaveCount(1);

        await cut.InvokeAsync(() => grid.CancelEditAsync());
        cut.FindAll(".dx-grid-row--editing").Should().HaveCount(0);
    }

    [Fact]
    public async Task IsEditing_ReturnsFalse_WhenNotEditing()
    {
        var cut = RenderEditableGrid();
        var grid = cut.Instance;

        grid.IsEditing(_testData[0]).Should().BeFalse();
        grid.IsEditing(_testData[1]).Should().BeFalse();
    }

    [Fact]
    public async Task SaveEditAsync_WhenNotEditing_DoesNothing()
    {
        EditableItem? updatedItem = null;
        var cut = RenderEditableGrid(
            rowUpdated: EventCallback.Factory.Create<EditableItem>(this, item => updatedItem = item));
        var grid = cut.Instance;

        // Save without starting edit
        await cut.InvokeAsync(() => grid.SaveEditAsync());

        updatedItem.Should().BeNull();
    }

    [Fact]
    public async Task CancelEditAsync_WhenNotEditing_DoesNothing()
    {
        EditableItem? cancelledItem = null;
        var cut = RenderEditableGrid(
            editCancelled: EventCallback.Factory.Create<EditableItem>(this, item => cancelledItem = item));
        var grid = cut.Instance;

        await cut.InvokeAsync(() => grid.CancelEditAsync());

        cancelledItem.Should().BeNull();
    }
}
