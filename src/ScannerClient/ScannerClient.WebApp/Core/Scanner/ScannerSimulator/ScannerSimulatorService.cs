using Microsoft.JSInterop;

namespace ScannerClient.WebApp.Core.Scanner.ScannerSimulator
{
    public class ScannerSimulatorService
    {
        private readonly IJSRuntime _jsRuntime;

        public ScannerSimulatorService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public virtual async Task InitScannerSimulator(ScannerSimulatorInvokeHelper invokeHelper) =>
            await _jsRuntime.InvokeVoidAsync("scannerSimulator.initScannerSimulator", DotNetObjectReference.Create(invokeHelper));

        public virtual void DisposeScannerSimulator() => 
            (_jsRuntime as IJSInProcessRuntime).InvokeVoid("scannerSimulator.disposeScannerSimulator");
    }
}