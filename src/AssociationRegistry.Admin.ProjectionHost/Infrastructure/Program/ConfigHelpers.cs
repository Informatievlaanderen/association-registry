namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program;

using Framework;
using Hosts.Configuration.ConfigurationBindings;

public static class ConfigHelpers
{
    public static void ThrowIfInvalidElasticOptions(ElasticSearchOptionsSection elasticSearchOptions)
    {
        const string sectionName = "ElasticClientOptions";

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(elasticSearchOptions.Uri, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Uri)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(elasticSearchOptions.Indices?.Verenigingen,
                               $"{sectionName}.{nameof(ElasticSearchOptionsSection.Indices)}.{nameof(ElasticSearchOptionsSection.Indices.Verenigingen)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(elasticSearchOptions.Username, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Username)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(elasticSearchOptions.Password, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Password)}");
    }

    public static void ThrowIfInvalidPostgreSqlOptions(PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string sectionName = "PostgreSQLOptions";

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(postgreSqlOptions.Database, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Database)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(postgreSqlOptions.Host, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Host)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(postgreSqlOptions.Username, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Username)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(postgreSqlOptions.Password, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Password)}");
    }

    public static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username}";
}
