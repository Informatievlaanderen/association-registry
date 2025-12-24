

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
using NodaTime;
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
            var envelope = SyncEnvelopeParser.Parse(record.Body);
            var commandMetadata = new CommandMetadata(
                WellknownOvoNumbers.DigitaalVlaanderenOvoNumber,
                SystemClock.Instance.GetCurrentInstant(),
                envelope.CorrelationId);  // ✅ Use from envelope instead of Guid.NewGuid()

            _logger.LogInformation("{MessageProcessor} processing sqs message of type {Type}", nameof(MessageProcessor), envelope.Type);

            using var activity = Telemetry.SyncEnvelopeActivity.Start(envelope);

            switch (envelope.Type)
            {
                case SyncMessageType.SyncKbo:
                    activity.TagAsKboSync(envelope.KboNummer!);
                    await kboSyncHandler.Handle(
                        new CommandEnvelope<SyncKboCommand>(
                            new SyncKboCommand(KboNummer.Create(envelope.KboNummer!)),
                            commandMetadata),
                        verenigingsRepository,
                        cancellationToken);
                    break;

                case SyncMessageType.SyncKsz:
                    activity.TagAsKszSync();
                    await kszSyncHandler.Handle(
                        new SyncKszMessage(
                            Insz.Create(envelope.InszMessage!.Insz),
                            envelope.InszMessage.Overleden,
                            envelope.CorrelationId),
                        cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown message shape for record {record.MessageId}. Body={record.Body}: expected KboNummer OR Insz.");
            }
        }
    }
}
