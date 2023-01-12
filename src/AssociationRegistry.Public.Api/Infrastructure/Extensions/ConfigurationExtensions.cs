namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using System;
using Framework;
using ConfigurationBindings;
using Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static PostgreSqlOptionsSection GetPostgreSqlOptionsSection(this IConfiguration configuration)
    {
        var postgreSqlOptionsSection = configuration
            .GetSection(PostgreSqlOptionsSection.Name)
            .Get<PostgreSqlOptionsSection>();
        postgreSqlOptionsSection.ThrowIfInvalid();
        return postgreSqlOptionsSection;
    }

    private static void ThrowIfInvalid(this PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string sectionName = nameof(PostgreSqlOptionsSection);
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Database, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Database)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Host, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Host)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Username, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Username)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Password, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Password)}");
    }

    public static ElasticSearchOptionsSection GetElasticSearchOptionsSection(this IConfiguration configuration)
    {
        var elasticSearchOptions = configuration
            .GetSection("ElasticClientOptions")
            .Get<ElasticSearchOptionsSection>();

        elasticSearchOptions.ThrowIfInvalid();
        return elasticSearchOptions;
    }

    private static void ThrowIfInvalid(this ElasticSearchOptionsSection elasticSearchOptions)
    {
        const string sectionName = nameof(ElasticSearchOptionsSection);
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(elasticSearchOptions.Uri, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Uri)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(elasticSearchOptions.Indices?.Verenigingen, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Indices)}.{nameof(ElasticSearchOptionsSection.Indices.Verenigingen)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(elasticSearchOptions.Username, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Username)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(elasticSearchOptions.Password, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Password)}");
    }


    public static string GetBaseUrl(this IConfiguration configuration)
        => configuration.GetValue<string>("BaseUrl").TrimEnd('/');
}
