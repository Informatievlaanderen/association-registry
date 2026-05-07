namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Wolverine;

public class StartBewaartermijnMessageHandler
{
    public async Task Handle(
        StartBewaartermijnMessage message,
        IMessageBus messageBus,
        CancellationToken cancellationToken
    )
    {
        await messageBus.InvokeAsync(message.ToCommand(), cancellationToken);
    }
}
