namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Stopzetting.Vzer;

using Scenario.Stopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGestopt(
    DuplicateDetectionScenarioFixture<VerenigingWerdGestoptScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingWerdGestoptScenario>
{
    [Fact]
    public void Document_IsGestopt_Is_True()
    {
        fixture.Result.IsGestopt.Should().BeTrue();
    }
}
