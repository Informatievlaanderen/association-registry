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
    private readonly SyncKboCommandHandler _kboSyncHandler;
    private readonly SyncKszMessageHandler _kszSyncHandler;
    private readonly INotifier _notifier;
    private readonly ILogger<MessageProcessor> _logger;

    public MessageProcessor(
        KboSyncConfiguration kboSyncConfiguration,
        SyncKboCommandHandler kboSyncHandler,
        SyncKszMessageHandler kszSyncHandler,
        INotifier notifier,
        ILogger<MessageProcessor> logger)
    {
        _kboSyncConfiguration = kboSyncConfiguration;
        _kboSyncHandler = kboSyncHandler;
        _kszSyncHandler = kszSyncHandler;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task ProcessMessage(
        SQSEvent sqsEvent,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileBucketName)}:{_kboSyncConfiguration.MutationFileBucketName}");
        _logger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileQueueUrl)}:{_kboSyncConfiguration.MutationFileQueueUrl}");
        _logger.LogInformation($"{nameof(_kboSyncConfiguration.SyncQueueUrl)}:{_kboSyncConfiguration.SyncQueueUrl}");

        foreach (var record in sqsEvent.Records)
        {
            try
            {
                _logger.LogInformation("Processing record {Id}", record.MessageId);

                var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;

                var envelope = MagdaEnvelopeParser.Parse(record.Body);

                switch (envelope.Type)
                {
                    case MagdaMessageType.SyncKbo:
                        await _kboSyncHandler.Handle(new CommandEnvelope<SyncKboCommand>(new SyncKboCommand(KboNummer.Create(envelope.KboNummer!)), commandMetadata), cancellationToken);
                        break;

                    case MagdaMessageType.SyncKsz:
                        await _kszSyncHandler.Handle(new SyncKszMessage(Insz.Create(envelope.Insz!)), cancellationToken);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown message shape for record {record.MessageId}. Body={record.Body}: expected KboNummer OR Insz.");
                }
            }
            catch (Exception ex)
            {
                await _notifier.Notify(new KboSyncLambdaGefaald(record.Body, ex));
                throw;
            }
        }
    }
}
