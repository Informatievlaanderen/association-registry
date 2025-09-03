namespace AssociationRegistry.Admin.Api.Infrastructure.MartenSetup;

using global::Wolverine.Marten;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using Marten;
using MartenDb.Logging;
using MartenDb.Setup;

public static class MartenExtensions
{
    private const string WolverineSchemaName = "public";

    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        IConfigurationRoot configuration,
        PostgreSqlOptionsSection postgreSqlOptions,
        bool isDevelopment)
    {
        var martenConfiguration = services
                                 .AddMarten(
                                      serviceProvider =>
                                      {
                                          var opts = new StoreOptions();

                                          opts
                                             .UsePostgreSqlOptions(postgreSqlOptions)
                                             .AddVCodeSequence()
                                             .ConfigureSerialization()
                                             .SetUpOpenTelemetry(isDevelopment)
                                             .RegisterAllEventTypes()
                                             .RegisterDocumentTypes();

                                          opts.Logger(new SecureMartenLogger(serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>()));

                                          opts.Events.StreamIdentity = StreamIdentity.AsString;
                                          opts.Events.MetadataConfig.EnableAll();
                                          opts.Events.AppendMode = EventAppendMode.Quick;

                                          opts.AutoCreateSchemaObjects = AutoCreate.All;

                                          return opts;
                                      })
                                 .IntegrateWithWolverine(integration =>
                                  {
                                      integration.TransportSchemaName = WolverineSchemaName;
                                      integration.MessageStorageSchemaName = WolverineSchemaName;
                                  })

                                 .UseLightweightSessions();

        if (configuration["ApplyAllDatabaseChangesDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
            //x.Development.ResourceAutoCreate = AutoCreate.CreateOrUpdate;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            //x.Production.ResourceAutoCreate = AutoCreate.CreateOrUpdate;
            x.Production.SourceCodeWritingEnabled = false;
        });

        return services;
    }

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username};";
}
