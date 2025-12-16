namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.SQSEvents;
using CommandHandling.MagdaSync.SyncKbo;
using CommandHandling.MagdaSync.SyncKsz;
using DecentraalBeheer.Vereniging;
using Framework;
using Integrations.Slack;
using KboMutations.Configuration;
using Magda.Kbo;
using Magda.Persoon;
using Microsoft.Extensions.Logging;
using Notifications;
using Persoonsgegevens;
using Wolverine;

public class MessageProcessor
{
    private readonly KboSyncConfiguration _kboSyncConfiguration;

    public MessageProcessor(KboSyncConfiguration kboSyncConfiguration)
    {
        _kboSyncConfiguration = kboSyncConfiguration;
    }

    public async Task ProcessMessage(SQSEvent sqsEvent,
        ILoggerFactory loggerFactory,
        IMagdaRegistreerInschrijvingService registreerInschrijvingService,
        IMagdaSyncGeefVerenigingService geefOndernemingService,
        IMagdaGeefPersoonService geefPersoonService,
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository,
        IVerenigingsRepository repository,
        IMessageBus messageBus,
        INotifier notifier,
        CancellationToken cancellationToken)
    {
        var contextLogger = loggerFactory.CreateLogger<MessageProcessor>();
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileBucketName)}:{_kboSyncConfiguration.MutationFileBucketName}");
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileQueueUrl)}:{_kboSyncConfiguration.MutationFileQueueUrl}");
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.SyncQueueUrl)}:{_kboSyncConfiguration.SyncQueueUrl}");

        var logger = loggerFactory.CreateLogger<SyncKboCommandHandler>();

        var kboSyncHandler = new SyncKboCommandHandler(
            repository,
            registreerInschrijvingService,
            geefOndernemingService,
            notifier,
            logger);

        var kszSyncHandler = new SyncKszMessageHandler(
            vertegenwoordigerPersoonsgegevensRepository,
            geefPersoonService,
            messageBus,
            loggerFactory.CreateLogger<SyncKszMessageHandler>());

        foreach (var record in sqsEvent.Records)
        {
            try
            {
                contextLogger.LogInformation("Processing record {Id}", record.MessageId);

                var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;

                var env = MagdaEnvelopeParser.Parse(record.Body);

                switch (env.Type)
                {
                    case MagdaMessageType.SyncKbo:
                        await kboSyncHandler.Handle( new CommandEnvelope<SyncKboCommand>(new SyncKboCommand(KboNummer.Create(env.KboNummer!)), commandMetadata), cancellationToken);
                        break;

                    case MagdaMessageType.SyncKsz:
                        await kszSyncHandler.Handle(new SyncKszMessage(Insz.Create(env.Insz!)), cancellationToken);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown message shape for record {record.MessageId}. Body={record.Body}: expected KboNummer OR Insz.");
                }
            }
            catch (Exception ex)
            {
                await notifier.Notify(new KboSyncLambdaGefaald(record.Body, ex));
                throw;
            }
        }
    }
}
