namespace AssociationRegistry.Admin.ProjectionHost.Projections.Rebuild;

using Bewaartermijn;
using Detail;
using Elastic.Clients.Elasticsearch;
using Historiek;
using Hosts.Configuration.ConfigurationBindings;
using Infrastructure.ConfigurationBindings;
using Infrastructure.ElasticSearch;
using JasperFx.Events.Projections;
using Locaties;
using Marten;
using Marten.Events.Daemon.Coordination;
using PowerBiExport;
using Search.DuplicateDetection;
using Search.Zoeken;
using Sync;
using Vertegenwoordiger;

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
                await StartRebuild(BeheerVerenigingDetailProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(BeheerVerenigingHistoriekProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(LocatiesGekoppeldMetGrarProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(LocatieZonderAdresMatchProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(PowerBiExportProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(PowerBiExportDubbelDetectieProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(BeheerKboSyncHistoriekProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(BeheerKszSyncHistoriekProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(VertegenwoordigersPerVCodeProjection.ShardName, coordinator, shardTimeout, logger);
                await StartRebuild(BewaartermijnProjection.ShardName, coordinator, shardTimeout, logger);

                await StartRebuild(
                    BeheerZoekProjectionHandler.ShardName,
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

                await StartRebuild(
                    DuplicateDetectionProjectionHandler.ShardName,
                    coordinator,
                    shardTimeout,
                    logger,
                    async () =>
                    {
                        await elasticClient.Indices.DeleteAsync(
                            options.Indices.DuplicateDetection,
                            cancellationToken: CancellationToken.None
                        );

                        await elasticClient.CreateDuplicateDetectionIndexAsync(options.Indices.DuplicateDetection);
                    }
                );

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/detail/rebuild",
            async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(BeheerVerenigingDetailProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/locaties/gekoppeldmetgrar/rebuild",
            async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(LocatiesGekoppeldMetGrarProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/locaties/zonderadresmatch/rebuild",
            async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(LocatieZonderAdresMatchProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/historiek/rebuild",
            async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(BeheerVerenigingHistoriekProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/powerbi/rebuild",
            async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(PowerBiExportProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/powerbi-dubbeldetectie/rebuild",
            async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(PowerBiExportDubbelDetectieProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/kbo-sync-historiek/rebuild",
            async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(BeheerKboSyncHistoriekProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/ksz-sync-historiek/rebuild",
            async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(BeheerKszSyncHistoriekProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/vertegenwoordigers/rebuild",
            async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                await StartRebuild(VertegenwoordigersPerVCodeProjection.ShardName, coordinator, shardTimeout, logger);

                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/search/rebuild",
            async (
                IProjectionCoordinator coordinator,
                ElasticsearchClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger
            ) =>
            {
                await StartRebuild(
                    BeheerZoekProjectionHandler.ShardName,
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
            "v1/projections/duplicatedetection/rebuild",
            async (
                IProjectionCoordinator coordinator,
                ElasticsearchClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger
            ) =>
            {
                await StartRebuild(
                    DuplicateDetectionProjectionHandler.ShardName,
                    coordinator,
                    shardTimeout,
                    logger,
                    async () =>
                    {
                        await elasticClient.Indices.DeleteAsync(
                            options.Indices.DuplicateDetection,
                            cancellationToken: CancellationToken.None
                        );

                        await elasticClient.CreateDuplicateDetectionIndexAsync(options.Indices.DuplicateDetection);
                    }
                );

                return Results.Accepted();
            }
        );

        app.MapGet(
            "v1/projections/status",
            async (IDocumentStore store, ILogger<Program> _, CancellationToken cancellationToken) =>
            {
                return await store.Advanced.AllProjectionProgress(token: cancellationToken);
            }
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
