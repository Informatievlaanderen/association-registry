namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Locaties;

using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdVerwijderd(
    DuplicateDetectionScenarioFixture<LocatieWerdVerwijderdScenario> fixture)
    : DuplicateDetectionClassFixture<LocatieWerdVerwijderdScenario>
{
    [Fact]
    public void Document_Locatie_Is_Removed()
    {
        var actual =
            fixture.Result.Locaties.SingleOrDefault(x => x.LocatieId ==
                                                         fixture.Scenario.LocatieWerdVerwijderd.Locatie.LocatieId);

        actual.Should().BeNull();
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
