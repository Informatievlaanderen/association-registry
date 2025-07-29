namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Stopzetting.Kbo;

using AssociationRegistry.Test.Projections.Scenario.Stopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGestoptInKbo(
    DuplicateDetectionScenarioFixture<VerenigingWerdGestoptInKBOScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingWerdGestoptInKBOScenario>
{

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.IsGestopt.Should().BeTrue();
    }
}
