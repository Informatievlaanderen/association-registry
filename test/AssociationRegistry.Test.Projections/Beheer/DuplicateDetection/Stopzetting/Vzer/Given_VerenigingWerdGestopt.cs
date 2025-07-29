namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Stopzetting.Vzer;

using AssociationRegistry.Test.Projections.Scenario.Stopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGestopt(
    DuplicateDetectionScenarioFixture<VerenigingWerdGestoptScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingWerdGestoptScenario>
{

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.IsGestopt.Should().BeTrue();
    }
}
