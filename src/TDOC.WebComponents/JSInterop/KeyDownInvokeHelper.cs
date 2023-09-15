using Microsoft.JSInterop;
using TDOC.WebComponents.JSInterop.Models;

namespace TDOC.WebComponents.JSInterop;

/// <summary>
/// Used for keydown event subscription in JS via <see cref="BrowserService" />.
/// </summary>
public class KeyDownInvokeHelper
{
    private readonly Action<KeyboardEvent> _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyDownInvokeHelper" /> class.
    /// </summary>
    /// <param name="action">Delegate that is called after key down event.</param>
    public KeyDownInvokeHelper(Action<KeyboardEvent> action)
    {
        _action = action;
    }

    /// <summary>
    /// Invokes key down event.
    /// </summary>
    /// <param name="keyboardEvent"><see cref="KeyboardEvent" /></param>
    [JSInvokable]
    public void AfterKeyDown(KeyboardEvent keyboardEvent) => _action.Invoke(keyboardEvent);
}