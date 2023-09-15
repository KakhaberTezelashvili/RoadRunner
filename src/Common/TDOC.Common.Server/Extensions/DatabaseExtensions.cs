using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TDOC.EntityFramework.DbContext;

namespace TDOC.Common.Server.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddAndConfigEntityFramework(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<TDocEFDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("TDocContext"));
                option.ConfigureWarnings(
                    warnings => { warnings.Ignore(CoreEventId.ForeignKeyAttributesOnBothNavigationsWarning); }
                );
            });

            return services;
        }
    }
}