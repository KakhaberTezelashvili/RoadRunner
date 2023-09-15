using IdentityModel;
using System.Security.Claims;
using TDOC.Common.Data.Auth.Constants;

namespace TDOC.Common.Server.Auth.Models;

/// <summary>
/// User claims.
/// </summary>
public static class AuthUserClaims
{
    /// <summary>
    /// Generator of user claims.
    /// </summary>
    /// <param name="keyId">User key identifier.</param>
    /// <param name="initials">User initials.</param>
    /// <param name="name">User name.</param>
    /// <param name="email">User email.</param>
    /// <returns></returns>
    public static Claim[] GetUserClaims(int keyId, string initials, string name, string email)
    {
        return new Claim[]
            {
                new Claim(JwtClaimTypes.Name, !string.IsNullOrWhiteSpace(name) ? name : ""),
                new Claim(JwtClaimTypes.Email, !string.IsNullOrWhiteSpace(email) ? email : ""),
                new Claim(UserClaimTypes.UserId, keyId.ToString()),
                new Claim(UserClaimTypes.Initials, initials),
                new Claim(UserClaimTypes.Level, "User")
            };
    }
}