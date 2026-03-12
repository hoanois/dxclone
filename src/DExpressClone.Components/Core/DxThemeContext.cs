namespace DExpressClone.Components.Core;

/// <summary>
/// Holds the current theme configuration and can be cascaded to child components.
/// </summary>
public class DxThemeContext
{
    /// <summary>
    /// Gets or sets the active theme name (e.g., "fluent", "material").
    /// </summary>
    public string ThemeName { get; set; } = "fluent";

    /// <summary>
    /// Gets or sets whether dark mode is active.
    /// </summary>
    public bool IsDarkMode { get; set; } = false;
}
