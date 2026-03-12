using System.Text.Json;
using DExpressClone.Components.Core;
using DExpressClone.Components.Grid.DataProcessing;
using DExpressClone.Components.Grid.Internal;
using DExpressClone.Components.Grid.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DExpressClone.Components.Grid;

public partial class DxGrid<TItem> : DxDataBoundComponentBase<TItem>, IDxGridColumnOwner, IAsyncDisposable
{
    private readonly List<DxGridColumnBase> _columns = new();
    private readonly List<GridSortDescriptor> _sortDescriptors = new();
    private readonly List<GridFilterDescriptor> _filterDescriptors = new();
    private readonly GridSelectionModel<TItem> _selectionModel = new();
    private readonly GridVirtualizationState _virtualizationState = new();
    private readonly VirtualScrollCalculator _scrollCalculator = new();

    private readonly GridEditState<TItem> _editState = new();

    private GridDataProcessor<TItem> _dataProcessor = new();
    private IReadOnlyList<TItem> _processedData = Array.Empty<TItem>();
    private IReadOnlyList<TItem>? _cachedPagedData;
    private int _cachedPageIndex = -1;
    private int _cachedProcessedDataCount = -1;
    private int _pageIndex;
    private TItem? _focusedItem;

    // Server-side data fields
    private int _serverTotalCount;
    private CancellationTokenSource? _customDataCts;
    private int _serverWindowOffset;
    private int _serverWindowTake;
    private System.Timers.Timer? _scrollDebounceTimer;
    private const int ScrollDebounceMs = 150;

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

    [Parameter]
    public EventCallback<GridEditStartingEventArgs<TItem>> EditStarting { get; set; }

    [Parameter]
    public EventCallback<TItem> EditStarted { get; set; }

    [Parameter]
    public EventCallback<GridRowUpdatingEventArgs<TItem>> RowUpdating { get; set; }

    [Parameter]
    public EventCallback<TItem> RowUpdated { get; set; }

    [Parameter]
    public EventCallback<GridRowInsertingEventArgs<TItem>> RowInserting { get; set; }

    [Parameter]
    public EventCallback<TItem> RowInserted { get; set; }

    [Parameter]
    public EventCallback<TItem> EditCancelled { get; set; }

    /// <summary>
    /// Gets or sets a callback for server-side data loading.
    /// When set, the grid operates in server mode: sorting, filtering, and paging
    /// are handled by the callback instead of client-side processing.
    /// </summary>
    [Parameter]
    public Func<GridDataLoadOptions, CancellationToken, Task<GridDataLoadResult<TItem>>>? CustomData { get; set; }

    // --- Computed Properties ---

    private bool IsServerMode => CustomData is not null;

    protected bool IsVirtualizationMode => PageSize <= 0;

    protected IReadOnlyList<TItem> ProcessedData => _processedData;

    protected IReadOnlyList<TItem> PagedData
    {
        get
        {
            if (IsServerMode || IsVirtualizationMode || PageSize <= 0)
                return _processedData;

            if (_cachedPagedData is not null
                && _cachedPageIndex == _pageIndex
                && _cachedProcessedDataCount == _processedData.Count)
                return _cachedPagedData;

            _cachedPagedData = _processedData
                .Skip(_pageIndex * PageSize)
                .Take(PageSize)
                .ToList()
                .AsReadOnly();
            _cachedPageIndex = _pageIndex;
            _cachedProcessedDataCount = _processedData.Count;
            return _cachedPagedData;
        }
    }

    protected int PageCount => IsServerMode
        ? (PageSize > 0 ? (int)Math.Ceiling((double)_serverTotalCount / PageSize) : 1)
        : (PageSize > 0 ? (int)Math.Ceiling((double)_processedData.Count / PageSize) : 1);

    protected string GridCssClass => BuildCssClass("dx-grid");

    protected string? GridStyle => null;

    // --- Lifecycle ---

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        SuppressAutoLoad = IsServerMode;

        _selectionModel.SelectionMode = SelectionMode;
        if (SelectedItems is not null)
            _selectionModel.SetSelectedItems(SelectedItems);

        _focusedItem = FocusedItem;

