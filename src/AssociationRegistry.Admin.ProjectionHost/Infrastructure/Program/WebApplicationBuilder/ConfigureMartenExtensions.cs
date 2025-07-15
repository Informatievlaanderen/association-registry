namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

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
using Schema.Detail;
using Schema.Historiek;
using Schema.KboSync;
using Schema.PowerBiExport;
using System.Configuration;
using ConfigurationManager = ConfigurationManager;

public static class ConfigureMartenExtensions
{
    public static IServiceCollection ConfigureProjectionsWithMarten(
        this IServiceCollection source,
        ConfigurationManager configurationManager,
        bool isDevelopment)
    {
        source
           .AddTransient<IElasticRepository, ElasticRepository>();

        var martenConfiguration = AddMarten(source, configurationManager);

        if (configurationManager["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(isDevelopment ? DaemonMode.Solo : DaemonMode.HotCold);

        martenConfiguration.ApplyAllDatabaseChangesOnStartup();

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

                return ConfigureStoreOptions(opts, serviceProvider.GetRequiredService<ILogger<LocatieLookupProjection>>(), serviceProvider.GetRequiredService<ILogger<LocatieZonderAdresMatchProjection>>(), serviceProvider.GetRequiredService<IElasticRepository>(), serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment(), serviceProvider.GetRequiredService<ILogger<BeheerZoekenEventsConsumer>>(), configurationManager.GetSection(PostgreSqlOptionsSection.SectionName)
                                            .Get<PostgreSqlOptionsSection>());
            });

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
            x.Development.ResourceAutoCreate = AutoCreate.CreateOrUpdate;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.SourceCodeWritingEnabled = false;
            x.Production.ResourceAutoCreate = AutoCreate.CreateOrUpdate;
        });

        return martenConfigurationExpression;
    }

    public static StoreOptions ConfigureStoreOptions(
        StoreOptions opts,
        ILogger<LocatieLookupProjection> locatieLookupLogger,
        ILogger<LocatieZonderAdresMatchProjection> locatieZonderAdresMatchProjectionLogger,
        IElasticRepository elasticRepository,
        bool isDevelopment,
        ILogger<BeheerZoekenEventsConsumer> beheerZoekenEventsConsumerLogger,
        PostgreSqlOptionsSection? postgreSqlOptionsSection)
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

                opts.Events.StreamIdentity = StreamIdentity.AsString;

        opts.Events.MetadataConfig.EnableAll();

        opts.Projections.StaleSequenceThreshold = TimeSpan.FromSeconds(30);

        opts.Projections.DaemonLockId = 1;

        opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
        opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();
        opts.RegisterDocumentType<PowerBiExportDocument>();
        opts.RegisterDocumentType<LocatieLookupDocument>();
        opts.RegisterDocumentType<LocatieZonderAdresMatchDocument>();

        opts.Schema.For<LocatieLookupDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.Schema.For<LocatieZonderAdresMatchDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.Schema.For<PowerBiExportDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.Projections.Add(new BeheerVerenigingHistoriekProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new BeheerVerenigingDetailProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new PowerBiExportProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new BeheerKboSyncHistoriekProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add( new LocatieLookupProjection(locatieLookupLogger), ProjectionLifecycle.Async);
        opts.Projections.Add( new LocatieZonderAdresMatchProjection(locatieZonderAdresMatchProjectionLogger), ProjectionLifecycle.Async);

        opts.Projections.Add(
            new MartenSubscription(
                new BeheerZoekenEventsConsumer(
                    new BeheerZoekProjectionHandler(
                        elasticRepository
                    ),
                    beheerZoekenEventsConsumerLogger)
            ),
            ProjectionLifecycle.Async,
            ProjectionNames.BeheerZoek);

        opts.Projections.Add(
            new MartenSubscription(
                new DuplicateDetectionEventsConsumer(
                    new DuplicateDetectionProjectionHandler(
                        elasticRepository)
                )
            ),
            ProjectionLifecycle.Async,
            ProjectionNames.DuplicateDetection);

        opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
        opts.RegisterDocumentType<PowerBiExportDocument>();
        opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();
        opts.RegisterDocumentType<BeheerKboSyncHistoriekGebeurtenisDocument>();
        opts.RegisterDocumentType<LocatieLookupDocument>();
        opts.RegisterDocumentType<LocatieZonderAdresMatchDocument>();

        opts.Schema.For<LocatieLookupDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);
        opts.Schema.For<LocatieZonderAdresMatchDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.Schema.For<PowerBiExportDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        if (isDevelopment)
        {
            opts.GeneratedCodeMode = TypeLoadMode.Dynamic;
        }
        else
        {
            opts.GeneratedCodeMode = TypeLoadMode.Static;
            opts.SourceCodeWritingEnabled = false;
        }

        return opts;
    }
}
