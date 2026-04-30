namespace AssociationRegistry.Admin.ProjectionHost.Projections.Bewaartermijn.EventHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Grar.Bewaartermijnen;
using DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using JasperFx.Events;
using JasperFx.Events.Projections;
using NodaTime;
using Wolverine;
using IEventStore = MartenDb.Store.IEventStore;

public static class BewaartermijnVertegenwoordigersEventHandler
{
    public static ShardName ShardName = new("beheer.eventsubscription.bewaartermijn.vertegenwoordigers");

    public static async Task Handle(
        IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> @event,
        IMessageBus messageBus)
    {
       await  messageBus.SendAsync(new StartBewaartermijnVoorOverledenVertegenwoordigerMessage());
    }

    public static async Task Handle(
        IEvent<VertegenwoordigerWerdVerwijderd> @event,
        IMessageBus messageBus)
    {
        await  messageBus.SendAsync(new StartBewaartermijnVoorVerwijderVerenigingMessage());

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
        IMessageBus messageBus,
        CancellationToken cancellationToken
    )
    {
        await messageBus.SendAsync(new StartBewaartermijnVoorVerwijderVertegenwoordigerMessage());

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
        IMessageBus messageBus,
        BewaartermijnOptions bewaartermijnOptions,
        CancellationToken cancellationToken
    )
    {
        await messageBus.SendAsync(new StartBewaartermijnVoorStopVerenigingMessage());

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
