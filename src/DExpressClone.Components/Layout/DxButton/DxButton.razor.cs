using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DExpressClone.Components.Core;

namespace DExpressClone.Components.Layout;

public partial class DxButton : DxInteractiveComponentBase
{
    /// <summary>
    /// Gets or sets the button text.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the button icon.
    /// </summary>
    [Parameter]
    public string? IconCssClass { get; set; }

    /// <summary>
    /// Gets or sets the URL to navigate to when clicked.
    /// </summary>
    [Parameter]
    public string? NavigateUrl { get; set; }

    /// <summary>
    /// Gets or sets the visual style of the button.
    /// </summary>
    [Parameter]
    public ButtonRenderStyle RenderStyle { get; set; } = ButtonRenderStyle.Primary;

    /// <summary>
    /// Gets or sets the rendering mode of the button.
    /// </summary>
    [Parameter]
    public ButtonRenderStyleMode RenderStyleMode { get; set; } = ButtonRenderStyleMode.Contained;

    /// <summary>
    /// Gets or sets the click event callback.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> Click { get; set; }

    /// <summary>
    /// Gets or sets the child content of the button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the HTML button type attribute.
    /// </summary>
    [Parameter]
    public ButtonType ButtonType { get; set; } = ButtonType.Button;

    private string CssClassString => CssClassBuilder.New("dx-btn")
        .Add($"dx-btn--{RenderStyle.ToString().ToLowerInvariant()}")
        .AddIf("dx-btn--outlined", RenderStyleMode == ButtonRenderStyleMode.Outlined)
        .AddIf("dx-btn--text", RenderStyleMode == ButtonRenderStyleMode.Text)
        .AddIf("dx-btn--icon-only", !string.IsNullOrEmpty(IconCssClass) && string.IsNullOrEmpty(Text) && ChildContent == null)
        .Add(InteractiveStateCssClass)
        .Add(CssClass)
        .Build();

    private string ButtonTypeString => ButtonType switch
    {
        ButtonType.Submit => "submit",
        ButtonType.Reset => "reset",
        _ => "button"
    };

    private async Task HandleClick(MouseEventArgs args)
    {
        if (Enabled && Click.HasDelegate)
        {
            await Click.InvokeAsync(args);
        }
    }
}

/// <summary>
/// Specifies the HTML button type attribute.
/// </summary>
public enum ButtonType
{
    Button,
    Submit,
    Reset
}
