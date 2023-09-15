using TDOC.Common.Server.Auth.Repositories;
using TDOC.Common.Server.Validations;
using TDOC.Data.Models;

namespace TDOC.Common.Server.Auth.Services;

/// <inheritdoc cref="IUserValidator" />
public class UserValidator : ValidatorBase<UserModel>, IUserValidator
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserValidator" /> class.
    /// </summary>
    /// <param name="userRepository">Repository provides methods to retrieve/handle users.</param>
    public UserValidator(IUserRepository userRepository) : base(userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public void AuthUserByInitialsValidate(string userInitials)
    {
        if (string.IsNullOrWhiteSpace(userInitials))
            throw ArgumentNotValidException();
    }
}