namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using AssociationRegistry.MartenDb.BeheerZoeken;
using AssociationRegistry.MartenDb.Logging;
using AssociationRegistry.MartenDb.Setup;
using AssociationRegistry.MartenDb.Subscriptions;
using Constants;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Json;
using Marten;
using Marten.Services;
using Elastic.Clients.Elasticsearch;
using MartenDb.Upcasters.Persoonsgegevens;
using Newtonsoft.Json;
using Projections;
using Projections.Bewaartermijn;
using Projections.Detail;
using Projections.Historiek;
using Projections.KboSync;
using Projections.Locaties;
using Projections.PowerBiExport;
using Projections.Search;
using Projections.Search.DuplicateDetection;
using Projections.Search.Zoeken;
using Projections.Vertegenwoordiger;
using Schema.Setup.Marten;
using System.Configuration;
using ConfigurationManager = ConfigurationManager;

public static class ConfigureMartenExtensions
{
    public static IServiceCollection ConfigureProjectionsWithMarten(
        this IServiceCollection source,
        ConfigurationManager configurationManager,
        bool isDevelopment)
    {
        var martenConfiguration = AddMarten(source, configurationManager);


        if (configurationManager["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(isDevelopment ? DaemonMode.Solo : DaemonMode.HotCold);

        martenConfiguration.AssertDatabaseMatchesConfigurationOnStartup();

        source.CritterStackDefaults(options =>
        {
            options.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
            options.Development.ResourceAutoCreate = AutoCreate.None;

            options.Production.GeneratedCodeMode = TypeLoadMode.Static;
            options.Production.ResourceAutoCreate = AutoCreate.None;
            options.Production.SourceCodeWritingEnabled = false;
        });


        return source;
    }

    private static MartenServiceCollectionExtensions.MartenConfigurationExpression AddMarten(
        IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        var martenConfigurationExpression = services.AddMarten(
            serviceProvider =>
            {
                var opts = new StoreOptions();

                return ConfigureStoreOptions(opts, serviceProvider, serviceProvider.GetRequiredService<ILogger<LocatieLookupProjection>>(),
                                             serviceProvider.GetRequiredService<ILogger<LocatieZonderAdresMatchProjection>>(),
                                             serviceProvider.GetRequiredService<ElasticsearchClient>(),
                                             serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment(),
                                             serviceProvider.GetRequiredService<ILogger<BeheerZoekenEventsConsumer>>(),
                                             serviceProvider.GetRequiredService<ILogger<DuplicateDetectionEventsConsumer>>(),
                                             () => serviceProvider.GetRequiredService<ILogger<MartenSubscription>>(),
                                             serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>(),
                                             configurationManager
                                                .GetSection(PostgreSqlOptionsSection.SectionName)
                                                .Get<PostgreSqlOptionsSection>(),
                                             configurationManager
                                                .GetSection(ElasticSearchOptionsSection.SectionName)
                                                .Get<ElasticSearchOptionsSection>());
            });

        return martenConfigurationExpression;
    }

    public static StoreOptions ConfigureStoreOptions(
        StoreOptions opts,
        IServiceProvider serviceProvider,
        ILogger<LocatieLookupProjection> locatieLookupLogger,
        ILogger<LocatieZonderAdresMatchProjection> locatieZonderAdresMatchProjectionLogger,
        ElasticsearchClient elasticClient,
        bool isDevelopment,
        ILogger<BeheerZoekenEventsConsumer> beheerZoekenEventsConsumerLogger,
        ILogger<DuplicateDetectionEventsConsumer> duplicateDetectionEventsConsumerLogger,
        Func<ILogger<MartenSubscription>> subscriptionLogger,
        ILogger<SecureMartenLogger> secureMartenLogger,
        PostgreSqlOptionsSection? postgreSqlOptionsSection,
        ElasticSearchOptionsSection? elasticSearchOptionsSection)
    {
        return ConfigureStoreOptionsCore(
            opts,
            () => serviceProvider.GetRequiredService<IDocumentStore>().QuerySession(),
            locatieLookupLogger,
            locatieZonderAdresMatchProjectionLogger,
            elasticClient,
            isDevelopment,
            beheerZoekenEventsConsumerLogger,
            duplicateDetectionEventsConsumerLogger,
            subscriptionLogger,
            secureMartenLogger,
            postgreSqlOptionsSection,
            elasticSearchOptionsSection);
    }

    public static StoreOptions ConfigureStoreOptions(
        StoreOptions opts,
        ILogger<LocatieLookupProjection> locatieLookupLogger,
        ILogger<LocatieZonderAdresMatchProjection> locatieZonderAdresMatchProjectionLogger,
        ElasticsearchClient elasticClient,
        bool isDevelopment,
        ILogger<BeheerZoekenEventsConsumer> beheerZoekenEventsConsumerLogger,
        ILogger<DuplicateDetectionEventsConsumer> duplicateDetectionEventsConsumerLogger,
        Func<ILogger<MartenSubscription>> subscriptionLogger,
        ILogger<SecureMartenLogger> secureMartenLogger,
        PostgreSqlOptionsSection? postgreSqlOptionsSection,
        ElasticSearchOptionsSection? elasticSearchOptionsSection)
    {
        IDocumentStore? builtStore = null;

        return ConfigureStoreOptionsCore(
            opts,
            () =>
            {
                builtStore ??= new DocumentStore(opts);
                return builtStore.QuerySession();
            },
            locatieLookupLogger,
            locatieZonderAdresMatchProjectionLogger,
            elasticClient,
            isDevelopment,
            beheerZoekenEventsConsumerLogger,
            duplicateDetectionEventsConsumerLogger,
            subscriptionLogger,
            secureMartenLogger,
            postgreSqlOptionsSection,
            elasticSearchOptionsSection);
    }

    private static StoreOptions ConfigureStoreOptionsCore(
        StoreOptions opts,
        Func<IQuerySession> querySessionFactory,
        ILogger<LocatieLookupProjection> locatieLookupLogger,
        ILogger<LocatieZonderAdresMatchProjection> locatieZonderAdresMatchProjectionLogger,
        ElasticsearchClient elasticClient,
        bool isDevelopment,
        ILogger<BeheerZoekenEventsConsumer> beheerZoekenEventsConsumerLogger,
        ILogger<DuplicateDetectionEventsConsumer> duplicateDetectionEventsConsumerLogger,
        Func<ILogger<MartenSubscription>> subscriptionLogger,
        ILogger<SecureMartenLogger> secureMartenLogger,
        PostgreSqlOptionsSection? postgreSqlOptionsSection,
        ElasticSearchOptionsSection? elasticSearchOptionsSection)
    {
        static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
            => $"host={postgreSqlOptions.Host};" +
               $"database={postgreSqlOptions.Database};" +
               $"password={postgreSqlOptions.Password};" +
               $"username={postgreSqlOptions.Username}";

        var postgreSqlOptions = postgreSqlOptionsSection ??
                                throw new ConfigurationErrorsException("Missing a valid postgres configuration");

        var connectionString = GetPostgresConnectionString(postgreSqlOptions);

        opts.UseNewtonsoftForSerialization(configure: settings =>
        {
            settings.DateParseHandling = DateParseHandling.None;
            settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
            settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
        });

        if (!string.IsNullOrEmpty(postgreSqlOptions.Schema))
        {
            opts.Events.DatabaseSchemaName = postgreSqlOptions.Schema;
            opts.DatabaseSchemaName = postgreSqlOptions.Schema;
        }

        opts.Connection(connectionString);

        opts.OpenTelemetry.TrackConnections = TrackLevel.Normal;
        opts.OpenTelemetry.TrackEventCounters();
        opts.DisableNpgsqlLogging = !isDevelopment;

        if(!postgreSqlOptions.IncludeErrorDetail)
            opts.Logger(new SecureMartenLogger(secureMartenLogger));

        opts.Events.StreamIdentity = StreamIdentity.AsString;

        opts.Events.MetadataConfig.EnableAll();

        opts.Projections.StaleSequenceThreshold = TimeSpan.FromSeconds(30);

        opts.Projections.DaemonLockId = 1;

        opts.UpcastLegacyTombstoneEvents()
            .RegisterAllEventTypes()
            .RegisterProjectionDocumentTypes()
            .UpcastEvents(querySessionFactory);

        opts.Projections.Add(new BeheerVerenigingHistoriekProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new BeheerVerenigingDetailProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new BewaartermijnProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new VertegenwoordigerProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new PowerBiExportProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new PowerBiExportDubbelDetectieProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new BeheerKboSyncHistoriekProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new LocatieLookupProjection(locatieLookupLogger), ProjectionLifecycle.Async);
        opts.Projections.Add(new LocatieZonderAdresMatchProjection(locatieZonderAdresMatchProjectionLogger), ProjectionLifecycle.Async);

        opts.Projections.Add(
            new MartenSubscription(
                new BeheerZoekenEventsConsumer(
                    elasticClient,
                    new BeheerZoekProjectionHandler(),
                    elasticSearchOptionsSection,
                    beheerZoekenEventsConsumerLogger),
                BeheerZoekenHandledEvents.Types,
                subscriptionLogger()
            ),
            ProjectionLifecycle.Async,
            ProjectionNames.BeheerZoek);

        opts.Projections.Add(
            new MartenSubscription(
                new DuplicateDetectionEventsConsumer(
                    elasticClient,
                    new DuplicateDetectionProjectionHandler(),
                    elasticSearchOptionsSection,
                    duplicateDetectionEventsConsumerLogger
                ),
                DuplicateDetectionHandledEvents.Types,
                subscriptionLogger()
            ),
            ProjectionLifecycle.Async,
            ProjectionNames.DuplicateDetection);


        return opts;
    }
}
