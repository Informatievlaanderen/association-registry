namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport.ScenarioClassFixtures;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;
using AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdScenario : ProjectionScenarioFixture<PowerBiExportContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }

    public FeitelijkeVerenigingWerdGeregistreerdScenario(PowerBiExportContext context): base(context)
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
