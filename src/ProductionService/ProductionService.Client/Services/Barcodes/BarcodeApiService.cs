using ProductionService.Shared.Dtos.Scanner;

namespace ProductionService.Client.Services.Barcodes;

/// <inheritdoc cref="IBarcodeApiService" />
public class BarcodeApiService : IBarcodeApiService
{
    private const string urlPathBase = "barcodes";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public BarcodeApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<BarcodeDto> GetBarcodeDataAsync(string barcode) => 
        await _httpClient.GetAsync<BarcodeDto>($"{urlPathBase}/{barcode}/details");
}