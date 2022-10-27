namespace AssociationRegistry.Test.Helpers;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Logging;
using Nest;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

public static class WaitFor
{
    public static  Task PostGreSQLToBecomeAvailable(IDocumentStore store, ILogger logger, CancellationToken cancellationToken = default)
        => store is DocumentStore
            ? Wait(logger, () => store.LightweightSession().Events.QueryAllRawEvents().Take(1).SingleOrDefaultAsync(cancellationToken), "PostgreSQL", cancellationToken)
            : Task.CompletedTask;

    public static Task ElasticSearchToBecomeAvailable(IElasticClient client, ILogger logger, CancellationToken cancellationToken = default)
        => Wait(logger, () => client.PingAsync(ct: cancellationToken), "ElasticSearch", cancellationToken);

    private static async Task Wait(ILogger logger, Func<Task> waitAction, string serviceName, CancellationToken cancellationToken)
    {
        var watch = Stopwatch.StartNew();
        var tryCount = 0;
        var exit = false;
        while (!exit)
        {
            try
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Waiting until {ServiceName} becomes available ... ({WatchElapsed})", serviceName, watch.Elapsed);
                }

                await waitAction();

                exit = true;
            }
            catch (Exception exception)
            {
                if (tryCount >= 5)
                    throw new TimeoutException($"Service {serviceName} throws exception {exception.Message} after 5 tries", exception);

                tryCount++;
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(exception, "Encountered an exception while waiting for {ServiceName} to become available", serviceName);
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("{ServiceName} became available ... ({WatchElapsed})", serviceName, watch.Elapsed);
        }
    }
}
