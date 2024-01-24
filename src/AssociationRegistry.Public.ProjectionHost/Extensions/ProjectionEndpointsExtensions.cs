namespace AssociationRegistry.Public.ProjectionHost.Extensions;

using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Nest;
using Projections;
using Projections.Detail;

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
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.RebuildProjection<PubliekVerenigingDetailProjection>(shardTimeout, CancellationToken.None);
                });

                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.StopShard($"{ProjectionNames.VerenigingZoeken}:All");

                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, ct: CancellationToken.None);
                    await elasticClient.Indices.CreateVerenigingIndexAsync(options.Indices.Verenigingen);

                    await projectionDaemon.RebuildProjection(ProjectionNames.VerenigingZoeken, shardTimeout, CancellationToken.None);
                    await projectionDaemon.StartShard($"{ProjectionNames.VerenigingZoeken}:All", CancellationToken.None);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/detail/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.RebuildProjection<PubliekVerenigingDetailProjection>(shardTimeout, CancellationToken.None);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/search/rebuild",
            handler: async (
                IDocumentStore store,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.StopShard($"{ProjectionNames.VerenigingZoeken}:All");

                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, ct: CancellationToken.None);
                    await elasticClient.Indices.CreateVerenigingIndexAsync(options.Indices.Verenigingen);

                    await projectionDaemon.RebuildProjection(ProjectionNames.VerenigingZoeken, shardTimeout, CancellationToken.None);
                    await projectionDaemon.StartShard($"{ProjectionNames.VerenigingZoeken}:All", CancellationToken.None);
                });

                return Results.Accepted();
            });

        app.MapGet(
            pattern: "v1/projections/status", handler: (IDocumentStore store, ILogger<Program> _, CancellationToken cancellationToken)
                => store.Advanced.AllProjectionProgress(token: cancellationToken));
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
}
