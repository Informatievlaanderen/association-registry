namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework;
using Framework.Fixtures;

public class WerkingsgebiedenWerdenBepaaldScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public WerkingsgebiedenWerdenBepaald WerkingsgebiedenWerdenBepaald { get; }

    public WerkingsgebiedenWerdenBepaaldScenario(ProjectionContext context) : base(context)
    {
        var fixture = new Fixture().CustomizeDomain();
        VerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        WerkingsgebiedenWerdenBepaald = new WerkingsgebiedenWerdenBepaald(VerenigingWerdGeregistreerd.VCode, new[]
        {
            new Registratiedata.Werkingsgebied(Code: "BE25535002", Naam: "Bredene"),
            new Registratiedata.Werkingsgebied(Code: "BE25535005", Naam: "Gistel"),
        });
    }

    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; private set; }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(VerenigingWerdGeregistreerd.VCode,
                              VerenigingWerdGeregistreerd);

        session.Events.Append(WerkingsgebiedenWerdenBepaald.VCode,
                              WerkingsgebiedenWerdenBepaald);

        await session.SaveChangesAsync();
    }
}
