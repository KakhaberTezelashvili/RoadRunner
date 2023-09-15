using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TDOC.Common.Server.Auth.Constants;
using TDOC.Common.Server.Auth.Repositories;
using TDOC.Common.Server.Auth.Services;

namespace TDOC.Common.Server.Auth.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddAndConfigIdentity(this IServiceCollection services)
    {
        // todo: Get configurations from AppServer through WebSocket.
        //var webSiteOptions = new SystemOptions.WebSiteOptions();
        // todo: Use AuthIdentityAndAccessControlExtensions from AuthServer.
        //services.AddIdentityAndAccessControlServices(AuthConstants.ScannerWebClientId, webSiteOptions);

        // Adding Authentication.
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = JWTValidationParameters.JWTValidAudience,
                    ValidIssuer = JWTValidationParameters.JWTValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTValidationParameters.JWTSecret))
                };
                // TODO: Move SignalR functionality together with this code into microservice "NotificationService".
                /*options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            // In case user is connection to a realtime hub we should fetch user token 
                            // and set it in hub context in order to identify each user.
                            StringValues accessToken = context.Request.Query[HubQueryParameters.AccessToken];

                            if (!string.IsNullOrEmpty(accessToken) &&
                                context.HttpContext.Request.Path.ToString().Contains($"/{HubQueryParameters.Hubs}/"))
                                    context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };*/
            });

        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserValidator, UserValidator>();
        services.AddTransient<IUserRepository, UserRepository>();

        return services;
    }
}