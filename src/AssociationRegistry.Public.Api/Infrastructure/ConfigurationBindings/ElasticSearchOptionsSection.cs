namespace AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;

public class ElasticSearchOptionsSection
{
    public string? Uri { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Fingerprint { get; set; }
    public IndicesOptionsSection? Indices { get; set; }
    public bool EnableDevelopmentLogs { get; set; }

    public class IndicesOptionsSection
    {
        public string? Verenigingen { get; set; }
    }
}
