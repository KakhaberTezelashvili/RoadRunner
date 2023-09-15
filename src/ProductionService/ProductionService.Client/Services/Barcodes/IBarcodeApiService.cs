using ProductionService.Shared.Dtos.Scanner;

namespace ProductionService.Client.Services.Barcodes;

/// <summary>
/// API service provides methods to retrieve barcodes.
/// </summary>
public interface IBarcodeApiService
{
    /// <summary>
    /// Retrieves barcode details: type and value.
    /// </summary>
    /// <param name="barcode">Barcode that keeps type and value.</param>
    /// <returns>Barcode details: type and value.</returns>
    Task<BarcodeDto> GetBarcodeDataAsync(string barcode);
}