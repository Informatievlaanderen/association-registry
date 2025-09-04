namespace AssociationRegistry.Admin.ProjectionHost.Projections.Rebuild;

using Elastic.Clients.Elasticsearch;
using Hosts.Configuration.ConfigurationBindings;
using Infrastructure.ConfigurationBindings;
using Infrastructure.ElasticSearch;
using JasperFx.Events.Projections;
using Marten;

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
                ILogger<Program> logger) =>
            {
                await StartRebuild(ProjectionNames.BeheerDetail, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.BeheerHistoriek, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.LocatieLookup, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.LocatieZonderAdresMatch, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.PowerBi, store, shardTimeout, logger);
                await StartRebuild(ProjectionNames.BeheerKboSync, store, shardTimeout, logger);

                await StartRebuild(ProjectionNames.BeheerZoek, store, shardTimeout, logger, async () =>
                {
                    await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, cancellationToken: CancellationToken.None);
                    await elasticClient.CreateVerenigingIndexAsync(options.Indices.Verenigingen);
                });

                await StartRebuild(ProjectionNames.DuplicateDetection, store, shardTimeout, logger, async () =>
                {
                    await elasticClient.Indices.DeleteAsync(options.Indices.DuplicateDetection, cancellationToken: CancellationToken.None);
                    await elasticClient.CreateDuplicateDetectionIndexAsync(options.Indices.DuplicateDetection);
                });

                return Results.Accepted();
            });

        app.MapPost("v1/projections/detail/rebuild", async (IDocumentStore store, ILogger<Program> logger) =>
        {
            await StartRebuild(ProjectionNames.BeheerDetail, store, shardTimeout, logger);
            return Results.Accepted();
        });

        app.MapPost("v1/projections/locaties/lookup/rebuild", async (IDocumentStore store, ILogger<Program> logger) =>
        {
            await StartRebuild(ProjectionNames.LocatieLookup, store, shardTimeout, logger);
            return Results.Accepted();
        });

        app.MapPost("v1/projections/locaties/zonderadresmatch/rebuild", async (IDocumentStore store, ILogger<Program> logger) =>
        {
            await StartRebuild(ProjectionNames.LocatieZonderAdresMatch, store, shardTimeout, logger);
            return Results.Accepted();
        });

        app.MapPost("v1/projections/historiek/rebuild", async (IDocumentStore store, ILogger<Program> logger) =>
        {
            await StartRebuild(ProjectionNames.BeheerHistoriek, store, shardTimeout, logger);
            return Results.Accepted();
        });

        app.MapPost("v1/projections/powerbi/rebuild", async (IDocumentStore store, ILogger<Program> logger) =>
        {
            await StartRebuild(ProjectionNames.PowerBi, store, shardTimeout, logger);
            return Results.Accepted();
        });

        app.MapPost("v1/projections/historiek-kbosync/rebuild", async (IDocumentStore store, ILogger<Program> logger) =>
        {
            await StartRebuild(ProjectionNames.BeheerKboSync, store, shardTimeout, logger);
            return Results.Accepted();
        });

        app.MapPost("v1/projections/search/rebuild", async (
            IDocumentStore store,
            ElasticsearchClient elasticClient,
            ElasticSearchOptionsSection options,
            ILogger<Program> logger) =>
        {
            await StartRebuild(ProjectionNames.BeheerZoek, store, shardTimeout, logger, async () =>
            {
                await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, cancellationToken: CancellationToken.None);
                await elasticClient.CreateVerenigingIndexAsync(options.Indices.Verenigingen);
            });

            return Results.Accepted();
        });

        app.MapPost("v1/projections/duplicatedetection/rebuild", async (
            IDocumentStore store,
            ElasticsearchClient elasticClient,
            ElasticSearchOptionsSection options,
            ILogger<Program> logger) =>
        {
            await StartRebuild(ProjectionNames.DuplicateDetection, store, shardTimeout, logger, async () =>
            {
                await elasticClient.Indices.DeleteAsync(options.Indices.DuplicateDetection, cancellationToken: CancellationToken.None);
                await elasticClient.CreateDuplicateDetectionIndexAsync(options.Indices.DuplicateDetection);
            });

            return Results.Accepted();
        });

        app.MapGet("v1/projections/status", async (
            IDocumentStore store,
            ILogger<Program> _,
            CancellationToken cancellationToken) =>
        {
            return await store.Advanced.AllProjectionProgress(token: cancellationToken);
        });
    }

    private static async Task StartRebuild(
        string projectionName,
        IDocumentStore store,
        TimeSpan shardTimeout,
        ILogger logger,
        Func<Task>? beforeRebuild = null)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var projectionDaemon = await store.BuildProjectionDaemonAsync();

                if (beforeRebuild is not null) await beforeRebuild();
                await projectionDaemon.RebuildProjectionAsync(projectionName, shardTimeout, CancellationToken.None);

                logger.LogInformation("Rebuild {ProjectionName} complete", projectionName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during rebuild {ProjectionName}", projectionName);
                throw;
            }
        });

        logger.LogInformation("Rebuild process {ProjectionName} started", projectionName);
    }
}
