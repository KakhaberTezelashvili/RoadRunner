namespace TDOC.Common.Client.Auth.Services;

/// <summary>
/// Provides methods for handling authentication actions.
/// </summary>
public interface IAuthActionService
{
    /// <summary>
    /// Force user to be login.
    /// </summary>
    /// <param name="loginUrl">Login url.</param>
    Task ForceUserToBeLoginAsync(string loginUrl);

    /// <summary>
    /// Login user.
    /// </summary>
    /// <param name="authToken">Authentication token.</param>
    /// <param name="defaultUrl">Default return url.</param>
    /// <param name="userName">User name.</param>
    Task LoginUserAsync(string authToken, string defaultUrl, string userName);

    /// <summary>
    /// Logout user.
    /// </summary>
    /// <param name="loginUrl">Login url.</param>
    Task LogoutUserAsync(string loginUrl);
}