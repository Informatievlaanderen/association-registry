namespace AssociationRegistry.Test.Helpers;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Logging;

public static class WaitFor
{
    public static async Task PostGreSQLToBecomeAvailable(IDocumentStore store, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (store is DocumentStore)
        {
            var watch = Stopwatch.StartNew();
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
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning(exception, "Encountered an exception while waiting for PostGreSql store to become available");
                    }

                    await Task.Delay(1000, cancellationToken);
                }
            }
        }
    }
}


