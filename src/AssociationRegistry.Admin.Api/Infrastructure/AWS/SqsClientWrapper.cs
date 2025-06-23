namespace AssociationRegistry.Admin.Api.Infrastructure.AWS;

using Framework;
using Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using Kbo;
using Middleware;
using Wolverine;

public class SqsClientWrapper : ISqsClientWrapper
{
    private readonly IMessageBus _messageBus;
    private readonly ICorrelationIdProvider _correlationIdProvider;

    public SqsClientWrapper(IMessageBus messageBus, ICorrelationIdProvider correlationIdProvider)
    {
        _messageBus = messageBus;
        _correlationIdProvider = correlationIdProvider;
    }

    public async Task QueueReaddressMessage(HeradresseerLocatiesMessage message)
    {
        await QueueMessage(message);
    }

    public async Task QueueMessage<TMessage>(TMessage message)
    {
        await _messageBus.SendAsync(message);
    }

    public async Task QueueKboNummerToSynchronise(string kboNummer)
    {
        await QueueMessage(new TeSynchroniserenKboNummerMessage(kboNummer));
    }
}
