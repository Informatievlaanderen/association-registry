namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using CommandHandling.Bewaartermijnen.Acties.Start;
using DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Hosts.Configuration;
using JasperFx;
using JasperFx.CodeGeneration;
using MartenDb;
using MartenDb.Setup;
using Projections.Bewaartermijn.EventHandling;
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

            options.Discovery.IncludeAssembly(typeof(BewaartermijnVertegenwoordigersEventHandler).Assembly);

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
    }

    private static void ConfigureStartBewaartermijn(WolverineOptions options)
    {
        //options.Discovery.IncludeType<StartBewaartermijnMessage>();
        options.Discovery.IncludeType<StartBewaartermijnMessageHandler>();

        options
            .PublishMessage<StartBewaartermijnMessage>()
            .ToPostgresqlQueue(WellknownQueueNames.StartBewaartermijnQueueName);

        //options.Discovery.IncludeType<StartBewaartermijnenVoorVerenigingMessage>();
        options.Discovery.IncludeType<StartBewaartermijnenVoorVerenigingMessageHandler>();

        options
            .PublishMessage<StartBewaartermijnenVoorVerenigingMessage>()
            .ToPostgresqlQueue(WellknownQueueNames.StartBewaartermijnQueueName);
    }
}
