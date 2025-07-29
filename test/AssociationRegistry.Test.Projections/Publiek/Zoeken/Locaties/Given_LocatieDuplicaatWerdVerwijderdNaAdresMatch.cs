namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Locaties;

using AssociationRegistry.Test.Projections.Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch(
    PubliekZoekenScenarioFixture<LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario> fixture)
    : PubliekZoekenScenarioClassFixture<LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario>
{
    [Fact]
    public void Locatie_Is_Verwijderd()
    {
        var actual = fixture.Result.Locaties.SingleOrDefault(x => x.LocatieId == fixture.Scenario.LocatieDuplicaatWerdVerwijderdNaAdresMatch.VerwijderdeLocatieId);
        actual.Should().BeNull();
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
