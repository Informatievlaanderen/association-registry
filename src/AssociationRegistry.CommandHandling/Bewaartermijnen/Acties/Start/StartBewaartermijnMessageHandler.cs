namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Events;
using Framework;

public class StartBewaartermijnMessageHandler
{
    public async Task Handle(
        StartBewaartermijnMessage message,
        IEventStore eventStore,
        CancellationToken cancellationToken
    )
    {
        var bewaartermijnId = new BewaartermijnId(
            VCode.Create(message.StreamKey),
            PersoonsgegevensType.Create(message.PersoonsgegevensType),
            message.EntityId
        );

        await eventStore.SaveNew(
            bewaartermijnId,
            CommandMetadata.ForDigitaalVlaanderenProcess,
            cancellationToken,
            [
                new BewaartermijnWerdGestartV2(
                    bewaartermijnId,
                    bewaartermijnId.VCode!,
                    bewaartermijnId.PersoonsgegevensType.Value,
                    bewaartermijnId.EntityId,
                    message.Vervaldag,
                    message.Reden
                ),
            ]
        );
    }
}
