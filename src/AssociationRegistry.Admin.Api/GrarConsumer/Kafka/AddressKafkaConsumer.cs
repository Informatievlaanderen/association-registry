namespace AssociationRegistry.Admin.Api.GrarConsumer.Kafka;

using Infrastructure.AWS;
using Infrastructure.Extensions;
using Notifications.Messages;
using AssociationRegistry.Notifications;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Confluent.Kafka;
using Grar.GrarUpdates.Fusies;
using Grar.GrarUpdates.Hernummering;
using IdentityModel;
using Marten;
using Polly;
using StraatHernummering;
using System.Text;

public class AddressKafkaConsumer : BackgroundService
{
    private readonly AddressKafkaConfiguration _configuration;
    private readonly IFusieEventProcessor _fusieEventProcessor;
    private readonly ITeHernummerenStraatEventProcessor _teHernummerenStraatEventProcessor;
    private readonly IDocumentStore _store;
    private readonly INotifier _notifier;
    private readonly ILogger<AddressKafkaConsumer> _logger;

    public AddressKafkaConsumer(
        AddressKafkaConfiguration configuration,
        IFusieEventProcessor fusieEventProcessor,
        ITeHernummerenStraatEventProcessor teHernummerenStraatEventProcessor,
        IDocumentStore store,
        INotifier notifier,
        ILogger<AddressKafkaConsumer> logger)
    {
        _configuration = configuration;
        _fusieEventProcessor = fusieEventProcessor;
        _teHernummerenStraatEventProcessor = teHernummerenStraatEventProcessor;
        _store = store;
        _notifier = notifier;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Policy
             .Handle<Exception>()
             .WaitAndRetryForeverAsync(
                  sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(x: 2, retryAttempt < 5 ? retryAttempt : 5)),
                  onRetryAsync: async (exception, _) =>
                  {
                      _logger.LogError(exception, $"{nameof(AddressKafkaConsumer)} failed");
                      await _notifier.Notify(new AdresKafkaConsumerGefaald(exception));
                  })
             .ExecuteAsync(Consume, stoppingToken);
    }

    private async Task Consume(CancellationToken stoppingToken)
    {
        var consumer = FetchConsumer();

        try
        {
            await using var session = _store.LightweightSession();

            consumer.Subscribe(_configuration.TopicPartition.Topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(1000);

                if (await DelayIfNull(result, i: 1000, stoppingToken))
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
                    case AddressWasRetiredBecauseOfMunicipalityMerger addressWasRetiredBecauseOfMunicipalityMerger:
                        await _fusieEventProcessor.Process(
                            addressWasRetiredBecauseOfMunicipalityMerger.AddressPersistentLocalId,
                            addressWasRetiredBecauseOfMunicipalityMerger.NewAddressPersistentLocalId);
                        break;

                    case AddressWasRejectedBecauseOfMunicipalityMerger addressWasRejectedBecauseOfMunicipalityMerger:
                        await _fusieEventProcessor.Process(
                            addressWasRejectedBecauseOfMunicipalityMerger.AddressPersistentLocalId,
                            addressWasRejectedBecauseOfMunicipalityMerger.NewAddressPersistentLocalId);
                        break;

                    case StreetNameWasReaddressed streetNameWasReaddressed:
                        _logger.LogInformation($"{nameof(StreetNameWasReaddressed)} found! Offset: {result.Offset}");

                        await _teHernummerenStraatEventProcessor.Process(
                            TeHernummerenStraatFactory.From(streetNameWasReaddressed), idempotenceKey);

                        break;

                    case null:
                        _logger.LogWarning($"Encountered a message we couldn't deserialize. Offset: {result.Offset}");

                        break;
                }

                consumer.Commit(result);

                await AppendLatestKnownOffset(session, result, stoppingToken);
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
        session.Insert(new AddressKafkaConsumerOffset
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
        var consumer = new ConsumerBuilder<string, string>(_configuration)
                      .SetValueDeserializer(Deserializers.Utf8)
                      .Build();

        var partitionSpecifier = new TopicPartition(_configuration.TopicPartition.Topic, partition: 0);
        var topicPartitionOffsets = consumer.Committed(new List<TopicPartition> { partitionSpecifier }, TimeSpan.FromSeconds(10));

        if (topicPartitionOffsets.FirstOrDefault()?.Offset < _configuration.Offset)
            consumer.Commit(new List<TopicPartitionOffset>
                                { new(partitionSpecifier, _configuration.Offset) });

        return consumer;
    }

    private bool MessageAlreadyProcessed(IDocumentSession session, string idempotenceKey)
        => session.Query<AddressKafkaConsumerOffset>()
                  .Any(m => m.IdempotenceKey == idempotenceKey);

    private string FetchIdempotenceKey(ConsumeResult<string, string> result)
        => result.Message.Headers.TryGetLastBytes(key: "IdempotenceKey", out var idempotenceHeaderAsBytes)
            ? Encoding.UTF8.GetString(idempotenceHeaderAsBytes)
            : result.Message.Value.ToSha512();
}
