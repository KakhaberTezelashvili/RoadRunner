using Newtonsoft.Json.Linq;

namespace SearchService.Client.Services;

/// <inheritdoc cref="ISearchApiService" />
public class SearchApiService : ISearchApiService
{
    private const string urlPathBase = "search";

    private readonly ITypedHttpClient _httpSearchClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public SearchApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpSearchClient = httpClientFactory.CreateClient(HttpClientConfigurationName.SearchClient);
    }

    /// <inheritdoc />
    public async Task<SelectDataResult<JObject>> SearchAsync(SearchArgs args) =>
        await _httpSearchClient.PostWithResultAsync<SearchArgs, SearchDataResultDto<JObject>>($"{urlPathBase}/search", args, true);

    /// <inheritdoc />
    public async Task<SelectDataResult<JObject>> SelectAsync(SelectArgs args) =>
        await _httpSearchClient.PostWithResultAsync<SelectArgs, SelectDataResultDto<JObject>>($"{urlPathBase}/select", args, true);
}