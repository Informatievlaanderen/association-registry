namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Einddatum;

using Formats;
using Scenario.Einddatum;

[Collection(nameof(ProjectionContext))]
public class Given_EinddatumWerdGewijzigd(
    BeheerZoekenScenarioFixture<EinddatumWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<EinddatumWerdGewijzigdScenario>
{
   [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Einddatum.Should().BeEquivalentTo(fixture.Scenario.EinddatumWerdGewijzigd.Einddatum.FormatAsBelgianDate());
    }
}
