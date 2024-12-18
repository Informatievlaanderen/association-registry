namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Marten;

[Collection(nameof(ProjectionContext))]
public class Given_MultipleFeitelijkeVerenigingenWerdenGeregistreerd(PowerBiScenarioFixture<MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario> fixture)
    : PowerBiScenarioClassFixture<MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario>
{
    [Fact]
    public async Task ARecordIsStoredForEachVCode()
    {
        await using var documentSession = fixture.Context.AdminStore.LightweightSession();

        //TODO:
        foreach (var feitelijkeVerenigingWerdGeregistreerd in fixture.Scenario.VerenigingenwerdenGeregistreerd)
        {
            var powerBiExportDocument =
                await documentSession.Query<PowerBiExportDocument>()
                                     .SingleAsync(x => x.VCode == feitelijkeVerenigingWerdGeregistreerd.VCode);

            powerBiExportDocument.Should().NotBeNull();
        }
    }
}
