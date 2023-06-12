namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Framework;
using Vereniging;
using AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithRemovedContactgegevenScenario : CommandhandlerScenarioBase
{
    public const string Naam = "Hulste Huldigt";
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly DateOnly Startdatum = new(year: 2023, month: 3, day: 6);
    public readonly VCode VCode = VCode.Create("V0009002");

    public FeitelijkeVerenigingWerdGeregistreerdWithRemovedContactgegevenScenario()
    {
        Contactgegevens = new[] { new Fixture().CustomizeAll().Create<Registratiedata.Contactgegeven>() with { ContactgegevenId = 1 } };
    }

    public Registratiedata.Contactgegeven[] Contactgegevens { get; }
    public FeitelijkeVerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;
    public ContactgegevenWerdVerwijderd ContactgegevenWerdVerwijderd { get; private set; } = null!;

    public override IEnumerable<IEvent> Events()
    {
        WerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            Naam,
            KorteNaam,
            KorteBeschrijving,
            Startdatum,
            false,
            Contactgegevens,
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());
        ContactgegevenWerdVerwijderd = new ContactgegevenWerdVerwijderd(Contactgegevens[0].ContactgegevenId, Contactgegevens[0].Type, Contactgegevens[0].Waarde, Contactgegevens[0].Beschrijving, Contactgegevens[0].IsPrimair);
        return new IEvent[]
        {
            WerdGeregistreerd,
            ContactgegevenWerdVerwijderd,
        };
    }
}
