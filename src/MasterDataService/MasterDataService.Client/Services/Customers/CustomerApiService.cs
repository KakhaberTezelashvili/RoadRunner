using MasterDataService.Shared.Dtos.Customers;

namespace MasterDataService.Client.Services.Customers;

/// <inheritdoc />
public class CustomerApiService : ICustomerApiService
{
    private const string urlPathBase = "customers";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public CustomerApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.MasterDataClient);
    }

    /// <inheritdoc />
    public Task<CustomerDetailsDto> GetByKeyIdAsync(int keyId) => _httpClient.GetAsync<CustomerDetailsDto>($"{urlPathBase}/{keyId}");

    /// <inheritdoc />
    public Task<IList<CustomerModel>> GetByUserKeyIdOrFactoryKeyIdOrAllAsync(int? userKeyId, int? factoryKeyId)
    {
        string urlParameters = "";

        if (userKeyId != null && factoryKeyId != null)
            urlParameters = $"?userId={userKeyId}&factoryId={factoryKeyId}";
        else if (userKeyId != null)
            urlParameters = $"?userId={userKeyId}";
        else if (factoryKeyId != null)
            urlParameters = $"?factoryId={factoryKeyId}";

        return _httpClient.GetAsync<IList<CustomerModel>>($"{urlPathBase}{urlParameters}");
    }
}