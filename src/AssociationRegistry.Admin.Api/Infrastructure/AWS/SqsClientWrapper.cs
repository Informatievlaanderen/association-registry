namespace AssociationRegistry.Admin.Api.Infrastructure.AWS;

using Acties.GrarConsumer.HeradresseerLocaties;
using Framework;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Kbo;
using System.Text.Json;
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
