namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using Events;
using Framework;
using Framework.Fixtures;

public class WerkingsgebiedenWerdenGewijzigdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public WerkingsgebiedenWerdenGewijzigd WerkingsgebiedenWerdenGewijzigd { get; }

    public WerkingsgebiedenWerdenGewijzigdScenario(ProjectionContext context) : base(context)
    {
        var scenario = new WerkingsgebiedenWerdenBepaaldScenario(context);
        scenario.Given().GetAwaiter().GetResult();

        WerkingsgebiedenWerdenGewijzigd = new WerkingsgebiedenWerdenGewijzigd(scenario.WerkingsgebiedenWerdenBepaald.VCode, new[]
        {
            new Registratiedata.Werkingsgebied(Code: "BE25535011", Naam: "Middelkerke"),
            new Registratiedata.Werkingsgebied(Code: "BE25535013", Naam: "Oostende"),
            new Registratiedata.Werkingsgebied(Code: "BE25535014", Naam: "Oudenburg"),
        });
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(WerkingsgebiedenWerdenGewijzigd.VCode, WerkingsgebiedenWerdenGewijzigd);
        await session.SaveChangesAsync();
    }
}
