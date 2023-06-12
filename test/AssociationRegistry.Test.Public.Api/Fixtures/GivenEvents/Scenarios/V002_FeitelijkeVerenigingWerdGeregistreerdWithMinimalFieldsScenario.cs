namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime;

public class V002_FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario : IScenario
{
    private readonly string Naam = "Feesten Hulste";

    public VCode VCode
        => VCode.Create("V0001002");

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                Naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                false,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public class V010_FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario : IScenario
{
    public VCode VCode
        => VCode.Create("V0001002");


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                "verenigingZonderNaam",
                string.Empty,
                string.Empty,
                Startdatum: null,
                false,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
            new VerenigingWerdUitgeschrevenUitPubliekeDatastroom(),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}
