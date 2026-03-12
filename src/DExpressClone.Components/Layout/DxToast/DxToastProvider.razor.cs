using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Layout;

public partial class DxToastProvider : DxComponentBase, IDisposable
{
    [Inject] private DxToastService ToastService { get; set; } = default!;

    [Parameter] public ToastPosition Position { get; set; } = ToastPosition.TopRight;
    [Parameter] public int MaxToasts { get; set; } = 5;

    private readonly List<ToastInstance> _toasts = new();
    private readonly Dictionary<Guid, System.Threading.Timer> _timers = new();

    private string ContainerCssClass => CssClassBuilder.New("dx-toast-container")
        .AddIf("dx-toast-container--top-right", Position == ToastPosition.TopRight)
        .AddIf("dx-toast-container--top-left", Position == ToastPosition.TopLeft)
        .AddIf("dx-toast-container--bottom-right", Position == ToastPosition.BottomRight)
        .AddIf("dx-toast-container--bottom-left", Position == ToastPosition.BottomLeft)
        .AddIf("dx-toast-container--top-center", Position == ToastPosition.TopCenter)
        .AddIf("dx-toast-container--bottom-center", Position == ToastPosition.BottomCenter)
        .Build();

    protected override void OnInitialized()
    {
        ToastService.OnShow += HandleShow;
        ToastService.OnDismiss += HandleDismiss;
    }

    private void HandleShow(ToastInstance toast)
    {
        InvokeAsync(() =>
        {
            _toasts.Insert(0, toast);
            while (_toasts.Count > MaxToasts)
            {
                var removed = _toasts[^1];
                CleanupTimer(removed.Id);
                _toasts.RemoveAt(_toasts.Count - 1);
            }

            if (toast.TimeoutMs > 0)
            {
                var timer = new System.Threading.Timer(_ =>
                {
                    InvokeAsync(() => DismissToast(toast.Id));
                }, null, toast.TimeoutMs, System.Threading.Timeout.Infinite);
                _timers[toast.Id] = timer;
            }

            RequestRender();
        });
    }

    private void HandleDismiss(Guid toastId)
    {
        InvokeAsync(() => DismissToast(toastId));
    }

    private async Task DismissToast(Guid toastId)
    {
        var toast = _toasts.FirstOrDefault(t => t.Id == toastId);
        if (toast is null) return;

        toast.IsExiting = true;
        RequestRender();

        await Task.Delay(300); // exit animation duration

        _toasts.Remove(toast);
        CleanupTimer(toastId);
        RequestRender();
    }

    private void CleanupTimer(Guid toastId)
    {
        if (_timers.Remove(toastId, out var timer))
            timer.Dispose();
    }

    private string GetToastCssClass(ToastInstance toast) => CssClassBuilder.New("dx-toast")
        .AddIf("dx-toast--info", toast.Type == ToastType.Info)
        .AddIf("dx-toast--success", toast.Type == ToastType.Success)
        .AddIf("dx-toast--warning", toast.Type == ToastType.Warning)
        .AddIf("dx-toast--error", toast.Type == ToastType.Error)
        .AddIf("dx-toast--exiting", toast.IsExiting)
        .Build();

    private static string GetToastIcon(ToastType type) => type switch
    {
        ToastType.Success => "✓",
        ToastType.Warning => "⚠",
        ToastType.Error => "✕",
        _ => "ℹ"
    };

    public void Dispose()
    {
        ToastService.OnShow -= HandleShow;
        ToastService.OnDismiss -= HandleDismiss;
        foreach (var timer in _timers.Values)
            timer.Dispose();
        _timers.Clear();
    }
}
