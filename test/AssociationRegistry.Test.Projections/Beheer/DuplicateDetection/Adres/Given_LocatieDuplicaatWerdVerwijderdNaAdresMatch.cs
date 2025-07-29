namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Adres;

using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch(DuplicateDetectionScenarioFixture<LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario> fixture)
    : DuplicateDetectionClassFixture<LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario>
{
    [Fact]
    public void Adres_Is_Removed()
    {
        fixture.Result.Locaties.Should()
               .NotContain(x => x.LocatieId == fixture.Scenario.LocatieDuplicaatWerdVerwijderdNaAdresMatch.VerwijderdeLocatieId);
        fixture.Result.Locaties.Should()
               .Contain(x => x.LocatieId == fixture.Scenario.LocatieDuplicaatWerdVerwijderdNaAdresMatch.BehoudenLocatieId);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
