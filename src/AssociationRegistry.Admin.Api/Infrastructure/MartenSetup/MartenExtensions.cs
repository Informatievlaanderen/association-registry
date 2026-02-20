namespace AssociationRegistry.Admin.Api.Infrastructure.MartenSetup;

using AssociationRegistry.MartenDb;
using AssociationRegistry.MartenDb.Logging;
using AssociationRegistry.MartenDb.Setup;
using global::Wolverine.Marten;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using MartenDb.Upcasters.Persoonsgegevens;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectionHost.Infrastructure.Program.WebApplicationBuilder;
using ProjectionHost.Projections.Bewaartermijn;
using ProjectionHost.Projections.Detail;
using ProjectionHost.Projections.Historiek;
using ProjectionHost.Projections.KboSync;
using ProjectionHost.Projections.Locaties;
using ProjectionHost.Projections.PowerBiExport;
using ProjectionHost.Projections.Vertegenwoordiger;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        IConfigurationRoot configuration,
        PostgreSqlOptionsSection postgreSqlOptions,
        bool isDevelopment
    )
    {
        var martenConfiguration = services
            .AddMarten(serviceProvider =>
            {
                var opts = new StoreOptions();

                var querySessionFunc = () => serviceProvider.GetRequiredService<IDocumentStore>().QuerySession();

                opts.UsePostgreSqlOptions(postgreSqlOptions)
                    .AddVCodeSequence()
                    .ConfigureSerialization()
                    .SetUpOpenTelemetry(isDevelopment)
                    .RegisterAllEventTypes()
                    .RegisterAdminDocumentTypes()
                    .UpcastEvents(querySessionFunc);

                if (!postgreSqlOptions.IncludeErrorDetail)
                    opts.Logger(
                        new SecureMartenLogger(serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>())
                    );

                opts.Events.StreamIdentity = StreamIdentity.AsString;
                opts.Events.MetadataConfig.EnableAll();
                opts.Events.AppendMode = EventAppendMode.Quick;

                opts.AutoCreateSchemaObjects = AutoCreate.None;

                opts.Projections.Add(new BeheerVerenigingHistoriekProjection(), ProjectionLifecycle.Async);
                opts.Projections.Add(new BeheerVerenigingDetailProjection(), ProjectionLifecycle.Async);
                opts.Projections.Add(new PowerBiExportProjection(), ProjectionLifecycle.Async);
                opts.Projections.Add(new PowerBiExportDubbelDetectieProjection(), ProjectionLifecycle.Async);
                opts.Projections.Add(new BeheerKboSyncHistoriekProjection(), ProjectionLifecycle.Async);
                opts.Projections.Add(
                    new LocatieLookupProjection(NullLogger<LocatieLookupProjection>.Instance),
                    ProjectionLifecycle.Async
                );
                opts.Projections.Add(
                    new LocatieZonderAdresMatchProjection(NullLogger<LocatieZonderAdresMatchProjection>.Instance),
                    ProjectionLifecycle.Async
                );
                opts.Projections.Add(new BewaartermijnProjection(), ProjectionLifecycle.Async);
                opts.Projections.Add(new VertegenwoordigerProjection(querySessionFunc), ProjectionLifecycle.Async);

                return opts;
            })
            .IntegrateWithWolverine(integration =>
            {
                integration.TransportSchemaName = WellknownSchemaNames.Wolverine;
                integration.MessageStorageSchemaName = WellknownSchemaNames.Wolverine;

                integration.AutoCreate = AutoCreate.None;
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

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions) =>
        $"host={postgreSqlOptions.Host};"
        + $"database={postgreSqlOptions.Database};"
        + $"password={postgreSqlOptions.Password};"
        + $"username={postgreSqlOptions.Username};";
}
