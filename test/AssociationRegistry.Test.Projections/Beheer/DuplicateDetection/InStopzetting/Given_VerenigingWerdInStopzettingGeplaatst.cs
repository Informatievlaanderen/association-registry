namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdInStopzettingGeplaatst(
    DuplicateDetectionScenarioFixture<VerenigingWerdInStopzettingGeplaatstScenario> fixture
) : DuplicateDetectionClassFixture<VerenigingWerdInStopzettingGeplaatstScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.InStopzetting.Should().BeTrue();
    }
}
