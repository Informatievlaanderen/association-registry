

namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.SQSEvents;
using CommandHandling.MagdaSync.SyncKbo;
using CommandHandling.MagdaSync.SyncKsz;
using DecentraalBeheer.Vereniging;
using Framework;
using Integrations.Slack;
using KboMutations.Configuration;
using Magda.Kbo;
using Microsoft.Extensions.Logging;
using Persoonsgegevens;

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
        IVerenigingsRepository verenigingsRepository,
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository,
        INotifier notifier,
        CancellationToken cancellationToken)
    {
        var contextLogger = loggerFactory.CreateLogger<MessageProcessor>();
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileBucketName)}:{_kboSyncConfiguration.MutationFileBucketName}");
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileQueueUrl)}:{_kboSyncConfiguration.MutationFileQueueUrl}");
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.SyncQueueUrl)}:{_kboSyncConfiguration.SyncQueueUrl}");

        var logger = loggerFactory.CreateLogger<SyncKboCommandHandler>();

        var kboSyncHandler = new SyncKboCommandHandler(
            registreerInschrijvingService,
            geefOndernemingService,
            notifier,
            logger
            );

        var kszSyncHandler = new SyncKszMessageHandler(
            vertegenwoordigerPersoonsgegevensRepository,
            verenigingsRepository,
            loggerFactory.CreateLogger<SyncKszMessageHandler>()
            );

        foreach (var record in sqsEvent.Records)
        {
            var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;

            var envelope = MagdaEnvelopeParser.Parse(record.Body);

            switch (envelope.Type)
            {
                case MagdaMessageType.SyncKbo:
                    await kboSyncHandler.Handle(new CommandEnvelope<SyncKboCommand>(new SyncKboCommand(KboNummer.Create(envelope.KboNummer!)), commandMetadata), verenigingsRepository, cancellationToken);
                    break;

                case MagdaMessageType.SyncKsz:
                    await kszSyncHandler.Handle(new SyncKszMessage(Insz.Create(envelope.InszMessage!.Insz), envelope.InszMessage.Overleden), cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown message shape for record {record.MessageId}. Body={record.Body}: expected KboNummer OR Insz.");
            }
        }
    }
}
