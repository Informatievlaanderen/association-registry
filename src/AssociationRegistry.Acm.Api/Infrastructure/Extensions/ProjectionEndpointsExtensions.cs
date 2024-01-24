namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using Api;
using Configuration;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Projections;
using System;
using System.Threading;
using System.Threading.Tasks;

public static class ProjectionEndpointsExtensions
{
    //TODO toevoegen aan program.cs om endpoints te activeren
    public static void AddProjectionEndpoints(this WebApplication app, RebuildConfigurationSection configurationSection)
    {
        var shardTimeout = TimeSpan.FromMinutes(configurationSection.TimeoutInMinutes);

        app.MapPost(
            pattern: "v1/projections/verenigingperinsz/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger) =>
            {
                StartRebuild(logger, projectionName: "Detail", rebuildFunc: async () =>
                {
                    var projectionDaemon = await store.BuildProjectionDaemonAsync();
                    await projectionDaemon.RebuildProjection<VerenigingenPerInszProjection>(shardTimeout,CancellationToken.None);
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
