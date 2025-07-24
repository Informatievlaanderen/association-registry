namespace AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;

using Messages;
using Wolverine;

public class TeSynchroniserenLocatieAdresMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(TeSynchroniserenLocatieAdresMessage message)
    {
        var command = message.ToCommand();

        await messageBus.SendAsync(command);
    }
}
