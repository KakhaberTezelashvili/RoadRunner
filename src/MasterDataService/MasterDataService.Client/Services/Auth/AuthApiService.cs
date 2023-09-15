using TDOC.Common.Data.Models.Auth;

namespace MasterDataService.Client.Services.Auth;

/// <inheritdoc />
public class AuthApiService : IAuthApiService
{
    private const string urlPathBase = "auth";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public AuthApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.MasterDataClient);
    }

    /// <inheritdoc />
    public async Task<LoginResult> LoginAsync(string userInitials)
    {
        var loginArgs = new LoginArgs
        {
            UserInitials = userInitials
        };
        return await _httpClient.PostWithResultAsync<LoginArgs, LoginResult>($"{urlPathBase}/login", loginArgs);
    }
}