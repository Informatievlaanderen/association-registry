namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using Events;
using Framework;
using Framework.Fixtures;

public class LidmaatschapWerdVerwijderdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public LidmaatschapWerdVerwijderd LidmaatschapWerdVerwijderd { get; }

    public LidmaatschapWerdVerwijderdScenario(ProjectionContext context) : base(context)
    {
        var scenario = new LidmaatschapWerdToegevoegdScenario(context);
        scenario.Given().GetAwaiter().GetResult();

        LidmaatschapWerdVerwijderd = new LidmaatschapWerdVerwijderd(
            Lidmaatschap: scenario.LidmaatschapWerdToegevoegd.Lidmaatschap,
            VCode: scenario.VerenigingWerdGeregistreerd.VCode);
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(LidmaatschapWerdVerwijderd.VCode,
                              LidmaatschapWerdVerwijderd);

        await session.SaveChangesAsync();
    }
}
