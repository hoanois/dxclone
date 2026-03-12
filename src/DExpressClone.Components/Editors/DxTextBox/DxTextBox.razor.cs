using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DExpressClone.Components.Editors.DxTextBox;

/// <summary>
/// A text input editor with support for password mode, clear button, and max length.
/// </summary>
public partial class DxTextBox : DxFormEditorBase<string>
{
    /// <summary>
    /// Gets or sets the text value.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Callback invoked when the text changes.
    /// </summary>
    [Parameter]
    public EventCallback<string> TextChanged { get; set; }

    /// <summary>
    /// Whether to render as a password field.
    /// </summary>
    [Parameter]
    public bool Password { get; set; } = false;

    /// <summary>
    /// Maximum number of characters allowed.
    /// </summary>
    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Controls when the clear button is displayed.
    /// </summary>
    [Parameter]
    public ClearButtonDisplayMode ClearButtonDisplayMode { get; set; } = ClearButtonDisplayMode.Never;

    /// <summary>
    /// Basic input mask pattern.
    /// </summary>
    [Parameter]
    public string? InputMask { get; set; }

    private string InputType => Password ? "password" : "text";

    private bool ShowClearButton => ClearButtonDisplayMode switch
    {
        ClearButtonDisplayMode.Auto => !string.IsNullOrEmpty(Text) && Enabled && !ReadOnly,
        _ => false
    };

    private string RootCssClass => CssClassBuilder.New("dx-textbox")
        .Add(InteractiveStateCssClass)
        .Add(ValidationCssClass)
        .Add(CssClass)
        .Build();

    private async Task HandleInputAsync(ChangeEventArgs e)
    {
        var newValue = e.Value?.ToString() ?? string.Empty;
        Text = newValue;
        await TextChanged.InvokeAsync(newValue);
        await SetValueAsync(newValue);
    }

    private async Task HandleChangeAsync(ChangeEventArgs e)
    {
        var newValue = e.Value?.ToString() ?? string.Empty;
        Text = newValue;
        await TextChanged.InvokeAsync(newValue);
        await SetValueAsync(newValue);
    }

    private async Task HandleClearAsync()
    {
        if (!Enabled || ReadOnly) return;
        Text = string.Empty;
        await TextChanged.InvokeAsync(string.Empty);
        await SetValueAsync(string.Empty);
    }
}

/// <summary>
/// Controls when a clear button is displayed in a text editor.
/// </summary>
public enum ClearButtonDisplayMode
{
    /// <summary>Show the clear button automatically when text is present.</summary>
    Auto,
    /// <summary>Never show the clear button.</summary>
    Never
}
