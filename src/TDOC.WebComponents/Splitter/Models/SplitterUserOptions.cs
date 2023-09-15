using TDOC.WebComponents.Shared.Enumerations;

namespace TDOC.WebComponents.Splitter.Models;

/// <summary>
/// Splitter user options.
/// </summary>
public class SplitterUserOptions
{
    /// <summary>
    /// Width of panel.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Whereabouts of panel is indication to keep left or right panel width.
    /// </summary>
    public PanelWhereabouts Whereabouts { get; set; }
}