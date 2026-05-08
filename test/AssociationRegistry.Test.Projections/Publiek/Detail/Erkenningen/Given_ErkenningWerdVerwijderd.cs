namespace AssociationRegistry.Test.Projections.Publiek.Detail.Erkenningen;

using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerwijderd(PubliekDetailScenarioFixture<ErkenningWerdVerwijderdScenario> fixture)
    : PubliekDetailScenarioClassFixture<ErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Document_Is_Updated() => fixture.Result.Erkenningen.Should().BeEmpty();
}
