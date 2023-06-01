namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Vereniging;

public class FeitelijkeVerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario : CommandhandlerScenarioBase
{
    public readonly string Initiator = "Een initiator";
    public readonly VCode VCode = VCode.Create("V0009002");

    public FeitelijkeVerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario()
    {
        ContactgegevenWerdToegevoegd1 = new ContactgegevenWerdToegevoegd(ContactgegevenId: 1, ContactgegevenType.Email, "test1@example.org", "", IsPrimair: true);
        ContactgegevenWerdToegevoegd2 = new ContactgegevenWerdToegevoegd(ContactgegevenId: 2, ContactgegevenType.Email, "test2@example.org", "", IsPrimair: false);
    }

    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd1 { get; }
    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd2 { get; }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                "Hulste Huldigt",
                "FOud",
                string.Empty,
                new DateOnly(year: 2023, month: 3, day: 6),
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
            ContactgegevenWerdToegevoegd1,
            ContactgegevenWerdToegevoegd2,
        };
    }
}
