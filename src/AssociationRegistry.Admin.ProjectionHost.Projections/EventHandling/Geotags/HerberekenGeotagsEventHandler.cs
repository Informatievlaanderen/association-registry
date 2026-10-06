namespace AssociationRegistry.Admin.ProjectionHost.Projections.EventHandling.Geotags;

using DecentraalBeheer.Vereniging.Geotags.Messages;
using Events;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Wolverine;

public static class HerberekenGeotagsEventHandler
{
    public static ShardName ShardName = new("beheer.eventsubscription.herberekengeotags");

    public static async Task Handle(IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> @event, IMessageBus messageBus)
    {
        await messageBus.SendAsync(new HerberekenGeotagsMessage(@event.StreamKey!));
    }

    public static async Task Handle(IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo> @event, IMessageBus messageBus)
    {
        await messageBus.SendAsync(new HerberekenGeotagsMessage(@event.StreamKey!));
    }

    public static async Task Handle(IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> @event, IMessageBus messageBus)
    {
        await messageBus.SendAsync(new HerberekenGeotagsMessage(@event.StreamKey!));
    }
}
