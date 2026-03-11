namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using Marten;
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
                    StartRebuild(
                        logger,
                        projectionName: VerenigingenPerInszProjection.ShardName.Name,
                        rebuildFunc: async () =>
                        {
                            var projectionDaemon = await store.BuildProjectionDaemonAsync();
                            await projectionDaemon.RebuildProjectionAsync<VerenigingenPerInszProjection>(
                                shardTimeout,
                                CancellationToken.None
                            );
                        }
                    );

                    return Results.Accepted();
                }
            )
            .RequireAuthorization(Program.SuperAdminPolicyName)
            .ExcludeFromDescription();
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
            }
        });

        logger.LogInformation(message: "Rebuild process {ProjectionName} started", projectionName);
    }
}
