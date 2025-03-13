namespace AssociationRegistry.Test.Projections.Publiek.Detail.Subtypes;

using AssociationRegistry.Test.Projections.Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_SubverengingVerfijndNaarFeitelijkeVereniging(
    PubliekDetailScenarioFixture<SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario> fixture)
    : PubliekDetailScenarioClassFixture<SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario>
{
    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeNull();
    }
}
