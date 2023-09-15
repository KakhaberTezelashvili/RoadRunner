namespace TDOC.Common.Server.Auth.Models;

/// <summary>
/// Authentication user data.
/// </summary>
public class AuthUserData
{
    /// <summary>
    /// Primary key identifier for the specified entity.
    /// </summary>
    public int KeyId { get; set; }

    /// <summary>
    /// User initials.
    /// </summary>
    public string Initials { get; set; }

    /// <summary>
    /// User name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// User e-mail.
    /// </summary>
    public string Email { get; set; }
}