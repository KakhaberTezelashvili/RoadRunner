using TDOC.Common.Server.Auth.Models;
using TDOC.Common.Server.Auth.Repositories;

namespace TDOC.Common.Server.Auth.Services;

/// <inheritdoc cref="IUserService" />
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserValidator _userValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService" /> class.
    /// </summary>
    /// <param name="userRepository">Repository provides methods to retrieve/handle users.</param>
    /// <param name="userValidator">Validator provides methods to validate users.</param>
    public UserService(IUserRepository userRepository, IUserValidator userValidator)
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
    }

    /// <inheritdoc />
    public async Task<AuthUserData> GetAuthUserByInitialsAsync(string userInitials)
    {
        _userValidator.AuthUserByInitialsValidate(userInitials);
        AuthUserData authUserData = await _userRepository.GetAuthUserByInitialsAsync(userInitials);
        return authUserData;
    }
}