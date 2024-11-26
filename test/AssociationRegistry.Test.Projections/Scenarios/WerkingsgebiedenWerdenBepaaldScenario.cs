namespace AssociationRegistry.Test.Projections.Scenarios;

using AutoFixture;
using Common.AutoFixture;
using Events;

public class WerkingsgebiedenWerdenBepaaldScenario : IScenario
{
    public WerkingsgebiedenWerdenBepaald WerkingsgebiedenWerdenBepaald { get; }

    public WerkingsgebiedenWerdenBepaaldScenario()
    {
        var fixture = new Fixture().CustomizeDomain();
        VerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        WerkingsgebiedenWerdenBepaald = new WerkingsgebiedenWerdenBepaald(VerenigingWerdGeregistreerd.VCode, new[]
        {
            new Registratiedata.Werkingsgebied(Code: "BE25535002", Naam: "Bredene"),
            new Registratiedata.Werkingsgebied(Code: "BE25535005", Naam: "Gistel"),
        });
    }

    public string VCode => VerenigingWerdGeregistreerd.VCode;

    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; private set; }

    public IEnumerable<EventsPerVCode> GivenEvents =>
    [
        new EventsPerVCode(VerenigingWerdGeregistreerd.VCode,
                           VerenigingWerdGeregistreerd, WerkingsgebiedenWerdenBepaald),
    ];
}
