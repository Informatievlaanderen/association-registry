namespace AssociationRegistry.CommandHandling.Bewaartermijnen.EventHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using Events;
using Framework;
using Integrations.Grar.Bewaartermijnen;
using JasperFx.Events;
using IEventStore = MartenDb.Store.IEventStore;

public static class BewaartermijnEventHandler
{
    public static async Task Handle(
        IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> @event,
        IEventStore eventStore,
        BewaartermijnOptions bewaartermijnOptions,
        CancellationToken cancellationToken
    )
    {
        var bewaartermijnId = new BewaartermijnId(
            VCode.Create(@event.StreamKey!),
            BewaartermijnType.Vertegenwoordigers,
            @event.Data.VertegenwoordigerId
        );

        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip);

        var vervaldag = tijdstip.PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await eventStore.SaveNew(
            bewaartermijnId,
            CommandMetadata.ForDigitaalVlaanderenProcess,
            cancellationToken,
            [
                new BewaartermijnWerdGestartV2(
                    bewaartermijnId,
                    bewaartermijnId.VCode!,
                    bewaartermijnId.BewaartermijnType.Value,
                    bewaartermijnId.RecordId,
                    vervaldag,
                    BewaartermijnReden.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden
                ),
            ]
        );
    }
}
