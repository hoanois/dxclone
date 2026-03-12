using DExpressClone.Components.Core;
using DExpressClone.Components.Grid.DataProcessing;
using DExpressClone.Components.Grid.Internal;
using DExpressClone.Components.Grid.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Grid;

public partial class DxGrid<TItem> : DxDataBoundComponentBase<TItem>, IDxGridColumnOwner
{
    private readonly List<DxGridColumnBase> _columns = new();
    private readonly List<GridSortDescriptor> _sortDescriptors = new();
    private readonly List<GridFilterDescriptor> _filterDescriptors = new();
    private readonly GridSelectionModel<TItem> _selectionModel = new();
    private readonly GridVirtualizationState _virtualizationState = new();
    private readonly VirtualScrollCalculator _scrollCalculator = new();

    private GridDataProcessor<TItem> _dataProcessor = new();
    private IReadOnlyList<TItem> _processedData = Array.Empty<TItem>();
    private int _pageIndex;
    private TItem? _focusedItem;

    // --- Parameters ---

    [Parameter]
    public int PageSize { get; set; }

    [Parameter]
    public bool ShowFilterRow { get; set; }

    [Parameter]
    public bool AllowSort { get; set; } = true;

    [Parameter]
    public bool AllowSelection { get; set; }

    [Parameter]
    public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.Single;

    [Parameter]
    public string GridHeight { get; set; } = "500px";

    [Parameter]
    public int RowHeight { get; set; } = 36;

    [Parameter]
    public IEnumerable<TItem>? SelectedItems { get; set; }

    [Parameter]
    public EventCallback<IEnumerable<TItem>> SelectedItemsChanged { get; set; }

    [Parameter]
    public TItem? FocusedItem { get; set; }

    [Parameter]
    public EventCallback<TItem?> FocusedItemChanged { get; set; }

    [Parameter]
    public RenderFragment? Columns { get; set; }

    [Parameter]
    public RenderFragment<TItem>? RowDetailTemplate { get; set; }

    [Parameter]
    public RenderFragment? EmptyDataTemplate { get; set; }

    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    [Parameter]
    public EventCallback<TItem> RowClick { get; set; }

    [Parameter]
    public EventCallback<TItem> RowDoubleClick { get; set; }

    [Parameter]
    public EventCallback<GridSortDescriptor> SortChanged { get; set; }

    [Parameter]
    public EventCallback<GridFilterDescriptor> FilterChanged { get; set; }

    [Parameter]
    public string? KeyFieldName { get; set; }

    // --- Computed Properties ---

    protected bool IsVirtualizationMode => PageSize <= 0;

    protected IReadOnlyList<TItem> ProcessedData => _processedData;

    protected IReadOnlyList<TItem> PagedData
    {
        get
        {
            if (IsVirtualizationMode || PageSize <= 0)
                return _processedData;

            return _processedData
                .Skip(_pageIndex * PageSize)
                .Take(PageSize)
                .ToList()
                .AsReadOnly();
        }
    }

    protected int PageCount =>
        PageSize > 0 ? (int)Math.Ceiling((double)_processedData.Count / PageSize) : 1;

    protected string GridCssClass => BuildCssClass("dx-grid");

    protected string? GridStyle => IsVirtualizationMode ? null : $"max-height:{GridHeight};";

    // --- Lifecycle ---

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        _selectionModel.SelectionMode = SelectionMode;
        if (SelectedItems is not null)
            _selectionModel.SetSelectedItems(SelectedItems);

        _focusedItem = FocusedItem;

        RefreshProcessedData();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        // Re-process after async data load completes
        RefreshProcessedData();
    }

    // --- Column Registration ---

    public void AddColumn(DxGridColumnBase column)
    {
        if (!_columns.Contains(column))
        {
            column.ColumnIndex = _columns.Count;
            _columns.Add(column);
            MarkDirty();
            StateHasChanged();
        }
    }

    public void RemoveColumn(DxGridColumnBase column)
    {
        _columns.Remove(column);
        MarkDirty();
        StateHasChanged();
    }

    // --- Data Processing ---

    private void RefreshProcessedData()
    {
        _dataProcessor = new GridDataProcessor<TItem>
        {
            SortDescriptors = _sortDescriptors,
            FilterDescriptors = _filterDescriptors.Where(f => f.FilterValue is not null).ToList(),
            PageSize = 0 // We handle paging separately for virtualization
        };

        var result = _dataProcessor.Process(InternalData);
        _processedData = result.FlatData;

        // Recalculate virtual scroll window
        if (IsVirtualizationMode)
        {
            RecalculateVirtualWindow();
        }
    }

    private void RecalculateVirtualWindow()
    {
        _virtualizationState.CurrentWindow = _scrollCalculator.Calculate(
            _processedData.Count,
            _virtualizationState.ScrollTop,
            _virtualizationState.ViewportHeight > 0 ? _virtualizationState.ViewportHeight : 500);
    }

    // --- Event Handlers ---

    private async Task HandleSortChanged(GridSortDescriptor descriptor)
    {
        var existing = _sortDescriptors.FirstOrDefault(s => s.FieldName == descriptor.FieldName);
        if (existing is not null)
            _sortDescriptors.Remove(existing);

        if (descriptor.SortOrder != GridSortOrder.None)
            _sortDescriptors.Add(descriptor);

        RefreshProcessedData();
        await SortChanged.InvokeAsync(descriptor);
        RequestRender();
    }

    private async Task HandleFilterChanged(GridFilterDescriptor descriptor)
    {
        var existing = _filterDescriptors.FirstOrDefault(f => f.FieldName == descriptor.FieldName);
        if (existing is not null)
            _filterDescriptors.Remove(existing);

        _filterDescriptors.Add(descriptor);
        _pageIndex = 0;

        RefreshProcessedData();
        await FilterChanged.InvokeAsync(descriptor);
        RequestRender();
    }

    private async Task HandleScroll(ScrollEventArgs args)
    {
        _virtualizationState.ScrollTop = args.ScrollTop;
        _virtualizationState.ViewportHeight = args.ViewportHeight;
        RecalculateVirtualWindow();
        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleRowClick(TItem item)
    {
        if (AllowSelection)
        {
            _selectionModel.Toggle(item);
            await SelectedItemsChanged.InvokeAsync(_selectionModel.SelectedItems);
        }

        _focusedItem = item;
        await FocusedItemChanged.InvokeAsync(item);
        await RowClick.InvokeAsync(item);
        RequestRender();
    }

    private async Task HandleRowDoubleClick(TItem item)
    {
        await RowDoubleClick.InvokeAsync(item);
    }

    private async Task HandleSelectionToggled(TItem item)
    {
        if (!AllowSelection) return;

        _selectionModel.Toggle(item);
        await SelectedItemsChanged.InvokeAsync(_selectionModel.SelectedItems);
        RequestRender();
    }

    private async Task HandlePageIndexChanged(int pageIndex)
    {
        _pageIndex = pageIndex;
        RequestRender();
        await Task.CompletedTask;
    }

    private bool IsItemFocused(TItem item) =>
        _focusedItem is not null && ReferenceEquals(_focusedItem, item);
}
