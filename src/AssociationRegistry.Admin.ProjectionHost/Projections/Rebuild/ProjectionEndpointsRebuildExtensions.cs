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
                IDocumentStore store,
                ElasticsearchClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger
            ) =>
            {
                await StartRebuild(BeheerVerenigingDetailProjection.ShardName, store, shardTimeout, logger);
                await StartRebuild(BeheerVerenigingHistoriekProjection.ShardName, store, shardTimeout, logger);
                await StartRebuild(LocatiesGekoppeldMetGrarProjection.ShardName, store, shardTimeout, logger);
                await StartRebuild(LocatieZonderAdresMatchProjection.ShardName, store, shardTimeout, logger);
                await StartRebuild(PowerBiExportProjection.ShardName, store, shardTimeout, logger);
                await StartRebuild(PowerBiExportDubbelDetectieProjection.ShardName, store, shardTimeout, logger);
                await StartRebuild(BeheerKboSyncHistoriekProjection.ShardName, store, shardTimeout, logger);
                await StartRebuild(BeheerKszSyncHistoriekProjection.ShardName, store, shardTimeout, logger);
                await StartRebuild(VertegenwoordigersPerVCodeProjection.ShardName, store, shardTimeout, logger);
                await StartRebuild(BewaartermijnProjection.ShardName, store, shardTimeout, logger);

                await StartRebuild(
                    BeheerZoekProjectionHandler.ShardName,
                    store,
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
                    store,
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
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(BeheerVerenigingDetailProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/bewaartermijn/rebuild",
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(BewaartermijnProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/locaties/gekoppeldmetgrar/rebuild",
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(LocatiesGekoppeldMetGrarProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/locaties/zonderadresmatch/rebuild",
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(LocatieZonderAdresMatchProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/historiek/rebuild",
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(BeheerVerenigingHistoriekProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/powerbi/rebuild",
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(PowerBiExportProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/powerbi-dubbeldetectie/rebuild",
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(PowerBiExportDubbelDetectieProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/kbo-sync-historiek/rebuild",
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(BeheerKboSyncHistoriekProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/ksz-sync-historiek/rebuild",
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(BeheerKszSyncHistoriekProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/vertegenwoordigers/rebuild",
            async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(VertegenwoordigersPerVCodeProjection.ShardName, store, shardTimeout, logger);
                return Results.Accepted();
            }
        );

        app.MapPost(
            "v1/projections/search/rebuild",
            async (
                IDocumentStore store,
                ElasticsearchClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger
            ) =>
            {
                await StartRebuild(
                    BeheerZoekProjectionHandler.ShardName,
                    store,
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
                IDocumentStore store,
                ElasticsearchClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger
            ) =>
            {
                await StartRebuild(
                    DuplicateDetectionProjectionHandler.ShardName,
                    store,
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
        IDocumentStore store,
        TimeSpan shardTimeout,
        ILogger logger,
        Func<Task>? beforeRebuild = null
    )
    {
        var projectionDaemon = await store.BuildProjectionDaemonAsync();
        var projectionName = shardName.Name;
        logger.LogInformation("Rebuild process {ProjectionName} started", projectionName);

        _ = Task.Run(async () =>
        {
            try
            {
                await projectionDaemon.StopAgentAsync(shardName);

                if (beforeRebuild is not null)
                    await beforeRebuild();

                await projectionDaemon.RebuildProjectionAsync(projectionName, shardTimeout, CancellationToken.None);

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
                    await projectionDaemon.StartAgentAsync(shardName, CancellationToken.None);
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
