namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel(
    DuplicateDetectionScenarioFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario> fixture
) : DuplicateDetectionClassFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
