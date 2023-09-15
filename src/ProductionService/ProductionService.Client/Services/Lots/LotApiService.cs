using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.Client.Services.Lots;

/// <inheritdoc cref="ILotApiService" />
public class LotApiService : ILotApiService
{
    private const string urlPathBase = "lots";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="LotApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public LotApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<LotInformationDto> GetUnitLotInformationAsync(int unitKeyId) => 
        await _httpClient.GetAsync<LotInformationDto>($"{urlPathBase}?unitId={unitKeyId}");
}