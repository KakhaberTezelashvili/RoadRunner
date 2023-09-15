using MasterDataService.Client.MapperProfiles;
using MasterDataService.Client.Services.Customers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProductionService.Client.Services.Positions;
using ScannerClient.WebApp;
using ScannerClient.WebApp.Core.Grids;
using ScannerClient.WebApp.Core.Scanner.Models;
using System.Reflection;
using TDOC.Common.Client.MediatorCarrier;
using TDOC.WebComponents.LoadingIndicator.Models;

await WebAppClientBuilder.SetupBuilder(
    args,
    new List<(string, string)>
    {
        (ProductionService.Client.Constants.HttpClientConfigurationName.ProductionClient, "ApiEndpoints:ProductionServiceApiUrl"),
        (SearchService.Client.Constants.HttpClientConfigurationName.SearchClient.ToString(), "ApiEndpoints:SearchServiceApiUrl"),
        (MasterDataService.Client.Constants.HttpClientConfigurationName.MasterDataClient.ToString(), "ApiEndpoints:MasterDataServiceApiUrl"),
    },
    new List<Assembly>(){ typeof(MediaMapperProfile).Assembly },
    RegisterRootComponents, RegisterServices);

static void RegisterRootComponents(WebAssemblyHostBuilder builder) => builder.RootComponents.Add<App>("#app");

static void RegisterServices(IServiceCollection services)
{
    // Setup Fluxor (support storing state based on Redux pattern).
    services.AddFluxor(options =>
    {
        options.ScanAssemblies(typeof(Program).Assembly);
        options.ScanAssemblies(typeof(NotificationState).Assembly);
        options.UseReduxDevTools();
        options.UseRouting();
    });

    // Setup DevExpress.Blazor.
    services.AddDevExpressBlazor();

    // Setup required services.
    services.Scan(scan => scan
        // To register classes without implementing services.
        .FromAssemblyOf<GridDefaultColumnsService>()
        .FromAssemblies(typeof(GridDefaultColumnsService).Assembly, 
                        typeof(ICustomerApiService).Assembly, 
                        typeof(IPositionApiService).Assembly, 
                        typeof(ISearchApiService).Assembly)
        .AddClasses().AsMatchingInterface().WithTransientLifetime());
    services.AddTransient<BrowserService>();

    // Setup MediatoR notification handlers.
    services.AddSingleton<INotificationHandler<BarcodeDataNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<HideToastNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<LoadingNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<NotificationDetails>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<ShowToastNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<ShowConfirmationNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<CompleteConfirmationNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
}