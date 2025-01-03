namespace AssociationRegistry.Admin.Api.MessageHandling.Postgres.Dubbels;

using Messages;
using Wolverine;

public class VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
