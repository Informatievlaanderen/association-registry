namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Hosts.Configuration.ConfigurationBindings;

public static class ConfigurationExtensions
{
    public static PostgreSqlOptionsSection GetValidPostgreSqlOptionsOrThrow(this ConfigurationManager source)
    {
        var postgreSqlOptions = source.GetSection(PostgreSqlOptionsSection.SectionName)
                                      .Get<PostgreSqlOptionsSection>();

        ConfigHelpers.ThrowIfInvalidPostgreSqlOptions(postgreSqlOptions);

        return postgreSqlOptions;
    }

    public static ElasticSearchOptionsSection GetValidElasticSearchOptionsOrThrow(this ConfigurationManager source)
    {
        var elasticSearchOptionsSection = source.GetSection("ElasticClientOptions")
                                                .Get<ElasticSearchOptionsSection>();

        ConfigHelpers.ThrowIfInvalidElasticOptions(elasticSearchOptionsSection);

        return elasticSearchOptionsSection;
    }
}
