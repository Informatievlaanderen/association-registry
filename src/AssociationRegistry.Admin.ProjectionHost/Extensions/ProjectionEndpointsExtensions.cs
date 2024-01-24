namespace AssociationRegistry.Admin.ProjectionHost.Extensions;

using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Marten.Events.Daemon;
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
                IDocumentStore store,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
            {
                var projectionDaemon = await store.BuildProjectionDaemonAsync();

                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    await projectionDaemon.RebuildProjection<BeheerVerenigingDetailProjection>(shardTimeout,CancellationToken.None);
                });

                StartRebuild(logger, projectionName: "Historiek", rebuildFunc: async () =>
                {
                    await projectionDaemon.RebuildProjection<BeheerVerenigingHistoriekProjection>(shardTimeout,CancellationToken.None);
                });

                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    await RebuildElasticProjections(projectionDaemon, elasticClient, options, shardTimeout);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/detail/rebuild",
            handler: (IDocumentStore store, ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.RebuildProjection<BeheerVerenigingDetailProjection>(shardTimeout,CancellationToken.None);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/historiek/rebuild",
            handler: (IDocumentStore store, ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Historiek", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.RebuildProjection<BeheerVerenigingHistoriekProjection>(shardTimeout,CancellationToken.None);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/search/rebuild",
            handler: (
                IDocumentStore store,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await RebuildElasticProjections(projectionDaemon, elasticClient, options, shardTimeout);
                });

                return Results.Accepted();
            });

        app.MapGet(
            pattern: "v1/projections/status",
            handler: async (IDocumentStore store, ILogger<Program> _, CancellationToken cancellationToken) =>
                await store.Advanced.AllProjectionProgress(token: cancellationToken));
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
        ElasticSearchOptionsSection options,
        TimeSpan shardTimeout)
    {
        await projectionDaemon.StopShard($"{ProjectionNames.VerenigingZoeken}:All");
        var oldVerenigingenIndices = await elasticClient.GetIndicesPointingToAliasAsync(options.Indices.Verenigingen);
        var newIndicesVerenigingen = options.Indices.Verenigingen + "-" + SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds();
        await elasticClient.Indices.CreateVerenigingIndexAsync(newIndicesVerenigingen).ThrowIfInvalidAsync();

        await elasticClient.Indices.DeleteAsync(options.Indices.DuplicateDetection, ct: CancellationToken.None).ThrowIfInvalidAsync();
        await elasticClient.Indices.CreateDuplicateDetectionIndexAsync(options.Indices.DuplicateDetection).ThrowIfInvalidAsync();
        await projectionDaemon.RebuildProjection(ProjectionNames.VerenigingZoeken,shardTimeout, CancellationToken.None);

        await elasticClient.Indices.PutAliasAsync(newIndicesVerenigingen, options.Indices.Verenigingen, ct: CancellationToken.None);

        foreach (var indeces in oldVerenigingenIndices)
        {
            await elasticClient.Indices.DeleteAsync(indeces, ct: CancellationToken.None).ThrowIfInvalidAsync();
        }

        await projectionDaemon.StartShard($"{ProjectionNames.VerenigingZoeken}:All", CancellationToken.None);
    }
}
