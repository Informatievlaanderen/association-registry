namespace AssociationRegistry.KboMutations.SyncLambda.Logging;

using Amazon.Lambda.Core;
using Exceptions;
using Microsoft.Extensions.Logging;
using Services;

public class SyncLogger
{
    public SyncLogger(LambdaServices? services, ILambdaContext context)
    {
        if (services != null)
        {
            var logger = services.LoggerFactory.CreateLogger("KboMutations.SyncLambda");

            _exceptionLoggingFunction = (e, m, objects) =>
            {
                logger.LogError(e, m, objects);
            };

            _InformationLoggingFunction = (m, objects) =>
            {
                logger.LogInformation(m, objects);
            };
        }
        else
        {
            _exceptionLoggingFunction = (e, m, objects) =>
            {
                context.Logger.LogError(e, m, objects);
            };

            _InformationLoggingFunction = (m, objects) =>
            {
                context.Logger.LogInformation(m, objects);
            };
        }
    }

    private readonly Action<Exception, string, object?[]> _exceptionLoggingFunction;
    private readonly Action<string, object?[]> _InformationLoggingFunction;

    public void LogException(Exception e, string? message, params object?[] args)
    {
        _exceptionLoggingFunction(e, message, args);
    }

    public void LogInformation(string message, params object?[] args)
    {
        _InformationLoggingFunction(message, args);
    }
}
