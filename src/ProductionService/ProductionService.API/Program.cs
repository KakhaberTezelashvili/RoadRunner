using ProductionService.Core.Services.Batches;
using ProductionService.Infrastructure.Repositories;
using TDOC.Common.Server.Extensions;

WebAppServerBuilder.SetupBuilder(
    args, "production",
    "ProductionService.Shared", "ProductionService API", "pswag",
    RegisterServices);

static void RegisterServices(IServiceCollection services)
{
    services.Scan(scan => scan
        .FromAssemblies(typeof(IBatchService).Assembly, typeof(BatchRepository).Assembly)
        .AddClasses().AsMatchingInterface().WithTransientLifetime());
}