namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Lidmaatschap;

using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd(
    PubliekZoekenScenarioFixture<LidmaatschapWerdVerwijderdScenario> fixture)
    : PubliekZoekenScenarioClassFixture<LidmaatschapWerdVerwijderdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
       fixture.Result.Lidmaatschappen.Should().NotContain(x => x.LidmaatschapId == fixture.Scenario.LidmaatschapWerdVerwijderd.Lidmaatschap.LidmaatschapId);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Lidmaatschappen.Select(x => x.LidmaatschapId).Should().BeInAscendingOrder();
    }
}
