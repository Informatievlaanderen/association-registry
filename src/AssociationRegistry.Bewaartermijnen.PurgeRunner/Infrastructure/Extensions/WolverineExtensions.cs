namespace AssociationRegistry.Bewaartermijnen.PurgeRunner.Infrastructure.Extensions;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Hosts.Configuration.ConfigurationBindings;
using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using JasperFx;
using MartenDb.Setup;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wolverine;
using Wolverine.Postgresql;

public static class WolverineExtensions
{
    public static void AddWolverine(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string wolverineSchema = "public";

        services.AddWolverine((options) =>
        {
            Log.Logger.Information("Setting up wolverine");
            options.ApplicationAssembly = typeof(Program).Assembly;
            options.Discovery.IncludeAssembly(typeof(Vereniging).Assembly);

            options.AutoBuildMessageStorageOnStartup = AutoCreate.All;
            options.UseNewtonsoftForSerialization(settings => settings.ConfigureForVerenigingsregister());

            ConfigurePostgresQueues(options, wolverineSchema, postgreSqlOptions);
        });
    }

    private static void ConfigurePostgresQueues(
        WolverineOptions options,
        string wolverineSchema,
        PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string NightlyExpiredBewaartermijnenProcessorQueueName = "nachtelijke-expiredbewaartermijnen-queue";

        options.Discovery.IncludeType<VerwijderVertegenwoordigerPersoonsgegevensCommand>();
        options.Discovery.IncludeType<VerwijderVertegenwoordigerPersoonsgegevensCommandHandler>();

        options.PersistMessagesWithPostgresql(postgreSqlOptions.GetConnectionString(), wolverineSchema)
               .EnableMessageTransport();

        options.PublishMessage<VerwijderVertegenwoordigerPersoonsgegevensCommand>()
               .ToPostgresqlQueue(NightlyExpiredBewaartermijnenProcessorQueueName);

        options.ListenToPostgresqlQueue(NightlyExpiredBewaartermijnenProcessorQueueName);
    }
}
