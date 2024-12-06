namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.ProjectionHost.Projections.PowerBiExport;
using Publiek.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdVerwijderd(PowerBiScenarioFixture<FeitelijkeVerenigingWerdVerwijderdScenario> fixture)
    : PowerBiScenarioClassFixture<FeitelijkeVerenigingWerdVerwijderdScenario>
{
    [Fact]
    public void ARecordIsStored_For_Vereniging1_With_StatusVerwijderd()
    {
        fixture.Result.Should().NotBeNull();
        fixture.Result.Status.Should().Be(PowerBiExportProjection.StatusVerwijderd);
    }

    [Fact]
    public void ARecordIsStored_For_Vereniging2()
    {
        fixture.Result.Should().NotBeNull();
    }
}
