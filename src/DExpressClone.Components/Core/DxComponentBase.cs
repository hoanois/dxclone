using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Core;

/// <summary>
/// Root base class for all DExpressClone components. Provides dirty-tracking rendering,
/// theme cascading, and common parameter support.
/// </summary>
public abstract class DxComponentBase : ComponentBase
{
    private bool _isDirty = true;

    /// <summary>
    /// The cascaded theme context, if provided by an ancestor.
    /// </summary>
    [CascadingParameter]
    public DxThemeContext? ThemeContext { get; set; }

    /// <summary>
    /// Gets or sets additional HTML attributes to render on the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the root element.
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute for the root element.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Marks the component as needing a re-render on the next render cycle.
    /// </summary>
    protected void MarkDirty()
    {
        _isDirty = true;
    }

    /// <summary>
    /// Marks the component dirty and requests a re-render from Blazor.
    /// </summary>
    protected void RequestRender()
    {
        MarkDirty();
        StateHasChanged();
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        if (_isDirty)
        {
            _isDirty = false;
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        MarkDirty();
    }

    /// <summary>
    /// Builds a composite CSS class string using the given component class as the base,
    /// appending the user-supplied <see cref="CssClass"/> parameter.
    /// </summary>
    protected string BuildCssClass(string componentClass)
    {
        return CssClassBuilder.New(componentClass)
            .Add(CssClass)
            .Build();
    }
}
