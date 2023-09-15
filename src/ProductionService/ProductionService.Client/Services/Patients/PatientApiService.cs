using ProductionService.Shared.Dtos.Patients;

namespace ProductionService.Client.Services.Patients;

/// <inheritdoc cref="IPatientApiService" />
public class PatientApiService : IPatientApiService
{
    private const string urlPathBase = "patients";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public PatientApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<IList<PatientDetailsBaseDto>> GetPatientsBasicInfoAsync() =>
        await _httpClient.GetAsync<IList<PatientDetailsBaseDto>>($"{urlPathBase}/basic-info");
}