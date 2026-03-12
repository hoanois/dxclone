using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Core;

/// <summary>
/// Base class for interactive components that support focus, hover, enabled/disabled,
/// and read-only states.
/// </summary>
public abstract class DxInteractiveComponentBase : DxComponentBase
{
    /// <summary>
    /// Gets or sets whether the component is enabled. Default is true.
    /// </summary>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the component is read-only. Default is false.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; } = false;

    /// <summary>
    /// Gets or sets the tab index for keyboard navigation.
    /// </summary>
    [Parameter]
    public int? TabIndex { get; set; }

    /// <summary>
    /// Gets whether the component currently has focus.
    /// </summary>
    protected bool IsFocused { get; private set; }

    /// <summary>
    /// Gets whether the mouse is currently hovering over the component.
    /// </summary>
    protected bool IsHovered { get; private set; }

    private string? _cachedInteractiveStateCss;
    private bool _cachedFocused;
    private bool _cachedHovered;
    private bool _cachedEnabled = true;
    private bool _cachedReadOnly;

    /// <summary>
    /// Handles the focus event on the component.
    /// </summary>
    protected Task HandleFocusAsync()
    {
        IsFocused = true;
        RequestRender();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the blur (focus-lost) event on the component.
    /// </summary>
    protected virtual Task HandleBlurAsync()
    {
        IsFocused = false;
        RequestRender();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the mouse enter event on the component.
    /// </summary>
    protected void HandleMouseEnter()
    {
        IsHovered = true;
        RequestRender();
    }

    /// <summary>
    /// Handles the mouse leave event on the component.
    /// </summary>
    protected void HandleMouseLeave()
    {
        IsHovered = false;
        RequestRender();
    }

    /// <summary>
    /// Gets the CSS classes representing the current interactive state of the component.
    /// </summary>
    protected string InteractiveStateCssClass
    {
        get
        {
            if (_cachedInteractiveStateCss is not null
                && _cachedFocused == IsFocused
                && _cachedHovered == IsHovered
                && _cachedEnabled == Enabled
                && _cachedReadOnly == ReadOnly)
                return _cachedInteractiveStateCss;

            _cachedFocused = IsFocused;
            _cachedHovered = IsHovered;
            _cachedEnabled = Enabled;
            _cachedReadOnly = ReadOnly;
            _cachedInteractiveStateCss = CssClassBuilder.New()
                .AddIf("dx-state-focused", IsFocused)
                .AddIf("dx-state-hovered", IsHovered && Enabled)
                .AddIf("dx-state-disabled", !Enabled)
                .AddIf("dx-state-readonly", ReadOnly)
                .Build();
            return _cachedInteractiveStateCss;
        }
    }
}
