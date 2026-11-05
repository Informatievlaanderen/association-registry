namespace AssociationRegistry.Hosts.Configuration.ConfigurationBindings;

using DecentraalBeheer.Vereniging.DubbelDetectie;
using JasperFx;
using Marten;
using Marten.Schema;
using Microsoft.Extensions.Caching.Memory;

public class ElasticSearchOptionsService : IMinimumScoreDuplicateDetectionConfig
{
    private readonly ElasticSearchOptionsSection _elasticSearchOptionsSection;
    private readonly IDocumentSession _session;
    private readonly IMemoryCache _cache;

    public ElasticSearchOptionsService(
        ElasticSearchOptionsSection elasticSearchOptionsSection,
        IDocumentSession session,
        IMemoryCache cache
    )
    {
        _elasticSearchOptionsSection = elasticSearchOptionsSection;
        _session = session;
        _cache = cache;
    }

    public async Task<MinimumScore> GetMinimumScoreDuplicateDetection(CancellationToken cancellationToken = default)
    {
        if (
            _cache.TryGetValue(
                SettingOverrideNames.ElasticSearch.MinimumScoreDuplicateDetection,
                out double cachedValue
            )
        )
        {
            return new MinimumScore(cachedValue);
        }

        var settingOverride = await _session
            .Query<SettingOverride>()
            .FirstOrDefaultAsync(
                x => x.Key == SettingOverrideNames.ElasticSearch.MinimumScoreDuplicateDetection,
                cancellationToken
            );

        if (!double.TryParse(settingOverride?.Value, out double minScoreOverride))
            return await _elasticSearchOptionsSection.GetMinimumScoreDuplicateDetection(cancellationToken);

        return new MinimumScore(minScoreOverride);
    }
}

public static class SettingOverrideNames
{
    public static class ElasticSearch
    {
        public const string MinimumScoreDuplicateDetection =
            $"{ElasticSearchOptionsSection.SectionName}:{nameof(ElasticSearchOptionsSection.MinimumScoreDuplicateDetection)}";
    }
}

public record SettingOverride([property: Identity] string Key, string Value);

public class ElasticSearchOptionsSection : IMinimumScoreDuplicateDetectionConfig
{
    public const string SectionName = "ElasticClientOptions";
    public string? Uri { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public IndicesOptionsSection? Indices { get; set; }
    public bool EnableDevelopmentLogs { get; set; }
    public double MinimumScoreDuplicateDetection { get; set; } = MinimumScore.Default.Value;
    public int RequestTimeoutInSeconds { get; set; } = 30;
    public int MaximumRetries { get; set; } = 1;
    public int MaxRetryTimeoutInSeconds { get; set; } = 60;
    public int PooledConnectionIdleTimeoutInSeconds { get; set; } = 30;
    public int PooledConnectionLifetimeInSeconds { get; set; } = 300;

    public class IndicesOptionsSection
    {
        public string? Verenigingen { get; set; }
        public string? DuplicateDetection { get; set; }
    }

    public Task<MinimumScore> GetMinimumScoreDuplicateDetection(CancellationToken cancellationToken = default) =>
        Task.FromResult(new MinimumScore(MinimumScoreDuplicateDetection));
}

public interface IMinimumScoreDuplicateDetectionConfig
{
    Task<MinimumScore> GetMinimumScoreDuplicateDetection(CancellationToken cancellationToken = default);
}
