namespace AssociationRegistry.Admin.Api.Infrastructure.AWS;

using Framework;
using Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using Kbo;
using Wolverine;

public class SqsClientWrapper : ISqsClientWrapper
{
    private readonly IMessageBus _messageBus;

    public SqsClientWrapper(IMessageBus messageBus)
    {
        _messageBus = messageBus;
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
