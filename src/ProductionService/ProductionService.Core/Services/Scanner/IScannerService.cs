using ProductionService.Shared.Dtos.Scanner;

namespace ProductionService.Core.Services.Scanner;

/// <summary>
/// Service provides methods to retrieve/handle scanner data.
/// </summary>
public interface IScannerService
{
    /// <summary>
    /// Retrieves barcode data.
    /// </summary>
    /// <param name="barcode">Scanned barcode.</param>
    /// <returns>Barcode data.</returns>
    BarcodeDto GetBarcodeData(string barcode);
}