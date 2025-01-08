namespace AssociationRegistry.Acties.GrarConsumer;

using Wolverine;

public class OverkoepelendeGrarConsumerMessageHandler
{
    private readonly IMessageBus _bus;

    public OverkoepelendeGrarConsumerMessageHandler(IMessageBus bus)
    {
        _bus = bus;
    }

    public async Task Handle(OverkoepelendeGrarConsumerMessage message)
    {
        if (message.HeradresseerLocatiesMessage is not null)
            await _bus.InvokeAsync(message.HeradresseerLocatiesMessage, CancellationToken.None);

        if (message.OntkoppelLocatiesMessage is not null)
            await _bus.InvokeAsync(message.OntkoppelLocatiesMessage, CancellationToken.None);
    }
}
