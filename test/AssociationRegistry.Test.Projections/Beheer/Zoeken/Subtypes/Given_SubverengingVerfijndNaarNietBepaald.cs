namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Subtypes;

using AssociationRegistry.Test.Projections.Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_SubverengingVerfijndNaarNietBepaald(
    BeheerZoekenScenarioFixture<SubverenigingWerdVerfijndNaarNietBepaaldScenario> fixture)
    : BeheerZoekenScenarioClassFixture<SubverenigingWerdVerfijndNaarNietBepaaldScenario>
{
    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeNull();
    }
}
