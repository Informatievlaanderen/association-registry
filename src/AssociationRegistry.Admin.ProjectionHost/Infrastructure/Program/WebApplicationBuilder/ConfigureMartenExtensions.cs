namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Constants;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Json;
using Marten;
using Marten.Services;
using MartenDb;
using MartenDb.BeheerZoeken;
using MartenDb.Setup;
using MartenDb.Subscriptions;
using MartenDb.Upcasters;
using Elastic.Clients.Elasticsearch;
using MartenDb.Logging;
using Newtonsoft.Json;
using Projections;
using Projections.Detail;
using Projections.Historiek;
using Projections.KboSync;
using Projections.Locaties;
using Projections.PowerBiExport;
using Projections.Search;
using Projections.Search.DuplicateDetection;
using Projections.Search.Zoeken;
using Schema;
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
            options.Production.SourceCodeWritingEnabled = false;
            options.Production.ResourceAutoCreate = AutoCreate.None;
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

                return ConfigureStoreOptions(opts, serviceProvider.GetRequiredService<ILogger<LocatieLookupProjection>>(),
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

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
            x.Development.ResourceAutoCreate = AutoCreate.None;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.SourceCodeWritingEnabled = false;
            x.Production.ResourceAutoCreate = AutoCreate.None;
        });

        return martenConfigurationExpression;
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
            .RegisterProjectionDocumentTypes();

        opts.Projections.Add(new BeheerVerenigingHistoriekProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new BeheerVerenigingDetailProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new PowerBiExportProjection(), ProjectionLifecycle.Async);
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
