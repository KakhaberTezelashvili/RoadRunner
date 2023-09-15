using TDOC.Common.Server.Auth.Models;

namespace TDOC.Common.Server.Auth.Services;

/// <summary>
/// Service provides methods to retrieve/handle users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves authentication user details by initials.
    /// </summary>
    /// <param name="userInitials">User initials.</param>
    /// <returns>Authentication user details.</returns>
    Task<AuthUserData> GetAuthUserByInitialsAsync(string userInitials);
}