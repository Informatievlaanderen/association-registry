namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;

using Wolverine;

public class VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
