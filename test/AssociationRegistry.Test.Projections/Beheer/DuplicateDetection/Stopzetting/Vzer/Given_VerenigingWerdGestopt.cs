namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Stopzetting.Vzer;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Projections.Scenario.Stopzetting;
using Detail;

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
