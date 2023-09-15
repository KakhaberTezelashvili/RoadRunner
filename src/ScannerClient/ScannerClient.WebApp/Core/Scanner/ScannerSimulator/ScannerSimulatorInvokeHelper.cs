using Microsoft.JSInterop;

namespace ScannerClient.WebApp.Core.Scanner.ScannerSimulator
{
    public class ScannerSimulatorInvokeHelper
    {
        private readonly Func<string, Task> _action;

        public ScannerSimulatorInvokeHelper(Func<string, Task> action)
        {
            _action = action;
        }

        [JSInvokable]
        public void ScanBarcode(string barcode) => _action.Invoke(barcode);
    }
}