        if (!IsServerMode)
            RefreshProcessedData();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (IsServerMode)
        {
            await LoadCustomDataAsync();
        }
        else
        {
            RefreshProcessedData();
        }
    }

    // --- Column Registration ---

    public void AddColumn(DxGridColumnBase column)
    {
        if (!_columns.Contains(column))
        {
            column.ColumnIndex = _columns.Count;
            _columns.Add(column);
            RequestRender();
        }
    }

    public void RemoveColumn(DxGridColumnBase column)
    {
        _columns.Remove(column);
        RequestRender();
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
        var totalCount = IsServerMode ? _serverTotalCount : _processedData.Count;
        _virtualizationState.CurrentWindow = _scrollCalculator.Calculate(
            totalCount,
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

        if (IsServerMode)
        {
            _pageIndex = 0;
            await LoadCustomDataAsync();
        }
        else
        {
            RefreshProcessedData();
        }

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

        if (IsServerMode)
            await LoadCustomDataAsync();
        else
            RefreshProcessedData();

        await FilterChanged.InvokeAsync(descriptor);
        RequestRender();
    }

    private async Task HandleScroll(ScrollEventArgs args)
    {
        _virtualizationState.ScrollTop = args.ScrollTop;
        _virtualizationState.ViewportHeight = args.ViewportHeight;

        if (IsServerMode)
        {
            // Debounce server calls
            _scrollDebounceTimer?.Stop();
            _scrollDebounceTimer?.Dispose();
            _scrollDebounceTimer = new System.Timers.Timer(ScrollDebounceMs);
            _scrollDebounceTimer.AutoReset = false;
            _scrollDebounceTimer.Elapsed += async (s, e) =>
            {
                await InvokeAsync(async () =>
                {
                    var visibleCount = (int)Math.Ceiling(_virtualizationState.ViewportHeight / RowHeight);
                    var startIndex = (int)Math.Floor(_virtualizationState.ScrollTop / RowHeight);
                    var overscan = visibleCount; // 1x overscan

                    _serverWindowOffset = Math.Max(0, startIndex - overscan);
                    _serverWindowTake = visibleCount + 2 * overscan;

                    await LoadCustomDataAsync();

                    RecalculateVirtualWindow();
                    RequestRender();
                });
            };
            _scrollDebounceTimer.Start();
        }
        else
        {
            RecalculateVirtualWindow();
            await InvokeAsync(RequestRender);
        }
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

        if (IsServerMode)
            await LoadCustomDataAsync();

        RequestRender();
    }

    private bool IsItemFocused(TItem item) =>
        _focusedItem is not null && ReferenceEquals(_focusedItem, item);

    // --- Server-Side Data Loading ---

    private async Task LoadCustomDataAsync()
    {
        _customDataCts?.Cancel();
        _customDataCts?.Dispose();
        _customDataCts = new CancellationTokenSource();
        var token = _customDataCts.Token;

        try
        {
            IsDataLoading = true;
            MarkDirty();

            var skip = IsVirtualizationMode ? _serverWindowOffset : _pageIndex * PageSize;
            var take = IsVirtualizationMode ? _serverWindowTake : PageSize;

            var options = new GridDataLoadOptions
            {
                Skip = skip,
                Take = take > 0 ? take : 50,
                SortDescriptors = _sortDescriptors.AsReadOnly(),
                FilterDescriptors = _filterDescriptors.Where(f => f.FilterValue is not null).ToList().AsReadOnly()
            };

            var result = await CustomData!(options, token);
            token.ThrowIfCancellationRequested();

            _processedData = result.Items as IReadOnlyList<TItem> ?? result.Items.ToList().AsReadOnly();
            _serverTotalCount = result.TotalCount;
        }
        catch (OperationCanceledException)
        {
            return;
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                IsDataLoading = false;
                MarkDirty();
            }
        }
    }

    // --- Inline Editing ---

    public async Task StartEditAsync(TItem item)
    {
        // Cancel any current edit
        if (_editState.IsEditing)
            await CancelEditAsync();

        var args = new GridEditStartingEventArgs<TItem> { Item = item };
        await EditStarting.InvokeAsync(args);
        if (args.Cancel) return;

        // Clone item via JSON round-trip
        var json = JsonSerializer.Serialize(item);
        var copy = JsonSerializer.Deserialize<TItem>(json)!;

        _editState.EditingItem = item;
        _editState.EditingItemCopy = copy;
        _editState.IsNewRow = false;
        _editState.EditContext = new EditContext(copy!);

        await EditStarted.InvokeAsync(item);
        RequestRender();
    }

    public async Task StartInsertAsync()
    {
        if (_editState.IsEditing)
            await CancelEditAsync();

        var newItem = Activator.CreateInstance<TItem>();

        _editState.EditingItem = newItem;
        _editState.EditingItemCopy = newItem;
        _editState.IsNewRow = true;
        _editState.EditContext = new EditContext(newItem!);

        RequestRender();
    }

    public async Task SaveEditAsync()
    {
        if (!_editState.IsEditing || _editState.EditContext is null)
            return;

        var isValid = _editState.EditContext.Validate();
        if (!isValid) return;

        if (_editState.IsNewRow)
        {
            var insertArgs = new GridRowInsertingEventArgs<TItem> { NewItem = _editState.EditingItemCopy! };
            await RowInserting.InvokeAsync(insertArgs);
            if (insertArgs.Cancel) return;

            await RowInserted.InvokeAsync(_editState.EditingItemCopy!);
        }
        else
        {
            var updateArgs = new GridRowUpdatingEventArgs<TItem>
            {
                OldItem = _editState.EditingItem!,
                NewItem = _editState.EditingItemCopy!
            };
            await RowUpdating.InvokeAsync(updateArgs);
            if (updateArgs.Cancel) return;

            // Copy values from copy back to original
            CopyProperties(_editState.EditingItemCopy!, _editState.EditingItem!);
            await RowUpdated.InvokeAsync(_editState.EditingItem!);
        }

        _editState.Clear();
        RequestRender();
    }

    public async Task CancelEditAsync()
    {
        if (!_editState.IsEditing) return;

        var item = _editState.EditingItem;
        _editState.Clear();
        await EditCancelled.InvokeAsync(item!);
        RequestRender();
    }

    public bool IsEditing(TItem item) =>
        _editState.IsEditing && ReferenceEquals(_editState.EditingItem, item);

    private static void CopyProperties(TItem source, TItem target)
    {
        var properties = typeof(TItem).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (prop.CanRead && prop.CanWrite)
            {
                prop.SetValue(target, prop.GetValue(source));
            }
        }
    }

    // --- Disposal ---

    public async ValueTask DisposeAsync()
    {
        _scrollDebounceTimer?.Stop();
        _scrollDebounceTimer?.Dispose();
        _customDataCts?.Cancel();
        _customDataCts?.Dispose();
    }
}
