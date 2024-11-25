namespace AssociationRegistry.Test.Projections.PowerBiExport.ScenarioClassFixtures;

using AutoFixture;
using Events;
using Framework.Fixtures;

public class FeitelijkeVerenigingWerdGeregistreerdScenario : ProjectionScenarioFixture<PowerBiExportContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }

    public FeitelijkeVerenigingWerdGeregistreerdScenario(PowerBiExportContext context) : base(context)
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(VerenigingWerdGeregistreerd.VCode, VerenigingWerdGeregistreerd);
        await session.SaveChangesAsync();
    }
}
