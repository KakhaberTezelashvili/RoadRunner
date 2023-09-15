// TODO: Move SignalR functionality together with this code into microservice "NotificationService".
/*using Microsoft.AspNetCore.SignalR;
using TDOC.Common.Server.Auth.Extensions;

namespace TDOC.Common.Server.Hubs;

/// <summary>
/// Maps user identity to field of user we use in SignalR hubs.
/// </summary>
public class HubBaseUserIdProvider : IUserIdProvider
{
    /// <summary>
    /// Get user identifier in string type.
    /// </summary>
    public string GetUserId(HubConnectionContext connection) => connection.User.GetUserKeyId().ToString();
}*/