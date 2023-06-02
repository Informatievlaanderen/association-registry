namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Vereniging;

public class FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario : CommandhandlerScenarioBase
{
    public const int ContactgegevenId = 1;
    public const string Waarde = "test@example.org";
    public const string Beschrijving = "";
    public const bool IsPrimair = true;
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly string KorteNaam = "FOud";

    public readonly string Naam = "Hulste Huldigt";
    public readonly DateOnly Startdatum = new(year: 2023, month: 3, day: 6);
    public readonly ContactgegevenType Type = ContactgegevenType.Email;
    public readonly VCode VCode = VCode.Create("V0009002");

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
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
            new ContactgegevenWerdToegevoegd(ContactgegevenId, Type, Waarde, Beschrijving, IsPrimair),
        };
    }
}
