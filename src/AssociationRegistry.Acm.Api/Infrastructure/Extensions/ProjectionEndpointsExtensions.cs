namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Daemon.Coordination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Projections;

public static class ProjectionEndpointsExtensions
{
    public static void AddProjectionEndpoints(this WebApplication app, RebuildConfigurationSection configurationSection)
    {
        var shardTimeout = TimeSpan.FromSeconds(configurationSection.TimeoutInSeconds);

        app.MapPost(
                pattern: "v1/projections/verenigingperinsz/rebuild",
                handler: async (IDocumentStore store, ILogger<Program> logger) =>
                {
                    await StartRebuild(shardName: VerenigingenPerInszProjection.ShardName, store, shardTimeout, logger);

                    return Results.Accepted();
                }
            )
            .RequireAuthorization(Program.SuperAdminPolicyName)
            .ExcludeFromDescription();
    }

    private static async Task StartRebuild(
        ShardName shardName,
        IDocumentStore store,
        TimeSpan shardTimeout,
        ILogger logger,
        Func<Task>? beforeRebuild = null
    )
    {
        var projectionDaemon = await store.BuildProjectionDaemonAsync();
        var projectionName = shardName.Name;
        logger.LogInformation("Rebuild process {ProjectionName} started", projectionName);

        _ = Task.Run(async () =>
        {
            try
            {
                await projectionDaemon.StopAgentAsync(shardName);

                if (beforeRebuild is not null)
                    await beforeRebuild();

                await projectionDaemon.RebuildProjectionAsync(projectionName, shardTimeout, CancellationToken.None);

                logger.LogInformation("Rebuild {ProjectionName} complete", projectionName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during rebuild {ProjectionName}", projectionName);
            }
            finally
            {
                try
                {
                    await projectionDaemon.StartAgentAsync(shardName, CancellationToken.None);
                    logger.LogInformation("Rebuild {ProjectionName} agent started", projectionName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to restart shard {ProjectionName}", projectionName);
                }
            }
        });
    }
}
