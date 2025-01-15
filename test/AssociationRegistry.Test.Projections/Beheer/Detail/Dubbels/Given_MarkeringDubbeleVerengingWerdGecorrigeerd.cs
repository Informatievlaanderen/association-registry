namespace AssociationRegistry.Test.Projections.Beheer.Detail.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_MarkeringDubbeleVerengingWerdGecorrigeerd(BeheerDetailScenarioFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario> fixture)
    : BeheerDetailScenarioClassFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_IsDubbelVan_Is_Updated()
        => fixture.Result.IsDubbelVan.Should().Be(String.Empty);

    [Fact]
    public void Document_Status_Is_VorigeStatus()
        => fixture.Result.Status.Should().Be(fixture.Scenario.MarkeringDubbeleVerengingWerdGecorrigeerd.VorigeStatus);
}
