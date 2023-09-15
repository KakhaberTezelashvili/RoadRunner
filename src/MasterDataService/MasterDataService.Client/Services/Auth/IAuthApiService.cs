using TDOC.Common.Data.Models.Auth;

namespace MasterDataService.Client.Services.Auth;

/// <summary>
/// API service provides methods to handle authentication.
/// </summary>
public interface IAuthApiService
{
    /// <summary>
    /// Login user.
    /// </summary>
    /// <param name="userInitials">User initials.</param>
    /// <returns>Login result with authentication token in it.</returns>
    Task<LoginResult> LoginAsync(string userInitials);
}