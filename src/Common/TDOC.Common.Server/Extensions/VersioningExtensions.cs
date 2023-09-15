using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace TDOC.Common.Server.Extensions
{
    public static class VersioningExtensions
    {
        public static IServiceCollection AddAndConfigApiVersioning(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

            services.AddApiVersioning(options => { options.ReportApiVersions = true; });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}