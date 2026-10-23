namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitStopzettingGeplaatst(
    DuplicateDetectionScenarioFixture<VerenigingWerdUitStopzettingGehaaldScenario> fixture
) : DuplicateDetectionClassFixture<VerenigingWerdUitStopzettingGehaaldScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
