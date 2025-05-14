namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.NaamWerdGewijzigd.Vzer;

using AssociationRegistry.Test.Projections.Scenario.NaamWerdGewijzigd.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_NaamWerdGewijzigd(
    BeheerZoekenScenarioFixture<NaamWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<NaamWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Naam.Should()
               .BeEquivalentTo(fixture.Scenario.NaamWerdGewijzigd.Naam);
    }
}
