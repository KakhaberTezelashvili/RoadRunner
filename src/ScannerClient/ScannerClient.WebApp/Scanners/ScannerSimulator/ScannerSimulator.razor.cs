using ScannerClient.WebApp.Core.Scanner.Models;
using ScannerClient.WebApp.Core.Scanner.ScannerReader;

namespace ScannerClient.WebApp.Scanners.ScannerSimulator
{
    public partial class ScannerSimulator : IDisposable
    {
        [Inject]
        private IScannerReader _scannerReader { get; set; }

        [Inject]
        private IMediator _mediator { get; set; }

        [Parameter]
        public Func<BarcodeData, Task> AfterProcessBarcode { get; set; }

        private string barcodeValue;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _scannerReader.StartListenScannerAsync(
                    true, GetBarcodeValue, ClearBarcodeValue, AfterProcessBarcode ?? HandleScannedBarcodeAsync);
            }
        }

        public void Dispose() => _scannerReader.StopListenScanner();

        private string GetBarcodeValue() => barcodeValue;

        private void ClearBarcodeValue()
        {
            barcodeValue = "";
            StateHasChanged();
        }

        private async Task HandleScannedBarcodeAsync(BarcodeData data)
        {
            await _mediator.Publish(new HideToastNotification());
            await _mediator.Publish(new BarcodeDataNotification(data));
        }
    }
}