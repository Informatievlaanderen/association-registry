namespace AssociationRegistry.Admin.ProjectionHost.Projections.Bewaartermijn.EventHandling;

using DecentraalBeheer.Vereniging.Bewaartermijnen;
using DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Events;
using Framework;
using Integrations.Grar.Bewaartermijnen;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Wolverine;

public static class BewaartermijnVertegenwoordigersEventHandler
{
    public static ShardName ShardName = new("beheer.eventsubscription.bewaartermijn.vertegenwoordigers");

    public static async Task Handle(
        IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> @event,
        BewaartermijnOptions bewaartermijnOptions,
        IMessageBus messageBus)
    {
        var vervaldag = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await messageBus.SendAsync(new StartBewaartermijnMessage(@event.StreamKey, @event.Data.VertegenwoordigerId, vervaldag, BewaartermijnReden.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden));
    }

    // public static async Task Handle(
    //     IEvent<VertegenwoordigerWerdVerwijderd> @event,
    //     IMessageBus messageBus)
    // {
    //     await messageBus.SendAsync(new StartBewaartermijnVoorVerwijderVerenigingMessage());
    //
    //
    // }

    // public static async Task Handle(
    //     IEvent<VerenigingWerdVerwijderd> @event,
    //     IMessageBus messageBus,
    //     CancellationToken cancellationToken
    // )
    // {
    //     await messageBus.SendAsync(new StartBewaartermijnVoorVerwijderVertegenwoordigerMessage());
    //
    //     var state = await eventStore.Load<VerenigingState>(@event.StreamKey!, expectedVersion: null);
    //
    //     foreach (var vertegenwoordiger in state.Vertegenwoordigers)
    //     {
    //         await CreateBewaartermijn(
    //             @event.StreamKey!,
    //             vertegenwoordiger.VertegenwoordigerId,
    //             @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip),
    //             eventStore,
    //             bewaartermijnOptions,
    //             cancellationToken,
    //             BewaartermijnReden.VerenigingWerdVerwijderd
    //         );
    //     }
    // }

    // public static async Task Handle(
    //     IEvent<VerenigingWerdGestopt> @event,
    //     IMessageBus messageBus,
    //     BewaartermijnOptions bewaartermijnOptions,
    //     CancellationToken cancellationToken
    // )
    // {
    //     await messageBus.SendAsync(new StartBewaartermijnVoorStopVerenigingMessage());
    //
    //     var state = await eventStore.Load<VerenigingState>(@event.StreamKey!, expectedVersion: null);
    //
    //     foreach (var vertegenwoordiger in state.Vertegenwoordigers)
    //     {
    //         await CreateBewaartermijn(
    //             @event.StreamKey!,
    //             vertegenwoordiger.VertegenwoordigerId,
    //             @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip),
    //             eventStore,
    //             bewaartermijnOptions,
    //             cancellationToken,
    //             BewaartermijnReden.VerenigingWerdGestopt
    //         );
    //     }
    // }

    // private static async Task CreateBewaartermijn(
    //     string streamKey,
    //     int entityId,
    //     Instant tijdstip,
    //     IEventStore eventStore,
    //     BewaartermijnOptions bewaartermijnOptions,
    //     CancellationToken cancellationToken,
    //     string reden
    // )
    // {
    //     var bewaartermijnId = new BewaartermijnId(
    //         VCode.Create(streamKey),
    //         PersoonsgegevensType.Vertegenwoordigers,
    //         entityId
    //     );
    //
    //     var vervaldag = tijdstip.PlusTicks(bewaartermijnOptions.Duration.Ticks);
    //
    //     await eventStore.SaveNew(
    //         bewaartermijnId,
    //         CommandMetadata.ForDigitaalVlaanderenProcess,
    //         cancellationToken,
    //         [
    //             new BewaartermijnWerdGestartV2(
    //                 bewaartermijnId,
    //                 bewaartermijnId.VCode!,
    //                 bewaartermijnId.PersoonsgegevensType.Value,
    //                 bewaartermijnId.EntityId,
    //                 vervaldag,
    //                 reden
    //             ),
    //         ]
    //     );
    // }
}
