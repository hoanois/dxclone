namespace DExpressClone.Components.Layout;

/// <summary>
/// Specifies how tab content is rendered.
/// </summary>
public enum TabRenderMode
{
    /// <summary>
    /// All tab pages are rendered; inactive ones are hidden via CSS.
    /// </summary>
    AllTabs,

    /// <summary>
    /// Only the active tab page is rendered.
    /// </summary>
    LazyTabs
}
