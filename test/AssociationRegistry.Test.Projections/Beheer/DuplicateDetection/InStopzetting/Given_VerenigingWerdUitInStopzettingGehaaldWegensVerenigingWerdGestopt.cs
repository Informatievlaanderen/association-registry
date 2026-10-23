namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario(
    DuplicateDetectionScenarioFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario> fixture
) : DuplicateDetectionClassFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
