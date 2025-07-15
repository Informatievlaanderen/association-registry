namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Constants;
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
        services
           .AddTransient<IElasticRepository, ElasticRepository>();

        var martenConfiguration = services.AddMarten(serviceProvider =>
        {
            var opts = new StoreOptions();

            return ConfigureStoreOptions(opts,
                                         serviceProvider.GetRequiredService<IElasticRepository>(),
                                         serviceProvider.GetRequiredService<ILogger<MartenEventsConsumer>>(),
                                         configurationManager.GetSection(PostgreSqlOptionsSection.SectionName)
                                                             .Get<PostgreSqlOptionsSection>(),
                                         serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment());
        });

        if (configurationManager["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.HotCold);

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
            x.Development.ResourceAutoCreate = AutoCreate.CreateOrUpdate;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.ResourceAutoCreate = AutoCreate.CreateOrUpdate;
            x.Production.SourceCodeWritingEnabled = false;
        });

        martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        return services;
    }

    public static StoreOptions ConfigureStoreOptions(
        StoreOptions opts,
        IElasticRepository elasticRepository,
        ILogger<MartenEventsConsumer> martenEventsConsumerLogger,
        PostgreSqlOptionsSection? postgreSqlOptionsSection,
        bool isDevelopment
    )
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

        opts.Projections.Add(new PubliekVerenigingDetailProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new PubliekVerenigingSequenceProjection(), ProjectionLifecycle.Async);

        opts.Projections.Add(
            new MartenSubscription(
                new MartenEventsConsumer(
                    new PubliekZoekProjectionHandler(elasticRepository),
                    martenEventsConsumerLogger
                )
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
