namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.KorteNaam.Kbo;

using AssociationRegistry.Test.Projections.Scenario.KorteNaamWerdGewijzigd.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KorteNaamWerdGewijzigdInKbo(
    BeheerZoekenScenarioFixture<KorteNaamWerdGewijzigdInKboScenario> fixture)
    : BeheerZoekenScenarioClassFixture<KorteNaamWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.KorteNaam.Should()
               .BeEquivalentTo(fixture.Scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam);
    }
}
