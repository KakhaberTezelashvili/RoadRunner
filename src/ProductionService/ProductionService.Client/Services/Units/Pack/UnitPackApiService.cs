using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Pack;

/// <inheritdoc />
public class UnitPackApiService : IUnitPackApiService
{
    private const string urlPathBase = "units";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitPackApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public UnitPackApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<UnitPackDetailsDto> GetPackDetailsAsync(int unitKeyId) =>
        await _httpClient.GetAsync<UnitPackDetailsDto>($"{urlPathBase}/{unitKeyId}/pack-details");

    /// <inheritdoc />
    public async Task<IList<int>> PackAsync(UnitPackArgs args) =>
        await _httpClient.PostWithResultAsync<UnitPackArgs, IList<int>>($"{urlPathBase}/pack", args);
}