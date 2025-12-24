

namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.SQSEvents;
using DecentraalBeheer.Vereniging;
using Framework;
using Integrations.Slack;
using KboMutations.Configuration;
using Magda.Kbo;
using MagdaSync.SyncKbo;
using MagdaSync.SyncKsz;
using MagdaSync.SyncKsz.Queries;
using Messaging;
using Messaging.Parsers;
using Microsoft.Extensions.Logging;
using Persoonsgegevens;

public class MessageProcessor
{
    private readonly KboSyncConfiguration _kboSyncConfiguration;
    private readonly ILogger<MessageProcessor> _logger;

    public MessageProcessor(
        KboSyncConfiguration kboSyncConfiguration,
        ILogger<MessageProcessor> logger)
    {
        _kboSyncConfiguration = kboSyncConfiguration;
        _logger = logger;
    }

    public async Task ProcessMessage(
        SQSEvent sqsEvent,
        SyncKboCommandHandler kboSyncHandler,
        SyncKszMessageHandler kszSyncHandler,
        IVerenigingsRepository verenigingsRepository,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("{ConfigKey}: {ConfigValue}", nameof(_kboSyncConfiguration.MutationFileBucketName), _kboSyncConfiguration.MutationFileBucketName);
        _logger.LogInformation("{ConfigKey}: {ConfigValue}", nameof(_kboSyncConfiguration.MutationFileQueueUrl), _kboSyncConfiguration.MutationFileQueueUrl);
        _logger.LogInformation("{ConfigKey}: {ConfigValue}", nameof(_kboSyncConfiguration.SyncQueueUrl), _kboSyncConfiguration.SyncQueueUrl);

        foreach (var record in sqsEvent.Records)
        {
            var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;
            var envelope = MagdaEnvelopeParser.Parse(record.Body);

            _logger.LogInformation("{MessageProcessor} processing sqs message of type {Type}", nameof(MessageProcessor), envelope.Type);

            using var activity = Telemetry.SyncMessageActivity.Start(envelope);

            switch (envelope.Type)
            {
                case MagdaMessageType.SyncKbo:
                    activity.TagAsKboSync(envelope.KboNummer!);
                    await kboSyncHandler.Handle(
                        new CommandEnvelope<SyncKboCommand>(
                            new SyncKboCommand(KboNummer.Create(envelope.KboNummer!)),
                            commandMetadata),
                        verenigingsRepository,
                        cancellationToken);
                    break;

                case MagdaMessageType.SyncKsz:
                    activity.TagAsKszSync(envelope.InszMessage!.Insz, envelope.InszMessage.Overleden);
                    await kszSyncHandler.Handle(
                        new SyncKszMessage(
                            Insz.Create(envelope.InszMessage!.Insz),
                            envelope.InszMessage.Overleden),
                        cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown message shape for record {record.MessageId}. Body={record.Body}: expected KboNummer OR Insz.");
            }
        }
    }
}
