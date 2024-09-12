namespace AssociationRegistry.Test.Admin.Api.Projections.PowerBiExport.ScenarioClassFixtures;

using AutoFixture;
using Events;

public class FeitelijkeVerenigingWerdGeregistreerdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }

    public FeitelijkeVerenigingWerdGeregistreerdScenario(ProjectionContext context): base(context)
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(VerenigingWerdGeregistreerd.VCode, VerenigingWerdGeregistreerd);
        await session.SaveChangesAsync();

        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
