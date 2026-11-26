namespace AssociationRegistry.Scheduled.Host.Infrastructure.Extensions;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.VervalRelatieaanvraag;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using MartenDb.Setup;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wolverine;
using Wolverine.Newtonsoft;
using Wolverine.Postgresql;
using Program = Host.Program;

public static class WolverineExtensions
{
    public const string VervalRelatieaanvragenQueueName = "verval-relatieaanvragen-queue";

    public static void AddWolverine(this IServiceCollection services, PostgreSqlOptionsSection postgreSqlOptions)
    {
        services.AddWolverine(options => Configure(options, postgreSqlOptions));
    }

    /// <summary>Public for the tests: they read the routing and the serializer from it, without starting a host.</summary>
    public static void Configure(WolverineOptions options, PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string wolverineSchema = "public";

        Log.Logger.Information("Setting up wolverine");
        options.ApplicationAssembly = typeof(Program).Assembly;
        options.Discovery.IncludeAssembly(typeof(Vereniging).Assembly);

        // De dubbel import markeert de dubbele en de authentieke vereniging samen in dit proces.
        options.Discovery.IncludeType<MarkeerAlsDubbelVanCommandHandler>();

        options.AutoBuildMessageStorageOnStartup = AutoCreate.All;
        options.UseNewtonsoftForSerialization(settings => settings.ConfigureForVerenigingsregister());

        ConfigurePostgresQueues(options, wolverineSchema, postgreSqlOptions);
    }

    private static void ConfigurePostgresQueues(
        WolverineOptions options,
        string wolverineSchema,
        PostgreSqlOptionsSection postgreSqlOptions
    )
    {
        ConfigureBewaartermijnen(options, wolverineSchema, postgreSqlOptions);
        ConfigureErkenningen(options, wolverineSchema, postgreSqlOptions);
        ConfigureVervalRelatieaanvragen(options);
    }

    /// <summary>
    ///     The rule names the type that the processor sends, the envelope: a rule for the bare command does not match it,
    ///     and Wolverine falls back to a local queue in memory. Sender and listener are durable: the command is saved in
    ///     Postgres before the handler runs, and survives a restart.
    /// </summary>
    private static void ConfigureVervalRelatieaanvragen(WolverineOptions options)
    {
        options.Discovery.IncludeType<VervalRelatieaanvraagCommandHandler>();

        options
            .PublishMessage<CommandEnvelope<VervalRelatieaanvraagCommand>>()
            .ToPostgresqlQueue(VervalRelatieaanvragenQueueName)
            .UseDurableOutbox();

        options.ListenToPostgresqlQueue(VervalRelatieaanvragenQueueName).UseDurableInbox();
    }

    private static void ConfigureBewaartermijnen(
        WolverineOptions options,
        string wolverineSchema,
        PostgreSqlOptionsSection postgreSqlOptions
    )
    {
        const string NightlyExpiredBewaartermijnenProcessorQueueName = "nachtelijke-expiredbewaartermijnen-queue";

        options.Discovery.IncludeType<VerloopBewaartermijnCommand>();
        options.Discovery.IncludeType<VerloopBewaartermijnCommandHandler>();

        options
            .PersistMessagesWithPostgresql(postgreSqlOptions.GetConnectionString(), wolverineSchema)
            .EnableMessageTransport();

        options
            .PublishMessage<VerloopBewaartermijnCommand>()
            .ToPostgresqlQueue(NightlyExpiredBewaartermijnenProcessorQueueName);

        options.ListenToPostgresqlQueue(NightlyExpiredBewaartermijnenProcessorQueueName);
    }

    private static void ConfigureErkenningen(
        WolverineOptions options,
        string wolverineSchema,
        PostgreSqlOptionsSection postgreSqlOptions
    )
    {
        const string ActiveerErkenningenProcessorQueueName = "activeer-erkenningen-queue";
        const string VerloopErkenningenProcessorQueueName = "verloop-erkenningen-queue";

        ConfigureActiveerErkenning(options, ActiveerErkenningenProcessorQueueName);
        ConfigureVerloopErkenning(options, VerloopErkenningenProcessorQueueName);

        options
            .PersistMessagesWithPostgresql(postgreSqlOptions.GetConnectionString(), wolverineSchema)
            .EnableMessageTransport();
    }

    private static void ConfigureActiveerErkenning(WolverineOptions options, string queueName)
    {
        options.Discovery.IncludeType<ActiveerErkenningCommand>();
        options.Discovery.IncludeType<ActiveerErkenningCommandHandler>();

        options.PublishMessage<ActiveerErkenningCommand>().ToPostgresqlQueue(queueName);

        options.ListenToPostgresqlQueue(queueName);
    }

    private static void ConfigureVerloopErkenning(WolverineOptions options, string queueName)
    {
        options.Discovery.IncludeType<VerloopErkenningCommand>();
        options.Discovery.IncludeType<VerloopErkenningCommandHandler>();

        options.PublishMessage<VerloopErkenningCommand>().ToPostgresqlQueue(queueName);

        options.ListenToPostgresqlQueue(queueName);
    }
}
