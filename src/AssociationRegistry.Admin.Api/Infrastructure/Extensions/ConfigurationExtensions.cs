namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using System;
using AssociationRegistry.Magda.Configuration;
using ConfigurationBindings;
using Framework;
using Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static PostgreSqlOptionsSection GetPostgreSqlOptionsSection(this IConfiguration configuration)
    {
        var postgreSqlOptionsSection = configuration
            .GetSection(PostgreSqlOptionsSection.SectionName)
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
            .GetSection(ElasticSearchOptionsSection.SectionName)
            .Get<ElasticSearchOptionsSection>();

        elasticSearchOptions.ThrowIfInvalid();
        return elasticSearchOptions;
    }

    public static string GetBaseUrl(this IConfiguration configuration)
        => configuration.GetValue<string>("BaseUrl").TrimEnd(trimChar: '/');

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

    public static MagdaOptionsSection GetMagdaOptionsSection(this IConfiguration configuration)
    {
        var magdaOptionsSection = configuration
            .GetSection(MagdaOptionsSection.SectionName)
            .Get<MagdaOptionsSection>();

        magdaOptionsSection.ThrowIfInvalid();
        return magdaOptionsSection;
    }

    private static void ThrowIfInvalid(this MagdaOptionsSection magdaOptionsSection)
    {
        const string sectionName = nameof(MagdaOptionsSection);
        Throw<ArgumentException>
            .If(magdaOptionsSection.ClientCertificate is null != magdaOptionsSection.ClientCertificatePassword is null, $"Both {sectionName}.{nameof(MagdaOptionsSection.ClientCertificate)} and {sectionName}.{nameof(MagdaOptionsSection.ClientCertificatePassword)} must be provided or ignored.");

        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(magdaOptionsSection.Hoedanigheid, $"{sectionName}.{nameof(MagdaOptionsSection.Hoedanigheid)}");

        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(magdaOptionsSection.Afzender, $"{sectionName}.{nameof(MagdaOptionsSection.Afzender)}");

        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(magdaOptionsSection.Ontvanger, $"{sectionName}.{nameof(MagdaOptionsSection.Ontvanger)}");

        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(magdaOptionsSection.GeefOndernemingVkboEndpoint, $"{sectionName}.{nameof(MagdaOptionsSection.GeefOndernemingVkboEndpoint)}");
    }
}
