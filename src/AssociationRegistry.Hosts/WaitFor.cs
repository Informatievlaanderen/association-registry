namespace AssociationRegistry.Hosts;

using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using Npgsql;
using System.Diagnostics;
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
        => Wait(logger, waitAction: () => TryProbePostgres(connectionString), serviceName: "PostgreSQL", maxRetryCount: 5, cancellationToken);

    public static Task ElasticSearchToBecomeAvailable(ElasticsearchClient client, ILogger logger, int maxRetryCount = 5, CancellationToken cancellationToken = default)
        => Wait(
            logger,
            waitAction: async () =>
            {
                var p = await client.InfoAsync();
                var response = await client.PingAsync(cancellationToken: cancellationToken);

                if (!response.IsValidResponse)
                    throw new Exception($"Could not Ping to ES: {response.DebugInformation}");
            },
            serviceName: "ElasticSearch", maxRetryCount,
            cancellationToken);

    public static async Task Wait(ILogger logger, Func<Task> waitAction, string serviceName, int maxRetryCount, CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();
        var tryCount = 0;
        var exit = false;

        while (!exit)
        {
            try
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation(message: "Waiting until {ServiceName} becomes available ... ({WatchElapsed})", serviceName,
                                          watch.Elapsed);

                await waitAction();

                exit = true;
            }
            catch (Exception exception)
            {
                if (tryCount >= maxRetryCount)
                    throw new TimeoutException($"Service {serviceName} throws exception {exception.Message} after 5 tries", exception);

                tryCount++;

                if (logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning(exception, message: "Encountered an exception while waiting for {ServiceName} to become available",
                                      serviceName);

                await Task.Delay(1000 * tryCount, cancellationToken);
            }
        }

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation(message: "{ServiceName} became available ... ({WatchElapsed})", serviceName, watch.Elapsed);
    }
}
