namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Framework;
using Vereniging;
using AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario : CommandhandlerScenarioBase
{
    public const string Naam = "Hulste Huldigt";
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly  Registratiedata.Locatie Locatie;
    public readonly DateOnly? Startdatum = null;
    public readonly VCode VCode = VCode.Create("V0009002");

    public FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        Locatie = fixture.Create<Registratiedata.Locatie>();
    }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam,
                KorteBeschrijving,
                Startdatum,
                Array.Empty<Registratiedata.Contactgegeven>(),
                new[] { Locatie },
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
        };
    }
}
