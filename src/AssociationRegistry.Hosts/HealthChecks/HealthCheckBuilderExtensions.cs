namespace AssociationRegistry.Hosts.HealthChecks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public static class HealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddElasticsearchHealthCheck(
        this IHealthChecksBuilder builder)
        => builder.AddCheck<ElasticsearchHealthCheck>(ElasticsearchHealthCheck.Name);
}
