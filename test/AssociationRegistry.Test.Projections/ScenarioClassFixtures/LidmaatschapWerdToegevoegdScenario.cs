namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework;
using Framework.Fixtures;

public class LidmaatschapWerdToegevoegdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; set; }

    public LidmaatschapWerdToegevoegdScenario(ProjectionContext context) : base(context)
    {
        var fixture = new Fixture().CustomizeDomain();
        VerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        LidmaatschapWerdToegevoegd = fixture.Create<LidmaatschapWerdToegevoegd>();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(VerenigingWerdGeregistreerd.VCode,
                              VerenigingWerdGeregistreerd);

        await session.SaveChangesAsync();
        await using var session2 = await Context.DocumentSession();

        session2.Events.Append(VerenigingWerdGeregistreerd.VCode,
                               LidmaatschapWerdToegevoegd);

        await session2.SaveChangesAsync();
    }
}
