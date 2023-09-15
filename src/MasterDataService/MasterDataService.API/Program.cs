using MasterDataService.Core.Services.Customers;
using MasterDataService.Infrastructure.Repositories;
using TDOC.Common.Server.Extensions;

WebAppServerBuilder.SetupBuilder(
    args, "master-data",
    "MasterDataService.Shared", "MasterDataService API", "mdswag",
    RegisterServices);

static void RegisterServices(IServiceCollection services)
{
    services.Scan(scan => scan
        .FromAssemblies(typeof(ICustomerService).Assembly, typeof(CustomerRepository).Assembly)
        .AddClasses().AsMatchingInterface().WithTransientLifetime());
}
