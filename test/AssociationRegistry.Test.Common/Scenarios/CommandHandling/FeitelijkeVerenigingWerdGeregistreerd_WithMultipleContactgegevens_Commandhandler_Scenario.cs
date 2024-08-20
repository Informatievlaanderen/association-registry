namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

public class FeitelijkeVerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario : CommandhandlerScenarioBase
{
    public readonly string Initiator = "Een initiator";
    public override VCode VCode => VCode.Create("V0009002");

    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd1 { get; } =
        new(ContactgegevenId: 1, Contactgegeventype.Email, Waarde: "test1@example.org", Beschrijving: "", IsPrimair: true);

    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd2 { get; } =
        new(ContactgegevenId: 2, Contactgegeventype.Email, Waarde: "test2@example.org", Beschrijving: "", IsPrimair: false);

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                Naam: "Hulste Huldigt",
                KorteNaam: "FOud",
                string.Empty,
                new DateOnly(year: 2023, month: 3, day: 6),
                Registratiedata.Doelgroep.With(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: false,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
            ContactgegevenWerdToegevoegd1,
            ContactgegevenWerdToegevoegd2,
        };
    }
}
