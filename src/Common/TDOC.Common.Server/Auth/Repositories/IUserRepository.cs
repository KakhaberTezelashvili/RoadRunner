using TDOC.Common.Server.Auth.Models;
using TDOC.Common.Server.Repositories;
using TDOC.Data.Models;

namespace TDOC.Common.Server.Auth.Repositories;

/// <summary>
/// Repository provides methods to retrieve/handle users.
/// </summary>
public interface IUserRepository : IRepositoryBase<UserModel>
{
    /// <summary>
    /// Retrieves authentication user details by initials.
    /// </summary>
    /// <param name="userInitials">User initials.</param>
    /// <returns>Authentication user details.</returns>
    Task<AuthUserData> GetAuthUserByInitialsAsync(string userInitials);
}