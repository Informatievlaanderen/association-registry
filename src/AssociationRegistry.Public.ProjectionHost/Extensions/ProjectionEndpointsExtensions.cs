namespace AssociationRegistry.Public.ProjectionHost.Extensions;

using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Nest;
using Projections;
using Projections.Detail;

public static class ProjectionEndpointsExtensions
{
    public static void AddProjectionEndpoints(this WebApplication app)
    {
        app.MapPost(
            pattern: "v1/projections/detail/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger, CancellationToken cancellationToken) =>
            {
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.RebuildProjection<PubliekVerenigingDetailProjection>(cancellationToken);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/search/rebuild",
            handler: async (
                IDocumentStore store,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger,
                CancellationToken cancellationToken) =>
            {
                StartRebuild(logger, projectionName: "Search", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.StopShard($"{ProjectionNames.VerenigingZoeken}:All");

                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, ct: cancellationToken);
                    await elasticClient.Indices.CreateVerenigingIndex(options.Indices.Verenigingen);

                    await projectionDaemon.RebuildProjection(ProjectionNames.VerenigingZoeken, cancellationToken);
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
