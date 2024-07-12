namespace AssociationRegistry.Public.ProjectionHost.Extensions;

using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Marten.Events.Daemon;
using Marten.Events.Daemon.Coordination;
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
                IProjectionCoordinator coordinator,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();
                    await projectionDaemon.StopRebuildStart<PubliekVerenigingDetailProjection>(shardTimeout);
                });

                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    var projectionDaemon = coordinator.DaemonForMainDatabase();
                    await projectionDaemon.StopAgentAsync($"{ProjectionNames.VerenigingZoeken}:All");

                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, ct: CancellationToken.None);
                    await elasticClient.Indices.CreateVerenigingIndexAsync(options.Indices.Verenigingen);

                    await projectionDaemon.RebuildProjectionAsync(ProjectionNames.VerenigingZoeken, shardTimeout, CancellationToken.None);

                    await projectionDaemon.WaitForNonStaleData(TimeSpan.FromSeconds(5));

                    await projectionDaemon.StopAgentAsync($"{ProjectionNames.VerenigingZoeken}:All");

                    await projectionDaemon.StartAgentAsync($"{ProjectionNames.VerenigingZoeken}:All", CancellationToken.None);
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
                    await projectionDaemon.StopRebuildStart<PubliekVerenigingDetailProjection>(shardTimeout);
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
                    await projectionDaemon.StopAgentAsync($"{ProjectionNames.VerenigingZoeken}:All");

                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, ct: CancellationToken.None);
                    await elasticClient.Indices.CreateVerenigingIndexAsync(options.Indices.Verenigingen);

                    await projectionDaemon.RebuildProjectionAsync(ProjectionNames.VerenigingZoeken, shardTimeout, CancellationToken.None);

                    await projectionDaemon.WaitForNonStaleData(TimeSpan.FromSeconds(5));

                    await projectionDaemon.StopAgentAsync($"{ProjectionNames.VerenigingZoeken}:All");

                    await projectionDaemon.StartAgentAsync($"{ProjectionNames.VerenigingZoeken}:All", CancellationToken.None);
                });

                return Results.Accepted();
            });

        app.MapGet(
            pattern: "v1/projections/status", handler: (IDocumentStore store, ILogger<Program> _, CancellationToken cancellationToken)
                => store.Advanced.AllProjectionProgress(token: cancellationToken));
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
}
