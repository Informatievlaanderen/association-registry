namespace AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;

using CommandHandling.Grar.NightlyAdresSync.SyncAdresLocaties;
using Integrations.Grar.Integration.Messages;
using Wolverine;

public class TeSynchroniserenLocatieAdresMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(TeSynchroniserenLocatieAdresMessage message, CancellationToken cancellationToken)
    {
        var command = new SyncAdresLocatiesCommand(
            message.VCode,
            message.LocatiesWithAdres,
            message.IdempotenceKey);

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
