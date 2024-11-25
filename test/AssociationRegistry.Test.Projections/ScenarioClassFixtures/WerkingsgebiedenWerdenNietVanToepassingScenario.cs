namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using Events;
using Framework;
using Framework.Fixtures;

public class WerkingsgebiedenWerdenNietVanToepassingScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public WerkingsgebiedenWerdenNietVanToepassing WerkingsgebiedenWerdenNietVanToepassing { get; }

    public WerkingsgebiedenWerdenNietVanToepassingScenario(ProjectionContext context) : base(context)
    {
        var scenario = new WerkingsgebiedenWerdenBepaaldScenario(context);
        scenario.Given().GetAwaiter().GetResult();

        WerkingsgebiedenWerdenNietVanToepassing = new WerkingsgebiedenWerdenNietVanToepassing(scenario.WerkingsgebiedenWerdenBepaald.VCode);
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(WerkingsgebiedenWerdenNietVanToepassing.VCode,
                              WerkingsgebiedenWerdenNietVanToepassing);

        await session.SaveChangesAsync();
    }
}
