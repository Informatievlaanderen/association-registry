namespace AssociationRegistry.Migrations.Generator;

using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using CommandHandling.DecentraalBeheer.Middleware;
using CommandHandling.Grar.GrarConsumer.Messaging;
using DecentraalBeheer.Vereniging;
using EventStore;
using Framework;
using MartenDb.Setup;
using Serilog;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;

public static class WolverineBootstrap
{
    public static void Configure(WolverineOptions options)
    {
        const string wolverineSchema = "public";


                Log.Logger.Information("Setting up wolverine");
                options.ApplicationAssembly = typeof(Program).Assembly;



                ConfigurePostgresQueues(options, "queues", "host=127.0.0.1;database=verenigingsregister;password=root;username=root;");


                options.UseNewtonsoftForSerialization(settings => settings.ConfigureForVerenigingsregister());
    }

    private static void ConfigurePostgresQueues(
        WolverineOptions options,
        string wolverineSchema,
        string connectionString)
    {
        const string AanvaardDubbeleVerenigingQueueName = "aanvaard-dubbele-vereniging-queue";

        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessage>();
        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessageHandler>();

        options.PersistMessagesWithPostgresql(connectionString, wolverineSchema).EnableMessageTransport();

        options.PublishMessage<AanvaardDubbeleVerenigingMessage>()
               .ToPostgresqlQueue(AanvaardDubbeleVerenigingQueueName);

        options.PublishMessage<ProbeerAdresTeMatchenCommand>()
               .ToPostgresqlQueue("ProbeerAdresQueue");
        options.ListenToPostgresqlQueue(AanvaardDubbeleVerenigingQueueName);



    }
}
