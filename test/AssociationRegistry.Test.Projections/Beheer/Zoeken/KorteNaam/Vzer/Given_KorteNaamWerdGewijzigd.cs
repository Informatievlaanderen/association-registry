namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.KorteNaam.Vzer;

using Scenario.KorteNaamWerdGewijzigd.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KorteNaamWerdGewijzigd(
    BeheerZoekenScenarioFixture<KorteNaamWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<KorteNaamWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Korte_Naam_Werd_Gewijzigd()
    {
        fixture.Result.KorteNaam.Should()
               .BeEquivalentTo(fixture.Scenario.KorteNaamWerdGewijzigd.KorteNaam);
    }
}
