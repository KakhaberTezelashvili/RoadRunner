// TODO: Move SignalR functionality together with this code into microservice "NotificationService".
/*using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using TDOC.Common.Client.Http;
using TDOC.Common.Data.Enumerations.Hubs;
using TDOC.Common.Data.Enumerations.Hubs.Commands;

namespace TDOC.Common.Client.Hubs;

/// <summary>
/// Connection to realtime hub that notifies progress of tasks.
/// </summary>
public class HubProgressConnection : HubBaseConnection
{
    public HubProgressConnection(ITypedHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
    {
    }

    public void SubscribeToNotification(Action<int> action)
    {
        Instance.On<int>(nameof(HubProgressCommand.Notify), (progress) =>
        {
            action.Invoke(progress);
        });
    }

    protected override string HubName() => nameof(HubNames.Progress);

    protected override string BaseAddress() => "ApiEndpoints:ProductionServiceHubUrl";
}*/