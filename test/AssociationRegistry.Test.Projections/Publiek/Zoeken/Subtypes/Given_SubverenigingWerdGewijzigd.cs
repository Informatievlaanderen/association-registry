namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Subtypes;

using Public.Schema.Search;
using Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_SubverenigingWerdGewijzigd(
    PubliekZoekenScenarioFixture<SubverenigingWerdGewijzigdScenario> fixture)
    : PubliekZoekenScenarioClassFixture<SubverenigingWerdGewijzigdScenario>
{
    [Fact]
    public void SubverenigingVan_Is_Changed()
    {
        fixture.Result.SubverenigingVan.Should().BeEquivalentTo(new VerenigingZoekDocument.Types.SubverenigingVan()
        {
            AndereVereniging = fixture.Scenario.SubverenigingRelatieWerdGewijzigd.AndereVereniging,
            Identificatie = fixture.Scenario.SubverenigingDetailsWerdenGewijzigd.Identificatie,
            Beschrijving = fixture.Scenario.SubverenigingDetailsWerdenGewijzigd.Beschrijving,
        });
    }
}
