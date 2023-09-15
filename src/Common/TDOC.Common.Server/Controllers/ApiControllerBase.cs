using Microsoft.AspNetCore.Mvc;
using TDOC.Common.Server.Http.Headers;

// TODO: BTA WI17200 - Code style violation: Documentation missing
namespace TDOC.Common.Server.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApiControllerBase : ControllerBase
    {
        protected bool IsContentCachedOnClient(DateTime? lastModifiedLocalTime)
        {
            if (lastModifiedLocalTime == null)
                return false;
            string modifiedSinceHeader = Request.Headers[HeaderKeys.IfModifiedSince];

            if (modifiedSinceHeader == null)
                return false;
            // Retrieve modified-since in local date/time
            if (DateTime.TryParse(modifiedSinceHeader, out DateTime sinceDateTime))
            {
                // Since from header we retrieving datetime without milliseconds we will do
                // next check
                return (lastModifiedLocalTime.Value - sinceDateTime).TotalSeconds < 1;
            }

            return false;
        }

        protected void AddCacheControlAndLastModifiedHeadersToResponse(DateTime? lastModifiedDateTime)
        {
            // Cache-Control:no-cache - means always validate cache actuality on the server
            Response.Headers.Add(
                HeaderKeys.CacheControl,
                new Microsoft.Extensions.Primitives.StringValues("no-cache"));

            // Last-Modified:01.01.2020 10:00:00
            if (lastModifiedDateTime == null)
                return;
            // Convert last modified date/time to GMT
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            DateTime lastModifiedGMT = TimeZoneInfo.ConvertTime(lastModifiedDateTime.Value, timeZoneInfo);

            string lastModifiedValue = LastModifiedHeader.GetValue(lastModifiedGMT);
            Response.Headers.Add(
                HeaderKeys.LastModified,
                new Microsoft.Extensions.Primitives.StringValues(lastModifiedValue)
            );
        }
    }
}