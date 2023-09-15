using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Patients;

/// <inheritdoc />
public class UnitPatientApiService : IUnitPatientApiService
{
    private const string urlPathBase = "units";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitPatientApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public UnitPatientApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task UpdatePatientAsync(int unitKeyId, UnitPatientArgs args) =>
         await _httpClient.PostWithoutResultAsync($"{urlPathBase}/{unitKeyId}/patients", args);
}