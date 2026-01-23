namespace AssociationRegistry.CommandHandling.Grar.GrarConsumer.Messaging.OntkoppelAdres;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class OntkoppelLocatiesMessageHandler
{
    private readonly IAggregateSession _aggregateSession;

    public OntkoppelLocatiesMessageHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task Handle(OntkoppelLocatiesMessage message, CancellationToken cancellationToken)
    {
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Hydrate(message.VCode),
            metadata,
            allowDubbeleVereniging: true
        );

        foreach (var teOntkoppelenLocatieId in message.TeOntkoppelenLocatieIds)
        {
            vereniging.OntkoppelLocatie(teOntkoppelenLocatieId);
        }

        await _aggregateSession.Save(
            vereniging,
            metadata with
            {
                ExpectedVersion = vereniging.Version,
            },
            cancellationToken
        );
    }
}
