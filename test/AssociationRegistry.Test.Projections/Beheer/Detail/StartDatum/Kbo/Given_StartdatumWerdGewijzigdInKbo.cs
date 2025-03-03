namespace AssociationRegistry.Test.Projections.Beheer.Detail.StartDatum.Kbo;

using Formats;
using Scenario.Startdatum.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_StartdatumWerdGewijzigdInKbo(
    BeheerDetailScenarioFixture<StartdatumWerdGewijzigdInKboScenario> fixture)
    : BeheerDetailScenarioClassFixture<StartdatumWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Startdatum.Should()
               .BeEquivalentTo(fixture.Scenario.StartdatumWerdGewijzigdInKbo.Startdatum.FormatAsBelgianDate());
    }
}
