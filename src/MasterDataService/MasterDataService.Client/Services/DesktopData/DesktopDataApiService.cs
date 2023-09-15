namespace MasterDataService.Client.Services.DesktopData;

/// <inheritdoc />
public class DesktopDataApiService : IDesktopDataApiService
{
    private const string urlPathBase = "desktopData";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopDataApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public DesktopDataApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.MasterDataClient);
    }

    /// <inheritdoc />
    public async Task<string> GetComponentStateAsync(string identifier) =>
        await _httpClient.GetStringAsync($"{urlPathBase}/component-state?identifier={identifier}");

    /// <inheritdoc />
    public async Task SetComponentStateAsync(string identifier, string data) =>
        await _httpClient.PostWithoutResultAsync($"{urlPathBase}/component-state?identifier={identifier}", data);
}