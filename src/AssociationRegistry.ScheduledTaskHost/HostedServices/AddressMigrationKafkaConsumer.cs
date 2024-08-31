namespace AssociationRegistry.ScheduledTaskHost.HostedServices;

using AssociationRegistry.Notifications;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Confluent.Kafka;
using Helpers;
using IdentityModel;
using Infrastructure.AWS;
using Infrastructure.Extensions;
using Marten;
using Marten.Schema;
using Polly;
using Polly.Registry;
using System.Text;

public class AddressMigrationKafkaConsumer : BackgroundService
{
    private readonly AddressMigrationKafkaConsumerConfiguration _consumerConfiguration;
    private readonly TeHeradresserenLocatiesMapper _teHeradresserenLocatiesMapper;
    private readonly SqsClientWrapper _sqsClientWrapper;
    private readonly ResiliencePipeline _pipeline;
    private readonly IDocumentStore _store;
    private readonly INotifier _notifier;
    private readonly ILogger<AddressMigrationKafkaConsumer> _logger;

    public AddressMigrationKafkaConsumer(
        AddressMigrationKafkaConsumerConfiguration consumerConfiguration,
        TeHeradresserenLocatiesMapper teHeradresserenLocatiesMapper,
        SqsClientWrapper sqsClientWrapper,
        ResiliencePipelineProvider<string> pipelineProvider,
        IDocumentStore store,
        INotifier notifier,
        ILogger<AddressMigrationKafkaConsumer> logger)
    {
        _consumerConfiguration = consumerConfiguration;
        _teHeradresserenLocatiesMapper = teHeradresserenLocatiesMapper;
        _sqsClientWrapper = sqsClientWrapper;
        _pipeline = pipelineProvider.GetPipeline(nameof(AddressMigrationKafkaConsumer));
        _store = store;
        _notifier = notifier;
        _logger = logger;
    }

    public AddressMigrationKafkaConsumer(ResiliencePipelineProvider<string> pipelineProvider, ILogger<AddressMigrationKafkaConsumer> logger)
    {
        _pipeline = pipelineProvider.GetPipeline(nameof(AddressMigrationKafkaConsumer));
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            await ConsumeAsync(cancellationToken);

            return ValueTask.CompletedTask;
        });
    }

    private async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        var consumer = FetchConsumer();
        await using var session = _store.LightweightSession();

        try
        {
            consumer.Subscribe(_consumerConfiguration.TopicPartition.Topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                var result = consumer.Consume(1000);

                if (await DelayIfNull(result, i: 1000, cancellationToken))
                    continue;

                var idempotenceKey = FetchIdempotenceKey(result);

                if (MessageAlreadyProcessed(session, idempotenceKey))
                {
                    consumer.Commit(result);

                    continue;
                }

                var message = result.Message.GetTypedMessage();

                switch (message)
                {
                    case StreetNameWasReaddressed streetNameWasReaddressed:
                        _logger.LogInformation($"{nameof(StreetNameWasReaddressed)} found! Offset: {result.Offset}");

                        var teHeradresserenLocatiesMessages =
                            await _teHeradresserenLocatiesMapper.ForAddress(streetNameWasReaddressed.ReaddressedHouseNumbers,
                                                                            idempotenceKey);

                        foreach (var teHeradresserenLocatiesMessage in teHeradresserenLocatiesMessages)
                        {
                            await _sqsClientWrapper.QueueReaddressMessage(teHeradresserenLocatiesMessage);
                        }

                        break;

                    case null:
                        _logger.LogWarning($"Encountered a message we couldn't deserialize. Offset: {result.Offset}");

                        break;
                }

                consumer.Commit(result);

                await AppendLatestKnownOffset(session, result, cancellationToken);
            }
        }
        finally
        {
            consumer.Unsubscribe();
        }
    }

    private async Task<bool> DelayIfNull(ConsumeResult<string, string>? result, int i, CancellationToken cancellationToken)
    {
        if (result is not null)
            return false;

        await Task.Delay(i, cancellationToken);

        return true;
    }

    private async Task AppendLatestKnownOffset(
        IDocumentSession session,
        ConsumeResult<string, string> result,
        CancellationToken cancellationToken)
    {
        session.Insert(new AddressMigrationKafkaConsumerOffset
        {
            Timestamp = result.Message.Timestamp.UnixTimestampMs,
            DateTime = result.Message.Timestamp.UtcDateTime,
            IdempotenceKey = FetchIdempotenceKey(result),
            Key = result.Message.Key,
            Offset = result.Offset,
        });

        await session.SaveChangesAsync(cancellationToken);
    }

    private IConsumer<string, string> FetchConsumer()
    {
        var consumer = new ConsumerBuilder<string, string>(_consumerConfiguration)
                      .SetValueDeserializer(Deserializers.Utf8)
                      .Build();

        var partitionSpecifier = new TopicPartition(_consumerConfiguration.TopicPartition.Topic, partition: 0);
        var topicPartitionOffsets = consumer.Committed(new List<TopicPartition> { partitionSpecifier }, TimeSpan.FromSeconds(10));

        if (topicPartitionOffsets.FirstOrDefault()?.Offset < _consumerConfiguration.Offset)
            consumer.Commit(new List<TopicPartitionOffset>
                                { new(partitionSpecifier, _consumerConfiguration.Offset) });

        return consumer;
    }

    private bool MessageAlreadyProcessed(IDocumentSession session, string idempotenceKey)
        => session.Query<AddressMigrationKafkaConsumerOffset>()
                  .Any(m => m.IdempotenceKey == idempotenceKey);

    private string FetchIdempotenceKey(ConsumeResult<string, string> result)
        => result.Message.Headers.TryGetLastBytes(key: "IdempotenceKey", out var idempotenceHeaderAsBytes)
            ? Encoding.UTF8.GetString(idempotenceHeaderAsBytes)
            : result.Message.Value.ToSha512();
}

public record AddressMigrationKafkaConsumerOffset
{
    [Identity] public string IdempotenceKey { get; set; }
    public long Timestamp { get; set; }
    public DateTime DateTime { get; set; }
    public string Key { get; init; }
    public long Offset { get; init; }
}
