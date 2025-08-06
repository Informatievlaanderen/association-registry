namespace AssociationRegistry.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardCorrectieDubbel;

using Wolverine;

public class AanvaardCorrectieDubbeleVerenigingMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(AanvaardCorrectieDubbeleVerenigingMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
