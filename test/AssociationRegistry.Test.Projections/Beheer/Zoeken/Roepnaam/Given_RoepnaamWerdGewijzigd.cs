namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Roepnaam;

using Scenario.Roepnaam;

[Collection(nameof(ProjectionContext))]
public class Given_RoepnaamWerdGewijzigd(
    BeheerZoekenScenarioFixture<RoepnaamWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<RoepnaamWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Roepnaam()
    {
        fixture.Result.Roepnaam.Should()
               .BeEquivalentTo(fixture.Scenario.RoepnaamWerdGewijzigd.Roepnaam);
    }
}
