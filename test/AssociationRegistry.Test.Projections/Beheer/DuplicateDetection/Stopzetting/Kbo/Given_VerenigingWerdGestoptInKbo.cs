namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Stopzetting.Kbo;

using Scenario.Stopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGestoptInKbo(
    DuplicateDetectionScenarioFixture<VerenigingWerdGestoptInKBOScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingWerdGestoptInKBOScenario>
{
    [Fact]
    public void Document_IsGestopt_Is_True()
    {
        fixture.Result.IsGestopt.Should().BeTrue();
    }
}
