namespace AssociationRegistry.Admin.ProjectionHost.Extensions;

using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Marten.Events.Daemon;
using Marten.Events.Daemon.Coordination;
using Nest;
using NodaTime;
using Projections;
using Projections.Detail;
using Projections.Historiek;

public static class ProjectionEndpointsExtensions
{
    public static void AddProjectionEndpoints(this WebApplication app, RebuildConfigurationSection configurationSection)
    {
        var shardTimeout = TimeSpan.FromMinutes(configurationSection.TimeoutInMinutes);

        app.MapPost(
            pattern: "v1/projections/all/rebuild",
            handler: async (
                IProjectionCoordinator coordinator,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await projectionDaemon.StopRebuildStart<BeheerVerenigingDetailProjection>(shardTimeout);
                });

                StartRebuild(logger, projectionName: "Detail Multi", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await projectionDaemon.StopRebuildStart<BeheerVerenigingDetailMultiProjection>(shardTimeout);
                });

                StartRebuild(logger, projectionName: "Locatie Lookup", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await projectionDaemon.StopRebuildStart<LocatieLookupProjection>(shardTimeout);
                });

                StartRebuild(logger, projectionName: "Historiek", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await projectionDaemon.StopRebuildStart<BeheerVerenigingHistoriekProjection>(shardTimeout);
                });

                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await RebuildElasticProjections(projectionDaemon, elasticClient, options.Indices.Verenigingen,
                                                    ProjectionNames.VerenigingZoeken,
                                                    createIndexCallbackAsync: async newIndex
                                                        => await elasticClient.Indices.CreateVerenigingIndexAsync(newIndex), shardTimeout);
                });

                StartRebuild(logger, projectionName: "DuplicateDetection", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await RebuildElasticProjections(projectionDaemon, elasticClient, options.Indices.DuplicateDetection,
                                                    ProjectionNames.DuplicateDetection,
                                                    createIndexCallbackAsync: async newIndex
                                                        => await elasticClient.Indices.CreateDuplicateDetectionIndexAsync(newIndex),
                                                    shardTimeout);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/detail/rebuild",
            handler: async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();
                    await projectionDaemon.StopRebuildStart<BeheerVerenigingDetailProjection>(shardTimeout);
                });

                StartRebuild(logger, projectionName: "Detail Multi", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();
                    await projectionDaemon.StopRebuildStart<BeheerVerenigingDetailMultiProjection>(shardTimeout);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/locaties/lookup/rebuild",
            handler: async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Locatie Lookup", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();
                    await projectionDaemon.StopRebuildStart<LocatieLookupProjection>(shardTimeout);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/historiek/rebuild",
            handler: async (IProjectionCoordinator coordinator, ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Historiek", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();
                    await projectionDaemon.StopRebuildStart<BeheerVerenigingHistoriekProjection>(shardTimeout);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/search/rebuild",
            handler: async (
                IProjectionCoordinator coordinator,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await RebuildElasticProjections(
                        projectionDaemon,
                        elasticClient,
                        options.Indices.Verenigingen,
                        ProjectionNames.VerenigingZoeken,
                        createIndexCallbackAsync: async newIndex => await elasticClient.Indices.CreateVerenigingIndexAsync(newIndex),
                        shardTimeout);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/duplicatedetection/rebuild",
            handler: async(IProjectionCoordinator coordinator,
                           IElasticClient elasticClient,
                           ElasticSearchOptionsSection options,
                           ILogger < Program > logger) =>

        {
            StartRebuild(logger, projectionName: "DuplicateDetection", rebuildFunc: async () =>
            {
                var projectionDaemon = coordinator.DaemonForMainDatabase();

                await RebuildElasticProjections(
                    projectionDaemon,
                    elasticClient,
                    options.Indices.DuplicateDetection,
                    ProjectionNames.DuplicateDetection,
                    createIndexCallbackAsync: async newIndex
                        => await elasticClient.Indices.CreateDuplicateDetectionIndexAsync(newIndex),
                    shardTimeout);
            });

            return Results.Accepted();
        });

        app.MapGet(
            pattern: "v1/projections/status",
            handler: async(IDocumentStore store, ILogger < Program > _, CancellationToken cancellationToken) =>

        await store.Advanced.AllProjectionProgress(token: cancellationToken));
    }

    private static async Task StopRebuildStart<TProjection>(this IProjectionDaemon projectionDaemon, TimeSpan shardTimeout)
    {
        await projectionDaemon.StopAgentAsync($"{typeof(TProjection).FullName}:All");
        await projectionDaemon.RebuildProjectionAsync<TProjection>(shardTimeout, CancellationToken.None);

        await projectionDaemon.WaitForNonStaleData(TimeSpan.FromSeconds(5));

        await projectionDaemon.StopAgentAsync($"{typeof(TProjection).FullName}:All");

        await projectionDaemon.StartAgentAsync($"{typeof(TProjection).FullName}:All",
                                          CancellationToken.None);
    }

    private static void StartRebuild(ILogger logger, string projectionName, Func<Task> rebuildFunc)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await rebuildFunc();
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

    private static async Task RebuildElasticProjections(
        IProjectionDaemon projectionDaemon,
        IElasticClient elasticClient,
        string indexName,
        string projectionName,
        Func<IndexName, Task> createIndexCallbackAsync,
        TimeSpan shardTimeout)
    {
        await projectionDaemon.StopAgentAsync($"{projectionName}:All");

        await elasticClient.Indices.DeleteAsync(indexName, ct: CancellationToken.None);
        await createIndexCallbackAsync(indexName);

        await projectionDaemon.RebuildProjectionAsync(projectionName, shardTimeout, CancellationToken.None);

        await projectionDaemon.WaitForNonStaleData(TimeSpan.FromSeconds(5));

        await projectionDaemon.StopAgentAsync($"{projectionName}:All");

        await projectionDaemon.StartAgentAsync($"{projectionName}:All", CancellationToken.None);
    }
}
