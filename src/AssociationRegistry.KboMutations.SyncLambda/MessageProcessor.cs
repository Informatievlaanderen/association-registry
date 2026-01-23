namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.SQSEvents;
using DecentraalBeheer.Vereniging;
using Framework;
using Framework.EventMetadata;
using Integrations.Slack;
using KboMutations.Configuration;
using Magda.Kbo;
using MagdaSync.SyncKbo;
using MagdaSync.SyncKsz;
using MagdaSync.SyncKsz.Queries;
using MartenDb.Store;
using Messaging;
using Messaging.Parsers;
using Microsoft.Extensions.Logging;
using NodaTime;
using Persoonsgegevens;

public class MessageProcessor
{
    private readonly KboSyncConfiguration _kboSyncConfiguration;
    private readonly ILogger<MessageProcessor> _logger;

    public MessageProcessor(KboSyncConfiguration kboSyncConfiguration, ILogger<MessageProcessor> logger)
    {
        _kboSyncConfiguration = kboSyncConfiguration;
        _logger = logger;
    }

    public async Task ProcessMessage(
        SQSEvent sqsEvent,
        SyncKboCommandHandler kboSyncHandler,
        SyncKszMessageHandler kszSyncHandler,
        IAggregateSession aggregateSession,
        IVerenigingStateQueryService verenigingStateQueryService,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "{ConfigKey}: {ConfigValue}",
            nameof(_kboSyncConfiguration.MutationFileBucketName),
            _kboSyncConfiguration.MutationFileBucketName
        );
        _logger.LogInformation(
            "{ConfigKey}: {ConfigValue}",
            nameof(_kboSyncConfiguration.MutationFileQueueUrl),
            _kboSyncConfiguration.MutationFileQueueUrl
        );
        _logger.LogInformation(
            "{ConfigKey}: {ConfigValue}",
            nameof(_kboSyncConfiguration.SyncQueueUrl),
            _kboSyncConfiguration.SyncQueueUrl
        );

        foreach (var record in sqsEvent.Records)
        {
            var envelope = SyncEnvelopeParser.Parse(record.Body);

            _logger.LogInformation(
                "{MessageProcessor} processing sqs message of type {Type}",
                nameof(MessageProcessor),
                envelope.Type
            );

            using var activity = Telemetry.SyncEnvelopeActivity.Start(envelope);

            switch (envelope.Type)
            {
                case SyncMessageType.SyncKbo:
                {
                    var additionalMetadata = new EventMetadataCollection()
                        .WithTrace(envelope.ParentTraceContext?.TraceId.ToString())
                        .WithSource(SourceFileMetadata.KboSync(envelope.SourceFileName));

                    var commandMetadata = new CommandMetadata(
                        WellknownOvoNumbers.DigitaalVlaanderenOvoNumber,
                        SystemClock.Instance.GetCurrentInstant(),
                        envelope.CorrelationId,
                        null,
                        additionalMetadata
                    );

                    activity.TagAsKboSync(envelope.KboNummer!);
                    await kboSyncHandler.Handle(
                        new CommandEnvelope<SyncKboCommand>(
                            new SyncKboCommand(KboNummer.Create(envelope.KboNummer!)),
                            commandMetadata
                        ),
                        aggregateSession,
                        verenigingStateQueryService,
                        cancellationToken
                    );
                    break;
                }

                case SyncMessageType.SyncKsz:
                {
                    var additionalMetadata = new EventMetadataCollection()
                        .WithTrace(envelope.ParentTraceContext?.TraceId.ToString())
                        .WithSource(SourceFileMetadata.KszSync(envelope.SourceFileName));

                    var commandMetadata = new CommandMetadata(
                        WellknownOvoNumbers.DigitaalVlaanderenOvoNumber,
                        SystemClock.Instance.GetCurrentInstant(),
                        envelope.CorrelationId,
                        null,
                        additionalMetadata
                    );

                    activity.TagAsKszSync();
                    await kszSyncHandler.Handle(
                        new CommandEnvelope<SyncKszMessage>(
                            new SyncKszMessage(
                                Insz.Create(envelope.InszMessage!.Insz),
                                envelope.InszMessage.Overleden,
                                envelope.CorrelationId
                            ),
                            commandMetadata
                        ),
                        cancellationToken
                    );

                    break;
                }

                default:
                    throw new InvalidOperationException(
                        $"Unknown message shape for record {record.MessageId}. Body={record.Body}: expected KboNummer OR Insz."
                    );
            }
        }
    }
}
