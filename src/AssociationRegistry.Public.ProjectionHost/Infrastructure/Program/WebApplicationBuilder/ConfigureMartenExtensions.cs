namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Constants;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Json;
using Marten;
using MartenDb.PubliekZoeken;
using MartenDb.Setup;
using MartenDb.Subscriptions;
using Elastic.Clients.Elasticsearch;
using Hosts.Configuration;
using MartenDb.Logging;
using Newtonsoft.Json;
using Projections;
using Projections.Detail;
using Projections.Search;
using Projections.Sequence;
using PostgreSqlOptionsSection = Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection;

public static class ConfigureMartenExtensions
{
    public static IServiceCollection ConfigureProjectionsWithMarten(
        this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        var martenConfiguration = services.AddMarten(serviceProvider =>
        {
            var opts = new StoreOptions();

            return ConfigureStoreOptions(opts,
                                         serviceProvider.GetRequiredService<ElasticsearchClient>(),
                                         serviceProvider.GetRequiredService<ILogger<PubliekZoekenEventsConsumer>>(),
                                         serviceProvider.GetRequiredService<ILogger<MartenSubscription>>(),
                                         serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>(),
                                         configurationManager.GetSection(PostgreSqlOptionsSection.SectionName)
                                                             .Get<PostgreSqlOptionsSection>(),
                                         serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment(),
                                         configurationManager
                                            .GetSection(ElasticSearchOptionsSection.SectionName)
                                            .Get<ElasticSearchOptionsSection>());
        });

        if (configurationManager["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.HotCold);

        if(FeatureFlags.IsTestingMode())
            martenConfiguration.ApplyAllDatabaseChangesOnStartup();
        else
            martenConfiguration.AssertDatabaseMatchesConfigurationOnStartup();

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
            x.Development.ResourceAutoCreate =
                FeatureFlags.IsTestingMode()
                    ? AutoCreate.All
                    : AutoCreate.None;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.ResourceAutoCreate =
                FeatureFlags.IsTestingMode()
                    ? AutoCreate.All
                    : AutoCreate.None;
            x.Production.SourceCodeWritingEnabled = false;
        });

        return services;
    }

    public static StoreOptions ConfigureStoreOptions(
        StoreOptions opts,
        ElasticsearchClient elasticClient,
        ILogger<PubliekZoekenEventsConsumer> publiekZoekenEventsConsumerLogger,
        ILogger<MartenSubscription> martenSubscriptionLogger,
        ILogger<SecureMartenLogger> secureMartenLogger,
        PostgreSqlOptionsSection? postgreSqlOptionsSection,
        bool isDevelopment,
        ElasticSearchOptionsSection? elasticSearchOptionsSection)
    {
        static string GetPostgresConnectionString(PostgreSqlOptionsSection? postgreSqlOptions)
            => $"host={postgreSqlOptions.Host};" +
               $"database={postgreSqlOptions.Database};" +
               $"password={postgreSqlOptions.Password};" +
               $"username={postgreSqlOptions.Username}";

        var postgreSqlOptions = postgreSqlOptionsSection;

        var connectionString = GetPostgresConnectionString(postgreSqlOptions);

        opts.Connection(connectionString);

        if (!string.IsNullOrEmpty(postgreSqlOptions.Schema))
        {
            opts.Events.DatabaseSchemaName = postgreSqlOptions.Schema;
            opts.DatabaseSchemaName = postgreSqlOptions.Schema;
        }

        opts.SetUpOpenTelemetry(isDevelopment);

        opts.Logger(new SecureMartenLogger(secureMartenLogger));

        opts.Events.StreamIdentity = StreamIdentity.AsString;

        opts.Projections.DaemonLockId = 3;

        opts.Events.MetadataConfig.EnableAll();

        opts.Projections.StaleSequenceThreshold = TimeSpan.FromSeconds(30);

        opts.UpcastLegacyTombstoneEvents();

        opts.RegisterAllEventTypes();

        opts.Projections.Add(new PubliekVerenigingDetailProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new PubliekVerenigingSequenceProjection(), ProjectionLifecycle.Async);

        opts.Projections.Add(
            new MartenSubscription(
                new PubliekZoekenEventsConsumer(
                    elasticClient,
                    new PubliekZoekProjectionHandler(),
                    elasticSearchOptionsSection,
                    publiekZoekenEventsConsumerLogger),
                PubliekZoekenHandledEvents.Types,
                martenSubscriptionLogger
            ),
            ProjectionLifecycle.Async,
            ProjectionNames.PubliekZoek);

        opts.UseNewtonsoftForSerialization(configure: settings =>
        {
            settings.DateParseHandling = DateParseHandling.None;
            settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
            settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
        });

        return opts;
    }
}
