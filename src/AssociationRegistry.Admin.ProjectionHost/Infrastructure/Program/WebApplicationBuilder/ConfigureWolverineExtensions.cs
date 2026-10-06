namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using CommandHandling.Bewaartermijnen.Acties.Start;
using DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using DecentraalBeheer.Vereniging.Geotags.Messages;
using Hosts.Configuration;
using JasperFx;
using JasperFx.CodeGeneration;
using MartenDb;
using MartenDb.Setup;
using Projections.EventHandling;
using Projections.EventHandling.Bewaartermijnen;
using Projections.EventHandling.Geotags;
using Serilog;
using Wolverine;
using Wolverine.Postgresql;
using WebApplicationBuilder = Microsoft.AspNetCore.Builder.WebApplicationBuilder;

public static class ConfigureWolverineExtensions
{
    public static void AddWolverine(this WebApplicationBuilder builder)
    {
        const string wolverineSchema = "public";

        builder.Host.UseWolverine(options =>
        {
            Log.Logger.Information("Setting up wolverine");

            options.AutoBuildMessageStorageOnStartup = AutoCreate.All;
            options.UseNewtonsoftForSerialization(settings => settings.ConfigureForVerenigingsregister());
            options.Discovery.IncludeType(typeof(BewaartermijnVertegenwoordigersEventHandler));
            options.Discovery.IncludeType(typeof(HerberekenGeotagsEventHandler));

            ConfigureQueues(options, WellknownSchemaNames.Wolverine, builder.Configuration);
        });

        builder.Services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.SourceCodeWritingEnabled = false;
        });
    }

    private static void ConfigureQueues(
        WolverineOptions options,
        string wolverineSchema,
        ConfigurationManager configuration
    )
    {
        var connectionString = configuration.GetPostgreSqlOptionsSection().GetConnectionString();

        options.PersistMessagesWithPostgresql(connectionString, wolverineSchema).EnableMessageTransport();

        ConfigureStartBewaartermijn(options);
        ConfigureHerberekenGeotags(options);
    }

    private static void ConfigureStartBewaartermijn(WolverineOptions options)
    {
        options
            .PublishMessage<StartBewaartermijnMessage>()
            .ToPostgresqlQueue(WellknownQueueNames.StartBewaartermijnQueueName);

        options
            .PublishMessage<StartBewaartermijnenVoorVerenigingMessage>()
            .ToPostgresqlQueue(WellknownQueueNames.StartBewaartermijnQueueName);
    }

    private static void ConfigureHerberekenGeotags(WolverineOptions options)
    {
        options
            .PublishMessage<HerberekenGeotagsMessage>()
            .ToPostgresqlQueue(WellknownQueueNames.HerberekenGeotagsQueueName);
    }
}
