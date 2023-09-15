namespace TDOC.WebComponents.Search.Models;

/// <summary>
/// SideSearchPanel user options.
/// </summary>
public class SideSearchPanelUserOptions
{
    /// <summary>
    /// Flag indicating search panel is expanded or collapsed.
    /// </summary>
    public bool Expand { get; set; } = true;

    /// <summary>
    /// Index of active tab.
    /// </summary>
    public int ActiveTabIndex { get; set; }
}