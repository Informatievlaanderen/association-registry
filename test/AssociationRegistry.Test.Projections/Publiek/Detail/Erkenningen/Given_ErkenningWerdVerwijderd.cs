namespace AssociationRegistry.Test.Projections.Publiek.Detail.Erkenningen;

using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerwijderd(PubliekDetailScenarioFixture<ErkenningWerdVerwijderdScenario> fixture)
    : PubliekDetailScenarioClassFixture<ErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture
            .Result.Erkenningen.Should()
            .NotContain(x => x.ErkenningId == fixture.Scenario.ErkenningWerdGeregistreerdToBeRemoved.ErkenningId);

        fixture
            .Result.Erkenningen.Should()
            .ContainSingle(x => x.ErkenningId == fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId);
    }
}
