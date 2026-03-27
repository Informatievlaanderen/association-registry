namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using Events;
using Framework;
using Integrations.Grar.Bewaartermijnen;
using MartenDb.Store;

public class StartBewaartermijnMessageHandler
{
    public StartBewaartermijnMessageHandler() { }

    public async Task Handle(
        CommandEnvelope<StartBewaartermijnMessage> message,
        IEventStore eventStore,
        BewaartermijnOptions bewaartermijnOptions,
        CancellationToken cancellationToken
    )
    {
        var bewaartermijnId = new BewaartermijnId(
            VCode.Create(message.Command.VCode),
            PersoonsgegevensType.Create(message.Command.PersoonsgegevensType),
            message.Command.EntityId
        );
        var vervaldag = message.Metadata.Tijdstip.PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await eventStore.SaveNew(
            bewaartermijnId,
            message.Metadata,
            cancellationToken,
            [
                new BewaartermijnWerdGestartV2(
                    bewaartermijnId,
                    bewaartermijnId.VCode!,
                    bewaartermijnId.PersoonsgegevensType.Value,
                    bewaartermijnId.EntityId,
                    vervaldag,
                    message.Command.Reden
                ),
            ]
        );
    }
}
