namespace TDOC.Common.Data.Auth.Constants;

/// <summary>
/// UserClaims constants are used to define custom claims in the AuthServer library.
/// Context accessors are used to retrieve the custom and build in claims from the request context.
/// alternatively <see cref="IHttpContextAccessor"/> is used for finding the claim values for an authenticated user.
/// <code>
/// HttpContext.User.FindFirst(UserClaimTypes.UserId);
/// </code>
/// </summary>
public class UserClaimTypes
{
    public const string UserId = "user_id";
    public const string Initials = "initials";
    public const string Level = "level";
}