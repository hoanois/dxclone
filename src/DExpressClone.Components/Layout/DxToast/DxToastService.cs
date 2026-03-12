namespace DExpressClone.Components.Layout;

/// <summary>
/// Service for showing toast notifications. Register as scoped.
/// </summary>
public class DxToastService
{
    /// <summary>Fires when a new toast should be displayed.</summary>
    public event Action<ToastInstance>? OnShow;

    /// <summary>Fires when a toast should be dismissed.</summary>
    public event Action<Guid>? OnDismiss;

    public void Show(string message, ToastType type = ToastType.Info, int timeoutMs = 5000, string? title = null)
    {
        var toast = new ToastInstance
        {
            Id = Guid.NewGuid(),
            Message = message,
            Title = title,
            Type = type,
            TimeoutMs = timeoutMs
        };
        OnShow?.Invoke(toast);
    }

    public void ShowSuccess(string message, string? title = null) =>
        Show(message, ToastType.Success, 5000, title);

    public void ShowError(string message, string? title = null) =>
        Show(message, ToastType.Error, 0, title);

    public void ShowWarning(string message, string? title = null) =>
        Show(message, ToastType.Warning, 5000, title);

    public void ShowInfo(string message, string? title = null) =>
        Show(message, ToastType.Info, 5000, title);

    public void Dismiss(Guid toastId) => OnDismiss?.Invoke(toastId);
}
