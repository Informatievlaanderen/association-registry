namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

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
using Projections.Detail;
using Projections.Historiek;
using Projections.Search;
using Schema.Detail;
using Schema.Historiek;
using System.Configuration;
using Wolverine;
using ConfigurationManager = ConfigurationManager;

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
            martenConfiguration.AddAsyncDaemon(DaemonMode.Solo);

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
                                                            .Get<PostgreSqlOptionsSection>() ??
                                        throw new ConfigurationErrorsException("Missing a valid postgres configuration");

                var connectionString = GetPostgresConnectionString(postgreSqlOptions);

                var opts = new StoreOptions();

                opts.Connection(connectionString);

                opts.Events.StreamIdentity = StreamIdentity.AsString;

                opts.Events.MetadataConfig.EnableAll();

                opts.Projections.OnException(_ => true).Stop();

                opts.Projections.AsyncListeners.Add(
                    new ProjectionStateListener(serviceProvider.GetRequiredService<AdminInstrumentation>()));

                opts.Projections.Add<BeheerVerenigingHistoriekProjection>(ProjectionLifecycle.Async);
                opts.Projections.Add<BeheerVerenigingDetailProjection>(ProjectionLifecycle.Async);

                opts.Projections.Add(
                    new MartenSubscription(
                        new MartenEventsConsumer(
                            serviceProvider.GetRequiredService<IMessageBus>()
                        )
                    ),
                    ProjectionLifecycle.Async,
                    projectionName: "BeheerVerenigingZoekenDocument");

                opts.Serializer(CreateCustomMartenSerializer());

                opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
                opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();

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

        return martenConfigurationExpression;
    }
}
