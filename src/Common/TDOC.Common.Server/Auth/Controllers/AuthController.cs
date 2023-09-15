using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TDOC.Common.Data.Models.Auth;
using TDOC.Common.Server.Attributes;
using TDOC.Common.Server.Auth.Constants;
using TDOC.Common.Server.Auth.Models;
using TDOC.Common.Server.Auth.Services;
using TDOC.Common.Server.Controllers;

namespace TDOC.Common.Server.Auth.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle users.
/// </summary>
[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController" /> class.
    /// </summary>
    /// <param name="userService">Service provides methods to retrieve/handle users.</param>
    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get user by initials and generate authentication token.
    /// </summary>
    /// <param name="loginArgs">Login arguments.</param>
    /// <returns>Login result with authentication token in it.</returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [NoTransaction]
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<LoginResult>> LoginAsync([FromBody] LoginArgs loginArgs)
    {
        AuthUserData user = await _userService.GetAuthUserByInitialsAsync(loginArgs.UserInitials);
        if (user != null)
        {
            // Issue the claims for current user.
            Claim[] authClaims = AuthUserClaims.GetUserClaims(user.KeyId, user.Initials, user.Name, user.Email);

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTValidationParameters.JWTSecret));

            var token = new JwtSecurityToken(
                JWTValidationParameters.JWTValidIssuer,
                JWTValidationParameters.JWTValidAudience,
                expires: DateTime.Now.AddYears(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return new LoginResult()
            {
                AuthToken = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        return BadRequest();
    }
}