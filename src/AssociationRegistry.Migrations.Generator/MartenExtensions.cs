namespace AssociationRegistry.Migrations.Generator;

using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using Marten;
using MartenDb.Setup;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Marten;

public static class MartenExtensions
{
    private const string WolverineSchemaName = "public";

    public static IServiceCollection AddMartenV2(
        this IServiceCollection services)
    {
        var martenConfiguration = services
                                 .AddMarten(
                                      serviceProvider =>
                                      {
                                          var opts = new StoreOptions();

                                          opts
                                             .UsePostgreSqlOptions()
                                             .AddVCodeSequence()
                                             .ConfigureSerialization()
                                             .SetUpOpenTelemetry()
                                             .RegisterAllEventTypes();

                                          opts.RegisterDocumentType<NewTableDocument2>();

                                          opts.Events.StreamIdentity = StreamIdentity.AsString;
                                          opts.Events.MetadataConfig.EnableAll();
                                          opts.Events.AppendMode = EventAppendMode.Quick;

                                          opts.AutoCreateSchemaObjects = AutoCreate.None;

                                          return opts;
                                      })
                                 .IntegrateWithWolverine(integration =>
                                  {
                                      integration.TransportSchemaName = WolverineSchemaName;
                                      integration.MessageStorageSchemaName = WolverineSchemaName;
                                  })
                                 .UseLightweightSessions();

        // if (configuration["ApplyAllDatabaseChangesDisabled"]?.ToLowerInvariant() != "true")
        //     martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        // services.CritterStackDefaults(x =>
        // {
        //     x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
        //     //x.Development.ResourceAutoCreate = AutoCreate.CreateOrUpdate;
        //
        //     x.Production.GeneratedCodeMode = TypeLoadMode.Static;
        //     //x.Production.ResourceAutoCreate = AutoCreate.CreateOrUpdate;
        //     x.Production.SourceCodeWritingEnabled = false;
        // });

        return services;
    }

    public static StoreOptions UsePostgreSqlOptions(this StoreOptions opts)
    {
        opts.Connection("host=127.0.0.1;database=verenigingsregister;password=root;username=root;");

        if (!string.IsNullOrEmpty("public"))
        {
            opts.Events.DatabaseSchemaName = "public";
            opts.DatabaseSchemaName = "public";
        }

        return opts;
    }

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username};";
}

public record NewTableDocument2
{
    public string Id { get; set; }
    public string FULLNAME { get; set; }
}
