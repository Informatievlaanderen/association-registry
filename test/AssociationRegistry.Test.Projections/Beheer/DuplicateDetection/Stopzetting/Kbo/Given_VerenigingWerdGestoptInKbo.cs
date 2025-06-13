namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Stopzetting.Kbo;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Projections.Scenario.Stopzetting;
using Detail;

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
