namespace AssociationRegistry.Admin.Api.GrarSync;

using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Confluent.Kafka;
using Constants;
using IdentityModel;
using Infrastructure.AWS;
using Infrastructure.Extensions;
using Marten;
using System.Text;

public class AddressKafkaConsumer : BackgroundService
{
    private readonly AddressKafkaConfiguration _configuration;
    private readonly TeHeradresserenLocatiesMapper _teHeradresserenLocatiesMapper;
    private readonly SqsClientWrapper _sqsClientWrapper;
    private readonly IDocumentStore _store;
    private readonly ILogger<AddressKafkaConsumer> _logger;

    public AddressKafkaConsumer(
        AddressKafkaConfiguration configuration,
        TeHeradresserenLocatiesMapper teHeradresserenLocatiesMapper,
        SqsClientWrapper sqsClientWrapper,
        IDocumentStore store,
        ILogger<AddressKafkaConsumer> logger)
    {
        _configuration = configuration;
        _teHeradresserenLocatiesMapper = teHeradresserenLocatiesMapper;
        _sqsClientWrapper = sqsClientWrapper;
        _store = store;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = FetchConsumer();
        try
        {
            await using var session = _store.LightweightSession();

            consumer.Subscribe(_configuration.TopicPartition.Topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(1000);

                if (await DelayIfNull(result, 1000, stoppingToken)) continue;

                var idempotenceKey = FetchIdempotenceKey(result);

                if (IsIdempotenceKeyAllowed(session, idempotenceKey))
                {
                    var message = result.Message.GetTypedMessage();

                    _logger.LogInformation($"Consumer: {consumer.MemberId}, Offset: {result.Offset}, Message: {message.GetType().Name}");

                    switch (message)
                    {
                        case StreetNameWasReaddressed streetNameWasReaddressed:
                            _logger.LogInformation($"########## {nameof(StreetNameWasReaddressed)} found! ##########");
                            _logger.LogInformation($"Consumer: {consumer.MemberId}, Offset: {result.Offset}, Partition: {result.Partition}");

                            var teHeradresserenLocatiesMessages = await _teHeradresserenLocatiesMapper.ForAddress(streetNameWasReaddressed.ReaddressedHouseNumbers, idempotenceKey);

                            foreach (var teHeradresserenLocatiesMessage in teHeradresserenLocatiesMessages)
                            {
                                await _sqsClientWrapper.QueueReaddressMessage(teHeradresserenLocatiesMessage);
                            }
                            break;
                    }
                }

                consumer.Commit(result);

                // await AppendLatestKnownOffset(session, result, stoppingToken);
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
        session.Events.Append(WellknownStreamNames.AddressKafkaConsumerOffset, new AddressKafkaConsumerOffset
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

        var partitionSpecifier = new TopicPartition(_configuration.TopicPartition.Topic, 0);
        var topicPartitionOffsets = consumer.Committed(new List<TopicPartition> { partitionSpecifier }, TimeSpan.FromSeconds(10));

        if (topicPartitionOffsets.FirstOrDefault()?.Offset < _configuration.Offset)
            consumer.Commit(new List<TopicPartitionOffset>() { new(partitionSpecifier, _configuration.Offset) });

        return consumer;
    }

    private bool IsIdempotenceKeyAllowed(IDocumentSession session, string idempotenceKey)
        => true;
        // => !session.Events
        //            .QueryRawEventDataOnly<AddressKafkaConsumerOffset>()
        //            .Any(m => m.IdempotenceKey == idempotenceKey);

    private string FetchIdempotenceKey(ConsumeResult<string, string> result)
        => result.Message.Headers.TryGetLastBytes("IdempotenceKey", out var idempotenceHeaderAsBytes)
            ? Encoding.UTF8.GetString(idempotenceHeaderAsBytes)
            : result.Message.Value.ToSha512();
}
