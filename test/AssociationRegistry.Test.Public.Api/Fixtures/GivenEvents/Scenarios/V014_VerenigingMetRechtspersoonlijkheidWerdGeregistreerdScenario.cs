namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime;
using Vereniging;

public class V014_VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario : IScenario
{
    private readonly string Naam = "Feesten Affligem";
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly string KboNummer= "0987654321";

    public VCode VCode
        => VCode.Create("V0001014");

    public V014_VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
            VCode,
            KboNummer,
            "VZW",
            Naam,
            string.Empty,
            Startdatum: null);
    }

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}
