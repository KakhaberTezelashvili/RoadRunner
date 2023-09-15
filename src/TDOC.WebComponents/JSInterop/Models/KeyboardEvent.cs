using TDOC.WebComponents.JSInterop.Models.Constants;

namespace TDOC.WebComponents.JSInterop.Models;

/// <summary>
/// Describes a single interaction between the user and a key (or combination of a keys) on the keyboard.
/// </summary>
public record KeyboardEvent
{
    /// <summary>
    /// Indicates if Alt key was used.
    /// </summary>
    public bool AltKey { get; set; }

    /// <summary>
    /// Indicates if Ctrl key was used.
    /// </summary>
    public bool CtrlKey { get; set; }

    /// <summary>
    /// Indicates if Shift key was used.
    /// </summary>
    public bool ShiftKey { get; set; }

    /// <summary>
    /// Keyboard key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Keyboard key code of the key that was used.
    /// </summary>
    public int KeyCode { get; set; }

    /// <summary>
    /// Indicates if combinations of used keys is shortcut.
    /// </summary>
    public bool IsShortcut => !string.IsNullOrWhiteSpace(Shortcut);

    /// <summary>
    /// Returns <see cref="Shortcuts"/>, If shortcut match found.
    /// </summary>
    public string Shortcut
    {
        get
        {
            var combination = (CtrlKey ? $"{Shortcuts.Ctrl}+" : string.Empty) + KeyCode;

            switch (combination)
            {
                case Shortcuts.New:
                    return Shortcuts.New;
                case Shortcuts.Save:
                    return Shortcuts.Save;
                case Shortcuts.Esc:
                    return Shortcuts.Esc;
                default:
                    return "";
            }
        }
    }
}