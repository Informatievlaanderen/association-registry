namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.ScenarioClassFixtures;

using AutoFixture;
using Common.AutoFixture;
using Events;

public class LidmaatschapWerdToegevoegdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd AndereVerenigingWerdGeregistreerd { get; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; set; }

    public LidmaatschapWerdToegevoegdScenario(ProjectionContext context): base(context)
    {
        var fixture = new Fixture().CustomizeDomain();
        VerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AndereVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        LidmaatschapWerdToegevoegd = fixture.Create<LidmaatschapWerdToegevoegd>() with
        {
            Lidmaatschap = fixture.Create<Registratiedata.Lidmaatschap>() with
            {
                AndereVereniging = AndereVerenigingWerdGeregistreerd.VCode,
            },
        };
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(AndereVerenigingWerdGeregistreerd.VCode,
                              AndereVerenigingWerdGeregistreerd);

        session.Events.Append(VerenigingWerdGeregistreerd.VCode,
                              VerenigingWerdGeregistreerd,
                              LidmaatschapWerdToegevoegd);

        await session.SaveChangesAsync();

        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
