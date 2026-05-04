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
        IMessageBus messageBus
    )
    {
        var vervaldag = @event
            .GetHeaderInstant(MetadataHeaderNames.Tijdstip)
            .PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await messageBus.SendAsync(
            new StartBewaartermijnMessage(
                @event.StreamKey!,
                PersoonsgegevensType.Vertegenwoordigers.Value,
                @event.Data.VertegenwoordigerId,
                vervaldag,
                BewaartermijnReden.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden
            )
        );
    }

    public static async Task Handle(
        IEvent<VertegenwoordigerWerdVerwijderd> @event,
        BewaartermijnOptions bewaartermijnOptions,
        IMessageBus messageBus
    )
    {
        var vervaldag = @event
            .GetHeaderInstant(MetadataHeaderNames.Tijdstip)
            .PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await messageBus.SendAsync(
            new StartBewaartermijnMessage(
                @event.StreamKey!,
                PersoonsgegevensType.Vertegenwoordigers.Value,
                @event.Data.VertegenwoordigerId,
                vervaldag,
                BewaartermijnReden.VertegenwoordigerWerdVerwijderd
            )
        );
    }

    public static async Task Handle(
        IEvent<VerenigingWerdVerwijderd> @event,
        BewaartermijnOptions bewaartermijnOptions,
        IMessageBus messageBus
    )
    {
        var vervaldag = @event
            .GetHeaderInstant(MetadataHeaderNames.Tijdstip)
            .PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await messageBus.SendAsync(
            new StartBewaartermijnenVoorVerenigingMessage(
                @event.StreamKey!,
                PersoonsgegevensType.Vertegenwoordigers.Value,
                vervaldag,
                BewaartermijnReden.VerenigingWerdVerwijderd
            )
        );
    }

    public static async Task Handle(
        IEvent<VerenigingWerdGestopt> @event,
        BewaartermijnOptions bewaartermijnOptions,
        IMessageBus messageBus
    )
    {
        var vervaldag = @event
            .GetHeaderInstant(MetadataHeaderNames.Tijdstip)
            .PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await messageBus.SendAsync(
            new StartBewaartermijnenVoorVerenigingMessage(
                @event.StreamKey!,
                PersoonsgegevensType.Vertegenwoordigers.Value,
                vervaldag,
                BewaartermijnReden.VerenigingWerdGestopt
            )
        );
    }
}
