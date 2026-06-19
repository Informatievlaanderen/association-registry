namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using Constants;
using Grar.NutsLau;
using Hosts.Configuration;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Json;
using Marten;
using Marten.Newtonsoft;
using MartenDb.Logging;
using MartenDb.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectionHost.Projections.Detail;
using ProjectionHost.Projections.Sequence;
using Schema.Detail;
using Schema.Sequence;
using PostgreSqlOptionsSection = Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions,
        IConfiguration configuration
    )
    {
        var martenConfiguration = services
            .AddMarten(serviceProvider =>
                 {
                     var opts = new StoreOptions();

                return ConfigureStoreOptions(
                    opts,
                    postgreSqlOptions,
                    serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>(),
                    AutoCreate.None
                );
            })
            .UseLightweightSessions();

        martenConfiguration.AssertDatabaseMatchesConfigurationOnStartup();

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
            x.Development.ResourceAutoCreate = AutoCreate.None;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.ResourceAutoCreate = AutoCreate.None;
            x.Production.SourceCodeWritingEnabled = false;
        });

        return services;
    }

    public static StoreOptions ConfigureStoreOptions(
        StoreOptions opts,
        PostgreSqlOptionsSection postgreSqlOptions,
        ILogger<SecureMartenLogger> secureMartenLogger,
        AutoCreate autoCreate
    )
    {
        var connectionString = GetPostgresConnectionString(postgreSqlOptions);

        opts.Connection(connectionString);

                     if (!string.IsNullOrEmpty(postgreSqlOptions.Schema))
                     {
                         opts.Events.DatabaseSchemaName = postgreSqlOptions.Schema;
                         opts.DatabaseSchemaName = postgreSqlOptions.Schema;
                     }

                     opts.SetUpOpenTelemetry();

                     if (!postgreSqlOptions.IncludeErrorDetail)
            opts.Logger(new SecureMartenLogger(secureMartenLogger));

                     opts.Events.StreamIdentity = StreamIdentity.AsString;

                     opts.Events.MetadataConfig.EnableAll();
        opts.Events.AppendMode = EventAppendMode.Quick;

        opts.AutoCreateSchemaObjects = autoCreate;

                     opts.RegisterDocumentType<PubliekVerenigingSequenceDocument>();
                     opts.RegisterDocumentType<PubliekVerenigingDetailDocument>();
                     opts.RegisterDocumentType<PostalNutsLauInfo>();

                     opts.Projections.Add(new PubliekVerenigingDetailProjection(), ProjectionLifecycle.Async);
                     opts.Projections.Add(new PubliekVerenigingSequenceProjection(), ProjectionLifecycle.Async);

                     opts.UseNewtonsoftForSerialization(configure: settings =>
                     {
                         settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                         settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
                     });

                     return opts;
    }

    private static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions) =>
        $"host={postgreSqlOptions.Host};"
        + $"database={postgreSqlOptions.Database};"
        + $"password={postgreSqlOptions.Password};"
        + $"username={postgreSqlOptions.Username}";
}
