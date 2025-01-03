namespace AssociationRegistry.Admin.Api.MessageHandling.Postgres.Dubbels;

using Messages;
using Wolverine;

public class AanvaardCorrectieDubbeleVerenigingMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(AanvaardCorrectieDubbeleVerenigingMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
