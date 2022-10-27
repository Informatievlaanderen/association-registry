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
    public static async Task PostGreSQLToBecomeAvailable(IDocumentStore store, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (store is DocumentStore)
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
                        logger.LogInformation("Waiting until PostGreSql store becomes available ... ({WatchElapsed})", watch.Elapsed);
                    }

                    await store.LightweightSession().Events.QueryAllRawEvents().Take(1).SingleOrDefaultAsync(cancellationToken);
                    exit = true;
                }
                catch (Exception exception)
                {
                    if (tryCount >= 5)
                        throw new TimeoutException($"Service throws exception {exception.Message} after 5 tries", exception);

                    tryCount++;
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning(exception, "Encountered an exception while waiting for PostGreSql store to become available");
                    }

                    await Task.Delay(1000, cancellationToken);
                }
            }
        }
    }

    public static async Task ElasticSearchToBecomeAvailable(IElasticClient client, ILogger logger, CancellationToken cancellationToken = default)
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
                    logger.LogInformation("Waiting until ElasticSearch becomes available ... ({WatchElapsed})", watch.Elapsed);
                }

                client.Ping();

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("ElasticSearch became available ... ({WatchElapsed})", watch.Elapsed);
                }

                exit = true;
            }
            catch (Exception exception)
            {
                if (tryCount >= 5)
                    throw new TimeoutException($"Service throws exception {exception.Message} after 5 tries", exception);

                tryCount++;
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(exception, "Encountered an exception while waiting for ElasticSearch to become available");
                }

                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
