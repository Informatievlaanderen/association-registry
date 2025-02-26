namespace AssociationRegistry.Test.Projections.Beheer.Detail.Einddatum;

using Formats;
using Scenario.Einddatum;

[Collection(nameof(ProjectionContext))]
public class Given_EinddatumWerdGewijzigd(
    BeheerDetailScenarioFixture<EinddatumWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<EinddatumWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Einddatum.Should().BeEquivalentTo(fixture.Scenario.EinddatumWerdGewijzigd.Einddatum.FormatAsBelgianDate());
    }
}
