namespace AssociationRegistry.Test.Projections.Beheer.Detail.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdInStopzettingGeplaatst(
    BeheerDetailScenarioFixture<VerenigingWerdInStopzettingGeplaatstScenario> fixture
) : BeheerDetailScenarioClassFixture<VerenigingWerdInStopzettingGeplaatstScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.InStopzetting.Should().BeTrue();
    }
}
