namespace AssociationRegistry.Admin.ProjectionHost.Extensions;

using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Marten.Events.Daemon;
using Marten.Events.Daemon.Coordination;
using Microsoft.AspNetCore.Mvc;
using Nest;
using NodaTime;
using Projections;
using Projections.Detail;
using Projections.Historiek;

public static class ProjectionEndpointsExtensions
{
    public static void AddProjectionEndpoints(
        this WebApplication app,
        RebuildConfigurationSection configurationSection,
        CancellationToken cancellationToken)
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

                    await projectionDaemon.StopRebuildStart<BeheerVerenigingDetailProjection>(shardTimeout, null, cancellationToken);
                });

                StartRebuild(logger, projectionName: "Detail Multi", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await projectionDaemon.StopRebuildStart<BeheerVerenigingDetailMultiProjection>(shardTimeout, null, cancellationToken);
                });

                StartRebuild(logger, projectionName: "Locatie Lookup", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await projectionDaemon.StopRebuildStart<LocatieLookupProjection>(shardTimeout, null, cancellationToken);
                });

                StartRebuild(logger, projectionName: "Historiek", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await projectionDaemon.StopRebuildStart<BeheerVerenigingHistoriekProjection>(shardTimeout, null, cancellationToken);
                });

                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await RebuildElasticProjections(projectionDaemon, elasticClient, options.Indices.Verenigingen,
                                                    ProjectionNames.VerenigingZoeken,
                                                    createIndexCallbackAsync: async newIndex
                                                        => await elasticClient.Indices.CreateVerenigingIndexAsync(newIndex),
                                                    shardTimeout,
                                                    cancellationToken);
                });

                StartRebuild(logger, projectionName: "DuplicateDetection", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();

                    await RebuildElasticProjections(projectionDaemon, elasticClient, options.Indices.DuplicateDetection,
                                                    ProjectionNames.DuplicateDetection,
                                                    createIndexCallbackAsync: async newIndex
                                                        => await elasticClient.Indices.CreateDuplicateDetectionIndexAsync(newIndex),
                                                    shardTimeout,
                                                    cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/detail/rebuild",
            handler: async (IProjectionCoordinator coordinator, [FromQuery] int? rewindTo, ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();
                    await projectionDaemon.StopRebuildStart<BeheerVerenigingDetailProjection>(shardTimeout, rewindTo, cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/locaties/lookup/rebuild",
            handler: async (
                IProjectionCoordinator coordinator,
                [FromQuery] int? rewindTo,
                ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Locatie Lookup", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();
                    await projectionDaemon.StopRebuildStart<LocatieLookupProjection>(shardTimeout, rewindTo, cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/historiek/rebuild",
            handler: async (
                IProjectionCoordinator coordinator,
                [FromQuery] int? rewindTo,
                ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Historiek", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();
                    await projectionDaemon.StopRebuildStart<BeheerVerenigingHistoriekProjection>(shardTimeout, rewindTo, cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/search/rebuild",
            handler: async (
                IProjectionCoordinator coordinator,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger,
                CancellationToken cancellationToken) =>
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
                        shardTimeout,
                        cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/duplicatedetection/rebuild",
            handler: async (
                IProjectionCoordinator coordinator,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
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
                        shardTimeout,
                        cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapGet(
            pattern: "v1/projections/status",
            handler: async (IDocumentStore store, ILogger<Program> _, CancellationToken cancellationToken) =>
                await store.Advanced.AllProjectionProgress(token: cancellationToken));
    }

    private static async Task StopRebuildStart<TProjection>(
        this IProjectionDaemon projectionDaemon,
        TimeSpan shardTimeout,
        int? rewindTo,
        CancellationToken cancellationToken)
    {
        var shardName = $"{typeof(TProjection).FullName}:All";

        // await projectionDaemon.StopAgentAsync(shardName);
        //
        if (rewindTo.HasValue)
            await projectionDaemon.RewindSubscriptionAsync(shardName, cancellationToken, rewindTo);
        else
            await projectionDaemon.RebuildProjectionAsync<TProjection>(shardTimeout, cancellationToken);

        // await projectionDaemon.WaitForNonStaleData(TimeSpan.FromSeconds(5));
        //
        // await projectionDaemon.StopAgentAsync(shardName);
        //
        // await projectionDaemon.StartAgentAsync(shardName,
        //                                   cancellationToken);
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
        TimeSpan shardTimeout,
        CancellationToken cancellationToken)
    {
        await projectionDaemon.StopAgentAsync($"{projectionName}:All");

        await elasticClient.Indices.DeleteAsync(indexName, ct: cancellationToken);
        await createIndexCallbackAsync(indexName);

        await projectionDaemon.RebuildProjectionAsync(projectionName, shardTimeout, cancellationToken);

        await projectionDaemon.WaitForNonStaleData(TimeSpan.FromSeconds(5));

        await projectionDaemon.StopAgentAsync($"{projectionName}:All");

        await projectionDaemon.StartAgentAsync($"{projectionName}:All", cancellationToken);
    }
}
