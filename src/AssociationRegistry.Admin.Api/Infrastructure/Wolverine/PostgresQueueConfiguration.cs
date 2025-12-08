namespace AssociationRegistry.Admin.Api.Infrastructure.Wolverine;

using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using CommandHandling.InschrijvingenVertegenwoordigers;
using Framework;
using global::Wolverine;
using global::Wolverine.Postgresql;
using HostedServices.InitialRegistreerInschrijvingVertegenwoordigers;
using Hosts.Configuration;

public static class PostgresQueueConfiguration
{
    public static void ConfigurePostgresQueues(
        WolverineOptions options,
        string wolverineSchema,
        ConfigurationManager configuration)
    {
        var connectionString = configuration.GetPostgreSqlOptionsSection().GetConnectionString();

        options.PersistMessagesWithPostgresql(connectionString, wolverineSchema).EnableMessageTransport();

        ConfigureAanvaardDubbeleVerenigingen(options);
        ConfigureSchrijfVertegenwoordigersInMessageQueue(options);
    }

    private static void ConfigureAanvaardDubbeleVerenigingen(WolverineOptions options)
    {
        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessage>();
        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessageHandler>();

        const string Naam = "aanvaard-dubbele-vereniging-queue";
        options.PublishMessage<AanvaardDubbeleVerenigingMessage>()
               .ToPostgresqlQueue(Naam);
        options.ListenToPostgresqlQueue(Naam);
    }

    private static void ConfigureSchrijfVertegenwoordigersInMessageQueue(WolverineOptions options)
    {
        options.Discovery.IncludeType<CommandEnvelope<SchrijfVertegenwoordigersInMessage>>();
        options.Discovery.IncludeType<SchrijfVertegenwoordigersInMessageHandler>();

        const string Naam = "schrijf-vertegenwoordiger-in-queue";
        options.PublishMessage<CommandEnvelope<AanvaardDubbeleVerenigingMessage>>()
               .ToPostgresqlQueue(Naam);
        options.ListenToPostgresqlQueue(Naam);
    }
}
