namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Subtypes;

using Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_SubverengingVerfijndNaarNietBepaald(
    PubliekZoekenScenarioFixture<SubverenigingWerdTerugGezetNaarNietBepaaldScenario> fixture)
    : PubliekZoekenScenarioClassFixture<SubverenigingWerdTerugGezetNaarNietBepaaldScenario>
{
    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeNull();
    }
}
