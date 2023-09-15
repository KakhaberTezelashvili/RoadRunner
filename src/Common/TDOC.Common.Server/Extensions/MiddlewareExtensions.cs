using Microsoft.AspNetCore.Builder;
using TDOC.Common.Server.Middlewares;

// Do not change namespace to Common.Server.Extensions
namespace TDOC.Common.Server.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app) => 
            app.UseMiddleware<ApiExceptionHandlingMiddleware>();

        public static IApplicationBuilder UseDbTransaction(this IApplicationBuilder app) => 
            app.UseMiddleware<DbTransactionMiddleware>();
    }
}