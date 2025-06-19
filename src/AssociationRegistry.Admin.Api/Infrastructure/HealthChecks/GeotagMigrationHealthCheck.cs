namespace AssociationRegistry.Admin.Api.Infrastructure.HealthChecks;

using AssociationRegistry.Admin.Api.HostedServices.GeotagsInitialisation;
using Marten;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class GeotagMigrationHealthCheck : IHealthCheck
{
    public const string Name = "geotags-migration";
    private readonly IDocumentStore _store;

    public GeotagMigrationHealthCheck(IDocumentStore store)
    {
        _store = store;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var session = _store.LightweightSession();
            var ok = await session.Query<GeotagMigration>()
                                  .AnyAsync(x => x.Id == new GeotagMigration().Id, cancellationToken);
            return ok
                ? HealthCheckResult.Healthy("Migration run")
                : HealthCheckResult.Unhealthy("Migration not yet run");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Migration check threw exception", ex);
        }
    }
}
