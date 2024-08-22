namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using Events;
using EventStore;
using NodaTime;
using Vereniging;

public class V021_FeitelijkeVerenigingWerdGeregistreerdAsUitgeschrevenUitPubliekeDatastroomScenario : IEventsInDbScenario
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public V021_FeitelijkeVerenigingWerdGeregistreerdAsUitgeschrevenUitPubliekeDatastroomScenario()
    {
        VCode = "V9999021";

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            Naam: "verenigingZonderNaam",
            string.Empty,
            string.Empty,
            Startdatum: null,
            Registratiedata.Doelgroep.With(Doelgroep.Null),
            IsUitgeschrevenUitPubliekeDatastroom: true,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());
    }

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
        };

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
