namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Vereniging;

public class FeitelijkeVerenigingWerdGeregistreerdWithoutVertegenwoordigers : CommandhandlerScenarioBase
{
    public const string Naam = "Hulste Huldigt";
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly DateOnly Startdatum = new(year: 2023, month: 3, day: 6);
    public readonly VCode VCode = VCode.Create("V0009002");
    public FeitelijkeVerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;

    public override IEnumerable<IEvent> Events()
    {
        WerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            Naam,
            KorteNaam,
            KorteBeschrijving,
            Startdatum,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());
        return new IEvent[]
        {
            WerdGeregistreerd,
        };
    }
}
