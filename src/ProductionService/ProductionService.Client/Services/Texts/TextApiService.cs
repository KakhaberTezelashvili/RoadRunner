using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.Client.Services.Text;

/// <inheritdoc cref="ITextApiService" />
public class TextApiService : ITextApiService
{
    private const string urlPathBase = "texts";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public TextApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<IList<ErrorCodeDetailsDto>> GetErrorCodesAsync() =>
        await _httpClient.GetAsync<IList<ErrorCodeDetailsDto>>($"{urlPathBase}/error-codes");
}