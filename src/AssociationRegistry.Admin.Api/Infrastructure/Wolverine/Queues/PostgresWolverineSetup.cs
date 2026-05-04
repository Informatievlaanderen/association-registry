namespace AssociationRegistry.Admin.Api.Infrastructure.Wolverine.Queues;

using global::Wolverine;
using CommandHandling.Bewaartermijnen.Acties.Start;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardCorrectieDubbel;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using CommandHandling.InschrijvingenVertegenwoordigers;
using DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Framework;
using global::Wolverine.Postgresql;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Integrations.Magda.Shared.Exceptions;
using JasperFx.Core;

internal static class PostgresWolverineSetup
{
    const string DubbelCorrectieQueueName = "dubbel-correctie-queue";
    const string DubbelWeigeringQueueName = "dubbel-weigering-queue";

    public static void ConfigureQueues(
        WolverineOptions options,
        string wolverineSchema,
        ConfigurationManager configuration
    )
    {
        var connectionString = configuration.GetPostgreSqlOptionsSection().GetConnectionString();
        var initialRegistreerInschrijvingOptions = configuration.GetInitialRegistreerInschrijvingOptions();

        options.PersistMessagesWithPostgresql(connectionString, wolverineSchema).EnableMessageTransport();

        ConfigureStartBewaartermijn(options);
        ConfigureAanvaardDubbeleVerenigingen(options);
        ConfigureSchrijfVertegenwoordigersInMessageQueue(options, initialRegistreerInschrijvingOptions);
        ConfigureAanvaardCorrectieDubbeleVerenigingMessageQueue(options);
        ConfigureVerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessageQueue(options);
    }

    private static void ConfigureStartBewaartermijn(WolverineOptions options)
    {
        options.Discovery.IncludeType<StartBewaartermijnMessage>();
        options.Discovery.IncludeType<StartBewaartermijnMessageHandler>();

        options.Discovery.IncludeType<StartBewaartermijnenVoorVerenigingMessage>();
        options.Discovery.IncludeType<StartBewaartermijnenVoorVerenigingMessageHandler>();

        options.ListenToPostgresqlQueue(WellknownQueueNames.StartBewaartermijnQueueName);
    }

    private static void ConfigureVerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessageQueue(WolverineOptions options)
    {
        options.Discovery.IncludeType<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage>();
        options.Discovery.IncludeType<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessageHandler>();

        options
            .PublishMessage<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage>()
            .ToPostgresqlQueue(DubbelWeigeringQueueName);

        options.ListenToPostgresqlQueue(DubbelWeigeringQueueName);
    }

    private static void ConfigureAanvaardCorrectieDubbeleVerenigingMessageQueue(WolverineOptions options)
    {
        options.Discovery.IncludeType<AanvaardCorrectieDubbeleVerenigingMessage>();
        options.Discovery.IncludeType<AanvaardCorrectieDubbeleVerenigingMessageHandler>();

        options.PublishMessage<AanvaardCorrectieDubbeleVerenigingMessage>().ToPostgresqlQueue(DubbelCorrectieQueueName);

        options.ListenToPostgresqlQueue(DubbelCorrectieQueueName);
    }

    private static void ConfigureAanvaardDubbeleVerenigingen(WolverineOptions options)
    {
        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessage>();
        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessageHandler>();

        const string naam = "aanvaard-dubbele-vereniging-queue";
        options.PublishMessage<AanvaardDubbeleVerenigingMessage>().ToPostgresqlQueue(naam);
        options.ListenToPostgresqlQueue(naam);
    }

    private static void ConfigureSchrijfVertegenwoordigersInMessageQueue(
        WolverineOptions options,
        InitialRegistreerInschrijvingOptions initialRegistreerInschrijvingOptions
    )
    {
        const string naam = "schrijf-vertegenwoordiger-in-queue";

        options.PublishMessage<CommandEnvelope<SchrijfVertegenwoordigersInMessage>>().ToPostgresqlQueue(naam);

        options
            .ListenToPostgresqlQueue(naam)
            .MaximumParallelMessages(initialRegistreerInschrijvingOptions.MaximumParallelMessages)
            .CircuitBreaker(breakerOptions =>
            {
                breakerOptions.TrackingPeriod = initialRegistreerInschrijvingOptions.TrackingPeriodInSeconds.Seconds();
                breakerOptions.SamplingPeriod = 10.Seconds();
                breakerOptions.FailurePercentageThreshold =
                    initialRegistreerInschrijvingOptions.FailurePercentageThreshold;
                breakerOptions.PauseTime = initialRegistreerInschrijvingOptions.PauseTimeInSeconds.Seconds();
                breakerOptions.Include<MagdaException>();
                breakerOptions.Include<TaskCanceledException>();
            });
    }
}
