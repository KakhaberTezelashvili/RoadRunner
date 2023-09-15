using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using TDOC.Common.Server.Swagger;

namespace TDOC.Common.Server.Extensions
{
    /// <summary>
    /// The swagger extensions.
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Adds and configure swagger.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="sharedModelsAssemblyName">The assembly name that contains shared models.</param>
        /// <returns>An IServiceCollection.</returns>
        public static IServiceCollection AddAndConfigSwagger(this IServiceCollection services, string sharedModelsAssemblyName)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                string xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                string[] swagAssemblies = new[] { sharedModelsAssemblyName };
                foreach (string swagAssembly in swagAssemblies)
                {
                    Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                        .SingleOrDefault(assembly => assembly.GetName().Name == swagAssembly);
                    string coreLocation = assembly.Location;
                    string coreFolder = coreLocation.Substring(0, coreLocation.IndexOf(swagAssembly));
                    string xmlPathCore = Path.Combine(coreFolder, $"{swagAssembly}.xml");
                    options.IncludeXmlComments(xmlPathCore);
                }

                options.IgnoreObsoleteActions();
                options.IgnoreObsoleteProperties();
                options.EnableAnnotations();
            });

            return services;
        }

        /// <summary>
        /// Adds and configure swagger UI.
        /// </summary>
        /// <param name="appBuilder">The app builder.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="environmentNames">The list of environment names to enable swagger.</param>
        /// <param name="apiProjectName">The API project name.</param>
        /// <param name="routePrefix">The swagger UI route prefix. Default value is "swagger-api".</param>
        /// <param name="fileUrl">The swagger first URL segment. Default value is "sswag".</param>
        /// <param name="jsonFileName">The swagger JSON file name. Default value is "swagger.json"</param>
        /// <returns>An IApplicationBuilder.</returns>
        public static IApplicationBuilder UseAndConfigSwaggerUI(
            this IApplicationBuilder appBuilder,
            IWebHostEnvironment hostEnvironment,
            string[] environmentNames,
            string apiProjectName,
            string routePrefix = "swagger-api",
            string fileUrl = "swag",
            string jsonFileName = "swagger.json"
            )
        {
            if (!environmentNames.All(hostEnvironment.IsEnvironment))
                return appBuilder;

            appBuilder.UseSwagger(option =>
            {
                // ocelot requires different address for swagger json file in production and search
                // service api
                option.RouteTemplate = $"{fileUrl}/{{documentName}}/{jsonFileName}";
            });

            using AsyncServiceScope scope = appBuilder.ApplicationServices.CreateAsyncScope();
            IApiVersionDescriptionProvider apiProvider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

            appBuilder.UseSwaggerUI(options =>
            {
                options.DocumentTitle = apiProjectName;
                options.RoutePrefix = routePrefix;
                options.DefaultModelRendering(ModelRendering.Example);

                // The default expansion depth for the model on the model-example section.
                options.DefaultModelExpandDepth(1);

                // The default expansion depth for models (set to -1 completely hide the models).
                options.DefaultModelsExpandDepth(-1);

                foreach (ApiVersionDescription description in apiProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/{fileUrl}/{description.GroupName}/{jsonFileName}",
                        description.GroupName.ToUpperInvariant());
                }
            });

            return appBuilder;
        }
    }
}