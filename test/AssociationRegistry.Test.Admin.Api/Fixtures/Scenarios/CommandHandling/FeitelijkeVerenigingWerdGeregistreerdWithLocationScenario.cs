namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario : CommandhandlerScenarioBase
{
    public const string Naam = "Hulste Huldigt";
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly FeitelijkeVerenigingWerdGeregistreerd.Locatie Locatie;
    public readonly DateOnly? Startdatum = null;
    public readonly VCode VCode = VCode.Create("V0009002");

    public FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        Locatie = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd.Locatie>();
    }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                VerenigingsType.FeitelijkeVereniging.Code,
                Naam,
                KorteNaam,
                KorteBeschrijving,
                Startdatum,
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>(),
                new[] { Locatie },
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
        };
    }
}