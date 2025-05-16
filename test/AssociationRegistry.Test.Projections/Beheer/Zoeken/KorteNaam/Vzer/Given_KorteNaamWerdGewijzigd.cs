namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.KorteNaam.Vzer;

using AssociationRegistry.Test.Projections.Scenario.KorteNaamWerdGewijzigd.Vzer;
using Detail;

[Collection(nameof(ProjectionContext))]
public class Given_KorteNaamWerdGewijzigd(
    BeheerZoekenScenarioFixture<KorteNaamWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<KorteNaamWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.KorteNaam.Should()
               .BeEquivalentTo(fixture.Scenario.KorteNaamWerdGewijzigd.KorteNaam);
    }
}
