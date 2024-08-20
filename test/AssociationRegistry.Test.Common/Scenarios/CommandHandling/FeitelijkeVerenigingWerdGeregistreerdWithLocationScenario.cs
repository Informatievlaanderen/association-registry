namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using global::AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario : CommandhandlerScenarioBase
{
    public const string Naam = "Hulste Huldigt";
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly Registratiedata.Locatie Locatie;
    public readonly DateOnly? Startdatum = null;
    public override VCode VCode => VCode.Create("V0009002");

    public FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
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
                Registratiedata.Doelgroep.With(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: false,
                Array.Empty<Registratiedata.Contactgegeven>(),
                new[] { Locatie },
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
        };
    }
}
