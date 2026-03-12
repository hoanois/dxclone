using DExpressClone.Components.Grid.DataProcessing;
using DExpressClone.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DExpressClone.Components.Grid.Internal;

public partial class VirtualGridViewport<TItem> : ComponentBase, IAsyncDisposable
{
    private ElementReference _scrollContainer;
    private DotNetObjectReference<VirtualGridViewport<TItem>>? _dotNetRef;
    private bool _jsAttached;

    [Inject]
    private JsInteropService JsInterop { get; set; } = default!;

    [Parameter]
    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();

    [Parameter]
    public IReadOnlyList<DxGridColumnBase> Columns { get; set; } = Array.Empty<DxGridColumnBase>();

    [Parameter]
    public int RowHeight { get; set; } = 36;

    [Parameter]
    public string Height { get; set; } = "500px";

    [Parameter]
    public VirtualizationWindow? VirtualizationWindow { get; set; }

    [Parameter]
    public RenderFragment<TItem>? RowTemplate { get; set; }

    [Parameter]
    public EventCallback<ScrollEventArgs> OnScroll { get; set; }

    [Parameter]
    public EventCallback<TItem> OnRowClick { get; set; }

    [Parameter]
    public EventCallback<TItem> OnRowDoubleClick { get; set; }

    [Parameter]
    public EventCallback<TItem> OnSelectionToggled { get; set; }

    [Parameter]
    public Func<TItem, bool>? IsItemSelected { get; set; }

    [Parameter]
    public Func<TItem, bool>? IsItemFocused { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_jsAttached)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            try
            {
                await JsInterop.AttachScrollListenerAsync(_scrollContainer, _dotNetRef);
                _jsAttached = true;
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected, ignore
            }
        }
    }

    [JSInvokable]
    public async Task OnScrollAsync(double scrollTop, double viewportHeight)
    {
        await OnScroll.InvokeAsync(new ScrollEventArgs(scrollTop, viewportHeight));
    }

    private bool GetIsSelected(TItem item) => IsItemSelected?.Invoke(item) ?? false;
    private bool GetIsFocused(TItem item) => IsItemFocused?.Invoke(item) ?? false;

    public async ValueTask DisposeAsync()
    {
        if (_jsAttached)
        {
            try
            {
                await JsInterop.DetachScrollListenerAsync(_scrollContainer);
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected, ignore
            }
        }

        _dotNetRef?.Dispose();
    }
}

public record ScrollEventArgs(double ScrollTop, double ViewportHeight);
