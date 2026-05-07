namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Wolverine;

public class StartBewaartermijnenVoorVerenigingMessageHandler
{
    public async Task Handle(
        StartBewaartermijnenVoorVerenigingMessage message,
        IMessageBus messageBus,
        CancellationToken cancellationToken
    )
    {
        await messageBus.InvokeAsync(message.ToCommand(), cancellationToken);
    }
}
