namespace AssociationRegistry.CommandHandling.Geotags;

using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags.Messages;
using AssociationRegistry.Framework;
using Wolverine;

public class HerberekenGeotagsMessageHandler
{
    public async Task Handle(
        HerberekenGeotagsMessage message,
        IMessageBus messageBus,
        CancellationToken cancellationToken
    )
    {
        var envelope = new CommandEnvelope<HerberekenGeotagsCommand>(
            message.ToCommand(),
            CommandMetadata.ForDigitaalVlaanderenProcess
        );

        await messageBus.InvokeAsync(envelope, cancellationToken);
    }
}
