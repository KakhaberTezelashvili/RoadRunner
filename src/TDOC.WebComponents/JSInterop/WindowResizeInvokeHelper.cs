using Microsoft.JSInterop;
using TDOC.WebComponents.JSInterop.Models;

namespace TDOC.WebComponents.JSInterop;

public class WindowResizeInvokeHelper
{
    private readonly Action<BrowserDimensions> _action;

    public WindowResizeInvokeHelper(Action<BrowserDimensions> action)
    {
        _action = action;
    }

    [JSInvokable]
    public void AfterWindowResized(BrowserDimensions dimensions) => _action.Invoke(dimensions);
}