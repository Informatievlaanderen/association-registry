namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using Marten;
using MartenDb.Logging;
using MartenDb.Setup;
using MartenDb.Upcasters.Persoonsgegevens;
using Schema.VerenigingenPerInsz;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions,
        IConfiguration configuration)
    {
        var martenConfiguration = services
                                 .AddSingleton(postgreSqlOptions)
                                 .AddMarten(
                                      serviceProvider =>
                                      {
                                          var opts = new StoreOptions();
                                          ConfigureStoreOptionsCore(opts, postgreSqlOptions,
                                                                serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>(),
                                                                () => serviceProvider.GetRequiredService<IDocumentStore>().QuerySession(),
                                                                serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment());
                                          return opts;
                                      });

        if (configuration["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.HotCold);

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
        bool isDevelopment)
    {
        IDocumentStore? builtStore = null;

        return ConfigureStoreOptionsCore(
            opts,
            postgreSqlOptions,
            secureMartenLogger,
            () =>
            {
                builtStore ??= new DocumentStore(opts);

                return builtStore.QuerySession();
            },
            true);
    }

    public static StoreOptions ConfigureStoreOptionsCore(
        StoreOptions opts,
        PostgreSqlOptionsSection postgreSqlOptions,
        ILogger<SecureMartenLogger> secureMartenLogger,
        Func<IQuerySession> querySessionFactory,
        bool isDevelopment)
    {
        opts.Connection(postgreSqlOptions.GetConnectionString());

        if (!string.IsNullOrEmpty(postgreSqlOptions.Schema))
        {
            opts.Events.DatabaseSchemaName = postgreSqlOptions.Schema;
            opts.DatabaseSchemaName = postgreSqlOptions.Schema;
        }

        opts.Events.StreamIdentity = StreamIdentity.AsString;
        opts.UseNewtonsoftForSerialization(configure: settings =>
        {
            settings.ConfigureForVerenigingsregister();
        });
        opts.Events.MetadataConfig.EnableAll();
        opts.AddPostgresProjections();

        opts.SetUpOpenTelemetry(isDevelopment);

        if(!postgreSqlOptions.IncludeErrorDetail)
            opts.Logger(new SecureMartenLogger(secureMartenLogger));

        opts.Projections.DaemonLockId = 2;

        opts.RegisterDocumentType<VerenigingenPerInszDocument>();
        opts.RegisterDocumentType<VerenigingDocument>();
        opts.UpcastEvents(querySessionFactory);

        return opts;
    }

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username}";
}
