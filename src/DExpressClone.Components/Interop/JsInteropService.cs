using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DExpressClone.Components.Interop;

/// <summary>
/// Provides managed access to the DExpressClone JavaScript interop module.
/// </summary>
public sealed class JsInteropService : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public JsInteropService(IJSRuntime jsRuntime)
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/DExpressClone.Components/js/dx-interop.js").AsTask());
    }

    private async Task<IJSObjectReference> GetModuleAsync() => await _moduleTask.Value;

    /// <summary>
    /// Attaches a scroll listener to the specified element.
    /// </summary>
    public async ValueTask AttachScrollListenerAsync<T>(ElementReference element, DotNetObjectReference<T> dotNetRef) where T : class
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("attachScrollListener", element, dotNetRef);
    }

    /// <summary>
    /// Detaches a previously attached scroll listener from the specified element.
    /// </summary>
    public async ValueTask DetachScrollListenerAsync(ElementReference element)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("detachScrollListener", element);
    }

    /// <summary>
    /// Gets the bounding client rectangle and viewport dimensions for the specified element.
    /// </summary>
    public async ValueTask<BoundingRect> GetBoundingRectAsync(ElementReference element)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<BoundingRect>("getBoundingRect", element);
    }

    /// <summary>
    /// Sets focus to the specified element.
    /// </summary>
    public async ValueTask FocusElementAsync(ElementReference element)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("focusElement", element);
    }

    /// <summary>
    /// Traps focus within the specified element (for modal dialogs).
    /// </summary>
    public async ValueTask TrapFocusAsync(ElementReference element)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("trapFocus", element);
    }

    /// <summary>
    /// Releases the focus trap from the specified element.
    /// </summary>
    public async ValueTask ReleaseFocusTrapAsync(ElementReference element)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("releaseFocusTrap", element);
    }

    /// <summary>
    /// Locks body scroll (prevents scrolling behind modal).
    /// </summary>
    public async ValueTask LockBodyScrollAsync()
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("lockBodyScroll");
    }

    /// <summary>
    /// Unlocks body scroll (restores scrolling).
    /// </summary>
    public async ValueTask UnlockBodyScrollAsync()
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("unlockBodyScroll");
    }

    /// <summary>
    /// Checks if a dropdown should open upward based on available viewport space.
    /// </summary>
    public async ValueTask<bool> ShouldDropUpAsync(ElementReference element, int dropdownHeight = 300)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("shouldDropUp", element, dropdownHeight);
    }

    /// <summary>
    /// Registers a click-outside listener for the specified element.
    /// </summary>
    public async ValueTask AddClickOutsideListenerAsync<T>(ElementReference element, DotNetObjectReference<T> dotNetRef) where T : class
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("addClickOutsideListener", element, dotNetRef);
    }

    /// <summary>
    /// Removes a click-outside listener for the specified element.
    /// </summary>
    public async ValueTask RemoveClickOutsideListenerAsync(ElementReference element)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("removeClickOutsideListener", element);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}

/// <summary>
/// Represents the bounding rectangle and viewport dimensions of an element.
/// </summary>
public record class BoundingRect
{
    public double Top { get; init; }
    public double Left { get; init; }
    public double Right { get; init; }
    public double Bottom { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    public double ViewportHeight { get; init; }
    public double ViewportWidth { get; init; }
}
