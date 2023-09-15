using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Dispatch;

/// <inheritdoc />
public class UnitDispatchApiService : IUnitDispatchApiService
{
    private const string urlPathBase = "units";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitDispatchApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public UnitDispatchApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task DispatchAsync(UnitDispatchArgs args) =>
        await _httpClient.PostWithResultAsync<UnitDispatchArgs, int>($"{urlPathBase}/dispatch", args);
}