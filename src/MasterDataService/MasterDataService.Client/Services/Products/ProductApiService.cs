using MasterDataService.Shared.Dtos.Products;

namespace MasterDataService.Client.Services.Products;

/// <inheritdoc cref="IProductApiService" />
public class ProductApiService : IProductApiService
{
    private const string urlPathBase = "products";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public ProductApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.MasterDataClient);
    }

    /// <inheritdoc />
    public Task<ProductDetailsDto> GetByKeyIdAsync(int keyId) => _httpClient.GetAsync<ProductDetailsDto>($"{urlPathBase}/{keyId}");    
}