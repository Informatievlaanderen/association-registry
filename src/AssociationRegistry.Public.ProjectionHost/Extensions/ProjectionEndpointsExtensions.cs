namespace AssociationRegistry.Public.ProjectionHost.Extensions;

using Hosts.Configuration.ConfigurationBindings;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Elastic.Clients.Elasticsearch;
using Projections;

public static class ProjectionEndpointsExtensions
{
    public static void AddProjectionEndpoints(this WebApplication app, RebuildConfigurationSection configurationSection)
    {
        var shardTimeout = TimeSpan.FromMinutes(configurationSection.TimeoutInMinutes);

        app.MapPost(
            pattern: "v1/projections/all/rebuild",
            handler: async (
                IDocumentStore store,
                ElasticsearchClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
            {
                await StartRebuild(ProjectionNames.PubliekDetail, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.PubliekZoek, store, shardTimeout, logger, async () =>
                {
                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, cancellationToken: CancellationToken.None);
                    await elasticClient.CreateVerenigingIndexAsync(options.Indices.Verenigingen);
                });

                await StartRebuild(ProjectionNames.PubliekSequence, store, shardTimeout, logger);

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/detail/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(ProjectionNames.PubliekDetail, store, shardTimeout, logger);

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/search/rebuild",
            handler: async (
                IDocumentStore store,
                ElasticsearchClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
            {

                await StartRebuild(ProjectionNames.PubliekZoek, store, shardTimeout, logger, async () =>
                {
                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, cancellationToken: CancellationToken.None);
                    await elasticClient.CreateVerenigingIndexAsync(options.Indices.Verenigingen);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/sequence/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(ProjectionNames.PubliekSequence, store, shardTimeout, logger);

                return Results.Accepted();
            });

        app.MapGet(
            pattern: "v1/projections/status", handler: (IDocumentStore store, ILogger<Program> _, CancellationToken cancellationToken)
                => store.Advanced.AllProjectionProgress(token: cancellationToken));
    }

    private static async Task StartRebuild(
        string projectionName,
        IDocumentStore store,
        TimeSpan shardTimeout,
        ILogger logger,
        Func<Task> beforeRebuild = null)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var shardState = await store.Advanced.AllProjectionProgress(token: CancellationToken.None);
                var shardName = shardState.Single(s => s.ShardName.Contains(projectionName)).ShardName;
                var projectionDaemon = await store.BuildProjectionDaemonAsync();

                await projectionDaemon.StopAgentAsync(shardName);
                if (beforeRebuild is not null) await beforeRebuild();
                await projectionDaemon.RebuildProjectionAsync(projectionName, shardTimeout, CancellationToken.None);
                await projectionDaemon.WaitForNonStaleData(TimeSpan.FromSeconds(5));
                await projectionDaemon.StopAgentAsync(shardName);
                await projectionDaemon.StartAgentAsync(shardName, CancellationToken.None);

                logger.LogInformation(message: "Rebuild {ProjectionName} complete", projectionName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, message: "Error during rebuild {ProjectionName}", projectionName);

                throw;
            }
        });

        logger.LogInformation(message: "Rebuild process {ProjectionName} started", projectionName);
    }
}
