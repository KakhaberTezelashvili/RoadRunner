using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.Client.Services.Items.Recent;

/// <inheritdoc cref="IItemRecentApiService" />
public class ItemRecentApiService : IItemRecentApiService
{
    private const string urlPathBase = "items";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemRecentApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public ItemRecentApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.MasterDataClient);
    }

    /// <inheritdoc />
    public Task<ItemDetailsDto> GetRecentAsync() => _httpClient.GetAsync<ItemDetailsDto>($"{urlPathBase}/recent");
}