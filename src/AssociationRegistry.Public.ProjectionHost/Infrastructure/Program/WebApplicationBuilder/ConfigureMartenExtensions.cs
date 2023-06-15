namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Constants;
using Projections.Detail;
using Projections.Search;
using ConfigurationBindings;
using JasperFx.CodeGeneration;
using Json;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Marten.Services;
using Newtonsoft.Json;
using Schema.Detail;
using Wolverine;

public static class ConfigureMartenExtensions
{
    public static IServiceCollection ConfigureProjectionsWithMarten(this IServiceCollection source, ConfigurationManager configurationManager)
    {
        source
            .AddTransient<IElasticRepository, ElasticRepository>();

        var martenConfiguration = AddMarten(source, configurationManager);

        if (configurationManager["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
        {
            martenConfiguration.AddAsyncDaemon(DaemonMode.Solo);
        }

        return source;
    }

    private static MartenServiceCollectionExtensions.MartenConfigurationExpression AddMarten(
        IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
        {
            return $"host={postgreSqlOptions.Host};" +
                   $"database={postgreSqlOptions.Database};" +
                   $"password={postgreSqlOptions.Password};" +
                   $"username={postgreSqlOptions.Username}";
        }

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

                opts.Events.MetadataConfig.EnableAll();

                opts.Projections.OnException(_ => true).Stop();
                opts.Projections.Add<PubliekVerenigingDetailProjection>(ProjectionLifecycle.Async);
                opts.Projections.Add(
                    new MartenSubscription(
                        new MartenEventsConsumer(
                            serviceProvider.GetRequiredService<IMessageBus>()
                        )
                    ),
                    ProjectionLifecycle.Async,
                    "PubliekVerenigingZoekenDocument");

                opts.Serializer(CreateCustomMartenSerializer());

                opts.RegisterDocumentType<PubliekVerenigingDetailDocument>();
                opts.GeneratedCodeMode = TypeLoadMode.Auto;
                return opts;
            });

        return martenConfigurationExpression;
    }
}
