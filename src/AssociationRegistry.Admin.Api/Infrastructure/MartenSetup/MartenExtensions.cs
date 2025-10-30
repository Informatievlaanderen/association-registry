namespace AssociationRegistry.Admin.Api.Infrastructure.MartenSetup;

using global::Wolverine.Marten;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using MartenDb;
using MartenDb.Logging;
using MartenDb.Setup;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectionHost.Projections.Detail;
using ProjectionHost.Projections.Historiek;
using ProjectionHost.Projections.KboSync;
using ProjectionHost.Projections.Locaties;
using ProjectionHost.Projections.PowerBiExport;

public static class MartenExtensions
{
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
                                             .RegisterAdminDocumentTypes();

                                          if(!postgreSqlOptions.IncludeErrorDetail)
                                            opts.Logger(new SecureMartenLogger(serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>()));

                                          opts.Events.StreamIdentity = StreamIdentity.AsString;
                                          opts.Events.MetadataConfig.EnableAll();
                                          opts.Events.AppendMode = EventAppendMode.Quick;

                                          opts.AutoCreateSchemaObjects = AutoCreate.None;

                                          opts.Projections.Add(new BeheerVerenigingHistoriekProjection(), ProjectionLifecycle.Async);
                                          opts.Projections.Add(new BeheerVerenigingDetailProjection(), ProjectionLifecycle.Async);
                                          opts.Projections.Add(new PowerBiExportProjection(), ProjectionLifecycle.Async);
                                          opts.Projections.Add(new PowerBiExportDubbelDetectieProjection(), ProjectionLifecycle.Async);
                                          opts.Projections.Add(new BeheerKboSyncHistoriekProjection(), ProjectionLifecycle.Async);
                                          opts.Projections.Add(new LocatieLookupProjection(NullLogger<LocatieLookupProjection>.Instance), ProjectionLifecycle.Async);
                                          opts.Projections.Add(new LocatieZonderAdresMatchProjection(NullLogger<LocatieZonderAdresMatchProjection>.Instance), ProjectionLifecycle.Async);

                                          return opts;
                                      })
                                 .IntegrateWithWolverine(integration =>
                                  {
                                      integration.TransportSchemaName = WellknownSchemaNames.Wolverine;
                                      integration.MessageStorageSchemaName = WellknownSchemaNames.Wolverine;

                                      integration.AutoCreate = AutoCreate.All;
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

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username};";
}
