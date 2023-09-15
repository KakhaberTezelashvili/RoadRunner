using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.Client.Services.Machines;

/// <inheritdoc cref="IMachineApiService" />
public class MachineApiService : IMachineApiService
{
    private const string urlPathBase = "machines";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MachineApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public MachineApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<IList<MachineDetailsBaseDto>> GetMachinesByLocationAsync(int locationKeyId, MachineType machineType) =>
        await _httpClient.GetAsync<IList<MachineDetailsBaseDto>>($"{urlPathBase}?locationId={locationKeyId}&machineType={(int)machineType}");
}