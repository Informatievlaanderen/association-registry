namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime;

public class V009_MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario : IScenario
{
    private readonly string Naam = "Feesten Affligem";
    public readonly string MoederKboNummer;
    public readonly string MoederVCode;
    public readonly string MoederNaam;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd MoederWerdGeregistreerd;
    public readonly AfdelingWerdGeregistreerd AfdelingWerdGeregistreerd;

    public VCode VCode
        => VCode.Create("V0001009");

    public V009_MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario()
    {
        MoederKboNummer = "0987654321";
        MoederVCode = "V0001008";
        MoederNaam = "Moeder 0987654321";

        MoederWerdGeregistreerd = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
            MoederVCode,
            MoederKboNummer,
            "VZW",
            MoederNaam,
            string.Empty,
            Startdatum: null);

        AfdelingWerdGeregistreerd = new AfdelingWerdGeregistreerd(
            VCode,
            Naam,
            new AfdelingWerdGeregistreerd.MoederverenigingsData(MoederKboNummer, MoederVCode, MoederNaam),
            string.Empty,
            string.Empty,
            Startdatum: null,
            Registratiedata.Doelgroep.With(Doelgroep.Null),
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());
    }

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            MoederWerdGeregistreerd,
            AfdelingWerdGeregistreerd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant(), Guid.NewGuid());
}
