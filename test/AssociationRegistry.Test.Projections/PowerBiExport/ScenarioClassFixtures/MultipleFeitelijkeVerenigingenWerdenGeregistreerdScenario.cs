namespace AssociationRegistry.Test.Projections.PowerBiExport.ScenarioClassFixtures;

using AutoFixture;
using Events;
using Framework.Fixtures;

public class MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario : ProjectionScenarioFixture<PowerBiExportContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd[] VerenigingenwerdenGeregistreerd { get; }

    public MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario(PowerBiExportContext context) : base(context)
    {
        VerenigingenwerdenGeregistreerd = AutoFixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd>()
                                                     .ToArray();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        foreach (var feitelijkeVerenigingWerdGeregistreerd in VerenigingenwerdenGeregistreerd)
        {
            session.Events.Append(feitelijkeVerenigingWerdGeregistreerd.VCode, feitelijkeVerenigingWerdGeregistreerd);
        }

        await session.SaveChangesAsync();
    }
}
