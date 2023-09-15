using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.Client.Services.Items;

/// <inheritdoc cref="IItemApiService" />
public class ItemApiService : IItemApiService
{
    private const string urlPathBase = "items";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public ItemApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.MasterDataClient);
    }

    /// <inheritdoc />
    public async Task<int> AddDataAsync(ItemAddArgs args) =>
        await _httpClient.PostWithResultAsync<ItemAddArgs, int>(urlPathBase, args);

    /// <inheritdoc />
    public async Task<ItemDetailsDto> GetByKeyIdAsync(int keyId) =>
        await _httpClient.GetAsync<ItemDetailsDto>($"{urlPathBase}/{keyId}");

    /// <inheritdoc />
    public async Task UpdateDataAsync(ItemUpdateArgs args) =>
        await _httpClient.PutAsync($"{urlPathBase}/{args.KeyId}", args);
}