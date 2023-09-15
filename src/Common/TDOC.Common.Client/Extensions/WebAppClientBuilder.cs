using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Polly;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;
using System.Globalization;
using System.Reflection;
using TDOC.Common.Client.Auth.Services;
using TDOC.Common.Client.Http;
using TDOC.Common.Client.Interceptors;
using TDOC.Common.Client.Navigation;
using TDOC.Common.Client.Translations;
using TDOC.Common.MediatorCarrier;
using TDOC.Common.Timers;

namespace TDOC.Common.Client.Extensions;

public class WebAppClientBuilder
{
    public static async Task SetupBuilder(
        string[] args,
        IList<(string, string)> httpClients,
        IList<Assembly> autoMapperAssemblies,
        Action<WebAssemblyHostBuilder> registerRootComponents,
        Action<IServiceCollection> registerServices)
    {
        SetupLogger();
        try
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            registerRootComponents(builder);

            builder.Services.AddOptions();

            // Setup mediator.
            SetupMediator(builder.Services);

            // Setup global exceptions handler.
            builder.Logging.ClearProviders();
            builder.Logging.AddUnhandledExceptionLoggerProvider();

            // Setup typed http client factory.
            SetupHttpClientFactory(builder, httpClients);

            // Setup local storage.
            builder.Services.AddBlazoredLocalStorage();

            // Setup authorization.
            SetupAuthorization(builder.Services);

            // Setup AutoMapper.
            ((List<Assembly>)autoMapperAssemblies).AddRange(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddAutoMapper(autoMapperAssemblies);

            // Setup localization.
            builder.Services.AddLocalization();

            // Setup navigation manager wrapper.
            SetupNavigationManagerWrapper(builder.Services);

            // Register required services.
            builder.Services.AddTransient<CustomTimer>();
            builder.Services.AddTransient<IExceptionService, ExceptionService>();
            builder.Services.AddTransient<IModelToDbMapper, ModelToDbMapper.ModelToDbMapper>();
            builder.Services.AddTransient<ITranslationService, TranslationService>();
            registerServices(builder.Services);

            WebAssemblyHost host = builder.Build();

            // Setup default application culture.
            await SetupDefaultAppCultureAsync(host);

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An exception occurred while creating the WASM host");
            throw;
        }
    }

    private static void SetupLogger()
    {
        var levelSwitch = new LoggingLevelSwitch();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
            .Enrich.WithProperty("User", "DA")
            .Enrich.WithProperty("Company", "Getinge")
            .Enrich.WithExceptionDetails()
            // To allow "BrowserHttp" required nuget: Serilog.Sinks.BrowserHttp 1.0.0-dev-00012.
            //.WriteTo.BrowserHttp(controlLevelSwitch: levelSwitch)
            .CreateLogger();
    }

    private static void SetupMediator(IServiceCollection services)
    {
        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<ServiceFactory>(provider => t => provider.GetService(t));
        services.AddSingleton<MediatorCarrier.MediatorCarrier>();
        services.AddSingleton<IMediatorCarrier>(provider => provider.GetRequiredService<MediatorCarrier.MediatorCarrier>());
        services.AddSingleton<INotificationHandler<ErrorMessagesNotification>>(provider => provider.GetRequiredService<MediatorCarrier.MediatorCarrier>());
    }

    private static void SetupNavigationManagerWrapper(IServiceCollection services)
    {
        var navigationService = services.FirstOrDefault(service => service.ServiceType.FullName == typeof(NavigationManager).FullName);
        if (navigationService != null)
        {
            // Get rid of default Navigation Manager.
            services.Remove(navigationService);

            // Add NavigationManager wrapper instead.
            var navigationManagerWrapper = new NavigationManagerWrapper((NavigationManager)navigationService.ImplementationInstance!);
            services.AddSingleton<NavigationManager>(navigationManagerWrapper);
            services.AddSingleton<NavigationManagerWrapper>(navigationManagerWrapper);
        }
    }

    private static void SetupHttpClientFactory(WebAssemblyHostBuilder builder, IList<(string, string)> httpClients)
    {
        builder.Services.AddScoped<HttpMessageResponseHandler>();

        // Include https client(s).
        httpClients.ToList().ForEach((httpClient) =>
        {
            builder.Services
                .AddHttpClient(httpClient.Item1, c => c.BaseAddress = new Uri(builder.Configuration.GetSection(httpClient.Item2).Value))
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { })
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1) }))
                .AddHttpMessageHandler<HttpMessageResponseHandler>();
        });

        // Add typed http client factory.
        builder.Services.AddScoped<ITypedHttpClientFactory, TypedHttpClientFactory>();
    }

    private static void SetupAuthorization(IServiceCollection services)
    {
        // By default, configuration for the app is loaded by convention from _configuration/{client-id}
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
        services.AddScoped<IAuthActionService, AuthActionService>();
    }

    private static async Task SetupDefaultAppCultureAsync(WebAssemblyHost host)
    {
        // Get current language: "appCulture.get" returns the active language. By default it is the
        // language of the browser. However, if the language has been save to the local storage by
        // calling "appCulture.set" then the function returns the stored language.
        IJSRuntime jsInterop = host.Services.GetRequiredService<IJSRuntime>();
        string language = await jsInterop.InvokeAsync<string>("appCulture.get");
        if (language != null)
        {
            // Set the default culture to match the language.
            var culture = new CultureInfo(language);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}