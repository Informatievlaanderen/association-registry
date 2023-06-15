namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using Events;
using EventStore;
using NodaTime;

public class V021_FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario : IEventsInDbScenario
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; }= null!;

    public V021_FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario()
    {
        VCode = "V9999021";
        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            "verenigingZonderNaam",
            string.Empty,
            string.Empty,
            Startdatum: null,
            false,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());
    }


    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            new VerenigingWerdUitgeschrevenUitPubliekeDatastroom(),
        };

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}
