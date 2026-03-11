namespace AssociationRegistry.Public.ProjectionHost.Extensions;

using Elastic.Clients.Elasticsearch;
using Hosts.Configuration.ConfigurationBindings;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Daemon.Coordination;
using Projections;
using Projections.Detail;
using Projections.Search;
using Projections.Sequence;

public static class ProjectionEndpointsExtensions
{
    public static void AddProjectionEndpoints(this WebApplication app, RebuildConfigurationSection configurationSection)
    {
        var shardTimeout = TimeSpan.FromMinutes(configurationSection.TimeoutInMinutes);

        app.MapPost(
            pattern: "v1/projections/all/rebuild",
            handler: async (
                IProjectionCoordinator coordinator,
                ElasticsearchClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger
            ) =>
            {
                await StartRebuild(PubliekVerenigingDetailProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(
                    PubliekZoekProjectionHandler.ShardName,
                    coordinator,
                    shardTimeout,
                    logger,
                    async () =>
                    {
                        await elasticClient.Indices.DeleteAsync(
                            options.Indices.Verenigingen,
                            cancellationToken: CancellationToken.None
                        );
                        await elasticClient.CreateVerenigingIndexAsync(options.Indices.Verenigingen);
                    }
                );

                await StartRebuild(PubliekVerenigingSequenceProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            pattern: "v1/projections/detail/rebuild",
            handler: async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(PubliekVerenigingDetailProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            pattern: "v1/projections/search/rebuild",
            handler: async (
                IProjectionCoordinator coordinator,
                ElasticsearchClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger
            ) =>
            {
                await StartRebuild(
                    PubliekZoekProjectionHandler.ShardName,
                    coordinator,
                    shardTimeout,
                    logger,
                    async () =>
                    {
                        await elasticClient.Indices.DeleteAsync(
                            options.Indices.Verenigingen,
                            cancellationToken: CancellationToken.None
                        );
                        await elasticClient.CreateVerenigingIndexAsync(options.Indices.Verenigingen);
                    }
                );

                return Results.Accepted();
            }
        );

        app.MapPost(
            pattern: "v1/projections/sequence/rebuild",
            handler: async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(PubliekVerenigingSequenceProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapGet(
            pattern: "v1/projections/status",
            handler: (IDocumentStore store, ILogger<Program> _, CancellationToken cancellationToken) =>
                store.Advanced.AllProjectionProgress(token: cancellationToken)
        );
    }

    private static async Task StartRebuild(
        ShardName shardName,
        IProjectionCoordinator coordinator,
        TimeSpan shardTimeout,
        ILogger logger,
        Func<Task>? beforeRebuild = null
    )
    {
        var projectionName = shardName.Name;
        var daemon = coordinator.DaemonForMainDatabase();
        logger.LogInformation("Rebuild process {ProjectionName} started", projectionName);

        _ = Task.Run(async () =>
        {
            try
            {
                await daemon.StopAgentAsync(shardName);
                logger.LogInformation("Rebuild {ProjectionName} agent stopped", projectionName);

                if (beforeRebuild is not null)
                    await beforeRebuild();

                await daemon.RebuildProjectionAsync(projectionName, shardTimeout, CancellationToken.None);

                logger.LogInformation("Rebuild {ProjectionName} complete", projectionName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during rebuild {ProjectionName}", projectionName);
            }
            finally
            {
                try
                {
                    await daemon.StartAgentAsync(shardName, CancellationToken.None);
                    logger.LogInformation("Rebuild {ProjectionName} agent started", projectionName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to restart shard {ProjectionName}", projectionName);
                }
            }
        });
    }
}
