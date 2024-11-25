namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using Events;
using Framework;
using Framework.Fixtures;

public class WerkingsgebiedenWerdenNietBepaaldScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public WerkingsgebiedenWerdenNietBepaald WerkingsgebiedenWerdenNietBepaald { get; }

    public WerkingsgebiedenWerdenNietBepaaldScenario(ProjectionContext context) : base(context)
    {
        var scenario = new WerkingsgebiedenWerdenBepaaldScenario(context);
        scenario.Given().GetAwaiter().GetResult();

        WerkingsgebiedenWerdenNietBepaald = new WerkingsgebiedenWerdenNietBepaald(scenario.WerkingsgebiedenWerdenBepaald.VCode);
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(WerkingsgebiedenWerdenNietBepaald.VCode,
                              WerkingsgebiedenWerdenNietBepaald);

        await session.SaveChangesAsync();
    }
}
