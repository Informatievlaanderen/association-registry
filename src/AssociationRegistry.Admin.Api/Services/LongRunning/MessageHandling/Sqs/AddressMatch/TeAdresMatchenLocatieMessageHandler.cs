namespace AssociationRegistry.Admin.Api.MessageHandling.Sqs.AddressMatch;

using Messages;
using Wolverine;

public class TeAdresMatchenLocatieMessageHandler(IMessageBus messageBus)
{
    public async Task Handle(TeAdresMatchenLocatieMessage message, CancellationToken cancellationToken)
    {
        var command = message.ToCommand();

        await messageBus.InvokeAsync(command, cancellationToken);
    }
}
