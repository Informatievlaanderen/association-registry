namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.ScenarioClassFixtures;

using AutoFixture;
using Common.AutoFixture;
using Events;

public class NaamAndereVerenigingWerdGewijzigdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public NaamWerdGewijzigd NaamWerdGewijzigd { get; }

    public LidmaatschapWerdToegevoegdScenario LidmaatschapWerdToegevoegdScenario { get; }

    public NaamAndereVerenigingWerdGewijzigdScenario(ProjectionContext context): base(context)
    {
        var fixture = new Fixture().CustomizeDomain();

        LidmaatschapWerdToegevoegdScenario = new LidmaatschapWerdToegevoegdScenario(context);
        LidmaatschapWerdToegevoegdScenario.Given().GetAwaiter().GetResult();

        NaamWerdGewijzigd = new NaamWerdGewijzigd(
            Naam: fixture.Create<string>(),
            VCode: LidmaatschapWerdToegevoegdScenario.AndereVerenigingWerdGeregistreerd.VCode);
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(NaamWerdGewijzigd.VCode,
                              NaamWerdGewijzigd);
        await session.SaveChangesAsync();

        await Context.WaitForNonStaleProjectionDataAsync();
    }

}
