﻿namespace AssociationRegistry.Test.Framework.Helpers;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
using Npgsql;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

public static class WaitFor
{
    private static async Task TryProbePostgres(string connectionString)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        conn.Open();
        const string cmdText = "SELECT 1 FROM pg_database WHERE datname='postgres'";
        await using var cmd = new NpgsqlCommand(cmdText, conn);
        if (cmd.ExecuteScalar() == null)
            throw new Exception("Root Postgres database does not yet exist");
    }

    public static Task PostGreSQLToBecomeAvailable(ILogger logger, string connectionString, CancellationToken cancellationToken = default)
        => Wait(logger, () => TryProbePostgres(connectionString), "PostgreSQL", cancellationToken);

    public static Task ElasticSearchToBecomeAvailable(IElasticClient client, ILogger logger, CancellationToken cancellationToken = default)
        => Wait(
            logger,
            async () =>
            {
                var response = await client.PingAsync(ct: cancellationToken);
                if (!response.IsValid)
                    throw new Exception("Could not Ping to ES");
            },
            "ElasticSearch",
            cancellationToken);

    public static async Task Wait(ILogger logger, Func<Task> waitAction, string serviceName, CancellationToken cancellationToken = default)
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
