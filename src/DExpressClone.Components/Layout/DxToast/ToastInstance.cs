namespace DExpressClone.Components.Layout;

/// <summary>
/// Represents a single toast notification instance.
/// </summary>
public class ToastInstance
{
    /// <summary>Unique identifier for this toast.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>The toast message text.</summary>
    public string Message { get; set; } = "";

    /// <summary>Optional title displayed above the message.</summary>
    public string? Title { get; set; }

    /// <summary>The visual type of the toast.</summary>
    public ToastType Type { get; set; } = ToastType.Info;

    /// <summary>Auto-dismiss timeout in milliseconds. 0 = no auto-dismiss.</summary>
    public int TimeoutMs { get; set; } = 5000;

    /// <summary>Whether this toast is currently in exit animation.</summary>
    internal bool IsExiting { get; set; }
}
