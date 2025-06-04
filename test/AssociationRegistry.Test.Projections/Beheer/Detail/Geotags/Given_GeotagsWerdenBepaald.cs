namespace AssociationRegistry.Test.Projections.Beheer.Detail.Geotags;

using PowerBiExport;

[Collection(nameof(ProjectionContext))]
public class Given_GeotagsWerdenBepaald(
    BeheerDetailScenarioFixture<GeotagsWerdenBepaaldScenario> fixture)
    : BeheerDetailScenarioClassFixture<GeotagsWerdenBepaaldScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);
}
