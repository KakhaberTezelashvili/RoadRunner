// TODO: Move SignalR functionality together with this code into microservice "NotificationService".
/*using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using TDOC.Common.Client.Http;
using TDOC.Common.Data.Constants.Hubs;

namespace TDOC.Common.Client.Hubs;

/// <summary>
/// Base class for connecting to realtime hub.
/// </summary>
public class HubBaseConnection
{
    private readonly ITypedHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private static HubConnection _instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="HubBaseConnection" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    /// <param name="configuration">Configuration to retrieve values from appsetting.</param>
    public HubBaseConnection(ITypedHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task StartAsync()
    {
        if (Instance.State != HubConnectionState.Connected)
            await Instance.StartAsync();
    }

    public async Task StopAsync()
    {
        if (Instance.State == HubConnectionState.Connected)
            await Instance.StopAsync();
    }

    public bool IsConnected => Instance.State == HubConnectionState.Connected;

    protected virtual string HubName() => "";

    protected virtual string BaseAddress() => "";

    protected HubConnection Instance => _instance ??= CreateInstance();

    private HubConnection CreateInstance()
    {
        string url = $"{_configuration.GetSection(BaseAddress()).Value}{HubName()}";
        return new HubConnectionBuilder()
            .WithUrl($"{url}?{HubQueryParameters.AccessToken}={_httpClientFactory.AuthToken}")
            .WithAutomaticReconnect()
            .Build();
    }
}*/