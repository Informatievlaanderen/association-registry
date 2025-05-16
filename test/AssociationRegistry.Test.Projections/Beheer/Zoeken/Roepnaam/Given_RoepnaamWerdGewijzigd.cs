namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Roepnaam;

using AssociationRegistry.Test.Projections.Scenario.Roepnaam;
using Detail;

[Collection(nameof(ProjectionContext))]
public class Given_RoepnaamWerdGewijzigd(
    BeheerZoekenScenarioFixture<RoepnaamWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<RoepnaamWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Roepnaam.Should()
               .BeEquivalentTo(fixture.Scenario.RoepnaamWerdGewijzigd.Roepnaam);
    }
}
