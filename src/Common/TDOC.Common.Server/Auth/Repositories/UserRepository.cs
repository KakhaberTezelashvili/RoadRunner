using Microsoft.EntityFrameworkCore;
using TDOC.Common.Server.Auth.Models;
using TDOC.Common.Server.Repositories;
using TDOC.Data.Models;
using TDOC.EntityFramework.DbContext;

namespace TDOC.Common.Server.Auth.Repositories;

/// <inheritdoc cref="IUserRepository" />
public class UserRepository : RepositoryBase<UserModel>, IUserRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository" /> class.
    /// </summary>
    /// <param name="context">Entity Framework database context.</param>
    public UserRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<AuthUserData> GetAuthUserByInitialsAsync(string userInitials)
    {
        return await _context.Users.AsNoTracking()
            .Where(u => u.Initials == userInitials)
            .Select(u => new AuthUserData()
            {
                KeyId = u.KeyId,
                Initials = u.Initials,
                Name = u.Name,
                Email = u.Email
            })
            .FirstOrDefaultAsync();
    }
}