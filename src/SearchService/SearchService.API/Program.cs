using SearchService.Core.Services.Search;
using SearchService.Infrastructure.Repositories;
using TDOC.Common.Server.Extensions;
using TDOC.Data.Models;
using TDOC.EntityFramework.DbContext;

WebAppServerBuilder.SetupBuilder(
    args, "search",
    "SearchService.Shared", "SearchService API", "sswag",
    RegisterServices, InitializeEntityFramework);

static void RegisterServices(IServiceCollection services)
{
    services.Scan(scan => scan
        // To register classes without implementing services like SearchSpecificationFactory.
        .FromAssemblyOf<ISearchService>() 
        .FromAssemblies(typeof(ISearchService).Assembly, typeof(SearchRepository).Assembly)
        .AddClasses().AsMatchingInterface().WithTransientLifetime());
}

static void InitializeEntityFramework(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TDocEFDbContext>();
    context.Set<AGSModel>();
}