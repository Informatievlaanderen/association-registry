using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AssociationRegistry.KboMutations.SyncLambda.Logging;

public class LambdaLoggerProvider : ILoggerProvider
{
    private readonly ILambdaLogger _lambdaLogger;

    public LambdaLoggerProvider(ILambdaLogger lambdaLogger)
    {
        _lambdaLogger = lambdaLogger;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new LambdaLogger(_lambdaLogger);
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}

public class LambdaLogger : ILogger
{
    private readonly ILambdaLogger _lambdaLogger;

    public LambdaLogger(ILambdaLogger lambdaLogger)
    {
        _lambdaLogger = lambdaLogger;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel)
    {
        // Implement logic to enable/disable logging based on the log level if needed
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        _lambdaLogger.LogLine($"[{logLevel}] {message}");
    }
}
