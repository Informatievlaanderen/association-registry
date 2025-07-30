namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

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
using MartenDb.Setup;
using MartenDb.Upcasters;
using Nest;
using Newtonsoft.Json;
using Projections;
using Projections.Detail;
using Projections.Search;
using Projections.Sequence;
using Schema.Detail;
using Schema.Sequence;
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
                                         serviceProvider.GetRequiredService<IElasticClient>(),
                                         serviceProvider.GetRequiredService<ILogger<PubliekZoekenEventsConsumer>>(),
                                         configurationManager.GetSection(PostgreSqlOptionsSection.SectionName)
                                                             .Get<PostgreSqlOptionsSection>(),
                                         serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment(),
                                         configurationManager
                                            .GetSection(ElasticSearchOptionsSection.SectionName)
                                            .Get<ElasticSearchOptionsSection>());
        });

        if (configurationManager["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.HotCold);

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.SourceCodeWritingEnabled = false;
        });

        martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        return services;
    }

    public static StoreOptions ConfigureStoreOptions(
        StoreOptions opts,
        IElasticClient elasticClient,
        ILogger<PubliekZoekenEventsConsumer> publiekZoekenEventsConsumerLogger,
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

        opts.Events.StreamIdentity = StreamIdentity.AsString;

        opts.Projections.DaemonLockId = 3;

        opts.Events.MetadataConfig.EnableAll();

        opts.Projections.StaleSequenceThreshold = TimeSpan.FromSeconds(30);

        opts.UpcastLegacyTombstoneEvents();

        opts.Projections.Add(new PubliekVerenigingDetailProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new PubliekVerenigingSequenceProjection(), ProjectionLifecycle.Async);

        opts.Projections.Add(
            new MartenSubscription(
                new PubliekZoekenEventsConsumer(
                    elasticClient,
                    new PubliekZoekProjectionHandler(),
                    elasticSearchOptionsSection,
                    publiekZoekenEventsConsumerLogger)
            ),
            ProjectionLifecycle.Async,
            ProjectionNames.PubliekZoek);

        opts.UseNewtonsoftForSerialization(configure: settings =>
        {
            settings.DateParseHandling = DateParseHandling.None;
            settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
            settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
        });

        opts.RegisterDocumentType<PubliekVerenigingDetailDocument>();
        opts.RegisterDocumentType<PubliekVerenigingSequenceDocument>();

        return opts;
    }
}
