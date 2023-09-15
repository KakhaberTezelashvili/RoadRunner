using Microsoft.JSInterop;

namespace TDOC.WebComponents.Splitter.Models;

public class SplitterResizeInvokeHelper
{
    private readonly Action<int> _action;

    public SplitterResizeInvokeHelper(Action<int> action)
    {
        _action = action;
    }

    [JSInvokable]
    public void AfterResized(int width) => _action.Invoke(width);
}