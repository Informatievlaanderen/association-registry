namespace AssociationRegistry.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;

using Wolverine;

public class AanvaardDubbeleVerenigingMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(AanvaardDubbeleVerenigingMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
