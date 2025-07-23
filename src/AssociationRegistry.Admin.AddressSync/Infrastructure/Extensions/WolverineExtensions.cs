namespace AssociationRegistry.Admin.AddressSync.Infrastructure.Extensions;

using Grar.NightlyAdresSync.SyncAdresLocaties;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx.CodeGeneration;
using MessageHandling.Sqs.AddressSync;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Vereniging;
using Wolverine;
using Wolverine.Postgresql;

public static class WolverineExtensions
{
    public static void AddWolverine(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string wolverineSchema = "public";

        services.AddWolverine(
            (options) =>
            {
                Log.Logger.Information("Setting up wolverine");
                options.ApplicationAssembly = typeof(Program).Assembly;
                options.Discovery.IncludeAssembly(typeof(Vereniging).Assembly);

                //TODO: static
                options.CodeGeneration.TypeLoadMode = TypeLoadMode.Dynamic;

                ConfigurePostgresQueues(options, wolverineSchema, postgreSqlOptions);
            });
    }

    private static void ConfigurePostgresQueues(
        WolverineOptions options,
        string wolverineSchema,
        PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string NightlyAddressSyncQueueName = "Nachtelijke-AdresSync-queue";

        options.Discovery.IncludeType<TeSynchroniserenLocatieAdresMessage>();
        options.Discovery.IncludeType<TeSynchroniserenLocatieAdresMessageHandler>();
        options.Discovery.IncludeType<SyncAdresLocatiesCommand>();
        options.Discovery.IncludeType<SyncAdresLocatiesCommandHandler>();

        options.PersistMessagesWithPostgresql(postgreSqlOptions.GetConnectionString(), wolverineSchema).EnableMessageTransport();

        options.PublishMessage<TeSynchroniserenLocatieAdresMessage>()
               .ToPostgresqlQueue(NightlyAddressSyncQueueName);
        options.ListenToPostgresqlQueue(NightlyAddressSyncQueueName);
    }
}
