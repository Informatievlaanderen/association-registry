namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
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
        Contactgegevens = new[] { new Fixture().CustomizeAll().Create<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>() with { ContactgegevenId = 1 } };
    }

    public FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven[] Contactgegevens { get; }
    public FeitelijkeVerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;
    public ContactgegevenWerdVerwijderd ContactgegevenWerdVerwijderd { get; private set; } = null!;

    public override IEnumerable<IEvent> Events()
    {
        WerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            VerenigingsType.FeitelijkeVereniging.Code,
            Naam,
            KorteNaam,
            KorteBeschrijving,
            Startdatum,
            Contactgegevens,
            Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>());
        ContactgegevenWerdVerwijderd = new ContactgegevenWerdVerwijderd(Contactgegevens[0].ContactgegevenId, Contactgegevens[0].Type, Contactgegevens[0].Waarde, Contactgegevens[0].Beschrijving, Contactgegevens[0].IsPrimair);
        return new IEvent[]
        {
            WerdGeregistreerd,
            ContactgegevenWerdVerwijderd,
        };
    }
}