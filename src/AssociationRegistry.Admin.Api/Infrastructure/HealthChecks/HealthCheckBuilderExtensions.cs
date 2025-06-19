namespace AssociationRegistry.Admin.Api.Infrastructure.HealthChecks;

using Hosts.HealthChecks;

public static class HealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddGeotagsMigrationHealthCheck(
        this IHealthChecksBuilder builder)
        => builder.AddCheck<GeotagMigrationHealthCheck>(GeotagMigrationHealthCheck.Name);
}
