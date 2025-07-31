namespace AssociationRegistry.MartenDb.Setup;

using Marten;
using Upcasters;
using Events;
using JasperFx.Events.Projections;
using Subscriptions;

public static class SetupExtensions
{
    public static void UpcastLegacyTombstoneEvents(this StoreOptions source)
    {
        source.Events.Upcast(
            new TombstoneUpcaster()
        );
    }

    public static void AddAllEventTypes(this StoreOptions opts)
    {
        var eventInterface = typeof(Events.IEvent);

        var eventTypes = eventInterface.Assembly
                                       .GetTypes()
                                       .Where(t =>
                                                  eventInterface.IsAssignableFrom(t) &&
                                                  t.IsClass &&
                                                  !t.IsAbstract &&
                                                  t != typeof(AfdelingWerdGeregistreerd) // skip obsolete one if needed
                                        );

        foreach (var eventType in eventTypes)
        {
            opts.Events.AddEventType(eventType);
        }
    }
}
