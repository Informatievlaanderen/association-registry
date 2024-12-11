namespace AssociationRegistry.Admin.Api.MessageHandling.Postgres.Dubbels;

using Messages;
using Wolverine;

public class VoegDubbelToeMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(VoegDubbelToeMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
