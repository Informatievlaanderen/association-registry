namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.KorteNaam.Kbo;

using Scenario.KorteNaamWerdGewijzigd.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KorteNaamWerdGewijzigdInKbo(
    BeheerZoekenScenarioFixture<KorteNaamWerdGewijzigdInKboScenario> fixture)
    : BeheerZoekenScenarioClassFixture<KorteNaamWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Document_Korte_Naam_Werd_Gewijzigd()
    {
        fixture.Result.KorteNaam.Should()
               .BeEquivalentTo(fixture.Scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam);
    }
}
