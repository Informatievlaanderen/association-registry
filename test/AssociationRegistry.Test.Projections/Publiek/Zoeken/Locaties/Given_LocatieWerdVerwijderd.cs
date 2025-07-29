namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Locaties;

using AssociationRegistry.Test.Projections.Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdVerwijderd(
    PubliekZoekenScenarioFixture<LocatieWerdVerwijderdScenario> fixture)
    : PubliekZoekenScenarioClassFixture<LocatieWerdVerwijderdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var actual = fixture.Result.Locaties.SingleOrDefault(x => x.LocatieId == fixture.Scenario.LocatieWerdVerwijderd.Locatie.LocatieId);

        actual.Should().BeNull();
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
