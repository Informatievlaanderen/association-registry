namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport.ScenarioClassFixtures;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;
using AutoFixture;

public class MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario : ProjectionScenarioFixture<PowerBiExportContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd[] VerenigingenwerdenGeregistreerd { get; }

    public MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario(PowerBiExportContext context): base(context)
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

        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
