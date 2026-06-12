namespace AssociationRegistry.Test.Projections.Beheer.Detail.StartDatum.Vzer;

using Formats;
using Scenario.Startdatum.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_StartdatumWerdGewijzigd(
    BeheerDetailScenarioFixture<StartdatumWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<StartdatumWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Has_Startdatum_Gewijzigd()
    {
        fixture.Result.Startdatum.Should()
               .BeEquivalentTo(fixture.Scenario.StartdatumWerdGewijzigd.Startdatum.FormatAsBelgianDate());
    }
}
