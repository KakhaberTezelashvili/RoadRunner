namespace TDOC.Common.Server.Auth.Services;

/// <summary>
/// Validator provides methods to validate users.
/// </summary>
public interface IUserValidator
{
    /// <summary>
    /// Validates authentication user.
    /// </summary>
    /// <param name="userInitials">User initials.</param>
    void AuthUserByInitialsValidate(string userInitials);
}