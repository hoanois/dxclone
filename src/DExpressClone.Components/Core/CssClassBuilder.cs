namespace DExpressClone.Components.Core;

/// <summary>
/// A fluent string builder for composing CSS class strings.
/// </summary>
public sealed class CssClassBuilder
{
    private readonly List<string> _classes = new();

    private CssClassBuilder(string? baseClass)
    {
        if (!string.IsNullOrWhiteSpace(baseClass))
        {
            _classes.Add(baseClass.Trim());
        }
    }

    /// <summary>
    /// Creates a new <see cref="CssClassBuilder"/> with an optional base CSS class.
    /// </summary>
    public static CssClassBuilder New(string? baseClass = null) => new(baseClass);

    /// <summary>
    /// Adds a CSS class unconditionally.
    /// </summary>
    public CssClassBuilder Add(string? cssClass)
    {
        if (!string.IsNullOrWhiteSpace(cssClass))
        {
            _classes.Add(cssClass.Trim());
        }
        return this;
    }

    /// <summary>
    /// Adds a CSS class only when the condition is true.
    /// </summary>
    public CssClassBuilder AddIf(string cssClass, bool condition)
    {
        if (condition && !string.IsNullOrWhiteSpace(cssClass))
        {
            _classes.Add(cssClass.Trim());
        }
        return this;
    }

    /// <summary>
    /// Builds the final space-joined, trimmed CSS class string.
    /// </summary>
    public string Build() => string.Join(" ", _classes).Trim();
}
