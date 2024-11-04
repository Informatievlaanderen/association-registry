namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.ScenarioClassFixtures;

using AutoFixture;
using Common.AutoFixture;
using Events;

public class LidmaatschapWerdGewijzigdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd { get; }
    public LidmaatschapWerdToegevoegdScenario LidmaatschapWerdToegevoegdScenario { get; }

    public LidmaatschapWerdGewijzigdScenario(ProjectionContext context): base(context)
    {
        var fixture = new Fixture().CustomizeDomain();

        var LidmaatschapWerdToegevoegdScenario = new LidmaatschapWerdToegevoegdScenario(context);
        LidmaatschapWerdToegevoegdScenario.Given().GetAwaiter().GetResult();

        LidmaatschapWerdGewijzigd = new LidmaatschapWerdGewijzigd(
            Lidmaatschap: fixture.Create<Registratiedata.Lidmaatschap>() with
            {
                LidmaatschapId = LidmaatschapWerdToegevoegdScenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
            },
            VCode: LidmaatschapWerdToegevoegdScenario.VerenigingWerdGeregistreerd.VCode);
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(LidmaatschapWerdGewijzigd.VCode,
                              LidmaatschapWerdGewijzigd);
        await session.SaveChangesAsync();

        await Context.WaitForNonStaleProjectionDataAsync();
    }

}
