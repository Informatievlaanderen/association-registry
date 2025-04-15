namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Adressen;

using AssociationRegistry.Test.Projections.Beheer.Historiek;
using AssociationRegistry.Test.Projections.Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister(
    BeheerHistoriekScenarioFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(1);
}
