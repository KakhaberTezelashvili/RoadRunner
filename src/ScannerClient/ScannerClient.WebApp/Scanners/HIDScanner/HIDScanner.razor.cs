using ScannerClient.WebApp.Core.Scanner.Models;

namespace ScannerClient.WebApp.Scanners.HIDScanner;

[Authorize]
[Route($"/{ScannerUrls.HID}")]
public partial class HIDScanner
{
    private readonly List<BarcodeData> HIDBarcodes = new();

    private void AddHIDBarcode(BarcodeData barcodeData)
    {
        HIDBarcodes.Add(barcodeData);
        StateHasChanged();
    }
}