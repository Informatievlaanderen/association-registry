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
    public static void AddProjectionEndpoints(this WebApplication app)
    {
        app.MapPost(
            pattern: "v1/projections/all/rebuild",
            handler: async (
                IDocumentStore store,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger,
                CancellationToken cancellationToken) =>
            {
                var projectionDaemon = await store.BuildProjectionDaemonAsync();

                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    await projectionDaemon.RebuildProjection<BeheerVerenigingDetailProjection>(cancellationToken);
                });

                StartRebuild(logger, projectionName: "Historiek", rebuildFunc: async () =>
                {
                    await projectionDaemon.RebuildProjection<BeheerVerenigingHistoriekProjection>(cancellationToken);
                });

                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    await RebuildElasticProjections(projectionDaemon, elasticClient, options, cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/detail/rebuild",
            handler: (IDocumentStore store, ILogger<Program> logger, CancellationToken cancellationToken) =>
            {
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.RebuildProjection<BeheerVerenigingDetailProjection>(cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/historiek/rebuild",
            handler: (IDocumentStore store, ILogger<Program> logger, CancellationToken cancellationToken) =>
            {
                StartRebuild(logger, projectionName: "Historiek", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.RebuildProjection<BeheerVerenigingHistoriekProjection>(cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/search/rebuild",
            handler: (
                IDocumentStore store,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger,
                CancellationToken cancellationToken) =>
            {
                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await RebuildElasticProjections(projectionDaemon, elasticClient, options, cancellationToken);
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
        CancellationToken cancellationToken)
    {
        await projectionDaemon.StopShard($"{ProjectionNames.VerenigingZoeken}:All");
        var oldVerenigingenIndices = await elasticClient.GetIndicesPointingToAliasAsync(options.Indices.Verenigingen);
        var newIndicesVerenigingen = options.Indices.Verenigingen + "-" + SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds();
        await elasticClient.Indices.CreateVerenigingIndexAsync(newIndicesVerenigingen).ThrowIfInvalidAsync();

        await elasticClient.Indices.DeleteAsync(options.Indices.DuplicateDetection, ct: cancellationToken).ThrowIfInvalidAsync();
        await elasticClient.Indices.CreateDuplicateDetectionIndexAsync(options.Indices.DuplicateDetection).ThrowIfInvalidAsync();
        await projectionDaemon.RebuildProjection(ProjectionNames.VerenigingZoeken, cancellationToken);

        await elasticClient.Indices.PutAliasAsync(newIndicesVerenigingen, options.Indices.Verenigingen, ct: cancellationToken);

        foreach (var indeces in oldVerenigingenIndices)
        {
            await elasticClient.Indices.DeleteAsync(indeces, ct: cancellationToken).ThrowIfInvalidAsync();
        }
    }
}
