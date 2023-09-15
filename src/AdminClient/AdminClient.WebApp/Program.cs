using AdminClient.WebApp;
using AdminClient.WebApp.Core.Grids;
using MasterDataService.Client.MapperProfiles;
using MasterDataService.Client.Services.Customers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;
using TDOC.Common.Client.Extensions;
using TDOC.Common.Client.MediatorCarrier;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.WebComponents.Shared.Models;

await WebAppClientBuilder.SetupBuilder(
    args,
    new List<(string, string)>
    {
        (MasterDataService.Client.Constants.HttpClientConfigurationName.MasterDataClient, "ApiEndpoints:MasterDataServiceApiUrl"),
        (SearchService.Client.Constants.HttpClientConfigurationName.SearchClient, "ApiEndpoints:SearchServiceApiUrl")
    },
    new List<Assembly>() { typeof(MediaMapperProfile).Assembly },
    RegisterRootComponents, RegisterServices);

static void RegisterRootComponents(WebAssemblyHostBuilder builder)
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

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
                        typeof(ISearchApiService).Assembly)
        .AddClasses().AsMatchingInterface().WithTransientLifetime());
    services.AddTransient<BrowserService>();

    // Setup MediatoR notification handlers.
    services.AddSingleton<INotificationHandler<HideToastNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<NotificationDetails>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<ShowToastNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<ShowConfirmationNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<CompleteConfirmationNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<ShortcutNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
    services.AddSingleton<INotificationHandler<ComponentDataChangedNotification>>(provider => provider.GetRequiredService<MediatorCarrier>());
}