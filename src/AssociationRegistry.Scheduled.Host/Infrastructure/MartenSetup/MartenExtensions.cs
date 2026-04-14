namespace AssociationRegistry.Scheduled.Host.Infrastructure.MartenSetup;

using Admin.ProjectionHost.Projections.Bewaartermijn;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Marten;
using MartenDb;
using MartenDb.Logging;
using MartenDb.Setup;
using MartenDb.Upcasters.Persoonsgegevens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wolverine.Marten;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions)
    {
        var martenConfiguration = services
                                 .AddMarten(serviceProvider =>
                                  {
                                      var opts = new StoreOptions();

                                      var querySessionFunc = ()
                                          => serviceProvider.GetRequiredService<IDocumentStore>().QuerySession();

                                      opts.UsePostgreSqlOptions(postgreSqlOptions)
                                          .AddVCodeSequence()
                                          .ConfigureSerialization()
                                          .SetUpOpenTelemetry()
                                          .RegisterAllEventTypes()
                                          .RegisterAdminDocumentTypes()
                                          .UpcastEvents(querySessionFunc);

                                      if (!postgreSqlOptions.IncludeErrorDetail)
                                          opts.Logger(
                                              new SecureMartenLogger(
                                                  serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>())
                                          );

                                      opts.Events.StreamIdentity = StreamIdentity.AsString;
                                      opts.Events.MetadataConfig.EnableAll();
                                      opts.Events.AppendMode = EventAppendMode.Quick;

                                      opts.AutoCreateSchemaObjects = AutoCreate.None;

                                      opts.Projections.Add(new BewaartermijnProjection(), ProjectionLifecycle.Async);

                                      return opts;
                                  })
                                 .IntegrateWithWolverine(integration =>
                                  {
                                      integration.TransportSchemaName = WellknownSchemaNames.Wolverine;
                                      integration.MessageStorageSchemaName = WellknownSchemaNames.Wolverine;

                                      integration.AutoCreate = AutoCreate.None;
                                  })
                                 .AddAsyncDaemon(DaemonMode.HotCold)
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

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions) =>
        $"host={postgreSqlOptions.Host};"
      + $"database={postgreSqlOptions.Database};"
      + $"password={postgreSqlOptions.Password};"
      + $"username={postgreSqlOptions.Username};";
}
