using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TDOC.Common.Server.Auth.Extensions;
using TDOC.Common.Server.Conventions;

namespace TDOC.Common.Server.Extensions;

public class WebAppServerBuilder
{
    public static void SetupBuilder(
        string[] args, 
        string routingPrefix,
        string swaggerSharedModelsAssemblyName,
        string swaggerApiProjectName,
        string swaggerFileUrl,
        Action<IServiceCollection> registerServices,
        Action<IServiceProvider> initializeServices = null)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
        Log.Information("Starting up");

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

            // Add services to the container.
            builder.Services.AddAndConfigControllers();
            builder.Services.AddAndConfigApiVersioning();
            builder.Services.AddAndConfigSwagger(swaggerSharedModelsAssemblyName);
            builder.Services.AddAndConfigEntityFramework(builder.Configuration);
            builder.Services.AddAndConfigIdentity();
            builder.Services.AddAndConfigLocalization();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            registerServices(builder.Services);

            builder.Services.AddControllersWithViews(options =>
            {
                options.Conventions.Add(new ControllerRoutingConvention(routingPrefix));
            });

            // TODO: Move SignalR functionality together with this code into microservice "NotificationService".
            /*if (useSignalR)
            {
                builder.Services.AddSignalR();
                builder.Services.AddSingleton<IUserIdProvider, HubBaseUserIdProvider>();
            }*/

            WebApplication app = builder.Build();

            app.UseApiExceptionHandling();

            if (app.Environment.IsDevelopment())
                app.UseWebAssemblyDebugging();
            else
            {
                //app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios,
                // see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection(); // TODO: https redirection not working together with ocelot
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            // Use Localization.
            app.UseRequestLocalization();

            // Use Swagger.
            app.UseAndConfigSwaggerUI(
                app.Environment, new[] { "Development" },
                swaggerApiProjectName, $"{routingPrefix}/swagger-api", swaggerFileUrl);

            // Enable Cross-Origin Requests (CORS).
            app.UseCors(policy =>
                policy.WithOrigins(builder.Configuration.GetSection("CorsOptions:Origins").Value.Split(";"))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            initializeServices?.Invoke(app.Services);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseDbTransaction();

            app.MapControllers();

            // TODO: Move SignalR functionality together with this code into microservice "NotificationService".
            /*if (useSignalR)
                app.MapHub<HubProgress>($"{routingPrefix}/{HubQueryParameters.Hubs}/{nameof(HubNames.Progress)}");*/

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
