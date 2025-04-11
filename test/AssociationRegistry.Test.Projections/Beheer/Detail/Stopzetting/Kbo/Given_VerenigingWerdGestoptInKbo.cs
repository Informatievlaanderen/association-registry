namespace AssociationRegistry.Test.Projections.Beheer.Detail.Stopzetting.Kbo;

using Admin.Schema.Constants;
using Formats;
using Scenario.Stopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGestoptInKbo(
    BeheerDetailScenarioFixture<VerenigingWerdGestoptInKBOScenario> fixture)
    : BeheerDetailScenarioClassFixture<VerenigingWerdGestoptInKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Einddatum.Should().BeEquivalentTo(fixture.Scenario.VerenigingWerdGestoptInKBO.Einddatum.FormatAsBelgianDate());
        fixture.Result.Status.Should().BeEquivalentTo(VerenigingStatus.Gestopt);
    }
}
