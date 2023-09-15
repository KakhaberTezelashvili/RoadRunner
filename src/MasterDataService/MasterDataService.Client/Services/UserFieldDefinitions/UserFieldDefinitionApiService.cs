namespace MasterDataService.Client.Services.UserFieldDefinitions;

/// <inheritdoc cref="IUserFieldDefinitionApiService" />
public class UserFieldDefinitionApiService : IUserFieldDefinitionApiService
{
    private const string urlPathBase = "userFieldDefinitions";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserFieldDefinitionApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public UserFieldDefinitionApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.MasterDataClient);
    }

    /// <inheritdoc />
    public async Task<IList<UserFieldDefModel>> GetUserFieldDefinitionsAsync(IList<string> tableNames) =>
        await _httpClient.GetAsync<IList<UserFieldDefModel>>($"{urlPathBase}?tables={string.Join(",", tableNames)}");
}