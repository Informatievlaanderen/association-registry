namespace AssociationRegistry.Hosts.Configuration.ConfigurationBindings;

using DuplicateVerenigingDetection;

public class ElasticSearchOptionsSection
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
