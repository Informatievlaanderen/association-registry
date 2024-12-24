namespace AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;

using AssociationRegistry.Messages;
using Wolverine;

public class TeSynchroniserenLocatieAdresMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(TeSynchroniserenLocatieAdresMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
