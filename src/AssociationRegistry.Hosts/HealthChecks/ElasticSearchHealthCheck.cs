namespace AssociationRegistry.Hosts.HealthChecks;

using Configuration.ConfigurationBindings;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Nest;


public class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly IElasticClient _client;
    private readonly ILogger<ElasticsearchHealthCheck> _logger;

    public ElasticsearchHealthCheck(IElasticClient client, ILogger<ElasticsearchHealthCheck> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var health = await _client.Cluster.HealthAsync(ct: cancellationToken);

            if (!health.IsValid)
            {
                // Log detailed error internally but don't expose it
                _logger.LogError("Elasticsearch health check failed: {Error}",
                                 health.ServerError?.ToString());

                return HealthCheckResult.Unhealthy("Elasticsearch is unavailable");
            }

            return health.Status switch
            {
                Health.Green => HealthCheckResult.Healthy("Elasticsearch is healthy"),
                Health.Yellow => HealthCheckResult.Degraded("Elasticsearch is degraded"),
                Health.Red => HealthCheckResult.Unhealthy("Elasticsearch is unhealthy"),
                _ => HealthCheckResult.Unhealthy("Elasticsearch status unknown")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Elasticsearch health check failed with exception");

            return HealthCheckResult.Unhealthy("Elasticsearch is unavailable");
        }
    }
    public const string Name = "ElasticSearch";



}
