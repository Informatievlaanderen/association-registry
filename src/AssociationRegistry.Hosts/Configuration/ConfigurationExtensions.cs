namespace AssociationRegistry.Hosts.Configuration;

using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Integrations.Grar.Bewaartermijnen;
using Integrations.Grar.Clients;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

public static class ConfigurationExtensions
{
    public static InitialRegistreerInschrijvingOptions GetInitialRegistreerInschrijvingOptions(this IConfiguration configuration)
    {
        var initialInschrijvingenWolverineOptions = configuration
                                      .GetSection(PostgreSqlOptionsSection.SectionName)
                                      .Get<InitialRegistreerInschrijvingOptions>();


        return initialInschrijvingenWolverineOptions ?? new InitialRegistreerInschrijvingOptions();
    }

    public static PostgreSqlOptionsSection GetPostgreSqlOptionsSection(this IConfiguration configuration)
    {
        var postgreSqlOptionsSection = configuration
                                      .GetSection(PostgreSqlOptionsSection.SectionName)
                                      .Get<PostgreSqlOptionsSection>();

        postgreSqlOptionsSection.ThrowIfInvalid();

        return postgreSqlOptionsSection!;
    }

    public static GrarOptions GetGrarOptions(this IConfiguration configuration)
    {
        var grarOptions = configuration
                         .GetSection(nameof(GrarOptions))
                         .Get<GrarOptions>();

        grarOptions.ThrowIfInValid();

        return grarOptions!;
    }

    public static BewaartermijnOptions GetBewaartermijnOptions(this IConfiguration configuration)
    {
        var bewaartermijnDuration = configuration
                         .GetSection(nameof(BewaartermijnOptions))
                         .Get<BewaartermijnOptions>();

        return bewaartermijnDuration is null ? new BewaartermijnOptions() : bewaartermijnDuration!;
    }

    public static AppSettings GetAppSettings(this IConfiguration configuration)
    {
        return configuration.Get<AppSettings>();
    }

    private static void ThrowIfInValid(this GrarOptions opt)
    {
        if (opt.Kafka.Enabled)
        {
            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(opt.Kafka.Username, nameof(opt.Kafka.Username));

            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(opt.Kafka.Password, nameof(opt.Kafka.Password));
        }

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(opt.Sqs.AddressMatchQueueName, nameof(opt.Sqs.AddressMatchQueueName));

        if (opt.Sqs.GrarSyncQueueListenerEnabled)
        {
            Throw<ArgumentNullException>.IfNullOrWhiteSpace(opt.Sqs.GrarSyncDeadLetterQueueName,
                                                            nameof(opt.Sqs.GrarSyncDeadLetterQueueName));

            Throw<ArgumentNullException>.IfNullOrWhiteSpace(opt.Sqs.GrarSyncQueueName, nameof(opt.Sqs.GrarSyncQueueName));

            Throw<ArgumentNullException>.IfNullOrWhiteSpace(opt.Sqs.GrarSyncQueueUrl, nameof(opt.Sqs.GrarSyncQueueUrl));
        }

        opt.HttpClient.ThrowIfInValid();
    }

    private static void ThrowIfInValid(this GrarOptions.HttpClientOptions opt)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(opt.BaseUrl, nameof(opt.BaseUrl));

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
