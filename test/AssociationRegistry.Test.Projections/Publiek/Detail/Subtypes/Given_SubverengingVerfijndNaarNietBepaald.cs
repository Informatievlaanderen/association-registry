namespace AssociationRegistry.Test.Projections.Publiek.Detail.Subtypes;

using AssociationRegistry.Test.Projections.Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_SubverengingVerfijndNaarNietBepaald(
    PubliekDetailScenarioFixture<SubverenigingWerdVerfijndNaarNietBepaaldScenario> fixture)
    : PubliekDetailScenarioClassFixture<SubverenigingWerdVerfijndNaarNietBepaaldScenario>
{
    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeNull();
    }
}
