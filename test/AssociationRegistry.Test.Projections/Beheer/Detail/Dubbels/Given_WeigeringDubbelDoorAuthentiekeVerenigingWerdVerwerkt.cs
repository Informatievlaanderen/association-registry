namespace AssociationRegistry.Test.Projections.Beheer.Detail.Dubbels;

using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(BeheerDetailScenarioFixture<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario> fixture)
    : BeheerDetailScenarioClassFixture<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_IsDubbelVan_Is_Updated()
        => fixture.Result.IsDubbelVan.Should().BeEmpty();

    [Fact]
    public void Document_Status_Is_Dubbel()
        => fixture.Result.Status.Should().Be(VerenigingStatus.Actief.StatusNaam);
}
