namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using ConfigurationBindings;
using Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static PostgreSqlOptionsSection GetValidPostgreSqlOptionsOrThrow(this ConfigurationManager source)
    {
        var postgreSqlOptions = source.GetSection(PostgreSqlOptionsSection.Name)
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
