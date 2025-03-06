namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using EventFactories;
using Events;
using NodaTime;
using System;
using Vereniging;

public class V023_WerdUitgeschrevenUitPubliekeDatastroomScenario_And_NaamWerdGewijzigd : IScenario
{
    public VCode VCode
        => VCode.Create("V0001023");

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                Naam: "verenigingZonderNaam",
                string.Empty,
                string.Empty,
                Startdatum: null,
                EventFactory.Doelgroep(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: false,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
            new VerenigingWerdUitgeschrevenUitPubliekeDatastroom(),
            new NaamWerdGewijzigd(VCode, Naam: "Gewijzigd"),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
