using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TDOC.Common.Client.Interceptors;

namespace TDOC.Common.Client.Extensions
{
    public static class LoggerExtensions
    {
        public static ILoggingBuilder AddUnhandledExceptionLoggerProvider(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<UnhandledExceptionSender>();
            builder.Services.AddSingleton<ILoggerProvider, UnhandledExceptionLoggerProvider>();

            return builder;
        }
    }
}