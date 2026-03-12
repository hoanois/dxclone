using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Core;

/// <summary>
/// Base class for components that bind to a collection of data items.
/// Supports synchronous data via <see cref="Data"/> or asynchronous loading via <see cref="DataFactory"/>.
/// </summary>
/// <typeparam name="TItem">The type of data item.</typeparam>
public abstract class DxDataBoundComponentBase<TItem> : DxComponentBase
{
    private CancellationTokenSource? _loadCts;

    /// <summary>
    /// Gets or sets the synchronous data source.
    /// </summary>
    [Parameter]
    public IEnumerable<TItem>? Data { get; set; }

    /// <summary>
    /// Gets or sets an asynchronous factory for loading data.
    /// </summary>
    [Parameter]
    public Func<CancellationToken, ValueTask<IEnumerable<TItem>>>? DataFactory { get; set; }

    /// <summary>
    /// Gets the loaded data as a read-only list.
    /// </summary>
    protected IReadOnlyList<TItem> InternalData { get; private set; } = Array.Empty<TItem>();

    /// <summary>
    /// Gets whether data is currently being loaded.
    /// </summary>
    protected bool IsDataLoading { get; set; }

    /// <summary>
    /// When true, suppresses automatic data loading in OnParametersSetAsync.
    /// </summary>
    protected bool SuppressAutoLoad { get; set; }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!SuppressAutoLoad)
            await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        // Cancel any previous load operation
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();
        var token = _loadCts.Token;

        try
        {
            if (DataFactory is not null)
            {
                IsDataLoading = true;
                MarkDirty();

                var result = await DataFactory(token);
                token.ThrowIfCancellationRequested();

                InternalData = result as IReadOnlyList<TItem> ?? result.ToList().AsReadOnly();
            }
            else if (Data is not null)
            {
                InternalData = Data as IReadOnlyList<TItem> ?? Data.ToList().AsReadOnly();
            }
            else
            {
                InternalData = Array.Empty<TItem>();
            }
        }
        catch (OperationCanceledException)
        {
            // Load was cancelled; do not update state.
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
}
