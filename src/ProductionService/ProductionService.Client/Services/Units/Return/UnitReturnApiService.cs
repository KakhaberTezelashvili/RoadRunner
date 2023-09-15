using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Return;

/// <inheritdoc />
public class UnitReturnApiService : IUnitReturnApiService
{
    private const string urlPathBase = "units";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitReturnApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public UnitReturnApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<UnitReturnDetailsDto> GetReturnDetailsAsync(int unitKeyId) =>
        await _httpClient.GetAsync<UnitReturnDetailsDto>($"{urlPathBase}/{unitKeyId}/return-details");

    /// <inheritdoc />
    public async Task<int> ReturnAsync(UnitReturnArgs args) =>
        await _httpClient.PostWithResultAsync<UnitReturnArgs, int>($"{urlPathBase}/return", args);
}