namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.NaamWerdGewijzigd.Kbo;

using AssociationRegistry.Test.Projections.Scenario.NaamWerdGewijzigd.Kbo;
using Detail;

[Collection(nameof(ProjectionContext))]
public class Given_NaamWerdGewijzigdInKbo(
    BeheerZoekenScenarioFixture<NaamWerdGewijzigdInKboScenario> fixture)
    : BeheerZoekenScenarioClassFixture<NaamWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Naam.Should()
               .BeEquivalentTo(fixture.Scenario.NaamWerdGewijzigdInKbo.Naam);
    }
}
