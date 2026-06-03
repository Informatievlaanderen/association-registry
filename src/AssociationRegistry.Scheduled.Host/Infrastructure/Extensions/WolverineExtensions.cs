namespace AssociationRegistry.Scheduled.Host.Infrastructure.Extensions;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Hosts.Configuration.ConfigurationBindings;
using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using JasperFx;
using MartenDb.Setup;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wolverine;
using Wolverine.Postgresql;
using Program = Host.Program;

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
        ConfigureBewaartermijnen(options, wolverineSchema, postgreSqlOptions);
        ConfigureErkenningen(options, wolverineSchema, postgreSqlOptions);
    }

    private static void ConfigureBewaartermijnen(WolverineOptions options, string wolverineSchema, PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string NightlyExpiredBewaartermijnenProcessorQueueName = "nachtelijke-expiredbewaartermijnen-queue";

        options.Discovery.IncludeType<VerloopBewaartermijnCommand>();
        options.Discovery.IncludeType<VerloopBewaartermijnCommandHandler>();

        options.PersistMessagesWithPostgresql(postgreSqlOptions.GetConnectionString(), wolverineSchema)
               .EnableMessageTransport();

        options.PublishMessage<VerloopBewaartermijnCommand>()
               .ToPostgresqlQueue(NightlyExpiredBewaartermijnenProcessorQueueName);

        options.ListenToPostgresqlQueue(NightlyExpiredBewaartermijnenProcessorQueueName);
    }

    private static void ConfigureErkenningen(WolverineOptions options, string wolverineSchema, PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string ActiveerErkenningenProcessorQueueName = "activeer-erkenningen-queue";

        options.Discovery.IncludeType<ActiveerErkenningCommand>();
        options.Discovery.IncludeType<ActiveerErkenningCommandHandler>();

        options.PersistMessagesWithPostgresql(postgreSqlOptions.GetConnectionString(), wolverineSchema)
               .EnableMessageTransport();

        options.PublishMessage<ActiveerErkenningCommand>()
               .ToPostgresqlQueue(ActiveerErkenningenProcessorQueueName);

        options.ListenToPostgresqlQueue(ActiveerErkenningenProcessorQueueName);
    }
}
