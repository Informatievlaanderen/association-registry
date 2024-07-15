namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using ConfigurationBindings;
using Constants;
using JasperFx.CodeGeneration;
using Json;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Marten.Services;
using Metrics;
using Newtonsoft.Json;
using Projections;
using Projections.Detail;
using Projections.Search;
using Schema.Detail;
using Wolverine;

public static class ConfigureMartenExtensions
{
    public static IServiceCollection ConfigureProjectionsWithMarten(
        this IServiceCollection source,
        ConfigurationManager configurationManager)
    {
        source
           .AddTransient<IElasticRepository, ElasticRepository>();

        var martenConfiguration = AddMarten(source, configurationManager);

        if (configurationManager["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.HotCold);

        return source;
    }

    private static MartenServiceCollectionExtensions.MartenConfigurationExpression AddMarten(
        IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
            => $"host={postgreSqlOptions.Host};" +
               $"database={postgreSqlOptions.Database};" +
               $"password={postgreSqlOptions.Password};" +
               $"username={postgreSqlOptions.Username}";

        static JsonNetSerializer CreateCustomMartenSerializer()
        {
            var jsonNetSerializer = new JsonNetSerializer();

            jsonNetSerializer.Customize(
                s =>
                {
                    s.DateParseHandling = DateParseHandling.None;
                    s.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                    s.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
                });

            return jsonNetSerializer;
        }

        var martenConfigurationExpression = services.AddMarten(
            serviceProvider =>
            {
                var postgreSqlOptions = configurationManager.GetSection(PostgreSqlOptionsSection.Name)
                                                            .Get<PostgreSqlOptionsSection>();

                var connectionString = GetPostgresConnectionString(postgreSqlOptions);

                var opts = new StoreOptions();

                opts.Connection(connectionString);

                opts.Events.StreamIdentity = StreamIdentity.AsString;

                opts.Projections.DaemonLockId = 3;

                opts.Events.MetadataConfig.EnableAll();

                opts.Projections.StaleSequenceThreshold = TimeSpan.FromSeconds(30);

                opts.Projections.Add(new PubliekVerenigingDetailProjection(), ProjectionLifecycle.Async);

                opts.Projections.Add(
                    new MartenSubscription(
                        new MartenEventsConsumer(
                            new PubliekZoekProjectionHandler(serviceProvider.GetRequiredService<IElasticRepository>()),
                            serviceProvider.GetRequiredService<ILogger<MartenEventsConsumer>>()
                        )
                    ),
                    ProjectionLifecycle.Async,
                    ProjectionNames.VerenigingZoeken);

                opts.Serializer(CreateCustomMartenSerializer());

                opts.RegisterDocumentType<PubliekVerenigingDetailDocument>();

                if (serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    opts.GeneratedCodeMode = TypeLoadMode.Dynamic;
                }
                else
                {
                    opts.GeneratedCodeMode = TypeLoadMode.Static;
                    opts.SourceCodeWritingEnabled = false;
                }

                return opts;
            });

        martenConfigurationExpression.ApplyAllDatabaseChangesOnStartup();

        return martenConfigurationExpression;
    }
}
