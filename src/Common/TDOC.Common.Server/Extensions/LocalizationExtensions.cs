using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using TDOC.Common.Server.Localization;

namespace TDOC.Common.Server.Extensions
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddAndConfigLocalization(this IServiceCollection services)
        {
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                List<CultureInfo> supportedCultures = SupportedCultures.AsList();
                options.DefaultRequestCulture = new RequestCulture(SupportedCultures.DefaultCultureName);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                // todo: uncomment "RequestCultureProviders" as soon as
                // LicenseRequestCultureProvider will be implemented.
                /*options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new LicenseRequestCultureProvider()
                };*/
            });
            // todo: use AuthLocalizationExtensions from AuthUI
            //builder.AddAuthDataAnnotationsLocalization();

            return services;
        }
    }
}