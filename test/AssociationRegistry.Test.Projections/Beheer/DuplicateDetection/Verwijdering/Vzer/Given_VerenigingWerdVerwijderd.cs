namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Verwijdering.Vzer;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Projections.Beheer.Detail;
using AssociationRegistry.Test.Projections.Scenario.Stopzetting;
using PowerBiExport;
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
