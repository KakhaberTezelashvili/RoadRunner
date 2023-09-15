using TDOC.WebComponents.JSInterop.Models.Constants;

namespace TDOC.WebComponents.JSInterop.Models;

/// <summary>
/// General <see cref="MediatR"/> data notification containing shortcut details.
/// </summary>
public class ShortcutNotification : INotification
{
    /// <summary>
    /// Shortcut combination from <see cref="Shortcuts"/> .
    /// </summary>
    public string Shortcut { get; set; }
}