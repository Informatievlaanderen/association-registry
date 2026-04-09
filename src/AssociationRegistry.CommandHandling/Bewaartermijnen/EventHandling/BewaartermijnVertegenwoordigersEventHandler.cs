namespace AssociationRegistry.CommandHandling.Bewaartermijnen.EventHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using Events;
using Framework;
using Integrations.Grar.Bewaartermijnen;
using JasperFx.Events;
using JasperFx.Events.Projections;
using NodaTime;
using IEventStore = MartenDb.Store.IEventStore;

public static class BewaartermijnVertegenwoordigersEventHandler
{
    public static ShardName ShardName = new("beheer.eventsubscription.bewaartermijn.vertegenwoordigers");

    public static async Task Handle(
        IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> @event,
        IEventStore eventStore,
        BewaartermijnOptions bewaartermijnOptions,
        CancellationToken cancellationToken
    )
    {
        await CreateBewaartermijn(
            @event.StreamKey!,
            @event.Data.VertegenwoordigerId,
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip),
            eventStore,
            bewaartermijnOptions,
            cancellationToken,
            BewaartermijnReden.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden
        );
    }

    public static async Task Handle(
        IEvent<VertegenwoordigerWerdVerwijderd> @event,
        IEventStore eventStore,
        BewaartermijnOptions bewaartermijnOptions,
        CancellationToken cancellationToken
    )
    {
        await CreateBewaartermijn(
            @event.StreamKey!,
            @event.Data.VertegenwoordigerId,
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip),
            eventStore,
            bewaartermijnOptions,
            cancellationToken,
            BewaartermijnReden.VertegenwoordigerWerdVerwijderd
        );
    }

    public static async Task Handle(
        IEvent<VerenigingWerdVerwijderd> @event,
        IEventStore eventStore,
        BewaartermijnOptions bewaartermijnOptions,
        CancellationToken cancellationToken
    )
    {
        var state = await eventStore.Load<VerenigingState>(@event.StreamKey!, expectedVersion: null);

        foreach (var vertegenwoordiger in state.Vertegenwoordigers)
        {
            await CreateBewaartermijn(
                @event.StreamKey!,
                vertegenwoordiger.VertegenwoordigerId,
                @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip),
                eventStore,
                bewaartermijnOptions,
                cancellationToken,
                BewaartermijnReden.VerenigingWerdVerwijderd
            );
        }
    }

    public static async Task Handle(
        IEvent<VerenigingWerdGestopt> @event,
        IEventStore eventStore,
        BewaartermijnOptions bewaartermijnOptions,
        CancellationToken cancellationToken
    )
    {
        var state = await eventStore.Load<VerenigingState>(@event.StreamKey!, expectedVersion: null);

        foreach (var vertegenwoordiger in state.Vertegenwoordigers)
        {
            await CreateBewaartermijn(
                @event.StreamKey!,
                vertegenwoordiger.VertegenwoordigerId,
                @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip),
                eventStore,
                bewaartermijnOptions,
                cancellationToken,
                BewaartermijnReden.VerenigingWerdGestopt
            );
        }
    }

    private static async Task CreateBewaartermijn(
        string streamKey,
        int entityId,
        Instant tijdstip,
        IEventStore eventStore,
        BewaartermijnOptions bewaartermijnOptions,
        CancellationToken cancellationToken,
        string reden
    )
    {
        var bewaartermijnId = new BewaartermijnId(
            VCode.Create(streamKey),
            PersoonsgegevensType.Vertegenwoordigers,
            entityId
        );

        var vervaldag = tijdstip.PlusTicks(bewaartermijnOptions.Duration.Ticks);

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
                    vervaldag,
                    reden
                ),
            ]
        );
    }
}
