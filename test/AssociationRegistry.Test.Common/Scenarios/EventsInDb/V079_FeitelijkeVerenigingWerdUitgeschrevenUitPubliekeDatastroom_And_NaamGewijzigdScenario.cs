namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using NodaTime;

public class V079_FeitelijkeVerenigingWerdUitgeschrevenUitPubliekeDatastroom_And_NaamGewijzigdScenario : IEventsInDbScenario
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public NaamWerdGewijzigd NaamWerdGewijzigd { get; set; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public V079_FeitelijkeVerenigingWerdUitgeschrevenUitPubliekeDatastroom_And_NaamGewijzigdScenario()
    {
        VCode = "V9999079";

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

        NaamWerdGewijzigd = new NaamWerdGewijzigd(VCode, "Gewijzigd");
    }

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            NaamWerdGewijzigd
        };

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
