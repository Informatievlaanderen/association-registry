namespace AssociationRegistry.Admin.Api.MessageHandling.Postgres.Dubbels;

using Messages;
using Wolverine;

public class CorrigeerAanvaardingDubbeleVerenigingMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(CorrigeerAanvaardingDubbeleVerenigingMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
