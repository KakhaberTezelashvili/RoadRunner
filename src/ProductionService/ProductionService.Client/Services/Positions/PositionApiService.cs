using ProductionService.Shared.Dtos.Positions;

namespace ProductionService.Client.Services.Positions;

/// <inheritdoc cref="IPositionApiService" />
public class PositionApiService : IPositionApiService
{
    private const string urlPathBase = "positions";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public PositionApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<List<PositionLocationsDetailsDto>> GetWorkflowsByPositionKeyIdAsync(int positionKeyId) =>
        await _httpClient.GetAsync<List<PositionLocationsDetailsDto>>($"{urlPathBase}/{positionKeyId}/scanner-locations");
}