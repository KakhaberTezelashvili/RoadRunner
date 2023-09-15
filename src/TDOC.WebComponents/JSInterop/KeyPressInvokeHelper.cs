using Microsoft.JSInterop;

namespace TDOC.WebComponents.JSInterop
{
    public class KeyPressInvokeHelper
    {
        private readonly Action<string> _action;

        public KeyPressInvokeHelper(Action<string> action)
        {
            _action = action;
        }

        [JSInvokable]
        public void AfterKeyPressed(string key) => _action.Invoke(key);
    }
}
