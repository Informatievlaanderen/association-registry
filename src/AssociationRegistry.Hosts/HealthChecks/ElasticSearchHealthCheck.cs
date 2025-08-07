namespace AssociationRegistry.Hosts.HealthChecks;

using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using HealthStatus = Elastic.Clients.Elasticsearch.HealthStatus;

public class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchHealthCheck> _logger;

    public ElasticsearchHealthCheck(ElasticsearchClient client, ILogger<ElasticsearchHealthCheck> logger)
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
            var health = await _client.Cluster.HealthAsync(cancellationToken: cancellationToken);

            if (!health.IsValidResponse)
            {
                // Log detailed error internally but don't expose it
                _logger.LogError("Elasticsearch health check failed: {Error}",
                                 health.ElasticsearchServerError?.ToString());

                return HealthCheckResult.Unhealthy("Elasticsearch is unavailable");
            }

            return health.Status switch
            {
                HealthStatus.Green => HealthCheckResult.Healthy("Elasticsearch is healthy"),
                HealthStatus.Yellow => HealthCheckResult.Degraded("Elasticsearch is degraded"),
                HealthStatus.Red => HealthCheckResult.Unhealthy("Elasticsearch is unhealthy"),
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
