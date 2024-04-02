namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Configuration;
using ConfigurationBindings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;

public static class ConfigurationExtensions
{
    public static PostgreSqlOptionsSection GetPostgreSqlOptionsSection(this IConfiguration configuration)
    {
        var postgreSqlOptionsSection = configuration
                                      .GetSection(PostgreSqlOptionsSection.SectionName)
                                      .Get<PostgreSqlOptionsSection>();

        postgreSqlOptionsSection.ThrowIfInvalid();

        return postgreSqlOptionsSection!;
    }

    public static AddressMatchOptionsSection GetAddressMatchOptionsSection(this IConfiguration configuration)
    {
        var addressMatchOptionsSection = configuration
                                      .GetSection(AddressMatchOptionsSection.SectionName)
                                      .Get<AddressMatchOptionsSection>();

        addressMatchOptionsSection.ThrowIfInvalid();

        return addressMatchOptionsSection!;
    }

    private static void ThrowIfInvalid(this AddressMatchOptionsSection? addressMatchOptionsSection)
    {
        const string sectionName = nameof(AddressMatchOptionsSection);

        if (addressMatchOptionsSection == null)
            throw new ArgumentNullException(nameof(addressMatchOptionsSection));

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(addressMatchOptionsSection.AddressMatchSqsQueueName, $"{sectionName}.{nameof(AddressMatchOptionsSection.AddressMatchSqsQueueName)}");
    }

    private static void ThrowIfInvalid(this PostgreSqlOptionsSection? postgreSqlOptions)
    {
        if (postgreSqlOptions == null)
            throw new ArgumentNullException(nameof(postgreSqlOptions));

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
           .IfNullOrWhiteSpace(elasticSearchOptions.Indices?.Verenigingen,
                               $"{sectionName}.{nameof(ElasticSearchOptionsSection.Indices)}.{nameof(ElasticSearchOptionsSection.Indices.Verenigingen)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(elasticSearchOptions.Username, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Username)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(elasticSearchOptions.Password, $"{sectionName}.{nameof(ElasticSearchOptionsSection.Password)}");
    }

    public static MagdaOptionsSection GetMagdaOptionsSection(
        this IConfiguration configuration,
        string magdaOptionsSectionName = MagdaOptionsSection.SectionName)
    {
        var magdaOptionsSection = configuration
                                 .GetSection(magdaOptionsSectionName)
                                 .Get<MagdaOptionsSection>();

        magdaOptionsSection.ThrowIfInvalid();

        return magdaOptionsSection;
    }

    public static TemporaryMagdaVertegenwoordigersSection GetMagdaTemporaryVertegenwoordigersSection(
        this IConfiguration configuration,
        IWebHostEnvironment environment,
        string magdaOptionsSectionName = TemporaryMagdaVertegenwoordigersSection.SectionName)
    {
        if (environment.IsProduction())
        {
            Log.Logger.Information("Not loading temporary vertegenwoordigers in Production");

            return new TemporaryMagdaVertegenwoordigersSection();
        }

        var vertegenwoordigersJson = configuration[magdaOptionsSectionName];
        var temporaryVertegenwoordigers = JsonConvert.DeserializeObject<TemporaryMagdaVertegenwoordigersSection>(vertegenwoordigersJson);

        Log.Logger.Information(messageTemplate: "Found {@Vertegenwoordigers}", temporaryVertegenwoordigers);

        return temporaryVertegenwoordigers;
    }

    private static void ThrowIfInvalid(this MagdaOptionsSection magdaOptionsSection)
    {
        const string sectionName = nameof(MagdaOptionsSection);

        Throw<ArgumentException>
           .If(magdaOptionsSection.ClientCertificate is null != magdaOptionsSection.ClientCertificatePassword is null,
               $"Both {sectionName}.{nameof(MagdaOptionsSection.ClientCertificate)} and {sectionName}.{nameof(MagdaOptionsSection.ClientCertificatePassword)} must be provided or ignored.");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(magdaOptionsSection.Hoedanigheid, $"{sectionName}.{nameof(MagdaOptionsSection.Hoedanigheid)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(magdaOptionsSection.Afzender, $"{sectionName}.{nameof(MagdaOptionsSection.Afzender)}");

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(magdaOptionsSection.Ontvanger, $"{sectionName}.{nameof(MagdaOptionsSection.Ontvanger)}");
    }
}
