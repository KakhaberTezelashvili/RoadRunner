using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore.Storage;
using TDOC.Common.Server.Attributes;
using TDOC.EntityFramework.DbContext;

namespace TDOC.Common.Server.Middlewares
{
    public class DbTransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public DbTransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, TDocEFDbContext dbContext)
        {
            // For HTTP GET opening transaction is not required
            if (httpContext.Request.Method.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
            {
                await _next(httpContext);
                return;
            }

            // If action is decorated with NoTransactionAttribute then skip opening transaction
            Endpoint endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            NoTransactionAttribute attribute = endpoint?.Metadata.GetMetadata<NoTransactionAttribute>();
            if (attribute != null)
            {
                await _next(httpContext);
                return;
            }

            IDbContextTransaction transaction = null;
            try
            {
                transaction = await dbContext.Database.BeginTransactionAsync();
                
                await _next(httpContext); // called savechanges 3 times

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                if (transaction != null)
                    await transaction.RollbackAsync();

                throw;
            }
            finally
            {
                if (transaction != null)
                    await transaction.DisposeAsync();
            }
        }
    }
}