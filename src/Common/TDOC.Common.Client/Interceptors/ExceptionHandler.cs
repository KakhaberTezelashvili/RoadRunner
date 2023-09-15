using Microsoft.Extensions.Logging;

namespace TDOC.Common.Client.Interceptors;

public interface IUnhandledExceptionSender
{
    event EventHandler<Exception> UnhandledExceptionThrown;
}

public class UnhandledExceptionSender : ILogger, IUnhandledExceptionSender
{
    public event EventHandler<Exception> UnhandledExceptionThrown;

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        if (exception != null)
            UnhandledExceptionThrown?.Invoke(this, exception);
    }
}

public class UnhandledExceptionLoggerProvider : ILoggerProvider
{
    private readonly UnhandledExceptionSender _unhandledExceptionSender;
    private readonly IExceptionService _exceptionService;

    public UnhandledExceptionLoggerProvider(UnhandledExceptionSender unhandledExceptionSender, IExceptionService exceptionService)
    {
        _unhandledExceptionSender = unhandledExceptionSender;
        _exceptionService = exceptionService;
    }

    public ILogger CreateLogger(string categoryName) => new UnhandledExceptionLogger(categoryName, _unhandledExceptionSender, _exceptionService);

    public void Dispose()
    {
    }

    public class UnhandledExceptionLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly UnhandledExceptionSender _unhandledExceptionSender;
        private readonly IExceptionService _exceptionService;

        public UnhandledExceptionLogger(string categoryName, UnhandledExceptionSender unhandledExceptionSender, IExceptionService exceptionService)
        {
            _categoryName = categoryName;
            _unhandledExceptionSender = unhandledExceptionSender;
            _exceptionService = exceptionService;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (exception != null)
                _ = _exceptionService.ShowException(exception);
        }

        public IDisposable BeginScope<TState>(TState state) => new NoopDisposable();

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}