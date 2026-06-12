namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.NaamWerdGewijzigd.Vzer;

using Scenario.NaamWerdGewijzigd.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_NaamWerdGewijzigd(
    BeheerZoekenScenarioFixture<NaamWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<NaamWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Naam_Werd_Gewijzigd()
    {
        fixture.Result.Naam.Should()
               .BeEquivalentTo(fixture.Scenario.NaamWerdGewijzigd.Naam);
    }
}
