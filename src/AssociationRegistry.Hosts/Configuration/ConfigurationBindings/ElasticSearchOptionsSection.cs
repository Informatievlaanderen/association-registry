namespace AssociationRegistry.Hosts.Configuration.ConfigurationBindings;

using DuplicateVerenigingDetection;
using Marten;
using Marten.Schema;
using Microsoft.Extensions.Caching.Memory;

public class ElasticSearchOptionsService: IMinimumScoreDuplicateDetectionConfig
{
    private readonly ElasticSearchOptionsSection _elasticSearchOptionsSection;
    private readonly IDocumentSession _session;
    private readonly IMemoryCache _cache;

    public ElasticSearchOptionsService(ElasticSearchOptionsSection elasticSearchOptionsSection, IDocumentSession session, IMemoryCache cache)
    {
        _elasticSearchOptionsSection = elasticSearchOptionsSection;
        _session = session;
        _cache = cache;
    }

    public double MinimumScoreDuplicateDetection
    {
        get
        {
            if (_cache.TryGetValue(SettingOverrideNames.ElasticSearch.MinimumScoreDuplicateDetection, out double cachedValue))
            {
                return cachedValue;
            }

            var settingOverride = _session.Query<SettingOverride>().FirstOrDefault(x => x.Key == SettingOverrideNames.ElasticSearch.MinimumScoreDuplicateDetection);

            if (!double.TryParse(settingOverride?.Value, out double minScoreOverride))
                return _elasticSearchOptionsSection.MinimumScoreDuplicateDetection;

            return minScoreOverride;
        }
    }
}

public static class SettingOverrideNames
{
    public static class ElasticSearch
    {
        public const string MinimumScoreDuplicateDetection = $"{ElasticSearchOptionsSection.SectionName}:{nameof(ElasticSearchOptionsSection.MinimumScoreDuplicateDetection)}";
    }
}
public record SettingOverride([property: Identity]string Key, string Value);

public class ElasticSearchOptionsSection: IMinimumScoreDuplicateDetectionConfig
{
    public const string SectionName = "ElasticClientOptions";
    public string? Uri { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public IndicesOptionsSection? Indices { get; set; }
    public bool EnableDevelopmentLogs { get; set; }
    public double MinimumScoreDuplicateDetection { get; set; } = MinimumScore.Default.Value;

    public class IndicesOptionsSection
    {
        public string? Verenigingen { get; set; }
        public string? DuplicateDetection { get; set; }
    }
}

public interface IMinimumScoreDuplicateDetectionConfig
{
    double MinimumScoreDuplicateDetection { get; }
}
