namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.ConfigurationBindings;

public class ElasticSearchOptionsSection
{
    public string? Uri { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public IndicesOptionsSection? Indices { get; set; }

    public class IndicesOptionsSection
    {
        public string? Verenigingen { get; set; }
    }
}
