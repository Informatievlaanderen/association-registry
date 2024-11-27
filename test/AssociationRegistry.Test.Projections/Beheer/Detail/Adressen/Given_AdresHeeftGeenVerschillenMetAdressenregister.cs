namespace AssociationRegistry.Test.Projections.Beheer.Detail.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister(
    BeheerDetailScenarioFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario> fixture)
    : BeheerDetailScenarioClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);
}
