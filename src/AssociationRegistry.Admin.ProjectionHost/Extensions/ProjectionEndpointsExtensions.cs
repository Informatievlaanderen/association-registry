namespace AssociationRegistry.Admin.ProjectionHost.Extensions;

using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Marten.Events.Daemon;
using Nest;
using Projections;
using Projections.Detail;
using Projections.Historiek;
using Projections.Locaties;

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
                await StartRebuild(ProjectionNames.BeheerDetail, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.BeheerDetailMulti, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.BeheerHistoriek, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.LocatieLookup, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.LocatieZonderAdresMatch, store, shardTimeout, logger);

                await StartRebuild(ProjectionNames.BeheerZoek, store, shardTimeout, logger, async () =>
                {
                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, ct: CancellationToken.None);
                    await elasticClient.Indices.CreateVerenigingIndexAsync(options.Indices.Verenigingen);
                });

                await StartRebuild(ProjectionNames.DuplicateDetection, store, shardTimeout, logger, async () =>
                {
                    await elasticClient.Indices.DeleteAsync(options.Indices.DuplicateDetection, ct: CancellationToken.None);
                    await elasticClient.Indices.CreateVerenigingIndexAsync(options.Indices.DuplicateDetection);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/detail/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(ProjectionNames.BeheerDetail, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.BeheerDetailMulti, store, shardTimeout, logger);

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/locaties/lookup/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(ProjectionNames.LocatieLookup, store, shardTimeout, logger);

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/locaties/zonderadresmatch/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(ProjectionNames.LocatieZonderAdresMatch, store, shardTimeout, logger);

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/historiek/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger) =>
            {
                await StartRebuild(ProjectionNames.BeheerHistoriek, store, shardTimeout, logger);

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
                await StartRebuild(ProjectionNames.BeheerZoek, store, shardTimeout, logger, async () =>
                {
                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, ct: CancellationToken.None);
                    await elasticClient.Indices.CreateVerenigingIndexAsync(options.Indices.Verenigingen);
                });

                return Results.Accepted();
            });

        app.MapPost(
            pattern: "v1/projections/duplicatedetection/rebuild",
            handler: async (
                IDocumentStore store,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger) =>
            {
                await StartRebuild(ProjectionNames.DuplicateDetection, store, shardTimeout, logger, async () =>
                {
                    await elasticClient.Indices.DeleteAsync(options.Indices.DuplicateDetection, ct: CancellationToken.None);
                    await elasticClient.Indices.CreateVerenigingIndexAsync(options.Indices.DuplicateDetection);
                });

                return Results.Accepted();
            });

        app.MapGet(
            pattern: "v1/projections/status",
            handler: async (IDocumentStore store, ILogger<Program> _, CancellationToken cancellationToken) =>
                await store.Advanced.AllProjectionProgress(token: cancellationToken));
    }

    private static async Task StartRebuild(
        string projectionName,
        IDocumentStore store,
        TimeSpan shardTimeout,
        ILogger logger,
        Func<Task> beforeRebuild = null)
    {
        try
        {
            var shardState = await store.Advanced.AllProjectionProgress(token: CancellationToken.None);
            var shardName = shardState.Single(s => s.ShardName.Contains(projectionName)).ShardName;
            var projectionDaemon = await store.BuildProjectionDaemonAsync();

            await projectionDaemon.StopAgentAsync(shardName);

            if (beforeRebuild is not null) await beforeRebuild();

            var status = projectionDaemon.StatusFor(shardName);
            await projectionDaemon.RebuildProjectionAsync(projectionName, shardTimeout, CancellationToken.None);
            await projectionDaemon.WaitForNonStaleData(TimeSpan.FromSeconds(5));
            await projectionDaemon.StopAgentAsync(shardName);
            await projectionDaemon.StartAgentAsync(shardName, CancellationToken.None);

            logger.LogInformation(message: "Rebuild {ProjectionName} complete", projectionName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error during rebuild {ProjectionName}", projectionName);

            throw;
        }
    }
}
