namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Verwijdering.Vzer;

using Scenario.Verwijdering;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdVerwijderd(
    DuplicateDetectionScenarioFixture<VerenigingWerdVerwijderdScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingWerdVerwijderdScenario>
{

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.IsVerwijderd.Should().BeTrue();
    }
}
