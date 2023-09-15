using ProductionService.Shared.Dtos.Programs;

namespace ProductionService.Client.Services.Program;

/// <inheritdoc cref="IProgramApiService" />
public class ProgramApiService : IProgramApiService
{
    private const string urlPathBase = "programs";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public ProgramApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<IList<ProgramDetailsBaseDto>> GetProgramsByMachineAsync(int machineKeyId) =>
        await _httpClient.GetAsync<IList<ProgramDetailsBaseDto>>($"{urlPathBase}?machineId={machineKeyId}");
}