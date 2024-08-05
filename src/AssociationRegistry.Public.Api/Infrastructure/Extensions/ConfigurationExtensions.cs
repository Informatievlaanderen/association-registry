namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using ConfigurationBindings;
using Framework;
using Microsoft.Extensions.Configuration;
using System;

public static class ConfigurationExtensions
{
    public static AssociationRegistry.Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection GetPostgreSqlOptionsSection(this IConfiguration configuration)
    {
        var postgreSqlOptionsSection = configuration
                                      .GetSection(PostgreSqlOptionsSection.Name)
                                      .Get<AssociationRegistry.Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection>();

        postgreSqlOptionsSection.ThrowIfInvalid();

        return postgreSqlOptionsSection;
    }

    private static void ThrowIfInvalid(this AssociationRegistry.Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string sectionName = nameof(AssociationRegistry.Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection);

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(postgreSqlOptions.Database, $"{sectionName}.{nameof(AssociationRegistry.Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection.Database)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(postgreSqlOptions.Host, $"{sectionName}.{nameof(AssociationRegistry.Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection.Host)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(postgreSqlOptions.Username, $"{sectionName}.{nameof(AssociationRegistry.Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection.Username)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(postgreSqlOptions.Password, $"{sectionName}.{nameof(AssociationRegistry.Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection.Password)}");
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
           .IfNullOrWhiteSpace(elasticSearchOptions.Indices?.Verenigingen,
                               $"{sectionName}.{nameof(ElasticSearchOptionsSection.Indices)}.{nameof(ElasticSearchOptionsSection.Indices.Verenigingen)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(elasticSearchOptions.Username, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Username)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(elasticSearchOptions.Password, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Password)}");
    }

    public static string GetBaseUrl(this IConfiguration configuration)
        => configuration.GetValue<string>("BaseUrl").TrimEnd(trimChar: '/');
}
