namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Factories;
using NodaTime;
using Vereniging;

public class V024_FeitelijkeVerenigingWerdGeregistreerdWithAllFacetsScenario : IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        VCode: "V0001024",
        Naam: "Feestzaal de vrolijke facets",
        string.Empty,
        string.Empty,
        Startdatum: null,
        EventFactory.Doelgroep(Doelgroep.Null),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        Array.Empty<Registratiedata.Contactgegeven>(),
        Array.Empty<Registratiedata.Locatie>(),
        Array.Empty<Registratiedata.Vertegenwoordiger>(),
        HoofdactiviteitVerenigingsloket.All().Select(s => new Registratiedata.HoofdactiviteitVerenigingsloket(s.Code, s.Naam)).ToArray());

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
