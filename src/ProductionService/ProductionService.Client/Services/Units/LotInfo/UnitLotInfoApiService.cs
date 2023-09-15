using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.LotInfo;

/// <inheritdoc />
public class UnitLotInfoApiService : IUnitLotInfoApiService
{
    private const string urlPathBase = "units";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitLotInfoApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public UnitLotInfoApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task UpdateLotsAsync(UnitLotsArgs args) =>
        await _httpClient.PutAsync($"{urlPathBase}/lots", args);
}