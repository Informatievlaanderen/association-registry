namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;

using System.Threading;
using System.Threading.Tasks;
using Wolverine;

public class AanvaardDubbeleVerenigingMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(AanvaardDubbeleVerenigingMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
