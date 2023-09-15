using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Cancel;

/// <inheritdoc />
public class UnitCancelApiService : IUnitCancelApiService
{
    private const string urlPathBase = "units";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitCancelApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public UnitCancelApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task CancelAsync(UnitCancelArgs args) =>
        await _httpClient.PostWithoutResultAsync($"{urlPathBase}/cancel", args);
}