namespace TDOC.Common.Server.Auth.Constants;

//todo: add jwt values in appsettings.json for dynamicity and security
public class JWTValidationParameters
{
    public const string JWTValidAudience = "https://localhost:5001";
    public const string JWTValidIssuer = "https://localhost:5001";
    public const string JWTSecret = "StrONGKAutHENTICATIONKEy";
}