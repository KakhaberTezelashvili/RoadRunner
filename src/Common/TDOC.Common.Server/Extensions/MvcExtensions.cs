using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TDOC.Common.Server.Validations;

namespace TDOC.Common.Server.Extensions
{
    public static class MvcExtensions
    {
        public static IServiceCollection AddAndConfigControllers(this IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            });

            services.AddRazorPages();

            services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();

            return services;
        }
    }
}