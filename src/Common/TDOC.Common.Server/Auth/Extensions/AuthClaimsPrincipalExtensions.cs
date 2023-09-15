using System.Security.Claims;
using TDOC.Common.Data.Auth.Constants;

namespace TDOC.Common.Server.Auth.Extensions;

public static class AuthClaimsPrincipalExtensions
{
    public static int GetUserKeyId(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        return int.TryParse(principal.FindFirst(UserClaimTypes.UserId)?.Value, out int userKeyId) ? userKeyId : 0;
    }
}