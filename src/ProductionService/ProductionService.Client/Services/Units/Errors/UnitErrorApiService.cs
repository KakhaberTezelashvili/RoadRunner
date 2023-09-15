using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Errors;

/// <inheritdoc />
public class UnitErrorApiService : IUnitErrorApiService
{
    private const string urlPathBase = "units";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitErrorApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public UnitErrorApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task UpdateErrorAsync(int unitKeyId, int errorNumber)
    {
        UnitErrorArgs args = new() { ErrorNumber = errorNumber };
        await _httpClient.PutAsync($"{urlPathBase}/{unitKeyId}/errors", args);
    }
       
}